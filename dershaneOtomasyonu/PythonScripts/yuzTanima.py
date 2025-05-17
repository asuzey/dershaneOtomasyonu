import cv2
import os
import sys
import numpy as np
from datetime import datetime
import threading

# --- Ayarlar ---
FILESERVER_PATH = "C:/Users/Public/FileServer/YuzTanima"
VIDEO_OUTPUT_PATH = "C:/Users/Public/FileServer/VideoKayit"
HAAR_CASCADE = "haarcascade_frontalface_default.xml"  # aynı dizine koy

# --- Video Kaydı için değişkenler ---
recording = False
video_writer = None

def yuz_dogrula(isim):
    # Yüz tanıma modeli yükle
    recognizer = cv2.face.LBPHFaceRecognizer_create()
    recognizer.read("trainer/trainer.yml")

    face_cascade = cv2.CascadeClassifier(HAAR_CASCADE)

    # Kayıtlı fotoğrafı alma
    klasor_yolu = os.path.join(FILESERVER_PATH, isim)
    eski_foto = os.path.join(klasor_yolu, "yuz.jpg")
    if not os.path.exists(eski_foto):
        print("Kayıtlı yüz verisi bulunamadı.")
        return False

    # Kamera başlat
    cam = cv2.VideoCapture(0)
    dogrulama_basari = False
    print("Yüz doğrulama başlatıldı...")

    while True:
        ret, frame = cam.read()
        if not ret:
            break
        gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
        faces = face_cascade.detectMultiScale(gray, 1.3, 5)

        for (x,y,w,h) in faces:
            id_, confidence = recognizer.predict(gray[y:y+h, x:x+w])
            if confidence < 40:  # %60 ve üstü benzerlik için 40'tan düşük olmalı
                dogrulama_basari = True
                cv2.putText(frame, f"Eşleşme: %{100-int(confidence)}", (x, y-10),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.9, (0,255,0), 2)
            else:
                cv2.putText(frame, "Eşleşme başarısız", (x, y-10),
                            cv2.FONT_HERSHEY_SIMPLEX, 0.9, (0,0,255), 2)
        cv2.imshow("Yüz Doğrulama", frame)

        if dogrulama_basari or cv2.waitKey(1) == 27:
            break

    cam.release()
    cv2.destroyAllWindows()
    return dogrulama_basari

def video_kayit(isim):
    global recording, video_writer
    cam = cv2.VideoCapture(0)
    fourcc = cv2.VideoWriter_fourcc(*'XVID')
    tarih = datetime.now().strftime("%Y%m%d_%H%M%S")
    video_yolu = os.path.join(VIDEO_OUTPUT_PATH, f"{isim}_{tarih}.avi")
    os.makedirs(VIDEO_OUTPUT_PATH, exist_ok=True)
    video_writer = cv2.VideoWriter(video_yolu, fourcc, 20.0, (640, 480))
    recording = True
    print("Video kaydı başladı...")

    while recording:
        ret, frame = cam.read()
        if not ret:
            break
        video_writer.write(frame)
        cv2.imshow("Kamera Kaydı", frame)
        if cv2.waitKey(1) == 27:
            break

    cam.release()
    video_writer.release()
    cv2.destroyAllWindows()
    print("Video kaydı durdu.")

def main():
    args = sys.argv
    if len(args) < 3:
        print("Eksik argüman: isim ve güvenlik bilgisi gerekiyor.")
        return

    isim = args[1]  # C# tarafından gelen isim
    güvenlik_modu = args[2].lower() == "güvenlik"

    print(f"Kullanıcı: {isim}")
    print(f"Güvenlik modu: {'Açık' if güvenlik_modu else 'Kapalı'}")

    if güvenlik_modu:
        if not yuz_dogrula(isim):
            print("Yüz doğrulama başarısız.")
            return

        video_thread = threading.Thread(target=video_kayit, args=(isim,))
        video_thread.start()

    input("E-Sınav başladı. Bitirmek için Enter'a basın...")

    if güvenlik_modu:
        global recording
        recording = False


if __name__ == "__main__":
    main()
