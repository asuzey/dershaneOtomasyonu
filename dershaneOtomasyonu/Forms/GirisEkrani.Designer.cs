namespace dershaneOtomasyonu
{
    partial class GirisEkrani
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GirisEkrani));
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            label1 = new Label();
            txtAd = new TextBox();
            txtSifre = new TextBox();
            pictureBox3 = new PictureBox();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            btnGirisYap = new Guna.UI2.WinForms.Guna2Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // txtAd
            // 
            txtAd.Cursor = Cursors.IBeam;
            resources.ApplyResources(txtAd, "txtAd");
            txtAd.Name = "txtAd";
            // 
            // txtSifre
            // 
            txtSifre.Cursor = Cursors.IBeam;
            resources.ApplyResources(txtSifre, "txtSifre");
            txtSifre.Name = "txtSifre";
            // 
            // pictureBox3
            // 
            resources.ApplyResources(pictureBox3, "pictureBox3");
            pictureBox3.Name = "pictureBox3";
            pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            resources.ApplyResources(pictureBox2, "pictureBox2");
            pictureBox2.Name = "pictureBox2";
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(pictureBox1, "pictureBox1");
            pictureBox1.Name = "pictureBox1";
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(label2, "label2");
            label2.Name = "label2";
            // 
            // btnGirisYap
            // 
            btnGirisYap.BorderRadius = 25;
            btnGirisYap.CustomizableEdges = customizableEdges5;
            btnGirisYap.DisabledState.BorderColor = Color.DarkGray;
            btnGirisYap.DisabledState.CustomBorderColor = Color.DarkGray;
            btnGirisYap.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnGirisYap.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnGirisYap.FillColor = Color.Black;
            resources.ApplyResources(btnGirisYap, "btnGirisYap");
            btnGirisYap.ForeColor = Color.White;
            btnGirisYap.Name = "btnGirisYap";
            btnGirisYap.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnGirisYap.Click += BtnGirisYap_Click;
            // 
            // GirisEkrani
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(btnGirisYap);
            Controls.Add(pictureBox2);
            Controls.Add(pictureBox3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtSifre);
            Controls.Add(pictureBox1);
            Controls.Add(txtAd);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "GirisEkrani";
            Tag = "";
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private TextBox txtAd;
        private TextBox txtSifre;
        private Label label2;
        private PictureBox pictureBox2;
        private PictureBox pictureBox1;
        private PictureBox pictureBox3;
        private Guna.UI2.WinForms.Guna2Button btnGirisYap;
    }
}
