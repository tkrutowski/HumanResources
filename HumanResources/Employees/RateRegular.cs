using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Konfiguracja;
using Logi;
using Pracownicy;

namespace HumanResources.Employees
{
    public class RateRegular:EmployeeRate
    {
        /// <summary>
        /// false - hourly
        /// true - monthly
        /// </summary>
        RateType isMonthlyOrHourly;

        
        public RateRegular(int idRate, DateTime dateFrom, float rateValue, RateType hourlyOrMonthly) : base(idRate, dateFrom, rateValue)
        {
            this.isMonthlyOrHourly = hourlyOrMonthly;
        }

        public RateRegular(DateTime dateFrom, float rateValue, RateType hourlyOrMonthly) : base(dateFrom, rateValue)
        {
            this.isMonthlyOrHourly = hourlyOrMonthly;
        }

        public RateType IsMonthlyOrHourly { get => isMonthlyOrHourly; set => isMonthlyOrHourly = value; }

        /// <summary>
        /// Sprawdza, czy stawka z podanego miesiaca jest juz w bazie danych
        /// false - nie ma wpisu z dana data
        /// true - jest taki wpis
        /// </summary>
        /// <param name="rateRegular"></param>
        public bool IsExist()
        {
            string select = "select id_stawki from stawka where id_pracownika=" + this.IdEmployee +
                    " AND datepart(year,data_od)=" + this.DateFrom.Year + " AND datepart(month,data_od)=" + this.DateFrom.Month;

            return Database.GetOneElementBool(select);
        }
    }
}
