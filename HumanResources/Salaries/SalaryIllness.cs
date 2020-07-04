using HumanResources.Employees;
using HumanResources.WorkTimeRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Salaries
{
    public class SalaryIllness
    {
        int idEmployee;
        DateTime date;

        double forIllness80;
        double forIllness100;
        
        int  numberOfMinutesIllness80;
        int numberOfMinutesIllness100;

        public SalaryIllness(int idEmployee, DateTime date)
        {
            this.idEmployee = idEmployee;
            this.date = date;

            GetIllness();
        }

        public double ForIllness80 { get => forIllness80; set => forIllness80 = value; }
        public double ForIllness100 { get => forIllness100; set => forIllness100 = value; }
        public double ForAll { get => forIllness100 + forIllness80; }
        public int NumberOfMinutesIllness80 { get => numberOfMinutesIllness80; set => numberOfMinutesIllness80 = value; }
        public int NumberOfMinutesIllness100 { get => numberOfMinutesIllness100; set => numberOfMinutesIllness100 = value; }

        private void GetIllness()
        {
            WorkManager.arrayListWorkTime.Clear();

            //pobieranie danych z bazy i zapisanie ich do listyGodzin
            WorkManager.GetIllnessToList(idEmployee, date, ConnectionToDB.disconnect);

            foreach (IWorkTime workTime in WorkManager.arrayListWorkTime)
            {
                Illness i = (Illness)workTime;
                if (workTime is Illness)
                {
                    if (i.PercentTypeIllness == 1)
                    {
                         NumberOfMinutesIllness100 += 480;//8 godzin
                    }
                    else
                         NumberOfMinutesIllness80 += 480;
                }
            }
        }

        public double CalculateSalaryIllness80(RateRegular rate, int hoursToWork)
        {
            if ( NumberOfMinutesIllness80 == 0)
                return 0;

            switch (rate.IsMonthlyOrHourly)
            {
                case RateType.hourly:
                    ForIllness80 = (rate.RateValue * 0.8d) * ((double)( NumberOfMinutesIllness80) / 60d);
                    break;
                case RateType.monthly:
                    ForIllness80 = ((rate.RateValue / hoursToWork) * 0.8d) * ((double)( NumberOfMinutesIllness80) / 60d);
                    break;
            }
            return Math.Round(ForIllness80, 2, MidpointRounding.AwayFromZero);
        }

        public double CalculateSalaryIllness100(RateRegular rate, int hoursToWork)
        {
            if (NumberOfMinutesIllness100 == 0)
                return 0;
            switch (rate.IsMonthlyOrHourly)
            {
                case RateType.hourly:
                    ForIllness100 = rate.RateValue * ((double)(NumberOfMinutesIllness100) / 60d);
                    break;
                case RateType.monthly:
                    ForIllness100 = (rate.RateValue / hoursToWork) * ((double)(NumberOfMinutesIllness100) / 60d);
                    break;
            }
            return Math.Round(ForIllness100, 2, MidpointRounding.AwayFromZero);
        }
        public void CalculateAll(RateRegular rate, int hoursToWork)
        {
            CalculateSalaryIllness100(rate, hoursToWork);
            CalculateSalaryIllness80(rate, hoursToWork);
        }
    }
}
