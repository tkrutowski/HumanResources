using Konfiguracja;
using Logi;
using Pracownicy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HumanResources.Loans
{
    class LoanManager
    {
        public static ArrayList arrayLoans = new ArrayList();

        public static void AddLoan(Loan l, ConnectionToDB disconnect)
        {
            string select = "insert into pozyczka (id_pracownika, nazwa, kwota, data, ile_pobierac, inne, czy_splacone)values('" +
                l.IdEmployee + "','" + l.Name + "','" + l.Amount.ToString().Replace(',', '.') + "','" + l.Date.ToString("d", DateFormat.TakeDateFormat()) + "','" + l.InstallmentLoan.ToString().Replace(',', '.') + "','" +
                l.OtherInfo + "','" + (int)Enum.Parse(typeof(Payment), l.IsPaid.ToString()) + "')";

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.dodawanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.pozyczka,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public static void EditLoan(Loan l, ConnectionToDB disconnect)
        {
            string select = "update pozyczka set nazwa='" + l.Name + "',kwota = '" + l.Amount.ToString().Replace(',', '.') + "', data = '" +
                l.Date.ToString("d", DateFormat.TakeDateFormat()) + "', ile_pobierac = '" + l.InstallmentLoan.ToString().Replace(',', '.') + "', inne = '" + l.OtherInfo + "' where id_pozyczki='" + l.IdLoan + "'";

            Database.Save(select, disconnect);

            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.pozyczka,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public static Loan GetLoan(int idLoan, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            Loan loan = new Loan();
            string select = "select * from pozyczka where id_pozyczki = " + idLoan;
            SqlDataReader dataReader = Database.GetData(select);

            while (dataReader.Read())
            {
                loan = AssignLoan(dataReader);
            }
            dataReader.Close();

            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
            return loan;
        }
        public static void GetAllLoanToList()
        {
            GetLoansToList("select * from pozyczka");
        }
        public static void GetLoanIsPaidToList(int idEmployee, Payment isPaid)
        {
            GetLoansToList("select * from pozyczka where id_pracownika = " + idEmployee +
                " AND czy_splacone='" + (int)Enum.Parse(typeof(Payment), isPaid.ToString()) + "'");
        }
        public static void GetLoanToList(int idEmployee)
        {
            GetLoansToList("select * from pozyczka where id_pracownika = " + idEmployee);
        }
        public static void GetLoanIsPaidToList(Payment isPaid)
        {
            string select=("select * from pozyczka where czy_splacone='" + (int)Enum.Parse(typeof(Payment), isPaid.ToString()) + "'");
            GetLoansToList(select);
        }

        public static void DeleteLoan(int idLoan, ConnectionToDB disconnect)
        {
            string select = "delete pozyczka where id_pozyczki=" + idLoan;
            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.usuwanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.pozyczka,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        private static void GetLoansToList(string select)
        {
            //odczyt dataReaderem zwróconego wiersza 
            SqlDataReader dataReader = Database.GetData(select);

            arrayLoans.Clear();

            while (dataReader.Read())
            {
                arrayLoans.Add(AssignLoan(dataReader));
            }
            dataReader.Close();
            Polaczenia.OdlaczenieOdBazy();
        }

        private static Loan AssignLoan(SqlDataReader dataReader)
        {
            Loan l = new Loan();

            l.IdLoan = dataReader.GetInt32(0);
            l.IdEmployee = dataReader.GetInt32(1);
            l.Name = dataReader.GetString(2);
            l.Amount = dataReader.GetFloat(3);
            l.Date = dataReader.GetDateTime(4);
            l.InstallmentLoan = dataReader.GetFloat(5);
            l.OtherInfo = dataReader.GetString(6);
            //bool value = (dataReader.GetBoolean(7));
            l.IsPaid = (Payment)Enum.Parse(typeof(Payment), Convert.ToInt32(dataReader.GetBoolean(7)).ToString());
            //l.GetListInstallmentLoan();
            return l;
        }
    }
}
