using HumanResources.Exceptions;
using Konfiguracja;
using Logi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.WorkTimeRecords
{
    /// <summary>
    /// Godziny przepracowane
    /// </summary>
    public class Work : WorkTime, IWorkTime
    {
        DateTime startTime;
        DateTime stopTime;

        public DateTime StartTime { get => startTime; set => startTime = value; }
        public DateTime StopTime { get => stopTime; set => stopTime = value; }

        public  void Edit(Work w, ConnectionToDB disconnect)
        {
            string select = "update praca set od_godz='" + w.StartTime.ToString("d", DateFormat.TakeDateFormat()) + " " + w.StartTime.ToString("T", DateFormat.TakeDateFormat()) + "', do_godz='" + w.StopTime.ToString("d", DateFormat.TakeDateFormat()) +
                 " " + w.StopTime.ToString("T", DateFormat.TakeDateFormat()) + "' where id_pracownika='" + w.IdEmployee + "' AND data='" + w.Date.ToString("d", DateFormat.TakeDateFormat()) + "'";
           
            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.praca, select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public  void Delete(int id, DateTime data, ConnectionToDB disconnect)
        {
           string select = "delete praca where id_pracownika=" + id +
                " AND datepart(year,data)=" + data.Year + " AND datepart(month,data)=" + data.Month + " AND datepart(day,data)=" + data.Day;

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.usuwanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.praca, select), disconnect == ConnectionToDB.disconnect ? true : false);            
        }

        public static bool IsAlreadyInDataBase(int idEmployee, DateTime data)
        {
            string select = "select * from praca where id_pracownika=" + idEmployee +
                " AND datepart(year,data)=" + data.Year + " AND datepart(month,data)=" + data.Month + " AND datepart(day,data)=" + data.Day;

            return Database.GetOneElementBool(select, ConnectionToDB.disconnect);
        }

        public bool IsAlreadyInDataBase()
        {
            string select = "select * from praca where id_pracownika=" + IdEmployee +
                " AND datepart(year,data)=" + Date.Year + " AND datepart(month,data)=" + Date.Month + " AND datepart(day,data)=" + Date.Day;

            return Database.GetOneElementBool(select, ConnectionToDB.disconnect);
        }

        public TimeSpan WorkTimeAll()
        {
            return stopTime - startTime;
        }
        public TimeSpan WorkTime50()
        {
            if (Date.DayOfWeek != DayOfWeek.Saturday && Date.DayOfWeek != DayOfWeek.Sunday)
            {
                if ((stopTime - startTime).Hours >= 8)
                    return stopTime - startTime - new TimeSpan(8, 0, 0);
                else//jeżeli poniżej 8 godzin (żeby nie wyświetlało godzin ulemnych
                    return new TimeSpan(0, 0, 0);
            }
            else
                return new TimeSpan(0, 0, 0);
        }
        public TimeSpan WorkTime100()
        {
            //dni wolne - weekwnd wszystkie godziny to nadgodziny 100%
            if (Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday)
            {
                return stopTime - startTime;
            }
            else
                return new TimeSpan(0, 0, 0);
        }
        public TimeSpan WorkTimeRegularWork()
        {
            if ((Date.DayOfWeek != DayOfWeek.Saturday && Date.DayOfWeek != DayOfWeek.Sunday) && ((stopTime - startTime).Hours >= 8))
                return new TimeSpan(8, 0, 0);
            else if ((Date.DayOfWeek != DayOfWeek.Saturday || Date.DayOfWeek != DayOfWeek.Sunday) && ((stopTime - startTime).Hours < 8))
                return stopTime - startTime;
            return new TimeSpan(0, 0, 0);
        }

        public int GetDay()
        {
            return Date.Day;
        }
    }
}
