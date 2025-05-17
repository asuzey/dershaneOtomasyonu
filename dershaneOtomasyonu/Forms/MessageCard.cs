using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace dershaneOtomasyonu.Forms
{
    public partial class MessageCard : UserControl
    {
        public Kullanici _kullanici { get; set; } // Kullanıcı bilgisi
        public string _message { get; set; } // Mesaj içeriği
        public DateTime _date { get; set; } // Mesaj tarihi

        public MessageCard(Kullanici kullanici, string message, DateTime date, bool isOwnMessage)
        {
            InitializeComponent();
            GlobalFontHelper.ApplySourceSansToAllControls(this); // Dinamik tüm kontroller
            _kullanici = kullanici;
            _message = message;
            _date = date;
            // Kullanıcı adını yazdır
            label2.Text = $"{kullanici.Adi} {kullanici.Soyadi}";

            // Mesaj içeriği için TextBox oluştur ve ekle
            var messageTextBox = CreateSelectableLabelWithMaxWidth(message, 532, label2.Font, isOwnMessage);
            messageTextBox.Location = new Point(13, 29); // Konum
            this.Controls.Add(messageTextBox);

            // Mesaj saatini yazdır
            label3.Text = date.ToString("HH:mm");

            // Kendi mesajıysa farklı arka plan rengi
            if (isOwnMessage)
            {
                this.BackColor = Color.FromArgb(156, 173, 199);
            }
        }

        private TextBox CreateSelectableLabelWithMaxWidth(string text, int maxWidth, Font font, bool isOwnMessage)
        {
            // TextBox oluştur
            TextBox textBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                BorderStyle = BorderStyle.None,
                BackColor = isOwnMessage ? Color.FromArgb(156, 173, 199) : SystemColors.Control,
                TabStop = false,
                Cursor = Cursors.IBeam,
                Font = font, // Yazı tipi
                WordWrap = true // Metni otomatik olarak satırlara böl
            };

            // Metni TextBox'a ata
            textBox.Text = text;

            // TextBox'ın yüksekliğini içeriğe göre ayarla
            using (Graphics g = textBox.CreateGraphics())
            {
                // TextBox içindeki metni ölç
                SizeF textSize = g.MeasureString(text, font, maxWidth);

                // TextBox genişlik ve yükseklik ayarı
                textBox.Width = maxWidth;
                textBox.Height = (int)Math.Ceiling(textSize.Height); // Yüksekliği içeriğe göre ayarla
            }

            return textBox;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            // Parent FlowLayoutPanel'e erişim
            if (this.Parent is FlowLayoutPanel panel)
            {
                // Mevcut scroll pozisyonunu güncelle
                int newScrollValue = panel.VerticalScroll.Value - e.Delta;
                if (newScrollValue < panel.VerticalScroll.Minimum)
                {
                    newScrollValue = panel.VerticalScroll.Minimum;
                }
                else if (newScrollValue > panel.VerticalScroll.Maximum)
                {
                    newScrollValue = panel.VerticalScroll.Maximum;
                }

                // Scroll pozisyonunu ayarla
                panel.VerticalScroll.Value = newScrollValue;
                panel.PerformLayout(); // Layout'u güncelle
            }
        }

        private void MessageCard_Load(object sender, EventArgs e)
        {

        }
    }
}
