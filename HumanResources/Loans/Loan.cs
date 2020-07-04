using HumanResources.Exceptions;
using Konfiguracja;
using Logi;
using Pracownicy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HumanResources.Loans
{
    public class Loan
    {
        private int idLoan;
        private int idEmployee;
        private float amount;
        private string name;
        private DateTime date;
        private float installmentLoan;
        private string otherInfo;
        private Payment isPaid;
        private List<LoanInstallment> arrayInstallmentLoan = new List<LoanInstallment>();

        public static bool correctLoan;

        public int IdLoan { get => idLoan; set => idLoan = value; }
        public int IdEmployee { get => idEmployee; set => idEmployee = value; }
        public float Amount { get => amount; set => amount = value; }
        public string Name { get => name; set => name = value; }
        public float InstallmentLoan { get => installmentLoan; set => installmentLoan = value; }
        public DateTime Date { get => date; set => date = value; }
        public Payment IsPaid { get => isPaid; set => isPaid = value; }
        public string OtherInfo { get => otherInfo; set => otherInfo = value; }
        internal List<LoanInstallment> ArrayInstallmentLoan
        {
            get
            {
                if(arrayInstallmentLoan.Count==0)
                    GetListInstallmentLoan();
                return arrayInstallmentLoan;
            }
            set => arrayInstallmentLoan = value;
        }


        /// <summary>
        /// Oznacza pożyczkę w bazie danych jako spłaconą lub niespłaconą
        /// </summary>
        /// <param name="isPaid">true - spłacona, false - niespłacona</param>
        /// <param name="idLoan">id pożyczki</param>
        public void SetLoanAsPaidOrNotPaid(Payment isPaid, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = "update pozyczka set czy_splacone='" + (int)Enum.Parse(typeof(Payment), isPaid.ToString()) + "' where id_pozyczki=" + idLoan;

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.pozyczka,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }
        public void PayInstallment(LoanInstallment li, ConnectionToDB disconnect)
        {
            string select = "insert into rata_pozyczki values('" +
                li.IdLoan + "','" + li.InstallmentAmount.ToString().Replace(',', '.') + "','" + li.Date.ToString("d", DateFormat.TakeDateFormat()) + "','" + li.IsOwnRepeyment + "')";
            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.dodawanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.rata_pozyczki,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public bool PayInstallment(LoanInstallment installment, Loan loan)
        {                 //sprawdzenie czy kwota raty nie przewyższa kwoty pozostałej do spłaty
            float remained = loan.Amount - installment.GetSumInstallment(loan.IdLoan);
            //zaokraglenie
            //doSplaty = Math.Round(doSplaty, 2);
            if (installment.InstallmentAmount < remained)
            {
                //dodanie wpłaty do bazy danych
                loan.PayInstallment(installment, ConnectionToDB.disconnect);
        
                return true;
            }
            //jeżeli kwota wpłaty jest większa niż kwota pozostała do spłaty to otwiera sięokno z zapytaniem...
            if (installment.InstallmentAmount > remained)
            {
                string temp = string.Format("Kwota wpłaty {0:C} przewyższa kwotę potrzebną do całkowitej spłaty pożyczki {1:C}.\n\n" +
                    "Naciśnij TAK jeżeli chcesz spłacić pożyczkę kwotą {2:C}\n\nNaciśnij NIE jeżeli chcesz wybrać inną kwotę spłaty.",
                    installment.InstallmentAmount, remained, remained);
                DialogResult result = MessageBox.Show(temp, "Wpłata raty pożyczki", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    //przyjmuje kwote wpłaty=kwota do Spłaty 
                    installment.InstallmentAmount = remained;
                    //dodanie wpłaty do bazy danych
                    loan.PayInstallment(installment, ConnectionToDB.notDisconnect);
                    //ustawienie pożyczki jako spłacona
                    loan.SetLoanAsPaidOrNotPaid(Payment.paidOff, ConnectionToDB.disconnect);
                    //informacja o spłacnie pożyczki
                    MessageBox.Show("Pożyczka została SPŁACONA", "Wpłata raty pożyczki", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                else
                    return false;
            }
            else
            //jeżeli kwota raty jest równa kwocie pozostałej do spłaty
            //oznacza pozyczke jako spłacową
            if (installment.InstallmentAmount == (float)remained)
            {
                //dodanie wpłaty do bazy danych
                loan.PayInstallment(installment, ConnectionToDB.notDisconnect);
                //ustawienie pożyczki jako spłacona
                loan.SetLoanAsPaidOrNotPaid(Payment.paidOff, ConnectionToDB.disconnect);
                //informacja o spłacie pożyczki
                MessageBox.Show("Pożyczka została SPŁACONA", "Wpłata raty pożyczki", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }

            throw new ErrorException("Nie udało się zapisać raty do bazy");
        }
        public static void EditInstallmentLone(LoanInstallment li, ConnectionToDB disconnect)
        {
            string select = "update rata_pozyczki set kwota_raty='" + li.InstallmentAmount.ToString().Replace(',', '.')
                + "',data_raty='" + li.Date.ToString("d", DateFormat.TakeDateFormat()) + "'where id_raty = '" + li.IdLoanInstallment + "'";

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.rata_pozyczki,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }
        public void DeleteAllInstallmentLoan(ConnectionToDB disconnect)
        {
            string select = "delete rata_pozyczki where id_pozyczki=" + idLoan;
            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.usuwanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.pozyczka,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }
        public static void DeleteInstallmentLoan(int idInstallment, ConnectionToDB disconnect)
        {
            string select = "delete rata_pozyczki where id_raty=" + idInstallment;
            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.usuwanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.pozyczka,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }
        /// <summary>
        /// Pobieranie rat pożyczek do listy 
        /// </summary>
        public void GetListInstallmentLoan()
        {
            string select = "select * from rata_pozyczki where id_pozyczki = " + IdLoan;

            SqlDataReader dataReader = Database.GetData(select);
            arrayInstallmentLoan.Clear();
            while (dataReader.Read())
            {
                arrayInstallmentLoan.Add(AssignInstallmentLoan(dataReader));
            }
            dataReader.Close();
        }

        public double GetSumPaidInstallmentLoan()
        {
            string select = "select sum(kwota_raty) from rata_pozyczki where id_pozyczki = " + IdLoan;

            try
            {
                return (Convert.ToDouble(Database.GetOneElement(select, ConnectionToDB.disconnect)));

            }
            catch (FormatException ex3)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.rata_pozyczki, "MainForm.btnPracownikEdytuj_Click()/n/n" + ex3.Message));
                
                return 0f;
            }
        }

        public static double GetSumPaidInstallmentLoanByDate(int idEmployee, DateTime date)
        {
            string select = "select sum(kwota_raty) from Pozyczka as p join rata_pozyczki as rp on p.id_pozyczki=rp.id_pozyczki where id_pracownika=" +
                idEmployee + " AND datepart(year,data_raty)=" + date.Year + " AND datepart(month,data_raty)=" + date.Month + "AND wplata_wlasna='false'";

            try
            {
                return (Convert.ToDouble(Database.GetOneElement(select, ConnectionToDB.disconnect)));

            }
            catch (FormatException ex3)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.rata_pozyczki, "MainForm.btnPracownikEdytuj_Click()/n/n" + ex3.Message));

                return 0f;
            }
        }

        public static  LoanInstallment GetInstallmentLoan(int idInstallmentLoan)
        {
            string select = "select * from rata_pozyczki where id_raty = " + idInstallmentLoan;

            SqlDataReader dataReader = Database.GetData(select);
            LoanInstallment li = new LoanInstallment();
            while (dataReader.Read())
            {
                li = AssignInstallmentLoan(dataReader);
            }
            dataReader.Close();
            Polaczenia.OdlaczenieOdBazy();
            return li;
        }
                
        /// <summary>
        /// Pobieranie rat pożyczek do listy wg daty i pracownika
        /// </summary>
        /// <param name="idEmployee"></param>
        /// <param name="date"></param>
        public void GetListInstallmentLoan(int idEmployee, DateTime date)
        {
            string select = "select * from rata_pozyczki as rp join pozyczka as p on p.id_pozyczki=rp.id_pozyczki where id_pracownika=" +
                idEmployee + " AND datepart(year,data_raty)=" + date.Year + " AND datepart(month,data_raty)=" + date.Month + "AND wplata_wlasna='false'";

            SqlDataReader dataReader = Database.GetData(select);
            arrayInstallmentLoan.Clear();
            while (dataReader.Read())
            {
                arrayInstallmentLoan.Add(AssignInstallmentLoan(dataReader));
            }
            dataReader.Close();
            Polaczenia.OdlaczenieOdBazy();
        }
 
        private static LoanInstallment AssignInstallmentLoan(SqlDataReader dataReader)
        {
            LoanInstallment li = new LoanInstallment();
            li.IdLoanInstallment = dataReader.GetInt32(0);
            li.IdLoan = dataReader.GetInt32(1);
            li.InstallmentAmount = dataReader.GetFloat(2);
            li.Date = dataReader.GetDateTime(3);
            if (dataReader.IsDBNull(4))
                li.IsOwnRepeyment = false;
            else
                li.IsOwnRepeyment = dataReader.GetBoolean(4);
            return li;
        }
    }
}
