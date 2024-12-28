using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class KullaniciSinif
    {
        public int KullaniciId { get; set; }
        public int SinifId { get; set; }

        public Kullanici Kullanici { get; set; }
        public Sinif Sinif { get; set; }
    }

}
