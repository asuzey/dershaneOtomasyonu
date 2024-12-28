using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciRepositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dershaneOtomasyonu
{
    public partial class OgrenciEkrani : Form
    {
        private readonly IKullaniciRepository _kullaniciRepository;

        public OgrenciEkrani(IKullaniciRepository kullaniciRepository)
        {
            InitializeComponent();
            _kullaniciRepository = kullaniciRepository;
        }

        private void CikisYap_Click(object sender, EventArgs e)
        {
            GirisEkrani OgrenciEkrani = new GirisEkrani(_kullaniciRepository); // form3e geçiş
            OgrenciEkrani.Show(); // form3ü açıyor
            this.Hide(); // form1i gizleyecek
            OgrenciEkrani.FormClosed += (s, args) => this.Close();
        }

        private void OgrenciEkrani_Load(object sender, EventArgs e)
        {
            kullaniciadogr.Text = GlobalData.KullaniciAd;
        }
    }
}
