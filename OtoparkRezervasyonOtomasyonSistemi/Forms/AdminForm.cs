using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using OtoparkRezervasyonOtomasyonSistemi.DataAccess;

namespace OtoparkRezervasyonOtomasyonSistemi
{
    public partial class AdminForm : Form
    {
        // --- ANA BİLEŞENLER ---
        TabControl tabMain;
        Button btnCikis;

        // --- SEKME 1: KROKİ BİLEŞENLERİ ---
        FlowLayoutPanel flpOtopark;
        Panel pnlDetay;
        Label lblDetayBaslik, lblDetayIcerik;
        Button btnAracGiris, btnRezervasyonIptal, btnAracCikis;
        DateTimePicker dtpKrokiSorgu;
        Button btnKrokiSorgula, btnKrokiSifirla;

        // --- SEKME 2: MÜŞTERİ BİLEŞENLERİ ---
        DataGridView dgvMusteriler;
        TextBox txtMusAd, txtMusSoyad, txtMusEmail, txtMusGSM;
        Button btnMusEkle, btnMusGuncelle, btnMusSil;
        int seciliMusteriID = 0;

        // --- SEKME 3: TARİFE BİLEŞENLERİ ---
        DataGridView dgvTarifeler;
        TextBox txtTarAdi, txtTarSaat, txtTarFiyat;
        Button btnTarEkle, btnTarGuncelle, btnTarSil;
        int seciliTarifeID = 0;

        // --- SEKME 4: KUPON BİLEŞENLERİ ---
        DataGridView dgvKuponlar;
        TextBox txtKupKod, txtKupOran;
        DateTimePicker dtpKupTarih;
        CheckBox chkKupDurum;
        Button btnKupEkle, btnKupGuncelle, btnKupSil;
        int seciliKuponID = 0;

        // --- HAFIZA DEĞİŞKENLERİ ---
        int aktifRezervasyonID = 0;
        string aktifAlanAdi = "";
        decimal aktifUcret = 0;
        bool isGelecekSorgusu = false;

        public AdminForm()
        {
            InitializeComponent();
            TasarimiVeBilesenleriOlustur();
        }

        private void AdminForm_Load(object sender, EventArgs e)
        {
            OtoparkKrokisiniCiz();
            MusterileriListele();
            TarifeleriListele();
            KuponlariListele();
        }

        #region 1. ARAYÜZ (UI) OLUŞTURMA MERKEZİ
        private void TasarimiVeBilesenleriOlustur()
        {
            this.Size = new Size(1220, 880);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = ColorTranslator.FromHtml("#F4F4F9");
            Color koyuBordo = ColorTranslator.FromHtml("#4d0000");

            Label lblAnaBaslik = new Label() { Text = "OTOPARK YÖNETİM VE KOMUTA MERKEZİ", Font = new Font("Segoe UI", 18, FontStyle.Bold), ForeColor = koyuBordo, Location = new Point(20, 15), AutoSize = true };
            this.Controls.Add(lblAnaBaslik);

            tabMain = new TabControl() { Location = new Point(20, 60), Size = new Size(1160, 700), Font = new Font("Segoe UI", 11, FontStyle.Bold) };

            TabPage tabKroki = new TabPage("OTOPARK KROKİSİ") { BackColor = Color.White };
            TabPage tabMusteriler = new TabPage("MÜŞTERİ YÖNETİMİ") { BackColor = Color.White };
            TabPage tabUcretler = new TabPage("ÜCRET & TARİFELER") { BackColor = Color.White };
            TabPage tabKuponlar = new TabPage("İNDİRİM KUPONLARI") { BackColor = Color.White };

            tabMain.TabPages.Add(tabKroki); tabMain.TabPages.Add(tabMusteriler); tabMain.TabPages.Add(tabUcretler); tabMain.TabPages.Add(tabKuponlar);
            this.Controls.Add(tabMain);

            btnCikis = new Button() { Text = "GÜVENLİ ÇIKIŞ", BackColor = koyuBordo, ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Location = new Point(860, 770), Size = new Size(320, 50), Cursor = Cursors.Hand };
            btnCikis.Click += (s, e) => { new LoginForm().Show(); this.Close(); };
            this.Controls.Add(btnCikis);

            SekmeKrokiOlustur(tabKroki); SekmeMusteriOlustur(tabMusteriler); SekmeUcretlerOlustur(tabUcretler); SekmeKuponlarOlustur(tabKuponlar);
        }

        private void SekmeKrokiOlustur(TabPage tab)
        {
            Label lblSorgu = new Label() { Text = "Tarih/Saat Doluluk Sorgula:", Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(20, 15), AutoSize = true };
            dtpKrokiSorgu = new DateTimePicker() { Location = new Point(210, 12), Size = new Size(220, 27), Font = new Font("Segoe UI", 11), Format = DateTimePickerFormat.Custom, CustomFormat = "dd.MM.yyyy HH:mm" };

            btnKrokiSorgula = new Button() { Text = "KONTROL ET", BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(440, 10), Size = new Size(120, 32), Cursor = Cursors.Hand };
            btnKrokiSorgula.Click += BtnKrokiSorgula_Click;

            btnKrokiSifirla = new Button() { Text = "ANLIK DURUM", BackColor = Color.Gray, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(570, 10), Size = new Size(120, 32), Cursor = Cursors.Hand };
            btnKrokiSifirla.Click += (s, e) => { isGelecekSorgusu = false; OtoparkKrokisiniCiz(); };

            flpOtopark = new FlowLayoutPanel() { Location = new Point(20, 55), Size = new Size(780, 600), BackColor = ColorTranslator.FromHtml("#F4F4F9"), BorderStyle = BorderStyle.FixedSingle, AutoScroll = true };
            pnlDetay = new Panel() { Location = new Point(820, 55), Size = new Size(310, 600), BackColor = ColorTranslator.FromHtml("#F4F4F9"), BorderStyle = BorderStyle.FixedSingle };
            lblDetayBaslik = new Label() { Text = "PARK YERİ DETAYI", Font = new Font("Segoe UI", 14, FontStyle.Bold), ForeColor = ColorTranslator.FromHtml("#a4161a"), Location = new Point(20, 20), AutoSize = true };
            lblDetayIcerik = new Label() { Text = "Detaylarını görmek için\nkrokiden bir araca tıklayın.", Font = new Font("Segoe UI", 11, FontStyle.Regular), Location = new Point(20, 70), AutoSize = true };

            btnAracCikis = new Button() { Text = "ÇIKIŞ VE TAHSİLAT YAP", BackColor = Color.Teal, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Location = new Point(15, 450), Size = new Size(280, 55), Cursor = Cursors.Hand, Visible = false };
            btnAracCikis.Click += BtnAracCikis_Click;

            btnAracGiris = new Button() { Text = "ARAÇ GİRİŞ YAPTI", BackColor = Color.SeaGreen, ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Location = new Point(15, 450), Size = new Size(280, 45), Cursor = Cursors.Hand, Visible = false };
            btnAracGiris.Click += BtnAracGiris_Click;

            btnRezervasyonIptal = new Button() { Text = "REZERVASYONU İPTAL ET", BackColor = ColorTranslator.FromHtml("#a4161a"), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Location = new Point(15, 505), Size = new Size(280, 45), Cursor = Cursors.Hand, Visible = false };
            btnRezervasyonIptal.Click += BtnRezervasyonIptal_Click;

            pnlDetay.Controls.Add(lblDetayBaslik); pnlDetay.Controls.Add(lblDetayIcerik); pnlDetay.Controls.Add(btnAracGiris); pnlDetay.Controls.Add(btnRezervasyonIptal); pnlDetay.Controls.Add(btnAracCikis);

            tab.Controls.Add(lblSorgu); tab.Controls.Add(dtpKrokiSorgu); tab.Controls.Add(btnKrokiSorgula); tab.Controls.Add(btnKrokiSifirla);
            tab.Controls.Add(flpOtopark); tab.Controls.Add(pnlDetay);
        }

        private void SekmeMusteriOlustur(TabPage tab)
        {
            dgvMusteriler = new DataGridView() { Location = new Point(20, 20), Size = new Size(1100, 450), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, Font = new Font("Segoe UI", 10, FontStyle.Regular) };
            dgvMusteriler.CellClick += DgvMusteriler_CellClick;

            txtMusAd = new TextBox() { Location = new Point(20, 490), Size = new Size(180, 30), Font = new Font("Segoe UI", 11) };
            txtMusSoyad = new TextBox() { Location = new Point(210, 490), Size = new Size(180, 30), Font = new Font("Segoe UI", 11) };
            txtMusEmail = new TextBox() { Location = new Point(400, 490), Size = new Size(250, 30), Font = new Font("Segoe UI", 11) };
            txtMusGSM = new TextBox() { Location = new Point(660, 490), Size = new Size(180, 30), Font = new Font("Segoe UI", 11) };

            Label l1 = new Label() { Text = "Ad", Location = new Point(20, 470), AutoSize = true, Font = new Font("Segoe UI", 9) };
            Label l2 = new Label() { Text = "Soyad", Location = new Point(210, 470), AutoSize = true, Font = new Font("Segoe UI", 9) };
            Label l3 = new Label() { Text = "E-Posta", Location = new Point(400, 470), AutoSize = true, Font = new Font("Segoe UI", 9) };
            Label l4 = new Label() { Text = "GSM", Location = new Point(660, 470), AutoSize = true, Font = new Font("Segoe UI", 9) };

            btnMusEkle = new Button() { Text = "EKLE", BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(20, 530), Size = new Size(120, 35), Cursor = Cursors.Hand };
            btnMusEkle.Click += BtnMusEkle_Click;
            btnMusGuncelle = new Button() { Text = "GÜNCELLE", BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(150, 530), Size = new Size(120, 35), Cursor = Cursors.Hand };
            btnMusGuncelle.Click += BtnMusGuncelle_Click;
            btnMusSil = new Button() { Text = "SİL", BackColor = ColorTranslator.FromHtml("#a4161a"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(280, 530), Size = new Size(120, 35), Cursor = Cursors.Hand };
            btnMusSil.Click += BtnMusSil_Click;

            tab.Controls.Add(dgvMusteriler); tab.Controls.Add(txtMusAd); tab.Controls.Add(txtMusSoyad); tab.Controls.Add(txtMusEmail); tab.Controls.Add(txtMusGSM);
            tab.Controls.Add(l1); tab.Controls.Add(l2); tab.Controls.Add(l3); tab.Controls.Add(l4);
            tab.Controls.Add(btnMusEkle); tab.Controls.Add(btnMusGuncelle); tab.Controls.Add(btnMusSil);
        }

        private void SekmeUcretlerOlustur(TabPage tab)
        {
            dgvTarifeler = new DataGridView() { Location = new Point(20, 20), Size = new Size(800, 350), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, Font = new Font("Segoe UI", 11, FontStyle.Regular) };
            dgvTarifeler.CellClick += DgvTarifeler_CellClick;

            txtTarAdi = new TextBox() { Location = new Point(20, 400), Size = new Size(200, 30), Font = new Font("Segoe UI", 11) };
            txtTarSaat = new TextBox() { Location = new Point(230, 400), Size = new Size(120, 30), Font = new Font("Segoe UI", 11) };
            txtTarFiyat = new TextBox() { Location = new Point(360, 400), Size = new Size(120, 30), Font = new Font("Segoe UI", 11) };

            Label l1 = new Label() { Text = "Tarife Adı (Örn: 3 Saatlik)", Location = new Point(20, 380), AutoSize = true, Font = new Font("Segoe UI", 9) };
            Label l2 = new Label() { Text = "Saat Sınırı (Örn: 3)", Location = new Point(230, 380), AutoSize = true, Font = new Font("Segoe UI", 9) };
            Label l3 = new Label() { Text = "Fiyat (TL)", Location = new Point(360, 380), AutoSize = true, Font = new Font("Segoe UI", 9) };

            btnTarEkle = new Button() { Text = "EKLE", BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(20, 440), Size = new Size(100, 35), Cursor = Cursors.Hand };
            btnTarEkle.Click += BtnTarEkle_Click;
            btnTarGuncelle = new Button() { Text = "GÜNCELLE", BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(130, 440), Size = new Size(100, 35), Cursor = Cursors.Hand };
            btnTarGuncelle.Click += BtnTarGuncelle_Click;
            btnTarSil = new Button() { Text = "SİL", BackColor = ColorTranslator.FromHtml("#a4161a"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(240, 440), Size = new Size(100, 35), Cursor = Cursors.Hand };
            btnTarSil.Click += BtnTarSil_Click;

            tab.Controls.Add(dgvTarifeler); tab.Controls.Add(txtTarAdi); tab.Controls.Add(txtTarSaat); tab.Controls.Add(txtTarFiyat);
            tab.Controls.Add(l1); tab.Controls.Add(l2); tab.Controls.Add(l3);
            tab.Controls.Add(btnTarEkle); tab.Controls.Add(btnTarGuncelle); tab.Controls.Add(btnTarSil);
        }

        private void SekmeKuponlarOlustur(TabPage tab)
        {
            dgvKuponlar = new DataGridView() { Location = new Point(20, 20), Size = new Size(900, 350), AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, AllowUserToAddRows = false, ReadOnly = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, Font = new Font("Segoe UI", 11, FontStyle.Regular) };
            dgvKuponlar.CellClick += DgvKuponlar_CellClick;

            txtKupKod = new TextBox() { Location = new Point(20, 400), Size = new Size(180, 30), Font = new Font("Segoe UI", 11), CharacterCasing = CharacterCasing.Upper };
            txtKupOran = new TextBox() { Location = new Point(210, 400), Size = new Size(120, 30), Font = new Font("Segoe UI", 11) };
            dtpKupTarih = new DateTimePicker() { Location = new Point(340, 400), Size = new Size(160, 30), Font = new Font("Segoe UI", 11), Format = DateTimePickerFormat.Short };
            chkKupDurum = new CheckBox() { Location = new Point(520, 403), Text = "Aktif Mi?", Font = new Font("Segoe UI", 11), AutoSize = true };

            Label l1 = new Label() { Text = "Kupon Kodu (Örn: YAZ10)", Location = new Point(20, 380), AutoSize = true, Font = new Font("Segoe UI", 9) };
            Label l2 = new Label() { Text = "İndirim Oranı (%)", Location = new Point(210, 380), AutoSize = true, Font = new Font("Segoe UI", 9) };
            Label l3 = new Label() { Text = "Son Kullanma Tarihi", Location = new Point(340, 380), AutoSize = true, Font = new Font("Segoe UI", 9) };

            btnKupEkle = new Button() { Text = "EKLE", BackColor = Color.SeaGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(20, 440), Size = new Size(100, 35), Cursor = Cursors.Hand };
            btnKupEkle.Click += BtnKupEkle_Click;
            btnKupGuncelle = new Button() { Text = "GÜNCELLE", BackColor = Color.SteelBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(130, 440), Size = new Size(100, 35), Cursor = Cursors.Hand };
            btnKupGuncelle.Click += BtnKupGuncelle_Click;
            btnKupSil = new Button() { Text = "SİL", BackColor = ColorTranslator.FromHtml("#a4161a"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(240, 440), Size = new Size(100, 35), Cursor = Cursors.Hand };
            btnKupSil.Click += BtnKupSil_Click;

            tab.Controls.Add(dgvKuponlar); tab.Controls.Add(txtKupKod); tab.Controls.Add(txtKupOran); tab.Controls.Add(dtpKupTarih); tab.Controls.Add(chkKupDurum);
            tab.Controls.Add(l1); tab.Controls.Add(l2); tab.Controls.Add(l3);
            tab.Controls.Add(btnKupEkle); tab.Controls.Add(btnKupGuncelle); tab.Controls.Add(btnKupSil);
        }
        #endregion

        #region 2. OTOPARK KROKİSİ VE SORGULAMA METOTLARI
        private void BtnKrokiSorgula_Click(object sender, EventArgs e)
        {
            isGelecekSorgusu = true; DateTime sorguTarihi = dtpKrokiSorgu.Value; OtoparkKrokisiniCiz(sorguTarihi);
        }

        private void OtoparkKrokisiniCiz(DateTime? sorguTarihi = null)
        {
            flpOtopark.Controls.Clear();
            btnAracGiris.Visible = false; btnRezervasyonIptal.Visible = false; btnAracCikis.Visible = false;

            if (sorguTarihi.HasValue) lblDetayIcerik.Text = $"\nGELECEK TARİH SORGUSU AKTİF\n\nSorgulanan Tarih: {sorguTarihi.Value:dd.MM.yyyy HH:mm}";
            else lblDetayIcerik.Text = "Detaylarını görmek için\nkrokiden bir araca tıklayın.";

            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    string query = ""; SqlCommand cmd = new SqlCommand(); cmd.Connection = conn;

                    if (sorguTarihi.HasValue)
                    {
                        query = @"SELECT P.AlanAdi, CASE WHEN EXISTS (SELECT 1 FROM Rezervasyon R WHERE R.AlanID = P.AlanID AND R.IslemDurumu != 'İptal' AND R.IslemDurumu != 'Tamamlandı' AND @tarih BETWEEN R.BaslangicZamani AND R.BitisZamani) THEN 'Rezerve' ELSE 'Boş' END AS Durum FROM Park_Alanlari P";
                        cmd.CommandText = query; cmd.Parameters.AddWithValue("@tarih", sorguTarihi.Value);
                    }
                    else
                    {
                        query = "SELECT AlanAdi, Durum FROM Park_Alanlari"; cmd.CommandText = query;
                    }

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        string alanAdi = reader["AlanAdi"].ToString(); string durum = reader["Durum"].ToString().Trim();
                        Button btnPark = new Button() { Width = 85, Height = 120, Text = alanAdi, TextAlign = ContentAlignment.TopCenter, Padding = new Padding(0, 5, 0, 0), Font = new Font("Segoe UI", 13, FontStyle.Bold), FlatStyle = FlatStyle.Flat, BackgroundImageLayout = ImageLayout.Zoom, Margin = new Padding(10), Tag = durum };
                        btnPark.FlatAppearance.BorderSize = 0;
                        if (durum == "Boş") { btnPark.BackgroundImage = Properties.Resources.durum_bos; btnPark.ForeColor = Color.Black; btnPark.Cursor = Cursors.Hand; }
                        else if (durum == "Rezerve") { btnPark.BackgroundImage = Properties.Resources.durum_rezerve; btnPark.ForeColor = Color.White; btnPark.Cursor = Cursors.Hand; }
                        else if (durum == "Dolu") { btnPark.BackgroundImage = Properties.Resources.durum_dolu; btnPark.ForeColor = Color.White; btnPark.Cursor = Cursors.Hand; }
                        btnPark.Click += ParkYeri_Click; flpOtopark.Controls.Add(btnPark);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void ParkYeri_Click(object sender, EventArgs e)
        {
            Button tiklananButon = (Button)sender; aktifAlanAdi = tiklananButon.Text; string durum = tiklananButon.Tag.ToString();
            lblDetayBaslik.Text = $"PARK YERİ: {aktifAlanAdi}";
            btnAracGiris.Visible = false; btnRezervasyonIptal.Visible = false; btnAracCikis.Visible = false;
            aktifRezervasyonID = 0; aktifUcret = 0;

            if (durum == "Boş") { lblDetayIcerik.Text = "\nBu alan şu an MÜSAİT.\n\nHerhangi bir araç bulunmuyor."; return; }
            if (isGelecekSorgusu) { lblDetayIcerik.Text = $"\nGELECEK TARİH SORGUSU\n\nSeçtiğiniz tarihte ({dtpKrokiSorgu.Value:dd.MM.yyyy HH:mm}) bu alanda aktif bir rezervasyon çakışması bulunmaktadır."; return; }

            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    string query = @"SELECT TOP 1 R.RezervasyonID, K.Ad + ' ' + K.Soyad AS MusteriAd, A.Plaka, R.BaslangicZamani, R.BitisZamani FROM Rezervasyon R INNER JOIN Park_Alanlari P ON R.AlanID = P.AlanID INNER JOIN Kullanici_Arac A ON R.AracID = A.AracID INNER JOIN Kullanicilar K ON R.KullaniciID = K.KullaniciID WHERE P.AlanAdi = @alanAdi AND (P.Durum = 'Dolu' OR P.Durum = 'Rezerve') ORDER BY R.RezervasyonID DESC";
                    SqlCommand cmd = new SqlCommand(query, conn); cmd.Parameters.AddWithValue("@alanAdi", aktifAlanAdi);
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        aktifRezervasyonID = Convert.ToInt32(reader["RezervasyonID"]); string musteri = reader["MusteriAd"].ToString(); string plaka = reader["Plaka"].ToString();
                        DateTime baslangic = reader["BaslangicZamani"] != DBNull.Value ? Convert.ToDateTime(reader["BaslangicZamani"]) : DateTime.Now;
                        if (durum == "Rezerve")
                        {
                            DateTime bitis = reader["BitisZamani"] != DBNull.Value ? Convert.ToDateTime(reader["BitisZamani"]) : baslangic.AddHours(1);
                            lblDetayIcerik.Text = $"\nDURUM: REZERVE\n\nMüşteri: {musteri}\nPlaka: {plaka}\n\nBaşlangıç: {baslangic:dd.MM.yyyy HH:mm}\nBitiş: {bitis:dd.MM.yyyy HH:mm}\n\nAraç henüz giriş yapmadı.";
                            btnAracGiris.Visible = true; btnRezervasyonIptal.Visible = true;
                        }
                        else if (durum == "Dolu")
                        {
                            TimeSpan iceridekiSure = DateTime.Now - baslangic; int saat = (int)iceridekiSure.TotalHours; int dk = iceridekiSure.Minutes;
                            aktifUcret = AnlikUcretHesapla(saat);
                            lblDetayIcerik.Text = $"\nDURUM: DOLU\n\nMüşteri: {musteri}\nPlaka: {plaka}\n\nGiriş: {baslangic:dd.MM.yyyy HH:mm}\nGeçen Süre: {saat} Saat {dk} Dk\n\nGüncel Tutar: {aktifUcret} TL";
                            btnAracCikis.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnAracGiris_Click(object sender, EventArgs e)
        {
            if (aktifRezervasyonID == 0) return;
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    new SqlCommand($"UPDATE Park_Alanlari SET Durum = 'Dolu' WHERE AlanAdi = '{aktifAlanAdi}'", conn).ExecuteNonQuery();
                    new SqlCommand($"UPDATE Rezervasyon SET BaslangicZamani = GETDATE() WHERE RezervasyonID = {aktifRezervasyonID}", conn).ExecuteNonQuery();
                    MessageBox.Show("Araç girişi onaylandı."); OtoparkKrokisiniCiz();
                }
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnRezervasyonIptal_Click(object sender, EventArgs e)
        {
            if (aktifRezervasyonID == 0) return;
            if (MessageBox.Show("İptal edilsin mi?", "İptal", MessageBoxButtons.YesNo) == DialogResult.No) return;
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    new SqlCommand($"UPDATE Park_Alanlari SET Durum = 'Boş' WHERE AlanAdi = '{aktifAlanAdi}'", conn).ExecuteNonQuery();
                    new SqlCommand($"UPDATE Rezervasyon SET IslemDurumu = 'İptal' WHERE RezervasyonID = {aktifRezervasyonID}", conn).ExecuteNonQuery();
                    MessageBox.Show("İptal edildi."); OtoparkKrokisiniCiz();
                }
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private void BtnAracCikis_Click(object sender, EventArgs e)
        {
            if (aktifRezervasyonID == 0) return;
            if (MessageBox.Show($"{aktifUcret} TL tahsilatı onaylıyor musunuz?", "Çıkış", MessageBoxButtons.YesNo) == DialogResult.No) return;
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    new SqlCommand($"UPDATE Park_Alanlari SET Durum = 'Boş' WHERE AlanAdi = '{aktifAlanAdi}'", conn).ExecuteNonQuery();
                    new SqlCommand($"UPDATE Rezervasyon SET IslemDurumu = 'Tamamlandı', BitisZamani = GETDATE(), ToplamUcret = {aktifUcret.ToString().Replace(",", ".")} WHERE RezervasyonID = {aktifRezervasyonID}", conn).ExecuteNonQuery();
                    MessageBox.Show("Tahsilat ve çıkış yapıldı."); OtoparkKrokisiniCiz();
                }
            }
            catch (Exception ex) { MessageBox.Show("Hata: " + ex.Message); }
        }

        private decimal AnlikUcretHesapla(int toplamSaat)
        {
            decimal ucret = 0; if (toplamSaat == 0) toplamSaat = 1;
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlDataReader reader = new SqlCommand("SELECT SureSaat, Fiyat FROM Otopark_Tarife ORDER BY SureSaat ASC", conn).ExecuteReader();
                    while (reader.Read()) { if (toplamSaat <= Convert.ToInt32(reader["SureSaat"])) { ucret = Convert.ToDecimal(reader["Fiyat"]); break; } }
                    return ucret == 0 ? 350 : ucret;
                }
            }
            catch (Exception ex) { MessageBox.Show("Ücret Hesaplama Hatası: " + ex.Message); return 10; }
        }
        #endregion

        #region 5. MÜŞTERİ CRUD İŞLEMLERİ
        private void MusterileriListele()
        {
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT KullaniciID, Ad, Soyad, Email, GSM FROM Kullanicilar WHERE RolID = 2", conn);
                    DataTable dt = new DataTable(); da.Fill(dt);
                    dgvMusteriler.DataSource = dt;
                    if (dgvMusteriler.Columns.Contains("KullaniciID")) dgvMusteriler.Columns["KullaniciID"].Visible = false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Müşteri listeleme hatası: " + ex.Message); }
        }

        private void DgvMusteriler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                seciliMusteriID = Convert.ToInt32(dgvMusteriler.Rows[e.RowIndex].Cells["KullaniciID"].Value);
                txtMusAd.Text = dgvMusteriler.Rows[e.RowIndex].Cells["Ad"].Value.ToString();
                txtMusSoyad.Text = dgvMusteriler.Rows[e.RowIndex].Cells["Soyad"].Value.ToString();
                txtMusEmail.Text = dgvMusteriler.Rows[e.RowIndex].Cells["Email"].Value.ToString();
                txtMusGSM.Text = dgvMusteriler.Rows[e.RowIndex].Cells["GSM"].Value.ToString();
            }
        }

        private void BtnMusEkle_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Kullanicilar (Ad, Soyad, Email, GSM, Password, RolID) VALUES (@ad, @soyad, @email, @gsm, '123456', 2)", conn);
                    cmd.Parameters.AddWithValue("@ad", txtMusAd.Text); cmd.Parameters.AddWithValue("@soyad", txtMusSoyad.Text);
                    cmd.Parameters.AddWithValue("@email", txtMusEmail.Text); cmd.Parameters.AddWithValue("@gsm", txtMusGSM.Text);
                    cmd.ExecuteNonQuery(); MessageBox.Show("Müşteri eklendi. (Varsayılan şifre: 123456)"); MusterileriListele();
                }
            }
            catch (Exception ex) { MessageBox.Show("Müşteri Ekleme Hatası: " + ex.Message); }
        }

        private void BtnMusGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliMusteriID == 0) { MessageBox.Show("Lütfen güncellenecek müşteriyi seçin!"); return; }
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE Kullanicilar SET Ad=@ad, Soyad=@soyad, Email=@email, GSM=@gsm WHERE KullaniciID=@id", conn);
                    cmd.Parameters.AddWithValue("@ad", txtMusAd.Text); cmd.Parameters.AddWithValue("@soyad", txtMusSoyad.Text);
                    cmd.Parameters.AddWithValue("@email", txtMusEmail.Text); cmd.Parameters.AddWithValue("@gsm", txtMusGSM.Text);
                    cmd.Parameters.AddWithValue("@id", seciliMusteriID);
                    cmd.ExecuteNonQuery(); MessageBox.Show("Güncellendi."); MusterileriListele();
                }
            }
            catch (Exception ex) { MessageBox.Show("Müşteri Güncelleme Hatası: " + ex.Message); }
        }

        private void BtnMusSil_Click(object sender, EventArgs e)
        {
            if (seciliMusteriID == 0)
            {
                MessageBox.Show("Lütfen silinecek müşteriyi tablodan seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Patrondan kesin onay alıyoruz çünkü bu işlem çok derin
            DialogResult onay = MessageBox.Show("DİKKAT! Bu müşteriyi silmek, müşteriye ait TÜM ARAÇLARI ve GEÇMİŞ REZERVASYONLARI da kalıcı olarak silecektir.\n\nEğer müşterinin şu an otoparkta aracı varsa, işgal ettiği alan da boşaltılacaktır.\n\nKabul ediyor musunuz?", "Kritik Silme İşlemi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (onay == DialogResult.No) return;

            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();

                    // 1. Önce aktif rezervasyonu varsa otopark alanlarını 'Boş' yapalım ki kroki kilitli kalmasın
                    SqlCommand cmdAlanBosalt = new SqlCommand("UPDATE Park_Alanlari SET Durum = 'Boş' FROM Park_Alanlari P INNER JOIN Rezervasyon R ON P.AlanID = R.AlanID WHERE R.KullaniciID = @id AND (P.Durum = 'Rezerve' OR P.Durum = 'Dolu')", conn);
                    cmdAlanBosalt.Parameters.AddWithValue("@id", seciliMusteriID);
                    cmdAlanBosalt.ExecuteNonQuery();

                    // 2. Müşteriye ait tüm rezervasyon geçmişini sil (Foreign Key bağlarını kopar)
                    SqlCommand cmdRezSil = new SqlCommand("DELETE FROM Rezervasyon WHERE KullaniciID = @id", conn);
                    cmdRezSil.Parameters.AddWithValue("@id", seciliMusteriID);
                    cmdRezSil.ExecuteNonQuery();

                    // 3. Müşteriye ait tüm araçları sil (İşte hatayı veren tablo buydu!)
                    SqlCommand cmdAracSil = new SqlCommand("DELETE FROM Kullanici_Arac WHERE KullaniciID = @id", conn);
                    cmdAracSil.Parameters.AddWithValue("@id", seciliMusteriID);
                    cmdAracSil.ExecuteNonQuery();

                    // 4. Artık müşteriyi hiçbir şey tutmuyor, güvenle silebiliriz!
                    SqlCommand cmdMusSil = new SqlCommand("DELETE FROM Kullanicilar WHERE KullaniciID = @id", conn);
                    cmdMusSil.Parameters.AddWithValue("@id", seciliMusteriID);
                    cmdMusSil.ExecuteNonQuery();

                    MessageBox.Show("Müşteri ve müşteriye ait tüm veriler sistemden tamamen silindi.", "İşlem Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Ekranı temizle
                    MusterileriListele();
                    OtoparkKrokisiniCiz(); // Alan boşalmış olabilir, krokiyi yenile

                    seciliMusteriID = 0;
                    txtMusAd.Text = ""; txtMusSoyad.Text = ""; txtMusEmail.Text = ""; txtMusGSM.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Silme işlemi sırasında beklenmeyen bir hata oluştu:\n" + ex.Message, "Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region 6. TARİFE CRUD İŞLEMLERİ
        private void TarifeleriListele()
        {
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT TarifeID, TarifeAdi, SureSaat, Fiyat FROM Otopark_Tarife", conn);
                    DataTable dt = new DataTable(); da.Fill(dt); dgvTarifeler.DataSource = dt;
                    if (dgvTarifeler.Columns.Contains("TarifeID")) dgvTarifeler.Columns["TarifeID"].Visible = false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Tarife listeleme hatası: " + ex.Message); }
        }

        private void DgvTarifeler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                seciliTarifeID = Convert.ToInt32(dgvTarifeler.Rows[e.RowIndex].Cells["TarifeID"].Value);
                txtTarAdi.Text = dgvTarifeler.Rows[e.RowIndex].Cells["TarifeAdi"].Value.ToString();
                txtTarSaat.Text = dgvTarifeler.Rows[e.RowIndex].Cells["SureSaat"].Value.ToString();
                txtTarFiyat.Text = dgvTarifeler.Rows[e.RowIndex].Cells["Fiyat"].Value.ToString();
            }
        }

        private void BtnTarEkle_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO Otopark_Tarife (TarifeAdi, SureSaat, Fiyat) VALUES (@ad, @saat, @fiyat)", conn);
                    cmd.Parameters.AddWithValue("@ad", txtTarAdi.Text); cmd.Parameters.AddWithValue("@saat", Convert.ToInt32(txtTarSaat.Text));
                    cmd.Parameters.AddWithValue("@fiyat", Convert.ToDecimal(txtTarFiyat.Text));
                    cmd.ExecuteNonQuery(); MessageBox.Show("Tarife eklendi."); TarifeleriListele();
                }
            }
            catch (Exception ex) { MessageBox.Show("Sayısal değerleri doğru giriniz!\nDetay: " + ex.Message); }
        }

        private void BtnTarGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliTarifeID == 0) { MessageBox.Show("Lütfen güncellenecek tarifeyi seçin!"); return; }
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE Otopark_Tarife SET TarifeAdi=@ad, SureSaat=@saat, Fiyat=@fiyat WHERE TarifeID=@id", conn);
                    cmd.Parameters.AddWithValue("@ad", txtTarAdi.Text); cmd.Parameters.AddWithValue("@saat", Convert.ToInt32(txtTarSaat.Text));
                    cmd.Parameters.AddWithValue("@fiyat", Convert.ToDecimal(txtTarFiyat.Text)); cmd.Parameters.AddWithValue("@id", seciliTarifeID);
                    cmd.ExecuteNonQuery(); MessageBox.Show("Tarife güncellendi."); TarifeleriListele();
                }
            }
            catch (Exception ex) { MessageBox.Show("Tarife Güncelleme Hatası: " + ex.Message); }
        }

        private void BtnTarSil_Click(object sender, EventArgs e)
        {
            if (seciliTarifeID == 0) return;
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    new SqlCommand($"DELETE FROM Otopark_Tarife WHERE TarifeID={seciliTarifeID}", conn).ExecuteNonQuery();
                    MessageBox.Show("Tarife silindi."); TarifeleriListele(); seciliTarifeID = 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("Tarife Silme Hatası: " + ex.Message); }
        }
        #endregion

        #region 7. KUPON CRUD İŞLEMLERİ (FLOAT & BIT)
        private void KuponlariListele()
        {
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter("SELECT KuponID, KuponKodu, IndirimOrani, SonKullanmaTarihi, Durum FROM IndirimKuponlari", conn);
                    DataTable dt = new DataTable(); da.Fill(dt); dgvKuponlar.DataSource = dt;
                    if (dgvKuponlar.Columns.Contains("KuponID")) dgvKuponlar.Columns["KuponID"].Visible = false;
                }
            }
            catch (Exception ex) { MessageBox.Show("Kupon listeleme hatası: " + ex.Message); }
        }

        private void DgvKuponlar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                seciliKuponID = Convert.ToInt32(dgvKuponlar.Rows[e.RowIndex].Cells["KuponID"].Value);
                txtKupKod.Text = dgvKuponlar.Rows[e.RowIndex].Cells["KuponKodu"].Value.ToString();
                txtKupOran.Text = dgvKuponlar.Rows[e.RowIndex].Cells["IndirimOrani"].Value.ToString();
                dtpKupTarih.Value = Convert.ToDateTime(dgvKuponlar.Rows[e.RowIndex].Cells["SonKullanmaTarihi"].Value);
                chkKupDurum.Checked = Convert.ToBoolean(dgvKuponlar.Rows[e.RowIndex].Cells["Durum"].Value);
            }
        }

        private void BtnKupEkle_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO IndirimKuponlari (KuponKodu, IndirimOrani, SonKullanmaTarihi, Durum) VALUES (@kod, @oran, @tarih, @durum)", conn);
                    cmd.Parameters.AddWithValue("@kod", txtKupKod.Text); cmd.Parameters.AddWithValue("@oran", Convert.ToDouble(txtKupOran.Text));
                    cmd.Parameters.AddWithValue("@tarih", dtpKupTarih.Value); cmd.Parameters.AddWithValue("@durum", chkKupDurum.Checked);
                    cmd.ExecuteNonQuery(); MessageBox.Show("Kupon eklendi."); KuponlariListele();
                }
            }
            catch (Exception ex) { MessageBox.Show("Kupon Ekleme Hatası: Lütfen değerleri doğru formatta girin.\nDetay: " + ex.Message); }
        }

        private void BtnKupGuncelle_Click(object sender, EventArgs e)
        {
            if (seciliKuponID == 0) { MessageBox.Show("Lütfen güncellenecek kuponu seçin!"); return; }
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    SqlCommand cmd = new SqlCommand("UPDATE IndirimKuponlari SET KuponKodu=@kod, IndirimOrani=@oran, SonKullanmaTarihi=@tarih, Durum=@durum WHERE KuponID=@id", conn);
                    cmd.Parameters.AddWithValue("@kod", txtKupKod.Text); cmd.Parameters.AddWithValue("@oran", Convert.ToDouble(txtKupOran.Text));
                    cmd.Parameters.AddWithValue("@tarih", dtpKupTarih.Value); cmd.Parameters.AddWithValue("@durum", chkKupDurum.Checked);
                    cmd.Parameters.AddWithValue("@id", seciliKuponID);
                    cmd.ExecuteNonQuery(); MessageBox.Show("Kupon güncellendi."); KuponlariListele();
                }
            }
            catch (Exception ex) { MessageBox.Show("Kupon Güncelleme Hatası: " + ex.Message); }
        }

        private void BtnKupSil_Click(object sender, EventArgs e)
        {
            if (seciliKuponID == 0) return;
            try
            {
                using (SqlConnection conn = new DbConnection().GetConnection())
                {
                    if (conn.State == ConnectionState.Closed) conn.Open();
                    new SqlCommand($"DELETE FROM IndirimKuponlari WHERE KuponID={seciliKuponID}", conn).ExecuteNonQuery();
                    MessageBox.Show("Kupon silindi."); KuponlariListele(); seciliKuponID = 0;
                }
            }
            catch (Exception ex) { MessageBox.Show("Kupon Silme Hatası: " + ex.Message); }
        }
        #endregion
    }
}