using dershaneOtomasyonu.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dershaneOtomasyonu.Database.Tables
{
    public class LogEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string IpAddress { get; set; }
        public int? KullaniciId { get; set; }
        public Kullanici Kullanici { get; set; }
    }
}
