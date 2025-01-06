using dershaneOtomasyonu.Database.Tables;
using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace dershaneOtomasyonu.Forms
{
    public partial class MessageCard : UserControl
    {
        public MessageCard(Kullanici kullanici, string message, DateTime date, bool isOwnMessage)
        {
            InitializeComponent();
            label2.Text = $"{kullanici.Adi} {kullanici.Soyadi}";
            label1.Text = message;
            label3.Text = date.ToString("HH:mm");
            if (isOwnMessage)
            {
                this.BackColor = Color.FromArgb(156, 173, 199);
            }
        }

    }
}
