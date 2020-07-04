using Pracownicy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Loans
{
    public class LoanInstallment
    {
        private int idLoanInstallment;
        private int idLoan;
        private float installmentAmount;
        private DateTime date;
        private bool isOwnRepeyment;

        public static bool correctLoanInstallment;

        public int IdLoanInstallment { get => idLoanInstallment; set => idLoanInstallment = value; }
        public int IdLoan { get => idLoan; set => idLoan = value; }
        public float InstallmentAmount { get => installmentAmount; set => installmentAmount = value; }
        public DateTime Date { get => date; set => date = value; }
        public bool IsOwnRepeyment { get => isOwnRepeyment; set => isOwnRepeyment = value; }

        public float GetSumInstallment(int idLoan)
        {
            string select = "select sum(kwota_raty) from rata_pozyczki where id_pozyczki = " + idLoan;
            string result = (Database.GetOneElement(select, ConnectionToDB.disconnect));
            if (String.IsNullOrWhiteSpace(result))
                return 0;
            else
                return Convert.ToSingle(result);
        }
    }
}
