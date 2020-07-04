using Konfiguracja;
using Logi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HumanResources.WorkTimeRecords
{
    public class WorkManager
    {
        public static ArrayList arrayListWorkTime = new ArrayList();
        public static void AddWorkTime(IWorkTime workTime, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = String.Empty;
            if (workTime is Work)
            // if (typeof(Work).IsInstanceOfType(workTime))
            {
                Work work = (Work)workTime;
                select = "insert into praca values('" + work.Date.ToString("d", DateFormat.TakeDateFormat()) + "'" +
               ",'" + work.IdEmployee + "','" + work.StartTime.ToString("d", DateFormat.TakeDateFormat()) + " " + work.StartTime.ToString("T", DateFormat.TakeDateFormat()) +
               "','" + work.StopTime.ToString("d", DateFormat.TakeDateFormat()) + " " + work.StopTime.ToString("T", DateFormat.TakeDateFormat()) + "')";
            }
            if (workTime is Illness)
            {
                Illness illness = (Illness)workTime;
                select = "insert into choroba values('" + illness.IdEmployee + "'" +
               ",'" + illness.Date.ToString("d", DateFormat.TakeDateFormat()) + "','" + illness.IdIllnessType + "')";
            }
            if (workTime is DayOff)
            {
                DayOff dayOff = (DayOff)workTime;
                select = "insert into urlop values('" + dayOff.IdEmployee + "'" +
                  ",'" + dayOff.Date.ToString("d", DateFormat.TakeDateFormat()) + "','" + dayOff.IdTypeDayOff + "')";
            }

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.dodawanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.praca, select), disconnect == ConnectionToDB.disconnect ? true : false);
        }


        public static void DeleteWorkTime(WorkType workType, int idEmployee, DateTime date, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = String.Empty;
            switch (workType)
            {
                case WorkType.work:
                    select = "delete praca where id_pracownika=" + idEmployee +
          " AND datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month + " AND datepart(day,data)=" + date.Day;
                    break;
                case WorkType.dayOff:
                    select = "delete urlop where id_pracownika=" + idEmployee +
                " AND datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month + " AND datepart(day,data)=" + date.Day;
                    break;
                case WorkType.illness:
                     select = "delete choroba where id_pracownika=" + idEmployee +
               " AND datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month + " AND datepart(day,data)=" + date.Day;
                    break;
                default:
                    break;
            }

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.dodawanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.praca, select), disconnect == ConnectionToDB.disconnect ? true : false);


        }

        public static void GetWorkToList(int idEmployee, DateTime date, ConnectionToDB disconnect)//misiąc+rok
        {
            string select = "select data,od_godz,do_godz from praca where id_pracownika=" + idEmployee +
                " AND datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month;

            //odczyt dataReaderem zwróconego wiersza 
            SqlDataReader dataReaderGodziny = Database.GetData(select);

            while (dataReaderGodziny.Read())
            {
                Work w = new Work();
                w.Date = dataReaderGodziny.GetDateTime(0);
                w.StartTime = dataReaderGodziny.GetDateTime(1);
                w.StopTime = dataReaderGodziny.GetDateTime(2);
                //id potrzebne do menu kontekstowego grida
                w.IdEmployee = idEmployee;
                arrayListWorkTime.Add(w);
            }
            dataReaderGodziny.Close();
            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
        }
        public static void GetIllnessToList(int idEmployee, DateTime date, ConnectionToDB disconnect)//misiąc+rok
        {
            string select = "select * from choroba_rodzaj_new where id_pracownika=" + idEmployee +
                " AND datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month;
            
            SqlDataReader dataReader = Database.GetData(select);

            while (dataReader.Read())
            {
                Illness i = new Illness();
                                
                i.IdEmployee = idEmployee;
                i.Date = dataReader.GetDateTime(1);
                i.IdIllnessType = dataReader.GetInt32(2);
                i.PercentTypeIllness = dataReader.GetFloat(3);

                arrayListWorkTime.Add(i);
            }
            dataReader.Close();
            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
        }
        /// <summary>
        /// Metoda pobiera z bazy dni w których dany pracownik był na urlopie
        /// w danym niesiącu i w danym roku.
        /// Wynik zapisuje do listy.
        /// </summary>
        /// <param name="idEmployee">Id pracownika</param>
        /// <param name="date">Data do sprawdzenia (miesiąc, rok)</param>
        /// <param name="disconnect">True - odłącza od bazy danych</param>
        public static void GetDayOffToList(int idEmployee, DateTime date, ConnectionToDB disconnect)//misiąc+rok
        {
            string select = "select * from urlop_rodzaj_new where id_pracownika=" + idEmployee +
                " AND datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month;

            SqlDataReader dataReader = Database.GetData(select);

            while (dataReader.Read())
            {
                DayOff d = new DayOff();
                //id potrzebne do grida
                d.IdEmployee = idEmployee;
                d.Date = dataReader.GetDateTime(1);
                d.IdTypeDayOff = dataReader.GetInt32(2);
                d.PercentTypeDayOff = dataReader.GetInt32(3);

                arrayListWorkTime.Add(d);
            }
            dataReader.Close();
            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
        }

        /// <summary>
        /// Metoda pobiera z bazy dni w których jakikolwiek pracownik był na urlopie
        /// w danym niesiącu i w danym roku.
        /// Wynik zapisuje do listy.
        /// </summary>
        /// <param name="date">Data do sprawdzenia (miesiąc, rok)</param>
        /// <param name="disconnect">True - odłącza od bazy danych</param>
        public static void GetDayOffToList(DateTime date, ConnectionToDB disconnect)//misiąc+rok
        {
            string select = "select * from urlop_rodzaj_new where datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month;

            arrayListWorkTime.Clear();

            SqlDataReader dataReader = Database.GetData(select);

            while (dataReader.Read())
            {
                DayOff d = new DayOff();
                //id potrzebne do grida
                d.IdEmployee = dataReader.GetInt32(0);
                d.Date = dataReader.GetDateTime(1);
                d.IdTypeDayOff = dataReader.GetInt32(2);
                d.PercentTypeDayOff = dataReader.GetInt32(3);

                arrayListWorkTime.Add(d);
            }
            dataReader.Close();
            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
        }
        public static DayOff GetDayOff(int idEmployee, DateTime date, ConnectionToDB disconnect)//misiąc+rok
        {
            string select = "select * from urlop where id_pracownika=" + idEmployee +
                " AND datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month + " AND datepart(day,data)=" + date.Day;

            SqlDataReader dataReader = Database.GetData(select);

                DayOff d = new DayOff();
            while (dataReader.Read())
            {
                //id potrzebne do grida
                d.IdEmployee = idEmployee;
                d.Date = dataReader.GetDateTime(1);
                d.IdTypeDayOff = dataReader.GetInt32(2);

            }
            dataReader.Close();
            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
            return d;
        }

    }
}
