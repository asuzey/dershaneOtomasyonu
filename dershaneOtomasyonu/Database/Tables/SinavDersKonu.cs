﻿namespace dershaneOtomasyonu.Database.Tables
{
    public class SinavDersKonu
    {
        public int Id { get; set; }
        public int SinavDersId { get; set; }

        // Navigation Props
        public SinavDers SinavDers { get; set; }
        public ICollection<Soru> Sorular { get; set; } // Konunun soruları

    }
}
