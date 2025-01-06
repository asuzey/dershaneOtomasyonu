using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class DersKayit
    {
        public int Id { get; set; } // Otomatik artan birincil anahtar
        public int SinifId { get; set; } // Sınıf ID'si
        public int KullaniciId { get; set; } // Kullanıcı ID'si (derse kaydolan)
        public string Oda { get; set; } // Oda adı veya kimliği
        public string? Mesajlar { get; set; } // Mesajlar
        public bool Durum { get; set; } // Durum (Aktif/Deaktif)

        // Navigation Properties
        public Sinif Sinif { get; set; } // Sınıf ilişkisi
        public Kullanici Kullanici { get; set; } // Kullanıcı ilişkisi
        public ICollection<Yoklama> Yoklamalar { get; set; } // Yoklamalar ile ilişki
    }

}
