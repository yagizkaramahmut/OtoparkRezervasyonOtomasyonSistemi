using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SqlClient;
using OtoparkRezervasyonOtomasyonSistemi.DataAccess;
using System.Net;
using System.Net.Mail;

namespace OtoparkRezervasyonOtomasyonSistemi
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
            TasarimiUygula();
        }

        // Tasarımcı hatasını önleyen Load metodu
        private void RegisterForm_Load(object sender, EventArgs e)
        {
        }

        private void TasarimiUygula()
        {
            // --- FORM AYARLARI ---
            this.Size = new Size(1024, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // Görselleri Login'deki gibi çekiyoruz
            this.BackgroundImage = Properties.Resources.autopark_login_background;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            // --- RENKLER ---
            Color anaBordo = ColorTranslator.FromHtml("#a4161a");
            Color softGri = ColorTranslator.FromHtml("#D6D6D6");
            Color koyuBordo = ColorTranslator.FromHtml("#4d0000");

            // --- YAZILAR (Label) ---
            // YENİ: GSM'i E-Posta ile Şifre arasına ekledik
            Label[] labels = { lblAd, lblSoyad, lblEmail, lblGsm, lblSifre };
            string[] labelTexts = { "Adınız", "Soyadınız", "E-Posta", "GSM Numarası", "Şifre" };
            int baslangicY = 120; // Form sığsın diye biraz daha yukarıdan başlattım

            for (int i = 0; i < labels.Length; i++)
            {
                labels[i].Text = labelTexts[i];
                labels[i].ForeColor = anaBordo;
                labels[i].BackColor = Color.Transparent;
                labels[i].Font = new Font("Segoe UI", 12, FontStyle.Bold);
                labels[i].Location = new Point(387, baslangicY + (i * 70));
                labels[i].AutoSize = true;
            }

            // --- KUTUCUKLAR (TextBox) ---
            // YENİ: txtGsm eklendi
            TextBox[] textBoxes = { txtAd, txtSoyad, txtEmail, txtGsm, txtSifre };

            for (int i = 0; i < textBoxes.Length; i++)
            {
                textBoxes[i].BackColor = softGri;
                textBoxes[i].BorderStyle = BorderStyle.FixedSingle;
                textBoxes[i].Font = new Font("Segoe UI", 12);
                textBoxes[i].Location = new Point(387, baslangicY + 25 + (i * 70));
                textBoxes[i].Size = new Size(250, 30);
            }

            // Sadece şifre kutusunu gizli yapıyoruz
            txtSifre.PasswordChar = '*';

            // İsteğe bağlı: GSM kutusuna sadece sayı girilmesi için uyarı yazısı mantığı da eklenebilir
            txtGsm.MaxLength = 11; // 05551234567 formatı için sınır

            // --- KAYIT OL BUTONU ---
            btnKayitOl.Text = "KAYIT OL";
            btnKayitOl.BackColor = koyuBordo;
            btnKayitOl.ForeColor = Color.White;
            btnKayitOl.FlatStyle = FlatStyle.Flat;
            btnKayitOl.FlatAppearance.BorderSize = 0;
            btnKayitOl.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            // YENİ: Araya GSM girdiği için butonu aşağı kaydırdık (440 -> 500)
            btnKayitOl.Location = new Point(387, 500);
            btnKayitOl.Size = new Size(250, 45);
            btnKayitOl.Cursor = Cursors.Hand;
            btnKayitOl.Click += btnKayitOl_Click;

            // --- GERİ DÖN BUTONU ---
            btnGeriDon.Text = "Giriş Ekranına Dön";
            btnGeriDon.BackColor = Color.Gray;
            btnGeriDon.ForeColor = Color.White;
            btnGeriDon.FlatStyle = FlatStyle.Flat;
            btnGeriDon.FlatAppearance.BorderSize = 0;
            btnGeriDon.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            // YENİ: Geri dön butonunu da aşağı kaydırdık (495 -> 555)
            btnGeriDon.Location = new Point(387, 555);
            btnGeriDon.Size = new Size(250, 35);
            btnGeriDon.Cursor = Cursors.Hand;
            btnGeriDon.Click += btnGeriDon_Click;
        }

        private void btnKayitOl_Click(object sender, EventArgs e)
        {
            string ad = txtAd.Text.Trim();
            string soyad = txtSoyad.Text.Trim();
            string email = txtEmail.Text.Trim();
            string gsm = txtGsm.Text.Trim(); // Eklediğin GSM alanı
            string sifre = txtSifre.Text.Trim();

            if (string.IsNullOrEmpty(ad) || string.IsNullOrEmpty(soyad) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(gsm) || string.IsNullOrEmpty(sifre))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == System.Data.ConnectionState.Closed) conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM Kullanicilar WHERE Email = @mail";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@mail", email);
                    int kayitSayisi = (int)checkCmd.ExecuteScalar();

                    if (kayitSayisi > 0)
                    {
                        MessageBox.Show("Bu e-posta adresi zaten kullanılıyor!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    MessageBox.Show("Doğrulama maili gönderiliyor, lütfen bekleyiniz...", "İşlem Sürüyor");
                    string uretilenKod = new Random().Next(100000, 999999).ToString();

                    bool mailGitti = MailGonder(email, uretilenKod);

                    if (mailGitti)
                    {
                        string girilenKod = OnayKoduSor($"Lütfen {email} adresine gönderilen 6 haneli kodu giriniz:", "Mail Doğrulaması");

                        if (girilenKod == uretilenKod)
                        {
                            string insertQuery = "INSERT INTO Kullanicilar (Ad, Soyad, Email, GSM, Password, RolID) VALUES (@ad, @soyad, @mail, @gsm, @pass, 2)";
                            SqlCommand cmd = new SqlCommand(insertQuery, conn);
                            cmd.Parameters.AddWithValue("@ad", ad);
                            cmd.Parameters.AddWithValue("@soyad", soyad);
                            cmd.Parameters.AddWithValue("@mail", email);
                            cmd.Parameters.AddWithValue("@gsm", gsm);
                            cmd.Parameters.AddWithValue("@pass", sifre);

                            cmd.ExecuteNonQuery();

                            MessageBox.Show("Tebrikler! Hesabınız başarıyla oluşturuldu. Giriş yapabilirsiniz.", "Kayıt Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoginForm loginForm = new LoginForm();
                            loginForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Hatalı doğrulama kodu girdiniz. Kayıt iptal edildi.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sistem Hatası: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool MailGonder(string aliciMail, string onayKodu)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");

                string gondericiMail = "gelisimotoparkotomasyonu@gmail.com";
                string uygulamaSifresi = "tgqs chbo auae xkzk";

                mail.From = new MailAddress(gondericiMail, "Otopark Otomasyonu");
                mail.To.Add(aliciMail);
                mail.Subject = "Otopark Kayıt Doğrulama Kodu";
                mail.Body = $"Muhterem müşterimiz,\n\nSisteme kayıt olabilmek için doğrulama kodunuz: {onayKodu}\n\nİyi günler dileriz.";

                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential(gondericiMail, uygulamaSifresi);
                smtp.EnableSsl = true;

                smtp.Send(mail);
                return true; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Mail gönderilemedi: " + ex.Message, "SMTP Hatası", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private string OnayKoduSor(string mesaj, string baslik)
        {
            Form prompt = new Form()
            {
                Width = 350,
                Height = 180,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = baslik,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 30, Top = 20, Text = mesaj, AutoSize = true };
            TextBox textBox = new TextBox() { Left = 30, Top = 50, Width = 270, Font = new Font("Segoe UI", 12) };
            Button confirmation = new Button() { Text = "Doğrula", Left = 200, Width = 100, Top = 90, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox); prompt.Controls.Add(confirmation); prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
        private void btnGeriDon_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Hide();
        }

        private void RegisterForm_Load_1(object sender, EventArgs e)
        {

        }

        private void RegisterForm_Load_2(object sender, EventArgs e)
        {

        }
    }
}