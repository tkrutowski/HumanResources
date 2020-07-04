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
    internal class MainFormIllness
    {
        internal static bool AssignIllnessData(Illness illness, out int dayCounter, MainForm form)
        {
            dayCounter = 0;
            try
            {
                MainForm.mainDate = Convert.ToDateTime(form.tbIllnessDate.Text);
                //jeżeli wpisywanie wielu dni
                if (form.chBoxIllnessMulti.Checked)
                {
                    DateTime fromDate = form.dtpIllness.Value.Date;
                    DateTime toDate = Convert.ToDateTime(form.tbIllnessDate.Text);
                    dayCounter = Convert.ToInt32((fromDate - toDate).Days + 1);//np od 10 - 13 = 3, a ma być 4 (<=13)

                    illness.IdEmployee = Convert.ToInt32(form.cbSelectEmployeeWork.SelectedValue);
                    illness.Date = Convert.ToDateTime(form.tbIllnessDate.Text);
                    illness.IdIllnessType = Convert.ToInt32(form.cbIllnessType.SelectedValue);
                    if (toDate > fromDate)
                        throw new WrongDateTimeException("Data rozpoczęcia choroby jest mniejsza od daty zakończenia.\n Popraw datę i spróbuj ponownie");
                    return true;
                }
                //jeżeli wpisywanie jednego dnia
                else
                {
                    dayCounter = 1;
                    illness.IdEmployee = Convert.ToInt32(form.cbSelectEmployeeWork.SelectedValue);
                    illness.Date = Convert.ToDateTime(form.tbIllnessDate.Text);
                    illness.IdIllnessType = Convert.ToInt32(form.cbIllnessType.SelectedValue);
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
                MessageBox.Show("Wpisałeś niepoprawną datę rozpoczęcia lub zakończenia choroby.\nPopraw dane i spróbuj ponownie.", "Błąd przy wpisywaniu choroby.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception ex)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.choroba, "MainForm.btnDodajGodziny_Click()/n/n" + ex.Message));
                MessageBox.Show(ex.Message, "Błąd przy wpisywaniu choroby.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        internal static void CheckPossibilityToSaveIllness(Illness illness, int dayCounter)
        {
            //sprawdza, czy był już wpis o podanej dacie w tabeli chobora
            if (illness.IsAlreadyInDataBase())
            {
                throw new AlreadyExistsException(string.Format("W dniu {0} pracownik był na zasiłku.\nProszę sprawdzić datę i spróbować ponownie.", illness.Date.ToShortDateString()));
            }
            else if (DayOff.IsAlreadyInDataBase(illness.IdEmployee, illness.Date))
            {
                throw new AlreadyExistsException(string.Format("W dniu {0} pracownik był na urlopie.\nCzy napewno dodać zasiłek do bazy danych?", illness.Date.ToShortDateString()));
            }
            else if (Work.IsAlreadyInDataBase(illness.IdEmployee, illness.Date))
            {
                throw new AlreadyExistsException(string.Format("W dniu {0} pracownik był w pracy.\nCzy napewno dodać zasiłek do bazy danych?", illness.Date.ToShortDateString()));
            }
        }
   
        internal static void AddIllness(Illness illness, int dayCounter)
        {
            string holidayDescription = "";
            int numbersOfDays = DateTime.DaysInMonth(MainForm.mainDate.Year, MainForm.mainDate.Month);
            Polaczenia.BeginTransactionSerializable();
            for (int i = 1; i <= dayCounter; i++)
            {
                if (illness.Date.DayOfWeek == DayOfWeek.Sunday)
                {
                    string temp = string.Format("Podawany dzień {0} jest dniem wolnym od pracy - NIEDZIELA\n Czy napewno dodać zasiłek do bazy danych?", illness.Date.ToShortDateString());
                    DialogResult result = MessageBox.Show(temp, "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        WorkManager.AddWorkTime(illness, ConnectionToDB.notDisconnect);
                }
                else if (illness.Date.DayOfWeek == DayOfWeek.Saturday)
                {
                    string temp = string.Format("Podawany dzień {0} jest dniem wolnym od pracy - SOBOTA\n Czy napewno dodać zasiłek do bazy danych?", illness.Date.ToShortDateString());
                    DialogResult result = MessageBox.Show(temp, "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        WorkManager.AddWorkTime(illness, ConnectionToDB.notDisconnect);
                }
                //sprawdza czy nie przypada w dzień wolny - święto
                else if (Holidays.IsHoliday(illness.Date, out holidayDescription, ConnectionToDB.notDisconnect))
                {
                    string temp = string.Format("W dniu {0} przypada {1}.\nCzy napewno dodać zasiłek do bazy danych?", illness.Date.ToShortDateString(), holidayDescription);
                    DialogResult result = MessageBox.Show(temp, "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                        WorkManager.AddWorkTime(illness, ConnectionToDB.notDisconnect);
                }
                else
                    WorkManager.AddWorkTime(illness, ConnectionToDB.notDisconnect);

                illness.Date = illness.Date.AddDays(1);//next day                
            }
            Polaczenia.CommitTransaction();
            //jeżeli wpisuje godziny w ostatni dzień miesiąca to nie przechodzi na kolejny           
            if (numbersOfDays != MainForm.mainDate.Day)
            {
                MainForm.mainDate = MainForm.mainDate.AddDays(1);
            }
        }

        internal static void InputWorkTimeData(bool isChecked, MainForm form)
        {
            if (isChecked == true)
            {
                form.tbIllnessDate.Enabled = true;
                form.cbIllnessType.Enabled = true;
                form.chBoxIllnessMulti.Enabled = true;
                form.tbIllnessDate.Text = MainForm.mainDate.ToShortDateString();
                form.cbIllnessType.SelectedIndex = 0;
            }
            else if (isChecked == false)
            {
                form.tbIllnessDate.Enabled = false;
                form.cbIllnessType.Enabled = false;
                form.chBoxIllnessMulti.Enabled = false;
                form.chBoxIllnessMulti.Checked = false;
                form.dtpIllness.Enabled = false;
            }
        }
    }
}
