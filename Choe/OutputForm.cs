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
        public OutputForm()
        {
            InitializeComponent();
        }
        InputForm inputForm;
        private void OutputForm_Load(object sender, EventArgs e)
        {
            Cannibal.Text = DATADATADATA.OutputTextDa;
        }
        private void button16_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            label2.Capture = false;
            Message m = Message.Create(base.Handle, 0xA1, new IntPtr(2), IntPtr.Zero);
            base.WndProc(ref m);
        }

        private void Mudak_Click(object sender, EventArgs e)
        {
            this.Close();
            inputForm = new InputForm();
            inputForm.Show();
        }
    }
}
