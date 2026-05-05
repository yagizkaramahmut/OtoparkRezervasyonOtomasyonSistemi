namespace OtoparkRezervasyonOtomasyonSistemi
{
    partial class RegisterForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            lblAd = new Label();
            lblSoyad = new Label();
            lblEmail = new Label();
            lblGsm = new Label();
            lblSifre = new Label();
            txtAd = new TextBox();
            txtSoyad = new TextBox();
            txtEmail = new TextBox();
            txtGsm = new TextBox();
            txtSifre = new TextBox();
            btnKayitOl = new Button();
            btnGeriDon = new Button();
            SuspendLayout();
            // 
            // lblAd
            // 
            lblAd.Location = new Point(0, 0);
            lblAd.Name = "lblAd";
            lblAd.Size = new Size(100, 23);
            lblAd.TabIndex = 0;
            // 
            // lblSoyad
            // 
            lblSoyad.Location = new Point(0, 0);
            lblSoyad.Name = "lblSoyad";
            lblSoyad.Size = new Size(100, 23);
            lblSoyad.TabIndex = 1;
            // 
            // lblEmail
            // 
            lblEmail.Location = new Point(0, 0);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(100, 23);
            lblEmail.TabIndex = 2;
            // 
            // lblGsm
            // 
            lblGsm.Location = new Point(0, 0);
            lblGsm.Name = "lblGsm";
            lblGsm.Size = new Size(100, 23);
            lblGsm.TabIndex = 3;
            // 
            // lblSifre
            // 
            lblSifre.Location = new Point(0, 0);
            lblSifre.Name = "lblSifre";
            lblSifre.Size = new Size(100, 23);
            lblSifre.TabIndex = 4;
            // 
            // txtAd
            // 
            txtAd.Location = new Point(0, 0);
            txtAd.Name = "txtAd";
            txtAd.Size = new Size(100, 23);
            txtAd.TabIndex = 5;
            // 
            // txtSoyad
            // 
            txtSoyad.Location = new Point(0, 0);
            txtSoyad.Name = "txtSoyad";
            txtSoyad.Size = new Size(100, 23);
            txtSoyad.TabIndex = 6;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(0, 0);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(100, 23);
            txtEmail.TabIndex = 7;
            // 
            // txtGsm
            // 
            txtGsm.Location = new Point(0, 0);
            txtGsm.Name = "txtGsm";
            txtGsm.Size = new Size(100, 23);
            txtGsm.TabIndex = 8;
            // 
            // txtSifre
            // 
            txtSifre.Location = new Point(0, 0);
            txtSifre.Name = "txtSifre";
            txtSifre.Size = new Size(100, 23);
            txtSifre.TabIndex = 9;
            // 
            // btnKayitOl
            // 
            btnKayitOl.Location = new Point(0, 0);
            btnKayitOl.Name = "btnKayitOl";
            btnKayitOl.Size = new Size(75, 23);
            btnKayitOl.TabIndex = 10;
            // 
            // btnGeriDon
            // 
            btnGeriDon.Location = new Point(0, 0);
            btnGeriDon.Name = "btnGeriDon";
            btnGeriDon.Size = new Size(75, 23);
            btnGeriDon.TabIndex = 11;
            // 
            // RegisterForm
            // 
            ClientSize = new Size(284, 261);
            Controls.Add(lblAd);
            Controls.Add(lblSoyad);
            Controls.Add(lblEmail);
            Controls.Add(lblGsm);
            Controls.Add(lblSifre);
            Controls.Add(txtAd);
            Controls.Add(txtSoyad);
            Controls.Add(txtEmail);
            Controls.Add(txtGsm);
            Controls.Add(txtSifre);
            Controls.Add(btnKayitOl);
            Controls.Add(btnGeriDon);
            Name = "RegisterForm";
            Text = "Otopark Otomasyonu - Kayıt Ol";
            Load += RegisterForm_Load_2;
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private System.Windows.Forms.Label lblAd;
        private System.Windows.Forms.Label lblSoyad;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblGsm; // YENİ
        private System.Windows.Forms.Label lblSifre;
        private System.Windows.Forms.TextBox txtAd;
        private System.Windows.Forms.TextBox txtSoyad;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtGsm; // YENİ
        private System.Windows.Forms.TextBox txtSifre;
        private System.Windows.Forms.Button btnKayitOl;
        private System.Windows.Forms.Button btnGeriDon;
    }
}