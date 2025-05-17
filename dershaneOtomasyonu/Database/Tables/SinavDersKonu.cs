namespace dershaneOtomasyonu.Database.Tables
{
    public class SinavDersKonu
    {
        public int Id { get; set; }
        public int SinavDersId { get; set; }
        public string Ad { get; set; }  // ya da Adi, hangisini tercih edersen

        public SinavDers SinavDers { get; set; }
        public ICollection<Soru> Sorular { get; set; } // Konunun soruları

    }
}
