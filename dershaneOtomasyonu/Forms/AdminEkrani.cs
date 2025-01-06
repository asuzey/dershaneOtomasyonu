using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Microsoft.Data.SqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciRepositories;
using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.Repositories;
using dershaneOtomasyonu.Mailer;
using System.IO;
using dershaneOtomasyonu.Helpers;
using dershaneOtomasyonu.Repositories.TableRepositories.LogRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.SinifRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DerslerRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDersRepositories;
using System.Security.Cryptography;
using FluentValidation.Results;
using FluentValidation;
using Bunifu.UI.WinForms;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDosyaRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DersKayitRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DegerlendirmeRepositories;

namespace dershaneOtomasyonu
{
    public partial class AdminEkrani : Form
    {
        private readonly IKullaniciRepository _kullaniciRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly ILogger _logger;
        private readonly IBaseRepository<LogEntry> _baseLogRepository;
        private readonly IBaseRepository<Dosya> _baseDosyaRepository;
        private readonly IKullaniciDosyaRepository _kullaniciDosyaRepository;
        private readonly ILogRepository _logRepository;
        private readonly ISinifRepository _sinifRepository;
        private readonly IDerslerRepository _derslerRepository;
        private readonly IKullaniciDersRepository _kullaniciDersRepository;
        private readonly IDersKayitRepository _dersKayitRepository;
        private readonly IDegerlendirmeRepository _degerlendirmeRepository;


        public AdminEkrani(ILogger logger,
            IKullaniciRepository kullaniciRepository,
            IBaseRepository<Role> roleRepository,
            IBaseRepository<LogEntry> baseLogRepository,
            ILogRepository logRepository,
            ISinifRepository sinifRepository,
            IDerslerRepository derslerRepository,
            IKullaniciDersRepository kullaniciDersRepository,
            IBaseRepository<Dosya> baseDosyaRepository,
            IKullaniciDosyaRepository kullaniciDosyaRepository,
            IDersKayitRepository dersKayitRepository,
            IDegerlendirmeRepository degerlendirmeRepository)
        {
            InitializeComponent();
            _kullaniciRepository = kullaniciRepository;
            _roleRepository = roleRepository;
            _logger = logger;
            _baseLogRepository = baseLogRepository;
            _logRepository = logRepository;
            _sinifRepository = sinifRepository;
            _derslerRepository = derslerRepository;
            _kullaniciDersRepository = kullaniciDersRepository;
            _baseDosyaRepository = baseDosyaRepository;
            _kullaniciDosyaRepository = kullaniciDosyaRepository;
            _dersKayitRepository = dersKayitRepository;
            _degerlendirmeRepository = degerlendirmeRepository;
            panels = [panelKullaniciEkle, panelLog, panelKullaniciVeri, panelSifreislem, panelDersAtama, panelDersveSinif, panelSinifAtama];
        }

        public static BunifuPanel[] panels;

        private void CikisYap_Click(object sender, EventArgs e)
        {
            GirisEkrani GirisEkrani = new GirisEkrani(_logger, _kullaniciRepository, _roleRepository, _baseLogRepository, _logRepository, _sinifRepository, _derslerRepository, _kullaniciDersRepository, _baseDosyaRepository, _kullaniciDosyaRepository, _dersKayitRepository, _degerlendirmeRepository); // form2 ye geçiş
            GirisEkrani.Show(); // form2yi açıyor
            this.Hide(); // form1i gizleyecek
            GirisEkrani.FormClosed += (s, args) => this.Close();
        }

        private void btnKullaniciEklePanel_Click(object sender, EventArgs e)
        {
            // Console.WriteLine("Button 1 clicked"); debugger ile hata kontrolü için kullandığım bir kod 
            TogglePanel(panelKullaniciEkle); // Show panel2 and hide panel3
        }

        private async void AdminEkrani_Load(object sender, EventArgs e)
        {
            var roles = await _roleRepository.GetAllAsync();
            cbRol.DataSource = roles;
            cbRol.DisplayMember = "RolAdi";
            cbRol.ValueMember = "Id";
        }

        private void TogglePanel(BunifuPanel panelToToggle)
        {
            foreach (var panel in panels)
            {
                if (panel == panelToToggle)
                {
                    // Seçili panelin görünürlük durumunu tersine çevirir
                    panel.Visible = !panel.Visible;
                }
                else
                {
                    // Diğer panelleri gizler
                    panel.Visible = false;
                }
            }
        }

        private async void btnKullaniciEkle_Click(object sender, EventArgs e)// kullancıı ekle
        {
            var password = GenerateRandomPassword();
            var yeniKullanici = new Kullanici();
            yeniKullanici.KullaniciAdi = txt_kullaniciad.Text;
            yeniKullanici.RoleId = (int)cbRol.SelectedValue;
            yeniKullanici.Sifre = password;
            yeniKullanici.Adi = txt_ad.Text;
            yeniKullanici.Soyadi = txt_soyad.Text;
            yeniKullanici.Tcno = txt_tcno.Text;
            yeniKullanici.DogumTarihi = DateTime.Parse(txt_dogtar.Text);
            yeniKullanici.Telefon = txt_telno.Text;
            yeniKullanici.Email = txt_email.Text;
            yeniKullanici.Adres = txt_adres.Text;

            // Validator kullanımı
            var validator = new KullaniciValidator();
            ValidationResult result = validator.Validate(yeniKullanici);

            if (!result.IsValid)
            {
                string errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.ErrorMessage));
                MessageBox.Show(errors, "Doğrulama Hataları", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kullanıcı ekleme işlemi
            await _kullaniciRepository.AddAsync(yeniKullanici);
            MessageBox.Show("Kullanıcı başarıyla eklendi!");

            // Mail kodları
            GmailMailer mailer = new GmailMailer();
            string recipient = txt_email.Text;
            string subject = "Yeni Şifre";
            string projectRoot = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string templatePath = Path.Combine(projectRoot, "Mailer", "create-password.html");
            string body = File.ReadAllText(templatePath, Encoding.UTF8);
            body = body.Replace("${newPassword}", password);
            bool success = mailer.SendMail(recipient, subject, body);
            if (success)
            {
                MessageBox.Show("Yeni şifre e-posta adresinize gönderildi.");
            }

            else
                MessageBox.Show("E-posta gönderilirken bir hata oluştu.");

        }

        static string GenerateRandomPassword()
        {
            Random random = new Random();
            string password = "";

            for (int i = 0; i < 10; i++)
            {
                password += random.Next(0, 10); // 0-9 arasında rastgele bir sayı ekler
            }

            return password;
        }

        private void btnLogPanel_Click(object sender, EventArgs e) // burası
        {
            TogglePanel(panelLog); // Show panel3 and hide panel2
            LoadLogData();
        }

        private async void LoadLogData()
        {
            //var logs = await _baseLogRepository.GetAllAsync(); // get all gibi baseden gelen bir komut çağırmak için güncelledim
            var logs = await _logRepository.GetAllAsDtoAsync();
            grid_logs.DataSource = logs;
        }

        private async void btnKullaniciVeriPanel_Click(object sender, EventArgs e)
        {
            TogglePanel(panelKullaniciVeri);
            var kullaniciList = await _kullaniciRepository.GetAllAsDtoAsync();
            kullaniciVeri.DataSource = kullaniciList;
        }

        private async void btnSifirla_Click(object sender, EventArgs e)
        {
            var kullanici = await _kullaniciRepository.GetByEmailAsync(txtMail.Text);
            if (kullanici == null)
            {
                MessageBox.Show("Kullanıcı bulunamadı.");
                return;
            }
            GmailMailer mailer = new GmailMailer();
            string recipient = txtMail.Text;
            string subject = "Yeni Şifre";
            string projectRoot = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string templatePath = Path.Combine(projectRoot, "Mailer", "create-password.html");
            string body = File.ReadAllText(templatePath, Encoding.UTF8);
            var password = GenerateRandomPassword();
            body = body.Replace("${newPassword}", password);
            bool success = mailer.SendMail(recipient, subject, body);
            if (success)
            {
                MessageBox.Show("Yeni şifre e-posta adresinize gönderildi.");
                kullanici.Sifre = password;
                await _kullaniciRepository.UpdateAsync(kullanici);
            }

            else
                MessageBox.Show("E-posta gönderilirken bir hata oluştu.");
        }

        private void btnSifrePanel_Click(object sender, EventArgs e)
        {
            TogglePanel(panelSifreislem);
        }

        private async void BtnDersatamaPanel_Click(object sender, EventArgs e)
        {
            TogglePanel(panelDersAtama);
            // Öğretmenleri çekelim
            var ogretmenler = await _kullaniciRepository.GetAllTeachersAsync();
            var dersler = await _derslerRepository.GetAllAsync();
            OgretmenlerDataGridView.DataSource = ogretmenler;
            DerslerDataGridView.DataSource = dersler;
            foreach (DataGridViewColumn column in OgretmenlerDataGridView.Columns)
            {
                column.Visible = false;
            }
            // Sadece 3. ve 4. indexli kolonları görünür yap
            OgretmenlerDataGridView.Columns[4].Visible = true;
            OgretmenlerDataGridView.Columns[5].Visible = true;

            DerslerDataGridView.Columns[0].Visible = false;
            DerslerDataGridView.Columns[3].Visible = false;
        }

        private async void BtnDersolusturPanel_Click(object sender, EventArgs e)
        {
            TogglePanel(panelDersveSinif);
            await LoadDersler();
            await LoadSiniflar();
        }

        private async Task LoadDersler()
        {
            var dersler = await _derslerRepository.GetAllAsDtoAsync();
            olusanDersDataGridView.DataSource = dersler;
        }

        private async Task LoadSiniflar()
        {
            var siniflar = await _sinifRepository.GetAllAsDtoAsync();
            olusanSinifDataGridView2.DataSource = siniflar;
        }

        private async Task LoadAtanmisDersler(int ogrId)
        {
            var dersler = await _kullaniciDersRepository.GetAllByKullaniciIdAsync(ogrId);
            var derslerWithDersAdi = dersler.Select(d => new
            {
                d.KullaniciId,
                d.DersId,
                DersAdi = d.Ders?.Adi ?? "Bilinmiyor"
            }).ToList();

            atanmisDersDataGridView.DataSource = derslerWithDersAdi;
            atanmisDersDataGridView.Columns[0].Visible = false;
            atanmisDersDataGridView.Columns[1].Visible = false;
        }

        private async Task LoadAtanmisOgrenciler(int sinifId)
        {
            var ogrenciler = await _kullaniciRepository.GetAllStudentsAsync();
            var sinifinOgrencileri = ogrenciler.Where(o => o.SinifId == sinifId).ToList();
            SinifinOgrDataGridView.DataSource = sinifinOgrencileri;
            foreach (DataGridViewColumn column in SinifinOgrDataGridView.Columns)
            {
                column.Visible = false;
            }
            SinifinOgrDataGridView.Columns[5].Visible = true;
            SinifinOgrDataGridView.Columns[6].Visible = true;
        }

        private async Task LoadAtanmamisOgrenciler()
        {
            var ogrenciler = await _kullaniciRepository.GetAllStudentsAsync();
            var sinifsizOgrenciler = ogrenciler.Where(o => o.SinifId == null).ToList();
            sinifsizlarDataGridView.DataSource = sinifsizOgrenciler;
            foreach (DataGridViewColumn column in sinifsizlarDataGridView.Columns)
            {
                column.Visible = false;
            }
            sinifsizlarDataGridView.Columns[5].Visible = true;
            sinifsizlarDataGridView.Columns[6].Visible = true;
        }

        private async void btnDersEkle_Click(object sender, EventArgs e)
        {
            var newders = new Ders();
            newders.Adi = txt_DersAdi.Text;
            newders.Aciklama = txt_DersAciklama.Text;
            await _derslerRepository.AddAsync(newders);
            LoadDersler();
        }

        private async void btnSinifEkle_Click(object sender, EventArgs e)
        {
            var newSinif = new Sinif();
            newSinif.Kodu = txt_SinifKodu.Text;
            await _sinifRepository.AddAsync(newSinif);
            LoadSiniflar();
        }

        private async void BtnSinifPanel_Click(object sender, EventArgs e)
        {

            var siniflar = await _sinifRepository.GetAllAsync();
            siniflarDataGridView.DataSource = siniflar;
            siniflarDataGridView.Columns[0].Visible = false;

            await LoadAtanmamisOgrenciler();
            TogglePanel(panelSinifAtama);
        }

        private async void Btn_AtamaYap_Click(object sender, EventArgs e)
        {
            if (OgretmenlerDataGridView.CurrentRow == null || DerslerDataGridView.CurrentRow == null)
            {
                MessageBox.Show("Lütfen hem öğretmen hemde ders tablosundan kayıt seçiniz.");
                return;
            }

            var ogrId = Convert.ToInt32(OgretmenlerDataGridView.CurrentRow.Cells[0].Value);
            var dersId = Convert.ToInt32(DerslerDataGridView.CurrentRow.Cells[0].Value);
            var isAssigned = await _kullaniciDersRepository.GetByKullaniciIdAndDersIdAsync(ogrId, dersId);
            if (isAssigned == null)
            {
                var newKullaniciDersi = new KullaniciDers();
                newKullaniciDersi.KullaniciId = ogrId;
                newKullaniciDersi.DersId = dersId;
                await _kullaniciDersRepository.AddAsync(newKullaniciDersi);
            }
            await LoadAtanmisDersler(ogrId);


        }

        private async void btnAtamayiKaldir_Click(object sender, EventArgs e)
        {
            if (atanmisDersDataGridView.CurrentRow == null)
            {
                MessageBox.Show("Lütfen hem atanmış dersler tablosundan kayıt seçiniz.");
                return;
            }
            var ogrId = Convert.ToInt32(atanmisDersDataGridView.CurrentRow.Cells[0].Value);
            var dersId = Convert.ToInt32(atanmisDersDataGridView.CurrentRow.Cells[1].Value);
            var isAssigned = await _kullaniciDersRepository.GetByKullaniciIdAndDersIdAsync(ogrId, dersId);
            if (isAssigned != null)
            {
                await _kullaniciDersRepository.DeleteByOgretmenIdAndDersIdAsync(ogrId, dersId);
                await LoadAtanmisDersler(ogrId);
            }
        }

        private async void OgretmenlerDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            int ogrId = 0;
            if (OgretmenlerDataGridView.CurrentRow != null) Convert.ToInt32(OgretmenlerDataGridView.CurrentRow.Cells[0].Value);
            await LoadAtanmisDersler(ogrId);
        }

        private void atanmisDersDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            var dersAdi = atanmisDersDataGridView.CurrentRow != null ? atanmisDersDataGridView.CurrentRow.Cells[2].Value.ToString() : null;
            seciliders.Text = dersAdi;
        }

        private void logo_Click(object sender, EventArgs e)
        {
            foreach (var panel in panels)
            {
                panel.Visible = false;
            }
        }

        private async void siniflarDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            var sinifId = 0;
            if (siniflarDataGridView.CurrentRow != null) sinifId = Convert.ToInt32(siniflarDataGridView.CurrentRow.Cells[0].Value);
            if (sinifId != 0) await LoadAtanmisOgrenciler(sinifId);

        }

        private async void BtnAtamaYap_Click(object sender, EventArgs e)
        {
            if (siniflarDataGridView.CurrentRow == null || sinifsizlarDataGridView.CurrentRow == null)
            {
                MessageBox.Show("Lütfen hem sınıf hemde sınıfsız öğrenciler tablosundan kayıt seçiniz.");
                return;
            }

            var sinifId = Convert.ToInt32(siniflarDataGridView.CurrentRow.Cells[0].Value);
            var ogrenciId = Convert.ToInt32(sinifsizlarDataGridView.CurrentRow.Cells[0].Value);
            var ogrenci = await _kullaniciRepository.GetByIdAsync(ogrenciId);
            if (ogrenci != null)
            {
                ogrenci.SinifId = sinifId;
                await _kullaniciRepository.UpdateAsync(ogrenci);
                await LoadAtanmisOgrenciler(sinifId);
                await LoadAtanmamisOgrenciler();
            }
        }

        private void SinifinOgrDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            var ogrenciAdi = SinifinOgrDataGridView.CurrentRow != null ? SinifinOgrDataGridView.CurrentRow.Cells[5].Value.ToString() + " " + SinifinOgrDataGridView.CurrentRow.Cells[6].Value.ToString() : null;
            seciliogrenci.Text = ogrenciAdi;
        }

        private async void BtnAtamaKaldir_Click(object sender, EventArgs e)
        {
            if (SinifinOgrDataGridView.CurrentRow == null && siniflarDataGridView.CurrentRow == null)
            {
                MessageBox.Show("Lütfen hem sınıfın öğrencileri ve sınıflar tablosundan kayıt seçiniz.");
                return;
            }
            var ogrId = Convert.ToInt32(SinifinOgrDataGridView.CurrentRow.Cells[0].Value);
            var sinifId = Convert.ToInt32(siniflarDataGridView.CurrentRow.Cells[0].Value);
            var ogrenci = await _kullaniciRepository.GetByIdAsync(ogrId);
            if (ogrenci != null)
            {
                ogrenci.SinifId = null;
                await _kullaniciRepository.UpdateAsync(ogrenci);
                await LoadAtanmisOgrenciler(sinifId);
                await LoadAtanmamisOgrenciler();
            }
        }
    }
}
