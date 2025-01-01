using System;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace dershaneOtomasyonu.Mailer
{
    public class GmailMailer
    {
        private string _smtpServer;
        private int _port;
        private string _username;// maili gönderecek hesabın adı
        private string _password;// maili gönderecek hesabın şifresi

        public GmailMailer()
        {
            _smtpServer = "smtp.gmail.com";
            _port = 587; // Google SMTP için TLS portu
            _username = "asu.proje01@gmail.com";
            _password = "bixg osfi njug vund";//"projec#asu";// bixg osfi njug vund
        }

        /// <summary>
        /// E-posta gönderme işlemi yapar.
        /// </summary>
        /// <param name="to">Alıcı e-posta adresi</param>
        /// <param name="subject">E-posta konusu</param>
        /// <param name="body">E-posta içeriği</param>
        /// <param name="isHtml">E-posta içeriği HTML mi?</param>
        /// <returns>Başarı durumu</returns>
        public bool SendMail(string to, string subject, string body)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(_smtpServer, _port))
                {
                    client.Credentials = new NetworkCredential(_username, _password);
                    client.EnableSsl = true;

                    MailMessage mail = new MailMessage()
                    {
                        From = new MailAddress(_username),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true,
                        BodyEncoding = Encoding.UTF8, // Türkçe karakterler için UTF-8 kodlaması
                        SubjectEncoding = Encoding.UTF8 // Konu başlığı için UTF-8 kodlaması
                    };
                    mail.Headers.Add("Content-Type", "text/html; charset=utf-8");
                    mail.To.Add(to);
                    client.Send(mail);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("E-posta gönderilirken bir hata oluştu: " + ex.Message);
                return false;
            }
        }
    }
}
