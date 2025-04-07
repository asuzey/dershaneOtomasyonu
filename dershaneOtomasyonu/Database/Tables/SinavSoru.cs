using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class SinavSoru
    {
        public int SinavId { get; set; }
        public int SoruId { get; set; }

        // Navigation Properties
        public Sinav Sinav { get; set; } 
        public Soru Soru { get; set; } 

    }
}
