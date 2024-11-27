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

namespace dershaneOtomasyonu
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(); // form2 ye geçiş
            form1.Show(); // form2yi açıyor
            this.Hide(); // form1i gizleyecek
            form1.FormClosed += (s, args) => this.Close();
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Button 1 clicked");
            TogglePanel(bunifuPanel2, bunifuPanel3); // Show panel2 and hide panel3
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            bunifuPanel2.Visible = false;
            bunifuPanel3.Visible = false;

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

        private void bunifuButton5_Click(object sender, EventArgs e)
        {
            // TextBox'lardan veri alın
            string kullaniciAd = textBox1.Text;
            string sifre = textBox2.Text;
            string rol = textBox3.Text;

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

        private void bunifuButton4_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Button 4 clicked");
            TogglePanel(bunifuPanel3, bunifuPanel2); // Show panel3 and hide panel2
            //bunifuPanel3.Visible = !bunifuPanel3.Visible; // Just toggle panel3
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
