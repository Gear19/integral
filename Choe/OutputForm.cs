using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Choe
{
    public partial class OutputForm : Form
    {
        string l;
        public OutputForm(string a)
        {
            InitializeComponent();
            l = a;
        }
        

        InputForm inputForm;
        private void OutputForm_Load(object sender, EventArgs e)
        {
            da.Text = DATADATADATA.OutputTextDa;
            otvet.Text = DATADATADATA.OutputTextNet;
            Size size = TextRenderer.MeasureText(da.Text, da.Font);
            Nigger.Location = new Point(97 + size.Width, 93);
        }
        private void button16_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            label2.Capture = false;
            Message m = Message.Create(base.Handle, 0xA1, new IntPtr(2), IntPtr.Zero);
            base.WndProc(ref m);
        }

        private void Krug_Click(object sender, EventArgs e)
        {
            this.Hide();
            inputForm = new InputForm();
            inputForm.Show();
        }
    }
}
