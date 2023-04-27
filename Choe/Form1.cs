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
    public partial class Form1 : Form
    {
        bool sidebarExpand = true;
        int poison = 562;
        public Form1()
        {
            InitializeComponent();
        }
        private void pidor_Click(object sender, EventArgs e)
        {
            sidebarTimer.Start();
        }

        private void sidebarTimer_Tick(object sender, EventArgs e)
        {
            
            if (sidebarExpand == true)
            {
                poison -= 5;
                sidebarContainer.Location = new Point(0, poison);
                if (poison == 512)
                {
                    sidebarExpand = false;
                    sidebarTimer.Stop();
                }
                
            }
            else
            {
                poison += 5;
                sidebarContainer.Location = new Point(0, poison);
                if (poison == 562)
                {
                    sidebarExpand = true;
                    sidebarTimer.Stop();
                }

            }
            
        }


        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            label2.Capture = false;
            Message m = Message.Create(base.Handle, 0xA1, new IntPtr(2), IntPtr.Zero);
            base.WndProc(ref m);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pidor_Click_1(object sender, EventArgs e)
        {
            sidebarTimer.Start();

        }

        
    }
}
