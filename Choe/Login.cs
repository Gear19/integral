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
    public partial class Login : Form
    {
        Registration registration;

        public Login()
        {
            InitializeComponent();
            LoginTextBox.Text = "Логин";
            LoginTextBox.ForeColor = Color.Gray;
            PasswordTextBox1.Text = "Пароль";
            PasswordTextBox1.ForeColor = Color.Gray;
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

        private void LoginTextBox_Enter(object sender, EventArgs e)
        {
            if (LoginTextBox.Text == "Логин")
            {
                LoginTextBox.Text = "";
                LoginTextBox.ForeColor = Color.White;
            }
        }
        private void LoginTextBox_Leave(object sender, EventArgs e)
        {
            if (LoginTextBox.Text == "")
            {
                LoginTextBox.Text = "Логин";
                LoginTextBox.ForeColor = Color.Gray;
            }
        }
        private void PasswordTextBox1_Enter(object sender, EventArgs e)
        {
            if (PasswordTextBox1.Text == "Пароль")
            {
                PasswordTextBox1.Text = "";
                PasswordTextBox1.ForeColor = Color.White;
            }
        }
        private void PasswordTextBox1_Leave(object sender, EventArgs e)
        {
            if (PasswordTextBox1.Text == "")
            {
                PasswordTextBox1.Text = "Пароль";
                PasswordTextBox1.ForeColor = Color.Gray;
            }
        }

        private void LoginLink_Click(object sender, EventArgs e)
        {
            this.Hide();
            registration = new Registration();
            registration.Show();
        }
    }
}
