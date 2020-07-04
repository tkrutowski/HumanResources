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
using Logi;
using Ustawienia.DaneFirmy;
using HumanResources.Exceptions;

namespace HumanResources.EmployeesFinances.Forms
{
    public partial class AdditionNewForm : Form
    {
        public AdditionNewForm()
        {
            InitializeComponent();
            //pasek tytułowy
            this.Text = DaneFirmy.NazwaProgramu + "Rodzaj dodatku";
            //ikona
            this.Icon = Properties.Resources.logo_firmy;
        }

        //Dodaje nowy dodatek do bazy danych
        private void btnSave_Click(object sender, EventArgs e)
        {
            if ((tbAdditionName.Text.Trim() == "") || (tbAdditionName.Text == "wpisz tutaj..."))
            {
                MessageBox.Show("Proszę wpisać poprawną wartość...", "Błędne dane.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    if (tbAdditionName.TextLength > 30)
                        throw new WrongSizeStringException("Nazwa dodatku nie może być większa niż 30 znaków");
                    //dodanie rodzaju dodatku do bazy danych
                    Additions.Addition.AddAdditionType(tbAdditionName.Text, ConnectionToDB.disconnect);
                    EmployeesFinances.EmployeeFinanse.isCorrect = true;
                }
                catch (WrongSizeStringException ex2)
                {
                    MessageBox.Show(ex2.Message, "Błąd podczas wprowadzania danych do tabeli Rodzaj Dodatku.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //log
                    LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.rodzaj_dodatku, ex2.Message));
                }
                finally
                {
                    this.Close();
                }

            }
        }
    }
}
