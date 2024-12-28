using System.Configuration;
using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.Repositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciRepositories;
using Microsoft.Data.SqlClient;
using NLog;

namespace dershaneOtomasyonu
{
    public partial class GirisEkrani : Form
    {
        private readonly IKullaniciRepository _kullaniciRepository;

        public GirisEkrani(IKullaniciRepository kullaniciRepository)
        {
            InitializeComponent();
            _kullaniciRepository = kullaniciRepository;
        }

        private async void BtnGirisYap_Click(object sender, EventArgs e)
        {
            var kullanici = await _kullaniciRepository.GetByUserNameAndPasswordAsync(txtAd.Text.Trim(), txtSifre.Text.Trim());
            if (kullanici.RoleId == 1)
            {
                // Admin
                AdminEkrani form2 = new AdminEkrani(_kullaniciRepository);
                form2.Show();
                this.Hide();
                form2.FormClosed += (s, args) => this.Close();
            }
            else if (kullanici.RoleId == 2)
            {
                // Personel
                Form4 form4 = new Form4(_kullaniciRepository);
                form4.Show();
                this.Hide();
                form4.FormClosed += (s, args) => this.Close();
            }
            else if (kullanici.RoleId == 3)
            {
                // Ogrenci
                OgrenciEkrani form3 = new OgrenciEkrani(_kullaniciRepository);
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
