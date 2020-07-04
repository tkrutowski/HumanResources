using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Konfiguracja;
using Ustawienia.DaneFirmy;
using Logi;
using HumanResources.WorkTimeRecords;
using HumanResources.Exceptions;

namespace HumanResources.Employees.Forms
{
    public partial class ChangeWorkTypeForm : Form
    {
        //true-zmiana godzin
        bool isWorkTimeChange = false;
     
        WorkType changeForWhatWorkType;
        int idEmployee;
        DateTime date;
        WorkType currentWorkType;


        public ChangeWorkTypeForm(DateTime workFrom, DateTime workTo, int id, DateTime date, bool nothing )
        {
            InitializeComponent();
            this.Text = DaneFirmy.NazwaProgramu + "Zmiana godzin.";
            //ikona
            this.Icon = Properties.Resources.logo_firmy;

            lblMainText.Text = "Proszę wybrać dane do wprowadzenia...";
            lblFrom.Visible = true;
            lblTo.Visible = true;
            tbWorkTo.Visible = true;
            tbWorkFrom.Visible = true;
            tbWorkFrom.Text = workFrom.ToShortTimeString();
            tbWorkTo.Text = workTo.ToShortTimeString();
            tbWorkTo.Focus();

            this.idEmployee = id;
            this.date = date;

            isWorkTimeChange = true;
        }

        public ChangeWorkTypeForm(WorkType changeForWhatWorkType, WorkType currentWorkType, int idEmployee, DateTime data)
        {
            InitializeComponent();
            this.Text = DaneFirmy.NazwaProgramu + "Zmiana godzin.";
            this.changeForWhatWorkType = changeForWhatWorkType;
            this.idEmployee = idEmployee;
            this.date = data;
            this.currentWorkType = currentWorkType;

            switch (changeForWhatWorkType)
            {
                case WorkType.work:  //praca
                    lblMainText.Text = "Proszę wybrać dane do wprowadzenia...";
                    lblFrom.Visible = true;
                    lblTo.Visible = true;
                    tbWorkTo.Visible = true;
                    tbWorkFrom.Visible = true;
                    tbWorkFrom.Text = "0700";
                    tbWorkTo.Focus();
                    break;
                case WorkType.illness: //choroba
                    cbType.Visible = true;
                    lblMainText.Text = "Proszę wybrać z poniższej listy rodzaj zasiłku...";
                    BindIllnessType();
                    break;
                case WorkType.dayOff:  //urlop
                    cbType.Visible = true;
                    lblMainText.Text = "Proszę wybrać z poniższej listy rodzaj urlopu...";
                    BindDayOffType();
                    break;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (isWorkTimeChange)
                {
                    Work w = new Work();
                    w.IdEmployee = idEmployee;
                    w.Date = date;
                    w.StartTime = Convert.ToDateTime(tbWorkFrom.Text.ToString());
                    w.StopTime = Convert.ToDateTime(tbWorkTo.Text.ToString());
                    //to sprawdza czy godzina przyjścia jest mniejsza od godziny wyjścia
                    if (w.StartTime.TimeOfDay > w.StopTime.TimeOfDay)
                        throw new WrongDateTimeException("Godzina wyjścia jest mniejsza od godziny przyjścia.");
                    w.Edit(w, ConnectionToDB.disconnect);
                }
                else
                {
                    Polaczenia.BeginTransactionSerializable();

                    //usuwanie zamienianych danych z bazy
                    switch (currentWorkType)
                    {
                        case WorkType.work://praca
                            WorkManager.DeleteWorkTime(WorkType.work, idEmployee, date, ConnectionToDB.notDisconnect);
                            break;
                        case WorkType.illness://choroba
                            WorkManager.DeleteWorkTime(WorkType.illness, idEmployee, date, ConnectionToDB.notDisconnect);
                            break;
                        case WorkType.dayOff://urlop
                            DayOff dayOff = WorkManager.GetDayOff(idEmployee, date, ConnectionToDB.notDisconnect);
                            WorkManager.DeleteWorkTime(WorkType.dayOff, idEmployee, date, ConnectionToDB.notDisconnect);
                            //dodaje urlop do puli urlopów z bazy danych
                            if (dayOff.IdTypeDayOff == (int)DayOffType.halfDay)
                                dayOff.DayOffAddition("0.5", ConnectionToDB.notDisconnect);
                            if (dayOff.IdTypeDayOff == (int)DayOffType.rest)
                                dayOff.DayOffAddition("1", ConnectionToDB.notDisconnect);
                            break;
                    }
                    //Polaczenia.CommitTransaction();
                    //dodawanie nowych danych do bazy
                    switch (changeForWhatWorkType)
                    {
                        case WorkType.work://praca
                            Work w = new Work();
                            w.IdEmployee = idEmployee;
                            w.Date = date;
                            w.StartTime = Convert.ToDateTime(tbWorkFrom.Text.ToString());
                            w.StopTime = Convert.ToDateTime(tbWorkTo.Text.ToString());
                            //to sprawdza czy godzina przyjścia jest mniejsza od godziny wyjścia
                            if (w.StartTime.TimeOfDay > w.StopTime.TimeOfDay)
                                throw new WrongDateTimeException("Godzina wyjścia jest mniejsza od godziny przyjścia.");
                            WorkManager.AddWorkTime(w, ConnectionToDB.notDisconnect);
                            break;
                        case WorkType.illness://choroba
                            Illness illness = new Illness();
                            illness.IdEmployee = idEmployee;
                            illness.Date = date;
                            illness.IdIllnessType = Convert.ToInt32(cbType.SelectedValue);
                            WorkManager.AddWorkTime(illness, ConnectionToDB.notDisconnect);
                            break;
                        case WorkType.dayOff://urlop
                            DayOff dayOff = new DayOff();
                            dayOff.IdEmployee = idEmployee;
                            dayOff.Date = date;
                            dayOff.IdTypeDayOff = Convert.ToInt32(cbType.SelectedValue);
                            if (dayOff.Date.DayOfWeek != DayOfWeek.Sunday && dayOff.Date.DayOfWeek != DayOfWeek.Saturday && !Holidays.IsHoliday(dayOff.Date, ConnectionToDB.notDisconnect))
                            {
                                WorkManager.AddWorkTime(dayOff, ConnectionToDB.notDisconnect);
                                if (dayOff.IdTypeDayOff == (int)DayOffType.halfDay)
                                    dayOff.DayOffSubtraction("0.5", ConnectionToDB.notDisconnect);
                                if (dayOff.IdTypeDayOff == (int)DayOffType.rest)
                                    dayOff.DayOffSubtraction("1", ConnectionToDB.notDisconnect);
                            }
                            else
                            {
                                throw new WrongDateTimeException(string.Format("Podawany dzień {0} jest dniem wolnym od pracy", dayOff.Date.ToShortDateString()));
                            }
                            break;
                    }
                    Polaczenia.CommitTransaction();
                }
            }
            catch (WrongDateTimeException ex)
            {
                MessageBox.Show(ex.Message, "Błąd przy zmianie godzin.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (CancelException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Polaczenia.RollbackTransaction();
                Polaczenia.OdlaczenieOdBazy();
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.praca, "ZamianaRodzajuPracyForm.btnZatwierdz_Click()/n/n" + ex.Message));
            }
            finally
            {
                this.Close();
            }
        }

        /// <summary>
        /// Zaznacza cały tekst po kliknięciu myszką
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbWorFrom_MouseClick(object sender, MouseEventArgs e)
        {
            tbWorkFrom.SelectAll();
        }

        /// <summary>
        /// Zaznacza cały tekst po kliknięciu myszką
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbWorkTo_MouseClick(object sender, MouseEventArgs e)
        {
            tbWorkTo.SelectAll();
        }

        /// <summary>
        /// zamyka formularz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Bindowanie comboboxów
        /// </summary>
        private void BindDayOffType()
        {
            try
            {
                cbType.DataSource = ChangeDayOffTypeToString.ArrayListDayOffType;
                cbType.DisplayMember = "Name";
                cbType.ValueMember = "Id";
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas bindowania danych rodzaju urlopu.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.praca, "BindRodzajUrlopu()/n/n" + ex1.Message));
            }
        }
        /// <summary>
        /// Bindowanie comboboxów
        /// </summary>
        private void BindIllnessType()
        {
            try
            {
                cbType.DataSource = ChangeIllnessTypeToString.ArrayListIllnessType;
                cbType.DisplayMember = "Name";
                cbType.ValueMember = "Id";
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas bindowania danych rodzaju choroby.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.praca, "BindRodzajChoroby()/n/n" + ex1.Message));
            }
        }
    }
}
