namespace dershaneOtomasyonu.Database.Tables
{
    public class SinavDers
    {
        public int Id { get; set; }
        public string Adi { get; set; }
        public int SinavKategoriId { get; set; }
        public int SoruSayisi { get; set; }
        public int DersId { get; set; } // Foreign Key olarak kullanılacak

        // Navigation Props
        public SinavKategori SinavKategori { get; set; }
        public ICollection<SinavDersKonu> SinavDersKonulari { get; set; } // sinavın derslerinin konuları
        public Ders Ders { get; set; }  // Navigation Property

    }
}
