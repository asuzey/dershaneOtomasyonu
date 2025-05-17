import tkinter as tk
from tkinter import messagebox
import cv2
import os
import time
import sys
import pyodbc
from pyodbc import connect
from PIL import Image, ImageTk

# ------------------------ C# Tarafından Kullanıcı Alımı ------------------------
kullanici = sys.argv[1] if len(sys.argv) > 1 else "bilinmeyen"

# ------------------------ AYARLAR ------------------------
try:
    SQL_BAGLANTI = "Driver={ODBC Driver 17 for SQL Server};Server=localhost\\SQLEXPRESS;Database=DERSHANE;Trusted_Connection=yes;"
    print("Bağlantı başarılı")
except Exception as e:
    print("Hata:", e)

FONT = ("Century Gothic", 12)
KAYIT_YOLU = "C:/Users/Public/FileServer/YuzVerileri"
os.makedirs(KAYIT_YOLU, exist_ok=True)

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))  # düzeltildi
HAAR_PATH = os.path.join(SCRIPT_DIR, "haarcascade_frontalface_default.xml")

# ------------------------ MSSQL Fonksiyonları ------------------------
def yuz_kaydi_var_mi(kullaniciAdi):
    try:
        conn = pyodbc.connect(SQL_BAGLANTI)
        cursor = conn.cursor()
        cursor.execute("SELECT COUNT(*) FROM Dosyalar WHERE FileName LIKE ?", f"%{kullaniciAdi}%")
        result = cursor.fetchone()
        conn.close()
        return result[0] > 0
    except Exception as e:
        print("SQL hata:", e)
        return False

def dosya_kaydi_ekle(file_name, file_path, kullanici_id):
    try:
        conn = pyodbc.connect(SQL_BAGLANTI)
        cursor = conn.cursor()
        cursor.execute("INSERT INTO Dosyalar (FileName, FilePath, OlusturucuId) VALUES (?, ?, ?)",
                       file_name, file_path, kullanici_id)
        conn.commit()
        conn.close()
        print("Dosya kaydı başarıyla eklendi.")
    except Exception as e:
        print("Dosya ekleme hatası:", e)

def get_kullanici_bilgisi(kullaniciAdi):
    try:
        conn = pyodbc.connect(SQL_BAGLANTI)
        cursor = conn.cursor()
        cursor.execute("SELECT Adi, Soyadi FROM Kullanicilar WHERE KullaniciAdi = ?", kullaniciAdi)
        result = cursor.fetchone()
        conn.close()
        if result:
            return f"{result[0]} {result[1]}"
        else:
            return "Bilinmeyen Kullanıcı"
    except Exception as e:
        print("Kullanıcı bilgisi alınamadı:", e)
        return "Hata"

# ------------------------ Kamera ve Fotoğraf Fonksiyonu ------------------------
def kamera_baslat(kayit_klasoru, pencere):
    face_cascade = cv2.CascadeClassifier(HAAR_PATH)
    cap = cv2.VideoCapture(0)

    sayac = 0
    onceki_yuz = None
    hareket_var_mi = False
    hareket_baslangic = time.time()
    bekleme_baslangic = time.time()

    etiket_canlilik = tk.Label(pencere, text="Canlılık: Bekleniyor", font=FONT, fg="orange", bg="white")
    etiket_arkaplan = tk.Label(pencere, text="Arka Plan: Bekleniyor", font=FONT, fg="orange", bg="white")
    etiket_kayit = tk.Label(pencere, text="Fotoğraf Kaydı: Bekleniyor", font=FONT, fg="orange", bg="white")
    etiket_canlilik.pack()
    etiket_arkaplan.pack()
    etiket_kayit.pack()

    panel = tk.Label(pencere)
    panel.pack(pady=10)

    def guncelle():
        nonlocal sayac, onceki_yuz, hareket_var_mi, hareket_baslangic, bekleme_baslangic
        ret, frame = cap.read()
        if not ret:
            pencere.after(100, guncelle)
            return

        frame = cv2.flip(frame, 1)  # Kamera düz gösterilsin

        gri = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        yuzler = face_cascade.detectMultiScale(gri, 1.1, 5)

        if len(yuzler) == 1:
            x, y, w, h = yuzler[0]
            etiket_arkaplan.config(text="Arka Plan: Uygun", fg="green")

            if onceki_yuz:
                dx = abs(onceki_yuz[0] - x)
                dy = abs(onceki_yuz[1] - y)
                if dx > 5 or dy > 5:
                    hareket_var_mi = True
            onceki_yuz = (x, y)

            if hareket_var_mi and (time.time() - hareket_baslangic) > 2:
                etiket_canlilik.config(text="Canlılık: Onaylandı", fg="green")

                roi = frame[y:y+h, x:x+w]
                dosya_yolu = os.path.join(kayit_klasoru, f"yuz_{sayac+1}.jpg")
                cv2.imwrite(dosya_yolu, roi)

                sayac += 1
                etiket_kayit.config(text=f"Fotoğraf Kaydı: {sayac}/5 alındı", fg="green")

                hareket_var_mi = False
                hareket_baslangic = time.time()
                bekleme_baslangic = time.time()

                if sayac >= 5:
                    messagebox.showinfo("Bilgi", "Yüz verileriniz kaydediliyor, lütfen bekleyiniz...")
                    cap.release()

                    file_name = f"yuz_{sayac}.jpg"
                    file_path = os.path.join(kayit_klasoru, file_name)
                    kullanici_id = kullanici

                    dosya_kaydi_ekle(file_name, file_path, kullanici_id)
                    messagebox.showinfo("Başarılı", "Tüm yüz verileri kaydedildi.")
                    pencere.destroy()
                    return

        elif len(yuzler) > 1:
            etiket_arkaplan.config(text="Arka Plan: Birden fazla yüz algılandı!", fg="red")
        else:
            etiket_arkaplan.config(text="Arka Plan: Yüz algılanamadı", fg="orange")
            etiket_canlilik.config(text="Canlılık: Bekleniyor", fg="orange")
            etiket_kayit.config(text="Fotoğraf Kaydı: Bekleniyor", fg="orange")

        if time.time() - bekleme_baslangic > 10 and sayac < 5:
            etiket_kayit.config(text="İşleminiz devam ediyor, lütfen bekleyiniz...", fg="blue")

        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        img = cv2.resize(rgb_frame, (640, 480))
        img = cv2.cvtColor(img, cv2.COLOR_RGB2BGR)
        img = cv2.cvtColor(img, cv2.COLOR_BGR2RGBA)
        imgtk = ImageTk.PhotoImage(image=Image.fromarray(img))
        panel.imgtk = imgtk
        panel.config(image=imgtk)

        pencere.after(100, guncelle)

    guncelle()

# ------------------------ Ana Arayüz ------------------------
def ekran_ortala(pencere, genislik=500, yukseklik=300):
    ekran_gen = pencere.winfo_screenwidth()
    ekran_yuk = pencere.winfo_screenheight()
    x = int((ekran_gen / 2) - (genislik / 2))
    y = int((ekran_yuk / 2) - (yukseklik / 2))
    pencere.geometry(f"{genislik}x{yukseklik}+{x}+{y}")

root = tk.Tk()
root.title("Yüz Kayıt Sistemi")
root.configure(bg="white")
ekran_ortala(root)

kullanici_adi = get_kullanici_bilgisi(kullanici)
tk.Label(root, text=f"Hoş Geldin {kullanici_adi}", font=("Century Gothic", 16, "bold"), fg="black", bg="white").pack(pady=20)

onay_var = tk.IntVar()
tk.Checkbutton(
    root,
    text="Kişisel verilerime erişimi kabul ediyorum",
    variable=onay_var,
    bg="white",
    font=FONT,
    fg="black"
).pack(pady=10)

def devam_et():
    if onay_var.get() != 1:
        messagebox.showwarning("Uyarı", "Lütfen kutucuğu işaretleyin.")
        return

    if yuz_kaydi_var_mi(kullanici):
        messagebox.showinfo("Bilgi", "Zaten yüz veriniz kayıtlı.")
        return

    messagebox.showinfo("Kamera Erişimi", "Uygulama kameranızı kullanmak istiyor. Lütfen kameranızın bağlı olduğundan emin olun.")

    root.destroy()

    yeni_pencere = tk.Tk()
    yeni_pencere.title("Yüz Kaydı Alınıyor")
    yeni_pencere.configure(bg="white")
    ekran_ortala(yeni_pencere, 700, 650)

    dosya_yolu = os.path.join(KAYIT_YOLU, kullanici)
    os.makedirs(dosya_yolu, exist_ok=True)

    kamera_baslat(dosya_yolu, yeni_pencere)

    yeni_pencere.mainloop()

tk.Button(root, text="Devam Et", command=devam_et, font=FONT).pack(pady=20)
root.mainloop()
