namespace dershaneOtomasyonu.Database.Tables
{
    public class SinavKategori
    {
        public int Id { get; set; }
        public string Adi { get; set; }
        public int VarsayilanSure { get; set; }

        // Navigation Props
        public ICollection<SinavDers> SinavDersleri { get; set; } // Kategorinin Dersleri
        public ICollection<Sinav> Sinavlar { get; set; } // Kategorinin Sınavları

    }
}
