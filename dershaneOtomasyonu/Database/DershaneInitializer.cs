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
                    Sifre = "denizgamzeasu471", // hash'lenmemiş düz metin - sonra değiştir
                    RoleId = 1,
                    Adi = "Sistem",
                    Soyadi = "Yöneticisi",
                    Email = "asu.proje01@gmail.com",
                    Tcno = "11111111111",
                });
            }

            // Kategori
            if (!context.SinavKategorileri.Any())
            {
                context.SinavKategorileri.AddRange(
                    new SinavKategori { Adi = "TYT", VarsayilanSure = 165 },
                    new SinavKategori { Adi = "AYT", VarsayilanSure = 180 },
                    new SinavKategori { Adi = "DGS", VarsayilanSure = 180 },
                    new SinavKategori { Adi= "YDS", VarsayilanSure = 120 }
                );
                context.SaveChanges();
            }

            var kategori = context.SinavKategorileri.First(k => k.Adi == "TYT");

            // TYT Dersleri ve Konuları
            var derslerVeKonular = new Dictionary<string, List<string>>
            {
                { "Türkçe", new List<string> { "Sözcükte Anlam", "Cümlede Anlam", "Paragraf", "Dil Bilgisi" } },
                { "Matematik", new List<string> { "Temel Kavramlar", "Sayılar", "Problemler", "Geometri" } },
                { "Fizik", new List<string> { "Hareket", "Kuvvet", "Enerji", "Optik" } },
                { "Kimya", new List<string> { "Atom ve Periyodik Sistem", "Kimyasal Türler", "Asit-Baz", "Organik Kimya" } },
                { "Biyoloji", new List<string> { "Hücre", "DNA", "Ekosistem", "Canlıların Sınıflandırılması" } },
                { "Tarih", new List<string> { "İslamiyet Öncesi", "Osmanlı Devleti", "Kurtuluş Savaşı", "Atatürk İlkeleri" } },
                { "Coğrafya", new List<string> { "Türkiye'nin Coğrafi Bölgeleri", "İklim", "Nüfus", "Doğal Kaynaklar" } },
                { "Felsefe", new List<string> { "Bilgi Felsefesi", "Varlık Felsefesi", "Ahlak Felsefesi", "Sanat Felsefesi" } },
                { "Din Kültürü", new List<string> { "İslam Düşüncesinde Yorumlar", "Hz. Muhammed", "Kur'an", "İbadetler" } },
                { "İngilizce", new List<string> { "Vocabulary", "Reading", "Grammar", "Listening" } },
            };

            var soruSayilari = new Dictionary<string, int>
            {
                { "Türkçe", 40 },
                { "Matematik", 40 },
                { "Fizik", 14 },
                { "Kimya", 13 },
                { "Biyoloji", 13 },
                { "Tarih", 10 },
                { "Coğrafya", 10 },
                { "Felsefe", 7 },
                { "Din Kültürü", 10 },
                { "İngilizce", 50 }
            };


            foreach (var item in derslerVeKonular)
            {
                var ders = new Ders { Adi = item.Key, Aciklama = item.Key + " dersi açıklaması" };
                context.Dersler.Add(ders);
                context.SaveChanges();

                var sinavDers = new SinavDers
                {
                    Adi = $"TYT {item.Key}",
                    DersId = ders.Id, 
                    SinavKategoriId = kategori.Id,
                    SoruSayisi = soruSayilari.ContainsKey(item.Key) ? soruSayilari[item.Key] : 20 // default 20
                };
                context.SinavDersleri.Add(sinavDers);
                context.SaveChanges();

                foreach (var konuAdi in item.Value)
                {
                    context.SinavDersKonulari.Add(new SinavDersKonu
                    {
                        Ad = konuAdi,
                        SinavDersId = sinavDers.Id
                    });
                }
            }

            context.SaveChanges();
        }
    }
}
