using HumanResources.Employees;
using HumanResources.WorkTimeRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Salaries
{
    public class SalaryWork
    {
        int idEmployee;
        DateTime date;

        double forOvertime50;
        double forOvertime100;
        double forRegularTime;

        int numberOfMinutesAll;
        int numberOfMinutesRegular;
        int numberOfMinutes100;
        int numberOfMinutes50;

        public SalaryWork(int idEmployee, DateTime date)
        {
            this.idEmployee = idEmployee;
            this.date = date;

            GetWork();
        }

        public double ForOvertime50 { get => forOvertime50; set => forOvertime50 = value; }
        public double ForOvertime100 { get => forOvertime100; set => forOvertime100 = value; }
        public double ForRegularTime { get => forRegularTime; set => forRegularTime = value; }
        public double ForAll { get => forRegularTime + forOvertime50 + forOvertime100; }
        public int NumberOfMinutesAll { get => numberOfMinutesAll; set => numberOfMinutesAll = value; }
        public int NumberOfMinutes100 { get => numberOfMinutes100; set => numberOfMinutes100 = value; }
        public int NumberOfMinutes50 { get => numberOfMinutes50; set => numberOfMinutes50 = value; }
        public int NumberOfMinutesRegular { get => numberOfMinutesRegular; set => numberOfMinutesRegular = value; }

        void GetWork()
        {
            WorkManager.arrayListWorkTime.Clear();

            //pobieranie danych z bazy i zapisanie ich do listyGodzin
            WorkManager.GetWorkToList(idEmployee, date, ConnectionToDB.notDisconnect);

            NumberOfMinutesAll = 0;
            NumberOfMinutes50 = 0;
            NumberOfMinutes100 = 0;
            NumberOfMinutesRegular = 0;

            //if (WorkManager.arrayListWorkTime.Count == 0)
            //    throw new Exceptions.NoNullException("Lista czasu pracy jest pusta");

            foreach (IWorkTime workTime in WorkManager.arrayListWorkTime)
            {
                if (workTime is Work)//wpisywanie godzin pracy do grida
                {
                    Work w = (Work)workTime;

                    NumberOfMinutesAll += (int)w.WorkTimeAll().TotalMinutes;
                    NumberOfMinutes50 += (int)w.WorkTime50().TotalMinutes;
                    NumberOfMinutes100 += (int)w.WorkTime100().TotalMinutes;
                    NumberOfMinutesRegular += (int)w.WorkTimeRegularWork().TotalMinutes;
                }
            }
        }

        public double CalculateSalaryRegular(RateRegular rate, int hoursToWork)
        {
            if (NumberOfMinutesAll == 0)
                return 0;
            switch (rate.IsMonthlyOrHourly)
            {
                case RateType.hourly:
                    forRegularTime = rate.RateValue * ((double)(NumberOfMinutesAll - NumberOfMinutes50 - NumberOfMinutes100) / 60d);
                    break;
                case RateType.monthly:
                    forRegularTime = (rate.RateValue / hoursToWork) * ((double)(NumberOfMinutesAll - NumberOfMinutes50 - NumberOfMinutes100) / 60d);
                    break;
            }
            return Math.Round(ForRegularTime, 2, MidpointRounding.AwayFromZero);
        }
        public double CalculateSalaryOvertime50(RateOvertime rate)
        {
            if (NumberOfMinutes50 != 0)
            {
                ForOvertime50 = (((double)NumberOfMinutes50 / 60d) * rate.RateValue) * 1.5d;//50%
            }
            return Math.Round(ForOvertime50, 2, MidpointRounding.AwayFromZero);
        }
        public double CalculateSalaryOvertime100(RateOvertime rate)
        {
            if (NumberOfMinutes100 != 0)
            {
                forOvertime100 = (((double)NumberOfMinutes100 / 60d) * rate.RateValue) * 2d;//100%
            }
            return Math.Round(ForOvertime100, 2, MidpointRounding.AwayFromZero);
        }

        public void CalculateAll(RateRegular rate, int hoursToWork, RateOvertime rateOver)
        {
            CalculateSalaryRegular(rate, hoursToWork);
            CalculateSalaryOvertime50(rateOver);
            CalculateSalaryOvertime100(rateOver);
        }
    }
}
