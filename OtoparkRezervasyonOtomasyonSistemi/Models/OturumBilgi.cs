using System;

namespace OtoparkRezervasyonOtomasyonSistemi
{
    // static yapıyoruz ki projenin her yerinden (tüm formlardan) ulaşılabilsin
    public static class OturumBilgi
    {
        public static int KullaniciID { get; set; }
        public static string AdSoyad { get; set; }
        public static string Email { get; set; }
        public static int RolID { get; set; }

        // Kullanıcı çıkış yaptığında hafızayı temizlemek için
        public static void Temizle()
        {
            KullaniciID = 0;
            AdSoyad = string.Empty;
            Email = string.Empty;
            RolID = 0;
        }
    }
}