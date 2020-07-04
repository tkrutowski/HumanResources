using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.WorkTimeRecords
{
    public abstract class WorkTime 
    {
        private int idEmployee;
        private DateTime date;

        public  int IdEmployee { get => idEmployee; set => idEmployee = value; }
        public DateTime Date { get => date; set => date = value; }
        //public abstract void Add(WorkTime workTime);
        //public abstract void Edit(WorkTime workTime);
        public bool  isHolidayOrWeekend()
        {
            foreach (Holidays d in Holidays.ArrayListHolidays)
            {
                if (d.Date == Date)
                {
                    return true;
                }
            }
            //dni wolne - weekwnd
            if (Date.DayOfWeek == DayOfWeek.Saturday || Date.DayOfWeek == DayOfWeek.Sunday)
            {
                return true;
            }
            return false;
        }


    }
}
