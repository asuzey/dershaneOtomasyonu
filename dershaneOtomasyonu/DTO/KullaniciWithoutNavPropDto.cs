using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.DTO
{
    public class KullaniciWithoutNavPropDto
    {
        public int Id { get; set; }
        public string KullaniciAdi { get; set; }
        public string Sifre { get; set; }
        public int RoleId { get; set; }
        public int? SinifId { get; set; }
        public string Adi { get; set; }
        public string Soyadi { get; set; }
        public string Tcno { get; set; }
        public DateTime DogumTarihi { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public string Adres { get; set; }
    }
}
