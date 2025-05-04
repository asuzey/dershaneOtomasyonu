using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class Sinav
    {
        public int Id { get; set; }
        public string Adi { get; set; }
        public int SinavKategoriId { get; set; }
        public DateTime Tarih { get; set; }
        public int Sure { get; set; }
        public int OlusturucuId { get; set; }


        // Navigation Props
        public SinavKategori SinavKategori { get; set; }
        public Kullanici Olusturucu { get; set; }
        public ICollection<Soru> Sorular { get; set; }
        public ICollection<OgrenciCevap> OgrenciCevaplari { get; set; }
        public ICollection<SinavSonuc> SinavSonuclari { get; set; }
        public ICollection<SinavSoru> SinavSorulari { get; set; }

    }
}
