using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class SinavSonuc
    {
        public int Id { get; set; }
        public int KullaniciId { get; set; }
        public int SinavId { get; set; }
        public int ToplamDogrular { get; set; }
        public int ToplamYanlislar { get; set; }
        public int ToplamPuan { get; set; }

        // Navigation Props
        public Kullanici Kullanici { get; set; }
        public Sinav Sinav { get; set; }


    }
}
