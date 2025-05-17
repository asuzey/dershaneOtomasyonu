using System.Configuration;
using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.Helpers;
using dershaneOtomasyonu.Repositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DegerlendirmeRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DersKayitRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DerslerRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.GorusmeRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDersRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDosyaRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciNotRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.LogRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.NotRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.SinifRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.YoklamaRepositories;
using Microsoft.Data.SqlClient;

namespace dershaneOtomasyonu
{
    public partial class GirisEkrani : Form
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
        private readonly IGorusmeRepository _gorusmeRepository;
        private readonly IKullaniciNotRepository _kullaniciNotRepository;
        private readonly INotRepository _notRepository;
        private readonly IYoklamaRepository _yoklamaRepository;

        public GirisEkrani(ILogger logger,
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
            IDegerlendirmeRepository degerlendirmeRepository,
            IGorusmeRepository gorusmeRepository,
            IKullaniciNotRepository kullaniciNotRepository,
            INotRepository notRepository,
            IYoklamaRepository yoklamaRepository)

        {
            InitializeComponent();
            GlobalFontHelper.ApplySourceSansToAllControls(this); // Dinamik tüm kontroller
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
            _gorusmeRepository = gorusmeRepository;
            _kullaniciNotRepository = kullaniciNotRepository;
            _notRepository = notRepository;
            _yoklamaRepository = yoklamaRepository;
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
                AdminEkrani form2 = new AdminEkrani(_logger,
                    _kullaniciRepository,
                    _roleRepository,
                    _baseLogRepository,
                    _logRepository,
                    _sinifRepository,
                    _derslerRepository,
                    _kullaniciDersRepository,
                    _baseDosyaRepository,
                    _kullaniciDosyaRepository,
                    _dersKayitRepository,
                    _degerlendirmeRepository,
                    _gorusmeRepository,
                    _kullaniciNotRepository,
                    _notRepository,
                    _yoklamaRepository);
                form2.Show();
                this.Hide();
                form2.FormClosed += (s, args) => this.Close();
            }
            else if (kullanici.RoleId == 2)
            {
                // Personel
                PersonelEkrani form4 = new PersonelEkrani(_logger,
                    _kullaniciRepository,
                    _roleRepository,
                    _baseLogRepository,
                    _logRepository,
                    _sinifRepository,
                    _derslerRepository,
                    _kullaniciDersRepository,
                    _baseDosyaRepository,
                    _kullaniciDosyaRepository,
                    _dersKayitRepository,
                    _degerlendirmeRepository,
                    _gorusmeRepository,
                    _kullaniciNotRepository,
                    _notRepository,
                    _yoklamaRepository);
                form4.Show();
                this.Hide();
                form4.FormClosed += (s, args) => this.Close();
            }
            else if (kullanici.RoleId == 3)
            {
                // Ogrenci
                OgrenciEkrani form3 = new OgrenciEkrani(_logger,
                    _kullaniciRepository,
                    _roleRepository,
                    _baseLogRepository,
                    _logRepository,
                    _sinifRepository,
                    _derslerRepository,
                    _kullaniciDersRepository,
                    _baseDosyaRepository,
                    _kullaniciDosyaRepository,
                    _dersKayitRepository,
                    _degerlendirmeRepository,
                    _gorusmeRepository,
                    _kullaniciNotRepository,
                    _notRepository,
                    _yoklamaRepository);
                form3.Show();
                this.Hide();
                form3.FormClosed += (s, args) => this.Close();
            }
            else
            {
                // Unknown role
                MessageBox.Show("Bilinmeyen rol denemesi, baþarýsýz.");
                await _logger.Error("Bilinmeyen rol denemesi, baþarýsýz.");

            }
        }
    }
}
