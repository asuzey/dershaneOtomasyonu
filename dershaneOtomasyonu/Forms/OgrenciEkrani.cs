using Bunifu.UI.WinForms;
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
            _fileService = new FileService();
            panels = [panelDersNotlari, panelNotlarim];

        }

        public static BunifuPanel[] panels;

        private void CikisYap_Click(object sender, EventArgs e)
        {
            GirisEkrani OgrenciEkrani = new GirisEkrani(_logger, _kullaniciRepository, _roleRepository, _baseLogRepository, _logRepository, _sinifRepository, _derslerRepository, _kullaniciDersRepository, _baseDosyaRepository, _kullaniciDosyaRepository, _dersKayitRepository, _degerlendirmeRepository, _gorusmeRepository); // form3e geçiş
            OgrenciEkrani.Show(); // form3ü açıyor
            this.Hide(); // form1i gizleyecek
            OgrenciEkrani.FormClosed += (s, args) => this.Close();
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
        }

        private void InitializeDataGridView()
        {
            // Dosya Adı sütunu
            DosyaDataGridView.ColumnCount = 1;
            DosyaDataGridView.Columns[0].Name = "Dosya Adı";
            DosyaDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            // İndirme butonu
            DataGridViewButtonColumn downloadButtonColumn = new DataGridViewButtonColumn
            {
                Name = "İndir",
                Text = "⬇️",
                UseColumnTextForButtonValue = true,
                Width = 50
            };
            DosyaDataGridView.Columns.Add(downloadButtonColumn);

            DosyaDataGridView.CellClick += DosyaDataGridView_CellClick;
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
                            MessageBox.Show("Dosya indirme işlemi iptal edildi.");
                        }
                    }

                }
            }
        }

        #region BURAYA BAKILACAK FİLE SERVER İLE İLGİLİ KODLAR
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


                        /*

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
                        } */
                    }

                }
            }
        }
        #endregion

        private void BtnNotTutmaPanel_Click(object sender, EventArgs e)
        {
            TogglePanel(panelNotlarim);
        }

        private void BtnKaydet_Click(object sender, EventArgs e)
        {

            // 1. Dosya adı TextBox'tan alınıyor
            string dosyaAdi = txtDosyaAdi.Text;

            // 2. Dosya adı boş mu kontrol ediliyor
            if (string.IsNullOrWhiteSpace(dosyaAdi))
            {
                MessageBox.Show("Lütfen bir dosya adı girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 3. .txt uzantısını kontrol et
            if (!dosyaAdi.EndsWith(".txt"))
            {
                dosyaAdi += ".txt"; // Eğer yoksa uzantıyı ekle
            }

            // 4. Dosya kaydetme yolu belirlenir
            string kaydetmeDizini = @"C:\Dosyalar\";

            // Klasör var mı kontrol edilir, yoksa oluşturulur
            if (!Directory.Exists(kaydetmeDizini))
            {
                Directory.CreateDirectory(kaydetmeDizini);
            }

            // Tam dosya yolu
            string tamDosyaYolu = Path.Combine(kaydetmeDizini, dosyaAdi);


            // 5. RichTextBox içindeki metni al
            string metin = OgrenciNotRichTextBox.Text;

            // 6. Dosya oluştur ve metni içine yaz
            try
            {
                File.WriteAllText(tamDosyaYolu, metin);
                MessageBox.Show($"Dosya başarıyla kaydedildi:\n{tamDosyaYolu}", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Dosya kaydedilirken bir hata oluştu: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void BtnSınıfChat_Click(object sender, EventArgs e)
        {
            TogglePanel(panelSinifGrubu);
            // chat ekranı yönlendirmesi yapılacak ve datagride dersler doldurulacak aynı zamanda açan öğretmen de bu listede gözükecek.
        }
    }
}
