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
            // Servis koleksiyonunu oluþtur
            var services = ConfigureServices();

            // Servis saðlayýcýyý oluþtur
            var serviceProvider = services.BuildServiceProvider();

            // Uygulama yapýlandýrmasýný baþlat
            ApplicationConfiguration.Initialize();

            // Dependency Injection ile giriþ ekranýný baþlat
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

            // Servislere formu ekle
            services.AddScoped<GirisEkrani>();


            return services;
        }
    }
}