namespace dershaneOtomasyonu.Database.Tables
{
    public class SinavDers
    {
        public int Id { get; set; }
        public string Adi { get; set; }
        public int SinavKategoriId { get; set; }
        public int SoruSayisi { get; set; }

        // Navigation Props
        public SinavKategori SinavKategori { get; set; }
        public ICollection<SinavDersKonu> SinavDersKonulari { get; set; } // sinavın derslerinin konuları

    }
}
