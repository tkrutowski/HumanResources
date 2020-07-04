using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Employees
{
    public abstract class EmployeeRate
    {
        int idRate;
        int idEmployee;
        DateTime dateFrom;
        float rateValue;

        protected EmployeeRate(int idRate, DateTime dateFrom, float rateValue)
        {
            this.idRate = idRate;
            this.dateFrom = dateFrom;
            this.rateValue = rateValue;
        }

        protected EmployeeRate(DateTime dateFrom, float rateValue)
        {
            this.dateFrom = dateFrom;
            this.rateValue = rateValue;
        }

        public int IdRate { get => idRate; set => idRate = value; }
        public int IdEmployee { get => idEmployee; set => idEmployee = value; }
        public DateTime DateFrom { get => dateFrom; set => dateFrom = value; }
        public float RateValue { get => rateValue; set => rateValue = value; }
    }

}
