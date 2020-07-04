using HumanResources.EmployeesFinances.Additions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Salaries
{
    public class SalaryAddition
    {
        int idEmployee;
        DateTime date;

        double forAdditions;

        public SalaryAddition(int idEmployee, DateTime date)
        {
            this.idEmployee = idEmployee;
            this.date = date;
            GetAdditionsSum();
        }

        public double ForAdditions { get => forAdditions; set => forAdditions = value; }

        private double GetAdditionsSum()
        {
            ForAdditions = AdditionManager.GetSumAllAdditionsByDate(idEmployee, date);
            return Math.Round(ForAdditions, 2, MidpointRounding.AwayFromZero);
        }
    }
}
