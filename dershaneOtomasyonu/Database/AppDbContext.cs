﻿using dershaneOtomasyonu.Database.Tables;
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
