using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class Not
    {
        public int Id { get; set; }
        public string Baslik { get; set; }
        public string Icerik { get; set; }
        public DateTime OlusturmaTarihi { get; set; }

        public ICollection<KullaniciNot> KullaniciNotlari { get; set; }
    }

}
