using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
using dershaneOtomasyonu.Helpers;
using dershaneOtomasyonu.Repositories.TableRepositories.DersKayitRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.GorusmeRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.YoklamaRepositories;
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
        private readonly IYoklamaRepository _yoklamaRepository;
        private WebSocketClient _webSocketClient;
        private DersKayit _newDersKayit;
        private Gorusme _newGorusme;

        public static int? dersKayitId = null;
        public static int? gorusmeId = null;

        public ChattingForm(DersKayit newDersKayit,
            IDersKayitRepository dersKayitRepository,
            IYoklamaRepository yoklamaRepository)
        {
            InitializeComponent();
            _newDersKayit = newDersKayit;
            _webSocketClient = new WebSocketClient();
            _dersKayitRepository = dersKayitRepository;
            _yoklamaRepository = yoklamaRepository;
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
                    data = GlobalData.Kullanici.Adapt<KullaniciWithoutNavPropDto>(),
                    room = _newDersKayit != null ? _newDersKayit.Oda : _newGorusme.Oda
                };
                await _webSocketClient.SendMessageAsync(userMessage);
                await ReceiveMessages();
            }
            catch (Exception ex)
            {
                //AppendMessage($"Bağlantı hatası: {ex.Message}");
            }
        }
        private async Task ReceiveMessages()
        {
            while (true)
            {
                try
                {
                    var messageJson = await _webSocketClient.ReceiveMessageAsync();

                    if (!string.IsNullOrWhiteSpace(messageJson))
                    {
                        var response = JsonConvert.DeserializeObject<WebSocketResponse>(messageJson);
                        if (response != null)
                        {
                            AppendMessage(response.User, response.Message, response.Date);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ReceiveMessages Hata: {ex.Message}");
                    await Task.Delay(1000); // Bekleme ekleyerek döngüyü devam ettirin
                }
            }
        }

        private void AppendMessage(Kullanici kullanici, string message, DateTime date)
        {
            bool isOwnMessage = kullanici.Id == GlobalData.Kullanici!.Id ? true : false;
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

                //AppendMessage(GlobalData.Kullanici, message, DateTime.Now, true);
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
                if (GlobalData.Kullanici!.RoleId == 2)// Öğretmen çıkışı
                {
                    if (_newDersKayit != null)
                    {
                        // burada açık olan ders kapatılacak
                        var aktifDers = await _dersKayitRepository.GetByIdAsync(_newDersKayit.Id);
                        aktifDers.Durum = false;
                        var x = await _dersKayitRepository.UpdateAsync(aktifDers);
                        dersKayitId = aktifDers.Id;
                    }
                    else
                    {
                        // burada açık olan görüşme kapatılacak
                        var aktifGorusme = await _gorusmeRepository.GetByIdAsync(_newGorusme.Id);
                        aktifGorusme.Durum = false;
                        var x = await _gorusmeRepository.UpdateAsync(aktifGorusme);
                        gorusmeId = aktifGorusme.Id;
                    }
                }
                else if (GlobalData.Kullanici!.RoleId == 3)// Öğrenci çıkışı
                {
                    if (_newDersKayit != null)
                    {
                        // burada öğrencinin yoklama kaydına çıkış tarihi atılacak
                        var yoklama = await _yoklamaRepository.GetByKullaniciIdAndDersKayitIdAsync(_newDersKayit.Id, GlobalData.Kullanici!.Id);
                        if (yoklama != null)
                        {
                            yoklama.AyrilmaTarihi = DateTime.Now;
                            var x = await _yoklamaRepository.UpdateAsync(yoklama);
                        }
                    }
                    else if (_gorusmeRepository != null)
                    {
                        var gorusme = await _gorusmeRepository.GetByIdAsync(_newGorusme.Id);
                        if (gorusme != null)
                        {
                            gorusme.Durum = false;
                            var x = await _gorusmeRepository.UpdateAsync(gorusme);
                        }
                    }
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
