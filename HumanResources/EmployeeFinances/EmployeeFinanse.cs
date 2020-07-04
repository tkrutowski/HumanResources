using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.EmployeesFinances
{
    public abstract class EmployeeFinanse
    {
        int id;
        int idEmployee;
        float amount;
        DateTime date;
        string otherInfo;

        public static bool isCorrect;

        public int Id { get => id; set => id = value; }
        public int IdEmployee { get => idEmployee; set => idEmployee = value; }
        public float Amount { get => amount; set => amount = value; }
        public DateTime Date { get => date; set => date = value; }
        public string OtherInfo { get => otherInfo; set => otherInfo = value; }
    }
}
