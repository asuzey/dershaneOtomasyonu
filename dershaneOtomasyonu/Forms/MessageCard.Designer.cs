namespace dershaneOtomasyonu.Forms
{
    partial class MessageCard
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label2 = new Label();
            label1 = new Label();
            label3 = new Label();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Transparent;
            label2.Font = new Font("Century Gothic", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label2.ForeColor = SystemColors.MenuText;
            label2.Location = new Point(-1, 1);
            label2.Name = "label2";
            label2.Size = new Size(56, 16);
            label2.TabIndex = 12;
            label2.Text = "Kimden";
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Century Gothic", 14.25F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label1.ForeColor = SystemColors.MenuText;
            label1.Location = new Point(3, 27);
            label1.Name = "label1";
            label1.Size = new Size(531, 22);
            label1.TabIndex = 13;
            label1.Text = "Mesaj";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Century Gothic", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label3.ForeColor = SystemColors.MenuText;
            label3.Location = new Point(557, 50);
            label3.Name = "label3";
            label3.Size = new Size(53, 17);
            label3.TabIndex = 14;
            label3.Text = "Zaman";
            // 
            // MessageCard
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(label3);
            Controls.Add(label1);
            Controls.Add(label2);
            Name = "MessageCard";
            Size = new Size(614, 67);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label2;
        private Label label1;
        private Label label3;
    }
}
