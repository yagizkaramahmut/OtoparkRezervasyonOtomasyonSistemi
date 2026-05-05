using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;
using OtoparkRezervasyonOtomasyonSistemi.DataAccess;

namespace OtoparkRezervasyonOtomasyonSistemi
{
    public partial class MusteriForm : Form
    {
        // --- ANA BİLEŞENLER ---
        TabControl tabMain;
        Button btnCikis;

        // --- SEKME 1: PROFİL VE ARAÇLARIM ---
        TextBox txtProfAd, txtProfSoyad, txtProfEmail, txtProfGSM, txtProfSifre;
        Button btnProfilGuncelle;
        DataGridView dgvAraclar;
        TextBox txtPlakaIslem;
        Button btnEkle, btnGuncelle, btnSil;
        int seciliAracID = 0;

        // --- SEKME 2: REZERVASYON VE KROKİ ---
        FlowLayoutPanel flpOtopark;
        Panel pnlRezerve;
        Label lblSecilenAlan, lblFiyatGostergesi;
        TextBox txtPlakaRezervasyon, txtKuponKodu;
        DateTimePicker dtpBaslangic, dtpBitis;
        Button btnKuponUygula, btnRezerveYap;

        string seciliParkYeri = "";
        decimal hesaplananUcret = 0;
        decimal odenecekUcret = 0;
        double uygulananKuponOrani = 0;

        // --- SEKME 3: GEÇMİŞ REZERVASYONLAR ---
        DataGridView dgvGecmis;

        public MusteriForm()
        {
            InitializeComponent();
            TasarimiVeBilesenleriOlustur();
        }

        private void MusteriForm_Load(object sender, EventArgs e)
        {
            ProfilBilgileriniGetir();
            MusteriAraclariniGetir();
            OtoparkKrokisiniCiz();
            GecmisRezervasyonlariGetir();
        }

        #region 1. ARAYÜZ (UI) OLUŞTURMA
        private void TasarimiVeBilesenleriOlustur()
        {
            this.Size = new Size(1220, 880);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = ColorTranslator.FromHtml("#F4F4F9");
            Color koyuBordo = ColorTranslator.FromHtml("#4d0000");

            Label lblAnaBaslik = new Label() { Text = $"HOŞ GELDİNİZ, {OturumBilgi.AdSoyad.ToUpper()}", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = koyuBordo, Location = new Point(20, 15), AutoSize = true };
            this.Controls.Add(lblAnaBaslik);

            // --- TAB CONTROL ---
            tabMain = new TabControl() { Location = new Point(20, 60), Size = new Size(1160, 700), Font = new Font("Segoe UI", 11, FontStyle.Bold) };

            TabPage tabProfil = new TabPage("PROFİL VE ARAÇLARIM") { BackColor = Color.White };
            TabPage tabRezervasyon = new TabPage("OTOPARK KROKİSİ VE REZERVASYON") { BackColor = Color.White };
            TabPage tabGecmis = new TabPage("GEÇMİŞ REZERVASYONLARIM") { BackColor = Color.White };

            tabMain.TabPages.Add(tabProfil); tabMain.TabPages.Add(tabRezervasyon); tabMain.TabPages.Add(tabGecmis);
            this.Controls.Add(tabMain);

            btnCikis = new Button() { Text = "GÜVENLİ ÇIKIŞ YAP", BackColor = koyuBordo, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Location = new Point(860, 770), Size = new Size(320, 50), Cursor = Cursors.Hand };
            btnCikis.Click += (s, e) => { OturumBilgi.Temizle(); new LoginForm().Show(); this.Close(); };
            this.Controls.Add(btnCikis);

            SekmeProfilOlustur(tabProfil);
            SekmeRezervasyonOlustur(tabRezervasyon);
            SekmeGecmisOlustur(tabGecmis);
        }

        private void SekmeProfilOlustur(TabPage tab)
        {
            // --- PROFİL GÜNCELLEME KISMI ---
            GroupBox grpProfil = new GroupBox() { Text = "Kişisel Bilgilerim", Location = new Point(20, 20), Size = new Size(400, 450), Font = new Font("Segoe UI", 12, FontStyle.Bold) };

            Label l1 = new Label() { Text = "Ad:", Location = new Point(20, 40), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtProfAd = new TextBox() { Location = new Point(20, 65), Size = new Size(350, 30), Font = new Font("Segoe UI", 11) };

            Label l2 = new Label() { Text = "Soyad:", Location = new Point(20, 110), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtProfSoyad = new TextBox() { Location = new Point(20, 135), Size = new Size(350, 30), Font = new Font("Segoe UI", 11) };

            Label l3 = new Label() { Text = "E-Posta:", Location = new Point(20, 180), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtProfEmail = new TextBox() { Location = new Point(20, 205), Size = new Size(350, 30), Font = new Font("Segoe UI", 11) };

            Label l4 = new Label() { Text = "GSM:", Location = new Point(20, 250), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtProfGSM = new TextBox() { Location = new Point(20, 275), Size = new Size(350, 30), Font = new Font("Segoe UI", 11) };

            Label l5 = new Label() { Text = "Şifre:", Location = new Point(20, 320), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtProfSifre = new TextBox() { Location = new Point(20, 345), Size = new Size(350, 30), Font = new Font("Segoe UI", 11), PasswordChar = '*' };

            btnProfilGuncelle = new Button() { Text = "BİLGİLERİMİ KAYDET", BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(20, 395), Size = new Size(350, 40), Cursor = Cursors.Hand };
            btnProfilGuncelle.Click += BtnProfilGuncelle_Click;

            grpProfil.Controls.Add(l1); grpProfil.Controls.Add(txtProfAd); grpProfil.Controls.Add(l2); grpProfil.Controls.Add(txtProfSoyad);
            grpProfil.Controls.Add(l3); grpProfil.Controls.Add(txtProfEmail); grpProfil.Controls.Add(l4); grpProfil.Controls.Add(txtProfGSM);
            grpProfil.Controls.Add(l5); grpProfil.Controls.Add(txtProfSifre); grpProfil.Controls.Add(btnProfilGuncelle);
            tab.Controls.Add(grpProfil);

            // --- ARAÇLARIM KISMI ---
            GroupBox grpAraclar = new GroupBox() { Text = "Kayıtlı Araçlarım", Location = new Point(450, 20), Size = new Size(680, 450), Font = new Font("Segoe UI", 12, FontStyle.Bold) };

            dgvAraclar = new DataGridView() { Location = new Point(20, 40), Size = new Size(640, 250), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false, Font = new Font("Segoe UI", 11, FontStyle.Regular) };
            dgvAraclar.CellClick += DgvAraclar_CellClick;

            Label lPlaka = new Label() { Text = "Plaka İşlemi:", Location = new Point(20, 310), AutoSize = true, Font = new Font("Segoe UI", 10) };
            txtPlakaIslem = new TextBox() { Location = new Point(20, 335), Size = new Size(640, 30), Font = new Font("Segoe UI", 12), CharacterCasing = CharacterCasing.Upper, MaxLength = 15 };

            btnEkle = new Button() { Text = "YENİ ARAÇ EKLE", BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(20, 385), Size = new Size(200, 40), Cursor = Cursors.Hand };
            btnEkle.Click += BtnEkle_Click;

            btnGuncelle = new Button() { Text = "GÜNCELLE", BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(240, 385), Size = new Size(200, 40), Cursor = Cursors.Hand };
            btnGuncelle.Click += BtnGuncelle_Click;

            btnSil = new Button() { Text = "SİL", BackColor = ColorTranslator.FromHtml("#a4161a"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(460, 385), Size = new Size(200, 40), Cursor = Cursors.Hand };
            btnSil.Click += BtnSil_Click;

            grpAraclar.Controls.Add(dgvAraclar); grpAraclar.Controls.Add(lPlaka); grpAraclar.Controls.Add(txtPlakaIslem);
            grpAraclar.Controls.Add(btnEkle); grpAraclar.Controls.Add(btnGuncelle); grpAraclar.Controls.Add(btnSil);
            tab.Controls.Add(grpAraclar);
        }

        private void SekmeRezervasyonOlustur(TabPage tab)
        {
            flpOtopark = new FlowLayoutPanel() { Location = new Point(20, 20), Size = new Size(760, 620), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, AutoScroll = true };
            tab.Controls.Add(flpOtopark);

            pnlRezerve = new Panel() { Location = new Point(800, 20), Size = new Size(330, 620), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            Label lblIslemBaslik = new Label() { Text = "YENİ REZERVASYON", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#a4161a"), Location = new Point(20, 15), AutoSize = true };
            lblSecilenAlan = new Label() { Text = "Seçilen Park Yeri: Yok", Font = new Font("Segoe UI", 11), Location = new Point(20, 50), AutoSize = true };

            Label lblPlaka = new Label() { Text = "Araç Plakası:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(20, 90), AutoSize = true };
            txtPlakaRezervasyon = new TextBox() { Location = new Point(20, 115), Size = new Size(290, 30), Font = new Font("Segoe UI", 12), CharacterCasing = CharacterCasing.Upper };

            Label lblBas = new Label() { Text = "Giriş Tarihi ve Saati:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(20, 160), AutoSize = true };
            dtpBaslangic = new DateTimePicker() { Location = new Point(20, 185), Size = new Size(290, 27), Font = new Font("Segoe UI", 11), Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy HH:mm" };
            dtpBaslangic.ValueChanged += FiyatHesapla;

            Label lblBit = new Label() { Text = "Çıkış Tarihi ve Saati:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(20, 230), AutoSize = true };
            dtpBitis = new DateTimePicker() { Location = new Point(20, 255), Size = new Size(290, 27), Font = new Font("Segoe UI", 11), Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy HH:mm" };
            dtpBitis.Value = DateTime.Now.AddHours(1);
            dtpBitis.ValueChanged += FiyatHesapla;

            Label lblKupon = new Label() { Text = "İndirim Kuponu (Opsiyonel):", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(20, 300), AutoSize = true };
            txtKuponKodu = new TextBox() { Location = new Point(20, 325), Size = new Size(180, 30), Font = new Font("Segoe UI", 12), CharacterCasing = CharacterCasing.Upper };

            btnKuponUygula = new Button() { Text = "UYGULA", BackColor = Color.Teal, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(210, 324), Size = new Size(100, 32), Cursor = Cursors.Hand };
            btnKuponUygula.Click += BtnKuponUygula_Click;

            lblFiyatGostergesi = new Label() { Text = "Toplam Ücret: 0.00 TL", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = Color.Green, Location = new Point(20, 390), AutoSize = true };

            btnRezerveYap = new Button() { Text = "GÜVENLİ REZERVE ET", BackColor = ColorTranslator.FromHtml("#4d0000"), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Location = new Point(20, 450), Size = new Size(290, 50), Cursor = Cursors.Hand, Enabled = false };
            btnRezerveYap.Click += BtnRezerveYap_Click;

            pnlRezerve.Controls.Add(lblIslemBaslik); pnlRezerve.Controls.Add(lblSecilenAlan); pnlRezerve.Controls.Add(lblPlaka); pnlRezerve.Controls.Add(txtPlakaRezervasyon);
            pnlRezerve.Controls.Add(lblBas); pnlRezerve.Controls.Add(dtpBaslangic); pnlRezerve.Controls.Add(lblBit); pnlRezerve.Controls.Add(dtpBitis);
            pnlRezerve.Controls.Add(lblKupon); pnlRezerve.Controls.Add(txtKuponKodu); pnlRezerve.Controls.Add(btnKuponUygula);
            pnlRezerve.Controls.Add(lblFiyatGostergesi); pnlRezerve.Controls.Add(btnRezerveYap);
            tab.Controls.Add(pnlRezerve);
        }

        private void SekmeGecmisOlustur(TabPage tab)
        {
            Label lblBilgi = new Label() { Text = "Daha Önceki Tüm Rezervasyon İşlemleriniz:", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };
            dgvGecmis = new DataGridView() { Location = new Point(20, 60), Size = new Size(1100, 550), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false, Font = new Font("Segoe UI", 11, FontStyle.Regular) };
            tab.Controls.Add(lblBilgi); tab.Controls.Add(dgvGecmis);
        }
        #endregion

        #region 2. PROFİL VE ARAÇ İŞLEMLERİ
        private void ProfilBilgileriniGetir()
        {
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT Ad, Soyad, Email, GSM, Password FROM Kullanicilar WHERE KullaniciID = @id", conn);
                    cmd.Parameters.AddWithValue("@id", OturumBilgi.KullaniciID);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        txtProfAd.Text = reader["Ad"].ToString(); txtProfSoyad.Text = reader["Soyad"].ToString();
                        txtProfEmail.Text = reader["Email"].ToString(); txtProfGSM.Text = reader["GSM"].ToString();
                        txtProfSifre.Text = reader["Password"].ToString();
                    }
                }
            }
            catch { }
        }

        private void BtnProfilGuncelle_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE Kullanicilar SET Ad=@ad, Soyad=@soyad, Email=@email, GSM=@gsm, Password=@sifre WHERE KullaniciID=@id", conn);
                    cmd.Parameters.AddWithValue("@ad", txtProfAd.Text); cmd.Parameters.AddWithValue("@soyad", txtProfSoyad.Text);
                    cmd.Parameters.AddWithValue("@email", txtProfEmail.Text); cmd.Parameters.AddWithValue("@gsm", txtProfGSM.Text);
                    cmd.Parameters.AddWithValue("@sifre", txtProfSifre.Text); cmd.Parameters.AddWithValue("@id", OturumBilgi.KullaniciID);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Profil bilgileriniz başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    OturumBilgi.AdSoyad = txtProfAd.Text + " " + txtProfSoyad.Text;
                }
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void MusteriAraclariniGetir()
        {
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT AracID, Plaka FROM Kullanici_Arac WHERE KullaniciID = @kid", conn);
                    da.SelectCommand.Parameters.AddWithValue("@kid", OturumBilgi.KullaniciID);
                    DataTable dt = new DataTable(); da.Fill(dt);
                    dgvAraclar.DataSource = dt;
                    if (dgvAraclar.Columns.Contains("AracID")) dgvAraclar.Columns["AracID"].Visible = false;
                }
            }
            catch { }
        }

        private void DgvAraclar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                seciliAracID = Convert.ToInt32(dgvAraclar.Rows[e.RowIndex].Cells["AracID"].Value);
                string plaka = dgvAraclar.Rows[e.RowIndex].Cells["Plaka"].Value.ToString();
                txtPlakaIslem.Text = plaka; txtPlakaRezervasyon.Text = plaka;
            }
        }

        private void BtnEkle_Click(object sender, EventArgs e)
        {
            string yeniPlaka = txtPlakaIslem.Text.Trim();
            if (string.IsNullOrEmpty(yeniPlaka)) { MessageBox.Show("Plaka giriniz!"); return; }
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Kullanici_Arac WHERE Plaka = @p AND KullaniciID = @kid", conn);
                    checkCmd.Parameters.AddWithValue("@p", yeniPlaka); checkCmd.Parameters.AddWithValue("@kid", OturumBilgi.KullaniciID);
                    if ((int)checkCmd.ExecuteScalar() > 0) { MessageBox.Show("Bu plaka zaten kayıtlı!"); return; }

                    SqlCommand cmd = new SqlCommand("INSERT INTO Kullanici_Arac (KullaniciID, Plaka) VALUES (@kid, @p)", conn);
                    cmd.Parameters.AddWithValue("@kid", OturumBilgi.KullaniciID); cmd.Parameters.AddWithValue("@p", yeniPlaka);
                    cmd.ExecuteNonQuery(); MessageBox.Show("Araç eklendi."); txtPlakaIslem.Text = ""; seciliAracID = 0; MusteriAraclariniGetir();
                }
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliAracID == 0 || string.IsNullOrEmpty(txtPlakaIslem.Text)) return;
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE Kullanici_Arac SET Plaka = @p WHERE AracID = @aid", conn);
                    cmd.Parameters.AddWithValue("@p", txtPlakaIslem.Text.Trim()); cmd.Parameters.AddWithValue("@aid", seciliAracID);
                    cmd.ExecuteNonQuery(); MessageBox.Show("Güncellendi."); MusteriAraclariniGetir();
                }
            }
            catch { }
        }

        private void BtnSil_Click(object sender, EventArgs e)
        {
            if (seciliAracID == 0) return;
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Rezervasyon R INNER JOIN Park_Alanlari P ON R.AlanID = P.AlanID WHERE R.AracID = @aid AND (P.Durum = 'Rezerve' OR P.Durum = 'Dolu')", conn);
                    checkCmd.Parameters.AddWithValue("@aid", seciliAracID);
                    if ((int)checkCmd.ExecuteScalar() > 0)
                    {
                        if (MessageBox.Show("Aktif rezervasyonlu araç! Silinirse rezervasyon iptal olur. Onaylıyor musunuz?", "Uyarı", MessageBoxButtons.YesNo) == DialogResult.No) return;
                        new SqlCommand("UPDATE Park_Alanlari SET Durum = 'Boş' FROM Park_Alanlari P INNER JOIN Rezervasyon R ON P.AlanID = R.AlanID WHERE R.AracID = " + seciliAracID, conn).ExecuteNonQuery();
                    }
                    new SqlCommand("DELETE FROM Rezervasyon WHERE AracID = " + seciliAracID, conn).ExecuteNonQuery();
                    new SqlCommand("DELETE FROM Kullanici_Arac WHERE AracID = " + seciliAracID, conn).ExecuteNonQuery();
                    MessageBox.Show("Silindi."); seciliAracID = 0; txtPlakaIslem.Text = ""; txtPlakaRezervasyon.Text = ""; MusteriAraclariniGetir(); OtoparkKrokisiniCiz();
                }
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }
        #endregion

        #region 3. KROKİ, DİNAMİK FİYAT VE REZERVASYON İŞLEMLERİ
        private void OtoparkKrokisiniCiz()
        {
            flpOtopark.Controls.Clear();
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlDataReader reader = new SqlCommand("SELECT AlanAdi, Durum FROM Park_Alanlari", conn).ExecuteReader();
                    while (reader.Read())
                    {
                        string alanAdi = reader["AlanAdi"].ToString(); string durum = reader["Durum"].ToString().Trim();
                        Button btnPark = new Button() { Width = 85, Height = 120, Text = alanAdi, TextAlign = ContentAlignment.TopCenter, Padding = new Padding(0, 5, 0, 0), Font = new Font("Segoe UI", 13, FontStyle.Bold), FlatStyle = FlatStyle.Flat, BackgroundImageLayout = ImageLayout.Zoom, Margin = new Padding(10), Tag = durum };
                        btnPark.FlatAppearance.BorderSize = 0;
                        if (durum == "Boş") { btnPark.BackgroundImage = Properties.Resources.durum_bos; btnPark.ForeColor = Color.Black; btnPark.Cursor = Cursors.Hand; }
                        else if (durum == "Rezerve") { btnPark.BackgroundImage = Properties.Resources.durum_rezerve; btnPark.ForeColor = Color.White; }
                        else if (durum == "Dolu") { btnPark.BackgroundImage = Properties.Resources.durum_dolu; btnPark.ForeColor = Color.White; }
                        btnPark.Click += ParkYeri_Click; flpOtopark.Controls.Add(btnPark);
                    }
                }
            }
            catch { }
            FiyatHesapla(null, null);
        }

        private void ParkYeri_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.Tag.ToString() != "Boş") { MessageBox.Show("Lütfen MÜSAİT (Yeşil) bir alan seçiniz."); return; }
            seciliParkYeri = btn.Text;
            lblSecilenAlan.Text = $"Seçilen Park Yeri: {seciliParkYeri}";
            lblSecilenAlan.ForeColor = Color.Green;
            btnRezerveYap.Enabled = true;
        }

        // --- İŞTE O YENİ NESİL KADEMELİ FİYAT HESAPLAMA ALGORİTMASI ---
        private void FiyatHesapla(object sender, EventArgs e)
        {
            if (dtpBitis.Value <= dtpBaslangic.Value)
            {
                lblFiyatGostergesi.Text = "Hatalı Tarih Seçimi!";
                lblFiyatGostergesi.ForeColor = Color.Red;
                btnRezerveYap.Enabled = false;
                return;
            }

            TimeSpan kalisSuresi = dtpBitis.Value - dtpBaslangic.Value;
            int toplamSaat = (int)Math.Ceiling(kalisSuresi.TotalHours);

            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    // Tarifeleri büyükten küçüğe alıyoruz ki önce haftalığa/günlüğe bölebilelim
                    SqlDataAdapter da = new SqlDataAdapter("SELECT SureSaat, Fiyat FROM Otopark_Tarife ORDER BY SureSaat DESC", conn);
                    DataTable dtTarifeler = new DataTable();
                    da.Fill(dtTarifeler);

                    hesaplananUcret = 0;
                    int kalanSaat = toplamSaat;

                    if (dtTarifeler.Rows.Count > 0)
                    {
                        // Saati en büyük dilimlere parçalayarak katla
                        foreach (DataRow row in dtTarifeler.Rows)
                        {
                            int tarifeSaati = Convert.ToInt32(row["SureSaat"]);
                            decimal fiyat = Convert.ToDecimal(row["Fiyat"]);

                            if (kalanSaat >= tarifeSaati)
                            {
                                int carpan = kalanSaat / tarifeSaati;
                                hesaplananUcret += carpan * fiyat;
                                kalanSaat = kalanSaat % tarifeSaati; // Kalan saati bir alt tarifeye aktar
                            }
                        }

                        // Eğer hesap tam bölünmediyse ve elde hala 1-2 saat kaldıysa, onu en küçük tarifeyle çarp
                        if (kalanSaat > 0)
                        {
                            decimal enKucukFiyat = Convert.ToDecimal(dtTarifeler.Rows[dtTarifeler.Rows.Count - 1]["Fiyat"]);
                            hesaplananUcret += enKucukFiyat;
                        }
                    }

                    if (hesaplananUcret == 0) hesaplananUcret = 10;

                    // Kupon uygulanmışsa fiyattan düş
                    if (uygulananKuponOrani > 0)
                    {
                        odenecekUcret = hesaplananUcret - (hesaplananUcret * (decimal)(uygulananKuponOrani / 100));
                        lblFiyatGostergesi.Text = $"Toplam: {odenecekUcret:F2} TL (%{uygulananKuponOrani} İndirimli)";
                    }
                    else
                    {
                        odenecekUcret = hesaplananUcret;
                        lblFiyatGostergesi.Text = $"Toplam Ücret: {odenecekUcret:F2} TL";
                    }

                    lblFiyatGostergesi.ForeColor = Color.Green;
                    if (seciliParkYeri != "") btnRezerveYap.Enabled = true;
                }
            }
            catch { }
        }

        private void BtnKuponUygula_Click(object sender, EventArgs e)
        {
            string kod = txtKuponKodu.Text.Trim();
            if (string.IsNullOrEmpty(kod)) { MessageBox.Show("Lütfen bir kupon kodu giriniz."); return; }

            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT IndirimOrani FROM IndirimKuponlari WHERE KuponKodu = @kod AND Durum = 1 AND SonKullanmaTarihi >= GETDATE()", conn);
                    cmd.Parameters.AddWithValue("@kod", kod);
                    object sonuc = cmd.ExecuteScalar();

                    if (sonuc != null)
                    {
                        uygulananKuponOrani = Convert.ToDouble(sonuc);
                        MessageBox.Show($"Tebrikler! %{uygulananKuponOrani} indirim kuponu başarıyla uygulandı.", "Kupon Geçerli", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        FiyatHesapla(null, null);
                    }
                    else
                    {
                        MessageBox.Show("Girdiğiniz kupon kodu geçersiz, süresi dolmuş veya pasif durumda.", "Geçersiz Kupon", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        uygulananKuponOrani = 0; txtKuponKodu.Text = ""; FiyatHesapla(null, null);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Kupon Hatası: " + ex.Message); }
        }

        private void BtnRezerveYap_Click(object sender, EventArgs e)
        {
            string plaka = txtPlakaRezervasyon.Text.Trim();
            if (string.IsNullOrEmpty(seciliParkYeri) || string.IsNullOrEmpty(plaka)) { MessageBox.Show("Lütfen alan ve plaka seçiniz!"); return; }
            if (dtpBitis.Value <= dtpBaslangic.Value) { MessageBox.Show("Çıkış tarihi, giriş tarihinden önce olamaz!"); return; }

            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    int aracId = 0;
                    SqlCommand checkArac = new SqlCommand("SELECT AracID FROM Kullanici_Arac WHERE Plaka = @p AND KullaniciID = @kid", conn);
                    checkArac.Parameters.AddWithValue("@p", plaka); checkArac.Parameters.AddWithValue("@kid", OturumBilgi.KullaniciID);
                    object result = checkArac.ExecuteScalar();

                    if (result != null) { aracId = Convert.ToInt32(result); }
                    else
                    {
                        SqlCommand insertArac = new SqlCommand("INSERT INTO Kullanici_Arac (KullaniciID, Plaka) VALUES (@kid, @p); SELECT SCOPE_IDENTITY();", conn);
                        insertArac.Parameters.AddWithValue("@p", plaka); insertArac.Parameters.AddWithValue("@kid", OturumBilgi.KullaniciID);
                        aracId = Convert.ToInt32(insertArac.ExecuteScalar());
                    }

                    SqlCommand cmdAlan = new SqlCommand("SELECT AlanID FROM Park_Alanlari WHERE AlanAdi = @ad", conn);
                    cmdAlan.Parameters.AddWithValue("@ad", seciliParkYeri);
                    int alanId = Convert.ToInt32(cmdAlan.ExecuteScalar());

                    string rezerveQuery = "INSERT INTO Rezervasyon (KullaniciID, AracID, AlanID, RezervasyonTarihi, BaslangicZamani, BitisZamani, ToplamUcret, IslemDurumu) VALUES (@kid, @aid, @alan, GETDATE(), @bas, @bit, @ucret, 'Aktif')";
                    SqlCommand cmdRez = new SqlCommand(rezerveQuery, conn);
                    cmdRez.Parameters.AddWithValue("@kid", OturumBilgi.KullaniciID); cmdRez.Parameters.AddWithValue("@aid", aracId);
                    cmdRez.Parameters.AddWithValue("@alan", alanId); cmdRez.Parameters.AddWithValue("@bas", dtpBaslangic.Value);
                    cmdRez.Parameters.AddWithValue("@bit", dtpBitis.Value); cmdRez.Parameters.AddWithValue("@ucret", odenecekUcret.ToString().Replace(",", "."));
                    cmdRez.ExecuteNonQuery();

                    SqlCommand updateAlan = new SqlCommand("UPDATE Park_Alanlari SET Durum = 'Rezerve' WHERE AlanID = @alan", conn);
                    updateAlan.Parameters.AddWithValue("@alan", alanId); updateAlan.ExecuteNonQuery();

                    MessageBox.Show("Rezervasyon işlemi başarıyla tamamlandı! Mail gönderiliyor...", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ResmiMailGonder(OturumBilgi.Email, plaka, seciliParkYeri, dtpBaslangic.Value, dtpBitis.Value, odenecekUcret);

                    seciliParkYeri = ""; lblSecilenAlan.Text = "Seçilen Park Yeri: Yok"; btnRezerveYap.Enabled = false;
                    uygulananKuponOrani = 0; txtKuponKodu.Text = "";

                    OtoparkKrokisiniCiz(); MusteriAraclariniGetir(); GecmisRezervasyonlariGetir();
                    tabMain.SelectedIndex = 2;
                }
            }
            catch (Exception ex) { MessageBox.Show("Sistem Hatası: " + ex.Message); }
        }

        #endregion

        #region 4. GEÇMİŞ İŞLEMLER VE MAİL
        private void GecmisRezervasyonlariGetir()
        {
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    string query = @"SELECT P.AlanAdi AS [Park Yeri], A.Plaka, R.BaslangicZamani AS [Giriş], R.BitisZamani AS [Çıkış], R.ToplamUcret AS [Tutar (TL)], R.IslemDurumu AS [Durum] 
                                     FROM Rezervasyon R 
                                     INNER JOIN Park_Alanlari P ON R.AlanID = P.AlanID 
                                     INNER JOIN Kullanici_Arac A ON R.AracID = A.AracID 
                                     WHERE R.KullaniciID = @kid ORDER BY R.RezervasyonID DESC";
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    da.SelectCommand.Parameters.AddWithValue("@kid", OturumBilgi.KullaniciID);
                    DataTable dt = new DataTable(); da.Fill(dt); dgvGecmis.DataSource = dt;
                }
            }
            catch { }
        }

        // --- MAİL TASARIMI DÜZELTİLDİ (\n YERİNE GERÇEK ALT SATIR KULLANILDI) ---
        private void ResmiMailGonder(string aliciMail, string plaka, string alan, DateTime bas, DateTime bit, decimal ucret)
        {
            try
            {
                MailMessage mail = new MailMessage(); SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                string gondericiMail = "gelisimotoparkotomasyonu@gmail.com"; string uygulamaSifresi = "tgqs chbo auae xkzk";

                mail.From = new MailAddress(gondericiMail, "Otopark Yönetimi");
                mail.To.Add(aliciMail); mail.Subject = "Otopark Rezervasyon Bilgilendirmesi";

                // \n işaretleri yerine gerçek boşluklar bırakılarak string formatı düzeltildi
                mail.Body =
$"Sayın {OturumBilgi.AdSoyad.ToUpper()},\r\n\r\n" +
$"{plaka} plakalı aracınız için {alan} numaralı otopark alanında rezervasyon işleminiz başarıyla gerçekleştirilmiştir.\r\n\r\n" +
$"• Başlangıç Zamanı: {bas:dd.MM.yyyy HH:mm}\r\n" +
$"• Çıkış Yapılması Gereken Zaman: {bit:dd.MM.yyyy HH:mm}\r\n" +
$"• Tahsil Edilecek Tutar: {ucret:F2} TL\r\n\r\n" +
$"ÖNEMLİ NOT: Rezervasyon sürenizin bitiminde otoparkımızda kalmaya devam etmeniz halinde, aşım süreniz standart ücret tarifemiz üzerinden hesaplanarak çıkışta tahsil edilecektir.\r\n\r\n" +
$"Hayırlı konaklamalar dileriz.\r\n\r\n" +
$"Saygılarımızla,\r\n" +
$"Otopark Otomasyon Yönetimi";

                smtp.Port = 587; smtp.Credentials = new NetworkCredential(gondericiMail, uygulamaSifresi); smtp.EnableSsl = true; smtp.Send(mail);
            }
            catch { MessageBox.Show("Rezervasyon yapıldı ancak bilgilendirme maili gönderilemedi.", "Uyarı"); }
        }
        #endregion
    }
}