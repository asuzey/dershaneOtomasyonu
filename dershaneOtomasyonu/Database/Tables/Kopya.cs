using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class Kopya
    {
        public int Id { get; set; }
        public int SinavId { get; set; }
        public int DosyaId { get; set; }
        public int KullaniciId { get; set; }
        public DateTime Tarih { get; set; }

        // Navigation Properties
        public Sinav Sinav { get; set; }
        public Kullanici Kullanici { get; set; }
        public Dosya Dosya { get; set; }
    }
}
