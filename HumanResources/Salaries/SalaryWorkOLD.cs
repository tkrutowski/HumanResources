using HumanResources.Employees;
using HumanResources.WorkTimeRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Salaries
{
    public class SalaryWorkOLD
    {
        int idEmployee;
        DateTime date;

        double forOvertime;
        double forRegularTime;

        int numberOfMinutesAll;
        int numberOfMinutesOver;

        public SalaryWorkOLD(int idEmployee, DateTime date)
        {
            this.idEmployee = idEmployee;
            this.date = date;

            GetWork();
        }

        public double ForOvertime { get => forOvertime; set => forOvertime = value; }
        public double ForRegularTime { get => forRegularTime; set => forRegularTime = value; }
        public double ForAll { get => forRegularTime + forOvertime; }
        public int NumberOfMinutesAll { get => numberOfMinutesAll; set => numberOfMinutesAll = value; }
        public int NumberOfMinutesOver { get => numberOfMinutesOver; set => numberOfMinutesOver = value; }

        void GetWork()
        {
            WorkManager.arrayListWorkTime.Clear();

            //pobieranie danych z bazy i zapisanie ich do listyGodzin
            WorkManager.GetWorkToList(idEmployee, date, ConnectionToDB.notDisconnect);

            NumberOfMinutesAll = 0;
            NumberOfMinutesOver = 0;

            if (WorkManager.arrayListWorkTime.Count == 0)
                throw new Exceptions.NoNullException("Lista czasu pracy jest pusta");

            foreach (IWorkTime workTime in WorkManager.arrayListWorkTime)
            {
                if (workTime is Work)//wpisywanie godzin pracy do grida
                {
                    Work w = (Work)workTime;

                    NumberOfMinutesAll += (int)w.WorkTimeAll().TotalMinutes;
                }
            }
        }

        public double CalculateSalaryRegular(RateRegular rate, int hoursToWork)
        {
            if (NumberOfMinutesAll == 0)
                throw new Exceptions.NoNullException("Nie ma danych o czasie pracy.");
            switch (rate.IsMonthlyOrHourly)
            {
                case RateType.hourly:
                    forRegularTime = rate.RateValue * ((double)(NumberOfMinutesAll - NumberOfMinutesOver) / 60d);
                    break;
                case RateType.monthly:
                    forRegularTime = (rate.RateValue / hoursToWork) * ((double)(NumberOfMinutesAll - NumberOfMinutesOver) / 60d);
                    break;
            }
            return Math.Round(ForRegularTime, 2, MidpointRounding.AwayFromZero);
        }
   
        public double CalculateSalaryOvertime_OLD(RateOvertime rate)
        {
            if (NumberOfMinutesOver != 0)
            {
                forOvertime = (((double)NumberOfMinutesOver / 60d) * rate.RateValue);
            }
            return Math.Round(ForOvertime, 2, MidpointRounding.AwayFromZero);
        }
    }
}
