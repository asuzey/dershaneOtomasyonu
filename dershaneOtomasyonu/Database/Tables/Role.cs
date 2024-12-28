using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class Role
    {
        public int Id { get; set; }
        public string RolAdi { get; set; }

        public ICollection<Kullanici> Kullanicilar { get; set; }
    }

}
