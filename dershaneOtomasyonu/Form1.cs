using System.Configuration;
using Microsoft.Data.SqlClient;

namespace dershaneOtomasyonu
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            textBox2.PasswordChar = '*'; // �ifrenin yaz�m s�ras�nda * ile g�sterilmesini sa�l�yor.

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bunifuButton1.Enabled = false; //rol se�imi i�in
            radioButton1.CheckedChanged += RadioButton_CheckedChanged;
            radioButton2.CheckedChanged += RadioButton_CheckedChanged;
            radioButton3.CheckedChanged += RadioButton_CheckedChanged;

        }


        private void RadioButton_CheckedChanged(object? sender, EventArgs e)
        {

            // Herhangi bir radiobutton se�iliyse butonu etkinle�tir
            if (radioButton1.Checked || radioButton2.Checked || radioButton3.Checked)
            {
                bunifuButton1.Enabled = true;
            }
            else
            {
                bunifuButton1.Enabled = false; // Hi�biri se�ilmediyse buton devre d���
            }
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            

            // Kullan�c� ad� ve �ifreyi al�yoruz
            string kullaniciAd = textBox1.Text.Trim();
            string sifre = textBox2.Text.Trim();
            string rol = radioButton1.Text.Trim();
            // Rol belirleme (radioButton kontrolleri)
            if (radioButton3.Checked)
                rol = "Admin";
            else if (radioButton1.Checked)
                rol = "��renci";
            else if (radioButton2.Checked)
                rol = "Personel";
            

            // Ba�lant� dizesini al�yoruz
            string connectionString = ConfigurationManager.ConnectionStrings["DershaneDB"].ConnectionString;

            // SQL sorgusu
            string query = "SELECT COUNT(1) FROM Kullanici WHERE kullaniciAd = @kullaniciAd AND sifre = @sifre AND rol = @rol";

            // Veritaban� ba�lant�s�n� kuruyoruz
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);

                // Parametreleri ekliyoruz
                command.Parameters.AddWithValue("@kullaniciAd", kullaniciAd);
                command.Parameters.AddWithValue("@sifre", sifre);
                command.Parameters.AddWithValue("@rol", rol);


                try
                {
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    Console.WriteLine("SQL Query: " + query);
                    Console.WriteLine("Parameters: " + "kullaniciAd=" + kullaniciAd + ", sifre=" + sifre + ", rol=" + rol);
                    Console.WriteLine("Se�ilen rol: " + rol);



                    // E�er kullan�c� ad� ve �ifre e�le�iyorsa
                    if (count == 1)
                    {
                        

                        MessageBox.Show("Giri� ba�ar�l�!", "Ba�ar�l�", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (rol == "Admin")
                        {
                            // admin sayfas�na y�nlendir
                            //MessageBox.Show("Admin giri�i onayland�.");
                            Form2 form2 = new Form2(); // form2 ye ge�i�
                            form2.Show(); // form2yi a��yor
                            this.Hide(); // form1i gizleyecek
                            form2.FormClosed += (s, args) => this.Close();
                        }
                        else if (rol == "��renci")
                        {
                            // Ogrenci sayfas�na y�nlendir
                            Form3 form3 = new Form3();
                            form3.Show();
                            this.Hide(); 
                            form3.FormClosed += (s, args) => this.Close();


                        }
                        else if (rol == "Personel")
                        {
                            // Personel sayfas�na y�nlendir
                            Form4 form4 = new Form4();
                            form4.Show();
                            this.Hide();
                            form4.FormClosed += (s, args) => this.Close();

                        }
                        else
                        {
                            // bilinmeyen rol giri�i yap�ld�, hata patlatal�m
                            MessageBox.Show("Bilinmeyen rol denemesi, ba�ar�s�z.");

                        }
                    }
                    else
                    {
                        MessageBox.Show("Kullan�c� ad� veya �ifre hatal�.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ba�lant� hatas�: " + ex.Message);
                }
            }
        }
    }
}
