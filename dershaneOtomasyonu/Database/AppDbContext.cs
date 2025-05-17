using dershaneOtomasyonu.Database.Tables;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Role> Roller { get; set; }
        public DbSet<Sinif> Siniflar { get; set; }
        public DbSet<Ders> Dersler { get; set; }
        public DbSet<KullaniciDers> KullaniciDersleri { get; set; }
        public DbSet<Not> Notlar { get; set; }
        public DbSet<KullaniciNot> KullaniciNotlari { get; set; }
        public DbSet<Degerlendirme> Degerlendirmeler { get; set; }
        public DbSet<DersKayit> DersKayitlari { get; set; }
        public DbSet<Yoklama> Yoklamalar { get; set; }
        public DbSet<Dosya> Dosyalar { get; set; }
        public DbSet<KullaniciDosya> KullaniciDosyalari { get; set; }
        public DbSet<Gorusme> Gorusmeler { get; set; }
        public DbSet<LogEntry> Logs { get; set; }

        public DbSet<Kopya> Kopyalar { get; set; }
        public DbSet<OgrenciCevap> OgrenciCevaplari { get; set; }
        public DbSet<Secenek> Secenekler { get; set; }
        public DbSet<Sinav> Sinavlar { get; set; }
        public DbSet<SinavDers> SinavDersleri { get; set; }
        public DbSet<SinavDersKonu> SinavDersKonulari { get; set; }
        public DbSet<SinavKategori> SinavKategorileri { get; set; }
        public DbSet<SinavSoru> SinavSorulari { get; set; }
        public DbSet<SinavSonuc> SinavSonuclari { get; set; }
        public DbSet<Soru> Sorular { get; set; }
        public DbSet<SinifSeviye> SinifSeviyeleri { get; set; }
        public DbSet<OgrenciSinav> OgrenciSinavlari { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // App.config dosyasındaki bağlantı dizesini kullan
                var connectionString = ConfigurationManager.ConnectionStrings["DershaneDB"].ConnectionString;
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Gorusme - Kullanici İlişkisi
            modelBuilder.Entity<Gorusme>()
                .HasOne(g => g.Katilimci)
                .WithMany(k => k.GorusmelerKatilimci)
                .HasForeignKey(g => g.KatilimciId)
                .OnDelete(DeleteBehavior.Restrict); // Çift yönlü cascade önlemek için

            // Gorusme - Kullanici İlişkisi
            modelBuilder.Entity<Dosya>()
                .HasOne(g => g.Olusturucu)
                .WithMany(k => k.Dosyalar)
                .HasForeignKey(g => g.OlusturucuId)
                .OnDelete(DeleteBehavior.Restrict); // Çift yönlü cascade önlemek için

            modelBuilder.Entity<Gorusme>()
                .HasOne(g => g.Olusturucu)
                .WithMany(k => k.GorusmelerOlusturucu)
                .HasForeignKey(g => g.OlusturucuId)
                .OnDelete(DeleteBehavior.Restrict); // Çift yönlü cascade önlemek için

            modelBuilder.Entity<Degerlendirme>()
                .HasOne(g => g.Creator)
                .WithMany(k => k.OgretmenDegerlendirmeleri)
                .HasForeignKey(g => g.CreatorId)
                .OnDelete(DeleteBehavior.Restrict); // Çift yönlü cascade önlemek için

            modelBuilder.Entity<Degerlendirme>()
                .HasOne(g => g.Kullanici)
                .WithMany(k => k.OgrenciDegerlendirmeleri)
                .HasForeignKey(g => g.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict); // Çift yönlü cascade önlemek için

            // DersKayitlari ilişkisi
            modelBuilder.Entity<Yoklama>()
                .HasOne(y => y.DersKayit)
                .WithMany(d => d.Yoklamalar)
                .HasForeignKey(y => y.DersKayitId)
                .OnDelete(DeleteBehavior.Cascade); // DersKayitları silindiğinde yoklamaları sil

            // Kullanicilar ilişkisi
            modelBuilder.Entity<Yoklama>()
                .HasOne(y => y.Kullanici)
                .WithMany(k => k.Yoklamalar)
                .HasForeignKey(y => y.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict); // Kaskadlı silmeyi önle

            // Kullanicilar ilişkisi
            modelBuilder.Entity<DersKayit>()
                .HasOne(y => y.Ders)
                .WithMany(k => k.DersKayitlari)
                .HasForeignKey(y => y.DersId)
                .OnDelete(DeleteBehavior.Restrict); // Kaskadlı silmeyi önle

            modelBuilder.Entity<LogEntry>()
                .HasOne(le => le.Kullanici)
                .WithMany()
                .HasForeignKey(le => le.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict); // Cascade yerine Restrict kullan

            // Kullanıcı-Ders ilişkisi
            modelBuilder.Entity<KullaniciDers>()
                .HasKey(kd => new { kd.KullaniciId, kd.DersId });

            // Kullanıcı-Not ilişkisi
            modelBuilder.Entity<KullaniciNot>()
                .HasKey(kn => new { kn.KullaniciId, kn.NotId });

            // Yoklamalar
            modelBuilder.Entity<Yoklama>()
                .HasKey(y => new { y.DersKayitId, y.KullaniciId });

            // Kullanici Dosyalari
            modelBuilder.Entity<KullaniciDosya>()
                .HasKey(y => new { y.DosyaId, y.KullaniciId });

            // Kullanici Sinavlari
            modelBuilder.Entity<OgrenciSinav>()
                .HasKey(y => new { y.KullaniciId, y.SinavId });




            // Sinav - Kullanici İlişkisi
            modelBuilder.Entity<Sinav>()
                .HasOne(g => g.Olusturucu)
                .WithMany(k => k.SinavlarOlusturucu)
                .HasForeignKey(g => g.OlusturucuId)
                .OnDelete(DeleteBehavior.Restrict); // Çift yönlü cascade önlemek için

            modelBuilder.Entity<Sinav>()
                .HasOne(s => s.SinavKategori)
                .WithMany(sk => sk.Sinavlar)
                .HasForeignKey(s => s.SinavKategoriId)
                .OnDelete(DeleteBehavior.Restrict); // Kategori silinse bile sınavlar kalsın




            modelBuilder.Entity<SinavSoru>()
                .HasKey(ss => new { ss.SinavId, ss.SoruId }); // composite PK

            modelBuilder.Entity<SinavSoru>()
                .HasOne(ss => ss.Sinav)
                .WithMany(s => s.SinavSorulari)
                .HasForeignKey(ss => ss.SinavId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SinavSoru>()
                .HasOne(ss => ss.Soru)
                .WithMany(s => s.SorununSinavlari)
                .HasForeignKey(ss => ss.SoruId)
                .OnDelete(DeleteBehavior.Restrict);




            // Kullanici Dosyalari
            modelBuilder.Entity<SinavSoru>()
                .HasKey(y => new { y.SinavId, y.SoruId });

            modelBuilder.Entity<Soru>()
                .HasOne(s => s.SinifSeviye)
                .WithMany(sv => sv.Sorular)
                .HasForeignKey(s => s.SinifSeviyeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Soru>()
                .HasOne(s => s.SinavDersKonu)
                .WithMany(sdk => sdk.Sorular)
                .HasForeignKey(s => s.SinavDersKonuId)
                .OnDelete(DeleteBehavior.Restrict);

            // Secenek sayisina default deger verme
            modelBuilder.Entity<Soru>()
                .Property(t => t.SecenekSayisi)
                .HasDefaultValue(4);

            modelBuilder.Entity<SinavSonuc>()
                .HasOne(ss => ss.Kullanici)
                .WithMany(k => k.OgrenciSinavSonuclari)
                .HasForeignKey(ss => ss.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict); // Öğrenci silinirse sonuçlar kalsın

            modelBuilder.Entity<SinavSonuc>()
                .HasOne(ss => ss.Sinav)
                .WithMany(s => s.SinavSonuclari)
                .HasForeignKey(ss => ss.SinavId)
                .OnDelete(DeleteBehavior.Cascade); // Sınav silinirse sonuçlar da silinsin

            modelBuilder.Entity<SinavDers>()
                .HasOne(sd => sd.SinavKategori)
                .WithMany(sk => sk.SinavDersleri)
                .HasForeignKey(sd => sd.SinavKategoriId)
                .OnDelete(DeleteBehavior.Cascade); // Kategori silinirse dersleri de sil

            modelBuilder.Entity<SinavDersKonu>()
                .HasOne(sdk => sdk.SinavDers)
                .WithMany(sd => sd.SinavDersKonulari)
                .HasForeignKey(sdk => sdk.SinavDersId)
                .OnDelete(DeleteBehavior.Cascade); // Ders silinirse konuları da sil

            modelBuilder.Entity<Secenek>()
                .HasOne(s => s.Soru)
                .WithMany(soru => soru.Secenekler)
                .HasForeignKey(s => s.SoruId)
                .OnDelete(DeleteBehavior.Cascade); // Soru silinirse seçenekler de silinir

            modelBuilder.Entity<OgrenciCevap>()
                .HasOne(oc => oc.Secenek)
                .WithMany() // Secenek → OgrenciCevap yönü tanımlı değil
                .HasForeignKey(oc => oc.SecenekId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OgrenciCevap>()
                .HasOne(oc => oc.Kullanici)
                .WithMany() // Kullanici tarafında navigation tanımı yoksa
                .HasForeignKey(oc => oc.KullaniciId)
                .OnDelete(DeleteBehavior.Restrict);




            base.OnModelCreating(modelBuilder);
        }
    }


    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            // Bağlantı dizesini belirleyin
            var connectionString = ConfigurationManager.ConnectionStrings["DershaneDB"].ConnectionString;
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
