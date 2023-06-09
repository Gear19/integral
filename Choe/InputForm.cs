﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace Choe
{
    public partial class InputForm : Form
    {
        OutputForm outputForm;
        Translator translator;
        Registration registration;
        List<Product> products;

        public InputForm()
        {
            InitializeComponent();
            translator = new Translator();
            Versia.Text = "Версия: "+DATADATADATA.version;
        }
        bool sidebarExpand = true;
        int poison = 562;
        private void vidvigatel_Click(object sender, EventArgs e)
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

        private void InputTextBox_TextChanged(object sender, EventArgs e)
        {
            Size size = TextRenderer.MeasureText(InputTextBox.Text, InputTextBox.Font);
            InputTextBox.Width = size.Width;
            InputTextBox.Height = size.Height;
        }


        private void label2_MouseDown(object sender, MouseEventArgs e)
        {
            label2.Capture = false;
            Message m = Message.Create(base.Handle, 0xA1, new IntPtr(2), IntPtr.Zero);
            base.WndProc(ref m);
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void reshit_Click(object sender, EventArgs e)
        {
            string newIntegral = InputTextBox.Text;
            DATADATADATA.OutputTextDa = InputTextBox.Text;

            if (translator.CheckSyntax(newIntegral))
            {
                string F = translator.Integral(newIntegral, "x");
                DATADATADATA.OutputTextNet = F;
            }
            else
            {
                DATADATADATA.OutputTextNet = "Wrong Syntax";
            }

            this.Hide();
            outputForm = new OutputForm("s");
            outputForm.Show();
        }


        //bool Achee = true;
        private void Ploti_Click(object sender, EventArgs e)
        {
            //PlotiZaKnopki.Start();
        }

        bool acheKuda = true;
        private void Ache_Click(object sender, EventArgs e)
        {
            if(acheKuda == true)
            {
                Ache.BackColor = Color.FromArgb(0, 0, 0, 0);
                Ache.Location = new Point(1051, 613);
                PanelPodpiski.Location = new Point(1051, 613);
            }
            else
            {
                Ache.Location = new Point(1051, 613);
                menuTimer.Start();
            }
            //PlotiZaKnopki.Start();
        }



        //int x = 0;
        //private void PlotiZaKnopki_Tick(object sender, EventArgs e)
        //{
        //    if (Achee == true)
        //    {
        //        x += 5;
        //        Ache.BackColor = Color.FromArgb(x, 0, 0, 0);
        //        if (x==200)
        //        {
        //            Achee = false;
        //            PlotiZaKnopki.Stop();
        //        }
        //
        //    }
        //    else
        //    {
        //        x -= 5;
        //        Ache.BackColor = Color.FromArgb(x, 0, 0, 0);
        //        if (x == 0)
        //        {
        //            Achee = true;
        //            Ache.Location = new Point(1051, 613);
        //            PlotiZaKnopki.Stop();
        //        }
        //    }
        //}
        //
        private void Ploti_MouseHover(object sender, EventArgs e)
        {
            PanelSDollarom.Location = new Point(0, 562);
        }

      

        private void ButtonDollar_Click(object sender, EventArgs e)
        {
            acheKuda = true;
            Ache.Location = new Point(0, 0);
            Ache.BackColor = Color.FromArgb(200, 0, 0, 0);
            PanelSDollarom.Location = new Point(1051, 613);
            PanelPodpiski.Location = new Point(350, 206);

        }

        private void NeDollar_Click(object sender, EventArgs e)
        {
            Ache.BackColor = Color.FromArgb(0, 0, 0, 0);
            Ache.Location = new Point(1051, 613);
            PanelPodpiski.Location = new Point(1051, 613);
        }


        bool menuExpand = true;
        int slimShady = -200;
        private void menuButton_Click(object sender, EventArgs e)
        {
            acheKuda = false;
            menuTimer.Start();
            Ache.Location = new Point(0, 27);
            Ache.BackColor = Color.FromArgb(200, 0, 0, 0);
            
        }

        private void menuTimer_Tick(object sender, EventArgs e)
        {
            if (menuExpand == true)
            {

                slimShady += 10;
                MenuPanel.Location = new Point(slimShady, 27);
                if (slimShady == 0)
                {
                    menuExpand = false;
                    menuTimer.Stop();
                }

            }
            else
            {
                slimShady -= 10;
                MenuPanel.Location = new Point(slimShady, 27);
                if (slimShady == -200)
                {
                    menuExpand = true;
                    menuTimer.Stop();
                }

            }
        }

        private void NazadButton_Click(object sender, EventArgs e)
        {
            menuTimer.Start();
            Ache.BackColor = Color.FromArgb(0, 0, 0, 0);
            Ache.Location = new Point(1051, 613);
        }

        private void RegButton_Click(object sender, EventArgs e)
        {
            registration = new Registration();
            registration.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RefreshVersion();

            string newVersion = products[products.Count-1].Version;
            bool Obnoba = DATADATADATA.version != newVersion;

            if (Obnoba == true)
            {
                Versia.Text = "Версия: " + newVersion;
                DATADATADATA.version = newVersion;
                MessageBox.Show(
                "Обновление завершено",
                "Обновление",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
            }
            else
            {
                MessageBox.Show(
                "У вас уже установлены\nвсе обновления",
                "Обновление",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
            }
        }

       


        //239487432234324

        struct Product
        {
            public string Id;
            public string Version;
        }
        private void RefreshVersion()
        {
            NpgsqlConnection conn = new NpgsqlConnection("Server=194.169.163.175;Port=5432;Database=okdatabase;User Id=vilya;Password=danger");
            conn.Open();
            NpgsqlCommand comm = new NpgsqlCommand("select * from versions", conn);
            NpgsqlDataReader reader = comm.ExecuteReader();
            if (reader.HasRows)
            {
                products = new List<Product>();
                DataTable dt = new DataTable();
                dt.Load(reader);
                foreach (DataRow row in dt.Rows)
                {
                    Product newP = new Product();
                    string Id = row["Id"].ToString();
                    string Version = row["Version"].ToString();
                    newP.Id = Id;
                    newP.Version = Version;
                    products.Add(newP);
                }
            }
            comm.Dispose();
            conn.Close();
        }
    }
}
