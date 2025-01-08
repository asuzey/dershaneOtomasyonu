using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
using dershaneOtomasyonu.Helpers;
using dershaneOtomasyonu.Repositories.TableRepositories.DersKayitRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.GorusmeRepositories;
using Mapster;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dershaneOtomasyonu.Forms
{
    public partial class ChattingForm : Form
    {
        private readonly IDersKayitRepository _dersKayitRepository;
        private readonly IGorusmeRepository _gorusmeRepository;
        private WebSocketClient _webSocketClient;
        private DersKayit _newDersKayit;
        private Gorusme _newGorusme;

        public ChattingForm(DersKayit newDersKayit,
            IDersKayitRepository dersKayitRepository)
        {
            InitializeComponent();
            _newDersKayit = newDersKayit;
            _webSocketClient = new WebSocketClient();
            _dersKayitRepository = dersKayitRepository;
        }

        public ChattingForm(Gorusme newGorusme,
            IGorusmeRepository gorusmeRepository)
        {
            InitializeComponent();
            _newGorusme = newGorusme;
            _webSocketClient = new WebSocketClient();
            _gorusmeRepository = gorusmeRepository;
        }

        private async void ChattingForm_Load(object sender, EventArgs e)
        {
            try
            {
                await _webSocketClient.ConnectAsync("ws://localhost:5000/ws");
                var userMessage = new
                {
                    type = "userInfo",
                    data = GlobalData.Kullanici.Adapt<KullaniciWithoutNavPropDto>()
                };
                await _webSocketClient.SendMessageAsync(userMessage);
                _ = Task.Run(ReceiveMessages);
            }
            catch (Exception ex)
            {
                //AppendMessage($"Bağlantı hatası: {ex.Message}");
            }
        }
        private async Task ReceiveMessages()
        {
            try
            {
                while (true)
                {
                    // WebSocket'ten gelen mesajı al
                    var messageJson = await _webSocketClient.ReceiveMessageAsync();

                    if (!string.IsNullOrWhiteSpace(messageJson))
                    {
                        try
                        {
                            // Gelen mesajı deserialize et
                            var response = JsonConvert.DeserializeObject<WebSocketResponse>(messageJson);

                            if (response != null)
                            {
                                // Mesajı UI'ye ekle
                                AppendMessage(response.User, response.Message, response.Date, false);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Mesaj işleme hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Mesaj alma hatası: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AppendMessage(Kullanici kullanici, string message, DateTime date, bool isOwnMessage)
        {
            var messageCard = new MessageCard(kullanici, message, date, isOwnMessage);

            if (flowLayoutPanel1.InvokeRequired)
            {
                flowLayoutPanel1.Invoke(new Action(() => flowLayoutPanel1.Controls.Add(messageCard)));
            }
            else
            {
                flowLayoutPanel1.Controls.Add(messageCard);
            }
        }


        private async Task SendMessage(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            try
            {
                var chatMessage = new
                {
                    type = "message",
                    data = message,
                    room = _newDersKayit != null ? _newDersKayit.Oda : _newGorusme.Oda
                };

                await _webSocketClient.SendMessageAsync(chatMessage);

                AppendMessage(GlobalData.Kullanici, message, DateTime.Now, true);
            }
            catch (Exception ex)
            {
                //AppendMessage($"Mesaj gönderme hatası: {ex.Message}");
            }
        }

        async Task PressSend()
        {
            string messageToSend = TxtMesaj.Text;
            if (string.IsNullOrWhiteSpace(messageToSend))
            {
                return;
            }
            await SendMessage(messageToSend);
            TxtMesaj.Text = "";
            TxtMesaj.Clear();
        }

        private async void bunifuButton1_Click(object sender, EventArgs e)
        {
            await PressSend();
        }

        private async void ChattingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Kullanıcıya emin olup olmadığını sormak için bir MessageBox gösteriyoruz
            DialogResult result = MessageBox.Show(
                "Görüşme sonlandırılacak, devam edilsin mi?",
                "Sistem",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            // Kullanıcı "Hayır" seçerse kapatma işlemini iptal et
            if (result == DialogResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                if (_newDersKayit != null)
                {
                    // burada açık olan ders kapatılacak
                    var aktifDers = await _dersKayitRepository.GetByIdAsync(_newDersKayit.Id);
                    aktifDers.Durum = false;
                    var x = await _dersKayitRepository.UpdateAsync(aktifDers);
                }
                else
                {
                    // burada açık olan görüşme kapatılacak
                    var aktifGorusme = await _gorusmeRepository.GetByIdAsync(_newGorusme.Id);
                    aktifGorusme.Durum = false;
                    var x = await _gorusmeRepository.UpdateAsync(aktifGorusme);
                }
            }

        }

        private async void TxtMesaj_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                await PressSend();
        }
    }
    public class WebSocketResponse
    {
        public Kullanici User { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
