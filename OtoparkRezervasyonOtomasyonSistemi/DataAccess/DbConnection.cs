using System;
using System.Data.SqlClient; // SQL işlemleri için kütüphane
using System.Windows.Forms;  // MessageBox kullanabilmek için

namespace OtoparkRezervasyonOtomasyonSistemi.DataAccess
{
    public class DbConnection
    {
        // SQL Server bağlantı cümlemiz (Kendi bilgisayar adına göre ufak bir ayar yapacağız)
        private string connectionString = @"Server=DESKTOP-FRD4TG8\SQLEXPRESS;Database=OtoparkRezervasyonDB;Trusted_Connection=True;";
        public SqlConnection GetConnection()
        {
            SqlConnection baglanti = new SqlConnection(connectionString);

            // ŞART 3: Exception Handling (Try-Catch yapısı)
            try
            {
                baglanti.Open(); // Veri tabanı kapısını açmayı deniyoruz
            }
            catch (SqlException ex)
            {
                // Eğer SQL Server kapalıysa veya şifre yanlışsa program çökmez, bu anlamlı hata mesajı çıkar
                MessageBox.Show("Veri tabanına bağlanılamadı!\nSunucu ayarlarını kontrol ediniz.\n\nSistem Mesajı: " + ex.Message,
                                "Kritik Veritabanı Hatası",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

            return baglanti;
        }
    }
}