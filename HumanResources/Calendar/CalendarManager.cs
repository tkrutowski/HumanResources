using HumanResources.WorkTimeRecords;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HumanResources.Calendar
{
    public class CalendarManager
    {
        private Dictionary<int, CalendarControl> hashMap;
        List<CalendarControl> calendarControlsTable; 
    
        /// <summary>
        /// Tworzy pusty kalendarz (tylko dzień tygodnia)
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Listę dni danego miesiąca</returns>
        List<CalendarControl> CreateEmptyCalendar(DateTime date)
        {
            calendarControlsTable = new List<CalendarControl>();

            for (int i = 1; i <= DateTime.DaysInMonth(date.Year, date.Month); i++)
            {
                calendarControlsTable.Add(new CalendarControl(i));

            }
            return calendarControlsTable;
        }

        /// <summary>
        /// Tworzy kalendarz urlopów
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dayOffList">lista urlopów wszystkich pracowników z danego miesiąca</param>
        /// <returns>Listę dni danego miesiąca</returns>
        List<CalendarControl> CreateDayOffCalendar(DateTime date, ArrayList dayOffList)
        {
            calendarControlsTable = new List<CalendarControl>();
            ArrayList arrayEmployeesFullName = new ArrayList();
            Employees.EmployeeManager employeeManager = new Employees.EmployeeManager();
            employeeManager.GetEmployeesHiredRealesedToList(true, TableView.table);          

            for (int i = 1; i <= DateTime.DaysInMonth(date.Year, date.Month); i++)
            {
                foreach (DayOff dayOff in dayOffList)
                {
                    if(dayOff.Date.Day == i)
                    {
                        foreach(Employees.Employee employee in Employees.EmployeeManager.arrayEmployees)
                        {
                            if(employee.IdEmployee==dayOff.IdEmployee)
                                arrayEmployeesFullName.Add(employee.GetFullName);
                        }
                    }
                }
                calendarControlsTable.Add(new CalendarControl(i, arrayEmployeesFullName));

                arrayEmployeesFullName.Clear();
            }
            return calendarControlsTable;
        }

        /// <summary>
        /// Pobiera dane urlopów wszystkich pracowników danego miesiąca
        /// z bazy danych i zapisuje do listy
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        ArrayList GetDayOffToList(DateTime date)
        {
            WorkManager.arrayListWorkTime.Clear();
            WorkManager.GetDayOffToList(date, ConnectionToDB.disconnect);

            return WorkManager.arrayListWorkTime;
        }

        /// <summary>
        /// Przypisuje utworzony kalendarz do formularza
        /// </summary>
        /// <param name="tlp"></param>
        /// <param name="date"></param>
        public void DisplayCalendar(TableLayoutPanel tlp,DateTime date)
        {
            for (int i = 0; i < NumberEmptyDaysBefore(date); i++)
            {
                tlp.Controls.Add(new Label());
            }
            
            foreach (CalendarControl entry in CreateDayOffCalendar(date, GetDayOffToList(date)))
            {
                tlp.Controls.Add(entry);
            }
        }

        /// <summary>
        /// Zwraca liczbę dni o które trzeba przesunąć kalendarz
        /// żeby pasowały dni (czwartek w kalendarzu = czwartek jako 1 dzień miesiąca)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        int NumberEmptyDaysBefore(DateTime date)
        {
            int result = 0;

            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    result = 6;
                    break;
                case DayOfWeek.Monday:
                    result = 0;
                    break;
                case DayOfWeek.Tuesday:
                    result = 1;
                    break;
                case DayOfWeek.Wednesday:
                    result = 2;
                    break;
                case DayOfWeek.Thursday:
                    result = 3;
                    break;
                case DayOfWeek.Friday:
                    result = 4;
                    break;
                case DayOfWeek.Saturday:
                    result = 5;
                    break;
            }

            return result;
        }
    }
}