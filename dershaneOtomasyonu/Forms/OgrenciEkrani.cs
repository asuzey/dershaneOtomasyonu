﻿using Bunifu.UI.WinForms;
using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.Helpers;
using dershaneOtomasyonu.Repositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DegerlendirmeRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DersKayitRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DerslerRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.GorusmeRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDersRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDosyaRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.LogRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.SinifRepositories;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciNotRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.NotRepositories;
using dershaneOtomasyonu.Forms;
using dershaneOtomasyonu.Repositories.TableRepositories.YoklamaRepositories;
using System.Windows.Forms.DataVisualization.Charting;

namespace dershaneOtomasyonu
{
    public partial class OgrenciEkrani : Form
    {
        private readonly IKullaniciRepository _kullaniciRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly ILogger _logger;
        private readonly IBaseRepository<LogEntry> _baseLogRepository;
        private readonly IBaseRepository<Dosya> _baseDosyaRepository;
        private readonly IKullaniciDosyaRepository _kullaniciDosyaRepository;
        private readonly ILogRepository _logRepository;
        private readonly ISinifRepository _sinifRepository;
        private readonly IDerslerRepository _derslerRepository;
        private readonly IKullaniciDersRepository _kullaniciDersRepository;
        private readonly IDersKayitRepository _dersKayitRepository;
        private readonly IDegerlendirmeRepository _degerlendirmeRepository;
        private readonly IGorusmeRepository _gorusmeRepository;
        private readonly FileService _fileService;
        private readonly IKullaniciNotRepository _kullaniciNotRepository;
        private readonly INotRepository _notRepository;
        private readonly IYoklamaRepository _yoklamaRepository;
        private ChattingForm _chattingForm;



        public OgrenciEkrani(ILogger logger,
            IKullaniciRepository kullaniciRepository,
            IBaseRepository<Role> roleRepository,
            IBaseRepository<LogEntry> baseLogRepository,
            ILogRepository logRepository,
            ISinifRepository sinifRepository,
            IDerslerRepository derslerRepository,
            IKullaniciDersRepository kullaniciDersRepository,
            IBaseRepository<Dosya> baseDosyaRepository,
            IKullaniciDosyaRepository kullaniciDosyaRepository,
            IDersKayitRepository dersKayitRepository,
            IDegerlendirmeRepository degerlendirmeRepository,
            IGorusmeRepository gorusmeRepository,
            IKullaniciNotRepository kullaniciNotRepository,
            INotRepository notRepository,
            IYoklamaRepository yoklamaRepository)

        {
            InitializeComponent();
            _kullaniciRepository = kullaniciRepository;
            _roleRepository = roleRepository;
            _logger = logger;
            _baseLogRepository = baseLogRepository;
            _logRepository = logRepository;
            _sinifRepository = sinifRepository;
            _derslerRepository = derslerRepository;
            _kullaniciDersRepository = kullaniciDersRepository;
            _baseDosyaRepository = baseDosyaRepository;
            _kullaniciDosyaRepository = kullaniciDosyaRepository;
            _dersKayitRepository = dersKayitRepository;
            _degerlendirmeRepository = degerlendirmeRepository;
            _gorusmeRepository = gorusmeRepository;
            _kullaniciNotRepository = kullaniciNotRepository;
            _notRepository = notRepository;
            _yoklamaRepository = yoklamaRepository;
            _fileService = new FileService();
            panels = [panelDersNotlari, panelNotlarim, panelSinifGrubu, panelGorusme, panelBilgilerim];

        }

        public static BunifuPanel[] panels;

        private async void CikisYap_Click(object sender, EventArgs e)
        {
            GirisEkrani OgrenciEkrani = new GirisEkrani(_logger,
                _kullaniciRepository,
                _roleRepository,
                _baseLogRepository,
                _logRepository,
                _sinifRepository,
                _derslerRepository,
                _kullaniciDersRepository,
                _baseDosyaRepository,
                _kullaniciDosyaRepository,
                _dersKayitRepository,
                _degerlendirmeRepository,
                _gorusmeRepository,
                _kullaniciNotRepository,
                _notRepository,
                _yoklamaRepository); // form3e geçiş
            OgrenciEkrani.Show(); // form3ü açıyor
            this.Hide(); // form1i gizleyecek
            OgrenciEkrani.FormClosed += (s, args) => this.Close();
            await _logger.Info($"Çıkış yapıldı. {GlobalData.Kullanici?.Adi}");
        }

        private void OgrenciEkrani_Load(object sender, EventArgs e)
        {
            kullaniciadogr.Text = $"{GlobalData.Kullanici?.Adi} {GlobalData.Kullanici?.Soyadi}";
            txtKarsilama.Text = $"Hoşgeldin {GlobalData.Kullanici?.Adi} !";
        }

        private void TogglePanel(BunifuPanel panelToToggle)
        {
            foreach (var panel in panels)
            {
                if (panel == panelToToggle)
                {
                    // Seçili panelin görünürlük durumunu tersine çevirir
                    panel.Visible = !panel.Visible;
                }
                else
                {
                    // Diğer panelleri gizler
                    panel.Visible = false;
                }
            }
        }

        private void BtnDersNotlari_Click(object sender, EventArgs e)
        {
            TogglePanel(panelDersNotlari);
            InitializeDataGridView();
            RefreshFileList();
        }

        private void InitializeDataGridView()
        {
            // Dosya Adı sütunu
            DosyaDataGridView.ColumnCount = 2;
            DosyaDataGridView.Columns[0].Name = "Dosya Adı";
            DosyaDataGridView.Columns[1].Name = "Yükleyen Öğretmen";

            // İndirme butonu
            DataGridViewButtonColumn downloadButtonColumn = new DataGridViewButtonColumn
            {
                Name = "İndir",
                Text = "⬇️",
                UseColumnTextForButtonValue = true,
                Width = 50
            };
            DosyaDataGridView.Columns.Add(downloadButtonColumn);

            DosyaDataGridView.CellClick -= DosyaDataGridView_CellClick; // Önce eski bağlantıyı kaldır
            DosyaDataGridView.CellClick += DosyaDataGridView_CellClick;
        }

        private async void RefreshFileList()
        {
            try
            {
                DosyaDataGridView.Rows.Clear();
                var files = _fileService.ListFiles();

                var assignedFiles = await _kullaniciDosyaRepository.GetAllByKullaniciIdAsync(GlobalData.Kullanici!.Id);
                foreach (var file in files)
                {
                    foreach (var assignedFile in assignedFiles)
                    {
                        if (file == assignedFile.Dosya.FileName)
                        {
                            DosyaDataGridView.Rows.Add(file, assignedFile.Dosya.Olusturucu.Adi + " " + assignedFile.Dosya.Olusturucu.Soyadi);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya listesi alınırken hata oluştu: {ex.Message}");
            }
        }

        private void logo_Click(object sender, EventArgs e)
        {
            foreach (var panel in panels)
            {
                panel.Visible = false;
            }
        }

        private void DosyaDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string fileName = DosyaDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();

                if (e.ColumnIndex == DosyaDataGridView.Columns["İndir"].Index)
                {
                    // İndirme işlemi
                    using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                    {
                        saveFileDialog.FileName = fileName;

                        if (saveFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            string savePath = saveFileDialog.FileName; // Seçilen dosya yolu
                            bool success = _fileService.DownloadFile(fileName, savePath);
                            if (success)
                            {
                                MessageBox.Show($"Dosya başarıyla indirildi.\nKaydedilen yer: {savePath}", "Sistem Mesajı");
                            }
                            else
                            {
                                MessageBox.Show("Dosya indirme başarısız.");
                            }
                        }
                        else
                        {
                            return;
                        }
                    }

                }
            }
        }

        private async void BtnNotTutmaPanel_Click(object sender, EventArgs e)
        {
            TogglePanel(panelNotlarim);
        }

        private async void BtnKaydet_Click(object sender, EventArgs e)
        {
            string dosyaAdi = txtDosyaAdi.Text;
            if (string.IsNullOrWhiteSpace(dosyaAdi))
            {
                MessageBox.Show("Lütfen bir dosya adı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!dosyaAdi.EndsWith(".txt"))
            {
                dosyaAdi += ".txt";
            }
            string kaydetmeDizini = @"C:\Users\Public\Dosyalar\";
            if (!Directory.Exists(kaydetmeDizini))
            {
                Directory.CreateDirectory(kaydetmeDizini);
            }
            string tamDosyaYolu = Path.Combine(kaydetmeDizini, dosyaAdi);
            string metin = OgrenciNotRichTextBox.Text;
            try
            {
                File.WriteAllText(tamDosyaYolu, metin);
                MessageBox.Show($"Dosya başarıyla kaydedildi:\n{tamDosyaYolu}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await _logger.Info($"Dosya kaydedildi: {tamDosyaYolu}");
                var newnot = new Not();
                newnot.Baslik = txtDosyaAdi.Text;
                newnot.Icerik = OgrenciNotRichTextBox.Text;
                newnot.OlusturmaTarihi = DateTime.Now;
                await _notRepository.AddAsync(newnot);

                var kullaniciNot = new KullaniciNot();
                kullaniciNot.KullaniciId = GlobalData.Kullanici!.Id;
                kullaniciNot.NotId = newnot.Id;
                await _kullaniciNotRepository.AddAsync(kullaniciNot);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya kaydedilirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private async void BtnSınıfChat_Click(object sender, EventArgs e)
        {
            TogglePanel(panelSinifGrubu);
            // chat ekranı yönlendirmesi yapılacak ve datagride dersler doldurulacak aynı zamanda açan öğretmen de bu listede gözükecek.
            await LoadAktifDersler();

        }

        private async Task LoadAktifDersler()
        {
            if (GlobalData.Kullanici!.SinifId != null)
            {
                var aktifDersler = await _dersKayitRepository.GetActiveDerslerBySinifIdAsync((int)GlobalData.Kullanici!.SinifId);
                var aktifDerslerList = aktifDersler
                    .Select(x => new
                    {
                        Id = x.Id,
                        SinifKodu = x.Sinif.Kodu,
                        OgretmenAdi = $"{x.Kullanici.Adi} {x.Kullanici.Soyadi}"
                    })
                    .ToList();
                activeDerslerDataGrid.DataSource = aktifDerslerList;
                activeDerslerDataGrid.Columns[0].Visible = false;
            }
        }

        private async void btnDersBaslat_Click(object sender, EventArgs e)
        {
            var dersKayitId = 0;
            if (activeDerslerDataGrid.CurrentRow != null) dersKayitId = Convert.ToInt32(activeDerslerDataGrid.CurrentRow.Cells[0].Value);
            if (dersKayitId != 0)
            {
                var dersKayit = await _dersKayitRepository.GetByIdAsync(dersKayitId);
                DialogResult userChoice = MessageBox.Show(
                    "Ders'e katılınacaktır, devam edilsin mi?",
                    "Ders Başlatma İşlemi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (dersKayit != null && userChoice == DialogResult.Yes)
                {
                    _chattingForm = new ChattingForm(dersKayit, _dersKayitRepository, _yoklamaRepository);
                    _chattingForm.Show();
                    var ders = await _derslerRepository.GetByIdAsync(dersKayit.DersId);
                    await _logger.Info($"Ders başlatıldı: {dersKayit.Ders.Adi}");

                    var yoklama = await _yoklamaRepository.GetByKullaniciIdAndDersKayitIdAsync(dersKayitId, GlobalData.Kullanici!.Id);
                    if (yoklama == null)
                    {
                        yoklama = new Yoklama();
                        yoklama.DersKayitId = dersKayitId;
                        yoklama.KullaniciId = GlobalData.Kullanici!.Id;
                        yoklama.KatilmaTarihi = DateTime.Now;
                        await _yoklamaRepository.AddAsync(yoklama);
                    }
                    else
                    {
                        yoklama.AyrilmaTarihi = null;
                        await _yoklamaRepository.UpdateAsync(yoklama);
                    }


                }
            }
            else
            {
                MessageBox.Show("Lütfen aktif dersler tablosundan bir kayıt seçiniz.", "Bilgi");
            }
        }

        private async void BtnGorusmePanel_Click(object sender, EventArgs e)
        {
            await LoadAktifGorusmeler();
            TogglePanel(panelGorusme);
        }

        private async Task LoadAktifGorusmeler()
        {
            var aktifgorusmeler = await _gorusmeRepository.GetAktifGorusmelerByKatilimciIdAsync(GlobalData.Kullanici!.Id);
            var aktifGorusmelerDto = aktifgorusmeler.Select(x => new
            {
                x.Id,
                Olusturucu = x.Olusturucu.Adi + " " + x.Olusturucu.Soyadi
            }).ToList();
            AktifGorusmelerGridView.DataSource = aktifGorusmelerDto;
            AktifGorusmelerGridView.Columns[0].Visible = false;
        }

        private async void BtnBilgilerimpanel_Click(object sender, EventArgs e)
        {
            await InitializeGraph();
            TogglePanel(panelBilgilerim);
        }

        private async Task InitializeGraph()
        {
            // Verileri alın
            var degerlendirmeler = await _degerlendirmeRepository.GetDegerlendirmelerByKullaniciIdAsync(GlobalData.Kullanici!.Id);
            if (degerlendirmeler.Count == 0)
            {
                MessageBox.Show("Belirtilen kullanıcı için değerlendirme bulunamadı.");
                return;
            }

            // Verileri ders ID’lerine göre gruplandırın
            var kullaniciDegerlendirmeleri = degerlendirmeler
                .GroupBy(d => new { d.Ders.Id, d.Ders.Adi }) // Hem Ders ID hem de Ders Adı
                .OrderBy(g => g.Key.Id) // ID’ye göre sıralayın
                .ToList();

            // Chart kontrolünü temizle ve ayarla
            chart1.Series.Clear();
            chart1.Titles.Clear();
            chart1.ChartAreas.Clear();
            chart1.Legends.Clear();

            // ChartArea ekle
            var chartArea = new ChartArea("ChartArea");
            chart1.ChartAreas.Add(chartArea);

            // Ortalama ve son puanlar için iki seri oluştur
            var ortalamaSerisi = new Series("Ortalama Puan")
            {
                ChartType = SeriesChartType.Column, // Kolon grafiği
                XValueType = ChartValueType.Int32   // X ekseni ID için integer
            };
            var sonPuanSerisi = new Series("Son Puan")
            {
                ChartType = SeriesChartType.Column, // Kolon grafiği
                XValueType = ChartValueType.Int32   // X ekseni ID için integer
            };

            // Verileri serilere ekle
            foreach (var dersGrubu in kullaniciDegerlendirmeleri)
            {
                int dersId = dersGrubu.Key.Id; // Ders ID
                string dersAdi = dersGrubu.Key.Adi; // Ders Adı
                var puanlar = dersGrubu.Select(d => d.Puan).ToList();

                double ortalamaPuan = puanlar.Average(); // Ortalama puan
                int sonPuan = puanlar.Last(); // Son puan

                // Ortalama ve son puanı serilere ekle
                var ortalamaPoint = ortalamaSerisi.Points.AddXY(dersId, ortalamaPuan);
                ortalamaSerisi.Points[ortalamaPoint].Label = dersAdi; // Ders adı kolonun üstüne

                var sonPuanPoint = sonPuanSerisi.Points.AddXY(dersId, sonPuan);
                sonPuanSerisi.Points[sonPuanPoint].Label = dersAdi; // Ders adı kolonun üstüne
            }

            // Serileri grafiğe ekle
            chart1.Series.Add(ortalamaSerisi);
            chart1.Series.Add(sonPuanSerisi);

            // X ve Y eksenlerini ayarla
            chartArea.AxisX.Title = "Ders ID";
            chartArea.AxisY.Title = "Puan";
            chartArea.AxisX.Interval = 1; // Her bir X ekseni elemanını göstermek için
            chartArea.AxisX.IsLabelAutoFit = true;
            chartArea.AxisY.Minimum = 0; // Y ekseni sıfırdan başlasın
            chartArea.RecalculateAxesScale(); // Ekseni yeniden hesapla

            // X ekseni yazı tiplerini ayarla
            chartArea.AxisX.LabelStyle.Font = new System.Drawing.Font("Arial", 10);
            chartArea.AxisY.LabelStyle.Font = new System.Drawing.Font("Arial", 10);

            // Çubukların görünümünü özelleştirme
            ortalamaSerisi.Color = Color.FromArgb(30, 24, 97);
            sonPuanSerisi.Color = Color.FromArgb(183, 183, 229);
            ortalamaSerisi["PointWidth"] = "0.4";
            sonPuanSerisi["PointWidth"] = "0.4";

            // Legend (açıklama) ekle
            chart1.Legends.Add(new Legend("Legend")
            {
                Docking = Docking.Top,
                Alignment = System.Drawing.StringAlignment.Center
            });

            // Grafik başlığı ekle
            chart1.Titles.Add("Derslere Göre Ortalama ve Son Puan");

        }

        private async void BtnGorusmeKatil_Click(object sender, EventArgs e)
        {
            var gorusmeId = 0;
            if (AktifGorusmelerGridView.CurrentRow != null) gorusmeId = Convert.ToInt32(AktifGorusmelerGridView.CurrentRow.Cells[0].Value);
            if (gorusmeId == 0)
            {
                MessageBox.Show("Lütfen görüşme kaydı seçiniz.", "Görüşme Başlatma İşlemi");
                return;
            }
            DialogResult userChoice = MessageBox.Show(
                    "Görüşmeye katılınacaktır, devam edilsin mi?",
                    "Görüşme Başlatma İşlemi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
            // global data'daki kullanıcı id'yi sorgulayarak aktif olan görüşmeleri gride yazacağız
            // eğer bi öğrenci ile görüşme başlamışsa, o görüşme aktifken yeni bir görüşme kaydı atılmayacak, o kayıt devam edecek
            if (userChoice == DialogResult.Yes)
            {
                // burada görüşme başlatırken sınıf id üzerinden arama yapmak mantıklı değil, çünkü oluşturucu ve kullanıcı idlerine göre aktif görüşmeleri
                // çekeceğiz, buna göre senin global data'daki kullanıcı ile tabloda seçili öğrencinin id 'sini alarak
                await _logger.Info($"Görüşmeye katılınıyor... {GlobalData.Kullanici?.Adi}");
                var aktifGorusme = await _gorusmeRepository.GetByIdAsync(gorusmeId);
                if (aktifGorusme != null)
                {
                    _chattingForm = new ChattingForm(aktifGorusme, _gorusmeRepository);
                    _chattingForm.Show();
                }

            }
        }
    }
}
