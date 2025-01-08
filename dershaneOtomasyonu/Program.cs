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


namespace dershaneOtomasyonu
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var services = ConfigureServices();
            var serviceProvider = services.BuildServiceProvider();
            ApplicationConfiguration.Initialize();
            var girisEkrani = serviceProvider.GetRequiredService<GirisEkrani>();
            Application.Run(girisEkrani);
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
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<ILogger, Logger>();
            services.AddScoped<ILogRepository, LogRepository>();
            services.AddScoped<ISinifRepository, SinifRepository>();
            services.AddScoped<IDerslerRepository, DerslerRepository>();
            services.AddScoped<IKullaniciDersRepository, KullaniciDersRepository>();
            services.AddScoped<IKullaniciDosyaRepository, KullaniciDosyaRepository>();
            services.AddScoped<IDersKayitRepository, DersKayitRepository>();
            services.AddScoped<IDegerlendirmeRepository, DegerlendirmeRepository>();
            services.AddScoped<IGorusmeRepository, GorusmeRepository>();


            // Servislere formu ekle
            services.AddScoped<GirisEkrani>();


            return services;
        }
    }
}