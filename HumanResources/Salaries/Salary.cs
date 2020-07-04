using HumanResources.WorkTimeRecords;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HumanResources.Salaries
{
    public class Salary
    {        
        /// <summary>
		/// Metoda pobiera informacje o ilości godzin do przepracowania
		/// w danym miesiącu pobierane z bazy danych
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static int GetHoursToWork(DateTime date)
        {
            string select = "select ilosc_godzin from godziny_robocze where id_rok=" + date.Year + " AND id_miesiac=" + date.Month;
            
            return  Convert.ToInt32(Database.GetOneElement(select, ConnectionToDB.disconnect));
        }
    }
}
