namespace dershaneOtomasyonu.Database.Tables
{
    public class SinifSeviye
    {
        public int Id { get; set; }
        public int Seviye { get; set; }

        // Navigation
        public ICollection<Soru> Sorular { get; set; } // Sınıf Seviyesinin soruları
    }
}
