using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Konfiguracja;
using Ustawienia.DaneFirmy;
using Ustawienia.Uzytkownicy;
using Logi;
using System.Collections;
using Ustawienia.Konfiguracja;
using HumanResources.Settings;

namespace Pracownicy.Settings.Forms
{
    public partial class SettingsForm : Form
    {
        Blokady blokady = new Blokady();
        Uzytkownik user = new Uzytkownik();
        LogErr logErr = new LogErr();

        //
        //zmienne
        //
        Column kolumna = new Column();
        SetEmployee setEmployee = new SetEmployee();
        SetLoan setLoan = new SetLoan();
        List<Column> listaTemp = new List<Column>();
        ArrayList lista = new ArrayList();

        bool isChangeEmployee = false;
        bool isChangeLoan = false;

        /// <summary>
        /// Konstruktor
        /// </summary>
        public SettingsForm()
        {
            InitializeComponent();

            //pasek tytułowy
            this.Text = DaneFirmy.NazwaProgramu + "Ustawienia";

            //blokowanie przycisku zatwierdz
            btnSave.Enabled = false;

            //wypełnienie zakładek
            FillEmployeeSettings();
            FillLoanSettings();

        }

        /// <summary>
        /// Wypełnianie zakładki pracownik
        /// </summary>
        private void FillLoanSettings()
        {
            //pobieranie ustawień
            setEmployee.GetSettings();

            //pobieranie listy ustawienia pocztkowe
            SetEmployee.InitialSettings();

            //usuwanie kolumn systemowych żeby nie były wyświetlane w kolumny dostepne
            //systemowe to zaczynajce się na __
            listaTemp.Clear();
            foreach (Column k in SetEmployee.listColumns)
            {
                if (k.Name.Contains("__"))
                {
                    listaTemp.Add(k);
                }
            }
            //usunięcie z listy 
            foreach (Column k in listaTemp)
            {
                SetEmployee.listColumns.Remove(k);
            }

            //
            //sortowanie
            //            
            cbEmployeeSortColumn.DataSource = SetEmployee.listColumns;
            cbEmployeeSortColumn.DisplayMember = "Description";
            cbEmployeeSortColumn.ValueMember = "Index";

            cbEmployeeSortColumn.SelectedValue = setEmployee.SortColumnIndex;

            if (setEmployee.SortTypeAscDesc == SortType.ascending)
                cbEmployeeSortType.SelectedIndex = 1;
            else
                cbEmployeeSortType.SelectedIndex = 0;

            //
            //opcje wyświetlania
            //
            switch (setEmployee.OptionDisplay)
            {
                case DisplayOptions.hired:
                    rbPracZatrudnieni.Checked = true;
                    break;
                case DisplayOptions.realesed:
                    rbPracZwolnieni.Checked = true;
                    break;
                case DisplayOptions.all:
                    rbPracWszystkie.Checked = true;
                    break;
                default:
                    break;
            }

            //dodanie eventów
            this.cbEmployeeSortColumn.SelectedIndexChanged += new System.EventHandler(this.changeEmployee);
            this.cbEmployeeSortType.SelectedIndexChanged += new System.EventHandler(this.changeEmployee);
            this.rbPracZatrudnieni.CheckedChanged += new System.EventHandler(this.changeEmployee);
            this.rbPracZwolnieni.CheckedChanged += new System.EventHandler(this.changeEmployee);
            this.rbPracWszystkie.CheckedChanged += new System.EventHandler(this.changeEmployee);
        }

        /// <summary>
        /// Wypełnianie zakładki pożyczka
        /// </summary>
        private void FillEmployeeSettings()
        {
            //pobieranie ustawień
            setLoan.GetSettings();

            //pobieranie listy ustawienia pocztkowe
            SetLoan.InitialSettings();

            //usuwanie kolumn systemowych żeby nie były wyświetlane w kolumny dostepne
            //systemowe to zaczynajce się na __
            listaTemp.Clear();
            foreach (Column k in SetLoan.listColumn)
            {
                if (k.Name.Contains("__"))
                {
                    listaTemp.Add(k);
                }
            }
            //usunięcie z listy 
            foreach (Column k in listaTemp)
            {
                SetLoan.listColumn.Remove(k);
            }

            //
            //sortowanie
            //            
            cbLoansSortColumn.DataSource = SetLoan.listColumn;
            cbLoansSortColumn.DisplayMember = "Description";
            cbLoansSortColumn.ValueMember = "Index";

            cbLoansSortColumn.SelectedValue = setLoan.SortColumnIndex;

            if (setLoan.SortTypeAscDesc == SortType.ascending)
                cbLoansSotyType.SelectedIndex = 1;
            else
                cbLoansSotyType.SelectedIndex = 0;

            //
            //opcje wyświetlania
            //
            switch (setLoan.OptionDisplay)
            {
                case DisplayOptions.paid:
                    rbLoanPaid.Checked = true;
                    break;
                case DisplayOptions.notPaid:
                    rbLoanNotPaid.Checked = true;
                    break;
                case DisplayOptions.all:
                    rbLoanAll.Checked = true;
                    break;
                default:
                    break;
            }

            //dodanie eventów
            this.cbLoansSortColumn.SelectedIndexChanged += new System.EventHandler(this.changeLoan);
            this.cbLoansSotyType.SelectedIndexChanged += new System.EventHandler(this.changeLoan);
            this.rbLoanNotPaid.CheckedChanged += new System.EventHandler(this.changeLoan);
            this.rbLoanPaid.CheckedChanged += new System.EventHandler(this.changeLoan);
            this.rbLoanAll.CheckedChanged += new System.EventHandler(this.changeLoan);
        }

        /// <summary>
        /// Jezeli były jakies zmiany w zakladce pracownik to wl przycisk do zapisu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeEmployee(object sender, EventArgs e)
        {
            isChangeEmployee = true;
            btnSave.Enabled = true;
        }

        /// <summary>
        /// Jezeli były jakies zmiany w zakladce pracownik to wl przycisk do zapisu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changeLoan(object sender, EventArgs e)
        {
            isChangeLoan = true;
            btnSave.Enabled = true;
        }
       
        /// <summary>
        /// Zapisuje zmiany w bazie danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //jeżeli była zmiana ustawień pracownik
            if (isChangeEmployee)
            {
                //
                //sortowanie
                //                    
                if (cbEmployeeSortColumn.SelectedValue != null)
                    setEmployee.SortColumnIndex = (int)cbEmployeeSortColumn.SelectedValue;
                setEmployee.SortTypeAscDesc = (SortType)Enum.Parse(typeof(SortType),cbEmployeeSortType.SelectedIndex.ToString());

                //
                //opcje wyświetlania
                //
                if (rbPracZatrudnieni.Checked)
                    setEmployee.OptionDisplay = DisplayOptions.hired;
                if (rbPracZwolnieni.Checked)
                    setEmployee.OptionDisplay = DisplayOptions.realesed;
                if (rbPracWszystkie.Checked)
                    setEmployee.OptionDisplay = DisplayOptions.all;

                //zapisanie do bazy
                setEmployee.SaveSettings(setEmployee,ConnectionToDB.disconnect);
            }

            //jeżeli była zmiana ustawień pracownik
            if (isChangeLoan)
            {
                //
                //sortowanie
                //                    
                if (cbLoansSortColumn.SelectedValue != null)
                    setLoan.SortColumnIndex = (int)cbLoansSortColumn.SelectedValue;
                setLoan.SortTypeAscDesc = (SortType)Enum.Parse(typeof(SortType),cbLoansSotyType.SelectedIndex.ToString());

                //
                //opcje wyświetlania
                //
                if (rbLoanNotPaid.Checked)
                    setLoan.OptionDisplay = DisplayOptions.notPaid;
                if (rbLoanPaid.Checked)
                    setLoan.OptionDisplay = DisplayOptions.paid;
                if (rbLoanAll.Checked)
                    setLoan.OptionDisplay = DisplayOptions.all;

                //zapisanie do bazy
                setLoan.SaveSettings(setLoan,ConnectionToDB.disconnect);
            }
                //wył przycisku zatwierdz
                btnSave.Enabled = false;
            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            //zamykanie formularza
            this.Close();
        }

        /// <summary>
        /// Podczas zamykania formularza pyta czy zapisać zmiany
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                DialogResult result = MessageBox.Show("Czy chcesz zapisać zmiany?", "Zapisywanie danych", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
                //jezeli zapisac zmiany
                if (result == DialogResult.Yes)
                {
                    btnSave_Click(sender, e);
                }
                if (result == DialogResult.No)
                {
                    //btnZapisz.Enabled = false;
                    this.Close();
                }
                if (result == DialogResult.Cancel)
                {
                    //anuluje zamknięcie
                    e.Cancel = true;
                    return;
                }
            }
        }

        /// <summary>
        /// Zmienia text przycisku anuluj
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_EnabledChanged(object sender, EventArgs e)
        {
            if (btnSave.Enabled)
                btnAnuluj.Text = "&Anuluj";
            else
                btnAnuluj.Text = "&Wyjście";
        }
    }
}
