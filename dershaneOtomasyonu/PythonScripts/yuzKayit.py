import cv2
import os
import pyodbc
import tkinter as tk
from tkinter import messagebox
import sys
import os
import cv2

cascade_path = os.path.join(os.path.dirname(__file__), 'haarcascade_frontalface_default.xml')
face_cascade = cv2.CascadeClassifier(cascade_path)

if face_cascade.empty():
    print("Cascade dosyası yüklenemedi. Dosya yolu:", cascade_path)
    exit(1)


# MSSQL bağlantısı
def baglanti_olustur():
    return pyodbc.connect(
        'DRIVER={SQL Server};SERVER=localhost;DATABASE=DERSHANE;UID=sa;PWD=124'
    )

# MSSQL'e kayıt logu ekle (KullaniciID ile)
def log_kaydet(kullanici_id):
    conn = baglanti_olustur()
    cursor = conn.cursor()
    cursor.execute("INSERT INTO YuzKayitLog (KullaniciID, YuzKaydiAlindi) VALUES (?, 1)", (kullanici_id,))
    conn.commit()
    conn.close()

# Yüz kayıt işlemi (ID bazlı klasörleme)
def yuz_kayit_sistemi(kullanici_id):
    hedef_klasor = f"C:/Users/Public/FileServer/YuzTanima/{kullanici_id}"
    os.makedirs(hedef_klasor, exist_ok=True)

    kamera = cv2.VideoCapture(0)
    cascade_path = os.path.join(os.path.dirname(__file__), 'haarcascade_frontalface_default.xml')
    face_cascade = cv2.CascadeClassifier(cascade_path)

    if face_cascade.empty():
        print("Cascade dosyası yüklenemedi. Dosya yolu:", cascade_path)
        exit(1)
    kayit_sayisi = 0

    while True:
        ret, kare = kamera.read()
        gri = cv2.cvtColor(kare, cv2.COLOR_BGR2GRAY)
        yuzler = face_cascade.detectMultiScale(gri, 1.3, 5)

        if len(yuzler) == 0:
            cv2.putText(kare, "Yüz algılanamadı.", (30, 30), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (0, 0, 255), 2)
        elif len(yuzler) > 1:
            cv2.putText(kare, "Birden fazla yüz algılandı!", (30, 30), cv2.FONT_HERSHEY_SIMPLEX, 0.7, (0, 0, 255), 2)
        else:
            for (x, y, w, h) in yuzler:
                roi = gri[y:y+h, x:x+w]
                yol = os.path.join(hedef_klasor, f"{kayit_sayisi+1}.jpg")
                cv2.imwrite(yol, roi)
                kayit_sayisi += 1
                print(f"{kayit_sayisi}. yüz verisi kaydedildi.")

        cv2.imshow("Kamera", kare)

        if kayit_sayisi >= 5:
            print("5 yüz verisi başarıyla kaydedildi.")
            log_kaydet(kullanici_id)
            break

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

    kamera.release()
    cv2.destroyAllWindows()

# Tkinter arayüz (isim sadece gösterim için)
def baslat_arayuz(kullanici_id, kullanici_adi):
    def onayla():
        if not var_kabul.get():
            messagebox.showwarning("Uyarı", "Kişisel veri onayı gereklidir.")
            return
        root.destroy()
        yuz_kayit_sistemi(kullanici_id)

    root = tk.Tk()
    root.title("Yüz Kaydı Onay")
    root.geometry("420x200")
    root.resizable(False, False)

    tk.Label(root, text=f"Hoş geldin, {kullanici_adi}", font=("Century Gothic", 14)).pack(pady=20)

    var_kabul = tk.BooleanVar()
    tk.Checkbutton(
        root,
        text="Kişisel verilerimin alınmasını kabul ediyorum.",
        variable=var_kabul,
        font=("Century Gothic", 10)
    ).pack(pady=10)

    tk.Button(
        root,
        text="Kayda Başla",
        command=onayla,
        font=("Century Gothic", 12),
        bg="#4CAF50",
        fg="white"
    ).pack(pady=10)

    root.mainloop()

# Ana giriş noktası
if __name__ == "__main__":
    if len(sys.argv) < 3:
        print("Kullanıcı ID ve isim parametre olarak verilmelidir. Örn: python yuz_kayit.py 15 Asu")
        sys.exit(1)

    kullanici_id = sys.argv[1]
    kullanici_adi = sys.argv[2]
    baslat_arayuz(kullanici_id, kullanici_adi)

