import tkinter as tk
from tkinter import messagebox
import cv2
import os
import numpy as np
from PIL import Image, ImageTk
import pyodbc
import sys
import subprocess

FONT = ("Century Gothic", 12)
KAYIT_YOLU = "C:/Users/Public/FileServer/YuzVerileri"

SQL_BAGLANTI = "Driver={ODBC Driver 17 for SQL Server};Server=localhost\\SQLEXPRESS;Database=DERSHANE;Trusted_Connection=yes;"

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))
HAAR_PATH = os.path.join(SCRIPT_DIR, "haarcascade_frontalface_default.xml")

kullanici = sys.argv[1] if len(sys.argv) > 1 else "bilinmeyen"

def ekran_ortala(pencere, genislik=700, yukseklik=600):
    ekran_gen = pencere.winfo_screenwidth()
    ekran_yuk = pencere.winfo_screenheight()
    x = int((ekran_gen / 2) - (genislik / 2))
    y = int((ekran_yuk / 2) - (yukseklik / 2))
    pencere.geometry(f"{genislik}x{yukseklik}+{x}+{y}")

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
    except:
        return "Hata"

def hesapla_benzerlik(img1, img2):
    try:
        img1_gray = cv2.cvtColor(img1, cv2.COLOR_BGR2GRAY)
        img2_gray = cv2.cvtColor(img2, cv2.COLOR_BGR2GRAY)
        img1_resized = cv2.resize(img1_gray, (100, 100))
        img2_resized = cv2.resize(img2_gray, (100, 100))
        diff = cv2.absdiff(img1_resized, img2_resized)
        benzerlik = 100 - np.mean(diff) * 100 / 255
        return max(0, round(benzerlik, 2))
    except:
        return 0

def kamera_dogrulama_ekrani():
    pencere = tk.Tk()
    pencere.title("Yüz Doğrulama")
    pencere.configure(bg="white")
    ekran_ortala(pencere)

    face_cascade = cv2.CascadeClassifier(HAAR_PATH)
    cap = cv2.VideoCapture(0)

    panel = tk.Label(pencere)
    panel.pack(pady=10)

    etiket_benzerlik = tk.Label(pencere, text="Yüz Benzerliği: Hesaplanıyor", font=FONT, bg="white", fg="orange")
    etiket_uzaklik = tk.Label(pencere, text="Yüz Uzaklığı: -", font=FONT, bg="white", fg="orange")
    etiket_arka = tk.Label(pencere, text="Arka Plan: -", font=FONT, bg="white", fg="orange")
    etiket_canlilik = tk.Label(pencere, text="Canlılık: -", font=FONT, bg="white", fg="orange")

    for e in [etiket_benzerlik, etiket_uzaklik, etiket_arka, etiket_canlilik]:
        e.pack()

    onceki_yuz = None
    hareket_sayaci = 0
    hareket_var = False
    gecis_yapildi = False

    referans_img_listesi = []
    try:
        klasor = os.path.join(KAYIT_YOLU, kullanici)
        for i in range(1, 6):
            yol = os.path.join(klasor, f"yuz_{i}.jpg")
            if os.path.exists(yol):
                img = cv2.imread(yol)
                if img is not None:
                    referans_img_listesi.append(img)
    except:
        referans_img_listesi = []

    def guncelle():
        nonlocal onceki_yuz, hareket_var, hareket_sayaci, gecis_yapildi

        ret, frame = cap.read()
        if not ret:
            pencere.after(100, guncelle)
            return

        frame = cv2.flip(frame, 1)
        gri = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        yuzler = face_cascade.detectMultiScale(gri, 1.1, 5)

        if len(yuzler) == 1:
            x, y, w, h = yuzler[0]
            roi = frame[y:y+h, x:x+w]

            uzaklik = 100 - min(100, abs(w - 100))
            etiket_uzaklik.config(text=f"Yüz Uzaklığı: {uzaklik}", fg="green")

            if onceki_yuz is not None:
                dx = abs(onceki_yuz[0] - x)
                dy = abs(onceki_yuz[1] - y)
                if dx > 5 or dy > 5:
                    hareket_sayaci += 1
                    if hareket_sayaci >= 3:
                        hareket_var = True
                        etiket_canlilik.config(text="Canlılık: Onaylandı", fg="green")
            onceki_yuz = (x, y)

            etiket_arka.config(text="Arka Plan: Uygun", fg="green")

            benzerlikler = [hesapla_benzerlik(img, roi) for img in referans_img_listesi]
            en_yuksek = max(benzerlikler) if benzerlikler else 0

            etiket_benzerlik.config(text=f"Yüz Benzerliği: %{en_yuksek}", fg="green" if en_yuksek >= 60 else "red")

            if en_yuksek >= 60 and hareket_var and not gecis_yapildi:
                gecis_yapildi = True
                messagebox.showinfo("Başarılı", "Kimlik doğrulama başarılı! Sınava geçiliyor...")
                pencere.destroy()
                # subprocess.call(["python", "sinav_uygulamasi.py", kullanici])
                return

        elif len(yuzler) > 1:
            etiket_arka.config(text="Arka Plan: Birden fazla yüz algılandı", fg="red")
        else:
            etiket_arka.config(text="Arka Plan: Yüz algılanamadı", fg="orange")

        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGBA)
        imgtk = ImageTk.PhotoImage(image=Image.fromarray(rgb_frame))
        panel.imgtk = imgtk
        panel.config(image=imgtk)

        pencere.after(100, guncelle)

    guncelle()
    pencere.mainloop()

# Açılış Ekranı
root = tk.Tk()
root.title("Yüz Doğrulama - Giriş")
root.configure(bg="white")
ekran_ortala(root, 500, 300)

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

    izin = messagebox.askyesno("Kamera İzni", "Uygulama kameranıza erişmek istiyor. Devam edilsin mi?")
    if not izin:
        root.destroy()
        return

    root.destroy()
    kamera_dogrulama_ekrani()

tk.Button(root, text="Devam Et", command=devam_et, font=FONT).pack(pady=20)
root.mainloop()