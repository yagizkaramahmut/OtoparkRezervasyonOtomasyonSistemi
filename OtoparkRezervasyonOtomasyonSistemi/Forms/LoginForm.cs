using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient; // Veri tabaný iþlemleri için
using OtoparkRezervasyonOtomasyonSistemi.DataAccess; // DbConnection'a eriþim için

namespace OtoparkRezervasyonOtomasyonSistemi
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            TasarimiUygula(); // Özel tasarýmýmýzý giydiriyoruz
        }

        // Tasarýmcý (Designer) dosyasýndaki hatayý gideren metod
        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Ýhtiyaç halinde form açýlýþ kodlarý buraya gelir
        }

        private void TasarimiUygula()
        {
            // --- 1. FORM VE ARKA PLAN ---
            this.Size = new Size(1024, 720); // Þart: 1024x720
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Görseller (Alt çizgili isimlerle çaðýrýyoruz)
            this.BackgroundImage = Properties.Resources.autopark_login_background;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // --- 2. RENK PALETÝ VE YAZILAR (#a4161a) ---
            Color anaBordo = ColorTranslator.FromHtml("#a4161a");
            Color softGri = ColorTranslator.FromHtml("#D6D6D6");
            Color koyuBordo = ColorTranslator.FromHtml("#4d0000");

            // E-Posta Yazýsý
            lblEmail.Text = "E-Posta Adresi";
            lblEmail.ForeColor = anaBordo;
            lblEmail.BackColor = Color.Transparent;
            lblEmail.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblEmail.Location = new Point(387, 240);
            lblEmail.AutoSize = true;

            // Þifre Yazýsý
            lblSifre.Text = "Þifre";
            lblSifre.ForeColor = anaBordo;
            lblSifre.BackColor = Color.Transparent;
            lblSifre.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblSifre.Location = new Point(387, 310);
            lblSifre.AutoSize = true;

            // --- 3. KUTUCUKLAR (TextBox - #D6D6D6) ---
            txtEmail.BackColor = softGri;
            txtEmail.BorderStyle = BorderStyle.FixedSingle;
            txtEmail.Font = new Font("Segoe UI", 12);
            txtEmail.Location = new Point(387, 265);
            txtEmail.Size = new Size(250, 30);

            txtSifre.BackColor = softGri;
            txtSifre.BorderStyle = BorderStyle.FixedSingle;
            txtSifre.Font = new Font("Segoe UI", 12);
            txtSifre.Location = new Point(387, 335);
            txtSifre.Size = new Size(250, 30);
            txtSifre.PasswordChar = '*';

            // --- 4. GÝRÝÞ BUTONU (#4d0000 arka, Beyaz yazý) ---
            btnGiris.Text = "GÝRÝÞ YAP";
            btnGiris.BackColor = koyuBordo;
            btnGiris.ForeColor = Color.White;
            btnGiris.FlatStyle = FlatStyle.Flat;
            btnGiris.FlatAppearance.BorderSize = 0;
            btnGiris.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnGiris.Location = new Point(387, 390);
            btnGiris.Size = new Size(250, 45);
            btnGiris.Cursor = Cursors.Hand;

            // --- 5. KAYIT OL BAÐLANTISI ---
            lblKayitOl.Text = "Hesabýn yok mu? Kayýt ol";
            lblKayitOl.ForeColor = Color.White;
            lblKayitOl.BackColor = Color.Transparent;
            lblKayitOl.Font = new Font("Segoe UI", 10, FontStyle.Underline);
            lblKayitOl.Location = new Point(435, 450);
            lblKayitOl.AutoSize = true;
            lblKayitOl.Cursor = Cursors.Hand;

            // --- OLAY BAÐLANTILARI ---
            btnGiris.Click += btnGiris_Click; // Butonun iþlevi buraya baðlanýr
            lblKayitOl.Click += lblKayitOl_Click; // Kayýt ol týklamasý
            lblKayitOl.MouseEnter += (s, e) => { lblKayitOl.ForeColor = Color.Silver; }; // Hover efekti
            lblKayitOl.MouseLeave += (s, e) => { lblKayitOl.ForeColor = Color.White; };
        }

        private void lblKayitOl_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Kayýt olma ekranýna yönlendiriliyorsunuz!", "Bilgi");
        }

        private void btnGiris_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text.Trim();
            string sifre = txtSifre.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(sifre))
            {
                MessageBox.Show("Lütfen tüm alanlarý doldurunuz!", "Uyarý", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    // HATA GÝDERME: Baðlantý zaten açýksa tekrar açma
                    if (conn.State == System.Data.ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string query = "SELECT RolID, Ad, Soyad FROM Kullanicilar WHERE Email = @mail AND Password = @pass";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@mail", email);
                    cmd.Parameters.AddWithValue("@pass", sifre);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int rolId = Convert.ToInt32(reader["RolID"]);
                        string adSoyad = reader["Ad"].ToString() + " " + reader["Soyad"].ToString();

                        MessageBox.Show($"Hoþ geldin {adSoyad}!", "Giriþ Baþarýlý", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // ÞART 1: Rol Bazlý Yönlendirme
                        if (rolId == 1) // Admin
                        {
                            MessageBox.Show("Admin Paneli Açýlýyor...");
                            // FormAdmin admin = new FormAdmin();
                            // admin.Show();
                        }
                        else // Müþteri
                        {
                            MessageBox.Show("Müþteri Paneli Açýlýyor...");
                            // FormMusteri musteri = new FormMusteri();
                            // musteri.Show();
                        }

                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Hatalý e-posta veya þifre!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sistem Hatasý: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}