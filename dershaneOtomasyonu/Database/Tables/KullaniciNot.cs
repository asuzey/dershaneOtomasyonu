using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class KullaniciNot
    {
        public int KullaniciId { get; set; }
        public int NotId { get; set; }

        public Kullanici Kullanici { get; set; }
        public Not Not { get; set; }
    }

}
