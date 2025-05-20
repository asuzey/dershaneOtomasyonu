# -*- coding: utf-8 -*-
import tkinter as tk
from tkinter import ttk, messagebox
from tkinter.font import Font
from ctypes import windll
import pyodbc
import datetime
import json
import os
from dotenv import load_dotenv
import google.generativeai as genai
import sys 

# DPI düzeltme (bulanıklığı giderir)
windll.shcore.SetProcessDpiAwareness(1)

if len(sys.argv) >= 2:
    giris_yapmis_kullanici_id = int(sys.argv[1])
else:
    giris_yapmis_kullanici_id = None


load_dotenv()
GEMINI_API_KEY = os.getenv("GEMINI_API_KEY")
genai.configure(api_key=GEMINI_API_KEY)
model = genai.GenerativeModel("gemini-1.5-flash")

THEME = {
    "bg": "#f7faff",              # Yumuşak beyaz
    "fg": "#002244",              # Lacivert
    "accent": "#003366",          # Koyu lacivert (buton hover + vurgu)
    "button_bg": "#e1ecf9",       # Hafif mavi buton rengi
    "button_fg": "#002244",
    "button_hover": "#003366",    # Buton hover rengi
    "optik_bg": "#f0f4ff",
    "highlight": "#dce9ff"
}

# Veritabanı bağlantısı

def get_db_connection():
    return pyodbc.connect(
        'DRIVER={ODBC Driver 17 for SQL Server};'
        'SERVER=localhost\\SQLEXPRESS;'
        'DATABASE=DERSHANE1;'
        'Trusted_Connection=yes;'
    )
class ScrollableFrame(ttk.Frame):
    def __init__(self, container, *args, **kwargs):
        super().__init__(container, *args, **kwargs)
        canvas = tk.Canvas(self, bg=THEME["bg"], highlightthickness=0)
        scrollbar = ttk.Scrollbar(self, orient="vertical", command=canvas.yview)
        self.inner = ttk.Frame(canvas)

        self.inner.bind(
            "<Configure>",
            lambda e: canvas.configure(scrollregion=canvas.bbox("all"))
        )

        canvas.create_window((0, 0), window=self.inner, anchor="nw")
        canvas.configure(yscrollcommand=scrollbar.set)

        canvas.pack(side="left", fill="both", expand=True)
        scrollbar.pack(side="right", fill="y")

        self.canvas = canvas
        self.inner = self.inner
        canvas.bind_all("<MouseWheel>", lambda e: canvas.yview_scroll(int(-1 * (e.delta / 120)), "units"))

class OdevApp:
    def __init__(self, root, giris_yapmis_kullanici_id=None):

        self.root = root
        self.kullanici_id = giris_yapmis_kullanici_id

        self.root.title("Modern Dershane Otomasyonu")
        self.root.geometry("1000x700")
        self.font = ("Segoe UI", 11)
        self.title_font = ("Segoe UI", 16, "bold")

        self.sf = ScrollableFrame(self.root)
        self.sf.pack(fill="both", expand=True)
        self.content_frame = self.sf.inner

        self.style = ttk.Style()
        self.style.theme_use("clam")
        self.style.configure("TFrame", background=THEME["bg"])
        self.style.configure("TLabel", background=THEME["bg"], foreground=THEME["fg"], font=self.font)
        self.style.configure("TButton", background=THEME["button_bg"], foreground=THEME["button_fg"],
                             font=self.font, padding=6, relief="flat")
        self.style.map("TButton", background=[("active", THEME["button_hover"])], foreground=[("active", "white")])
        self.style.configure("TEntry", fieldbackground="white", foreground=THEME["fg"])
        self.style.configure("TCombobox", fieldbackground="white", background="white",
                             foreground=THEME["fg"], arrowcolor=THEME["fg"])

        self.conn = get_db_connection()
        self.kullanici_id = None

        self.show_ogretmen_secimi()

    def clear(self):
        # Önceki frame'i açıkça kaldır
        if hasattr(self, 'ogretmen_frame'):
            self.ogretmen_frame.place_forget()  # place() ile eklenen widget'ı kaldır
        for widget in self.content_frame.winfo_children():
            widget.destroy()

    def show_ogretmen_secimi(self):
        if self.kullanici_id:
            self.show_odev_menu()
            return

        self.clear()

        # Frame'i oluştur ve saklamak için bir referans tut
        self.ogretmen_frame = ttk.Frame(self.root, padding=20, relief="solid")
        self.ogretmen_frame.place(relx=0.5, rely=0.5, anchor="center")

        ttk.Label(self.ogretmen_frame, text="Lütfen Giriş Yapacak Öğretmeni Seçin", font=self.title_font).pack(pady=50)

        self.ogretmen_var = tk.StringVar()
        self.ogretmen_cb = ttk.Combobox(self.ogretmen_frame, textvariable=self.ogretmen_var, state="readonly",
                                        font=self.font, width=30)
        self.ogretmen_cb.pack(pady=10)

        cur = self.conn.cursor()
        cur.execute("SELECT Id, KullaniciAdi FROM Kullanicilar WHERE RoleId = 2")
        self.ogretmenler = cur.fetchall()
        self.ogretmen_cb['values'] = [k[1] for k in self.ogretmenler]

        ttk.Button(self.ogretmen_frame, text="Giriş Yap", command=self.handle_giris).pack(pady=10)


    def handle_giris(self):
        secilen = self.ogretmen_cb.get()
        if not secilen:
            messagebox.showerror("Hata", "Lütfen bir öğretmen seçin.")
            return
        for ogretmen in self.ogretmenler:
            if ogretmen[1] == secilen:
                self.kullanici_id = ogretmen[0]
                break
        self.show_odev_menu()

    def show_odev_menu(self):
        self.clear()  # Önceki içeriği temizle

        # Ana container - scrollable alana yerleştir
        main_container = ttk.Frame(self.content_frame)
        main_container.pack(fill="both", expand=True, pady=50)

        # Kart container'ı - ortalamak için
        card = ttk.Frame(main_container, padding=30, style="Card.TFrame")
        card.pack(expand=True)

        # Widget'ları kartın içine ekle
        ttk.Label(card, text="Ödev Tanımlama Seçimi",
                  font=self.title_font).pack(pady=20)

        ttk.Button(card, text="Kişiye Özel Ödev Tanımla", width=30,
                   command=self.show_kisiye_odev_tanimla).pack(pady=10)

        ttk.Button(card, text="Sınıfa Ödev Tanımla", width=30,
                   command=self.show_sinifa_odev_tanimla).pack(pady=10)

        ttk.Button(card, text="Çıkış Yap",
                   command=self.show_ogretmen_secimi).pack(pady=10)

        ttk.Button(card, text="Yapılmış Ödevleri Görüntüle", width=30,
                   command=self.show_yapilmis_odevler).pack(pady=10)

        # Frame boyutlarını güncelle
        self.content_frame.update_idletasks()

    def show_kisiye_odev_tanimla(self):
        self.clear()

        # Frame'i oluştur ve saklamak için bir referans tut
        self.ogretmen_frame = ttk.Frame(self.root, padding=20, relief="solid")
        self.ogretmen_frame.place(relx=0.5, rely=0.5, anchor="center")

        ttk.Label(self.content_frame, text="Kişiye Özel Ödev Tanımlama", font=self.title_font).pack(pady=20)

        frame = ttk.Frame(self.content_frame)
        frame.pack(pady=10)

        ttk.Label(frame, text="Kategori:").grid(row=0, column=0, padx=5, pady=5)
        self.kategori_var = tk.StringVar()
        self.kategori_cb = ttk.Combobox(frame, textvariable=self.kategori_var, state="readonly", width=40)
        self.kategori_cb.grid(row=0, column=1, padx=5, pady=5)

        self.load_kategoriler()

        ttk.Label(frame, text="Öğrenciler:").grid(row=1, column=0, padx=5, pady=5)
        self.ogrenci_listbox = tk.Listbox(frame, selectmode="multiple", width=40, height=10, exportselection=False)
        self.ogrenci_listbox.grid(row=1, column=1, padx=5, pady=5)

        cur = self.conn.cursor()
        cur.execute("SELECT Id, KullaniciAdi FROM Kullanicilar WHERE RoleId = 3")
        self.ogrenciler = cur.fetchall()
        for o in self.ogrenciler:
            self.ogrenci_listbox.insert(tk.END, o[1])

        ttk.Button(self.content_frame, text="Ödev Tanımla", command=self.kisiye_odev_tanimla_veritabani).pack(pady=20)
        ttk.Button(self.content_frame, text="Geri", command=self.show_odev_menu).pack(pady=20)

    def kisiye_odev_tanimla_veritabani(self):

        kategori_adi = self.kategori_var.get()
        kategori = next((k for k in self.kategoriler if k[1] == kategori_adi), None)
        if not kategori:
            messagebox.showerror("Hata", "Kategori seçilmedi.")
            return

        secilenler = self.ogrenci_listbox.curselection()
        if not secilenler:
            messagebox.showerror("Hata", "En az bir öğrenci seçmelisiniz.")
            return

        tarih = datetime.datetime.now()
        cur = self.conn.cursor()
        cur.execute("SELECT VarsayilanSure, SoruSayisi FROM SinavKategorileri WHERE Id = ?", kategori[0])
        sure, soru_sayisi = cur.fetchone()

        cur.execute("""
            INSERT INTO Sinavlar (Adi, SinavKategoriId, Tarih, Sure, OlusturucuId, Atanan_Sinif)
            VALUES (?, ?, ?, ?, ?, NULL)
        """, (kategori_adi + " Kişiye Özel", kategori[0], tarih, sure, self.kullanici_id))
        self.conn.commit()
        sinav_id = cur.execute("SELECT @@IDENTITY").fetchval()

        for i in secilenler:
            ogrenci_id = self.ogrenciler[i][0]
            cur.execute("INSERT INTO OgrenciSinavlari (SinavId, KullaniciId) VALUES (?, ?)", sinav_id, ogrenci_id)
        self.conn.commit()

        messagebox.showinfo("Başarılı", "Ödev başarıyla öğrencilere tanımlandı.")
        self.show_soru_olusturma(sinav_id, kategori[0], soru_sayisi)

    def show_sinifa_odev_tanimla(self):
        self.clear()

        # Frame'i oluştur ve saklamak için bir referans tut
        self.ogretmen_frame = ttk.Frame(self.root, padding=20, relief="solid")
        self.ogretmen_frame.place(relx=0.5, rely=0.5, anchor="center")

        ttk.Label(self.content_frame, text="Sınıfa Ödev Tanımlama", font=self.title_font).pack(pady=20)

        frame = ttk.Frame(self.content_frame)
        frame.pack(pady=10)

        ttk.Label(frame, text="Kategori:").grid(row=0, column=0, padx=5, pady=5)
        self.kategori_var = tk.StringVar()
        self.kategori_cb = ttk.Combobox(frame, textvariable=self.kategori_var, state="readonly", width=40)
        self.kategori_cb.grid(row=0, column=1, padx=5, pady=5)
        self.load_kategoriler()

        ttk.Label(frame, text="Sınıflar:").grid(row=1, column=0, padx=5, pady=5)
        self.sinif_listbox = tk.Listbox(frame, selectmode="multiple", width=40, height=10, exportselection=False)
        self.sinif_listbox.grid(row=1, column=1, padx=5, pady=5)

        cur = self.conn.cursor()
        cur.execute("SELECT Id, Kodu FROM Siniflar")
        self.siniflar = cur.fetchall()
        for s in self.siniflar:
            self.sinif_listbox.insert(tk.END, s[1])

        ttk.Button(self.content_frame, text="Ödev Tanımla", command=self.sinifa_odev_tanimla_veritabani).pack(pady=20)
        ttk.Button(self.content_frame, text="Geri", command=self.show_odev_menu).pack(pady=20)

    def sinifa_odev_tanimla_veritabani(self):

        kategori_adi = self.kategori_var.get()
        kategori = next((k for k in self.kategoriler if k[1] == kategori_adi), None)
        if not kategori:
            messagebox.showerror("Hata", "Kategori seçilmedi.")
            return

        secilenler = self.sinif_listbox.curselection()
        if not secilenler:
            messagebox.showerror("Hata", "En az bir sınıf seçmelisiniz.")
            return

        tarih = datetime.datetime.now()
        cur = self.conn.cursor()
        cur.execute("SELECT VarsayilanSure, SoruSayisi FROM SinavKategorileri WHERE Id = ?", kategori[0])
        sure, soru_sayisi = cur.fetchone()

        for i in secilenler:
            sinif_id = self.siniflar[i][0]
            cur.execute("""
                INSERT INTO Sinavlar (Adi, SinavKategoriId, Tarih, Sure, OlusturucuId, Atanan_Sinif)
                VALUES (?, ?, ?, ?, ?, ?)
            """, (
            kategori_adi + f" Sınıf {self.siniflar[i][1]}", kategori[0], tarih, sure, self.kullanici_id, sinif_id))
            self.conn.commit()
            sinav_id = cur.execute("SELECT @@IDENTITY").fetchval()
            self.show_soru_olusturma(sinav_id, kategori[0], soru_sayisi)

    def load_siniflar(self):
        self.clear()
        container = ttk.Frame(self.content_frame)
        container.pack(pady=30)

        card = ttk.Frame(container, padding=30, style="Card.TFrame")
        card.pack()

        cur = self.conn.cursor()
        cur.execute("SELECT Id, Kodu FROM Siniflar")
        self.siniflar = cur.fetchall()
        self.sinif_cb['values'] = [s[1] for s in self.siniflar]

    def tanimla_odev(self):
        kategori_adi = self.kategori_var.get()
        sinif_kodu = self.sinif_var.get()
        tarih = datetime.datetime.now()

        kategori = next((k for k in self.kategoriler if k[1] == kategori_adi), None)
        sinif = next((s for s in self.siniflar if s[1] == sinif_kodu), None)
        if not kategori or not sinif:
            messagebox.showerror("Hata", "Lütfen tüm alanları doldurun.")
            return

        cur = self.conn.cursor()
        cur.execute("SELECT VarsayilanSure, SoruSayisi FROM SinavKategorileri WHERE Id = ?", kategori[0])
        sure, soru_sayisi = cur.fetchone()

        cur.execute("""
            INSERT INTO Sinavlar (Adi, SinavKategoriId, Tarih, Sure, OlusturucuId, Atanan_Sinif)
            VALUES (?, ?, ?, ?, ?, ?)
        """, (kategori_adi + " Ödevi", kategori[0], tarih, sure, self.kullanici_id, sinif[0]))
        self.conn.commit()
        sinav_id = cur.execute("SELECT @@IDENTITY").fetchval()

        messagebox.showinfo("Başarılı", "Ödev başarıyla tanımlandı.")
        self.show_soru_olusturma(sinav_id, kategori[0], soru_sayisi)

    def show_soru_olusturma(self, sinav_id, kategori_id, soru_sayisi):
        self.clear()

        # Ana frame
        self.ogretmen_frame = ttk.Frame(self.root, padding=20, relief="solid")
        self.ogretmen_frame.place(relx=0.5, rely=0.5, anchor="center")

        ttk.Label(self.content_frame, text="Soru Oluşturma Ekranı", font=self.title_font).pack(pady=10)
        scrollable = ttk.Frame(self.root)
        scrollable.pack(fill="both", expand=True)

        # Bu eksikti 👇
        frame = ttk.Frame(self.content_frame)
        frame.pack(pady=10)

        # Kategori seçimi
        ttk.Label(frame, text="Kategori:").grid(row=0, column=0, padx=5, pady=5)
        self.kategori_var = tk.StringVar()
        self.kategori_cb = ttk.Combobox(frame, textvariable=self.kategori_var, state="readonly", width=30)
        self.kategori_cb.grid(row=0, column=1, padx=5, pady=5)

        cur = self.conn.cursor()
        cur.execute("SELECT Id, Adi FROM SinavKategorileri WHERE Id IN (4, 5, 6);")
        kategori_data = cur.fetchall()
        self.kategoriler = {k[1]: k[0] for k in kategori_data}
        self.kategori_cb['values'] = list(self.kategoriler.keys())

        # Ders seçimi çoklu Listbox
        ttk.Label(frame, text="Dersler:").grid(row=1, column=0, padx=5, pady=5)
        self.ders_listbox = tk.Listbox(frame, selectmode="multiple", width=40, height=6, exportselection=False)
        self.ders_listbox.grid(row=1, column=1, padx=5, pady=5)

        # Mousewheel olayını bağla (Windows)
        self.ders_listbox.bind("<Enter>", lambda e: self.bind_listbox_scroll(self.ders_listbox))
        self.ders_listbox.bind("<Leave>", lambda e: self.unbind_listbox_scroll())

        # Kategori değiştiğinde dersleri güncelle
        def kategori_degisti(event):
            kategori_adi = self.kategori_var.get()
            kategori_id = self.kategoriler.get(kategori_adi)
            if not kategori_id:
                return
            cur.execute("SELECT Adi FROM SinavDersleri WHERE SinavKategoriId = ?", kategori_id)
            self.ders_listbox.delete(0, tk.END)
            for r in cur.fetchall():
                self.ders_listbox.insert(tk.END, r[0])

        self.kategori_cb.bind("<<ComboboxSelected>>", kategori_degisti)

        # Şık sayısı seçimi
        ttk.Label(frame, text="Şık Sayısı:").grid(row=2, column=0, padx=5, pady=5)
        self.sik_sayisi_var = tk.IntVar(value=4)
        sik_spin = ttk.Spinbox(frame, from_=2, to=8, textvariable=self.sik_sayisi_var, width=5)
        sik_spin.grid(row=2, column=1, padx=5, pady=5)

        # Diğer kontroller
        ttk.Label(self.content_frame, text=f"Oluşturulacak Soru Sayısı: {soru_sayisi}", font=self.font).pack(pady=5)

        ttk.Button(self.content_frame, text="Soruları Oluştur",
                   command=lambda: self.generate_questions(sinav_id, soru_sayisi)).pack(pady=10)

        ttk.Button(self.content_frame, text="Ana Menü", command=self.show_ogretmen_secimi).pack(pady=10)
        ttk.Button(self.content_frame, text="Elle Soru Ekle",
                   command=lambda: self.show_elle_soru_ekleme(sinav_id, kategori_id)).pack(pady=5)
        ttk.Button(self.content_frame, text="Geri", command=self.show_odev_menu).pack(pady=20)

    def bind_listbox_scroll(self, widget):
        def on_mousewheel(event):
            widget.yview_scroll(int(-1 * (event.delta / 120)), "units")
            return "break"

        widget.bind("<MouseWheel>", on_mousewheel)
        self._active_listbox = widget

    def unbind_listbox_scroll(self):
        if hasattr(self, "_active_listbox"):
            self._active_listbox.unbind("<MouseWheel>")
            del self._active_listbox

    def generate_questions(self, sinav_id, soru_sayisi):
        kategori_adi = self.kategori_var.get()
        kategori_id = self.kategoriler.get(kategori_adi)
        ders_indeksleri = self.ders_listbox.curselection()
        if not ders_indeksleri:
            messagebox.showwarning("Uyarı", "Lütfen en az bir ders seçin.")
            return

        secilen_dersler = [self.ders_listbox.get(i) for i in ders_indeksleri]
        secenek_sayisi = self.sik_sayisi_var.get()

        cur = self.conn.cursor()
        placeholders = ', '.join(['?'] * len(secilen_dersler))

        cur.execute(f"""
            SELECT d.Adi, k.Ad FROM SinavDersleri d
            JOIN SinavDersKonulari k ON d.Id = k.SinavDersId
            WHERE d.SinavKategoriId = ? AND d.Adi IN ({placeholders})
        """, [kategori_id] + secilen_dersler)

        if not secilen_dersler:
            messagebox.showwarning("Uyarı", "Seçilen ders için konu bulunamadı.")
            return

        secenek_harfleri = [chr(65 + i) for i in range(secenek_sayisi)]  # A, B, C, D, E...
        ders_soru_adet = soru_sayisi // len(secilen_dersler)
        dagilim = [ders_soru_adet] * len(secilen_dersler)

        # Artan varsa sırayla dağıt
        kalan = soru_sayisi - sum(dagilim)
        for i in range(kalan):
            dagilim[i] += 1

        prompt = (
            f"Sen bir eğitim asistanısın.\n\n"
            f"Aşağıdaki kategoride, her ders için belirtilen sayıda çoktan seçmeli soru oluştur.\n\n"
            f"Kategori: {kategori_adi}\n"
            f"Şık sayısı: {secenek_sayisi}\n\n"
        )

        for ders_adi, adet in zip(secilen_dersler, dagilim):
            cur.execute("""
                SELECT k.Ad FROM SinavDersKonulari k
                JOIN SinavDersleri d ON d.Id = k.SinavDersId
                WHERE d.SinavKategoriId = ? AND d.Adi = ?
            """, kategori_id, ders_adi)
            konular = [row[0] for row in cur.fetchall()]
            if not konular:
                continue
            konu_text = "\n".join([f"- {k}" for k in konular])
            prompt += (
                f"Ders: {ders_adi}\n"
                f"Üretilecek Soru Sayısı: {adet}\n"
                f"Konular:\n{konu_text}\n\n"
            )
            if "Ders:" not in prompt:
                messagebox.showerror("Hata", "Hiçbir derse ait konu bulunamadı. Lütfen önce konuları tanımlayın.")
                return

        prompt += (
                f"Her soru aşağıdaki formatta olmalı:\n"
                f"Soru: [metin]\n" +
                "\n".join([f"{chr(65 + i)}) [şık {chr(65 + i)}]" for i in range(secenek_sayisi)]) + "\n" +
                "Doğru: [harf]\nZorluk: [1-5]\nKonu: [konu adı]\nDers: [ders adı]\n\n"
                f"Lütfen format dışına çıkmadan toplam {soru_sayisi} soru üret."
        )

        response = model.generate_content(prompt)
        lines = response.text.strip().splitlines()

        questions = []
        current = {}
        for line in lines:
            line = line.strip()
            if line.startswith("Soru:"):
                if current:
                    questions.append(current)
                current = {"question": line[5:].strip(), "options": [], "answer": "", "difficulty": 1, "topic": "", "lesson": ""}
            elif any(line.startswith(f"{h})") for h in secenek_harfleri):
                current["options"].append(line[2:].strip())
            elif line.startswith("Doğru:"):
                current["answer"] = line.split(":", 1)[1].strip()
            elif line.startswith("Zorluk:"):
                current["difficulty"] = int(line.split(":", 1)[1].strip())
            elif line.startswith("Konu:"):
                current["topic"] = line.split(":", 1)[1].strip()
            elif line.startswith("Ders:"):
                current["lesson"] = line.split(":", 1)[1].strip()
        if current:
            questions.append(current)

        with open("question_regist.json", "w", encoding="utf-8") as f:
            json.dump({"questions": questions}, f, ensure_ascii=False, indent=4)

        self.kaydet_sorular(sinav_id, kategori_id, questions)

        messagebox.showinfo("Başarılı", f"{len(questions)} soru oluşturuldu ve veritabanına kaydedildi.")


    def kaydet_sorular(self, sinav_id, kategori_id, questions):
        cur = self.conn.cursor()
        for q in questions:
            metin = q["question"]
            yildiz = q["difficulty"]
            konu = q["topic"]
            ders = q["lesson"]

            cur.execute("""
                SELECT k.Id FROM SinavDersKonulari k
                JOIN SinavDersleri d ON k.SinavDersId = d.Id
                WHERE k.Ad = ? AND d.Adi = ? AND d.SinavKategoriId = ?
            """, konu, ders, kategori_id)
            konu_row = cur.fetchone()
            if not konu_row:
                continue
            konu_id = konu_row[0]

            cur.execute("SELECT Id FROM SinavDersleri WHERE Adi = ? AND SinavKategoriId = ?", ders, kategori_id)
            ders_row = cur.fetchone()
            if not ders_row:
                continue
            ders_id = ders_row[0]

            cur.execute("""
                INSERT INTO Sorular (SoruMetni, SinifSeviyeId, YildizSeviyesi, SinavDersKonuId, SecenekSayisi, SinavId, Ders)
                VALUES (?, ?, ?, ?, ?, ?, ?)
            """, metin, 1, yildiz, konu_id, len(q["options"]), sinav_id, ders_id)
            soru_id = cur.execute("SELECT @@IDENTITY").fetchval()

            for idx, secenek in enumerate(q["options"]):
                dogru = 1 if chr(65 + idx) == q["answer"] else 0
                cur.execute("""
                    INSERT INTO Secenekler (SoruId, Aciklama, Status, Ogr_select)
                    VALUES (?, ?, ?, ?)
                """, soru_id, secenek, dogru, 0)

            cur.execute("INSERT INTO SinavSorulari (SinavId, SoruId) VALUES (?, ?)", sinav_id, soru_id)

        self.conn.commit()
        messagebox.showinfo("Başarılı", "Sorular veritabanına kaydedildi.")
        self.show_ogretmen_secimi()

    def load_kategoriler(self):
        cur = self.conn.cursor()
        cur.execute("SELECT Id, Adi FROM SinavKategorileri WHERE Adi IN ('Test1', 'Test2', 'Test3', 'Test4')")
        self.kategoriler = cur.fetchall()
        self.kategori_cb['values'] = [k[1] for k in self.kategoriler]



    def show_tanimlanmis_odevler(self):
        self.clear()
        container = ttk.Frame(self.content_frame)
        container.place(relx=0.5, rely=0.1, anchor="n")
        card = ttk.Frame(container, padding=30, style="Card.TFrame")
        card.pack()

        ttk.Label(self.content_frame, text="Tanımlanmış Ödevler", font=self.title_font).pack(pady=20)
        scrollable = ttk.Frame(self.root)
        scrollable.pack(fill="both", expand=True)

        tree = ttk.Treeview(self.root, columns=("Adi", "SoruSayisi", "Tarih", "Kullanici", "Sure", "Sinif", "Detay"), show="headings")
        tree.heading("Adi", text="Sınav Adı")
        tree.heading("SoruSayisi", text="Soru Sayısı")
        tree.heading("Tarih", text="Tarih")
        tree.heading("Kullanici", text="Oluşturan")
        tree.heading("Sure", text="Süre (dk)")
        tree.heading("Sinif", text="Sınıf")
        tree.heading("Detay", text="Sonuçlar")
        tree.pack(fill="both", expand=True, padx=10, pady=10)

        cur = self.conn.cursor()
        cur.execute("""
            SELECT s.Id, s.Adi, s.Tarih, u.KullaniciAdi, s.Sure, c.Kodu,
                (SELECT COUNT(*) FROM Sorular WHERE SinavId = s.Id) AS SoruSayisi
            FROM Sinavlar s
            JOIN Kullanicilar u ON s.OlusturucuId = u.Id
            JOIN Siniflar c ON s.Atanan_Sinif = c.Id
        """)
        rows = cur.fetchall()

        for row in rows:
            tree.insert('', 'end', values=(row[1], row[6], row[2], row[3], row[4], row[5], "Görüntüle"), tags=(str(row[0]),))

        tree.bind("<Double-1>", lambda event: self.show_sinav_sonuclari(tree))
        ttk.Button(self.content_frame, text="Ana Menü", command=self.show_ogretmen_secimi).pack(pady=10)

    def show_elle_soru_ekleme(self, sinav_id, kategori_id):
        self.clear()
        container = ttk.Frame(self.content_frame)
        container.place(relx=0.5, rely=0.1, anchor="n")
        card = ttk.Frame(container, padding=30, style="Card.TFrame")
        card.pack()

        ttk.Label(self.content_frame, text="Manuel Soru Ekleme Ekranı", font=self.title_font).pack(pady=10)

        form = ttk.Frame(self.content_frame)
        form.pack(pady=10)

        # --- Kategori Seçimi ---
        ttk.Label(form, text="Kategori:").grid(row=0, column=0, padx=5, pady=5, sticky="e")
        self.elle_kategori_var = tk.StringVar()
        self.elle_kategori_cb = ttk.Combobox(form, textvariable=self.elle_kategori_var, state="readonly", width=30)
        self.elle_kategori_cb.grid(row=0, column=1, padx=5, pady=5)

        cur = self.conn.cursor()
        cur.execute("SELECT Id, Adi FROM SinavKategorileri WHERE Id IN (4, 5, 6)")
        kategoriler = cur.fetchall()
        self.kategori_dict = {k[1]: k[0] for k in kategoriler}
        self.elle_kategori_cb['values'] = list(self.kategori_dict.keys())

        # --- Ders Seçimi ---
        ttk.Label(form, text="Ders:").grid(row=1, column=0, padx=5, pady=5, sticky="e")
        self.elle_ders_var = tk.StringVar()
        self.elle_ders_cb = ttk.Combobox(form, textvariable=self.elle_ders_var, state="readonly", width=30)
        self.elle_ders_cb.grid(row=1, column=1, padx=5, pady=5)

        # --- Konu Seçimi ---
        ttk.Label(form, text="Konu:").grid(row=2, column=0, padx=5, pady=5, sticky="e")
        self.elle_konu_var = tk.StringVar()
        self.elle_konu_cb = ttk.Combobox(form, textvariable=self.elle_konu_var, state="readonly", width=30)
        self.elle_konu_cb.grid(row=2, column=1, padx=5, pady=5)

        def update_dersler(*args):
            secili_kat = self.elle_kategori_var.get()
            kat_id = self.kategori_dict.get(secili_kat)
            if not kat_id:
                return
            cur.execute("SELECT Adi FROM SinavDersleri WHERE SinavKategoriId = ?", kat_id)
            dersler = [row[0] for row in cur.fetchall()]
            self.elle_ders_cb['values'] = dersler
            self.elle_ders_cb.set("")
            self.elle_konu_cb.set("")
            self.elle_konu_cb['values'] = []

        def update_konular(*args):
            secili_kat = self.elle_kategori_var.get()
            kat_id = self.kategori_dict.get(secili_kat)
            secili_ders = self.elle_ders_var.get()
            if not kat_id or not secili_ders:
                return
            cur.execute("""
                SELECT k.Ad FROM SinavDersKonulari k
                JOIN SinavDersleri d ON d.Id = k.SinavDersId
                WHERE d.SinavKategoriId = ? AND d.Adi = ?
            """, kat_id, secili_ders)
            konular = [row[0] for row in cur.fetchall()]
            self.elle_konu_cb['values'] = konular
            self.elle_konu_cb.set("")

        self.elle_kategori_var.trace_add("write", update_dersler)
        self.elle_ders_var.trace_add("write", update_konular)

        # --- Soru Metni ---
        ttk.Label(form, text="Soru:").grid(row=3, column=0, padx=5, pady=5, sticky="ne")
        self.soru_entry = tk.Text(form, height=4, width=50)
        self.soru_entry.grid(row=3, column=1, padx=5, pady=5)

        # --- Şıklar ---
        ttk.Label(form, text="Şıklar:").grid(row=4, column=0, padx=5, pady=5, sticky="ne")
        self.sik_frame = ttk.Frame(form)
        self.sik_frame.grid(row=4, column=1, padx=5, pady=5, sticky="w")
        self.sik_entries = []
        self.dogru_sik_var = tk.StringVar()

        def add_sik():
            index = len(self.sik_entries)
            harf = chr(65 + index)
            row = ttk.Frame(self.sik_frame)
            row.pack(anchor="w", pady=2)
            ttk.Label(row, text=f"{harf})").pack(side="left")
            entry = ttk.Entry(row, width=40)
            entry.pack(side="left", padx=5)
            radio = ttk.Radiobutton(row, variable=self.dogru_sik_var, value=harf)
            radio.pack(side="left")
            self.sik_entries.append((entry, harf))

        def remove_last_sik():
            if len(self.sik_entries) <= 2:
                messagebox.showwarning("Uyarı", "En az 2 şık kalmalı.")
                return
            entry, harf, row = self.sik_entries.pop()
            row.destroy()

        ttk.Button(form, text="Yeni Şık Ekle", command=add_sik).grid(row=5, column=1, pady=5, sticky="w")
        ttk.Button(form, text="Son Şıkkı Sil", command=remove_last_sik).grid(row=5, column=1, padx=(150, 0), pady=5,
                                                                             sticky="w")
        for _ in range(4):
            add_sik()

        # --- Zorluk ---
        ttk.Label(form, text="Zorluk (1-5):").grid(row=6, column=0, padx=5, pady=5, sticky="e")
        self.zorluk_var = tk.IntVar(value=1)
        ttk.Spinbox(form, from_=1, to=5, textvariable=self.zorluk_var, width=5).grid(row=6, column=1, sticky="w")

        # --- Butonlar ---
        ttk.Button(form, text="Soruyu Kaydet",
                   command=lambda: self.kaydet_elle_soru(sinav_id, kategori_id)).grid(row=7, column=1, pady=10,
                                                                                      sticky="w")
        ttk.Button(form, text="Geri",
                   command=lambda: self.show_soru_olusturma(sinav_id, kategori_id, 0)).grid(row=8, column=1, pady=5,
                                                                                            sticky="w")

    def kaydet_elle_soru(self, sinav_id, _kategori_id):
        soru_metin = self.soru_entry.get("1.0", tk.END).strip()
        zorluk = self.zorluk_var.get()
        konu = self.elle_konu_var.get()
        ders = self.elle_ders_var.get()
        kategori_adi = self.elle_kategori_var.get()
        kategori_id = self.kategori_dict.get(kategori_adi)
        dogru_harf = self.dogru_sik_var.get()

        if not soru_metin or not konu or not ders or not dogru_harf:
            messagebox.showerror("Hata", "Lütfen tüm alanları doldurun.")
            return

        secenekler = [entry.get().strip() for entry, harf in self.sik_entries if entry.get().strip()]
        if len(secenekler) < 2:
            messagebox.showerror("Hata", "En az 2 şık olmalı.")
            return

        cur = self.conn.cursor()
        cur.execute("""
            SELECT k.Id FROM SinavDersKonulari k
            JOIN SinavDersleri d ON k.SinavDersId = d.Id
            WHERE k.Ad = ? AND d.Adi = ? AND d.SinavKategoriId = ?
        """, konu, ders, kategori_id)
        konu_row = cur.fetchone()
        if not konu_row:
            messagebox.showerror("Hata", "Konu bulunamadı.")
            return
        konu_id = konu_row[0]

        cur.execute("SELECT Id FROM SinavDersleri WHERE Adi = ? AND SinavKategoriId = ?", ders, kategori_id)
        ders_row = cur.fetchone()
        if not ders_row:
            messagebox.showerror("Hata", "Ders bulunamadı.")
            return
        ders_id = ders_row[0]

        cur.execute("""
            INSERT INTO Sorular (SoruMetni, SinifSeviyeId, YildizSeviyesi, SinavDersKonuId, SecenekSayisi, SinavId, Ders)
            VALUES (?, ?, ?, ?, ?, ?, ?)
        """, soru_metin, 1, zorluk, konu_id, len(secenekler), sinav_id, ders_id)
        soru_id = cur.execute("SELECT @@IDENTITY").fetchval()

        for i, secenek in enumerate(secenekler):
            harf = chr(65 + i)
            dogru = 1 if harf == dogru_harf else 0
            cur.execute("""
                INSERT INTO Secenekler (SoruId, Aciklama, Status, Ogr_select)
                VALUES (?, ?, ?, ?)
            """, soru_id, secenek, dogru, 0)

        cur.execute("INSERT INTO SinavSorulari (SinavId, SoruId) VALUES (?, ?)", sinav_id, soru_id)
        self.conn.commit()
        messagebox.showinfo("Başarılı", "Soru kaydedildi.")

    def show_yapilmis_odevler(self):
        self.clear()
        ttk.Label(self.content_frame, text="Sınıf Seçimi", font=self.title_font).pack(pady=20)

        self.sinif_var = tk.StringVar()
        self.sinif_cb = ttk.Combobox(self.content_frame, textvariable=self.sinif_var, state="readonly", width=30)
        self.sinif_cb.pack(pady=10)

        cur = self.conn.cursor()
        cur.execute("SELECT Id, Kodu FROM Siniflar")
        self.siniflar = cur.fetchall()
        self.sinif_cb['values'] = [s[1] for s in self.siniflar]

        ttk.Button(self.content_frame, text="Listele", command=self.listele_odev_yapan_ogrenciler).pack(pady=10)
        ttk.Button(self.content_frame, text="Geri", command=self.show_odev_menu).pack(pady=10)

    def listele_odev_yapan_ogrenciler(self):
        self.clear()

        sinif_kodu = self.sinif_var.get()
        sinif_id = next((s[0] for s in self.siniflar if s[1] == sinif_kodu), None)
        if not sinif_id:
            messagebox.showerror("Hata", "Lütfen bir sınıf seçin.")
            return

        ttk.Label(self.content_frame, text=f"{sinif_kodu} Sınıfında Ödevi Tamamlayanlar", font=self.title_font).pack(
            pady=10)

        self.ogrenci_tree = ttk.Treeview(self.content_frame, columns=("Ad", "SınavId", "Tarih", "Puan"),
                                         show="headings")
        self.ogrenci_tree.heading("Ad", text="Öğrenci Adı")
        self.ogrenci_tree.heading("SınavId", text="Sınav ID")
        self.ogrenci_tree.heading("Tarih", text="Tarih")
        self.ogrenci_tree.heading("Puan", text="Puan")
        self.ogrenci_tree.pack(fill="both", expand=True, padx=10, pady=10)

        cur = self.conn.cursor()
        cur.execute("""
            SELECT k.Id, k.KullaniciAdi, s.Id, s.Tarih, r.ToplamPuan
            FROM SinavSonuclari r
            JOIN Kullanicilar k ON r.KullaniciId = k.Id
            JOIN Sinavlar s ON r.SinavId = s.Id
            WHERE k.SinifId = ?
        """, sinif_id)

        for row in cur.fetchall():
            self.ogrenci_tree.insert('', 'end', values=(row[1], row[2], row[3], row[4]),
                                     tags=(str(row[2]), str(row[0])))

        self.ogrenci_tree.bind("<Double-1>", lambda e: self.show_ogrenci_sonuc_detayi())
        ttk.Button(self.content_frame, text="Geri", command=self.show_yapilmis_odevler).pack(pady=10)

    def show_ogrenci_sonuc_detayi(self):
        selected = self.ogrenci_tree.selection()
        if not selected:
            return
        item = self.ogrenci_tree.item(selected[0])
        sinav_id = int(item['tags'][0])
        ogrenci_id = int(item['tags'][1])

        win = tk.Toplevel(self.root)
        win.title("Sınav Özeti")
        win.geometry("500x300")

        ttk.Label(win, text="Sınav Sonucu", font=self.title_font).pack(pady=20)

        tree = ttk.Treeview(win, columns=("Doğru", "Yanlış", "Puan"), show="headings", height=1)
        tree.heading("Doğru", text="Toplam Doğru")
        tree.heading("Yanlış", text="Toplam Yanlış")
        tree.heading("Puan", text="Toplam Puan")
        tree.pack(pady=10)

        cur = self.conn.cursor()
        cur.execute("""
            SELECT ToplamDogrular, ToplamYanlislar, ToplamPuan
            FROM SinavSonuclari
            WHERE KullaniciId = ? AND SinavId = ?
        """, ogrenci_id, sinav_id)

        sonuc = cur.fetchone()
        if sonuc:
            tree.insert('', 'end', values=sonuc)


if __name__ == '__main__':
    import sys
    root = tk.Tk()

    if len(sys.argv) >= 2:
        giris_yapmis_kullanici_id = int(sys.argv[1])
    else:
        giris_yapmis_kullanici_id = None  # test için elle öğretmen seçimi

    app = OdevApp(root, giris_yapmis_kullanici_id)
    root.mainloop()

