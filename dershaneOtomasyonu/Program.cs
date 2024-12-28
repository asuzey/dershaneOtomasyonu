using dershaneOtomasyonu.Database;
using dershaneOtomasyonu.Repositories.TableRepositories.KullaniciRepositories;
using dershaneOtomasyonu.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Configuration;

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
            // Servis koleksiyonunu olu�tur
            var services = ConfigureServices();

            // Servis sa�lay�c�y� olu�tur
            var serviceProvider = services.BuildServiceProvider();

            // Uygulama yap�land�rmas�n� ba�lat
            ApplicationConfiguration.Initialize();

            // Dependency Injection ile giri� ekran�n� ba�lat
            var girisEkrani = serviceProvider.GetRequiredService<GirisEkrani>();
            Application.Run(girisEkrani);
        }

        private static ServiceCollection ConfigureServices()
        {
            var services = new ServiceCollection();

            // DbContext yap�land�rmas� (connection string ekleyin)
            var connectionString = ConfigurationManager.ConnectionStrings["DershaneDB"].ConnectionString;
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Repository'leri kaydet
            services.AddScoped<IKullaniciRepository, KullaniciRepository>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

            // Servislere formu ekle
            services.AddScoped<GirisEkrani>();


            return services;
        }
    }
}