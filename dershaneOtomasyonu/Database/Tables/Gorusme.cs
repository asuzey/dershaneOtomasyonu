using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class Gorusme
    {
        public int Id { get; set; }
        public string Oda { get; set; }
        public string Mesajlar { get; set; }
        public int KatilimciId { get; set; }
        public int OlusturucuId { get; set; }

        public Kullanici Katilimci { get; set; }
        public Kullanici Olusturucu { get; set; }
    }


}
