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
    public partial class LoanInstallmentDetailsForm : Form
    {
        Loan loan;

        public LoanInstallmentDetailsForm(Loan loan)
        {
            //pasek tytułowy
            this.Text = DaneFirmy.NazwaProgramu + "Raty";
            //ikona
            this.Icon = Properties.Resources.logo_firmy;
            this.loan = loan;
            InitializeComponent();
            RefreshDgv();
            btnClose.Focus();
        }

        /// <summary>
        /// Metoda wypisujące dane z listyRat do grida
        /// </summary>
        private void RefreshDgv()
        {
            float sum = 0;
            foreach (LoanInstallment li in loan.ArrayInstallmentLoan)
            {
                dgvLoanUnstallment.Rows.Add(li.Date.ToShortDateString(), string.Format("{0:C}", li.InstallmentAmount));
                sum += li.InstallmentAmount;
            }
            dgvCount.Rows.Add(string.Format("{0:C}", sum));
            //czyszczenie wyboru
            dgvLoanUnstallment.ClearSelection();
            dgvCount.ClearSelection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Czyszczenie wyboru w gridach po ukazaniu się formuarza
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoanInstallmentDetailsForm_Shown(object sender, EventArgs e)
        {
            //czyszczenie wyboru
            dgvLoanUnstallment.ClearSelection();
            dgvCount.ClearSelection();

        }
    }
}
