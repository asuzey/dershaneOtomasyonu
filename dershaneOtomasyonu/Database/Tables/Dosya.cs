namespace dershaneOtomasyonu.Database.Tables
{
    public class Dosya
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int OlusturucuId { get; set; }

        // Navigation Properties
        public Kullanici Olusturucu { get; set; } // Ders Kayıt ilişkisi
    }

}
