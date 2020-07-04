using HumanResources.WorkTimeRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HumanResources.MainForm
{
    internal class MainFormSalary
    {
        internal static void RefreshDgv(int idEmployee, DateTime date, DataGridView dgv)
        {
            //zmienne do wyświetlania nadgodzin na bieżąco
            int numberOfMinutesAll = 0;
            int numberOfMinutes50 = 0;
            int numberOfMinutes100 = 0;
            int numberOfMinutesIllness = 0;
            int numberOfMinutesDayOff = 0;
            int numberOfMinutesWork = 0;

            //zerowanie grida i listy
            dgv.Rows.Clear();
            WorkManager.arrayListWorkTime.Clear();

            //pobieranie danych z bazy i zapisanie ich do listyGodzin
            WorkManager.GetWorkToList(idEmployee, date, ConnectionToDB.notDisconnect);
            WorkManager.GetDayOffToList(idEmployee, date, ConnectionToDB.notDisconnect);
            WorkManager.GetIllnessToList(idEmployee, date, ConnectionToDB.notDisconnect);
            Holidays.GetAll(date);

            bool isHoliday = false;
            foreach (IWorkTime workTime in WorkManager.arrayListWorkTime)
            {
                isHoliday = false;
                if (workTime is Work)//wpisywanie godzin pracy do grida
                {
                    Work w = (Work)workTime;

                    isHoliday = w.isHolidayOrWeekend();

                    dgv.Rows.Add(w.Date.ToString("d", DateFormat.TakeDateFormatDayFirst()), w.Date.ToString("dddd"), w.StartTime.ToString("t", DateFormat.TakeTimeFormat()), w.StopTime.ToString("t", DateFormat.TakeTimeFormat()),
                        w.WorkTimeAll().ToString(DateFormat.TakeTimeSpanFormat()),//wszystkie godziny
                        w.WorkTime50().ToString(DateFormat.TakeTimeSpanFormat()),
                        isHoliday, w.IdEmployee, WorkType.work,
                        w.WorkTime100().ToString(DateFormat.TakeTimeSpanFormat()));

                    if (!isHoliday)
                    {
                        numberOfMinutes50 += (int)w.WorkTime50().TotalMinutes;
                        numberOfMinutesWork += (int)w.WorkTimeRegularWork().TotalMinutes;
                    }
                    else
                        numberOfMinutes100 += (int)w.WorkTime100().TotalMinutes;
                }
                else if (workTime is DayOff)//wpisywanie urlopu do grida
                {
                    DayOff d = (DayOff)workTime;

                    isHoliday = d.isHolidayOrWeekend();

                    dgv.Rows.Add(d.Date.ToString("d", DateFormat.TakeDateFormatDayFirst()), d.Date.ToString("dddd"), "Urlop", ChangeDayOffTypeToString.ChangeToString((DayOffType)d.IdTypeDayOff),
                        d.WorkTimeAll().ToString(DateFormat.TakeTimeSpanFormat()),
                        d.WorkTime50().ToString(DateFormat.TakeTimeSpanFormat()),
                        isHoliday, d.IdEmployee, WorkType.dayOff,
                        d.WorkTime100().ToString(DateFormat.TakeTimeSpanFormat()));
                    numberOfMinutesDayOff += (int)d.WorkTimeAll().TotalMinutes;
                }
                else if (workTime is Illness)//wpisywanie zasiłku do grida
                {
                    Illness i = (Illness)workTime;

                    isHoliday = i.isHolidayOrWeekend();

                    dgv.Rows.Add(i.Date.ToString("d", DateFormat.TakeDateFormatDayFirst()), i.Date.ToString("dddd"), "Zasiłek", ChangeIllnessTypeToString.ChangeToString((IllnessType)i.IdIllnessType), i.WorkTimeAll().ToString(DateFormat.TakeTimeSpanFormat()),
                      i.WorkTime50().ToString(DateFormat.TakeTimeSpanFormat()), isHoliday, i.IdEmployee, WorkType.illness,
                            i.WorkTime100().ToString(DateFormat.TakeTimeSpanFormat()));
                    numberOfMinutesIllness += (int)i.WorkTimeAll().TotalMinutes;
                }
            }
            numberOfMinutesAll = numberOfMinutesWork + numberOfMinutes50 + numberOfMinutes100 + numberOfMinutesDayOff + numberOfMinutesIllness;
            DateTime tempDate;
            bool isFound = false;
            //dodanie dat tam gdzie niema jeszcze wpisów
            for (int i = 1; i <= DateTime.DaysInMonth(date.Year, date.Month); i++)
            {
                isHoliday = false;
                isFound = false;
                tempDate = new DateTime(date.Year, date.Month, i);
                for (int j = 0; j < dgv.RowCount; j++)
                {
                    //sprawda czy szukana data jest już w gridzie
                    if (dgv[0, j].Value.ToString() == tempDate.ToString("d", DateFormat.TakeDateFormatDayFirst()))
                    {
                        isFound = true;
                        continue;
                    }
                }
                //jeżeli nie ma to dodaje
                if (!isFound)
                {
                    if (Holidays.IsHolidayOrWeekend(tempDate.Date))
                    {
                        isHoliday = true;
                    }
                    dgv.Rows.Add(tempDate.ToString("d", DateFormat.TakeDateFormatDayFirst()), tempDate.ToString("dddd"), "", "", "", "", isHoliday, "", "");
                }
            }
        }

        /// <summary>
        /// Metoda zerująca "..." formularz
        /// </summary>
        internal static void ClearLabel(MainForm form)
        {
            form.lblGodzinyPrzepracowane.Text = "...";
            form.lblGodzinyUrlopowePlatne.Text = "...";
            form.lblGodzinyUrlopoweBezplatne.Text = "...";
            form.lblGodzinyChorobowe80.Text = "...";
            form.lblGodzinyChorobowe100.Text = "...";
            form.lblSumaGodzin.Text = "...";
            form.lblIloscGodzinDoPrzepracowania.Text = "...";
            form.lblGodzNadliczbowe.Text = "...";
            form.lblGodzNadliczbowe50.Text = "...";
            form.lblGodzNadliczbowe100.Text = "...";
            form.lblPozostaloUrlopu.Text = "...";

            form.lblZaPrzepracowane.Text = "...";
            form.lblZaNadgodziny.Text = "...";
            form.lblZaNadgodziny50.Text = "...";
            form.lblZaNadgodziny100.Text = "...";
            form.lblZaUrlopowe.Text = "...";
            form.lblZaChorobowe80.Text = "...";
            form.lblZaChorobowe100.Text = "...";
            form.lblRazem.Text = "...";
            form.lblZaliczki.Text = "...";
            form.lblPozyczka.Text = "...";
            form.lblDodatki.Text = "...";
            form.lblPozostaloPozyczki.Text = "...";

            form.lblStawka.Text = "...";
            form.lblStawkaNadgodzinowa.Text = "...";
            form.lblDoWypłaty.Text = "...";
        }
    }
}
