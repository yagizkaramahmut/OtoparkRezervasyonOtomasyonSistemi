namespace OtoparkRezervasyonOtomasyonSistemi.Models
{
    public class Kullanici
    {
        public int KullaniciID { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string GSM { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

        public int RolID { get; set; }

        public Rol KullaniciRolu { get; set; }
    }
}