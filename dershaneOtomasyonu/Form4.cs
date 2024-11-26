using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dershaneOtomasyonu
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1(); // form4e geçiş
            form1.Show(); // form4ü açıyor
            this.Hide(); // form1i gizleyecek
            form1.FormClosed += (s, args) => this.Close();
        }
    }
}
