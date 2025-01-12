using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class Degerlendirme
    {
        public int Id { get; set; }
        public int Puan { get; set; }
        public string? Aciklama { get; set; }
        public int KullaniciId { get; set; }
        public int CreatorId { get; set; }
        public int DersId { get; set; }

        public Kullanici Kullanici { get; set; }
        public Kullanici Creator { get; set; }
        public Ders Ders { get; set; }
    }
}
