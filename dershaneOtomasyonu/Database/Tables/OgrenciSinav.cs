namespace dershaneOtomasyonu.Database.Tables
{
    public class OgrenciSinav
    {
        public int SinavId { get; set; }
        public int KullaniciId { get; set; }

        // Navigation Props
        public Sinav Sinav { get; set; }
        public Kullanici Kullanici { get; set; }
    }
}
