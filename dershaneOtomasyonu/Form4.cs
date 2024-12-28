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
    public partial class Form4 : Form
    {
        private readonly IKullaniciRepository _kullaniciRepository;

        public Form4(IKullaniciRepository kullaniciRepository)
        {
            InitializeComponent();
            _kullaniciRepository = kullaniciRepository;
        }

        private void CikisYap_Click(object sender, EventArgs e)
        {
            GirisEkrani form1 = new GirisEkrani(_kullaniciRepository); // form4e geçiş
            form1.Show(); // form4ü açıyor
            this.Hide(); // form1i gizleyecek
            form1.FormClosed += (s, args) => this.Close();
        }

        private void Form4_Load(object sender, EventArgs e)
        {

        }
    }
}
