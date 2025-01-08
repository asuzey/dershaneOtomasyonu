using Bunifu.UI.WinForms;
using dershaneOtomasyonu.Database.Tables;
using dershaneOtomasyonu.DTO;
using dershaneOtomasyonu.Forms;
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
using Mapster;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.Adapters;

namespace dershaneOtomasyonu
{
    public partial class PersonelEkrani : Form
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
        private ChattingForm _chattingForm;

        public PersonelEkrani(ILogger logger,
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
            IGorusmeRepository gorusmeRepository)

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
            panels = [Panel_SiniflarVeOgrenciler, panelDosyaGonderme, panelDersBaslat, PanelGorusme];
            _fileService = new FileService();

        }


        public static BunifuPanel[] panels;

        private void InitializeDataGridView()
        {
            // Dosya Adı sütunu
            DosyaDataGridView.ColumnCount = 1;
            DosyaDataGridView.Columns[0].Name = "Dosya Adı";
            DosyaDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // Silme butonu
            DataGridViewButtonColumn deleteButtonColumn = new DataGridViewButtonColumn
            {
                Name = "Sil",
                Text = "❌",
                UseColumnTextForButtonValue = true,
                Width = 50
            };
            DosyaDataGridView.Columns.Add(deleteButtonColumn);

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
        } // dosya işlemleri


        private void CikisYap_Click(object sender, EventArgs e)
        {
            GirisEkrani form1 = new GirisEkrani(_logger, _kullaniciRepository, _roleRepository, _baseLogRepository, _logRepository, _sinifRepository, _derslerRepository, _kullaniciDersRepository, _baseDosyaRepository, _kullaniciDosyaRepository, _dersKayitRepository, _degerlendirmeRepository, _gorusmeRepository); // form4e geçiş
            form1.Show(); // form4ü açıyor
            this.Hide(); // form1i gizleyecek
            form1.FormClosed += (s, args) => this.Close();
        }


        private async void Btn_SiniflarVeOgrenciler_Click(object sender, EventArgs e)
        {
            var siniflar = await _sinifRepository.GetAllAsync();
            siniflarDataGridView.DataSource = siniflar;
            siniflarDataGridView.Columns[0].Visible = false;
            TogglePanel(Panel_SiniflarVeOgrenciler);
        }

        private async Task LoadAtanmisOgrenciler(int sinifId)
        {
            var ogrenciler = await _kullaniciRepository.GetAllStudentsAsync();
            var sinifinOgrencileri = ogrenciler.Where(o => o.SinifId == sinifId).ToList();
            sinifinOgrencileriDataGridView.DataSource = sinifinOgrencileri;
            foreach (DataGridViewColumn column in sinifinOgrencileriDataGridView.Columns)
            {
                column.Visible = false;
            }
            sinifinOgrencileriDataGridView.Columns[5].Visible = true;
            sinifinOgrencileriDataGridView.Columns[6].Visible = true;
        }

        private async Task LoadSinifinOgrencileriToDersBaslat(int sinifId) // ders başlatma panelindeki öğrenci listesini dolduruyoruz
        {
            var ogrenciler = await _kullaniciRepository.GetAllStudentsAsync();
            var sinifinOgrencileri = ogrenciler.Where(o => o.SinifId == sinifId).ToList();
            chattingOgrDataGridView.DataSource = sinifinOgrencileri;
            foreach (DataGridViewColumn column in chattingOgrDataGridView.Columns)
            {
                column.Visible = false;
            }
            chattingOgrDataGridView.Columns[5].Visible = true;
            chattingOgrDataGridView.Columns[6].Visible = true;
        }

        private async Task LoadSinifinOgrencileriToDersBaslatOnPanelGorusme(int sinifId) // gorusme panelindeki ogrenci listesini dolduruyor
        {
            var ogrenciler = await _kullaniciRepository.GetAllStudentsAsync();
            var sinifinOgrencileri = ogrenciler.Where(o => o.SinifId == sinifId).ToList();
            GorusmeOgrencilerDataGridView.DataSource = sinifinOgrencileri;
            foreach (DataGridViewColumn column in GorusmeOgrencilerDataGridView.Columns)
            {
                column.Visible = false;
            }
            GorusmeOgrencilerDataGridView.Columns[5].Visible = true;
            GorusmeOgrencilerDataGridView.Columns[6].Visible = true;
        }

        private void TogglePanel(BunifuPanel panelToToggle) // panel geçişleri için
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

        private async void siniflarDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            var sinifId = 0;
            if (siniflarDataGridView.CurrentRow != null) sinifId = Convert.ToInt32(siniflarDataGridView.CurrentRow.Cells[0].Value);
            if (sinifId != 0) await LoadAtanmisOgrenciler(sinifId);
        }

        private async void Btn_YeniDosyaYukle_Click(object sender, EventArgs e)
        {
            bool isClassSelected;
            if (SiniflarDosyaDataGridView1.CurrentRow == null)
                isClassSelected = false;
            else
                isClassSelected = true;

            if (!isClassSelected)
            {
                DialogResult userChoice = MessageBox.Show(
                    "Seçili dosya tüm sınıflarla paylaşılacaktır. Devam edilsin mi?",
                    "Dosya Seçimi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (userChoice == DialogResult.Yes)
                {
                    await OpenFileSelection();
                }
            }
            else
            {
                var selectedClass = SiniflarDosyaDataGridView1.CurrentRow.Cells[1].Value.ToString();
                DialogResult userChoice = MessageBox.Show(
                    $"Seçili dosya {selectedClass} sınıfı ile paylaşılacaktır. Devam edilsin mi?",
                    "Dosya Seçimi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (userChoice == DialogResult.Yes)
                {
                    await OpenFileSelection();
                }
            }

        }

        async Task OpenFileSelection()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string timestamp = DateTime.Now.ToString("ddMMyyyyHHmmssfff");
                    bool success = _fileService.UploadFile(openFileDialog.FileName, timestamp);
                    if (success)
                    {
                        string originalFileName = openFileDialog.FileName;
                        string timestampedFileName = $"{Path.GetFileNameWithoutExtension(originalFileName)}_{timestamp}{Path.GetExtension(originalFileName)}";
                        var newFile = new Dosya
                        {
                            FileName = timestampedFileName,
                            FilePath = @"C:\Users\Public\FileServer"
                        };
                        await _baseDosyaRepository.AddAsync(newFile);
                        var assignment = new KullaniciDosya();
                        assignment.KullaniciId = GlobalData.Kullanici!.Id;
                        assignment.DosyaId = newFile.Id;
                        await _kullaniciDosyaRepository.AddAsync(assignment);
                        var ogrenciler = await _kullaniciRepository.GetAllStudentsAsync();
                        var selectedClass = SiniflarDosyaDataGridView1.CurrentRow != null ? SiniflarDosyaDataGridView1.CurrentRow.Cells[0] : null;

                        if (selectedClass == null)
                        {
                            foreach (var ogrenci in ogrenciler)
                            {
                                var newAssignment = new KullaniciDosya();
                                newAssignment.KullaniciId = ogrenci.Id;
                                newAssignment.DosyaId = newFile.Id;
                                await _kullaniciDosyaRepository.AddAsync(newAssignment);
                            }
                        }
                        else
                        {
                            var selectedOgrenciler = ogrenciler.Where(o => o.SinifId == (int)selectedClass.Value).ToList();
                            foreach (var ogrenci in selectedOgrenciler)
                            {
                                var newAssignment = new KullaniciDosya();
                                newAssignment.KullaniciId = ogrenci.Id;
                                newAssignment.DosyaId = newFile.Id;
                                await _kullaniciDosyaRepository.AddAsync(newAssignment);
                            }
                        }
                        MessageBox.Show("Dosya başarıyla yüklendi.");
                        RefreshFileList();
                    }
                    else
                    {
                        MessageBox.Show("Dosya yükleme başarısız.");
                    }
                }
            }
        }

        private void DosyaDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string fileName = DosyaDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString();

                if (e.ColumnIndex == DosyaDataGridView.Columns["Sil"].Index)
                {
                    // Silme işlemi
                    var result = MessageBox.Show($"{fileName} dosyasını silmek istediğinizden emin misiniz?",
                                                 "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        bool success = _fileService.DeleteFile(fileName);
                        if (success)
                        {
                            MessageBox.Show("Dosya başarıyla silindi.");
                            RefreshFileList();
                        }
                        else
                        {
                            MessageBox.Show("Dosya silme başarısız.");
                        }
                    }
                }
                else if (e.ColumnIndex == DosyaDataGridView.Columns["İndir"].Index)
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
                    }
                }
            }
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
                            DosyaDataGridView.Rows.Add(file);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya listesi alınırken hata oluştu: {ex.Message}");
            }
        }

        private async void BtnNotGondermePanel_Click(object sender, EventArgs e)
        {
            await LoadClassrooms();
            InitializeDataGridView();
            RefreshFileList();
            TogglePanel(panelDosyaGonderme);
        }

        private async Task LoadClassrooms()
        {
            var siniflar = await _sinifRepository.GetAllAsync();
            SiniflarDosyaDataGridView1.DataSource = siniflar;
            SiniflarDosyaDataGridView1.Columns[0].Visible = false;
        }

        private async Task LoadClassroomsOnPanelGorusme()
        {
            var siniflar = await _sinifRepository.GetAllAsync();
            gorusmeSiniflarDataGridView.DataSource = siniflar;
            gorusmeSiniflarDataGridView.Columns[0].Visible = false;
        }

        private void BtnSecimiTemizle_Click_Click(object sender, EventArgs e)
        {
            SiniflarDosyaDataGridView1.ClearSelection(); // Seçimi temizle
            SiniflarDosyaDataGridView1.CurrentCell = null; // CurrentRow'u etkisiz yapar
        }

        private void logo_Click(object sender, EventArgs e)
        {
            foreach (var panel in panels)
            {
                panel.Visible = false;
            }
        }

        private async void btnDersBaslatmaPanel_Click(object sender, EventArgs e)
        {
            var siniflar = await _sinifRepository.GetAllAsync();
            chattingSinifDataGridView.DataSource = siniflar;
            chattingSinifDataGridView.Columns[0].Visible = false;
            LoadActiveDerslerData();
            TogglePanel(panelDersBaslat);
        }

        private async void chattingSinifDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            var sinifId = 0;
            if (chattingSinifDataGridView.CurrentRow != null) sinifId = Convert.ToInt32(chattingSinifDataGridView.CurrentRow.Cells[0].Value);
            if (sinifId != 0) await LoadSinifinOgrencileriToDersBaslat(sinifId);
        }

        private async void btnDersBaslat_Click(object sender, EventArgs e)
        {
            if (chattingOgrDataGridView.Rows.Count == 0)
            {
                MessageBox.Show("Lütfen öğrencilerin bulunduğu bir sınıf seçiniz.", "Ders Başlatma İşlemi");
                return;
            }
            DialogResult userChoice = MessageBox.Show(
                    "Ders başlatılacaktır, devam edilsin mi?",
                    "Ders Başlatma İşlemi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

            if (userChoice == DialogResult.Yes)
            {

                var sinifId = 0;
                if (chattingSinifDataGridView.CurrentRow != null) { sinifId = Convert.ToInt32(chattingSinifDataGridView.CurrentRow.Cells[0].Value); } else if (sinifId == 0) { return; } else { return; }

                var sinifAdi = chattingSinifDataGridView.CurrentRow.Cells[1].Value.ToString();
                var dersKayit = await _dersKayitRepository.GetActiveDersBySinifAndOgretmenIdAsync(sinifId, GlobalData.Kullanici.Id);
                if (dersKayit == null)
                {
                    dersKayit = new DersKayit();
                    dersKayit.SinifId = sinifId;
                    dersKayit.KullaniciId = GlobalData.Kullanici!.Id;
                    dersKayit.Oda = $"{sinifAdi}-{GlobalData.Kullanici.Id}-{DateTime.Now}";
                    dersKayit.Durum = true;
                    await _dersKayitRepository.AddAsync(dersKayit);
                }


                _chattingForm = new ChattingForm(dersKayit, _dersKayitRepository);
                _chattingForm.FormClosed += _chattingForm_FormClosed;
                _chattingForm.Show();

                await LoadActiveDerslerData();

            }
        }

        private async void _chattingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Thread.Sleep(2000); // 2000 milisaniye (2 saniye)
            await LoadActiveDerslerData();
        }
        private async void _chattingFormGorusme_FormClosed(object sender, FormClosedEventArgs e)
        {
            Thread.Sleep(2000); // 2000 milisaniye (2 saniye)
            await LoadActiveGorusmelerData();
        }

        private async Task LoadActiveDerslerData()// aktif dersleri çekiyorduk
        {
            var devamEdenDersler = await _dersKayitRepository.GetActiveDerslerByOgretmenIdAsync(GlobalData.Kullanici.Id);
            var siniflar = new List<Sinif>();
            foreach (var ders in devamEdenDersler)
            {
                siniflar.Add(await _sinifRepository.GetByIdAsync(ders.SinifId));
            }
            activeDerslerDataGrid.DataSource = siniflar;
            activeDerslerDataGrid.Columns[0].Visible = false;
        }
        private async Task LoadActiveGorusmelerData()// aktif görüşmeleri çekeceğiz
        {
            var devamEdenGorusmeler = await _dersKayitRepository.GetActiveDerslerByOgretmenIdAsync(GlobalData.Kullanici.Id);
            var siniflar = new List<Sinif>();
            foreach (var ders in devamEdenGorusmeler)
            {
                siniflar.Add(await _sinifRepository.GetByIdAsync(ders.SinifId));
            }
            activeDerslerDataGrid.DataSource = siniflar;
            activeDerslerDataGrid.Columns[0].Visible = false;
        }

        public static Tuple<int, string> ShowInputDialog()
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 200,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Değer Girişi",
                StartPosition = FormStartPosition.CenterScreen
            };

            Label rateLabel = new Label() { Left = 20, Top = 20, Text = "Puan (0-100):", AutoSize = true };
            TextBox rateInput = new TextBox() { Left = 150, Top = 20, Width = 200 };

            Label descLabel = new Label() { Left = 20, Top = 60, Text = "Açıklama (isteğe bağlı):", AutoSize = true };
            TextBox descInput = new TextBox() { Left = 150, Top = 60, Width = 200 };

            Button confirmation = new Button() { Text = "Tamam", Left = 150, Width = 90, Top = 120 };
            Button cancel = new Button() { Text = "İptal", Left = 260, Width = 90, Top = 120, DialogResult = DialogResult.Cancel };

            confirmation.Click += (sender, e) =>
            {
                // Zorunlu rate kontrolü
                if (!int.TryParse(rateInput.Text, out int rate) || rate < 0 || rate > 100)
                {
                    MessageBox.Show("Lütfen 0 ile 100 arasında geçerli bir sayısal puan değeri girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Form kapanmaz
                }

                // Validasyon başarılı, form kapatılır
                prompt.DialogResult = DialogResult.OK;
                prompt.Close();
            };

            prompt.Controls.Add(rateLabel);
            prompt.Controls.Add(rateInput);
            prompt.Controls.Add(descLabel);
            prompt.Controls.Add(descInput);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);

            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                int rate = int.Parse(rateInput.Text); // Rate doğrulandığından kesin parse edilir
                string description = descInput.Text; // Description opsiyonel
                return Tuple.Create(rate, description);
            }

            return null; // Kullanıcı iptal ettiyse
        }


        private async void BtnDegerlendirmeYap_Click(object sender, EventArgs e)
        {
            if (chattingOgrDataGridView.CurrentRow == null)
            {
                MessageBox.Show("Lütfen öğrenci tablosundan bir kayıt seçiniz.", "Bilgi");
                return;
            }

            var result = ShowInputDialog();
            if (result != null)
            {
                var degerlendirme = new Degerlendirme();
                degerlendirme.Puan = result.Item1;
                degerlendirme.Aciklama = result.Item2;
                degerlendirme.KullaniciId = Convert.ToInt32(chattingOgrDataGridView.CurrentRow.Cells[0].Value);
                degerlendirme.CreatorId = GlobalData.Kullanici!.Id;
                await _degerlendirmeRepository.AddAsync(degerlendirme);
                MessageBox.Show($"Değerlendirme kaydedildi.", "Bilgi");
            }
            else ShowInputDialog(); // eğer hatalı veri girişi yapılırsa diyalog box tekrar açılıyor.
        }

        private async void BtnGorusmePanel_Click(object sender, EventArgs e)
        {
            await LoadClassroomsOnPanelGorusme();
            TogglePanel(PanelGorusme);
        }

        private async void gorusmeSiniflarDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            var sinifId = 0;
            if (gorusmeSiniflarDataGridView.CurrentRow != null) sinifId = Convert.ToInt32(gorusmeSiniflarDataGridView.CurrentRow.Cells[0].Value);
            if (sinifId != 0) await LoadSinifinOgrencileriToDersBaslatOnPanelGorusme(sinifId);
        }

        private async void BtnGorusmeBaslat_Click(object sender, EventArgs e)
        {
            // görüşme başlat kodu yazacağım
            if (gorusmeSiniflarDataGridView.CurrentRow == null || GorusmeOgrencilerDataGridView.CurrentRow == null)
            {
                MessageBox.Show("Lütfen bir sınıf ve bir öğrenci kaydı seçiniz.", "Görüşme Başlatma İşlemi");
                return;
            }
            DialogResult userChoice = MessageBox.Show(
                    "Görüşme başlatılacaktır, devam edilsin mi?",
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

                var ogrenciId = Convert.ToInt32(GorusmeOgrencilerDataGridView.CurrentRow.Cells[0].Value);
                var aktifGorusme = await _gorusmeRepository.GetActiveGorusmeByOlusturucuIdAndKullaniciIdAsync(GlobalData.Kullanici!.Id, ogrenciId);
                if (aktifGorusme == null)
                {
                    // eğer aktif görüşme yoksa yeni bir görüşme oluşturuyoruz
                    aktifGorusme = new Gorusme();
                    aktifGorusme.OlusturucuId = GlobalData.Kullanici!.Id;
                    aktifGorusme.KatilimciId = ogrenciId;
                    aktifGorusme.Oda = $"{GlobalData.Kullanici.Id}-{ogrenciId}-{DateTime.Now}";
                    aktifGorusme.Durum = true;
                    await _gorusmeRepository.AddAsync(aktifGorusme);
                }

                _chattingForm = new ChattingForm(aktifGorusme, _gorusmeRepository);
                _chattingForm.FormClosed += _chattingFormGorusme_FormClosed;
                _chattingForm.Show();

                await LoadActiveDerslerData();
            }
        }
    }

}
