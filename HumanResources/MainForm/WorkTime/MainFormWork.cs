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
    internal class MainFormWork
    {
        internal static bool AssignWorkData(Work w, MainForm form)
        {
            try
            {
                w.Date = Convert.ToDateTime(form.tbWorkDate.Text);
                w.IdEmployee = Convert.ToInt32(form.cbSelectEmployeeWork.SelectedValue);
                w.StartTime = Convert.ToDateTime(form.tbWorkTimeFrom.Text.ToString());
                w.StopTime = Convert.ToDateTime(form.tbWorkTimeTo.Text.ToString());
                //to sprawdza czy godzina przyjścia jest mniejsza od godziny wyjścia
                if (w.StartTime.TimeOfDay > w.StopTime.TimeOfDay)
                    throw new WrongDateTimeException("Godzina wyjścia jest mniejsza od godziny przyjścia.");

                return true;
            }
            catch (WrongDateTimeException ex)
            {
                MessageBox.Show(ex.Message, "Błąd przy wpisywaniu godzin.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (FormatException)
            {
                MessageBox.Show("Wpisałeś niepoprawną datę lub godziny rozpoczęcia i zakończenia pracy.\nPopraw dane i spróbuj ponownie.", "Błąd przy wpisywaniu godzin.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.praca, "MainForm.btnGodzinyDodaj_Click()/n/n" + ex.Message));
                MessageBox.Show(ex.Message, "Błąd przy wpisywaniu godzin.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        internal static void CheckPossibilityToSaveWork(Work work)
        {
            string holidayDescription = "";
            //sprawdza, czy był już wpis o podanej dacie w tabeli praca
            if (work.IsAlreadyInDataBase())
            {
                throw new AlreadyExistsException(string.Format("W dniu {0} pracownik był w pracy.\nProszę sprawdzić datę i spróbować ponownie.", work.Date.ToShortDateString()));
            }//sprawdza, czy był już wpis o podanej dacie w tabeli choroba
            else if (Illness.IsAlreadyInDataBase(work.IdEmployee, work.Date))
            {
                string temp = string.Format("W dniu {0} pracownik był na zwolnieniu lekarskim.\nCzy napewno dodać godziny do bazy danych?", work.Date.ToShortDateString());
                DialogResult result = MessageBox.Show(temp, "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    throw new CancelException("Anulowano");
            }
            //jeżeli nie było wpisu to sprawdza następnie tabele urlop
            else if (DayOff.IsAlreadyInDataBase(work.IdEmployee, work.Date))
            {
                string temp = string.Format("W dniu {0} pracownik był na urlopie.\nCzy napewno dodać godziny do bazy danych?", work.Date.ToShortDateString());
                DialogResult result = MessageBox.Show(temp, "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    throw new CancelException("Anulowano");
            }
            else if (Holidays.IsHoliday(work.Date, out holidayDescription))
            {
                string temp = string.Format("W dniu {0} przypada {1}.\nCzy napewno dodać godziny do bazy danych?", work.Date.ToShortDateString(), holidayDescription);
                DialogResult result = MessageBox.Show(temp, "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    throw new CancelException("Anulowano");
            }
        }
        internal static void AddWork(Work work)
        {
            WorkManager.AddWorkTime(work, ConnectionToDB.disconnect);

            //jeżeli wpisuje godziny w ostatni dzień miesiąca to nie przechodzi na kolejny
            int numbersOfDays = DateTime.DaysInMonth(MainForm.mainDate.Year, MainForm.mainDate.Month);
            if (numbersOfDays != MainForm.mainDate.Day)
            {
                //dodaje jeden dzień do aktualnej daty do zmiennej pomocniczej
                MainForm.mainDate = MainForm.mainDate.AddDays(1);
            }
            else
                //ustawia spowrotem na 1 dzień miesiąca
                MainForm.mainDate =  new DateTime(MainForm.mainDate.Year, MainForm.mainDate.Month,1);
        }

        internal static void InputWorkTimeData(bool isChecked, MainForm form)
        {
            if (isChecked == true)
            {
                form.tbWorkDate.Enabled = true;
                form.tbWorkTimeTo.Enabled = true;
                form.tbWorkTimeFrom.Enabled = true;
                form.tbWorkDate.Text = MainForm.mainDate.ToShortDateString();
                form.tbWorkTimeFrom.Text = "0700";
                form.tbWorkTimeTo.Text = "";
                form.tbWorkTimeTo.Focus();
            }
            else if (isChecked == false)
            {
                form.tbWorkDate.Enabled = false;
                form.tbWorkTimeTo.Enabled = false;
                form.tbWorkTimeFrom.Enabled = false;
            }
        }

    }
}
