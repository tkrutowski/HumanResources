using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Konfiguracja;
using Ustawienia.DaneFirmy;
using Logi;
using System.Collections;
using WorkTimeRecords.Prints;

namespace HumanResources.Employees.Forms
{
    public partial class TimeRecordSheetPrintForm : Form
    {

        /// <summary>
        /// Konstruktor  - drukuje ewidencje dla jednego pracownika
        /// </summary>
        /// <param name="idPracownika"></param>
        public TimeRecordSheetPrintForm(int idPracownika)
        {
            InitializeComponent();
            //pasek tytułowy
            this.Text = DaneFirmy.NazwaProgramu + "Wydruk";
            //ikona
            this.Icon = Properties.Resources.logo_firmy;
            //listaPrac.Add(idPracownika);
        }

        /// <summary>
        /// Konstruktor - drukuje ewidencje czasu pracy dla wszystkich zatrudnionych pracowników
        /// </summary>
        public TimeRecordSheetPrintForm()
        {
            InitializeComponent();
            //pasek tytułowy
            this.Text = DaneFirmy.NazwaProgramu + "Wydruk";
            //ikona
            this.Icon = Properties.Resources.logo_firmy;
        }

        /// <summary>
        /// Metoda drukująca ewidencje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDrukuj_Click(object sender, EventArgs e)
        {
            string temp = string.Format("Czy napewno chcesz wydrukować plik 'Ewidencja czasu pracy' dla WSZYSTKICH zatrudnionych pracowników?");
            DialogResult result = MessageBox.Show(temp, "Wydruk", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                TimeRecordSheet timeRecordSheet = new TimeRecordSheet();
                timeRecordSheet.Print(dtpData.Value);
            }
            else
            {
                MessageBox.Show("Drukowanie anulowane.", "Anulowanie...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            //zamykanie form
            this.Close();
        }

        private void btnAnuluj_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
