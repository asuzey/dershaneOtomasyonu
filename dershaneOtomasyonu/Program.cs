using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.LogRepositories;
using dershaneOtomasyonu.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Configuration;
using System.Text;
using dershaneOtomasyonu.Helpers;
using dershaneOtomasyonu.Repositories.TableRepositories.SinifRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DerslerRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDersRepositories;
using FluentValidation;
using FluentValidation.Validators;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciDosyaRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DersKayitRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.DegerlendirmeRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.GorusmeRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciNotRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.NotRepositories;
using dershaneOtomasyonu.Repositories.TableRepositories.YoklamaRepositories;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Drawing.Text;
using System.Windows.Forms; // MessageBox için
using System.IO; // log.txt için



namespace dershaneOtomasyonu
{
    internal static class Program
    {
        public static ILogger _logger { get; private set; }
        private static Thread _logThread;
        private static readonly BlockingCollection<Exception> _exceptionQueue = new();
        private static bool _isRunning = true;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            LoadFonts(); // fontları uygulama başlarken yükle

            // Global Exception Handling
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();

            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    // Veritabanı yoksa oluştur
                    if (context.Database.EnsureCreated())
                    {
                        DershaneInitializer.Seed(context);
                    }

                    // İsteğe bağlı log
                    File.AppendAllText("log.txt", DateTime.Now + ": Veritabanı bağlantısı kuruldu.\n");
                }
            }
            catch (Exception ex)
            {
            #if DEBUG
                MessageBox.Show(
                    "Veritabanı bağlantısı kurulamadı. Bu hata genellikle SQL Server veya SSMS yüklü olmadığında oluşur.\n\n" +
                    "Lütfen `localhost` SQL Server örneğinizin açık olduğundan emin olun.\n\n" +
                    "Detay: " + ex.Message,
                    "Geliştirici Uyarısı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            #endif

                // İsteğe bağlı log
                File.AppendAllText("log.txt", DateTime.Now + ": HATA - " + ex.Message + "\n");
            }
                _logger = serviceProvider.GetRequiredService<ILogger>();
            StartLogThread(_logger);
            ApplicationConfiguration.Initialize();
            var girisEkrani = serviceProvider.GetRequiredService<GirisEkrani>();
            Application.Run(girisEkrani);
        }

        private static void StartLogThread(ILogger logger)
        {
            _logThread = new Thread(() =>
            {
                while (_isRunning || !_exceptionQueue.IsCompleted)
                {
                    try
                    {
                        // Kuyruktan bir hata al
                        if (_exceptionQueue.TryTake(out var exception, Timeout.Infinite))
                        {
                            // Hata loglama işlemini yap
                            logger.Error("Exception", exception).Wait();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Loglama sırasında bir hata oluşursa konsola yaz
                        Console.WriteLine($"Log thread hatası: {ex.Message}");
                    }
                }
            })
            {
                IsBackground = true // Uygulama kapanırken otomatik olarak durdurulması için arka plan thread'i
            };
            _logThread.Start();
        }

        private static ServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            // DbContext yapılandırması (connection string ekleyin)
            var connectionString = ConfigurationManager.ConnectionStrings["DershaneDB"].ConnectionString;
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Repository'leri kaydet
            services.AddScoped<IKullaniciRepository, KullaniciRepository>();
            services.AddScoped(typeof(BaseRepository<>));
            services.AddScoped(typeof(IBaseRepository<>), typeof(QueuedBaseRepository<>));
            services.AddScoped<ILogger, Logger>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<ISinifRepository, SinifRepository>();
            services.AddScoped<IDerslerRepository, DerslerRepository>();
            services.AddScoped<IKullaniciDersRepository, KullaniciDersRepository>();
            services.AddScoped<IKullaniciDosyaRepository, KullaniciDosyaRepository>();
            services.AddScoped<IDersKayitRepository, DersKayitRepository>();
            services.AddScoped<IDegerlendirmeRepository, DegerlendirmeRepository>();
            services.AddScoped<IGorusmeRepository, GorusmeRepository>();
            services.AddScoped<INotRepository, NotRepository>();
            services.AddScoped<IKullaniciNotRepository, KullaniciNotRepository>();
            services.AddScoped<IYoklamaRepository, YoklamaRepository>();
            services.AddSingleton(provider =>
            {
                var dbContext = provider.GetRequiredService<AppDbContext>();
                return new QueuedDbContextManager<AppDbContext>(dbContext);
            });


            // Servislere formu ekle
            services.AddScoped<GirisEkrani>();


            return services;
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            LogException(e.Exception);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                LogException(ex);
            }
        }

        private static async void LogException(Exception ex)
        {
            if (_logger != null)
            {
                await _logger.Error("Exception", ex);
            }
            else
            {
                Console.WriteLine($"[HATA] Logger başlatılamadı: {ex.Message}");
            }
        }

        static PrivateFontCollection fontCollection = new PrivateFontCollection();

        static void LoadFonts()
        {
            string[] fontFiles = new[]
            {
                "SourceSansPro-Regular.otf",
                "SourceSansPro-Bold.otf",
                "SourceSansPro-BoldIt.otf",
                "SourceSansPro-It.otf",
                "SourceSansPro-Light.otf",
                "SourceSansPro-LightIt.otf",
                "SourceSansPro-Semibold.otf",
                "SourceSansPro-ExtraLight.otf",
                "SourceSansPro-ExtraLightIt.otf"
            };

            foreach (var fontFile in fontFiles)
            {
                string resourceName = $"dershaneOtomasyonu.Resources.Fonts.{fontFile}";

                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        throw new Exception($"Font bulunamadı: {resourceName}");

                    byte[] fontData = new byte[stream.Length];
                    stream.Read(fontData, 0, (int)stream.Length);

                    IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
                    Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
                    fontCollection.AddMemoryFont(fontPtr, fontData.Length);
                    Marshal.FreeCoTaskMem(fontPtr);
                }
            }

            // Örnek: varsayılan fontu uygula
            Application.SetDefaultFont(new Font(fontCollection.Families[0], 10f));
        }
    }
}