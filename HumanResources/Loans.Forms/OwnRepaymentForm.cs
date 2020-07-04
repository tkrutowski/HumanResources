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
using HumanResources.Loans;
using HumanResources.Exceptions;

namespace HumanResources.Loans.Forms
{
    public partial class OwnRepaymentForm : Form
    {
        Loan loan;
        LoanInstallment installment;
        public OwnRepaymentForm(Loan loan)
        {
            InitializeComponent();
            this.loan = loan;
            //pasek tytułowy
            this.Text = DaneFirmy.NazwaProgramu + "Wpłata własna";
            //ikona
            this.Icon = Properties.Resources.logo_firmy;
            lblNazwa.Text = loan.Name;
        }

        #region PROPERTYSY

        float TbKwota
        {
            set
            {
                tbKwota.Text = string.Format("{0:N}", value);
            }
        }

        #endregion

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CheckDataCorrectness();

                DataAssignment();

                if(loan.PayInstallment(installment, loan))
                    this.Close();

            }
            catch (FormatException)
            {
                MessageBox.Show("Wpisałeś niepoprawną kwotę pożyczki.", "Błędne dane..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (EmptyStringException ex1)
            {
                MessageBox.Show(ex1.Message, "Błędne dane..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ErrorException ex2)
            {
                MessageBox.Show(ex2.Message, "Błędne dane..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Błędne dane, popraw i spróbuj ponownie", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.rata_pozyczki, "PozyczkaWplataWlasnaForm.btnZapisz_Click()/n/n" + ex2.Message));
            }
        }

        private void CheckDataCorrectness()
        {
            if (tbKwota.Text == "")
                throw new EmptyStringException("Musisz wypełnić pole \"Kwota wpłaty\".");
        }

        private void DataAssignment()
        {
            installment = new LoanInstallment();
            installment.IdLoan = loan.IdLoan;
            installment.Date = dtpData.Value.Date;
            installment.InstallmentAmount = Convert.ToSingle(tbKwota.Text.Replace('.', ','));
            //oznacza że wpłata własna nie będzie odciagana od wypłaty
            installment.IsOwnRepeyment = true;
        }

        /// <summary>
        /// sprawdza poprawność wpisanej kwoty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbKwota_Leave(object sender, EventArgs e)
        {
            try
            {
                TbKwota = Convert.ToSingle(tbKwota.Text.Replace('.', ','));
            }
            catch (FormatException)
            {                
                //występuje gdy kontrolka traci focus i nie ma wpisanej żadnej wartości
            }
        }
        private void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Back))
            {
                if (e.KeyChar == ',')
                {
                    e.Handled = (tb.Text.Contains(","));
                }
                else
                    e.Handled = true;
            }
        }
    }
}
