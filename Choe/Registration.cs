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
    public partial class Registration : Form
    {
        Login login;

        public Registration()
        {
            InitializeComponent();
            LoginTextBox.Text = "Логин";
            LoginTextBox.ForeColor = Color.Gray;
            PasswordTextBox1.Text = "Пароль";
            PasswordTextBox1.ForeColor = Color.Gray;
            PasswordTextBox2.Text = "Пароль";
            PasswordTextBox2.ForeColor = Color.Gray;
            NomerKartiTextBox.Text = "Номер карты";
            NomerKartiTextBox.ForeColor = Color.Gray;
            DataTextBox.Text = "ММ/ГГ";
            DataTextBox.ForeColor = Color.Gray;
            CVCTextBox.Text = "CVC/CVV";
            CVCTextBox.ForeColor = Color.Gray;

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

        private void LoginTextBox_Enter(object sender, EventArgs e)
        {
            if (LoginTextBox.Text == "Логин"){
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
        private void PasswordTextBox2_Enter(object sender, EventArgs e)
        {
            if (PasswordTextBox2.Text == "Пароль")
            {
                PasswordTextBox2.Text = "";
                PasswordTextBox2.ForeColor = Color.White;
            }
        }
        private void PasswordTextBox2_Leave(object sender, EventArgs e)
        {
            if (PasswordTextBox2.Text == "")
            {
                PasswordTextBox2.Text = "Пароль";
                PasswordTextBox2.ForeColor = Color.Gray;
            }
        }
        private void NomerKartiTextBox_Enter(object sender, EventArgs e)
        {
            if (NomerKartiTextBox.Text == "Номер карты")
            {
                NomerKartiTextBox.Text = "";
                NomerKartiTextBox.ForeColor = Color.White;
            }
        }
        private void NomerKartiTextBox_Leave(object sender, EventArgs e)
        {
            if (NomerKartiTextBox.Text == "")
            {
                NomerKartiTextBox.Text = "Номер карты";
                NomerKartiTextBox.ForeColor = Color.Gray;
            }
        }
        private void DataTextBox_Enter(object sender, EventArgs e)
        {
            if (DataTextBox.Text == "ММ/ГГ")
            {
                DataTextBox.Text = "";
                DataTextBox.ForeColor = Color.White;
            }
        }
        private void DataTextBox_Leave(object sender, EventArgs e)
        {
            if (DataTextBox.Text == "")
            {
                DataTextBox.Text = "ММ/ГГ";
                DataTextBox.ForeColor = Color.Gray;
            }
        }
        private void CVCTextBox_Enter(object sender, EventArgs e)
        {
            if (CVCTextBox.Text == "CVC/CVV")
            {
                CVCTextBox.Text = "";
                CVCTextBox.ForeColor = Color.White;
            }
        }

        private void CVCTextBox_Leave(object sender, EventArgs e)
        {
            if (CVCTextBox.Text == "")
            {
                CVCTextBox.Text = "CVC/CVV";
                CVCTextBox.ForeColor = Color.Gray;
            }
        }

        private void LoginLink_Click(object sender, EventArgs e)
        {
            this.Hide();
            login = new Login();
            login.Show();
        }
    }
}
