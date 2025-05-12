namespace dershaneOtomasyonu.Forms
{
    partial class ChattingForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            flowLayoutPanel1 = new FlowLayoutPanel();
            TxtMesaj = new TextBox();
            btnGonder = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoScroll = true;
            flowLayoutPanel1.Location = new Point(26, 24);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(614, 404);
            flowLayoutPanel1.TabIndex = 3;
            // 
            // TxtMesaj
            // 
            TxtMesaj.Font = new Font("Segoe UI", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 162);
            TxtMesaj.Location = new Point(26, 447);
            TxtMesaj.Name = "TxtMesaj";
            TxtMesaj.Size = new Size(488, 33);
            TxtMesaj.TabIndex = 6;
            TxtMesaj.KeyDown += TxtMesaj_KeyDown;
            // 
            // btnGonder
            // 
            btnGonder.CustomizableEdges = customizableEdges1;
            btnGonder.DisabledState.BorderColor = Color.DarkGray;
            btnGonder.DisabledState.CustomBorderColor = Color.DarkGray;
            btnGonder.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnGonder.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnGonder.Font = new Font("Segoe UI", 9F);
            btnGonder.ForeColor = Color.White;
            btnGonder.Location = new Point(525, 447);
            btnGonder.Name = "btnGonder";
            btnGonder.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnGonder.Size = new Size(115, 33);
            btnGonder.TabIndex = 0;
            btnGonder.Text = "Gönder";
            btnGonder.Click += btnGonder_Click;
            // 
            // ChattingForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(661, 501);
            Controls.Add(btnGonder);
            Controls.Add(TxtMesaj);
            Controls.Add(flowLayoutPanel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ChattingForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ders";
            FormClosing += ChattingForm_FormClosing;
            Load += ChattingForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FlowLayoutPanel flowLayoutPanel1;
        private TextBox TxtMesaj;
        private Guna.UI2.WinForms.Guna2Button btnGonder;
    }
}