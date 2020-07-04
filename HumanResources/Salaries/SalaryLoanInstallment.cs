using HumanResources.Loans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Salaries
{
    class SalaryLoanInstallment
    {
        double reminedToPay = 0;
        double forInstallment;

        public double ForInstallment { get => forInstallment; set => forInstallment = value; }

        public double LoansRemianedToPay(int idEmployee)
        {
            LoanManager.GetLoanIsPaidToList(idEmployee, Payment.toPay);

            foreach (Loan l in LoanManager.arrayLoans)
            {
                reminedToPay += l.Amount - l.GetSumPaidInstallmentLoan();
            }
            return Math.Round(reminedToPay, 2, MidpointRounding.AwayFromZero);
        }
        public double AmountForPaidOffInstallmentInMonth(int idEmployee, DateTime date)
        {
            ForInstallment = Loan.GetSumPaidInstallmentLoanByDate(idEmployee, date);
            return Math.Round(ForInstallment, 2, MidpointRounding.AwayFromZero);
        }
    }
}
