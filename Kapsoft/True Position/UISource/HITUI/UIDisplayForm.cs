using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HITUI
{
    public partial class UIDisplayForm : Form
    {
        public UIDisplayForm()
        {
            InitializeComponent();
        }

        public void HideOKbutton()
        {
            butOk.Hide();
        }

        public void SetText(string text)
        {
            TextControl.Text = text;
        }

        private void butOk_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
