# -*- coding: utf-8 -*-
import tkinter as tk
from tkinter import ttk, messagebox
from tkinter.font import Font
from ctypes import windll
import pyodbc
from dotenv import load_dotenv
import os
import google.generativeai as genai
import threading
import json
import logging
import tkinterweb
from tkinterweb import HtmlFrame
import sys, json

# Komut satırından JSON veriyi oku # added by asu
selected_user_id   = None
selected_sinif_id  = None
if len(sys.argv) > 1:
    try:
        data = json.loads(sys.argv[1])
        selected_user_id  = data.get("KullaniciId")
        selected_sinif_id = data.get("SinifId")
    except Exception as e:
        print("JSON parse hatası:", e)

# Windows’ta varsayılan kodlamayı UTF-8’e çeviriyoruz:
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")
    sys.stderr.reconfigure(encoding="utf-8", errors="replace")
else:
    # Python <3.7 fallback
    sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", errors="replace")
    sys.stderr = io.TextIOWrapper(sys.stderr.buffer, encoding="utf-8", errors="replace")

# DPI ayarları
windll.shcore.SetProcessDpiAwareness(1)
load_dotenv()
GEMINI_API_KEY = os.getenv("GEMINI_API_KEY")
genai.configure(api_key=GEMINI_API_KEY)
model = genai.GenerativeModel('gemini-1.5-flash')


def get_db_connection():
    server = r'localhost\SQLEXPRESS'
    database = 'DERSHANE1'
    return pyodbc.connect(
        f'DRIVER={{ODBC Driver 17 for SQL Server}};'
        f'SERVER={server};'
        f'DATABASE={database};'
        f'Trusted_Connection=yes;'
    )


def save_to_json(data, filename='quiz_results.json'):
    # StringVar gibi serileştirilemeyen objeleri dönüştür
    def serialize(obj):
        if isinstance(obj, tk.StringVar):
            return obj.get()
        elif isinstance(obj, dict):
            return {k: serialize(v) for k, v in obj.items()}
        elif isinstance(obj, list):
            return [serialize(i) for i in obj]
        return obj

    serializable_data = serialize(data)

    with open(filename, 'w', encoding='utf-8') as f:
        json.dump(serializable_data, f, ensure_ascii=False, indent=4)


# Temalar
THEMES = {
    "light": {
        "bg": "#f9f9ff",
        "fg": "#003366",
        "accent": "#0055cc",
        "button_bg": "#e6f0ff",
        "button_fg": "#003366",
        "button_hover": "#ccf2e6",
        "optik_bg": "#f0f4ff",
        "highlight": "#dce9ff"
    },
    "dark": {
        "bg": "#121212",
        "fg": "#ffffff",
        "accent": "#00cc66",
        "button_bg": "#1e1e1e",
        "button_fg": "#00cc66",
        "button_hover": "#145c33",
        "optik_bg": "#1c1c1c",
        "highlight": "#2d2d2d"
    }
}


class ThemeManager:
    def __init__(self, app):
        self.app = app
        self.current = "light"
        self._style = ttk.Style()
        self._configure_base_styles()
        self._is_generating_questions = False  # Soru oluşturma durumunu takip etmek için

    def _configure_base_styles(self):
        """Temel stil ayarlarını yapılandır"""
        self._style.configure(".", font=('Segoe UI', 10))
        self._style.configure("TFrame", background=THEMES[self.current]['bg'])
        self._style.configure("TLabel",
                              background=THEMES[self.current]['bg'],
                              foreground=THEMES[self.current]['fg'],
                              font=('Segoe UI', 10))

    def apply(self, theme_name: str) -> None:
        """Tema değişikliğini uygula"""
        if theme_name not in THEMES:
            logging.error(f"Geçersiz tema: {theme_name}")
            return

        try:
            # Soru oluşturma durumunu kontrol et
            if hasattr(self.app, 'generating') and self.app.generating:
                self._is_generating_questions = True
                logging.info("Soru oluşturma sırasında tema değişikliği yapılıyor")
                return

            self.current = theme_name
            theme = THEMES[theme_name]

            # Ana pencere temasını güncelle
            self.app.root.configure(bg=theme['bg'])

            # Temel stilleri güncelle
            self._configure_base_styles()

            # Buton stilleri
            self._style.configure("TButton",
                                  background=theme['button_bg'],
                                  foreground=theme['button_fg'],
                                  padding=8,
                                  relief="flat",
                                  font=('Segoe UI', 10),
                                  borderwidth=0)

            self._style.map("TButton",
                            background=[("active", theme['accent'])],
                            foreground=[("active", 'white')])

            # Radio buton stilleri
            active_bg = '#145c33' if theme_name == 'dark' else theme['highlight']
            active_fg = 'white' if theme_name == 'dark' else theme['fg']

            self._style.configure('Question.TRadiobutton',
                                  background=theme['bg'],
                                  foreground=theme['fg'],
                                  font=('Arial', 10),
                                  relief='flat',
                                  padding=3)

            self._style.map('Question.TRadiobutton',
                            background=[
                                ('active', active_bg),
                                ('selected', theme['highlight'])
                            ],
                            foreground=[
                                ('active', active_fg),
                                ('selected', theme['accent'])
                            ])

            # Diğer widget stilleri
            self._style.configure("TLabelframe",
                                  background=theme['bg'],
                                  foreground=theme['fg'])

            self._style.configure("TLabelframe.Label",
                                  background=theme['bg'],
                                  foreground=theme['accent'])

            self._style.configure('TCombobox',
                                  fieldbackground=theme['bg'],
                                  foreground=theme['fg'],
                                  background=theme['bg'])

            self._style.configure('TEntry',
                                  fieldbackground=theme['bg'],
                                  foreground=theme['fg'],
                                  insertcolor=theme['fg'])

            self._style.configure('Optik.TFrame', background=theme['optik_bg'])
            self._style.configure('Optik.TLabel',
                                  background=theme['accent'],
                                  foreground=theme['bg'])

            # Mevcut ekranı yenile
            if hasattr(self.app, 'current_screen'):
                self._refresh_current_screen()

            # Özel stilleri güncelle
            self.app.configure_styles()

            logging.info(f"Tema başarıyla değiştirildi: {theme_name}")

        except Exception as e:
            logging.error(f"Tema değiştirme hatası: {str(e)}")
            messagebox.showerror("Hata", "Tema değiştirilirken bir hata oluştu.")

    def _refresh_current_screen(self) -> None:
        """Mevcut ekranı yenile"""
        try:
            # Eğer soru oluşturuluyorsa ekranı yenileme
            if self._is_generating_questions:
                return

            if self.app.current_screen == 'results':
                if self.app.mode == 'test':
                    self.app.show_test_results()
                else:
                    self.app.show_results()
            elif self.app.current_screen == 'exam':
                self.app.show_exam_interface()
            elif self.app.current_screen == 'main':
                self.app.show_main_menu()
            elif self.app.current_screen == 'test' and self.app.test_questions:
                self.app.show_test_question_page()
            elif self.app.current_screen == 'test':
                self.app.show_test_setup()

        except Exception as e:
            logging.error(f"Ekran yenileme hatası: {str(e)}")
            messagebox.showerror("Hata", "Ekran yenilenirken bir hata oluştu.")

    def set_generating_state(self, is_generating: bool) -> None:
        """Soru oluşturma durumunu ayarla"""
        self._is_generating_questions = is_generating


class ScrollableFrame(ttk.Frame):
    def __init__(self, container, theme_name="light", *args, **kwargs):
        super().__init__(container, *args, **kwargs)

        self.root = container # added by asu

        self.theme_name = theme_name

        # Canvas ve scrollbar oluştur
        self.canvas = tk.Canvas(self,
                                highlightthickness=0,
                                bg=THEMES[theme_name]['bg'])
        self.scrollbar = ttk.Scrollbar(self, orient='vertical', command=self.canvas.yview)

        # Inner frame oluştur
        self.inner = ttk.Frame(self.canvas, style='Custom.TFrame')

        # Canvas'ı yapılandır
        self.canvas.configure(yscrollcommand=self.scrollbar.set)

        # Widget'ları yerleştir
        self.canvas.pack(side='left', fill='both', expand=True)
        self.scrollbar.pack(side='right', fill='y')

        # Inner frame'i canvas'a ekle
        self.canvas_frame = self.canvas.create_window((0, 0), window=self.inner, anchor='nw')

        # Event binding'leri
        self.inner.bind('<Configure>', self._on_frame_configure)
        self.canvas.bind('<Configure>', self._on_canvas_configure)
        # sadece canvas aktifken wheel olayını al
        self.canvas.bind('<Leave>', lambda e: self.root.focus_set())
        self.canvas.bind('<Enter>', lambda e: self.canvas.focus_set())
        self.canvas.bind('<MouseWheel>', self._on_mousewheel)


        # Tema değişikliği için event binding
        self.bind('<<ThemeChanged>>', self._on_theme_change)

    def _on_frame_configure(self, event=None):
        """Inner frame boyutu değiştiğinde scroll bölgesini güncelle"""
        self.canvas.configure(scrollregion=self.canvas.bbox('all'))

    def _on_canvas_configure(self, event):
        """Canvas boyutu değiştiğinde inner frame genişliğini güncelle"""
        self.canvas.itemconfig(self.canvas_frame, width=event.width)

    def _on_mousewheel(self, event):
        """Mouse tekerleği ile kaydırma"""
        self.canvas.yview_scroll(int(-1 * (event.delta / 120)), 'units')

    def _on_theme_change(self, event=None):
        """Tema değiştiğinde canvas arkaplan rengini güncelle"""
        self.canvas.configure(bg=THEMES[self.theme_name]['bg'])

    def update_theme(self, theme_name):
        """Tema değişikliğini uygula"""
        if self.winfo_exists():  # Frame hala mevcutsa
            self.theme_name = theme_name
            self.canvas.configure(bg=THEMES[theme_name]['bg'])
            self.inner.configure(style='Custom.TFrame')


class QuestionApp:
    def __init__(self, root):
        self.root = root
        self.root.title('Star Soru Çözme Alanı')
        self.root.geometry('1366x768')
        self.style = ttk.Style()
        self.theme_manager = ThemeManager(self)
        ttk.Style().theme_use('clam')
        self.theme_manager.apply("light")

        self.selected_user_id = None
        self.selected_sinif_id = None
        self.kullanici_adi = None

        # eğer C# tarafı JSON göndermişse parse edelim:
        if len(sys.argv) > 1:
            try:
                data = json.loads(sys.argv[1])
                # JSON anahtarlarının birebir aynı olduğuna dikkat edin:
                self.selected_user_id  = data.get("KullaniciId")
                self.selected_sinif_id = data.get("SinifId")
            except json.JSONDecodeError:
                # eğer JSON değilse,
                # eski yönteme (manuel kullanıcı adı) gardiyanı:
                self.kullanici_adi = sys.argv[1]


        self.conn = get_db_connection()
        self.root.protocol("WM_DELETE_WINDOW", self.on_close)
        self.font = Font(family='Segoe UI', size=12)
        self.title_font = Font(family='Segoe UI', size=18, weight='bold')
        self.mode = None
        self.test_type = None
        self.modules = []
        self.current_module = 0
        self.selected_answers = {}
        self.timer_label = None
        self.remaining_sec = 0
        self.sf = None

        self.cat_var = tk.StringVar()
        self.ders_var = tk.StringVar()
        self.konu_var = tk.StringVar()
        self.num_entry = None
        self.test_questions = []
        self.test_selected_answers = {}

        self.style.configure('Custom.TFrame',
                             background=THEMES[self.theme_manager.current]['bg'])
        self.style.configure('Custom.TLabel',
                             background=THEMES[self.theme_manager.current]['bg'],
                             foreground=THEMES[self.theme_manager.current]['fg'])
        self.create_menu()
        self.show_main_menu()

    def on_close(self):
        if self.conn: self.conn.close()
        self.root.destroy()

    def refresh_current_screen(self):
        """Mevcut ekranı yenile"""
        # Sadece widget'ların stillerini güncelle
        theme = THEMES[self.theme_manager.current]

        # Tüm widget'ları güncelle
        for widget in self.root.winfo_children():
            if isinstance(widget, ttk.Frame):
                widget.configure(style='Custom.TFrame')
            elif isinstance(widget, tk.Label):
                widget.configure(bg=THEMES[self.theme_manager.current]['bg'],
                                 fg=THEMES[self.theme_manager.current]['fg'])
            elif isinstance(widget, ttk.Button):
                widget.configure(style='TButton')
            elif isinstance(widget, ttk.Radiobutton):
                widget.configure(style='Question.TRadiobutton')
            elif isinstance(widget, ttk.Combobox):
                widget.configure(style='TCombobox')
            elif isinstance(widget, ttk.Entry):
                widget.configure(style='TEntry')
            elif isinstance(widget, ttk.Labelframe):
                widget.configure(style='TLabelframe')

        # ScrollableFrame'i güncelle
        if hasattr(self, 'sf') and self.sf:
            self.sf.update_theme(self.theme_manager.current)

    def create_menu(self):
        menu_bar = tk.Menu(self.root)
        settings_menu = tk.Menu(menu_bar, tearoff=0)
        theme_submenu = tk.Menu(settings_menu, tearoff=0)
        theme_submenu.add_command(label="Açık Tema (Lacivert)", command=lambda: self.theme_manager.apply('light'))
        theme_submenu.add_command(label="Koyu Tema (Siyah/Yeşil)", command=lambda: self.theme_manager.apply('dark'))
        settings_menu.add_cascade(label="Tema Ayarları", menu=theme_submenu)
        menu_bar.add_cascade(label="Ayarlar", menu=settings_menu)
        self.root.config(menu=menu_bar)

    def clear(self):
        for w in self.root.winfo_children():
            if not isinstance(w, tk.Menu):
                w.destroy()

    def configure_styles(self):
        style = ttk.Style()
        theme = THEMES[self.theme_manager.current]

        # Mevcut stil ayarlarına ek olarak:
        style.configure('Custom.TFrame',
                        background=theme['bg'])
        style.configure('Custom.TLabel',
                        background=theme['bg'],
                        foreground=theme['fg'])

        style.configure('Optik.TRadiobutton',
                        width=3,
                        padding=5,
                        relief='flat',
                        font=('Arial', 9),
                        background=theme['optik_bg'],
                        foreground=theme['accent'])

        style.configure('Selected.TRadiobutton',
                        background=theme['highlight'],
                        relief='solid',
                        borderwidth=1)

        style.map('Optik.TRadiobutton',
                  background=[('active', theme['highlight'])],
                  foreground=[('selected', theme['accent']), ('!selected', theme['fg'])],
                  indicatorcolor=[('selected', theme['accent']), ('!selected', theme['fg'])])
        style.configure('Question.TRadioButton',
                        background=theme['bg'],
                        foreground=theme['fg'],
                        font=('Arial', 10))
        self.style.configure('Test.TRadiobutton',
                             background=THEMES[self.theme_manager.current]['bg'],
                             foreground=THEMES[self.theme_manager.current]['fg'],
                             font=('Arial', 10))

        self.style.map('Test.TRadiobutton',
                       background=[('active', THEMES[self.theme_manager.current]['highlight'])],
                       foreground=[('active', THEMES[self.theme_manager.current]['accent'])])

    def configure_mode(self, mode):
        self.mode = mode
        if mode == 'test':
            self.show_category_selection()
        else:
            self.show_exam_setup()

    def show_main_menu(self):
        self.clear()
        self.current_screen = 'main'
        ttk.Label(self.root, text='Star Soru Çözme Alanı', font=self.title_font).pack(pady=40)
        frame = ttk.Frame(self.root)
        frame.pack(pady=20)
        ttk.Button(frame, text='E-Sınav', command=lambda: self.show_exam_setup('exam'), width=20).grid(row=0, column=0,
                                                                                                       padx=10)
        ttk.Button(frame, text='Test', command=lambda: self.show_exam_setup('test'), width=20).grid(row=0, column=1,
                                                                                                    padx=10)

    def get_star_rating(self, difficulty):
        try:
            stars = '⭐' * max(1, min(int(difficulty), 5))  # min/max ile 1-5 aralığına sabitleriz
            return stars
        except:
            return ''

    # Öğrencinin test modülü giriş ekranı:
    def show_test_menu(self):
        self.clear()
        # Scrollable alanı tekrar oluştur
        self.sf = ScrollableFrame(self.root, theme_name=self.theme_manager.current)
        self.sf.pack(fill="both", expand=True)
        self.content_frame = self.sf.inner

        ttk.Label(self.content_frame, text="Test Modülü", font=self.title_font).pack(pady=30)

        ttk.Button(self.content_frame, text="📘 Ödevlerim", width=30, command=self.show_odevlerim).pack(pady=10)
        ttk.Button(self.content_frame, text="🧠 Test Çöz", width=30, command=self.show_category_selection).pack(pady=10)
        ttk.Button(self.content_frame, text="Ana Menü", width=30, command=self.show_main_menu).pack(pady=10)

    def show_exam_setup(self, mode='exam'): # updated by asu
        self.clear()
        self.current_screen = 'user_select'
        self.kullanicilar = self.get_users()

        # Eğer C# tarafından Id geldiyse, manuel seçim ekranını atla:
        if self.selected_user_id is not None and self.selected_sinif_id is not None:
            if mode == 'exam':
                return self.load_user_exams()
            else:
                return self.show_test_menu()


            # Buradan sonra normal seçim ekranına geçilsin

        # Aksi halde normal kullanıcı seçim ekranı göster
        ttk.Label(self.root, text="Kullanıcı Seçin", font=self.title_font).pack(pady=20)

        self.kullanici_var = tk.StringVar()
        self.kullanici_cb = ttk.Combobox(self.root, textvariable=self.kullanici_var, state="readonly")
        self.kullanici_cb.pack(pady=10)

        self.kullanicilar = self.get_users()
        self.kullanici_cb['values'] = [k['KullaniciAdi'] for k in self.kullanicilar]

        # Buradaki buton mode'a göre yönlendirecek:
        ttk.Button(self.root, text="Giriş Yap", command=lambda: self.on_kullanici_giris(mode)).pack(pady=10)
        ttk.Button(self.root, text="Geri", command=self.show_main_menu).pack(pady=10)

    def on_kullanici_giris(self, mode='exam'):
        secilen_kadi = self.kullanici_var.get()
        kullanici = next((k for k in self.kullanicilar if k['KullaniciAdi'] == secilen_kadi), None)

        if not kullanici:
            messagebox.showerror("Hata", "Lütfen geçerli bir kullanıcı seçin.")
            return

        self.selected_user_id = kullanici['Id']
        self.selected_sinif_id = kullanici['SinifId']

        if mode == 'exam':
            self.load_user_exams()
        else:
            self.show_test_menu()

    def show_odevlerim(self):
        self.clear()
        self.sf = ScrollableFrame(self.root, theme_name=self.theme_manager.current)
        self.sf.pack(fill="both", expand=True)
        self.content_frame = self.sf.inner

        ttk.Label(self.content_frame, text="📘 Ödevlerim", font=self.title_font).pack(pady=20)

        tree = ttk.Treeview(self.content_frame, columns=("Ad", "Tarih", "Süre", "Soru Sayısı"), show="headings",
                            height=12)
        tree.heading("Ad", text="📘 Sınav Adı")
        tree.heading("Tarih", text="📅 Tarih")
        tree.heading("Süre", text="⏱ Süre (dk)")
        tree.heading("Soru Sayısı", text="❓ Soru Sayısı")

        tree.column("Ad", anchor="center", width=300)
        tree.column("Tarih", anchor="center", width=150)
        tree.column("Süre", anchor="center", width=100)
        tree.column("Soru Sayısı", anchor="center", width=120)

        tree.pack(fill="both", expand=True, padx=20, pady=10)

        cur = self.conn.cursor()
        cur.execute("""
            SELECT s.Id, s.Adi, s.Tarih, s.Sure,
                   (SELECT COUNT(*) FROM Sorular WHERE SinavId = s.Id) AS SoruSayisi
            FROM Sinavlar s
            LEFT JOIN OgrenciSinavlari os ON s.Id = os.SinavId
            WHERE (os.KullaniciId = ? OR s.Atanan_Sinif = (
                SELECT SinifId FROM Kullanicilar WHERE Id = ?
            ))
            AND s.Id NOT IN (
                SELECT SinavId FROM OgrenciCevaplari WHERE KullaniciId = ?
            )
        """, (self.selected_user_id, self.selected_user_id, self.selected_user_id))

        for row in cur.fetchall():
            sinav_id, adi, tarih, sure, soru_sayisi = row
            tree.insert('', 'end', values=(adi, tarih, sure, soru_sayisi), tags=(sinav_id,))

        tree.bind("<Double-1>", lambda e: self.odev_coz_baslat(tree))

        frame = ttk.Frame(self.root)
        frame.pack(pady=20)
        ttk.Button(frame, text='Geri', command=lambda: self.show_exam_setup('test'), width=20).grid(row=0, column=1,
                                                                                                padx=10)

    def odev_coz_baslat(self, tree):
        secilen = tree.selection()
        if not secilen:
            return
        sinav_id = int(tree.item(secilen[0], "tags")[0])
        self.mode = 'test'
        self.sinav_id = sinav_id
        self.secilen_sinav_id = sinav_id  # ✅ Bu satır eklenecek

        self.load_test_questions(sinav_id)
        self.current_screen = 'test'
        self.show_test_question_page()

    def show_category_selection(self):
        self.clear()
        ttk.Label(self.root, text='Kategori Seçiniz (TYT/AYT)', font=self.title_font).pack(pady=20)
        self.cat_var = tk.StringVar()
        cat_cb = ttk.Combobox(self.root, textvariable=self.cat_var, state='readonly', values=self.get_categories())
        cat_cb.pack()
        # ileri buton
        ttk.Button(self.root, text='İleri', command=self.show_ders_selection).pack(pady=10)
        # Geri Buton
        ttk.Button(self.root, text='Geri', command=self.show_main_menu).pack(pady=10)

    def get_categories(self):
        cur = self.conn.cursor()
        cur.execute('SELECT Adi FROM SinavKategorileri')
        return [r[0] for r in cur]

    def show_ders_selection(self):
        cat = self.cat_var.get()
        if not cat: return messagebox.showerror('Hata', 'Kategori seçin')
        self.test_type = cat
        self.clear()
        ttk.Label(self.root, text=f'{cat}: Ders Seçiniz', font=self.title_font).pack(pady=20)
        self.ders_var = tk.StringVar()
        ders_cb = ttk.Combobox(self.root, textvariable=self.ders_var, state='readonly', values=self.get_dersler(cat))
        ders_cb.pack()
        # ileri buton
        ttk.Button(self.root, text='İleri', command=self.show_konu_selection).pack(pady=10)
        # geri Buton
        ttk.Button(self.root, text='Geri', command=self.show_category_selection).pack(pady=10)

    def get_dersler(self, kategori):
        cur = self.conn.cursor()
        cur.execute(
            'SELECT d.Adi FROM SinavDersleri d JOIN SinavKategorileri k ON d.SinavKategoriId=k.Id WHERE k.Adi=?',
            kategori
        )
        return [r[0] for r in cur]

    def show_konu_selection(self):
        self.mode = 'test'
        ders = self.ders_var.get()
        if not ders:
            messagebox.showerror('Hata', 'Ders seçin')
            return

        self.selected_ders = ders
        self.clear()
        ttk.Label(self.root, text=f'{ders}: Konu Seçiniz', font=self.title_font).pack(pady=20)

        self.konu_var = tk.StringVar()
        konu_cb = ttk.Combobox(self.root, textvariable=self.konu_var, state='readonly', values=self.get_konular(ders))
        konu_cb.pack()

        ttk.Label(self.root, text='Soru Sayısı:').pack(pady=5)
        self.num_entry = ttk.Entry(self.root)
        self.num_entry.insert(0, '4')
        self.num_entry.pack()

        ttk.Button(self.root, text='Soruları Getir', command=self.generate_questions).pack(pady=10)
        # geri Buton
        ttk.Button(self.root, text='Geri', command=self.show_ders_selection).pack(pady=10)

    def get_konular(self, ders):
        cur = self.conn.cursor()
        cur.execute(
            'SELECT k.Ad FROM SinavDersKonulari k JOIN SinavDersleri d ON k.SinavDersId = d.Id WHERE d.Adi = ?',
            ders
        )
        return [r[0] for r in cur]


    def update_test_optic(self, idx):
        """Test modülü için optik form güncelleme"""
        selected = self.test_selected_answers[idx].get()
        for i, opt in enumerate(['A', 'B', 'C', 'D']):
            btn = self.test_optik_circles[idx][i]
            if opt == selected:
                btn.state(['selected'])
            else:
                btn.state(['!selected'])

    def scroll_to_test_question(self, idx):
        """Test modülü için soruya kaydırma"""
        if hasattr(self, 'sf') and self.sf:
            self.sf.canvas.yview_moveto(idx / len(self.test_questions))

    def start_exam(self, sinav_id, kind):
        self.test_type = kind
        self.sinav_id = sinav_id

        # Veritabanından kategoriye ait dersleri çek
        cur = self.conn.cursor()
        cur.execute("""
            SELECT d.Adi FROM SinavDersleri d
            JOIN SinavKategorileri k ON d.SinavKategoriId = k.Id
            WHERE k.Adi = ?
        """, kind)
        ders_listesi = [row[0] for row in cur.fetchall()]

        # Modülleri dinamik oluştur (Her ders için varsayılan 5 soru)
        self.modules = [{'name': ders, 'num': 5} for ders in ders_listesi]

        self.clear()
        ttk.Label(self.root, text='Sorular oluşturuluyor...', font=self.title_font).pack(pady=200)
        self.root.update()

        self.generate_questions()

    def generate_questions(self):
        def worker():
            self.generating = True
            self.theme_manager.set_generating_state(True)

            try:
                self.num_questions = int(self.num_entry.get())
            except:
                self.num_questions = 4

            if self.mode == 'test':
                if not hasattr(self, 'selected_konu'):
                    self.selected_konu = self.konu_var.get()

                if not self.selected_konu:
                    messagebox.showerror('Hata', 'Lütfen bir konu seçin')
                    self.generating = False
                    self.theme_manager.set_generating_state(False)
                    return

                prompt = (
                    f"{self.test_type} {self.selected_ders} "
                    f"dersinin {self.selected_konu} konusundan {self.num_questions} adet test sorusu üret. "
                    "Her soru şu formatta olsun:\n\n"
                    "Soru 1: [soru metni]\nA) [şık A]\nB) [şık B]\nC) [şık C]\nD) [şık D]\n"
                    "Cevap: [doğru şık]\nAçıklama: [açıklama metni]\n\nLütfen bu formata kesinlikle uyun!"
                )

                try:
                    # Yükleniyor ekranı
                    self.clear()
                    progress_frame = ttk.Frame(self.root)
                    progress_frame.pack(pady=150)

                    ttk.Label(progress_frame, text='Sorular oluşturuluyor, lütfen bekleyin...',
                              font=self.title_font).pack(pady=10)

                    progress = ttk.Progressbar(progress_frame, mode='indeterminate', length=300)
                    progress.pack(pady=10)
                    progress.start()  # animasyonu başlat

                    self.root.update()

                    res = model.generate_content(prompt)
                    print("[DEBUG] AI çıktısı:\n", res.text)
                    self.test_questions = self.parse_ai_questions(res.text)

                    if not self.test_questions:
                        messagebox.showerror("Hata", "AI tarafından geçerli soru üretilemedi.")
                        self.generating = False
                        self.theme_manager.set_generating_state(False)
                        self.show_main_menu()
                        return

                    self.test_selected_answers = {
                        idx: tk.StringVar(value="") for idx in range(len(self.test_questions))
                    }

                    # GUI'ye geri dön
                    self.root.after(0, self.show_test_question_page)

                except Exception as e:
                    logging.error(f"[generate_questions] AI üretim hatası: {e}")
                    messagebox.showerror("Hata", "Soru oluşturulurken bir hata oluştu.")

                finally:
                    self.generating = False
                    self.theme_manager.set_generating_state(False)

        # Thread başlat
        threading.Thread(target=worker).start()

    def parse_ai_questions(self, text):
        import re

        questions = []
        current_q = {
            'question': '',
            'options': {},
            'answer': '',
            'explanation': '',
            'selected_answer': tk.StringVar(value="")
        }

        lines = text.split('\n')
        option_pattern = re.compile(r'^([A-Da-d])[\)|\.]\s*(.+)$')  # A) Şık | A. Şık

        for line in lines:
            line = line.strip()
            if line.lower().startswith("soru"):
                if current_q['question']:
                    questions.append(current_q)
                    current_q = {
                        'question': '',
                        'options': {},
                        'answer': '',
                        'explanation': '',
                        'selected_answer': tk.StringVar(value="")
                    }
                # örnek: Soru 1: ... → iki parçaya böl
                parts = line.split(':', 1)
                if len(parts) == 2:
                    current_q['question'] = parts[1].strip()
                else:
                    current_q['question'] = line

            elif option_pattern.match(line):
                opt, val = option_pattern.match(line).groups()
                current_q['options'][opt.upper()] = val.strip()

            elif line.lower().startswith("cevap"):
                current_q['answer'] = line.split(':', 1)[-1].strip().upper()

            elif line.lower().startswith("açıklama"):
                current_q['explanation'] = line.split(':', 1)[-1].strip()

        if current_q['question']:
            questions.append(current_q)

        # Temizlik
        valid_questions = []
        for q in questions:
            if q.get('question') and len(q.get('options', {})) >= 4:
                valid_questions.append(q)

        print(f"[DEBUG] parse_ai_questions sonucu: {len(valid_questions)} geçerli soru bulundu.")
        return valid_questions

    def show_test_question_page(self):
        self.clear()
        self.current_screen = 'test'

        # Ana frame oluştur
        main_frame = ttk.Frame(self.root)
        main_frame.pack(fill='both', expand=True)

        # Üst çubuk
        top = ttk.Frame(main_frame)
        top.pack(fill='x', padx=10, pady=5)

        # Sınavı tamamla butonu
        complete_btn = ttk.Button(top, text="Testi Tamamla", command=self.show_test_results)
        complete_btn.pack(side='right', padx=10)

        # Soru alanı ve Optik Form için container
        content_frame = ttk.Frame(main_frame)
        content_frame.pack(fill='both', expand=True)

        # Soru alanı (Scrollable)
        self.sf = ScrollableFrame(content_frame, theme_name=self.theme_manager.current)
        self.sf.pack(side='left', fill='both', expand=True)

        # Optik Form (Sağda)
        opt_frame = ttk.Frame(content_frame, style='Optik.TFrame')
        opt_frame.pack(side='right', fill='y', padx=10, pady=10)

        ttk.Label(opt_frame, text="Optik Form", style='Optik.TLabel').pack(pady=5)

        # Optik formu oluştur
        self.test_optik_circles = []
        for i in range(len(self.test_questions)):
            cell_frame = ttk.Frame(opt_frame, style='Optik.TFrame')
            cell_frame.pack(pady=2, fill='x')

            ttk.Label(cell_frame, text=str(i + 1), width=3).pack(side='left', padx=5)
            circles_frame = ttk.Frame(cell_frame)
            circles_frame.pack(side='left', fill='x', expand=True)

            circles = []
            for opt in ['A', 'B', 'C', 'D']:
                rb = ttk.Radiobutton(
                    circles_frame,
                    text=opt,
                    style='Optik.TRadiobutton',
                    value=opt,
                    variable=self.test_selected_answers[i],
                    command=lambda idx=i: self.update_test_optic(idx)
                )
                rb.pack(side='left', expand=True)
                circles.append(rb)
            self.test_optik_circles.append(circles)
            cell_frame.bind('<Button-1>', lambda e, idx=i: self.scroll_to_test_question(idx))

        # Soruları göster
        for idx, q in enumerate(self.test_questions):
            fr = ttk.LabelFrame(self.sf.inner, text=f"Soru {idx + 1}", padding=10)
            fr.pack(fill='x', pady=5)

            # Soru metni
            stars = self.get_star_rating(q.get('difficulty', 1))

            # Üst başlık satırı: Soru metni solda, yıldızlar sağda
            top_row = ttk.Frame(fr)
            top_row.pack(fill='x', pady=(0, 5))

            # Soru metni (sola yaslı)
            ttk.Label(top_row,
                      text=f"Soru {idx + 1}: {q.get('question', 'Soru metni yok')}",
                      font=('Arial', 10, 'bold'),
                      wraplength=700,
                      anchor='w',
                      justify='left').pack(side='left', fill='x', expand=True)

            # Yıldızlar (sağ üstte)
            ttk.Label(top_row,
                      text=stars,
                      font=('Arial', 12),
                      foreground='gold',
                      background=THEMES[self.theme_manager.current]['bg']).pack(side='right')

            # Şıklar
            options = q.get('options', {})
            for opt in ['A', 'B', 'C', 'D']:
                if opt in options:
                    ttk.Radiobutton(
                        fr,
                        text=f"{opt}) {options[opt]}",
                        value=opt,
                        variable=self.test_selected_answers[idx],
                        command=lambda qidx=idx: self.update_test_optic(qidx),
                        style='Question.TRadiobutton'
                    ).pack(anchor='w')


    def show_test_setup(self):
        self.clear()
        self.current_screen = 'test'
        self.mode = 'test'  # Test modunu açıkça belirt

        ttk.Label(self.root, text='Test Modu: Kategori ve Konu Seçimi', font=self.title_font).pack(pady=20)

        # Kategori seçimi
        ttk.Label(self.root, text='Kategori Seçiniz (TYT/AYT)', font=self.font).pack(pady=10)
        self.cat_var = tk.StringVar()
        cat_cb = ttk.Combobox(self.root, textvariable=self.cat_var, state='readonly', values=self.get_categories())
        cat_cb.pack()

        # İleri butonu
        ttk.Button(self.root, text='İleri', command=self.show_ders_selection).pack(pady=10)
        # Geri butonu
        ttk.Button(self.root, text='Geri', command=self.show_test_menu).pack(pady=5)

    def get_users(self):
        cur = self.conn.cursor()
        cur.execute("SELECT Id, KullaniciAdi, SinifId FROM Kullanicilar")
        return [{'Id': row[0], 'KullaniciAdi': row[1], 'SinifId': row[2]} for row in cur.fetchall()]

    def show_test_results(self):
        # Test cevaplarını kaydet
        try:
            cur = self.conn.cursor()
            for idx, q in enumerate(self.test_questions):
                selected = self.test_selected_answers[idx].get()
                if not selected:
                    continue

                soru_metin = q['question']
                secenek_aciklama = q['options'].get(selected)
                if not secenek_aciklama:
                    continue

                # Soru ID
                cur.execute("SELECT Id FROM Sorular WHERE SoruMetni = ?", (soru_metin,))
                row = cur.fetchone()
                if not row:
                    continue
                soru_id = row[0]

                # SecenekId
                cur.execute("""
                    SELECT Id FROM Secenekler 
                    WHERE SoruId = ? AND Aciklama LIKE ?
                """, (soru_id, f"%{secenek_aciklama}%"))
                row = cur.fetchone()
                if not row:
                    continue
                secenek_id = row[0]

                # Kaydet
                cur.execute("""
                    INSERT INTO OgrenciCevaplari (SecenekId, KullaniciId, Sure, SinavId)
                    VALUES (?, ?, ?, ?)
                """, (
                    secenek_id,
                    self.selected_user_id,
                    0,
                    self.sinav_id  # test sırasında da set edilmişti
                ))
            self.conn.commit()
        except Exception as e:
            logging.error(f"Test cevapları kaydedilemedi: {e}")

        self.clear()
        self.current_screen = 'results'

        # Başlık
        title_frame = ttk.Frame(self.root, style='Custom.TFrame')
        title_frame.pack(fill='x', pady=20)
        ttk.Label(title_frame,
                  text='Test Sonuçları',
                  font=self.title_font,
                  style='Custom.TLabel').pack()

        # Scrollable frame oluştur
        sf = ScrollableFrame(self.root, theme_name=self.theme_manager.current)
        sf.pack(fill='both', expand=True)

        # Soruları göster
        for idx, q in enumerate(self.test_questions):
            q_frame = ttk.Frame(sf.inner, style='Custom.TFrame')
            q_frame.pack(fill='x', pady=5, padx=5)

            # Öğrenci cevap vermiş mi?
            is_blank = self.test_selected_answers[idx].get() == ""
            fg_color = 'orange' if is_blank else THEMES[self.theme_manager.current]['fg']
            stars = self.get_star_rating(q.get('difficulty', 1))

            # Soru başlığı ve yıldızlar hizası
            top_row = ttk.Frame(q_frame)
            top_row.pack(fill='x')

            question_text = f"Soru {idx + 1}: {q['question']}"
            if is_blank:
                question_text = f"🟠{question_text} | Boş"

            ttk.Label(top_row,
                      text=question_text,
                      font=('Arial', 10, 'bold'),
                      foreground=fg_color,
                      background=THEMES[self.theme_manager.current]['bg'],
                      wraplength=700,
                      anchor='w',
                      justify='left').pack(side='left', fill='x', expand=True)

            ttk.Label(top_row,
                      text=stars,
                      font=('Arial', 12),
                      foreground='gold',
                      background=THEMES[self.theme_manager.current]['bg']).pack(side='right')

            # Şıkları gösterme
            options = q.get('options', {})
            for opt in ['A', 'B', 'C', 'D']:
                if opt in options:
                    # Doğru şık ise yeşil renk
                    is_correct = opt == q['answer']
                    # Kullanıcının seçtiği şık ise ve yanlışsa kırmızı renk
                    user_selected = self.test_selected_answers[idx].get() == opt
                    is_wrong_selection = user_selected and not is_correct

                    theme = THEMES[self.theme_manager.current]
                    # Renk belirleme
                    if is_correct:
                        bg_color = '#1f4025' if self.theme_manager.current == 'dark' else '#e6ffe6'
                        text_color = '#00ff88' if self.theme_manager.current == 'dark' else 'green'
                        prefix = ""
                        suffix = "✓ "
                    elif is_wrong_selection:
                        bg_color = '#402020' if self.theme_manager.current == 'dark' else '#ffe6e6'
                        text_color = '#ff6666' if self.theme_manager.current == 'dark' else 'red'
                        prefix = ""
                        suffix = "✗  (Sizin Cevabınız)"
                    else:
                        bg_color = theme['bg']
                        text_color = theme['fg']
                        prefix = ""
                        suffix = ""

                    # Şık frame'i
                    option_frame = tk.Frame(q_frame, bg=THEMES[self.theme_manager.current]['bg'])
                    option_frame.pack(fill='x', pady=1)

                    # Şık etiketi
                    tk.Label(
                        option_frame,
                        text=f"{prefix}{opt}) {options[opt]}{suffix}",
                        bg=bg_color,
                        fg=text_color,
                        wraplength=750,
                        font=('Arial', 9),
                        padx=5, pady=2
                    ).pack(side='left', fill='x', expand=True)

            # Açıklama
            explanation_color = '#7ecbff' if self.theme_manager.current == 'dark' else 'blue'
            tk.Label(q_frame,
                     text=f"Açıklama: {q.get('explanation', 'Açıklama yok')}",
                     fg=explanation_color,
                     bg=THEMES[self.theme_manager.current]['bg'],
                     wraplength=800,
                     font=('Arial', 9, 'italic')).pack(anchor='w', pady=(10, 5))

            # Ayırıcı çizgi
            ttk.Separator(q_frame, orient='horizontal').pack(fill='x', pady=5)

        # Ana menüye dön butonu
        ttk.Button(self.root, text="Ana Menü", command=self.show_main_menu).pack(pady=20)
        ttk.Button(self.root, text="Performans Değerlendirmesi", command=self.show_test_rating).pack(pady=10)

    def show_test_rating(self):
        self.clear()
        self.current_screen = 'test_report'

        correct = 0
        blank = 0
        for idx, q in enumerate(self.test_questions):
            selected = self.test_selected_answers[idx].get()
            if selected == "":
                blank += 1
            elif selected == q['answer']:
                correct += 1

        total = len(self.test_questions)
        attempted = total - blank
        wrong = attempted - correct
        ratio = correct / total if total else 0

        if ratio >= 0.9:
            grade = "S+ (Mükemmel)"
        elif ratio >= 0.75:
            grade = "A (İyi)"
        elif ratio >= 0.5:
            grade = "B (Orta)"
        else:
            grade = "C (Zayıf)"

        ttk.Label(self.root, text="Test Değerlendirme Raporu", font=self.title_font).pack(pady=20)
        ttk.Label(self.root, text=f"Toplam Soru: {total}").pack()
        ttk.Label(self.root, text=f"Doğru: {correct} | Yanlış: {wrong} | Boş: {blank}").pack()
        ttk.Label(self.root, text=f"Başarı Notu: {grade}").pack(pady=10)
        ttk.Button(self.root, text="Sonuçlara Geri Dön", command=self.show_test_results).pack(pady=20)



        # E-sınav modülü için işlemler

    def load_user_exams(self):
   
        # yeni doğrudan:
        if self.selected_user_id is None:
            messagebox.showerror("Hata", "Kullanıcı verisi yok!")
            return 

        sinif_id = self.selected_sinif_id

        #sınavları çeker
        cur = self.conn.cursor()
        cur.execute("""
            SELECT s.Id, s.Adi
            FROM Sinavlar s
            WHERE s.Atanan_Sinif = ? AND s.SinavKategoriId IN (4, 5, 6, 10)
        """, (sinif_id,))
        self.uygun_sinavlar = cur.fetchall()

        if not self.uygun_sinavlar:
            messagebox.showinfo("Bilgi", "Bu kullanıcının sınıfına atanmış sınav yok.")
            return

        self.show_user_exam_selection()


    def show_user_exam_selection(self):
        self.clear()
        ttk.Label(self.root, text="Sınav Seçimi", font=self.title_font).pack(pady=20)

        ttk.Button(self.root, text="📘 Tanımlanmış Sınavlar", command=self.show_assigned_exams).pack(pady=10)
        ttk.Button(self.root, text="📚 Geçmiş Sınavlar", command=self.show_previous_exams).pack(pady=10)
        ttk.Button(self.root, text="↩ Geri", command=self.show_exam_setup).pack(pady=20)

    def show_assigned_exams(self):
        self.clear()
        ttk.Label(self.root, text="Tanımlanmış Sınavlar", font=self.title_font).pack(pady=20)

        cur = self.conn.cursor()
        cur.execute("""
            SELECT s.Id, s.Adi FROM Sinavlar s
            WHERE s.Atanan_Sinif = ? AND s.SinavKategoriId IN (4, 5, 6, 10)
            AND s.Id NOT IN (
                SELECT SinavId FROM SinavSonuclari WHERE KullaniciId = ?
            )
        """, (self.selected_sinif_id, self.selected_user_id))

        self.tanimli_sinavlar = cur.fetchall()
        self.sinav_sec_var = tk.StringVar()
        ttk.Combobox(self.root, textvariable=self.sinav_sec_var,
                     values=[s[1] for s in self.tanimli_sinavlar],
                     state='readonly').pack(pady=10)

        ttk.Button(self.root, text="Sınava Başla", command=self.baslat_sinav).pack(pady=10)
        ttk.Button(self.root, text="↩ Geri", command=self.show_user_exam_selection).pack(pady=10)

    def show_previous_exams(self):
        if not hasattr(self, 'selected_user_id') or self.selected_user_id is None:
            messagebox.showerror("Hata", "Önce kullanıcı seçmeniz gerekiyor.")
            return

        self.clear()
        ttk.Label(self.root, text="Geçmiş Sınavlar", font=self.title_font).pack(pady=20)

        cur = self.conn.cursor()
        cur.execute("""
            SELECT s.Id, s.Adi FROM SinavSonuclari ss
            JOIN Sinavlar s ON ss.SinavId = s.Id
            WHERE ss.KullaniciId = ?
        """, (self.selected_user_id,))

        self.gecmis_sinavlar = cur.fetchall()
        self.sinav_sec_var = tk.StringVar()
        ttk.Combobox(self.root, textvariable=self.sinav_sec_var,
                     values=[s[1] for s in self.gecmis_sinavlar],
                     state='readonly').pack(pady=10)

        ttk.Button(self.root, text="Sonuçları Gör", command=self.goster_sonuc_raporu).pack(pady=10)
        ttk.Button(self.root, text="↩ Geri", command=self.show_user_exam_selection).pack(pady=10)

    def baslat_sinav(self):
        secilen_ad = self.sinav_sec_var.get()
        sinav = next((s for s in self.uygun_sinavlar if s[1] == secilen_ad), None)
        if sinav:
            self.secilen_sinav_id = sinav[0]

            try:
                cur = self.conn.cursor()

                # 1. Kategori adını al
                cur.execute("""
                    SELECT k.Adi
                    FROM Sinavlar s
                    JOIN SinavKategorileri k ON s.SinavKategoriId = k.Id
                    WHERE s.Id = ?
                """, (self.secilen_sinav_id,))
                row_kategori = cur.fetchone()
                if not row_kategori:
                    messagebox.showerror("Hata", "Kategori alınamadı!")
                    return
                kategori_adi = row_kategori[0]

                # 2. Süreyi al
                cur.execute("""
                    SELECT K.VarsayilanSure  -- ← sütun adın 'Sure' değilse düzelt
                    FROM Sinavlar S
                    JOIN SinavKategorileri K ON S.SinavKategoriId = K.Id
                    WHERE S.Id = ?
                """, (self.secilen_sinav_id,))
                row_sure = cur.fetchone()
                if row_sure:
                    self.remaining_sec = int(row_sure[0]) * 60
                else:
                    self.remaining_sec = 30 * 60  # Default süre

                # 3. Soruları yükle ve arayüzü başlat
                self.load_exam_questions(self.secilen_sinav_id)
                self.current_screen = 'exam'
                self.show_exam_interface()
                self.theme_manager.apply(self.theme_manager.current)
                self.start_timer()

            except Exception as e:
               logging.error(f"Sınav başlatılırken hata: {e}")
               messagebox.showerror("Hata", "Sınav başlatılamadı. Lütfen tekrar deneyin.")
               return

            # 2) C# tarafı için sinav başlatıldı mesajı:
            print("Sinav basladi") # , flush=True
            
            # 3) İsteğe bağlı kullanıcıya bilgi:
            messagebox.showinfo("Başarılı", "Sınav başlatılıyor...")
            

    def load_exam_questions(self, sinav_id):
        print("Kullanıcı ID:", self.selected_user_id)
        try:
            cur = self.conn.cursor()

            # Cevaplar: {SoruId: SecenekId}
            cur.execute("""
                SELECT SoruId, SecenekId
                FROM OgrenciCevaplari
                WHERE KullaniciId = ? AND SinavId = ?
            """, (self.selected_user_id, sinav_id))
            cevaplar = cur.fetchall()
            soru_cevaplari = {soru_id: secenek_id for soru_id, secenek_id in cevaplar}

            # Sorular ve seçenekler
            cur.execute("""
                SELECT s.Id, s.SoruMetni, s.YildizSeviyesi, s.SinavDersKonuId, s.SecenekSayisi,
                       d.Adi as DersAdi, o.Aciklama, o.Status, o.Id as SecenekId
                  FROM Sorular s
                  JOIN Secenekler o ON s.Id = o.SoruId
                  JOIN SinavDersKonulari k ON s.SinavDersKonuId = k.Id
                  JOIN SinavDersleri d ON k.SinavDersId = d.Id
                 WHERE s.SinavId = ?
            """, sinav_id)
            rows = cur.fetchall()

            modules_dict = {}
            for row in rows:
                soru_id, metin, zorluk, _, _, ders, secenek_aciklama, status, secenek_id = row
                modules_dict.setdefault(ders, {})

                if soru_id not in modules_dict[ders]:
                    modules_dict[ders][soru_id] = {
                        'id': soru_id,
                        'question': metin,
                        'difficulty': zorluk,
                        'options': {},
                        'secenek_ids': {},
                        'answer': '',
                        'explanation': ''
                    }

                harf = ['A', 'B', 'C', 'D'][len(modules_dict[ders][soru_id]['options'])]
                modules_dict[ders][soru_id]['options'][harf] = secenek_aciklama
                modules_dict[ders][soru_id]['secenek_ids'][harf] = secenek_id

                if status:
                    modules_dict[ders][soru_id]['answer'] = harf

            # Seçilen cevapları harfe çevirerek set et
            for ders, sorular in modules_dict.items():
                for soru_id, q in sorular.items():
                    selected_value = ""
                    secenek_id = soru_cevaplari.get(soru_id)
                    if secenek_id:
                        for harf, s_id in q['secenek_ids'].items():
                            if s_id == secenek_id:
                                selected_value = harf
                                break
                    q['selected_answer'] = tk.StringVar(value=selected_value)

            self.modules = [{'name': ders, 'questions': list(sorular.values())} for ders, sorular in
                            modules_dict.items()]
            self.current_module = 0
            self.root.after(0, self.show_exam_interface)
            self.root.after(0, self.start_timer)

            if not self.modules:
                logging.error("HATA: Modüller boş geldi, sınav yüklenemedi.")
                messagebox.showerror("Hata", "Bu sınav için yüklenecek soru bulunamadı.")
                return


        except Exception as e:
            logging.error(f"Sınav başlatılırken hata: {str(e)}")
            messagebox.showerror("Hata", "Sorular yüklenirken bir hata oluştu.")

        except Exception as e:
            logging.error(f"Sınav başlatılırken hata: {str(e)}")
            messagebox.showerror("Hata", "Sorular yüklenirken bir hata oluştu.")

    # ✅ TEST/ÖDEV MODÜLÜ FONKSİYONU: Süreyi çeker, veri kaydı yapmaz
    def load_test_questions(self, sinav_id):
        try:
            cur = self.conn.cursor()

            cur.execute("""
                SELECT K.VarsayilanSure
                FROM Sinavlar S
                JOIN SinavKategorileri K ON S.SinavKategoriId = K.Id
                WHERE S.Id = ?
            """, (sinav_id,))
            row = cur.fetchone()
            self.remaining_sec = int(row[0]) * 60 if row else 30 * 60

            cur.execute("""
                SELECT s.Id, s.SoruMetni, s.YildizSeviyesi, s.SinavDersKonuId, s.SecenekSayisi,
                       d.Adi as DersAdi, o.Aciklama, o.Status, o.Id as SecenekId
                  FROM Sorular s
                  JOIN Secenekler o ON s.Id = o.SoruId
                  JOIN SinavDersKonulari k ON s.SinavDersKonuId = k.Id
                  JOIN SinavDersleri d ON k.SinavDersId = d.Id
                 WHERE s.SinavId = ?
            """, sinav_id)
            rows = cur.fetchall()

            modules_dict = {}
            for row in rows:
                soru_id, metin, zorluk, _, _, ders, secenek_aciklama, status, secenek_id = row
                modules_dict.setdefault(ders, {})
                if soru_id not in modules_dict[ders]:
                    modules_dict[ders][soru_id] = {
                        'question': metin,
                        'difficulty': zorluk,
                        'options': {},
                        'secenek_ids': {},
                        'answer': '',
                        'explanation': '',
                        'selected_answer': tk.StringVar(value="")
                    }
                harf = ['A', 'B', 'C', 'D'][len(modules_dict[ders][soru_id]['options'])]
                modules_dict[ders][soru_id]['options'][harf] = secenek_aciklama
                modules_dict[ders][soru_id]['secenek_ids'][harf] = secenek_id
                if status:
                    modules_dict[ders][soru_id]['answer'] = harf

            self.modules = [{'name': ders, 'questions': list(sorular.values())} for ders, sorular in
                            modules_dict.items()]
            self.current_module = 0
            self.root.after(0, self.show_exam_interface)
            self.root.after(0, self.start_timer)

        except Exception as e:
            logging.error(f"Test başlatılırken hata: {str(e)}")
            messagebox.showerror("Hata", "Test soruları yüklenirken bir hata oluştu.")



    def show_exam_interface(self):
        self.clear()

        self.current_screen = 'exam'
        # Üst çubuk
        top = ttk.Frame(self.root)
        top.pack(fill='x', padx=10, pady=5)

        # Modül butonları
        btn_frame = ttk.Frame(top)
        btn_frame.pack(side='left')
        for idx, module in enumerate(self.modules):
            ttk.Button(btn_frame, text=module['name'],
                       command=lambda i=idx: self.switch_module(i)).pack(side='left', padx=5)

        # Timer
        self.timer_label = ttk.Label(top, text="00:00", font=self.font)
        self.timer_label.pack(side='right')

        # Sınavı tamamla butonu
        complete_btn = ttk.Button(top, text="Sınavı Tamamla", command=self.complete_exam)
        complete_btn.pack(side='right', padx=10)

        # Ana içerik
        main_frame = ttk.Frame(self.root)
        main_frame.pack(fill='both', expand=True)

        # Optik Form (Sağda)
        opt_frame = ttk.Frame(main_frame, width=120, style='Optik.TFrame')
        opt_frame.pack(side='right', fill='y', padx=10, pady=10)

        ttk.Label(opt_frame, text="Optik Form", style='Optik.TLabel').pack(pady=5)

        self.optik_circles = []
        questions = self.modules[self.current_module]['questions']

        for i in range(len(questions)):
            cell_frame = ttk.Frame(opt_frame, style='Optik.TFrame')
            cell_frame.pack(pady=2, fill='x')

            ttk.Label(cell_frame, text=str(i + 1), width=3).pack(side='left', padx=5)
            circles_frame = ttk.Frame(cell_frame)
            circles_frame.pack(side='left', fill='x', expand=True)

            circles = []
            for opt in ['A', 'B', 'C', 'D']:
                circle = ttk.Radiobutton(
                    circles_frame,
                    text=opt,
                    style='Optik.TRadiobutton',
                    value=opt,
                    variable=self.modules[self.current_module]['questions'][i]['selected_answer'],
                    command=lambda idx=i: self.update_optik_display(idx),
                    state='disabled'
                )
                circle.pack(side='left', expand=True)
                circles.append(circle)

            self.optik_circles.append(circles)
            cell_frame.bind('<Button-1>', lambda e, idx=i: self.scroll_to_question(idx))

        # Sorular (Scrollable)
        self.sf = ScrollableFrame(main_frame, theme_name=self.theme_manager.current)
        self.sf.pack(fill='both', expand=True)

        self.q_frames = []
        self.explanation_labels = []  # Açıklama label'larını saklamak için liste
        questions = self.modules[self.current_module]['questions']

        for idx, q in enumerate(questions):
            module_name = self.modules[self.current_module]['name']
            unique_key = f"{self.test_type}_{module_name}_{idx}"


            fr = ttk.LabelFrame(self.sf.inner, text=f"Soru {idx + 1}", padding=10)
            fr.pack(fill='x', pady=5)

            stars = self.get_star_rating(q.get('difficulty', 1))

            # Üst başlık satırı: Soru metni solda, yıldızlar sağda
            top_row = ttk.Frame(fr)
            top_row.pack(fill='x', pady=(0, 5))

            # Soru metni (sola yaslı)
            ttk.Label(top_row,
                      text=f"Soru {idx + 1}: {q.get('question', 'Soru metni yok')}",
                      font=('Arial', 10, 'bold'),
                      wraplength=700,
                      anchor='w',
                      justify='left').pack(side='left', fill='x', expand=True)

            # Yıldızlar (sağ üstte)
            ttk.Label(top_row,
                      text=stars,
                      font=('Arial', 12),
                      foreground='gold',
                      background=THEMES[self.theme_manager.current]['bg']).pack(side='right')

            # Şıklar
            options = q.get('options', {})
            for opt in ['A', 'B', 'C', 'D']:
                if opt in options:
                    bg_color = THEMES[self.theme_manager.current]['bg']
                    fg_color = THEMES[self.theme_manager.current]['fg']
                    ttk.Radiobutton(
                        fr,
                        text=f"{opt}) {options[opt]}",
                        value=opt,
                        variable=q['selected_answer'],
                        command=lambda qidx=idx: self.save_answer(qidx),
                        style='Question.TRadiobutton'
                    ).pack(anchor='w')

            # Açıklama (başlangıçta gizli)
            explanation = ttk.Label(
                fr,
                text=f"Açıklama: {q.get('explanation', 'Açıklama yok')}",
                foreground='blue',
                wraplength=800
            )
            # Açıklamayı hemen pack yapmıyoruz, sadece oluşturuyoruz
            self.explanation_labels.append(explanation)  # Listeye ekle

            self.q_frames.append(fr)

        # Stil ayarları
        ttk.Style().configure('Opt.TFrame', background='#f5f5f5')

    def start_timer(self):
        print(f"[DEBUG] start_timer başladı, remaining_sec = {self.remaining_sec}")

        def tick():
            try:
                if self.remaining_sec > 0 and hasattr(self, 'timer_label') and self.timer_label:
                    m, s = divmod(self.remaining_sec, 60)
                    self.timer_label.config(text=f"Kalan Süre: {m:02d}:{s:02d}")
                    self.remaining_sec -= 1
                    self.root.after(1000, tick)
                elif self.remaining_sec <= 0:
                    if hasattr(self, 'timer_label') and self.timer_label:
                        self.timer_label.config(text="Süre Doldu!")
                    messagebox.showinfo("Bilgi", "Sınav süreniz doldu!")
                    self.show_results()
            except Exception as e:
                print(f"Timer hatası: {e}")  # Hata ayıklama için

        tick()

    def update_optik_display(self, question_idx):
        """optik formdaki görünümü güncelleyen metod"""
        selected = self.modules[self.current_module]['questions'][question_idx]['selected_answer'].get()
        for i, opt in enumerate(['A', 'B', 'C', 'D']):
            btn = self.optik_circles[question_idx][i]
            if opt == selected:
                btn.state(['selected'])
            else:
                btn.state(['!selected'])

    def save_answer(self, question_idx):
        q = self.modules[self.current_module]['questions'][question_idx]
        selected = q['selected_answer'].get()

        # Seçimi kaydet
        module_name = self.modules[self.current_module]['name']
        unique_key = f"{self.test_type}_{module_name}_{question_idx}"
        self.selected_answers[unique_key] = selected

        # Optik formu güncelle
        self.update_optik_display(question_idx)

    def switch_module(self, idx):
        self.save_current_module_answers()
        self.current_module = idx
        self.show_exam_interface()
        # optik formu da güncelle
        for q_idx in range(len(self.modules[self.current_module]['questions'])):
            self.update_optik_display(q_idx)

    def save_current_module_answers(self):
        for q_idx, q in enumerate(self.modules[self.current_module]['questions']):
            self.save_answer(q_idx)

    def scroll_to_question(self, idx):
        if idx < len(self.q_frames):
            self.sf.canvas.yview_moveto(self.q_frames[idx].winfo_y() / self.sf.inner.winfo_height())

            # optik formda seçili hücreyi vurgula
            for i, circles in enumerate(self.optik_circles):
                for circle in circles:
                    if i == idx:
                        circle.config(style='Selected.TRadiobutton')
                    else:
                        circle.config(style='Optik.TRadiobutton')

    def complete_exam(self):
        self.save_current_module_answers()

        if not messagebox.askyesno("Onay", "Sınavı tamamlamak istediğinize emin misiniz?"):
            return

        self.remaining_sec = 0
        if hasattr(self, 'timer_label'):
            self.timer_label = None

        # Cevapları OgrenciCevaplari tablosuna kaydet
        try:
            cur = self.conn.cursor()

            sinav_id = self.secilen_sinav_id  # Sınav başlatıldığında set edilmiş olmalı
            sure = 0  # Şimdilik sabit süre, ileride hesaplanabilir

            for module in self.modules:
                for q in module['questions']:
                    secilen_sik = q['selected_answer'].get()
                    soru_metin = q['question']

                    if not secilen_sik:
                        continue  # Boş bırakılan soruları geç

                    # Soru ID zaten varsa doğrudan kullan (q['id']), yoksa metinden al
                    soru_id = q.get('id')
                    if not soru_id:
                        cur.execute("SELECT Id FROM Sorular WHERE SoruMetni = ?", soru_metin)
                        row = cur.fetchone()
                        if not row:
                            continue
                        soru_id = row[0]

                    # Seçilen şık açıklaması
                    secilen_aciklama = q['options'].get(secilen_sik)
                    if not secilen_aciklama:
                        continue

                    # SeçenekId'yi bul
                    cur.execute("""
                        SELECT Id FROM Secenekler 
                        WHERE SoruId = ? AND Aciklama LIKE ?
                    """, (soru_id, f"%{secilen_aciklama}%"))
                    secenek_row = cur.fetchone()
                    if not secenek_row:
                        continue
                    secenek_id = secenek_row[0]

                    # Cevabı OgrenciCevaplari tablosuna ekle
                    cur.execute("""
                        INSERT INTO OgrenciCevaplari (SoruId, SecenekId, KullaniciId, Sure, SinavId)
                        VALUES (?, ?, ?, ?, ?)
                    """, (soru_id, secenek_id, self.selected_user_id, sure, sinav_id))

            self.conn.commit()

        except Exception as e:
            logging.error(f"Cevaplar kaydedilirken hata: {e}")
            messagebox.showerror("Hata", "Cevaplar kaydedilemedi!")

        # Sınav sonucu genel istatistiklerini kaydet
        try:
            cur = self.conn.cursor()
            toplam_dogru = toplam_yanlis = 0

            for module in self.modules:
                for q in module['questions']:
                    selected = q['selected_answer'].get()
                    if not selected:
                        continue
                    if selected == q['answer']:
                        toplam_dogru += 1
                    else:
                        toplam_yanlis += 1

            puan = toplam_dogru * 5  # örnek puan hesaplama

            cur.execute("""
                INSERT INTO SinavSonuclari (KullaniciId, SinavId, ToplamDogrular, ToplamYanlislar, ToplamPuan)
                VALUES (?, ?, ?, ?, ?)
            """, (self.selected_user_id, self.secilen_sinav_id, toplam_dogru, toplam_yanlis, puan))
            self.conn.commit()

        except Exception as e:
            logging.error(f"Sonuç kaydında hata: {e}")

        # Sonuç ekranına geç
        self.show_results()

    def goster_sonuc_raporu(self):
        secilen_ad = self.sinav_sec_var.get()
        sinav = next((s for s in self.gecmis_sinavlar if s[1] == secilen_ad), None)
        if sinav:
            self.secilen_sinav_id = sinav[0]
            self.load_exam_questions(self.secilen_sinav_id)
            self.show_results()

    def show_results(self):

        self.current_screen = 'results'

        # Timer'ı tamamen durdur
        self.remaining_sec = 0
        self.clear()

        theme = THEMES[self.theme_manager.current]

        # Renkleri tema değişkenlerinden al
        theme = THEMES[self.theme_manager.current]
        correct_bg = theme['highlight'] if self.theme_manager.current == 'dark' else '#e6ffe6'
        correct_fg = '#00ff88' if self.theme_manager.current == 'dark' else 'green'

        # Başlık
        title_frame = ttk.Frame(self.root, style='Custom.TFrame')
        title_frame.pack(fill='x', pady=20)
        ttk.Label(title_frame,
                  text='Sınav Sonuçları',
                  font=self.title_font,
                  style='Custom.TLabel').pack()

        # Scrollable frame oluştur
        sf = ScrollableFrame(self.root, theme_name=self.theme_manager.current)
        sf.pack(fill='both', expand=True)

        # Tüm modülleri ve soruları göster
        for module_idx, module in enumerate(self.modules):
            module_frame = ttk.LabelFrame(sf.inner,
                                          text=module['name'],
                                          padding=10,
                                          style='Custom.TLabelframe')
            module_frame.pack(fill='x', pady=10, padx=5)

            for q_idx, q in enumerate(module['questions']):
                q_frame = ttk.Frame(module_frame, style='Custom.TFrame')
                q_frame.pack(fill='x', pady=5, padx=5)

                # Soru bilgileri
                is_blank = q['selected_answer'].get() == ""
                stars = self.get_star_rating(q.get('difficulty', 1))
                fg_color = 'orange' if is_blank else THEMES[self.theme_manager.current]['fg']

                # Yıldız ve soru başlığı ayrı hizalı
                top_row = ttk.Frame(q_frame)
                top_row.pack(fill='x')

                # Soru başlığı sol üstte
                question_text = f"Soru {q_idx + 1}: {q['question']}"
                if is_blank:
                    question_text = f"🟠{question_text} | Boş"

                ttk.Label(top_row,
                          text=question_text,
                          font=('Arial', 10, 'bold'),
                          foreground=fg_color,
                          background=THEMES[self.theme_manager.current]['bg'],
                          wraplength=700,
                          anchor='w',
                          justify='left').pack(side='left', fill='x', expand=True)

                # Yıldızlar sağ üstte
                ttk.Label(top_row,
                          text=stars,
                          font=('Arial', 12),
                          foreground='gold',
                          background=THEMES[self.theme_manager.current]['bg']).pack(side='right')

                # Şıkları gösterme
                options = q.get('options', {})
                for opt in ['A', 'B', 'C', 'D']:
                    if opt in options:
                        # Doğru şık ise yeşil renk
                        is_correct = opt == q['answer']
                        # Kullanıcının seçtiği şık ise ve yanlışsa kırmızı renk
                        user_selected = q['selected_answer'].get() == opt
                        is_wrong_selection = user_selected and not is_correct
                        # eğer soru boş bırakıldıysa
                        is_blank = q['selected_answer'].get() == ""

                        theme = THEMES[self.theme_manager.current]
                        # Renk belirleme
                        if is_correct:
                            bg_color = '#1f4025' if self.theme_manager.current == 'dark' else '#e6ffe6'
                            text_color = '#00ff88' if self.theme_manager.current == 'dark' else 'green'
                            prefix = ""
                            suffix = "✓ "
                        elif is_wrong_selection:
                            bg_color = '#402020' if self.theme_manager.current == 'dark' else '#ffe6e6'
                            text_color = '#ff6666' if self.theme_manager.current == 'dark' else 'red'
                            prefix = ""
                            suffix = "✗  (Sizin Cevabınız)"
                        else:
                            bg_color = theme['bg']
                            text_color = theme['fg']
                            prefix = ""
                            suffix = ""

                        # Şık frame'i
                        option_frame = tk.Frame(q_frame, bg=THEMES[self.theme_manager.current]['bg'])
                        option_frame.pack(fill='x', pady=1)

                        # Şık etiketi
                        tk.Label(
                            option_frame,
                            text=f"{prefix}{opt}) {options[opt]}{suffix}",
                            bg=bg_color,  # THEMES[self.theme_manager.current]['bg'],  # Arkaplan rengi
                            fg=text_color,
                            wraplength=750,
                            font=('Arial', 9),
                            padx=5, pady=2
                        ).pack(side='left', fill='x', expand=True)

                # Açıklama
                bg = THEMES[self.theme_manager.current]['bg']
                explanation_color = '#7ecbff' if self.theme_manager.current == 'dark' else 'blue'
                tk.Label(q_frame,
                         text=f"Açıklama: {q.get('explanation', 'Açıklama yok')}",
                         fg=explanation_color,
                         bg=THEMES[self.theme_manager.current]['bg'],  # <-- Eksikti
                         wraplength=800,
                         font=('Arial', 9, 'italic')).pack(anchor='w', pady=(10, 5))

                # Ayırıcı çizgi
                ttk.Separator(q_frame, orient='horizontal').pack(fill='x', pady=5)

        # Ana menüye dön butonu
        ttk.Button(self.root, text="Ana Menü", command=self.show_main_menu).pack(pady=20)
        ttk.Button(self.root, text="Raporu Görüntüle", command=self.show_exam_report).pack(pady=10)

    def show_exam_report(self):
        self.clear()
        self.current_screen = 'exam_report'

        sf = ScrollableFrame(self.root, theme_name=self.theme_manager.current)
        sf.pack(fill='both', expand=True)

        ttk.Label(sf.inner, text="Yapay Zeka Destekli Sınav Raporu", font=self.title_font).pack(pady=20)

        total = correct = incorrect = blank = 0
        for module in self.modules:
            for q in module['questions']:
                total += 1
                selected = q['selected_answer'].get()
                if selected == "":
                    blank += 1
                elif selected == q['answer']:
                    correct += 1
                else:
                    incorrect += 1

        accuracy = (correct / total) * 100 if total else 0

        yorum = model.generate_content(
            f"Kullanıcı {total} soruda {correct} doğru, {incorrect} yanlış ve {blank} boş yapmıştır. Performansı nasıl değerlendirirsin?"
        )

        # Konu bazlı hata analizi
        konular = {}
        for module in self.modules:
            for q in module['questions']:
                konu = module['name']
                selected = q['selected_answer'].get()
                if konu not in konular:
                    konular[konu] = {'dogru': 0, 'yanlis': 0, 'bos': 0}
                if selected == "":
                    konular[konu]['bos'] += 1
                elif selected == q['answer']:
                    konular[konu]['dogru'] += 1
                else:
                    konular[konu]['yanlis'] += 1

        ttk.Label(sf.inner, text="\nKonu Bazlı Performans:", font=('Segoe UI', 10, 'bold')).pack(pady=(10, 2))
        konu_yorum_prompt = "Öğrencinin her konuya ait performans durumu aşağıda verilmiştir:\n"
        for konu, veriler in konular.items():
            toplam = veriler['dogru'] + veriler['yanlis'] + veriler['bos']
            if toplam == 0:
                continue
            oran_yanlis = (veriler['yanlis'] / toplam) * 100
            oran_bos = (veriler['bos'] / toplam) * 100
            ttk.Label(sf.inner, text=f"{konu}: %{oran_yanlis:.1f} yanlış, %{oran_bos:.1f} boş").pack()
            konu_yorum_prompt += f"- {konu}: %{oran_yanlis:.1f} yanlış, %{oran_bos:.1f} boş\n"

        konu_yorum_prompt += ("Bu konulara göre öğrenciye kısa bir çalışma planı öner. Herhangi bir emoji ekleme, bold yazı kullanma. Bu verilere"
                              " göre, özellikle boş bırakılan soruların da eksik öğrenme olarak ele alınması gerektiğini düşünerek öğrenciye hangi konularda eksik olduğu,"
                              "nasıl çalışması gerektiği hakkında güzel bir rapor hazırla. Öğrenciye 'Çaylak' diye hitap ederek daha samimi bir "
                              "rapor oluşturabilirsin. Günlük-haftalık raporlamaya gerek yok.")
        konu_yorum = model.generate_content(konu_yorum_prompt)

        ttk.Label(sf.inner, text="Çalışma Planı:", font=('Segoe UI', 10, 'bold')).pack(pady=(10, 2))
        ttk.Label(sf.inner, text=konu_yorum.text, wraplength=800, justify='center').pack(padx=10, pady=10)

        ttk.Label(sf.inner, text=f"Toplam Soru: {total}").pack()
        ttk.Label(sf.inner, text=f"Doğru: {correct} | Yanlış: {incorrect} | Boş: {blank}").pack(pady=5)
        ttk.Label(sf.inner, text="Yorum:", font=('Segoe UI', 10, 'bold')).pack(pady=(10, 2))
        ttk.Label(sf.inner, text=yorum.text, wraplength=800, justify='center').pack(padx=10, pady=10)

        ttk.Button(sf.inner, text="🧪 Aç Bitir Testi Oluştur", command=self.baslat_acbitir_test_from_report).pack(
            pady=10)

        ttk.Button(sf.inner, text="Sonuçlara Geri Dön", command=self.show_results).pack(pady=20)

    def baslat_acbitir_test_from_report(self):
        self.clear()
        self.current_screen = 'acbitir_test_hazirlaniyor'

        ttk.Label(self.root, text="Aç Bitir Testi Hazırlanıyor...", font=self.title_font).pack(pady=200)
        self.root.update()

        # Konu bazlı AI promptunu oluştur
        konular = {}
        for module in self.modules:
            for q in module['questions']:
                konu = module['name']
                selected = q['selected_answer'].get()
                if konu not in konular:
                    konular[konu] = {'dogru': 0, 'yanlis': 0, 'bos': 0}
                if selected == "":
                    konular[konu]['bos'] += 1
                elif selected == q['answer']:
                    konular[konu]['dogru'] += 1
                else:
                    konular[konu]['yanlis'] += 1

        konu_yorum_prompt = "Öğrencinin her konuya ait performans durumu aşağıda verilmiştir:\n"
        for konu, veriler in konular.items():
            toplam = veriler['dogru'] + veriler['yanlis'] + veriler['bos']
            if toplam == 0:
                continue
            oran_yanlis = (veriler['yanlis'] / toplam) * 100
            oran_bos = (veriler['bos'] / toplam) * 100
            konu_yorum_prompt += f"- {konu}: %{oran_yanlis:.1f} yanlış, %{oran_bos:.1f} boş\n"

        konu_yorum_prompt += (
            "\nBu verilere göre, özellikle boş bırakılan soruların da eksik öğrenme olarak ele alınması gerektiğini düşünerek "
            "öğrenciye 30 soruyu geçmeyecek şekilde eksik olduğu konulardan karışık yeni bir test üret. "
            "Her sorunun formatı şu şekilde olmalı:\n\n"
            "Soru 1: [soru metni]\nA) [şık A]\nB) [şık B]\nC) [şık C]\nD) [şık D]\n"
            "Cevap: [doğru şık]\nAçıklama: [açıklama metni]"
        )

        try:
            yanit = model.generate_content(konu_yorum_prompt)
            self.acbitir_questions = self.parse_ai_questions(yanit.text)
            self.acbitir_selected = {i: tk.StringVar(value="") for i in range(len(self.acbitir_questions))}
            self.show_acbitir_question_page()
        except Exception as e:
            logging.error(f"Aç Bitir testi oluşturulurken hata: {e}")
            messagebox.showerror("Hata", "Aç Bitir testi oluşturulamadı.")
            self.show_exam_report()

    def show_acbitir_question_page(self):
        self.clear()
        self.current_screen = 'acbitir_test'

        ttk.Label(self.root, text="Aç Bitir Testi", font=self.title_font).pack(pady=20)

        sf = ScrollableFrame(self.root, theme_name=self.theme_manager.current)
        sf.pack(fill='both', expand=True)

        for idx, q in enumerate(self.acbitir_questions):
            fr = ttk.LabelFrame(sf.inner, text=f"Soru {idx + 1}", padding=10)
            fr.pack(fill='x', pady=5)

            top_row = ttk.Frame(fr)
            top_row.pack(fill='x', pady=(0, 5))

            ttk.Label(top_row,
                      text=f"Soru {idx + 1}: {q.get('question', 'Soru metni yok')}",
                      font=('Arial', 10, 'bold'),
                      wraplength=700).pack(side='left', fill='x', expand=True)

            ttk.Label(top_row,
                      text=self.get_star_rating(q.get('difficulty', 1)),
                      font=('Arial', 12),
                      foreground='gold',
                      background=THEMES[self.theme_manager.current]['bg']).pack(side='right')

            for opt in ['A', 'B', 'C', 'D']:
                if opt in q['options']:
                    ttk.Radiobutton(
                        fr,
                        text=f"{opt}) {q['options'][opt]}",
                        value=opt,
                        variable=self.acbitir_selected[idx],
                        style='Question.TRadiobutton'
                    ).pack(anchor='w')

        ttk.Button(self.root, text="Testi Bitir", command=self.show_acbitir_results).pack(pady=20)

    def show_acbitir_results(self):
        self.clear()
        self.current_screen = 'acbitir_sonuc'

        ttk.Label(self.root, text="Aç Bitir Testi Sonuçları", font=self.title_font).pack(pady=20)

        sf = ScrollableFrame(self.root, theme_name=self.theme_manager.current)
        sf.pack(fill='both', expand=True)

        for idx, q in enumerate(self.acbitir_questions):
            fr = ttk.LabelFrame(sf.inner, text=f"Soru {idx + 1}", padding=10)
            fr.pack(fill='x', pady=5)

            selected = self.acbitir_selected[idx].get()
            stars = self.get_star_rating(q.get('difficulty', 1))
            ttk.Label(fr, text=f"{q['question']} {stars}", wraplength=800, font=('Arial', 10, 'bold')).pack(anchor='w')

            for opt in ['A', 'B', 'C', 'D']:
                if opt in q['options']:
                    is_correct = opt == q['answer']
                    user_selected = selected == opt
                    is_wrong = user_selected and not is_correct

                    if is_correct:
                        bg = '#e6ffe6'
                        fg = 'green'
                        suffix = "✓"
                    elif is_wrong:
                        bg = '#ffe6e6'
                        fg = 'red'
                        suffix = "✗ (Sizin cevabınız)"
                    else:
                        bg = THEMES[self.theme_manager.current]['bg']
                        fg = THEMES[self.theme_manager.current]['fg']
                        suffix = ""

                    tk.Label(fr,
                             text=f"{opt}) {q['options'][opt]} {suffix}",
                             bg=bg, fg=fg,
                             wraplength=750,
                             font=('Arial', 9),
                             padx=5, pady=2).pack(anchor='w')

        ttk.Button(self.root, text="Ana Menü", command=self.show_main_menu).pack(pady=20)


if __name__ == '__main__':
    root = tk.Tk()
    app = QuestionApp(root)
    root.mainloop()