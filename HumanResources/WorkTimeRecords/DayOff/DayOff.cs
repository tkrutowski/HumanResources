using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Logi;
using Konfiguracja;

namespace HumanResources.WorkTimeRecords
{
    public class DayOff:WorkTime, IWorkTime
    {
        int idTypeDayOff;
        int percentTypeDayOff;

        //lista rodzaj urlopu
        public static ArrayList listaRodzajUrlopu = new ArrayList();

        public int IdTypeDayOff { get => idTypeDayOff; set => idTypeDayOff = value; }
        public int PercentTypeDayOff { get => percentTypeDayOff; set => percentTypeDayOff = value; }

        public int GetDay()
        {
            return Date.Day;
        }

        public void DayOffSubtraction(string howMuch, ConnectionToDB disconnect)
        {
            string select = "update pracownik set urlop_pozostaly=urlop_pozostaly-" + howMuch + " where id_pracownika=" + IdEmployee;

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.urlop,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        } 
        public void DayOffAddition(string howMuch, ConnectionToDB disconnect)
        {
            string select = "update pracownik set urlop_pozostaly=urlop_pozostaly + " + howMuch + " where id_pracownika=" + IdEmployee;

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.urlop,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public TimeSpan WorkTimeAll()
        {
            if (IdTypeDayOff ==  (int)Enum.Parse(typeof(DayOffType), DayOffType.halfDay.ToString()))
            {
                return new TimeSpan(4, 0, 0);
            }
            else if (IdTypeDayOff == (int)Enum.Parse(typeof(DayOffType), DayOffType.free.ToString()) || (IdTypeDayOff == (int)Enum.Parse(typeof(DayOffType), DayOffType.educational.ToString())))
                return new TimeSpan(0, 0, 0);
            else
            {
                return new TimeSpan(8, 0, 0);
            }
        }

        public TimeSpan WorkTime50()
        {
            return new TimeSpan(0, 0, 0);
        }

        public TimeSpan WorkTime100()
        {
            return new TimeSpan(0, 0, 0);
        }

        public bool IsAlreadyInDataBase()
        {
            string select = "select nazwa from urlop as u join rodzaj_urlopu as ru on u.id_rodzaj_urlopu=ru.id_rodzaj_urlopu where id_pracownika=" + IdEmployee +
                " AND datepart(year,data)=" + Date.Year + " AND datepart(month,data)=" + Date.Month + " AND datepart(day,data)=" + Date.Day;

            return Database.GetOneElementBool(select, ConnectionToDB.disconnect);
        }
        public static bool IsAlreadyInDataBase(int idEmployee, DateTime date)
        {
            string select = "select nazwa from urlop as u join rodzaj_urlopu as ru on u.id_rodzaj_urlopu=ru.id_rodzaj_urlopu where id_pracownika=" + idEmployee +
                  " AND datepart(year,data)=" + date.Year + " AND datepart(month,data)=" + date.Month + " AND datepart(day,data)=" + date.Day;

            return Database.GetOneElementBool(select, ConnectionToDB.disconnect);
        }

       
    }
}
