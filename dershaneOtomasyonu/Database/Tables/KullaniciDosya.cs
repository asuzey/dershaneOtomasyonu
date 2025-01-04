namespace dershaneOtomasyonu.Database.Tables
{
    public class KullaniciDosya
    {
        public int DosyaId { get; set; }
        public int KullaniciId { get; set; }

        // Navigation Properties
        public Dosya Dosya { get; set; } // Kullanıcı ilişkisi
        public Kullanici Kullanici { get; set; } // Ders Kayıt ilişkisi
    }

}
