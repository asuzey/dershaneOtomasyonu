import os
import sys
import cv2
import time
import tkinter as tk
import threading
import pygetwindow as gw
import pyautogui
import numpy as np
from datetime import datetime
from PIL import Image, ImageTk
import pyodbc
import dlib

# -------------------- PARAMETRELER --------------------
if len(sys.argv) < 5:
    print("Kullanım: ogrenciKopya.exe <ogrenciAdi> <sinavId> <ogretmenAdi> <kamera_flag>")
    sys.exit(1)

kullanici_adi = sys.argv[1]
sinav_id = int(sys.argv[2])
ogretmen_adi = sys.argv[3]
kamera_acik_flag = sys.argv[4] == "1"

# -------------------- GLOBAL AYARLAR --------------------
face_detector = dlib.get_frontal_face_detector()
cap = cv2.VideoCapture(0)
kamera_gorunur = kamera_acik_flag
file_server_path = f"C:/Users/Public/FileServer/Kopyalar/ogrenciKopya/KopyaGoruntuleri"
os.makedirs(file_server_path, exist_ok=True)

# -------------------- SQL BAĞLANTISI --------------------
def get_conn():
    return pyodbc.connect(
        "Driver={ODBC Driver 17 for SQL Server};"
        "Server=localhost\\SQLEXPRESS;"
        "Database=DERSHANE1;"
        "Trusted_Connection=yes;"
    )

# -------------------- GÖRÜNTÜ ALMA --------------------
def ekran_goruntusu_al(etiket):
    timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
    dosya_adi = f"{etiket}_{timestamp}.png"
    tam_yol = os.path.join(file_server_path, dosya_adi)
    pyautogui.screenshot(tam_yol)
    print(f"[KOPYA] Ekran görüntüsü kaydedildi: {tam_yol}")
    return tam_yol

# -------------------- SQL KOPYA LOG KAYDI --------------------
def kopya_log_kaydet(tip, dosyaYolu):
    try:
        conn = get_conn()
        cursor = conn.cursor()
        cursor.execute("""
            INSERT INTO Kopyalar (OgrenciAdi, SinavId, OgretmenAdi, Tip, DosyaYolu, Tarih)
            VALUES (?, ?, ?, ?, ?, ?)
        """, kullanici_adi, sinav_id, ogretmen_adi, tip, dosyaYolu, datetime.now())
        conn.commit()
        conn.close()
    except Exception as e:
        print("[SQL HATA]", e)

# -------------------- GÖZETİM THREAD --------------------
def gozetim():
    aktif_pencere = None
    while True:
        ret, frame = cap.read()
        if not ret:
            continue

        frame = cv2.flip(frame, 1)
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        faces = face_detector(gray)

        if len(faces) > 1:
            print("[ALERT] İkinci yüz algılandı!")
            dosya = ekran_goruntusu_al("ikinci_yuz")
            kopya_log_kaydet("İkinciYuz", dosya)
            cap.release()
            os._exit(0)

        yeni_pencere = gw.getActiveWindow()
        if yeni_pencere and aktif_pencere and yeni_pencere.title != aktif_pencere.title:
            print(f"[ALERT] Pencere değişti: {yeni_pencere.title}")
            dosya = ekran_goruntusu_al("pencere_degisti")
            kopya_log_kaydet("PencereDegisimi", dosya)
        aktif_pencere = yeni_pencere

        if kamera_gorunur:
            img = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
            img = cv2.resize(img, (320, 240))  # BÜYÜTÜLDÜ
            img_pil = Image.fromarray(img)
            imgtk = ImageTk.PhotoImage(image=img_pil)
            video_label.config(image=imgtk)
            video_label.image = imgtk

        time.sleep(1)

# -------------------- ONAY KUTUSU --------------------
def izin_onayi():
    global kamera_gorunur
    pencere = tk.Tk()
    pencere.title("İzin Gerekli")
    pencere.geometry("400x250")
    pencere.attributes("-topmost", True)

    tk.Label(pencere, text="Lütfen aşağıdaki izinleri verin:", font=("Segoe UI", 11, "bold")).pack(pady=10)
    izin1 = tk.BooleanVar()
    izin2 = tk.BooleanVar()
    goster = tk.BooleanVar(value=kamera_gorunur)

    tk.Checkbutton(pencere, text="📷 Kameraya erişime izin veriyorum", variable=izin1).pack(anchor="w", padx=20)
    tk.Checkbutton(pencere, text="📄 Kişisel verilerime erişime izin veriyorum", variable=izin2).pack(anchor="w", padx=20)
    tk.Checkbutton(pencere, text="🙋 Görüntümü görmek istiyorum", variable=goster).pack(anchor="w", padx=20)

    def devam():
        if izin1.get() and izin2.get():
            kamera_gorunur = goster.get()
            pencere.destroy()
        else:
            tk.messagebox.showwarning("Uyarı", "Devam etmek için tüm izinleri vermelisiniz.")

    tk.Button(pencere, text="Devam Et", command=devam).pack(pady=15)
    pencere.mainloop()

# -------------------- ARAYÜZ --------------------
izin_onayi()
pencere = tk.Tk()
pencere.title("Gözetim Sistemi")
pencere.geometry("340x300+1100+50")  # GENİŞLETİLDİ
pencere.attributes('-topmost', True)
pencere.resizable(False, False)
pencere.protocol("WM_DELETE_WINDOW", lambda: None)

pos = {'x': 0, 'y': 0}
def start_move(event):
    pos['x'] = event.x
    pos['y'] = event.y
def do_move(event):
    x = pencere.winfo_pointerx() - pos['x']
    y = pencere.winfo_pointery() - pos['y']
    pencere.geometry(f"+{x}+{y}")
pencere.bind("<Button-1>", start_move)
pencere.bind("<B1-Motion>", do_move)

frame = tk.Frame(pencere)
frame.pack()
video_label = tk.Label(frame)
video_label.pack()

icon_path = "user_icon.png"
icon_img = Image.open(icon_path).resize((320, 240))  # BÜYÜTÜLDÜ
img_icon = ImageTk.PhotoImage(icon_img)

def degistir():
    global kamera_gorunur
    kamera_gorunur = not kamera_gorunur
    if not kamera_gorunur:
        video_label.config(image=img_icon)
        video_label.image = img_icon

toggle_btn = tk.Button(pencere, text="Görünümü Değiştir", command=degistir)
toggle_btn.pack(pady=5)

# -------------------- BAŞLAT --------------------
threading.Thread(target=gozetim, daemon=True).start()
pencere.mainloop()
cap.release()
