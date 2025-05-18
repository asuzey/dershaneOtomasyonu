using dershaneOtomasyonu.Database.Tables;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace dershaneOtomasyonu.Database
{
    public static class DershaneInitializer
    {
        public static void Seed(AppDbContext context)
        {
            if (!context.Database.EnsureCreated()) return;

            // Admin Kullanıcı
            if (!context.Kullanicilar.Any())
            {
                context.Kullanicilar.Add(new Kullanici
                {
                    KullaniciAdi = "admin",
                    Sifre = "denizgamzeasu471",
                    RoleId = 1,
                    SinifId = null, // admin sınıfa ait olmayabilir
                    Adi = "Sistem",
                    Soyadi = "Yöneticisi",
                    Tcno = "11111111111",
                    DogumTarihi = new DateTime(1990, 1, 1),
                    Telefon = "05555555555",
                    Email = "asu.proje01@gmail.com",
                    Adres = "Merkez / İstanbul"
                });
                context.SaveChanges(); // ⬅️ kullanıcıyı veritabanına kaydet
            }


            // Kategori
            if (!context.SinavKategorileri.Any())
            {
                context.SinavKategorileri.AddRange(
                    new SinavKategori { Adi = "TYT", VarsayilanSure = 165 },
                    new SinavKategori { Adi = "AYT", VarsayilanSure = 180 },
                    new SinavKategori { Adi = "DGS", VarsayilanSure = 180 },
                    new SinavKategori { Adi = "YDS", VarsayilanSure = 120 }
                );
                context.SaveChanges();
            }

            var kategori = context.SinavKategorileri.FirstOrDefault(k => k.Adi == "TYT");
            if (kategori == null)
            {
                kategori = new SinavKategori { Adi = "TYT", VarsayilanSure = 165 };
                context.SinavKategorileri.Add(kategori);
                context.SaveChanges();
            }


            // Dersler tablosu boşsa ekle
            if (!context.Dersler.Any())
            {
                var derslerVeKonular = new Dictionary<string, List<string>>
                {
                    { "Türkçe", new() { "Sözcükte Anlam", "Cümlede Anlam", "Paragraf", "Dil Bilgisi" } },
                    { "Matematik", new() { "Temel Kavramlar", "Sayılar", "Problemler", "Geometri" } },
                    { "Fizik", new() { "Hareket", "Kuvvet", "Enerji", "Optik" } },
                    { "Kimya", new() { "Atom ve Periyodik Sistem", "Kimyasal Türler", "Asit-Baz", "Organik Kimya" } },
                    { "Biyoloji", new() { "Hücre", "DNA", "Ekosistem", "Canlıların Sınıflandırılması" } },
                    { "Tarih", new() { "İslamiyet Öncesi", "Osmanlı Devleti", "Kurtuluş Savaşı", "Atatürk İlkeleri" } },
                    { "Coğrafya", new() { "Türkiye'nin Coğrafi Bölgeleri", "İklim", "Nüfus", "Doğal Kaynaklar" } },
                    { "Felsefe", new() { "Bilgi Felsefesi", "Varlık Felsefesi", "Ahlak Felsefesi", "Sanat Felsefesi" } },
                    { "Din Kültürü", new() { "İslam Düşüncesinde Yorumlar", "Hz. Muhammed", "Kur'an", "İbadetler" } },
                    { "İngilizce", new() { "Vocabulary", "Reading", "Grammar", "Listening" } },
                };

                var soruSayilari = new Dictionary<string, int>
                 {
                     { "Türkçe", 40 }, { "Matematik", 40 }, { "Fizik", 14 },
                     { "Kimya", 13 }, { "Biyoloji", 13 }, { "Tarih", 10 },
                     { "Coğrafya", 10 }, { "Felsefe", 7 }, { "Din Kültürü", 10 }, { "İngilizce", 50 }
                 };

                foreach (var item in derslerVeKonular)
                {
                    var ders = new Ders
                    {
                        Adi = item.Key,
                        Aciklama = item.Key + " dersi açıklaması"
                    };
                    context.Dersler.Add(ders);
                    context.SaveChanges();

                    var sinavDers = new SinavDers
                    {
                        Adi = $"TYT {item.Key}",
                        DersId = ders.Id,
                        SinavKategoriId = kategori.Id,
                        SoruSayisi = soruSayilari.TryGetValue(item.Key, out var sayi) ? sayi : 20
                    };
                    context.SinavDersleri.Add(sinavDers);
                    context.SaveChanges();

                    foreach (var konu in item.Value)
                    {
                        context.SinavDersKonulari.Add(new SinavDersKonu
                        {
                            Ad = konu,
                            SinavDersId = sinavDers.Id
                        });
                    }
                }

                context.SaveChanges();
            }
        }
    }
}
