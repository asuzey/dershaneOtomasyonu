using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class Sinif
    {
        public int Id { get; set; }
        public string Kodu { get; set; }

        public ICollection<KullaniciSinif> KullaniciSiniflari { get; set; }
    }

}
