using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.DTO
{
    public class RaporYoklamaDto
    {
        public DateTime Tarih { get; set; } // Noktanın tarih etiketi
        public string AdiSoyadi { get; set; } // Kullanıcı adı soyadı
        public string DersAdi { get; set; } // Kullanıcı adı soyadı
        public bool Katildi { get; set; } // Katılım durumu (true: katıldı, false: katılmadı)
        public int XValue { get; set; } // X ekseni için integer değer
        public int YValue { get; set; } // Y ekseni için integer değer
    }
}
