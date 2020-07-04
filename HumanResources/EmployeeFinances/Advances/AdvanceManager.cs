using Konfiguracja;
using Logi;
using Pracownicy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HumanResources.EmployeesFinances.Advances
{
    class AdvanceManager
    {
        public static ArrayList arrayListAdvances = new ArrayList();

        public static void Add(Advance a, ConnectionToDB disconnect)
        {
            string select = "insert into zaliczka values('" + a.IdEmployee + "'" +
                ",'" + a.Amount.ToString().Replace(',', '.') + "','" + a.Date.ToString("d", DateFormat.TakeDateFormat()) + "','" + a.OtherInfo + "')";

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.dodawanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.zaliczka,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public static void Edit(Advance a, ConnectionToDB disconnect)
        {
            string select = "update zaliczka set kwota='" + a.Amount.ToString().Replace(',', '.') +
                "', inne='" + a.OtherInfo + "', data='" + a.Date.ToString("d", DateFormat.TakeDateFormat()) + "'where id_zaliczki = '" + a.Id + "'";

            Database.Save(select, disconnect);

            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.zaliczka,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
            //odłączenie od bazy
            Polaczenia.OdlaczenieOdBazy();
        }

        public static void Delete(int idAdvance, ConnectionToDB disconnect)
        {
            string select = "delete zaliczka where id_zaliczki=" + idAdvance;

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.usuwanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.zaliczka,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public static double GetSumAdvancesByDate(int idEmployee, DateTime date)
        {
            string select = "select sum(kwota) from zaliczka where id_pracownika=" + idEmployee +
                " AND datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month;
            try
            {
                return (Convert.ToDouble(Database.GetOneElement(select, ConnectionToDB.disconnect)));

            }
            catch (FormatException ex3)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.zaliczka, "AdditionsAdvancesManager.GetAdvancesByDate()/n/n" + ex3.Message));

                return 0d;
            }
        }
        public static void GetAdvancesByDate(int idEmployee, DateTime date)
        {
            string select = "select * from zaliczka where id_pracownika=" + idEmployee +
                " AND datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month;
            DownloadAdvancesToList(select);
        }
        private static void DownloadAdvancesToList(string select, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            //odczyt dataReaderem zwróconego wiersza 
            SqlDataReader dataReader = Database.GetData(select);

            arrayListAdvances.Clear();

            while (dataReader.Read())
            {
                arrayListAdvances.Add(AssignAdvance(dataReader));
            }
            dataReader.Close();
            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
        }

        private static Advance AssignAdvance(SqlDataReader dataReader)
        {
            Advance d = new Advance();
            d.Id = dataReader.GetInt32(0);
            d.IdEmployee = dataReader.GetInt32(1);
            d.Amount = dataReader.GetFloat(2);
            d.Date = dataReader.GetDateTime(3);
            d.OtherInfo = dataReader.GetString(4);

            return d;
        }
    }
}
