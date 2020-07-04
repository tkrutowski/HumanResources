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

namespace HumanResources.Loans.Forms
{
    public partial class ChangeAmountOrDateForm : Form
    {
        LoanInstallment loanInstallment;
        /// <summary>
        /// Konstruktor zmiany daty, kwoty raty pożyczki
        /// </summary>
        /// <param name="dod"></param>
        /// <param name="isAmountChange">czy edycja kwoty: true/false</param>
        /// <param name="co">z-zaliczka</param>
        /// <param name="isDateChange">czy edycja daty: true/false</param>
        public ChangeAmountOrDateForm(LoanInstallment li, bool isAmountChange, bool isDateChange)
        {
            InitializeComponent();        
            //pasek tytułowy
            this.Text = DaneFirmy.NazwaProgramu + "Edycja raty";
            //ikona
            this.Icon = Properties.Resources.logo_firmy;
            this.loanInstallment = li;
            if (isDateChange)
            {
                dtpDate.Enabled = true;
                dtpDate.Value = li.Date;
            }
            if (isAmountChange)
            {
                tbAmount.Enabled = true;
                TbAmountChange = li.InstallmentAmount;
            }
        }

        float TbAmountChange
        {
            set
            {
                tbAmount.Text = string.Format("{0:N}", value);
            }
        }

        /// <summary>
        /// Przycisk wprowadzania danych do bazy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtpDate.Enabled == true)
                    loanInstallment.Date = dtpDate.Value.Date;
                if (tbAmount.Enabled == true)
                {
                    loanInstallment.InstallmentAmount = Convert.ToSingle(tbAmount.Text.Replace('.', ','));
                }
                //edycja w bazie danych
                Loan.EditInstallmentLone(loanInstallment, ConnectionToDB.disconnect);
                LoanInstallment.correctLoanInstallment = true;
                //zamknięcie formularza
                this.Close();
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Błąd aktualizacji.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Błąd aktualizacji.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.rata_pozyczki, "ZmianaKwotyDatyForm.btnZatwierdz_Click()/n/n" + ex2.Message));
            }            
        }

        /// <summary>
        /// Sprawdzenie poprawności kwoty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbZmianaKwoty_Leave(object sender, EventArgs e)
        {
            TbAmountChange = Convert.ToSingle(tbAmount.Text.Replace('.', ','));
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
