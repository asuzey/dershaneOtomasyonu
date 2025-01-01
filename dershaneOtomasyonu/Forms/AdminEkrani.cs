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

namespace dershaneOtomasyonu
{
    public partial class AdminEkrani : Form
    {
        private readonly IKullaniciRepository _kullaniciRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly ILogger _logger;
        private readonly IBaseRepository<LogEntry> _baseLogRepository;
        private readonly ILogRepository _logRepository;
        private readonly ISinifRepository _sinifRepository;
        private readonly IDerslerRepository _derslerRepository;
        private readonly IKullaniciDersRepository _kullaniciDersRepository;


        public AdminEkrani(ILogger logger,
            IKullaniciRepository kullaniciRepository,
            IBaseRepository<Role> roleRepository,
            IBaseRepository<LogEntry> baseLogRepository,
            ILogRepository logRepository,
            ISinifRepository sinifRepository,
            IDerslerRepository derslerRepository,
            IKullaniciDersRepository kullaniciDersRepository)
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
        }

        private void CikisYap_Click(object sender, EventArgs e)
        {
            GirisEkrani GirisEkrani = new GirisEkrani(_logger, _kullaniciRepository, _roleRepository, _baseLogRepository, _logRepository, _sinifRepository, _derslerRepository, _kullaniciDersRepository); // form2 ye geçiş
            GirisEkrani.Show(); // form2yi açıyor
            this.Hide(); // form1i gizleyecek
            GirisEkrani.FormClosed += (s, args) => this.Close();
        }

        private void btnKullaniciEklePanel_Click(object sender, EventArgs e)
        {
            // Console.WriteLine("Button 1 clicked"); debugger ile hata kontrolü için kullandığım bir kod 
            TogglePanel(panelKullaniciEkle, panelLog); // Show panel2 and hide panel3
        }

        private async void AdminEkrani_Load(object sender, EventArgs e)
        {
            panelKullaniciEkle.Visible = false;
            panelLog.Visible = false;
            panelKullaniciVeri.Visible = false;
            panelSifreislem.Visible = false;
            panelSinifAtama.Visible = false;
            panelDersveSinif.Visible = false;
            panelDersAtama.Visible = false;
            var roles = await _roleRepository.GetAllAsync();
            cbRol.DataSource = roles;
            cbRol.DisplayMember = "RolAdi";
            cbRol.ValueMember = "Id";
        }

        private void TogglePanel(Panel panelToShow, Panel panelToHide)
        {
            // If the panel to show is currently visible, hide it
            // Otherwise, hide the other panel and show the one you want
            if (panelToShow.Visible)
            {
                panelToShow.Visible = false;
            }
            else
            {
                panelToHide.Visible = false; // Hide the other panel
                panelToShow.Visible = true;   // Show the desired panel
            }
        }

        private async void btnKullaniciEkle_Click(object sender, EventArgs e)// kullancıı ekle
        {
            // Kullanıcı ekleme işlemi
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
            await _kullaniciRepository.AddAsync(yeniKullanici);
            MessageBox.Show("Kullanıcı başarıyla eklendi!");

            // Validator kullanımı
            var validator = new KullaniciValidator();
            ValidationResult result = validator.Validate(yeniKullanici);

            if (!result.IsValid)
            {
                string errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.ErrorMessage));
                MessageBox.Show(errors, "Doğrulama Hataları", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

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
            TogglePanel(panelLog, panelKullaniciEkle); // Show panel3 and hide panel2
            //bunifuPanel3.Visible = !bunifuPanel3.Visible; // Just toggle panel3
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
            panelKullaniciVeri.Visible = !panelKullaniciVeri.Visible;
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
            panelSifreislem.Visible = !panelSifreislem.Visible;
        }

        private async void BtnDersatamaPanel_Click(object sender, EventArgs e)
        {
            panelDersAtama.Visible = !panelDersAtama.Visible;
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
            panelDersveSinif.Visible = !panelDersveSinif.Visible;
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

        private void BtnSinifPanel_Click(object sender, EventArgs e)
        {
            panelSinifAtama.Visible = !panelSinifAtama.Visible;
            // Sınıflara öğrenci atama işleminde kaldık.
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
    }
}
