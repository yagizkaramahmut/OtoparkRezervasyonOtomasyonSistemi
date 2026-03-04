namespace OtoparkRezervasyonOtomasyonSistemi
{
    partial class LoginForm
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
            lblEmail = new Label();
            lblSifre = new Label();
            txtEmail = new TextBox();
            txtSifre = new TextBox();
            btnGiris = new Button();
            lblKayitOl = new Label();
            SuspendLayout();
            // 
            // lblEmail
            // 
            lblEmail.Location = new Point(0, 0);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(100, 23);
            lblEmail.TabIndex = 0;
            // 
            // lblSifre
            // 
            lblSifre.Location = new Point(0, 0);
            lblSifre.Name = "lblSifre";
            lblSifre.Size = new Size(100, 23);
            lblSifre.TabIndex = 1;
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(0, 0);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(100, 23);
            txtEmail.TabIndex = 2;
            // 
            // txtSifre
            // 
            txtSifre.Location = new Point(0, 0);
            txtSifre.Name = "txtSifre";
            txtSifre.Size = new Size(100, 23);
            txtSifre.TabIndex = 3;
            // 
            // btnGiris
            // 
            btnGiris.Location = new Point(0, 0);
            btnGiris.Name = "btnGiris";
            btnGiris.Size = new Size(75, 23);
            btnGiris.TabIndex = 4;
            // 
            // lblKayitOl
            // 
            lblKayitOl.Location = new Point(0, 0);
            lblKayitOl.Name = "lblKayitOl";
            lblKayitOl.Size = new Size(100, 23);
            lblKayitOl.TabIndex = 5;
            // 
            // LoginForm
            // 
            ClientSize = new Size(284, 261);
            Controls.Add(lblEmail);
            Controls.Add(lblSifre);
            Controls.Add(txtEmail);
            Controls.Add(txtSifre);
            Controls.Add(btnGiris);
            Controls.Add(lblKayitOl);
            Name = "LoginForm";
            Text = "Otopark Otomasyonu - Giriş";
            Load += LoginForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion

        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblSifre;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtSifre;
        private System.Windows.Forms.Button btnGiris;
        private System.Windows.Forms.Label lblKayitOl;
    }
}