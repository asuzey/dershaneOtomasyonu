using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class Ders
    {
        public int Id { get; set; }
        public string Adi { get; set; }
        public string Aciklama { get; set; }

        public ICollection<KullaniciDers> KullaniciDersleri { get; set; }
    }

}
