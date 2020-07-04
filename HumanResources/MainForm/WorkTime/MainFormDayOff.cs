using Konfiguracja;
using Logi;
using HumanResources.Exceptions;
using HumanResources.WorkTimeRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HumanResources.MainForm
{
    internal class MainFormDayOff
    {
        internal static bool AssignDayOffData(DayOff dayOff, out int dayCounter, MainForm form)
        {
            dayCounter = 0;
            try
            {
                MainForm.mainDate = Convert.ToDateTime(form.tbDayOffDate.Text);
                //jeżeli urlop wielodniowy
                if (form.chBoxDayOffMulti.Checked)
                {
                    DateTime fromDate = Convert.ToDateTime(form.tbDayOffDate.Text);
                    DateTime toDate = form.dtpDayOff.Value.Date;
                    //oblicza ilość dni urlopowych
                    dayCounter = Convert.ToInt32((toDate - fromDate).Days + 1);//np od 10 - 13 = 3, a ma być 4 (<=13)

                    //zmienne do bazy danych do tabeli urlop
                    dayOff.IdEmployee = Convert.ToInt32(form.cbSelectEmployeeWork.SelectedValue);
                    dayOff.Date = Convert.ToDateTime(form.tbDayOffDate.Text);
                    dayOff.IdTypeDayOff = Convert.ToInt32(form.cbDayOffType.SelectedValue);
                    //sprawdza czy data zakończenia urlopu nie jest mniejsza od daty rozpoczęcia
                    if (fromDate > toDate)
                        throw new WrongDateTimeException("Data rozpoczęcia urlopu jest mniejsza od daty zakończenia.\n Popraw datę i spróbuj ponownie");
                    return true;
                }
                //urlop jednodniowy
                else
                {
                    dayCounter = 1;
                    dayOff.IdEmployee = Convert.ToInt32(form.cbSelectEmployeeWork.SelectedValue);
                    dayOff.Date = Convert.ToDateTime(form.tbDayOffDate.Text);
                    dayOff.IdTypeDayOff = Convert.ToInt32(form.cbDayOffType.SelectedValue);
                    return true;
                }
            }
            catch (WrongDateTimeException ex)
            {
                MessageBox.Show(ex.Message, "Błąd przy wpisywaniu godzin.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (FormatException)
            {
                MessageBox.Show("Wpisałeś niepoprawną datę rozpoczęcia lub zakończenia urlopu.\nPopraw dane i spróbuj ponownie.", "Błąd przy wpisywaniu urlopu.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.urlop, "MainForm.btnDodajGodziny_Click()/n/n" + ex.Message));
                MessageBox.Show(ex.Message, "Błąd przy wpisywaniu urlopu.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        internal static void CheckPossibilityToSaveDayOff(DayOff dayOff, int dayCounter)
        {
            string holidayDescription = "";

            if (dayOff.IsAlreadyInDataBase())
            {
                throw new AlreadyExistsException(string.Format("W dniu {0} pracownik był na urlopie.\nProszę sprawdzić datę i spróbować ponownie.", dayOff.Date.ToShortDateString()));
            }
            else if (Illness.IsAlreadyInDataBase(dayOff.IdEmployee, dayOff.Date))
            {
                throw new AlreadyExistsException(string.Format("W dniu {0} pracownik był na zwolnieniu lekarskim.\nProszę sprawdzić datę i spróbować ponownie.", dayOff.Date.ToShortDateString()));
            }
            else if (Work.IsAlreadyInDataBase(dayOff.IdEmployee, dayOff.Date))
            {
                string temp = string.Format("W dniu {0} pracownik był w pracy.\nCzy napewno dodać urlop do bazy danych?", dayOff.Date.ToShortDateString());
                DialogResult result = MessageBox.Show(temp, "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    throw new CancelException("Anulowano");
            }

            if (dayCounter > 1)//jeżeli wpisywanie wielu dni to exit
                return;

            if (dayOff.Date.DayOfWeek == DayOfWeek.Sunday)
            {
                throw new WrongDateTimeException(string.Format("Podawany dzień {0} jest dniem wolnym od pracy - NIEDZIELA", dayOff.Date.ToShortDateString()));
            }
            else if (dayOff.Date.DayOfWeek == DayOfWeek.Saturday)
            {
                throw new WrongDateTimeException(string.Format("Podawany dzień {0} jest dniem wolnym od pracy - SOBOTA", dayOff.Date.ToShortDateString()));
            }
            else if (Holidays.IsHoliday(dayOff.Date, out holidayDescription))
            {
                throw new WrongDateTimeException(string.Format("W dniu {0} przypada {1}.", dayOff.Date.ToShortDateString(), holidayDescription));
            }
        }
        
        internal static void AddDayOff(DayOff dayOff, int dayCounter)
        {
            string holidayDescription = "";
            Polaczenia.BeginTransaction();
            for (int i = 1; i <= dayCounter; i++)
            {
                if (dayOff.Date.DayOfWeek != DayOfWeek.Sunday && dayOff.Date.DayOfWeek != DayOfWeek.Saturday && !Holidays.IsHoliday(dayOff.Date, out holidayDescription, ConnectionToDB.notDisconnect))
                {
                    WorkManager.AddWorkTime(dayOff, ConnectionToDB.notDisconnect);
                    if (dayOff.IdTypeDayOff == (int)DayOffType.halfDay)
                        dayOff.DayOffSubtraction("0.5", ConnectionToDB.notDisconnect);
                    if (dayOff.IdTypeDayOff == (int)DayOffType.rest)
                        dayOff.DayOffSubtraction("1", ConnectionToDB.notDisconnect);

                    dayOff.Date = dayOff.Date.AddDays(1);//next day
                }
                else
                {
                    dayOff.Date = dayOff.Date.AddDays(1);//next day
                }
            }
            Polaczenia.CommitTransaction();
            //jeżeli wpisuje godziny w ostatni dzień miesiąca to nie przechodzi na kolejny
            int numbersOfDays = DateTime.DaysInMonth(MainForm.mainDate.Year, MainForm.mainDate.Month);
            if (numbersOfDays != MainForm.mainDate.Day)
            {
                //dodaje jeden dzień do aktualnej daty do zmiennej pomocniczej
                MainForm.mainDate = MainForm.mainDate.AddDays(dayCounter);
            }
        }

        internal static void InputWorkTimeData(bool isChecked, MainForm form)
        {
            if (isChecked == true)
            {
                form.tbDayOffDate.Enabled = true;
                form.cbDayOffType.Enabled = true;
                form.chBoxDayOffMulti.Enabled = true;
                //wpisuje date ze zmiennej pomocniczej
                form.tbDayOffDate.Text = MainForm.mainDate.ToShortDateString();
                //ustawia domyślnie wartość urlopu "wypoczynkowy"
                form.cbDayOffType.SelectedIndex = 1;
            }
            else if (isChecked == false)
            {
                form.tbDayOffDate.Enabled = false;
                form.cbDayOffType.Enabled = false;
                form.chBoxDayOffMulti.Enabled = false;
                form.chBoxDayOffMulti.Checked = false;
                form.dtpIllness.Enabled = false;
            }
        }

    }
}
