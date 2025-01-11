using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class Yoklama
    {
        public int DersKayitId { get; set; } // Ders Kayıt ID'si
        public int KullaniciId { get; set; } // Kullanıcı ID'si
        public DateTime KatilmaTarihi { get; set; }
        public DateTime? AyrilmaTarihi { get; set; }


        // Navigation Properties
        public DersKayit DersKayit { get; set; } // Ders Kayıt ilişkisi
        public Kullanici Kullanici { get; set; } // Kullanıcı ilişkisi
    }


}
