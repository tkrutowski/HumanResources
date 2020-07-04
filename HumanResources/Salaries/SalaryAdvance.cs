using HumanResources.EmployeesFinances.Advances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Salaries
{
    class SalaryAdvance
    {
        int idEmployee;
        DateTime date;

        double forAdvances;

        public SalaryAdvance(int idEmployee, DateTime date)
        {
            this.idEmployee = idEmployee;
            this.date = date;

            GetAdvansesSum();
        }

        public double ForAdvances { get => forAdvances; set => forAdvances = value; }

        private double GetAdvansesSum()
        {
            ForAdvances = AdvanceManager.GetSumAdvancesByDate(idEmployee, date);
            return Math.Round(ForAdvances, 2, MidpointRounding.AwayFromZero);
        }
    }
}
