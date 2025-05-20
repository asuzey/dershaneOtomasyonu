import os
import sys
import time
import pyautogui
import pygetwindow as gw
import keyboard
import psutil
from tkinter import messagebox, Tk
from datetime import datetime
import pyodbc
import threading

# -------------------- PARAMETRE KONTROLÜ --------------------
if len(sys.argv) < 4:
    print("Kullaným: kopyaList.exe <ogrenciAdi> <sinavId> <ogretmenAdi>")
    sys.exit(1)

kullanici_adi = sys.argv[1]
sinav_id = int(sys.argv[2])
ogretmen_adi = sys.argv[3]

# -------------------- VERÝTABANI --------------------
def baglanti_olustur():
    return pyodbc.connect(
        "Driver={ODBC Driver 17 for SQL Server};"
        "Server=localhost\\SQLEXPRESS;"
        "Database=DERSHANE;"
        "Trusted_Connection=yes;"
    )

def kullanici_id_getir(kullanici_adi):
    conn = baglanti_olustur()
    cursor = conn.cursor()
    cursor.execute("SELECT Id FROM Kullanicilar WHERE KullaniciAdi = ?", kullanici_adi)
    sonuc = cursor.fetchone()
    conn.close()
    return sonuc[0] if sonuc else None

def sinav_ogretmen_id_getir(sinav_id):
    conn = baglanti_olustur()
    cursor = conn.cursor()
    cursor.execute("SELECT OlusturucuId FROM Sinavlar WHERE Id = ?", sinav_id)
    sonuc = cursor.fetchone()
    conn.close()
    return sonuc[0] if sonuc else None

def ogretmen_adi_kontrol_et(sinav_id, ogretmen_adi):
    conn = baglanti_olustur()
    cursor = conn.cursor()
    cursor.execute("""
        SELECT k.KullaniciAdi 
        FROM Sinavlar s
        JOIN Kullanicilar k ON s.OlusturucuId = k.Id
        WHERE s.Id = ?
    """, sinav_id)
    sonuc = cursor.fetchone()
    conn.close()
    return sonuc and sonuc[0].lower() == ogretmen_adi.lower()

def dosya_kaydet(file_name, file_path, kullanici_id):
    conn = baglanti_olustur()
    cursor = conn.cursor()
    cursor.execute("""
        INSERT INTO Dosyalar (FileName, FilePath, OlusturucuId)
        VALUES (?, ?, ?)
    """, (file_name, file_path, kullanici_id))
    conn.commit()
    cursor.execute("SELECT SCOPE_IDENTITY()")
    dosya_id = cursor.fetchone()[0]
    conn.close()
    return dosya_id

def kopya_kaydi_ekle(sinav_id, kullanici_id, dosya_id):
    conn = baglanti_olustur()
    cursor = conn.cursor()
    cursor.execute("""
        INSERT INTO Kopyalar (SinavId, KullaniciId, DosyaId, Tarih)
        VALUES (?, ?, ?, ?)
    """, (sinav_id, kullanici_id, dosya_id, datetime.now()))
    conn.commit()
    conn.close()

def log_ekle(level, message, kullanici_id=None, ip_address=None):
    conn = baglanti_olustur()
    cursor = conn.cursor()
    cursor.execute("""
        INSERT INTO Logs (Timestamp, Level, Message, IpAddress, KullaniciId)
        VALUES (?, ?, ?, ?, ?)
    """, (datetime.now(), level, message, ip_address, kullanici_id))
    conn.commit()
    conn.close()

def admin_kullanicilari_getir():
    conn = baglanti_olustur()
    cursor = conn.cursor()
    cursor.execute("""
        SELECT k.Id, k.KullaniciAdi 
        FROM Kullanicilar k
        JOIN Roller r ON k.RoleId = r.Id
        WHERE r.RolAdi = 'Admin'
    """)
    adminler = cursor.fetchall()
    conn.close()
    return adminler  # Liste [(Id, KullaniciAdi), ...]

def kopya_kaydet(sinav_id, kullanici_id, dosya_id):
    ogretmen_id = sinav_ogretmen_id_getir(sinav_id)
    # Kopya kaydýný ekle
    kopya_kaydi_ekle(sinav_id, kullanici_id, dosya_id)

    # Öðretmene log bildirimi
    log_ekle(
        level="ERROR",
        message=f"Kopya tespiti: SinavId={sinav_id}, KullaniciId={kullanici_id}, DosyaId={dosya_id} (Öðretmen bildirimi)",
        kullanici_id=ogretmen_id
    )

    # Adminlere log bildirimi
    adminler = admin_kullanicilari_getir()
    for admin_id, _ in adminler:
        log_ekle(
            level="ERROR",
            message=f"Kopya tespiti: SinavId={sinav_id}, KullaniciId={kullanici_id}, DosyaId={dosya_id} (Admin bildirimi)",
            kullanici_id=admin_id
        )

    print("[KOPYA] Veri tabanýna kayýt tamamlandý.")

# -------------------- SINAV GEÇERLÝLÝK KONTROLÜ --------------------
if not ogretmen_adi_kontrol_et(sinav_id, ogretmen_adi):
    print("[HATA] Bu sýnav size ait deðil. Kopya izleme baþlatýlamaz.")
    sys.exit(1)

# -------------------- SINAV SÜRESÝ --------------------
try:
    conn = baglanti_olustur()
    cursor = conn.cursor()
    cursor.execute("SELECT COUNT(*) FROM Sorular WHERE SinavId = ?", sinav_id)
    soru_sayisi = cursor.fetchone()[0]
    conn.close()

    if soru_sayisi == 0:
        print("[HATA] Bu sýnava ait hiç soru yok!")
        sys.exit(1)

    sinav_suresi_dk = soru_sayisi * 2
    print(f"[INFO] Sýnav süresi: {sinav_suresi_dk} dakika")

except Exception as e:
    print(f"[HATA] Süre hesaplanamadý: {e}")
    sinav_suresi_dk = 30  # Varsayýlan

bitis = time.time() + sinav_suresi_dk * 60
#klasor = f"C:/Users/Public/FileServer/Kopyalar/{sinav_id}/{kullanici_adi}"
klasor = os.path.join("C:", "Users", "Public", "FileServer", "Kopyalar", str(sinav_id), kullanici_adi)
os.makedirs(klasor, exist_ok=True)
stop_flag = {"deger": False}

try:
    onceki_pencere = gw.getActiveWindowTitle()
except:
    onceki_pencere = ""

# -------------------- EKRAN GÖRÜNTÜSÜ --------------------
def ekran_goruntusu_al():
    while time.time() < bitis and not stop_flag["deger"]:
        an = datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
        dosya_adi = f"{an}_normal.png"
        tam_yol = os.path.join(klasor, dosya_adi)
        ekran = pyautogui.screenshot()
        ekran.save(tam_yol)
        print(f"[GÖRÜNTÜ] {dosya_adi}")
        time.sleep(10)

# -------------------- KOPYA KONTROL --------------------
def show_warning(sebep):
    root = Tk()
    root.withdraw()
    messagebox.showerror("Kopya Tespit Edildi", f"{sebep}\nBu nedenle sýnav sonlandýrýldý.")
    root.destroy()

def kopya_kontrol_et():
    yasakli_uygulamalar = {"chrome.exe", "msedge.exe", "firefox.exe"}
    global onceki_pencere
    baslangic_zamani = time.time()

    while time.time() < bitis and not stop_flag["deger"]:
        sebep = None
        try:
            aktif = gw.getActiveWindowTitle()
        except Exception as ex:
            aktif = ""
            print(f"[HATA] Aktif pencere alýnamadý: {ex}")

        if time.time() - baslangic_zamani > 10:
            if aktif and aktif != onceki_pencere:
                yeni_aktif = aktif
                if yeni_aktif != onceki_pencere:
                    sebep = f"Pencere deðiþtirildi: {yeni_aktif}"
                    onceki_pencere = yeni_aktif

        if keyboard.is_pressed("alt+tab") or keyboard.is_pressed("win+d") or keyboard.is_pressed("esc"):
            sebep = "Klavye kýsayolu kullanýldý (Alt+Tab / Win+D / Esc)!"

        for proc in psutil.process_iter(['name']):
            try:
                if proc.info['name'] and proc.info['name'].lower() in yasakli_uygulamalar:
                    sebep = f"Yasaklý uygulama çalýþýyor: {proc.info['name']}"
                    break
            except Exception:
                continue

        if sebep:
            print(f"[KOPYA TESPÝTÝ] {sebep}")
            ekran = pyautogui.screenshot()
            an = datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
            temiz_sebep = sebep.replace(":", "").replace(" ", "_")
            dosya_adi = f"{an}_KOPYA_{temiz_sebep}.png"
            txt_adi = dosya_adi.replace(".png", ".txt")
            tam_yol = os.path.join(klasor, dosya_adi)

            ekran.save(tam_yol)
            with open(os.path.join(klasor, txt_adi), "w", encoding="utf-8") as f:
                f.write(f"Tarih: {an}\nKullanýcý: {kullanici_adi}\nSebep: {sebep}\n")

            try:
                kullanici_id = kullanici_id_getir(kullanici_adi)
                dosya_id = dosya_kaydet(dosya_adi, tam_yol, kullanici_id)
                kopya_kaydet(sinav_id, kullanici_id, dosya_id)
                print("[SQL] Kopya verisi ve log kaydedildi.")
            except Exception as e:
                print(f"[SQL HATA] {e}")

            try:
                with open("esinav_pid.txt", "r") as f:
                    pid = int(f.read())
                    psutil.Process(pid).terminate()
                    print("[E-SINAV] Sýnav uygulamasý kapatýldý.")
            except Exception as e:
                print(f"[KAPATMA HATASI] {e}")

            stop_flag["deger"] = True
            threading.Thread(target=show_warning, args=(sebep,)).start()
            time.sleep(2)
            sys.exit(15)

        time.sleep(0.5)

# -------------------- BAÞLAT --------------------
threading.Thread(target=ekran_goruntusu_al).start()
kopya_kontrol_et()

