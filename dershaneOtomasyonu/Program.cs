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
            // Global Exception Handling
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
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
                            // Hata loglama iþlemini yap
                            logger.Error("Exception", exception).Wait();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Loglama sýrasýnda bir hata oluþursa konsola yaz
                        Console.WriteLine($"Log thread hatasý: {ex.Message}");
                    }
                }
            })
            {
                IsBackground = true // Uygulama kapanýrken otomatik olarak durdurulmasý için arka plan thread'i
            };
            _logThread.Start();
        }

        private static ServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            // DbContext yapýlandýrmasý (connection string ekleyin)
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
            // Global Exception Handling
            await _logger.Error("Exception", ex);
        }

    }

}