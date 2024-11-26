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
            textBox2.PasswordChar = '*'; // þifrenin yazým sýrasýnda * ile gösterilmesini saðlýyor.

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            bunifuButton1.Enabled = false; //rol seçimi için
            radioButton1.CheckedChanged += RadioButton_CheckedChanged;
            radioButton2.CheckedChanged += RadioButton_CheckedChanged;
            radioButton3.CheckedChanged += RadioButton_CheckedChanged;

        }


        private void RadioButton_CheckedChanged(object? sender, EventArgs e)
        {

            // Herhangi bir radiobutton seçiliyse butonu etkinleþtir
            if (radioButton1.Checked || radioButton2.Checked || radioButton3.Checked)
            {
                bunifuButton1.Enabled = true;
            }
            else
            {
                bunifuButton1.Enabled = false; // Hiçbiri seçilmediyse buton devre dýþý
            }
        }

        private void bunifuButton1_Click(object sender, EventArgs e)
        {
            

            // Kullanýcý adý ve þifreyi alýyoruz
            string kullaniciAd = textBox1.Text.Trim();
            string sifre = textBox2.Text.Trim();
            string rol = radioButton1.Text.Trim();
            // Rol belirleme (radioButton kontrolleri)
            if (radioButton3.Checked)
                rol = "Admin";
            else if (radioButton1.Checked)
                rol = "Öðrenci";
            else if (radioButton2.Checked)
                rol = "Personel";
            

            // Baðlantý dizesini alýyoruz
            string connectionString = ConfigurationManager.ConnectionStrings["DershaneDB"].ConnectionString;

            // SQL sorgusu
            string query = "SELECT COUNT(1) FROM Kullanici WHERE kullaniciAd = @kullaniciAd AND sifre = @sifre AND rol = @rol";

            // Veritabaný baðlantýsýný kuruyoruz
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
                    Console.WriteLine("Seçilen rol: " + rol);



                    // Eðer kullanýcý adý ve þifre eþleþiyorsa
                    if (count == 1)
                    {
                        

                        MessageBox.Show("Giriþ baþarýlý!", "Baþarýlý", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (rol == "Admin")
                        {
                            // admin sayfasýna yönlendir
                            //MessageBox.Show("Admin giriþi onaylandý.");
                            Form2 form2 = new Form2(); // form2 ye geçiþ
                            form2.Show(); // form2yi açýyor
                            this.Hide(); // form1i gizleyecek
                            form2.FormClosed += (s, args) => this.Close();
                        }
                        else if (rol == "Öðrenci")
                        {
                            // Ogrenci sayfasýna yönlendir
                            Form3 form3 = new Form3();
                            form3.Show();
                            this.Hide(); 
                            form3.FormClosed += (s, args) => this.Close();


                        }
                        else if (rol == "Personel")
                        {
                            // Personel sayfasýna yönlendir
                            Form4 form4 = new Form4();
                            form4.Show();
                            this.Hide();
                            form4.FormClosed += (s, args) => this.Close();

                        }
                        else
                        {
                            // bilinmeyen rol giriþi yapýldý, hata patlatalým
                            MessageBox.Show("Bilinmeyen rol denemesi, baþarýsýz.");

                        }
                    }
                    else
                    {
                        MessageBox.Show("Kullanýcý adý veya þifre hatalý.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Baðlantý hatasý: " + ex.Message);
                }
            }
        }
    }
}
