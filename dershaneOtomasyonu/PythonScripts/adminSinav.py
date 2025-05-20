import threading
import tkinter as tk
from tkinter import ttk, messagebox
from tkinter.font import Font
from ctypes import windll
import google.generativeai as genai
import pyodbc
import datetime
import json
import os
from dotenv import load_dotenv
import sys

# DPI düzeltme (bulanıklığı giderir)
windll.shcore.SetProcessDpiAwareness(1)

load_dotenv()
GEMINI_API_KEY = os.getenv("GEMINI_API_KEY")
genai.configure(api_key=GEMINI_API_KEY)
model = genai.GenerativeModel("gemini-1.5-flash")


# Tema renkleri
THEME = {
    "bg": "#f9f9ff",
    "fg": "#003366",
    "accent": "#0055cc",
    "button_bg": "#e6f0ff",
    "button_fg": "#003366",
    "button_hover": "#ccf2e6",
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

class SinavApp:
    def __init__(self, root, giris_yapmis_kullanici_id=None):
        self.root = root
        self.root.title("Sınav Uygulaması")
        self.root.geometry("800x600")
        self.conn = get_db_connection()

        self.giris_yapmis_kullanici_id = giris_yapmis_kullanici_id  # 👈 EKLENDİ

        # Admin kontrolü
        if not self.kullanici_admin_mi(self.giris_yapmis_kullanici_id):
            messagebox.showerror("Yetkisiz Giriş", "Bu ekrana yalnızca admin kullanıcılar erişebilir.")
            root.destroy()
            return

        self.title_font = Font(family='Segoe UI', size=18, weight='bold')
        self.font = Font(family='Segoe UI', size=12)

        # Stil ayarları
        self.style = ttk.Style()
        self.style.theme_use("clam")
        self.style.configure("TFrame", background=THEME["bg"])
        self.style.configure("TLabel", background=THEME["bg"], foreground=THEME["fg"], font=self.font)
        self.style.configure("TButton", background=THEME["button_bg"], foreground=THEME["button_fg"],
                             font=self.font, padding=8, relief="flat", borderwidth=0)
        self.style.map("TButton", background=[("active", THEME["accent"])], foreground=[("active", "white")])
        self.style.configure("TEntry", fieldbackground=THEME["bg"], foreground=THEME["fg"], insertcolor=THEME["fg"])
        self.style.configure("TCombobox", fieldbackground=THEME["bg"], foreground=THEME["fg"], background=THEME["bg"])

        self.show_main_menu()

    def kullanici_admin_mi(self, kullanici_id):
        try:
            cur = self.conn.cursor()
            cur.execute("SELECT RolId FROM Kullanicilar WHERE Id = ?", (kullanici_id,))
            row = cur.fetchone()
            return row and row[0] == 1  # sadece RolId = 1 olanlar admin
        except:
            return False

    def show_main_menu(self):
        self.clear()
        ttk.Label(self.root, text="Hoş Geldiniz", font=self.title_font).pack(pady=50)
        ttk.Button(self.root, text="Sınav Tanımla", command=self.show_exam_form).pack(pady=10)
        ttk.Button(self.root, text="Oluşturulmuş Sınavları Görüntüle", command=self.show_exam_list).pack(pady=10)

    def show_exam_form(self):
        self.clear()
        ttk.Label(self.root, text="Sınav Tanımlama Ekranı", font=self.title_font).pack(pady=20)

        form_frame = ttk.Frame(self.root)
        form_frame.pack(pady=20)

        ttk.Label(form_frame, text="Sınav Adı:").grid(row=0, column=0, sticky="e", padx=5, pady=5)
        self.sinav_adi_var = tk.StringVar()
        ttk.Entry(form_frame, textvariable=self.sinav_adi_var).grid(row=0, column=1, pady=5)

        ttk.Label(form_frame, text="Sınav Kategorisi:").grid(row=1, column=0, sticky="e", padx=5, pady=5)
        self.kategori_var = tk.StringVar()
        self.kategori_cb = ttk.Combobox(form_frame, textvariable=self.kategori_var, state="readonly")
        self.kategori_cb.grid(row=1, column=1, pady=5)
        self.kategori_cb.bind("<<ComboboxSelected>>", self.update_sure)

        kategoriler_listesi = self.get_categories()
        self.kategoriler = {k['Adi']: k['Id'] for k in kategoriler_listesi}
        self.kategori_cb['values'] = list(self.kategoriler.keys())
        self.kategori_sureleri = {k['Adi']: k['VarsayilanSure'] for k in kategoriler_listesi}

        ttk.Label(form_frame, text="Süre (dk):").grid(row=2, column=0, sticky="e", padx=5, pady=5)
        self.sure_var = tk.StringVar()
        ttk.Entry(form_frame, textvariable=self.sure_var, state='readonly').grid(row=2, column=1, pady=5)

        ttk.Label(form_frame, text="Sınıf:").grid(row=3, column=0, sticky="e", padx=5, pady=5)
        self.sinif_var = tk.StringVar()
        self.sinif_cb = ttk.Combobox(form_frame, textvariable=self.sinif_var, state="readonly")
        self.sinif_cb.grid(row=3, column=1, pady=5)
        self.siniflar = self.get_classes()
        self.sinif_cb['values'] = [s['Kodu'] for s in self.siniflar]

        ttk.Button(self.root, text="Sınavı Kaydet", command=self.save_exam).pack(pady=20)

    def update_sure(self, event):
        selected = self.kategori_var.get()
        self.sure_var.set(self.kategori_sureleri.get(selected, ""))
        kategori = next((k for k in self.get_categories() if k["Adi"] == selected), None)
        if kategori:
            self.sure_var.set(kategori["VarsayilanSure"])

    def get_categories(self):
        cur = self.conn.cursor()
        cur.execute("SELECT Id, Adi, VarsayilanSure FROM SinavKategorileri WHERE Id IN (4, 5, 6, 10)")
        rows = cur.fetchall()
        return [{'Id': row[0], 'Adi': row[1], 'VarsayilanSure': row[2]} for row in rows]

    def get_classes(self):
        cur = self.conn.cursor()
        cur.execute("SELECT Id, Kodu FROM Siniflar")
        rows = cur.fetchall()
        return [{'Id': row[0], 'Kodu': row[1]} for row in rows]

    def save_exam(self):
        adi = self.sinav_adi_var.get()
        kategori_adi = self.kategori_var.get()
        tarih = datetime.datetime.now()
        olusturucu_id = self.giris_yapmis_kullanici_id or 2  # 👈 TEST için fallback

        kategori_id = self.kategoriler.get(kategori_adi)
        sure = self.sure_var.get()
        sinif_adi = self.sinif_var.get()
        sinif_id = next((s['Id'] for s in self.siniflar if s['Kodu'] == sinif_adi), None)

        if not adi or not kategori_id or not sure:
            messagebox.showerror("Hata", "Lütfen tüm alanları doldurun.")
            return

        cur = self.conn.cursor()
        cur.execute("""
            INSERT INTO Sinavlar (Adi, SinavKategoriId, Tarih, Sure, OlusturucuId, Atanan_Sinif)
            VALUES (?, ?, ?, ?, ?, ?)
        """, (adi, kategori_id, tarih, sure, olusturucu_id, sinif_id))
        self.conn.commit()

        sinav_id = cur.execute("SELECT @@IDENTITY").fetchval()

        messagebox.showinfo("Başarılı", "Sınav başarıyla tanımlandı!")
        self.show_question_setup(sinav_id, kategori_id)

    def show_question_setup(self, sinav_id, kategori_id):
        self.clear()
        ttk.Label(self.root, text="Soru Oluşturma Ekranı", font=self.title_font).pack(pady=20)

        cur = self.conn.cursor()
        cur.execute("SELECT SoruSayisi FROM SinavKategorileri WHERE Id = ?", kategori_id)
        soru_sayisi = cur.fetchone()[0]

        ttk.Label(self.root, text=f"Oluşturulacak Soru Sayısı: {soru_sayisi}", font=self.font).pack(pady=5)

        ttk.Button(self.root, text="Soruları Oluştur ve Kaydet",
                   command=lambda: self.generate_questions(sinav_id, soru_sayisi)).pack(pady=10)

        ttk.Button(self.root, text="Soruları Veritabanına Kaydet",
                   command=lambda: self.sorulari_veritabanina_kaydet(kategori_id, sinav_id)).pack(pady=10)

        ttk.Button(self.root, text="Ana Menü", command=self.show_main_menu).pack(pady=10)

    def generate_questions(self, sinav_id, soru_sayisi):
        def worker():
            progress_frame = ttk.Frame(self.root)
            progress_frame.pack(pady=100)

            ttk.Label(progress_frame, text="Sorular oluşturuluyor, lütfen bekleyin...", font=self.title_font).pack(
                pady=10)
            progress = ttk.Progressbar(progress_frame, mode='indeterminate', length=300)
            progress.pack(pady=10)
            progress.start()
            self.root.update()

            try:
                kategori_adi = self.kategori_var.get()
                kategori_id = self.kategoriler.get(kategori_adi)
                secenek_sayisi = 4

                cur = self.conn.cursor()
                cur.execute("SELECT DISTINCT Adi FROM SinavDersleri WHERE SinavKategoriId = ?", (kategori_id,))
                secilen_dersler = [row[0] for row in cur.fetchall()]

                if not secilen_dersler:
                    messagebox.showerror("Hata", "Bu sınav kategorisine ait tanımlı ders bulunamadı.")
                    return

                ders_soru_adet = soru_sayisi // len(secilen_dersler)
                dagilim = [ders_soru_adet] * len(secilen_dersler)
                kalan = soru_sayisi - sum(dagilim)
                for i in range(kalan):
                    dagilim[i] += 1

                prompt = (
                    f"Sen bir eğitim uzmanısın ve {kategori_adi} sınavı için AI destekli çoktan seçmeli sorular hazırlıyorsun.\n"
                    f"Her ders için aşağıda belirtilen konu dağılımına göre ve her biri 4 şıklı olacak şekilde soru üret.\n"
                    f"Her soru şu formatta olmalı:\n"
                    f"Soru: [metin]\n"
                    f"A) [şık A]\nB) [şık B]\nC) [şık C]\nD) [şık D]\n"
                    f"Doğru: [harf]\nZorluk: [1-5 arası zorluk seviyesi]\nKonu: [konu adı]\nDers: [ders adı]\n\n"
                )

                for ders_adi, adet in zip(secilen_dersler, dagilim):
                    cur.execute("""
                        SELECT k.Ad FROM SinavDersKonulari k
                        JOIN SinavDersleri d ON d.Id = k.SinavDersId
                        WHERE d.SinavKategoriId = ? AND d.Adi = ?
                    """, (kategori_id, ders_adi))

                    konular = [row[0] for row in cur.fetchall()]
                    if not konular:
                        print(f"[Uyarı] '{ders_adi}' dersi için konu bulunamadı, atlandı.")
                        continue

                    konu_text = ", ".join(konular)
                    prompt += (
                        f"Ders: {ders_adi}\n"
                        f"Toplam Soru: {adet}\n"
                        f"Konular: {konu_text}\n"
                        f"Sorular, konulara homojen dağılmalı ve konu bilgisi içermeli.\n\n"
                    )

                prompt += (
                    "Lütfen toplam tam olarak "
                    f"{soru_sayisi} adet soru üret. Eksik veya fazla üretim yapma. "
                    "Her soruda mutlaka konu ve ders adı yer almalı. Format dışına çıkma! Kaç adet soru istendiyse o kadar soru üret. Bolca vaktin var, soru sayısı çok olsa bile "
                    "o soruların hepsini üret. Örneğin 100 soru istendiyse 100 adet soru üret. Veya 180 soru istenmişse, tamı tamına 180 soru üret. Ne eksik ne fazla olsun. Unutma, acele"
                    "etmene gerek yok. İstenilen formatta, istenilen soru sayısı kadar soru üretmen gerekiyor. Hızlı olmana gerek yok. İhtiyacın kadar vakte sahipsin. "
                    "Soru sayısı eksik olmamalı! Buna çok dikkat ederek soruları oluştur, soru adedini eksik üretme!"
                )

                response = model.generate_content(prompt)
                lines = response.text.strip().splitlines()

                questions = []
                current = {}
                secenek_harfleri = ["A", "B", "C", "D"]

                for line in lines:
                    line = line.strip()
                    if line.startswith("Soru:"):
                        if current:
                            questions.append(current)
                        current = {"question": line[5:].strip(), "options": [], "answer": "", "difficulty": 1,
                                   "topic": "", "lesson": ""}
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

                # Soru verilerini modüllere ayır (ders bazlı)
                modul_dict = {}
                for q in questions:
                    ders_adi = q.get("lesson", "Genel")
                    if ders_adi not in modul_dict:
                        modul_dict[ders_adi] = {"name": ders_adi, "questions": []}
                    modul_dict[ders_adi]["questions"].append(q)

                modules = list(modul_dict.values())

                with open("question_regist.json", "w", encoding="utf-8") as f:
                    json.dump({"modules": modules}, f, ensure_ascii=False, indent=4)

                self.sorulari_veritabanina_kaydet(kategori_id, sinav_id)
                messagebox.showinfo("Başarılı",
                                    f"{sum(len(m['questions']) for m in modules)} soru oluşturuldu ve veritabanına kaydedildi.")

            except Exception as e:
                import traceback
                traceback.print_exc()
                messagebox.showerror("Hata", f"Soru üretimi sırasında hata oluştu:\n{e}")

            finally:
                progress.stop()
                progress_frame.destroy()
                self.root.update()

        threading.Thread(target=worker).start()

    def sorulari_veritabanina_kaydet(self, kategori_id, sinav_id):
        try:
            with open("question_regist.json", "r", encoding="utf-8") as f:
                data = json.load(f)
            modules = data.get("modules", [])  # ✅ doğru alan
            questions = []
            for m in modules:
                questions.extend(m.get("questions", []))  # ✅ tüm soruları topla
        except Exception as e:
            messagebox.showerror("Hata", f"JSON dosyası okunamadı: {e}")
            return

        cur = self.conn.cursor()

        for q in questions:
            metin = q.get("question", "")
            yildiz = q.get("difficulty", 1)
            konu_adi = q.get("topic", "")
            ders_adi = q.get("lesson", "")

            # Konu ID'sini al
            cur.execute("""
                    SELECT k.Id FROM SinavDersKonulari k
                    JOIN SinavDersleri d ON k.SinavDersId = d.Id
                    WHERE k.Ad = ? AND d.Adi = ? AND d.SinavKategoriId = ?
                """, konu_adi, ders_adi, kategori_id)

            konu_row = cur.fetchone()
            if not konu_row:
                print(f"Konu bulunamadı: {konu_adi} / {ders_adi}")
                continue
            konu_id = konu_row[0]

            # Ders ID'sini al (Ders sütunu int bekliyor)
            cur.execute("""
                    SELECT Id FROM SinavDersleri
                    WHERE Adi = ? AND SinavKategoriId = ?
                """, ders_adi, kategori_id)
            ders_row = cur.fetchone()
            if not ders_row:
                print(f"Ders bulunamadı: {ders_adi}")
                continue
            ders_id = ders_row[0]

            # Sorular tablosuna ekle
            cur.execute("""
                    INSERT INTO Sorular (SoruMetni, SinifSeviyeId, YildizSeviyesi, SinavDersKonuId, SecenekSayisi, SinavId, Ders)
                    VALUES (?, ?, ?, ?, ?, ?, ?)
                """, metin, 1, yildiz, konu_id, len(q["options"]), sinav_id, ders_id)

            soru_id = cur.execute("SELECT @@IDENTITY").fetchval()

            # Secenekler tablosuna ekle
            for idx, secenek in enumerate(q["options"]):
                is_true = 1 if chr(65 + idx) == q.get("answer", "") else 0
                cur.execute("""
                        INSERT INTO Secenekler (SoruId, Aciklama, Status, Ogr_select)
                        VALUES (?, ?, ?, ?)
                    """, soru_id, secenek, is_true, 0)

            # SinavSorulari tablosuna ekle
            cur.execute("""
                    INSERT INTO SinavSorulari (SinavId, SoruId)
                    VALUES (?, ?)
                """, sinav_id, soru_id)

        self.conn.commit()
        messagebox.showinfo("Başarılı", "Sorular veritabanına başarıyla kaydedildi.")

    def clear(self):
        for widget in self.root.winfo_children():
            widget.destroy()

    def show_exam_list(self):
        self.clear()
        ttk.Label(self.root, text="Tanımlanmış Sınavlar", font=self.title_font).pack(pady=20)

        tree = ttk.Treeview(self.root,
                            columns=("Adi", "Kategori", "Süre", "Tarih", "Oluşturucu", "Sınıf", "SoruSayisi"),
                            show='headings')
        tree.heading("Adi", text="Sınav Adı")
        tree.heading("Kategori", text="Kategori")
        tree.heading("Süre", text="Süre (dk)")
        tree.heading("Tarih", text="Tarih")
        tree.heading("Oluşturucu", text="Oluşturan Kişi")
        tree.heading("Sınıf", text="Sınıf")
        tree.heading("SoruSayisi", text="Soru Sayısı")

        tree.pack(fill="both", expand=True, padx=10, pady=10)

        cur = self.conn.cursor()
        cur.execute("""
            SELECT s.Id, s.Adi, k.Adi, s.Sure, s.Tarih, u.KullaniciAdi, c.Kodu,
                   (SELECT COUNT(*) FROM Sorular WHERE SinavId = s.Id) as SoruSayisi
            FROM Sinavlar s
            JOIN SinavKategorileri k ON s.SinavKategoriId = k.Id
            JOIN Kullanicilar u ON s.OlusturucuId = u.Id
            JOIN Siniflar c ON s.Atanan_Sinif = c.Id
        """)
        rows = cur.fetchall()
        for row in rows:
            tree.insert('', 'end', values=row[1:])  # row[1:] = Adi, Kategori, Süre, Tarih, Kullanıcı, Sınıf, SoruSayisi

        ttk.Button(self.root, text="Sınavı Sil", command=lambda: self.delete_exam(tree)).pack(pady=10)
        ttk.Button(self.root, text="Ana Menü", command=self.show_main_menu).pack(pady=10)

    def delete_exam(self, tree):
        selected = tree.selection()
        if not selected:
            messagebox.showwarning("Uyarı", "Lütfen silmek için bir sınav seçin.")
            return

        item = tree.item(selected[0])
        sinav_adi = item['values'][0]  # Sınav adı
        cur = self.conn.cursor()
        cur.execute("SELECT Id FROM Sinavlar WHERE Adi = ?", sinav_adi)
        row = cur.fetchone()
        if not row:
            messagebox.showerror("Hata", "Sınav ID bulunamadı.")
            return

        sinav_id = row[0]

        if not messagebox.askyesno("Onay", "Bu sınavı ve ilişkili soruları silmek istediğinize emin misiniz?"):
            return

        cur.execute("DELETE FROM Secenekler WHERE SoruId IN (SELECT Id FROM Sorular WHERE SinavId = ?)", sinav_id)
        cur.execute("DELETE FROM SinavSorulari WHERE SinavId = ?", sinav_id)
        cur.execute("DELETE FROM Sorular WHERE SinavId = ?", sinav_id)
        cur.execute("DELETE FROM Sinavlar WHERE Id = ?", sinav_id)
        self.conn.commit()

        messagebox.showinfo("Başarılı", "Sınav ve ilişkili sorular silindi.")
        self.show_exam_list()

    def clear(self):
        for widget in self.root.winfo_children():
            widget.destroy()

if __name__ == '__main__':
  import sys
  # -------------------- ARGÜMAN KONTROLÜ --------------------
  if len(sys.argv) < 2:
      print("Kullanım: python adminSinav.py <adminKullaniciId>")
      sys.exit(1)

  # C# tarafından gelen admin ID
  root = tk.Tk()

  if len(sys.argv) >= 2:
      giris_yapmis_kullanici_id = int(sys.argv[1])
  else:
      giris_yapmis_kullanici_id = 38  # test için default
  giris_yapmis_kullanici_id = int(sys.argv[1])

  app = SinavApp(root, giris_yapmis_kullanici_id)
  root.mainloop()