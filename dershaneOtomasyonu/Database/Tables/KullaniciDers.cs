using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class KullaniciDers
    {
        public int KullaniciId { get; set; }
        public int DersId { get; set; }

        public Kullanici Kullanici { get; set; }
        public Ders Ders { get; set; }
    }

}
