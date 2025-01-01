using System.Configuration;
using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.Helpers;
using dershaneOtomasyonu.Repositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DerslerRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDersRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.LogRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.SinifRepositories;
using Microsoft.Data.SqlClient;

namespace dershaneOtomasyonu
{
    public partial class GirisEkrani : Form
    {
        private readonly IKullaniciRepository _kullaniciRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly ILogger _logger;
        private readonly IBaseRepository<LogEntry> _baseLogRepository;
        private readonly ILogRepository _logRepository;
        private readonly ISinifRepository _sinifRepository;
        private readonly IDerslerRepository _derslerRepository;
        private readonly IKullaniciDersRepository _kullaniciDersRepository;


        public GirisEkrani(ILogger logger, 
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

        private async void BtnGirisYap_Click(object sender, EventArgs e)
        {

            var kullanici = await _kullaniciRepository.GetByUserNameAndPasswordAsync(txtAd.Text.Trim(), txtSifre.Text.Trim());
            if (kullanici == null)
            {
                MessageBox.Show("Kullanýcý adý veya þifre hatalý.", "Hata");
                await _logger.Error("Kullanýcý adý veya þifre hatalý.");
                return;
            }
            GlobalData.Kullanici = kullanici;
            await _logger.Info("Giriþ yapýlýyor...");
            if (kullanici.RoleId == 1)
            {
                // Admin
                AdminEkrani form2 = new AdminEkrani(_logger, _kullaniciRepository, _roleRepository, _baseLogRepository, _logRepository, _sinifRepository, _derslerRepository, _kullaniciDersRepository);
                form2.Show();
                this.Hide();
                form2.FormClosed += (s, args) => this.Close();
            }
            else if (kullanici.RoleId == 2)
            {
                // Personel
                PersonelEkrani form4 = new PersonelEkrani(_logger, _kullaniciRepository, _roleRepository, _baseLogRepository, _logRepository, _sinifRepository, _derslerRepository, _kullaniciDersRepository);
                form4.Show();
                this.Hide();
                form4.FormClosed += (s, args) => this.Close();
            }
            else if (kullanici.RoleId == 3)
            {
                // Ogrenci
                OgrenciEkrani form3 = new OgrenciEkrani(_logger, _kullaniciRepository, _roleRepository, _baseLogRepository, _logRepository, _sinifRepository, _derslerRepository, _kullaniciDersRepository);
                form3.Show();
                this.Hide();
                form3.FormClosed += (s, args) => this.Close();
            }
            else
            {
                // Unknown role
                MessageBox.Show("Bilinmeyen rol denemesi, baþarýsýz.");

            }
        }
    }
}
