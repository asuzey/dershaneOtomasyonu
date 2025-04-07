using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class OgrenciCevap
    {
        public int Id { get; set; }
        public int SecenekId { get; set; }
        public int KullaniciId { get; set; }
        public int Sure { get; set; }
        public DateTime OlusturmaTarihi { get; set; }

        // Navigation Props
        public Secenek Secenek { get; set; }
        public Kullanici Kullanici { get; set; }
    }
}
