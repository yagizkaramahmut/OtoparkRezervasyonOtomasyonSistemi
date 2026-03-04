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

        // Hangi role ait olduğunu tuttuğumuz Yabancı Anahtar (Foreign Key)
        public int RolID { get; set; }

        // ŞART 6: Sınıflar Arası İlişki (Bir kullanıcının bir rolü vardır)
        public Rol KullaniciRolu { get; set; }
    }
}