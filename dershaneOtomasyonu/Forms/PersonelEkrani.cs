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
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciNotRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.LogRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.NotRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.SinifRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.YoklamaRepositories;
using Guna.UI2.WinForms;
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
using System.Windows.Forms.DataVisualization.Charting;
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
        private readonly IKullaniciNotRepository _kullaniciNotRepository;
        private readonly INotRepository _notRepository;
        private readonly IYoklamaRepository _yoklamaRepository;
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
            panels = [panelSiniflarVeOgrenciler, panelDersNotuGonderme, panelDersBaslat, panelGorusme, panelRaporlama, panelESinav];
            _fileService = new FileService();

        }


        public static Guna2Panel[] panels;

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


        private async void CikisYap_Click(object sender, EventArgs e)
        {
            GirisEkrani form1 = new GirisEkrani(_logger,
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
                _yoklamaRepository); // form4e geçiş
            form1.Show(); // form4ü açıyor
            this.Hide(); // form1i gizleyecek
            form1.FormClosed += (s, args) => this.Close();
            await _logger.Info($"Çıkış yapıldı. {GlobalData.Kullanici?.Adi}");
        }


        private async void btnSiniflarVeOgrenciler_Click(object sender, EventArgs e)
        {
            var siniflar = await _sinifRepository.GetAllAsync();
            siniflarDataGridView.DataSource = siniflar;
            siniflarDataGridView.Columns[0].Visible = false;
            TogglePanel(panelSiniflarVeOgrenciler);
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
            gorusmeOgrencilerDataGridView.DataSource = sinifinOgrencileri;
            foreach (DataGridViewColumn column in gorusmeOgrencilerDataGridView.Columns)
            {
                column.Visible = false;
            }
            gorusmeOgrencilerDataGridView.Columns[5].Visible = true;
            gorusmeOgrencilerDataGridView.Columns[6].Visible = true;
        }

        private void TogglePanel(Guna2Panel panelToToggle) // panel geçişleri için
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

        private async void btnYeniDosyaYukle_Click(object sender, EventArgs e)
        {
            bool isClassSelected;
            if (SiniflarDosyaDataGridView.CurrentRow == null)
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
                    await _logger.Info($"Yeni dosya yükleme işlemi gerçekleştirildi. {GlobalData.Kullanici?.Adi}");
                }
            }
            else
            {
                var selectedClass = SiniflarDosyaDataGridView.CurrentRow.Cells[1].Value.ToString();
                DialogResult userChoice = MessageBox.Show(
                    $"Seçili dosya {selectedClass} sınıfı ile paylaşılacaktır. Devam edilsin mi?",
                    "Dosya Seçimi",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
                if (userChoice == DialogResult.Yes)
                {
                    await OpenFileSelection();
                    await _logger.Info($"Dosya paylaşma işlemi yapıldı. {GlobalData.Kullanici?.Adi}");
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
                            FilePath = @"C:\Users\Public\FileServer",
                            OlusturucuId = GlobalData.Kullanici!.Id
                        };
                        await _baseDosyaRepository.AddAsync(newFile);
                        var assignment = new KullaniciDosya();
                        assignment.KullaniciId = GlobalData.Kullanici!.Id;
                        assignment.DosyaId = newFile.Id;
                        await _kullaniciDosyaRepository.AddAsync(assignment);
                        var ogrenciler = await _kullaniciRepository.GetAllStudentsAsync();
                        var selectedClass = SiniflarDosyaDataGridView.CurrentRow != null ? SiniflarDosyaDataGridView.CurrentRow.Cells[0] : null;

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

        private async void btnNotGondermePanel_Click(object sender, EventArgs e)
        {
            await LoadClassrooms();
            InitializeDataGridView();
            RefreshFileList();
            TogglePanel(panelDersNotuGonderme);
        }

        private async Task LoadClassrooms()
        {
            var siniflar = await _sinifRepository.GetAllAsync();
            SiniflarDosyaDataGridView.DataSource = siniflar;
            SiniflarDosyaDataGridView.Columns[0].Visible = false;
        }

        private async Task LoadClassroomsOnPanelGorusme()
        {
            var siniflar = await _sinifRepository.GetAllAsync();
            gorusmeSiniflarDataGridView.DataSource = siniflar;
            gorusmeSiniflarDataGridView.Columns[0].Visible = false;
        }

        private void btnSecimiTemizle_Click(object sender, EventArgs e)
        {
            SiniflarDosyaDataGridView.ClearSelection(); // Seçimi temizle
            SiniflarDosyaDataGridView.CurrentCell = null; // CurrentRow'u etkisiz yapar
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
            await HandleDerslerMouseClickAsync(sender, e);
        }

        private async Task HandleDerslerMouseClickAsync(object sender, MouseEventArgs e)
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

                int? dersId = null;
                dersId = dersKayit == null ? await ShowInputDialogCombobox() : dersKayit.DersId;

                if (dersId == null)
                {
                    MessageBox.Show("Lütfen bir ders seçimi yapınız.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (dersKayit == null)
                {
                    dersKayit = new DersKayit();
                    dersKayit.SinifId = sinifId;
                    dersKayit.KullaniciId = GlobalData.Kullanici!.Id;
                    dersKayit.Oda = $"{sinifAdi}-{GlobalData.Kullanici.Id}-{DateTime.Now}";
                    dersKayit.Durum = true;
                    dersKayit.BaslangicTarihi = DateTime.Now;
                    dersKayit.DersId = (int)dersId;
                    await _dersKayitRepository.AddAsync(dersKayit);
                    await _logger.Info($"Ders başlatıldı. {GlobalData.Kullanici?.Adi}");
                }

                _chattingForm = new ChattingForm(dersKayit, _dersKayitRepository, _yoklamaRepository);
                _chattingForm.FormClosed += _chattingForm_FormClosed;
                _chattingForm.Show();

                await LoadActiveDerslerData();

            }
        }

        private async void _chattingForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Thread.Sleep(2000); // 2000 milisaniye (2 saniye)
            await LoadActiveDerslerData();
            await _logger.Info($"Ders sonlandırıldı. {GlobalData.Kullanici?.Adi}");
        }
        private async void _chattingFormGorusme_FormClosed(object sender, FormClosedEventArgs e)
        {
            Thread.Sleep(2000); // 2000 milisaniye (2 saniye)
            await LoadActiveDerslerData();
            await _logger.Info($"Görüşme sonlandırıldı. {GlobalData.Kullanici?.Adi}");
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

        public async Task<Tuple<int, string, int>> ShowInputDialog()
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 250,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Değer Girişi",
                StartPosition = FormStartPosition.CenterScreen
            };

            Label rateLabel = new Label() { Left = 20, Top = 20, Text = "Puan (0-100):", AutoSize = true };
            TextBox rateInput = new TextBox() { Left = 150, Top = 20, Width = 200 };

            Label descLabel = new Label() { Left = 20, Top = 60, Text = "Açıklama (isteğe bağlı):", AutoSize = true };
            TextBox descInput = new TextBox() { Left = 150, Top = 60, Width = 200 };

            Label comboLabel = new Label() { Left = 20, Top = 100, Text = "Ders Seçimi:", AutoSize = true };
            ComboBox comboBox = new ComboBox() { Left = 150, Top = 100, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            // Dersler tablosundan verileri doldurmak için EF Core kullanımı
            var dersler = await _derslerRepository.GetAllByOgretmenIdAsync(GlobalData.Kullanici!.Id);
            comboBox.DataSource = dersler;
            comboBox.DisplayMember = "Adi"; // Görünecek sütun
            comboBox.ValueMember = "Id";       // Seçildiğinde alacağınız ID sütunu

            Button confirmation = new Button() { Text = "Tamam", Left = 150, Width = 90, Top = 150 };
            Button cancel = new Button() { Text = "İptal", Left = 260, Width = 90, Top = 150, DialogResult = DialogResult.Cancel };

            confirmation.Click += (sender, e) =>
            {
                // Zorunlu rate kontrolü
                if (!int.TryParse(rateInput.Text, out int rate) || rate < 0 || rate > 100)
                {
                    MessageBox.Show("Lütfen 0 ile 100 arasında geçerli bir sayısal puan değeri girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Form kapanmaz
                }

                // Zorunlu ComboBox seçimi kontrolü
                if (comboBox.SelectedValue == null)
                {
                    MessageBox.Show("Lütfen bir ders seçin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            prompt.Controls.Add(comboLabel);
            prompt.Controls.Add(comboBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);

            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                int rate = int.Parse(rateInput.Text); // Rate doğrulandığından kesin parse edilir
                string description = descInput.Text; // Description opsiyonel
                int selectedDersId = (int)comboBox.SelectedValue; // ComboBox'tan seçilen ID
                return Tuple.Create(rate, description, selectedDersId);
            }

            return null; // Kullanıcı iptal ettiyse
        }
        public async Task<int?> ShowInputDialogCombobox()
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Değer Girişi",
                StartPosition = FormStartPosition.CenterScreen
            };

            Label comboLabel = new Label() { Left = 20, Top = 20, Text = "Ders Seçimi:", AutoSize = true };
            ComboBox comboBox = new ComboBox() { Left = 150, Top = 20, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            // Dersler tablosundan verileri doldurmak için EF Core kullanımı
            var dersler = await _derslerRepository.GetAllByOgretmenIdAsync(GlobalData.Kullanici!.Id);
            comboBox.DataSource = dersler;
            comboBox.DisplayMember = "Adi"; // Görünecek sütun
            comboBox.ValueMember = "Id";       // Seçildiğinde alacağınız ID sütunu

            Button confirmation = new Button() { Text = "Tamam", Left = 150, Width = 90, Top = 60 };
            Button cancel = new Button() { Text = "İptal", Left = 260, Width = 90, Top = 60, DialogResult = DialogResult.Cancel };

            confirmation.Click += (sender, e) =>
            {
                // Zorunlu ComboBox seçimi kontrolü
                if (comboBox.SelectedValue == null)
                {
                    MessageBox.Show("Lütfen bir ders seçin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Form kapanmaz
                }

                // Validasyon başarılı, form kapatılır
                prompt.DialogResult = DialogResult.OK;
                prompt.Close();
            };

            prompt.Controls.Add(comboLabel);
            prompt.Controls.Add(comboBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);

            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            if (prompt.ShowDialog() == DialogResult.OK)
            {
                int selectedDersId = (int)comboBox.SelectedValue; // ComboBox'tan seçilen ID
                return selectedDersId;
            }

            return null; // Kullanıcı iptal ettiyse
        }

        private async void btnDegerlendirmeYap_Click(object sender, EventArgs e)
        {
            if (chattingOgrDataGridView.CurrentRow == null)
            {
                MessageBox.Show("Lütfen öğrenci tablosundan bir kayıt seçiniz.", "Bilgi");
                return;
            }

            var result = await ShowInputDialog();
            if (result != null)
            {
                var degerlendirme = new Degerlendirme();
                degerlendirme.Puan = result.Item1;
                degerlendirme.Aciklama = result.Item2;
                degerlendirme.KullaniciId = Convert.ToInt32(chattingOgrDataGridView.CurrentRow.Cells[0].Value);
                degerlendirme.CreatorId = GlobalData.Kullanici!.Id;
                degerlendirme.DersId = result.Item3;
                await _degerlendirmeRepository.AddAsync(degerlendirme);
                MessageBox.Show($"Değerlendirme kaydedildi.", "Bilgi");
                await _logger.Info($"Değerlendirme yapıldı. {GlobalData.Kullanici?.Adi}");
            }
            else ShowInputDialog(); // eğer hatalı veri girişi yapılırsa diyalog box tekrar açılıyor.
        }

        private async void btnGorusmePanel_Click(object sender, EventArgs e)
        {
            await LoadClassroomsOnPanelGorusme();
            await LoadDevamEdenGorusmeler();
            TogglePanel(panelGorusme);
        }

        private async Task LoadDevamEdenGorusmeler()
        {
            var gorusmeler = await _gorusmeRepository.GetAktifGorusmelerByOlusturucuIdAsync(GlobalData.Kullanici!.Id);
            var gorusmedto = gorusmeler.Select(x => new
            {
                x.Id,
                x.Katilimci.SinifId,
                OgrenciId = x.Katilimci.Id,
                Katilimci = x.Katilimci.Adi + " " + x.Katilimci.Soyadi,
            });
            devamEdenGorusmeDataGridView.DataSource = gorusmedto.ToList();
            devamEdenGorusmeDataGridView.Columns[0].Visible = false;
            devamEdenGorusmeDataGridView.Columns[1].Visible = false;
            devamEdenGorusmeDataGridView.Columns[2].Visible = false;
        }

        private async void gorusmeSiniflarDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            await HandleMouseClickAsync(sender, e);
        }

        private async Task HandleMouseClickAsync(object sender, MouseEventArgs e)
        {
            var sinifId = 0;
            if (gorusmeSiniflarDataGridView.CurrentRow != null)
                sinifId = Convert.ToInt32(gorusmeSiniflarDataGridView.CurrentRow.Cells[0].Value);

            if (sinifId != 0)
                await LoadSinifinOgrencileriToDersBaslatOnPanelGorusme(sinifId);
        }

        private async void btnGorusmeBaslat_Click(object sender, EventArgs e)
        {
            // görüşme başlat kodu yazacağım
            if (gorusmeSiniflarDataGridView.CurrentRow == null || gorusmeOgrencilerDataGridView.CurrentRow == null)
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

                var ogrenciId = Convert.ToInt32(gorusmeOgrencilerDataGridView.CurrentRow.Cells[0].Value);
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
                    await _logger.Info($"Görüşme başlatıldı. {GlobalData.Kullanici?.Adi}");
                }

                _chattingForm = new ChattingForm(aktifGorusme, _gorusmeRepository);
                _chattingForm.FormClosed += _chattingFormGorusme_FormClosed;
                _chattingForm.Show();

                await LoadActiveDerslerData();
            }
        }

        private async void btnRaporlamaPanel_Click(object sender, EventArgs e)
        {
            await LoadClassRoomsOnReportPanel();
            TogglePanel(panelRaporlama);
        }

        private async Task LoadClassRoomsOnReportPanel()
        {
            var siniflar = await _sinifRepository.GetAllAsync();
            cbSiniflar.DataSource = siniflar;
            cbSiniflar.DisplayMember = "Kodu"; // görünen
            cbSiniflar.ValueMember = "Id"; // seçilen
        }

        private async void activeDerslerDataGrid_MouseClick(object sender, MouseEventArgs e)
        {
            if (activeDerslerDataGrid.CurrentRow == null) return;
            var selectedId = activeDerslerDataGrid.CurrentRow.Cells["Id"].Value;
            chattingSinifDataGridView.ClearSelection();
            // İlk gridde bu ID değerine sahip satırı bul ve seç
            foreach (DataGridViewRow row in chattingSinifDataGridView.Rows)
            {
                if (row.Cells["Id"].Value.Equals(selectedId))
                {
                    // Satırı seçili hale getir
                    chattingSinifDataGridView.CurrentCell = row.Cells[1];
                    row.Selected = true;
                    await HandleDerslerMouseClickAsync(sender, e);
                    // Seçili satırı görünür yapmak (isteğe bağlı)
                    chattingSinifDataGridView.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        private async void DevamEdenGorusmeDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (devamEdenGorusmeDataGridView.CurrentRow == null) return;

            var selectedId = devamEdenGorusmeDataGridView.CurrentRow.Cells["SinifId"].Value;
            gorusmeSiniflarDataGridView.ClearSelection();
            foreach (DataGridViewRow row in gorusmeSiniflarDataGridView.Rows)
            {
                if (row.Cells["Id"].Value.Equals(selectedId))
                {
                    // Satırı seçili hale getir
                    row.Selected = true;
                    gorusmeSiniflarDataGridView.CurrentCell = row.Cells[1];
                    // Seçili satırı görünür yapmak (isteğe bağlı)
                    gorusmeSiniflarDataGridView.FirstDisplayedScrollingRowIndex = row.Index;

                    await HandleMouseClickAsync(sender, e);

                    break;
                }
            }

            var selectedOgrenciId = devamEdenGorusmeDataGridView.CurrentRow.Cells["OgrenciId"].Value;
            gorusmeOgrencilerDataGridView.ClearSelection();
            foreach (DataGridViewRow row in gorusmeOgrencilerDataGridView.Rows)
            {
                if (row.Cells["Id"].Value.Equals(selectedOgrenciId))
                {
                    row.Selected = true;
                    gorusmeOgrencilerDataGridView.CurrentCell = row.Cells[5];
                    gorusmeOgrencilerDataGridView.FirstDisplayedScrollingRowIndex = row.Index;
                    break;
                }
            }
        }

        private async void cbSiniflar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(cbSiniflar.SelectedValue?.ToString(), out int selectedValue))
            {
                return;
            }
            int sinifId = Convert.ToInt32(cbSiniflar.SelectedValue);
            var ogrenciler = await _kullaniciRepository.GetAllStudentsAsync();
            var sinifinOgrencileri = ogrenciler.Where(o => o.SinifId == sinifId).ToList();
            ogrListDataGridView.DataSource = sinifinOgrencileri;
            foreach (DataGridViewColumn column in ogrListDataGridView.Columns)
            {
                column.Visible = false;
            }
            ogrListDataGridView.Columns[5].Visible = true;
            ogrListDataGridView.Columns[6].Visible = true;
        }

        private async void ogrListDataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            var ogrenciId = 0;
            if (ogrListDataGridView.CurrentRow != null) ogrenciId = Convert.ToInt32(ogrListDataGridView.CurrentRow.Cells[0].Value);
            if (ogrenciId != 0) await LoadOgrenciReport(ogrenciId);
        }

        private async Task LoadOgrenciReport(int ogrenciId)
        {
            // Yoklama verilerini al
            var yoklamaRaporu = await _kullaniciRepository.GetAllYoklamaRaporByOgrenciIdAsync(ogrenciId);

            if (!yoklamaRaporu.Any())
            {
                MessageBox.Show("Bu öğrenci için yoklama verisi bulunamadı.");
                return;
            }

            // DataGridView'i temizle
            raporYoklamaDataGrid.Rows.Clear();
            raporYoklamaDataGrid.Columns.Clear();

            // Tarih sütunlarını oluştur
            var tarihListesi = yoklamaRaporu
                .Select(r => r.Tarih.Date) // Sadece tarih kısmını alıyoruz
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            foreach (var tarih in tarihListesi)
            {
                var column = new DataGridViewCheckBoxColumn
                {
                    HeaderText = tarih.ToShortDateString(), // Tarihi sütun başlığına yaz
                    Name = tarih.ToShortDateString(),
                    ReadOnly = true // Checkbox'lar düzenlenemez olacak
                };
                raporYoklamaDataGrid.Columns.Add(column);
            }

            // Ders adlarını satır başlığı olarak ekle
            var dersListesi = yoklamaRaporu
                .Select(r => r.DersAdi) // Ders adlarını alıyoruz
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            foreach (var dersAdi in dersListesi)
            {
                var rowIndex = raporYoklamaDataGrid.Rows.Add();
                raporYoklamaDataGrid.Rows[rowIndex].HeaderCell.Value = dersAdi; // Satır başlığına ders adını ekle

                // Her tarih için bu dersin kaydı olup olmadığını kontrol et
                foreach (var tarih in tarihListesi)
                {
                    var dersTarihi = yoklamaRaporu.FirstOrDefault(r => r.DersAdi == dersAdi && r.Tarih.Date == tarih);

                    var cell = raporYoklamaDataGrid.Rows[rowIndex].Cells[tarihListesi.IndexOf(tarih)];

                    if (dersTarihi == null)
                    {
                        // Dersin bu tarihte kaydı yok, hücreyi boş bırak
                        cell.Value = DBNull.Value;
                    }
                    else
                    {
                        // Ders kaydı varsa, katılım durumunu ayarla
                        cell.Value = dersTarihi.Katildi;

                        // Hücre rengini ayarla
                        cell.Style.BackColor = dersTarihi.Katildi
                            ? Color.FromArgb(200, 255, 200) // Hafif yeşil
                            : Color.FromArgb(255, 200, 200); // Hafif kırmızı
                    }
                }
            }

            // DataGridView düzenleme
            raporYoklamaDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; // Sütun genişliklerini otomatik ayarla
            raporYoklamaDataGrid.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders; // Satır başlıklarını otomatik genişlet
            raporYoklamaDataGrid.AllowUserToAddRows = false; // Kullanıcının yeni satır eklemesini engelle
            raporYoklamaDataGrid.RowHeadersVisible = true; // Satır başlıklarını görünür yap
            raporYoklamaDataGrid.ClearSelection();

            await InitializeGraph(ogrenciId);
        }

        private async Task InitializeGraph(int ogrenciId)
        {
            // Verileri alın
            var degerlendirmeler = await _degerlendirmeRepository.GetDegerlendirmelerByKullaniciIdAsync(ogrenciId);
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
            raporDegerlendirmeChart.Series.Clear();
            raporDegerlendirmeChart.Titles.Clear();
            raporDegerlendirmeChart.ChartAreas.Clear();
            raporDegerlendirmeChart.Legends.Clear();

            // ChartArea ekle
            var chartArea = new ChartArea("ChartArea");
            raporDegerlendirmeChart.ChartAreas.Add(chartArea);

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
            raporDegerlendirmeChart.Series.Add(ortalamaSerisi);
            raporDegerlendirmeChart.Series.Add(sonPuanSerisi);

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
            raporDegerlendirmeChart.Legends.Add(new Legend("Legend")
            {
                Docking = Docking.Top,
                Alignment = System.Drawing.StringAlignment.Center
            });

            // Grafik başlığı ekle
            raporDegerlendirmeChart.Titles.Add("Derslere Göre Ortalama ve Son Puan");

        }

        private void btnESinavPanel_Click(object sender, EventArgs e)
        {
            TogglePanel(panelESinav);
        }

        private void PersonelEkrani_Load(object sender, EventArgs e)
        {
            // Tüm panelleri gizle
            foreach (var panel in panels)
            {
                panel.Visible = false;
            }
        }

        private void guna2Button1_Click(object sender, EventArgs e)
        {

        }
    }

}
