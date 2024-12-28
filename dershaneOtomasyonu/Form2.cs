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

namespace dershaneOtomasyonu
{
    public partial class AdminEkrani : Form
    {
        private readonly IKullaniciRepository _kullaniciRepository;

        public AdminEkrani(IKullaniciRepository kullaniciRepository)
        {
            InitializeComponent();
            _kullaniciRepository = kullaniciRepository;
        }

        private void CikisYap_Click(object sender, EventArgs e)
        {
            GirisEkrani GirisEkrani = new GirisEkrani(_kullaniciRepository); // form2 ye geçiş
            GirisEkrani.Show(); // form2yi açıyor
            this.Hide(); // form1i gizleyecek
            GirisEkrani.FormClosed += (s, args) => this.Close();
        }

        private void btnKullaniciEklePanel_Click(object sender, EventArgs e)
        {
            // Console.WriteLine("Button 1 clicked"); debugger ile hata kontrolü için kullandığım bir kod 
            TogglePanel(panelKullaniciEkle, panelLog); // Show panel2 and hide panel3
        }

        private void AdminEkrani_Load(object sender, EventArgs e)
        {
            panelKullaniciEkle.Visible = false;
            panelLog.Visible = false;

            LoadLogData();

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

        private string GetIPAddress()
        {
            string ip = System.Net.Dns.GetHostAddresses(System.Net.Dns.GetHostName())
                .FirstOrDefault(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)?.ToString();
            return ip ?? "IP Bulunamadı";
        }

        private void btnKullaniciEkle_Click(object sender, EventArgs e)// kullancıı ekle
        {
            // TextBox'lardan veri alın
            string kullaniciAd = txtAd.Text;
            string sifre = txtSifre.Text;
            string rol = cbRol.Text;

            // Veritabanı bağlantı dizesi
            string connectionString = ConfigurationManager.ConnectionStrings["DershaneDB"].ConnectionString;

            string query = @"
            INSERT INTO Kullanici (kullaniciAd, sifre, rol)
            OUTPUT INSERTED.kullaniciid
            VALUES (@kullaniciAd, @sifre, @rol)";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Parametreleri ekle
                        command.Parameters.AddWithValue("@kullaniciAd", string.IsNullOrEmpty(kullaniciAd) ? (object)DBNull.Value : (object)kullaniciAd);
                        command.Parameters.AddWithValue("@sifre", string.IsNullOrEmpty(sifre) ? (object)DBNull.Value : (object)sifre);
                        command.Parameters.AddWithValue("@rol", string.IsNullOrEmpty(rol) ? (object)DBNull.Value : (object)rol);

                        // Yeni kaydın ID'sini al
                        object idResult = command.ExecuteScalar();

                        if (idResult != null && idResult != DBNull.Value)
                        {
                            int yeniKullaniciId = Convert.ToInt32(idResult);

                            // Kullanıcı ID'si veritabanında mevcutsa Log Tablosuna Ekle
                            string ipAdres = GetIPAddress();
                            string islem = "Yeni kullanıcı eklendi: " + kullaniciAd;

                            LogEkle(yeniKullaniciId, islem, ipAdres);

                            MessageBox.Show("Kullanıcı başarıyla eklendi!");
                        }
                        else
                        {
                            MessageBox.Show("Kullanıcı eklenemedi, geçerli bir kullanıcı ID'si bulunamadı.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}");
                }
            }



        }

        private void btnLogPanel_Click(object sender, EventArgs e) // burası
        {
            TogglePanel(panelLog, panelKullaniciEkle); // Show panel3 and hide panel2
            //bunifuPanel3.Visible = !bunifuPanel3.Visible; // Just toggle panel3
            LoadLogData();
        }

        private int GetNewKullaniciId(SqlConnection connection)
        {
            string getIdQuery = "SELECT IDENT_CURRENT('Kullanici')";
            SqlCommand getIdCommand = new SqlCommand(getIdQuery, connection);
            return Convert.ToInt32(getIdCommand.ExecuteScalar());
        }

        private void LoadLogData()
        {
            // Kullanıcı loglarını sorgulayan SQL cümlesi
            string query = @"
                SELECT 
                    l.LogID, 
                    k.kullaniciAd AS KullanıcıAdı, 
                    l.Islem AS Yapılanİşlem, 
                    l.IslemTarihi AS İşlemTarihi, 
                    l.IPAdres AS IPAdresi, 
                    l.EkBilgi AS EkBilgi 
                FROM KullaniciLog l
                INNER JOIN Kullanici k ON l.KullaniciID = k.kullaniciid
                ORDER BY l.IslemTarihi DESC";

            // Bağlantı dizesi, App.config'deki ConnectionStrings bölümünden alınır
            string connectionString = ConfigurationManager.ConnectionStrings["DershaneDB"].ConnectionString;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Verileri almak için SqlDataAdapter kullanılır
                    SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection);
                    DataTable dataTable = new DataTable();
                    dataAdapter.Fill(dataTable);

                    // Verileri DataGridView'e bağla
                    dataGridView1.DataSource = dataTable;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Log verilerini yüklerken bir hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LogEkle(int kullaniciId, string islem, string ipAdres, string ekBilgi = null)
        {
            string query = @"
        INSERT INTO KullaniciLog (KullaniciID, Islem, IslemTarihi, IPAdres, EkBilgi)  
        VALUES (@KullaniciID, @Islem, GETDATE(), @IPAdres, @EkBilgi)";

            string connectionString = ConfigurationManager.ConnectionStrings["DershaneDB"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@KullaniciID", kullaniciId);
                    command.Parameters.AddWithValue("@Islem", islem);
                    // Null kontrolü yaparak IP adresini ekle
                    command.Parameters.AddWithValue("@IPAdres", (object)ipAdres ?? DBNull.Value);
                    // Ek bilgi de null ise DBNull.Value kullan
                    command.Parameters.AddWithValue("@EkBilgi", (object)ekBilgi ?? DBNull.Value);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
