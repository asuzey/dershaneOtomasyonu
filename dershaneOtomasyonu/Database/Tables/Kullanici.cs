using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class Kullanici
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public int RoleId { get; set; }
        public int? SinifId { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Tcno { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string Adres { get; set; }

        // Navigation Properties
        public Role Role { get; set; } // Kullanıcıların Rolü
        public Sinif Sinif { get; set; } // Kullanıcıların Sınıfı
        public ICollection<DersKayit> DersKayitlari { get; set; } // Kullanıcının ders kayıtları
        public ICollection<Yoklama> Yoklamalar { get; set; } // Kullanıcının yoklamaları
        public ICollection<KullaniciDosya> Dosyalar { get; set; } // Kullanıcının dosyaları
        public ICollection<Gorusme> GorusmelerOlusturucu { get; set; } // Kullanıcının oluşturduğu görüşmeler
        public ICollection<Gorusme> GorusmelerKatilimci { get; set; } // Kullanıcının katıldığı görüşmeler
    }
}
