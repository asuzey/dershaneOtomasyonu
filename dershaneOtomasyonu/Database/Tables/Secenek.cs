using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class Secenek
    {
        public int Id { get; set; }
        public int SoruId { get; set; }
        public string Aciklama { get; set; }
        public bool Status { get; set; }

        // Navigation Props
        public Soru Soru { get; set; }
    }
}
