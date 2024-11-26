namespace dershaneOtomasyonu
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            Bunifu.UI.WinForms.BunifuButton.BunifuButton.BorderEdges borderEdges1 = new Bunifu.UI.WinForms.BunifuButton.BunifuButton.BorderEdges();
            label1 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            bunifuGradientPanel1 = new Bunifu.UI.WinForms.BunifuGradientPanel();
            pictureBox2 = new PictureBox();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            bunifuButton1 = new Bunifu.UI.WinForms.BunifuButton.BunifuButton();
            bunifuGradientPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Name = "label1";
            // 
            // textBox1
            // 
            textBox1.Cursor = Cursors.IBeam;
            resources.ApplyResources(textBox1, "textBox1");
            textBox1.Name = "textBox1";
            // 
            // textBox2
            // 
            textBox2.Cursor = Cursors.IBeam;
            resources.ApplyResources(textBox2, "textBox2");
            textBox2.Name = "textBox2";
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // radioButton1
            // 
            resources.ApplyResources(radioButton1, "radioButton1");
            radioButton1.Name = "radioButton1";
            radioButton1.TabStop = true;
            radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            resources.ApplyResources(radioButton2, "radioButton2");
            radioButton2.Name = "radioButton2";
            radioButton2.TabStop = true;
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton3
            // 
            resources.ApplyResources(radioButton3, "radioButton3");
            radioButton3.Name = "radioButton3";
            radioButton3.TabStop = true;
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // bunifuGradientPanel1
            // 
            bunifuGradientPanel1.BackColor = Color.Transparent;
            resources.ApplyResources(bunifuGradientPanel1, "bunifuGradientPanel1");
            bunifuGradientPanel1.BorderRadius = 1;
            bunifuGradientPanel1.Controls.Add(pictureBox2);
            bunifuGradientPanel1.Controls.Add(pictureBox1);
            bunifuGradientPanel1.Controls.Add(label2);
            bunifuGradientPanel1.Controls.Add(bunifuButton1);
            bunifuGradientPanel1.Controls.Add(label1);
            bunifuGradientPanel1.Controls.Add(radioButton3);
            bunifuGradientPanel1.Controls.Add(radioButton2);
            bunifuGradientPanel1.Controls.Add(radioButton1);
            bunifuGradientPanel1.Controls.Add(textBox2);
            bunifuGradientPanel1.Controls.Add(textBox1);
            bunifuGradientPanel1.GradientBottomLeft = Color.Azure;
            bunifuGradientPanel1.GradientBottomRight = Color.LightSteelBlue;
            bunifuGradientPanel1.GradientTopLeft = Color.LightSteelBlue;
            bunifuGradientPanel1.GradientTopRight = Color.AliceBlue;
            bunifuGradientPanel1.Name = "bunifuGradientPanel1";
            bunifuGradientPanel1.Quality = 10;
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
            // bunifuButton1
            // 
            bunifuButton1.AllowAnimations = true;
            bunifuButton1.AllowMouseEffects = true;
            bunifuButton1.AllowToggling = false;
            bunifuButton1.AnimationSpeed = 200;
            bunifuButton1.AutoGenerateColors = false;
            bunifuButton1.AutoRoundBorders = false;
            bunifuButton1.AutoSizeLeftIcon = true;
            bunifuButton1.AutoSizeRightIcon = true;
            bunifuButton1.BackColor = Color.Transparent;
            bunifuButton1.BackColor1 = Color.FromArgb(51, 122, 183);
            resources.ApplyResources(bunifuButton1, "bunifuButton1");
            bunifuButton1.BorderStyle = Bunifu.UI.WinForms.BunifuButton.BunifuButton.BorderStyles.Solid;
            bunifuButton1.ButtonText = "Giriş Yap";
            bunifuButton1.ButtonTextMarginLeft = 0;
            bunifuButton1.ColorContrastOnClick = 45;
            bunifuButton1.ColorContrastOnHover = 45;
            borderEdges1.BottomLeft = true;
            borderEdges1.BottomRight = true;
            borderEdges1.TopLeft = true;
            borderEdges1.TopRight = true;
            bunifuButton1.CustomizableEdges = borderEdges1;
            bunifuButton1.DialogResult = DialogResult.None;
            bunifuButton1.DisabledBorderColor = Color.FromArgb(191, 191, 191);
            bunifuButton1.DisabledFillColor = Color.Empty;
            bunifuButton1.DisabledForecolor = Color.Empty;
            bunifuButton1.FocusState = Bunifu.UI.WinForms.BunifuButton.BunifuButton.ButtonStates.Pressed;
            bunifuButton1.ForeColor = Color.White;
            bunifuButton1.IconLeft = null;
            bunifuButton1.IconLeftAlign = ContentAlignment.MiddleLeft;
            bunifuButton1.IconLeftCursor = Cursors.Default;
            bunifuButton1.IconLeftPadding = new Padding(11, 3, 3, 3);
            bunifuButton1.IconMarginLeft = 11;
            bunifuButton1.IconPadding = 10;
            bunifuButton1.IconRight = null;
            bunifuButton1.IconRightAlign = ContentAlignment.MiddleRight;
            bunifuButton1.IconRightCursor = Cursors.Default;
            bunifuButton1.IconRightPadding = new Padding(3, 3, 7, 3);
            bunifuButton1.IconSize = 25;
            bunifuButton1.IdleBorderColor = Color.Empty;
            bunifuButton1.IdleBorderRadius = 0;
            bunifuButton1.IdleBorderThickness = 0;
            bunifuButton1.IdleFillColor = Color.Empty;
            bunifuButton1.IdleIconLeftImage = null;
            bunifuButton1.IdleIconRightImage = null;
            bunifuButton1.IndicateFocus = false;
            bunifuButton1.Name = "bunifuButton1";
            bunifuButton1.OnDisabledState.BorderColor = Color.FromArgb(191, 191, 191);
            bunifuButton1.OnDisabledState.BorderRadius = 40;
            bunifuButton1.OnDisabledState.BorderStyle = Bunifu.UI.WinForms.BunifuButton.BunifuButton.BorderStyles.Dash;
            bunifuButton1.OnDisabledState.BorderThickness = 0;
            bunifuButton1.OnDisabledState.FillColor = Color.FromArgb(204, 204, 204);
            bunifuButton1.OnDisabledState.ForeColor = Color.FromArgb(168, 160, 168);
            bunifuButton1.OnDisabledState.IconLeftImage = null;
            bunifuButton1.OnDisabledState.IconRightImage = null;
            bunifuButton1.onHoverState.BorderColor = Color.Black;
            bunifuButton1.onHoverState.BorderRadius = 40;
            bunifuButton1.onHoverState.BorderStyle = Bunifu.UI.WinForms.BunifuButton.BunifuButton.BorderStyles.Dash;
            bunifuButton1.onHoverState.BorderThickness = 0;
            bunifuButton1.onHoverState.FillColor = Color.Black;
            bunifuButton1.onHoverState.ForeColor = Color.White;
            bunifuButton1.onHoverState.IconLeftImage = null;
            bunifuButton1.onHoverState.IconRightImage = null;
            bunifuButton1.OnIdleState.BorderColor = Color.Black;
            bunifuButton1.OnIdleState.BorderRadius = 40;
            bunifuButton1.OnIdleState.BorderStyle = Bunifu.UI.WinForms.BunifuButton.BunifuButton.BorderStyles.Solid;
            bunifuButton1.OnIdleState.BorderThickness = 0;
            bunifuButton1.OnIdleState.FillColor = Color.Black;
            bunifuButton1.OnIdleState.ForeColor = Color.White;
            bunifuButton1.OnIdleState.IconLeftImage = null;
            bunifuButton1.OnIdleState.IconRightImage = null;
            bunifuButton1.OnPressedState.BorderColor = Color.Black;
            bunifuButton1.OnPressedState.BorderRadius = 40;
            bunifuButton1.OnPressedState.BorderStyle = Bunifu.UI.WinForms.BunifuButton.BunifuButton.BorderStyles.Solid;
            bunifuButton1.OnPressedState.BorderThickness = 0;
            bunifuButton1.OnPressedState.FillColor = Color.FromArgb(64, 64, 64);
            bunifuButton1.OnPressedState.ForeColor = Color.White;
            bunifuButton1.OnPressedState.IconLeftImage = null;
            bunifuButton1.OnPressedState.IconRightImage = null;
            bunifuButton1.TextAlign = ContentAlignment.MiddleCenter;
            bunifuButton1.TextAlignment = HorizontalAlignment.Center;
            bunifuButton1.TextMarginLeft = 0;
            bunifuButton1.TextPadding = new Padding(1);
            bunifuButton1.UseDefaultRadiusAndThickness = true;
            bunifuButton1.Click += bunifuButton1_Click;
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(bunifuGradientPanel1);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "Form1";
            Tag = "";
            Load += Form1_Load;
            bunifuGradientPanel1.ResumeLayout(false);
            bunifuGradientPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Label label1;
        private TextBox textBox1;
        private TextBox textBox2;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private Bunifu.UI.WinForms.BunifuGradientPanel bunifuGradientPanel1;
        private Bunifu.UI.WinForms.BunifuButton.BunifuButton bunifuButton1;
        private Label label2;
        private PictureBox pictureBox2;
        private PictureBox pictureBox1;
    }
}
