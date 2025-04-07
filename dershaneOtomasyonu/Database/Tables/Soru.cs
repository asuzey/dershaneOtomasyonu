using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class Soru
    {
        public int Id { get; set; }
        public string SoruMetni { get; set; }
        public int SinifSeviyeId { get; set; }
        public short YildizSeviyesi { get; set; }
        public int SinavDersKonuId { get; set; }
        public int SecenekSayisi { get; set; }

        // Navigation Props
        public SinifSeviye SinifSeviye { get; set; }
        public SinavDersKonu SinavDersKonu { get; set; }
        public Sinav Sinav { get; set; }
        public ICollection<Secenek> Secenekler { get; set; } // sorunun seçenekleri

    }
}
