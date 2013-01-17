﻿using ApplicationLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace Rodoslovno_stablo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void izlazToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            splitContainer1.VerticalScroll.Enabled = !splitContainer1.VerticalScroll.Enabled;

        }

        private void printToolStripButton_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {

        }

        private void otvoriKonzoluToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConsoleForm consoleForm = new ConsoleForm();
            consoleForm.Show();

        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            RefreshTree();


        }

        private void SaveToJpeg(string path)
        {
            Panel myPanel = splitContainer1.Panel1;

            Bitmap image = new Bitmap(myPanel.Width, myPanel.Height);

            myPanel.DrawToBitmap(image, new Rectangle(0, 0, image.Width, image.Height));
            image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private void SaveToJpeg(Stream file)
        {
            Panel myPanel = splitContainer1.Panel1;

            Bitmap image = new Bitmap(myPanel.Width, myPanel.Height);

            myPanel.DrawToBitmap(image, new Rectangle(0, 0, image.Width, image.Height));
            image.Save(file, System.Drawing.Imaging.ImageFormat.Jpeg);
        }

        private void RefreshTree()
        {
            List<PersonControl> persons = new List<PersonControl>();
            Person p = new Person(new System.Guid(), "Netko", "Netkic");
            Person p1 = new Person(new System.Guid(), "Bla", "Netkic");

            PersonControl c = new PersonControl(p, this);
            PersonControl c1 = new PersonControl(p1,this);
            persons.Add(c);
            persons.Add(c1);


            splitContainer1.Panel1.Controls.Add(c);

            splitContainer1.Panel1.Controls.Add(c1);
        }


        public void personSelected(Person p) {
            textBoxIme.Text = p.name;
            textBoxPrezime.Text = p.surname;


        }

        private void spremiKaoJpegToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stream myStream = null;

            SaveFileDialog dialog = new SaveFileDialog();

            dialog.AddExtension = true;
            dialog.DefaultExt = "jpeg";
            dialog.Filter = "jpeg files (*.jpeg)|*.jpeg";
            dialog.FilterIndex = 0;
            //dialog.FileOk += dialog_FileOk;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                myStream = dialog.OpenFile();

                if (myStream != null)
                {
                    SaveToJpeg(myStream);
                    myStream.Close();
                }
            }
        }

    }
}
