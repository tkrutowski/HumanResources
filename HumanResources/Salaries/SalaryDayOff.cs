using HumanResources.Employees;
using HumanResources.WorkTimeRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Salaries
{
    public class SalaryDayOff
    {
        int idEmployee;
        DateTime date;

        int numberOfMinutesDayOffFree;
        int numberOfMinutesDayOffPaid;
        double forDayOff;

        public SalaryDayOff(int idEmployee, DateTime date)
        {
            this.idEmployee = idEmployee;
            this.date = date;

            GetDayOff();
        }
        public int NumberOfMinutesDayOffFree { get => numberOfMinutesDayOffFree; set => numberOfMinutesDayOffFree = value; }
        public int NumberOfMinutesDayOffPaid { get => numberOfMinutesDayOffPaid; set => numberOfMinutesDayOffPaid = value; }
        public double ForDayOff { get => forDayOff; set => forDayOff = value; }

        private void GetDayOff()
        {
            WorkManager.arrayListWorkTime.Clear();

            //pobieranie danych z bazy i zapisanie ich do listyGodzin
            WorkManager.GetDayOffToList(idEmployee, date, ConnectionToDB.notDisconnect);

            foreach (IWorkTime workTime in WorkManager.arrayListWorkTime)
            {
                DayOff d = (DayOff)workTime;
                if (workTime is DayOff)
                {
                    if (d.PercentTypeDayOff == 1)
                    {
                        if (d.IdTypeDayOff == (int)Enum.Parse(typeof(DayOffType), DayOffType.halfDay.ToString()))
                            NumberOfMinutesDayOffPaid += 240;//4 godziny
                        else
                            NumberOfMinutesDayOffPaid += 480;//8 godzin
                    }
                    else
                        NumberOfMinutesDayOffFree += 480;
                }
            }
        }

        public double CalculateSalaryDayOff(RateRegular rate, int hoursToWork)
        {
            if (NumberOfMinutesDayOffPaid == 0)
                return 0;

            switch (rate.IsMonthlyOrHourly)
            {
                case RateType.hourly:
                    ForDayOff = rate.RateValue * ((double)(NumberOfMinutesDayOffPaid) / 60d);
                    break;
                case RateType.monthly:
                    ForDayOff = (rate.RateValue / hoursToWork) * ((double)(NumberOfMinutesDayOffPaid) / 60d);
                    break;
            }
            return Math.Round(ForDayOff, 2, MidpointRounding.AwayFromZero);
        }
    }
}