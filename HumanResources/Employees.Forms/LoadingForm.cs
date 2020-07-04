using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HumanResources.Employees.Forms
{
    public partial class LoadingForm : Form
    {
        
        public LoadingForm()
        {
            InitializeComponent();

            timer1.Enabled = true;
        }

        public void WyswietlaniePasekPostepu(int wartosc)
        {
            if (wartosc > 99)
            {
                wartosc = 100;
                this.Close();
            }

            pbLoading.Value = wartosc;
            lblProcent.Text = string.Format("{0} %", wartosc);
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            WyswietlaniePasekPostepu(MainForm.MainForm.progressLoading);
        }
    }
}
