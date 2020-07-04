using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using Konfiguracja;
namespace HumanResources.WorkTimeRecords
{
    public class Holidays
    {
        private int id;
        private DateTime date;
        private string description;

        private static ArrayList arrayListHolidays = new ArrayList();

        /// <summary>
        /// Konstruktor 
        /// dodaje przy wydruku karty godzin dni weekendowe
        /// </summary>
        /// <param name="data"></param>
        public Holidays(DateTime data)
        {
            this.date = data;
        }

        public Holidays()
        {
        }

        #region PROPERTYSY

        public int Id
        {
            get { return id; }
        }
        public DateTime Date
        {
            get { return date; }
        }

        public string Description
        {
            get { return description; }
        }

        public static ArrayList ArrayListHolidays
        {
            get
            {
                return arrayListHolidays;
            }
        }

        #endregion

        public static void GetAll(DateTime data, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = "select * from dni_wolne where datepart(year,data)=" + data.Year + " AND datepart(month,data)=" + data.Month;

            SqlDataReader dataReader = Database.GetData(select);

            //zerowanie listy
            ArrayListHolidays.Clear();

            while (dataReader.Read())
            {
                Holidays dw = new Holidays();

                dw.id = dataReader.GetInt32(0);
                dw.date = dataReader.GetDateTime(1);
                dw.description = dataReader.GetString(2);

                ArrayListHolidays.Add(dw);
            }
            dataReader.Close();

            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
        }
        internal static bool IsHoliday(DateTime date, out string description, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            if (!isAlreadyInList(date))
                GetAll(date, disconnect);

            description = "";
            foreach (Holidays d in Holidays.ArrayListHolidays)
            {
                if (d.Date == date)
                {
                    description = d.Description;
                    return true;
                }
            }
            return false;
        }
        internal static bool IsHoliday(DateTime date, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            if (!isAlreadyInList(date))
                GetAll(date, disconnect);
            
            foreach (Holidays d in Holidays.ArrayListHolidays)
            {
                if (d.Date == date)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsHolidayOrWeekend(DateTime date, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            if (!isAlreadyInList(date))
                GetAll(date, disconnect);

            foreach (Holidays d in Holidays.ArrayListHolidays)
            {
                if (d.Date == date)
                {
                    return true;
                }
            }
            //dni wolne - weekwnd
            if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }
            return false;
        }

        private static bool isAlreadyInList(DateTime date)
        {
            if (arrayListHolidays.Count == 0)
                return false;
                    
            Holidays h = (Holidays)arrayListHolidays[0];
                                
            if (h.Date.Year != date.Year)
                return false;

            if (h.Date.Month != date.Month)
                return false;

            return true;
        }     
    }
}
