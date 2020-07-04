using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Konfiguracja;
using Logi;
using Pracownicy;

namespace HumanResources.Employees
{
    public class RateOvertime : EmployeeRate
    {
        public RateOvertime(int idRate, DateTime dateFrom, float rateValue) : base(idRate, dateFrom, rateValue)
        {
        }
        public RateOvertime(DateTime dateFrom, float rateValue) : base(dateFrom, rateValue)
        {
        }

        public bool IsExist()
        {
            string select = "select id_stawki_nadgodziny from stawka_nadgodziny where id_pracownika=" + this.IdEmployee +
                    " AND datepart(year,data_od)=" + this.DateFrom.Year + " AND datepart(month,data_od)=" + this.DateFrom.Month;

            return Database.GetOneElementBool(select);
        }
    }
}
