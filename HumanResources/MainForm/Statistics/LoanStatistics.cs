using HumanResources.Loans;
using HumanResources.Salaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace HumanResources.MainForm
{
    class LoanStatistics
    {
        public static void DisplayChart(MainForm form)
        {
            DateTime data = new DateTime(form.dtpStatystykiWyborDaty.Value.Date.Year, 1, 1);
            int idEmployee = Convert.ToInt32(form.cbStatisticSelectEmployee.SelectedValue);
            double[] months = new double[12];

            //zerowanie punktów żeby przy wyświetlaniu kolejnego roku się nie dodawały
            foreach (Series s in form.chart2.Series)
                s.Points.Clear();

            LoanManager.GetLoanToList(idEmployee);

            //dodaje do paska postępu
            MainForm.progressLoading += 8;

            foreach (Loan l in LoanManager.arrayLoans)
            {
                //jezeli data jest mniejsza od wybranej to przypisuje kwote
                //pożyczki od tego miesiąca w góre
                if (l.Date.Year == data.Year)
                    AddLoanAmount(months, l.Date.Month - 1, l.Amount);

                //jeżeli kredyt był w poprzednim roku to pwisuje kwote kredytu na casły rok
                if (l.Date.Year < data.Year)
                    AddLoanAmount(months, 0, l.Amount);

                foreach (LoanInstallment rp in l.ArrayInstallmentLoan)
                {
                    if (rp.Date.Year == data.Year)
                    {
                       SubLoanInstallmentAmount(months, rp.Date.Month - 1, rp.InstallmentAmount);
                    }
                    if (rp.Date.Year < data.Year)
                    {
                        SubLoanInstallmentAmount(months, 0, rp.InstallmentAmount);
                    }
                }
            }
            // i wyświetla w grafie
            for (int i=0; i< months.Length;i++)
            {
                form.chart2.Series["pozyczki"].Points.AddXY(i+1, months[i]);
            }
            
            //nie wyświetla kwoty gdy =0.00
            for (int i = 0; i < 12; i++)
            {
                if ((int)form.chart2.Series["pozyczki"].Points[i].YValues[0] == 0)
                {
                    form.chart2.Series["pozyczki"].Points[i].Label = string.Format(" ");
                }
            }
        }
        static void AddLoanAmount(double[] tab, int startIndex, double amount)
        {
            for (int i = startIndex; i < tab.Length; i++)
            {
                tab[i] += amount;
            }
        }
        static void SubLoanInstallmentAmount(double[] tab, int startIndex, double amount)
        {
            for (int i = startIndex; i < tab.Length; i++)
            {
                tab[i] -= amount;
            }
        }
    }
}
