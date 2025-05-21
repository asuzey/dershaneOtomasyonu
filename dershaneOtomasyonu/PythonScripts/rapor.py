# -*- coding: utf-8 -*-
import tkinter as tk
from tkinter import ttk, messagebox
import pyodbc

class ThemeManager:
    def __init__(self):
        self.current = "light"

THEMES = {
    "light": {
        "bg": "#f9fafb",
        "fg": "#111827",
        "accent": "#3b82f6",
        "button_bg": "#e5e7eb",
        "button_fg": "#1f2937",
        "button_hover_bg": "#3b82f6",
        "button_hover_fg": "#ffffff",
        "highlight": "#dbeafe"
    }
}

class App(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("Star Soru Çözme Alanı")
        self.state('zoomed')  # Tam ekran başlat
        self.theme = ThemeManager()
        self.conn = self.get_db_connection()
        self.selected_users = []
        self.current_frame = None
        self.configure(bg=THEMES['light']['bg'])

        style = ttk.Style()
        style.theme_use("clam")
        style.configure("TLabel", background=THEMES['light']['bg'], foreground=THEMES['light']['fg'], font=("Segoe UI", 11))
        style.configure("TButton", background=THEMES['light']['button_bg'], foreground=THEMES['light']['button_fg'], font=("Segoe UI", 10, "bold"), padding=6)
        style.map("TButton", background=[("active", THEMES['light']['button_hover_bg'])], foreground=[("active", THEMES['light']['button_hover_fg'])])
        style.configure("TCombobox", fieldbackground=THEMES['light']['bg'], selectbackground=THEMES['light']['highlight'], font=("Segoe UI", 10))
        style.configure("Treeview", font=("Segoe UI", 10), rowheight=28, fieldbackground=THEMES['light']['bg'], background=THEMES['light']['bg'], foreground=THEMES['light']['fg'])
        style.configure("Treeview.Heading", font=("Segoe UI", 10, "bold"), background=THEMES['light']['accent'], foreground="#ffffff")

        self.show_main_menu()

    def get_db_connection(self):
        return pyodbc.connect(
            'DRIVER={ODBC Driver 17 for SQL Server};'
            'SERVER=localhost\\SQLEXPRESS;'
            'DATABASE=DERSHANE;'
            'Trusted_Connection=yes;'
        )

    def switch_frame(self, new_frame_class):
        if self.current_frame:
            self.current_frame.destroy()
        self.current_frame = new_frame_class(self)
        self.current_frame.pack(fill="both", expand=True)

    def show_main_menu(self):
        self.switch_frame(MainMenuFrame)

    def show_user_filter(self):
        self.switch_frame(UserFilterFrame)

    def show_class_filter(self):
        self.switch_frame(ClassFilterFrame)

    def show_user_results(self):
        self.switch_frame(UserResultsFrame)

    def show_class_results(self, sinif_id):
        self.switch_frame(lambda parent: ClassResultsFrame(parent, sinif_id))

class MainMenuFrame(ttk.Frame):
    def __init__(self, parent):
        super().__init__(parent)
        ttk.Label(self, text="📘 Test Modülü", font=("Segoe UI", 20, "bold")).pack(pady=40)
        ttk.Button(self, text="👤 Öğrenci Bazlı Filtreleme", command=parent.show_user_filter).pack(pady=20, ipadx=20)
        ttk.Button(self, text="🏫 Sınıf Bazlı Listeleme", command=parent.show_class_filter).pack(pady=20, ipadx=20)

class UserFilterFrame(ttk.Frame):
    def __init__(self, parent):
        super().__init__(parent)
        self.parent = parent

        ttk.Label(self, text="🔎 Öğrenci Arama", font=("Segoe UI", 14, "bold")).pack(pady=10)
        self.search_var = tk.StringVar()
        entry = ttk.Entry(self, textvariable=self.search_var, width=40)
        entry.pack(pady=5)
        ttk.Button(self, text="Ara", command=self.search).pack(pady=5)

        self.tree = ttk.Treeview(self, columns=("id", "ad"), show="headings", selectmode="browse")
        self.tree.heading("id", text="ID")
        self.tree.heading("ad", text="Kullanıcı Adı")
        self.tree.pack(fill="both", expand=True, padx=20, pady=10)

        ttk.Button(self, text="📥 Öğrencinin Bilgilerini Getir", command=self.get_and_show_results).pack(pady=10)

    def search(self):
        query = self.search_var.get()
        cur = self.parent.conn.cursor()
        cur.execute("SELECT Id, KullaniciAdi FROM Kullanicilar WHERE KullaniciAdi LIKE ?", f"%{query}%")
        rows = cur.fetchall()
        for i in self.tree.get_children():
            self.tree.delete(i)
        for row in rows:
            self.tree.insert("", "end", values=(int(row[0]), row[1]))

    def get_and_show_results(self):
        selected = self.tree.selection()
        if not selected:
            messagebox.showwarning("Uyarı", "Lütfen bir öğrenci seçin.")
            return
        try:
            selected_id = int(self.tree.item(selected[0])['values'][0])
        except Exception as e:
            messagebox.showerror("Hata", f"Geçersiz öğrenci ID: {e}")
            return
        self.parent.selected_users = [selected_id]
        self.parent.show_user_results()

class UserResultsFrame(ttk.Frame):
    def __init__(self, parent):
        super().__init__(parent)
        self.parent = parent
        ttk.Label(self, text="📊 Öğrenci Sonuçları", font=("Segoe UI", 14, "bold")).pack(pady=10)

        self.tree = ttk.Treeview(self, columns=("ad", "sinav", "tarih", "puan"), show="headings")
        for col, name in zip(self.tree["columns"], ["Ad", "Sınav", "Tarih", "Puan"]):
            self.tree.heading(col, text=name)
            self.tree.column(col, anchor="center")
        self.tree.pack(fill="both", expand=True, padx=20, pady=10)

        self.load_results()

        ttk.Button(self, text="📄 Öğrenciler Sonuç Raporu", command=lambda: None).pack(pady=10)

    def load_results(self):
        cur = self.parent.conn.cursor()

        if not self.parent.selected_users:
            messagebox.showwarning("Uyarı", "Hiçbir öğrenci seçilmedi.")
            return

        try:
            selected_ids = list(map(int, self.parent.selected_users))
        except ValueError:
            messagebox.showerror("Hata", "Seçilen kullanıcı ID'leri geçersiz.")
            return

        placeholders = ','.join(['?'] * len(selected_ids))
        query = f"""
            SELECT k.KullaniciAdi, s.Adi, s.Tarih, r.ToplamPuan
            FROM SinavSonuclari r
            JOIN Kullanicilar k ON r.KullaniciId = k.Id
            JOIN Sinavlar s ON r.SinavId = s.Id
            WHERE k.Id IN ({placeholders})
        """
        cur.execute(query, selected_ids)

        for row in cur.fetchall():
            self.tree.insert("", "end", values=row)

class ClassFilterFrame(ttk.Frame):
    def __init__(self, parent):
        super().__init__(parent)
        self.parent = parent
        ttk.Label(self, text="🏫 Sınıf Seçimi", font=("Segoe UI", 14, "bold")).pack(pady=10)

        self.class_var = tk.StringVar()
        self.class_cb = ttk.Combobox(self, textvariable=self.class_var, state="readonly", width=40)
        self.class_cb.pack(pady=5)

        self.load_classes()
        ttk.Button(self, text="Listele", command=self.go_to_results).pack(pady=10)

    def load_classes(self):
        cur = self.parent.conn.cursor()
        cur.execute("SELECT Id, Kodu FROM Siniflar")
        self.classes = cur.fetchall()
        self.class_cb['values'] = [s[1] for s in self.classes]

    def go_to_results(self):
        selected_code = self.class_var.get()
        sinif_id = next((s[0] for s in self.classes if s[1] == selected_code), None)
        if sinif_id:
            self.parent.show_class_results(sinif_id)

class ClassResultsFrame(ttk.Frame):
    def __init__(self, parent, sinif_id):
        super().__init__(parent)
        ttk.Label(self, text="📚 Sınıf Sonuçları", font=("Segoe UI", 14, "bold")).pack(pady=10)

        self.tree = ttk.Treeview(self, columns=("ad", "sinav", "tarih", "puan"), show="headings")
        for col, name in zip(self.tree["columns"], ["Ad", "Sınav", "Tarih", "Puan"]):
            self.tree.heading(col, text=name)
            self.tree.column(col, anchor="center")
        self.tree.pack(fill="both", expand=True, padx=20, pady=10)

        ttk.Button(self, text="📄 Sınıfın Raporunu Görüntüle", command=lambda: None).pack(pady=10)

        self.load_results(sinif_id)

    def load_results(self, sinif_id):
        cur = self.master.conn.cursor()
        cur.execute("""
            SELECT k.KullaniciAdi, s.Adi, s.Tarih, r.ToplamPuan
            FROM SinavSonuclari r
            JOIN Kullanicilar k ON r.KullaniciId = k.Id
            JOIN Sinavlar s ON r.SinavId = s.Id
            WHERE k.SinifId = ?
        """, sinif_id)
        for row in cur.fetchall():
            self.tree.insert("", "end", values=row)

if __name__ == '__main__':
    app = App()
    app.mainloop()
