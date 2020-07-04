using Konfiguracja;
using Logi;
using Pracownicy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HumanResources.EmployeesFinances.Additions
{
    class AdditionManager
    {
        public static ArrayList arrayListAddition = new ArrayList();
        public static void Add(Addition a, ConnectionToDB disconnect)
        {
            string select = "insert into dodatek values('" + a.IdEmployee + "'" +
                ",'" + a.AdditionType.Id + "','" + a.Amount.ToString().Replace(',', '.') + "','" + a.Date.ToString("d", DateFormat.TakeDateFormat()) + "','" + a.OtherInfo + "')";

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.dodawanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.dodatek,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public static void Edit(Addition a, ConnectionToDB disconnect)
        {
            string select = "update dodatek set kwota='" + a.Amount.ToString().Replace(',', '.') + "', data='"
            + a.Date.ToString("d", DateFormat.TakeDateFormat()) + "',inne='" + a.OtherInfo + "'where id_dodatku = '" + a.Id + "'";

            Database.Save(select, disconnect);

            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.dodatek,
                select),disconnect == ConnectionToDB.disconnect ? true : false);
            //odłączenie od bazy
            Polaczenia.OdlaczenieOdBazy();
        }

        public static void Delete(int idAddition, ConnectionToDB disconnect)
        {
            string select = "delete dodatek where id_dodatku=" + idAddition;

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.usuwanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.dodatek,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public static double GetSumAllAdditionsByDate(int idEmployee, DateTime date)
        {
            string select = "select sum(kwota) from dodatek where id_pracownika=" + idEmployee +
                " AND datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month;
            try
            {
                return (Convert.ToDouble(Database.GetOneElement(select, ConnectionToDB.disconnect)));

            }
            catch (FormatException ex3)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.dodatek, "AdditionsAdvancesManager.GetAdditionsByDate()/n/n" + ex3.Message));

                return 0d;
            }
        }

        public static void GetAdditionsByDate(int idEmployee, DateTime date)
        {
            string select = "select * from dodatek_rodzaj where id_pracownika=" + idEmployee +
                " AND datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month;
            DownloadAdditionsToList(select);
        }
        private static void DownloadAdditionsToList(string select, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            //odczyt dataReaderem zwróconego wiersza 
            SqlDataReader dataReader = Database.GetData(select);

            arrayListAddition.Clear();

            while (dataReader.Read())
            {
                arrayListAddition.Add(AssignAddition(dataReader));
            }
            dataReader.Close();
            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
        }

        private static Addition AssignAddition(SqlDataReader dataReader)
        {
            AdditionType at = new AdditionType();
            Addition d = new Addition();
            d.Id = dataReader.GetInt32(0);
            d.IdEmployee = dataReader.GetInt32(1);
            d.Amount = dataReader.GetFloat(2);
            d.Date = dataReader.GetDateTime(3);
            d.OtherInfo = dataReader.GetString(4);

            at.Id = dataReader.GetInt32(5);
            at.Name = dataReader.GetString(6);
            d.AdditionType = at;

            return d;
        }
    }
}
