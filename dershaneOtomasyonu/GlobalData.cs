using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu
{
    public static class GlobalData
    {
        public static int KullaniciId { get; set; } // Nullable yapıldı
        public static string? KullaniciAd { get; set; } // Nullable yapıldı
        public static string? Sifre { get; set; }      // Nullable yapıldı
        public static string? Rol { get; set; }        // Nullable yapıldı
    }
    

}
