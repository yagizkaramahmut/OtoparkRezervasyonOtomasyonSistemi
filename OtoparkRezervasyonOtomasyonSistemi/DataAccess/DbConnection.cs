using System;
using System.Data.SqlClient; 
using System.Windows.Forms;  

namespace OtoparkRezervasyonOtomasyonSistemi.DataAccess
{
    public class DbConnection
    {
        private string connectionString = @"Server=DESKTOP-FRD4TG8\SQLEXPRESS;Database=OtoparkRezervasyonDB;Trusted_Connection=True;";
        public SqlConnection GetConnection()
        {
            SqlConnection baglanti = new SqlConnection(connectionString);

          
            try
            {
                baglanti.Open(); 
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Veri tabanına bağlanılamadı!\nSunucu ayarlarını kontrol ediniz.\n\nSistem Mesajı: " + ex.Message,
                                "Kritik Veritabanı Hatası",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }

            return baglanti;
        }
    }
}