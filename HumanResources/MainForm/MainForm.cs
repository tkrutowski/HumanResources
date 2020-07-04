using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Konfiguracja;
using Logi;
using Ustawienia.Uprawnienia;
using System.Data.SqlClient;
using Pracownicy.Settings;
using Ustawienia.Uzytkownicy;
using System.Collections;
using Ustawienia.Konfiguracja;
using HumanResources.Loans;
using HumanResources.Loans.Forms;
using HumanResources.WorkTimeRecords;
using HumanResources.Exceptions;
using HumanResources.Employees.Forms;
using HumanResources.Employees;
using HumanResources.EmployeesFinances.Advances;
using HumanResources.EmployeesFinances;
using HumanResources.EmployeesFinances.Additions;
using HumanResources.EmployeesFinances.Forms;
using HumanResources.Employees.Prints;
using HumanResources.Salaries;
using HumanResources.Settings;
using HumanResources.Calendar;
using System.Collections.Generic;
using System.Diagnostics;

namespace HumanResources.MainForm

{
    public partial class MainForm : Form //5600
    {
        Stopwatch stopWatch = new Stopwatch();
        TimeSpan ts = new TimeSpan();
        string elapsedTime = "";


        Blokady blokady = new Blokady();
        EmployeeManager employeeManager = new EmployeeManager();
        Employee employee = new Employee();

        //id do przelączania między zakladkami (przypisuje je pierwsza otwarta zakładka)
        int idEmployeeCB = -1;
        bool doRefreshDgvWork = true;
        public static int progressLoading;

        //zmienna ustawien
        SetEmployee ustawieniaPracownik = new SetEmployee();
        SetLoan ustawieniaPozyczka = new SetLoan();

        //zmienna pomocnicza do zmieniania daty wpisywanych godzin
        public static DateTime mainDate;

        //zmienna do wyświetlania kalendarza
        DateTime calendarDate;
        CalendarManager calendarManager = new CalendarManager();

        public MainForm()
        {
            InitializeComponent();

            //formatowanie grida
            this.dgvEmployee.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvPracownikCellFormatting);
            this.dgvWork.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvWorkCellFormatting);
            this.dgvSalary.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvSalaryCellFormatting);
            this.dgvLoans.CellFormatting += new DataGridViewCellFormattingEventHandler(dgvLoan_CellFormatting);

            //bindowanie cbPracownik
            BindEmployeeWorkTime();//musi być pierwszy
            BindEmployeeAdditions();
            BindEmployeeLoans();
            BindEmployeeSalary();
            BindEmployeeAdvances();
            BindEmployeeStatistics();

            //ukrywanie paneli
            HideAllPanels();

            //blokowanie przycisków
            BlockButtons(false);
        }


        #region NOWY, USUŃ, EDYTUJ

        /// <summary>
        /// Otwiera formularz dodawania nowych pracowników/pożyczek.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNowy_Click(object sender, EventArgs e)
        {
            try
            {
                //jeżeli jest zaznaczony pracownik
                if (btnEmployees.Checked)
                {
                    Employee.correctEmployee = false;
                    //sprawdzenie uprawnien
                    if (Uprawnienie.PracownikForm == 'w')
                    {
                        try
                        {
                            NewEmployeeForm newEmployeeForm = new NewEmployeeForm();
                            newEmployeeForm.ShowDialog();

                            //jeżeli nie było błędów podczas dodawania pracownika do bazy danych
                            //to odświerzanie grida
                            if (Employee.correctEmployee)
                            {
                                RefreshDgvEmployees();
                            }
                        }
                        catch (Exception ex3)
                        {
                            MessageBox.Show(ex3.Message, "Błąd podczas dodawania nowego pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //log
                            LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnPracownikNowy_Click()/n/n" + ex3.Message));
                        }
                    }
                    else
                        throw new UnauthorizedAccessException("Nie masz uprawnień do tworzenia nowych pracowników!");
                }
                else
                    //jeżeli jest zaznaczona pożyczka
                    if (btnLoan.Checked)
                {
                    //sprawdzenie uprawnien
                    if (Uprawnienie.PracownikPozyczkiForm == 'w')
                    {
                        try
                        {
                            Loan.correctLoan = false;
                            LoanNewForm newLoan = new LoanNewForm();
                            newLoan.ShowDialog();
                            //jeżeli nie było błędów podczas dodawania pożyczki do bazy danych
                            //to wyświetlane jest okno z potwierdzeniem
                            if (Loan.correctLoan)
                            {
                                RefreshDgvLoan();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "Dodawanie pożyczki do bazy", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //log
                            LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "MainForm.btnNowy_Click()/n/n" + ex.Message));
                        }
                    }
                    else
                        throw new UnauthorizedAccessException("Nie masz uprawnień do wprowadzania nowych pożczyek!");
                }
                else
                    MessageBox.Show("Musisz najpierw wybrać rodzaj, np. pracownicy.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (NullReferenceException nex)
            {
                MessageBox.Show(nex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Otwiera formularz do edytowania informacji o pracowniku/pożyczce w bazie danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdytuj_Click(object sender, EventArgs e)
        {
            try
            {
                //Edycja pracownika
                if (btnEmployees.Checked)
                {
                    Employee.correctEmployee = false;

                    if (Uprawnienie.PracownikForm == 'w')
                    {
                        try
                        {
                            int id = (int)dgvEmployee.SelectedCells[0].Value;

                            //sprawdzenie czy dany rekord nie jest zablokowany                
                            if (Blokady.SprawdzenieBlokady(NazwaTabeli.pracownik, id) != null)
                            {
                                blokady = Blokady.SprawdzenieBlokady(NazwaTabeli.pracownik, id);
                                throw new ReadOnlyException(string.Format("Dane tego pracownika są właśnie edytowane \nprzez użytkownika: {0}", Uzytkownik.PobieranieNazwyUzytkownika(blokady.KtoBlokuje)));
                            }

                            NewEmployeeForm editEmployeeForm = new NewEmployeeForm(id);
                            editEmployeeForm.ShowDialog();

                            //jeżeli nie było błędów podczas edytowania wpisu pracownika w bazie danych
                            //to odświerzanie grida
                            if (Employee.correctEmployee)
                            {
                                RefreshDgvEmployees();
                            }
                        }
                        catch (NullReferenceException)
                        {
                            MessageBox.Show("Nie możesz edytować danych pracownika ponieważ tabela wyświetlająca pracowników jest pusta.\nZmień opcje wyświetlania lub uzupełnij bazę pracowników.",
                                "Błąd podczas edycji danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            MessageBox.Show("Musisz najpierw wybrać pracownika do edycji.",
                                "Błąd podczas edycji pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex3)
                        {
                            //log
                            LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnPracownikEdytuj_Click()/n/n" + ex3.Message));
                            MessageBox.Show(ex3.Message, "Błąd odczytu z tabeli pracownik", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                        throw new UnauthorizedAccessException("Nie masz uprawnień do edycji danych pracowników!");
                }
                else if (btnLoan.Checked)//pożyczka
                {
                    //sprawdzenie uprawnien
                    if (Uprawnienie.PracownikPozyczkiForm == 'w')
                    {
                        try
                        {
                            Loan.correctLoan = false;
                            int id = Convert.ToInt32(dgvLoans.SelectedCells[0].Value);
                            Payment paid = (Payment)Enum.Parse(typeof(Payment), dgvLoans.SelectedCells[9].Value.ToString());
                            //jeżeli spłacona to blokada edycji
                            if (paid == Payment.paidOff)
                                throw new Exception("Nie można edytować spłaconych pożyczek");

                            //sprawdzenie czy dany rekord nie jest zablokowany                
                            if (Blokady.SprawdzenieBlokady(NazwaTabeli.pozyczka, id) != null)
                            {
                                blokady = Blokady.SprawdzenieBlokady(NazwaTabeli.pozyczka, id);
                                throw new ReadOnlyException(string.Format("Dane tej pożyczki są właśnie edytowane \nprzez użytkownika: {0}", Uzytkownik.PobieranieNazwyUzytkownika(blokady.KtoBlokuje)));
                            }

                            LoanNewForm newLoan = new LoanNewForm(id);
                            newLoan.ShowDialog();
                            if (Loan.correctLoan)
                            {
                                RefreshDgvLoan();
                            }
                        }
                        catch (NullReferenceException)
                        {
                            MessageBox.Show("Nie możesz edytować pożyczki ponieważ tabela wyświetlająca pożyczki jest pusta.\nZmień opcje wyświetlania lub uzupełnij bazę pożyczek.",
                                "Błąd podczas edycji pożyczki.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            MessageBox.Show("Musisz najpierw wybrać pożyczkę do edycji.",
                                "Błąd podczas edycji pożyczki.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex3)
                        {
                            //log
                            LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "MainForm.btnEdytuj_Click()/n/n" + ex3.Message));
                            MessageBox.Show(ex3.Message, "Błąd odczytu z tabeli pożyczki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                        throw new UnauthorizedAccessException("Nie masz uprawnień do edycji pożyczek!");
                }
                else
                    MessageBox.Show("Musisz najpierw wybrać rodzaj, np. pożyczka.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ReadOnlyException ex3)
            {
                MessageBox.Show(ex3.Message, "Blokada rekordu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Usuwanie danych z bazy danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUsun_Click(object sender, EventArgs e)
        {
            try
            {
                //usuwanie pracownika
                if (btnEmployees.Checked)
                {
                    //sprawdzenie uprawnien                
                    if (Uprawnienie.PracownikForm == 'w')
                    {
                        try
                        {
                            employee = employeeManager.GetEmployee((int)dgvEmployee.SelectedCells[0].Value, TableView.view, ConnectionToDB.disconnect);

                            //sprawdzenie czy dany rekord nie jest zablokowany                
                            if (Blokady.SprawdzenieBlokady(NazwaTabeli.pracownik, employee.IdEmployee) != null)
                            {
                                blokady = Blokady.SprawdzenieBlokady(NazwaTabeli.pracownik, employee.IdEmployee);
                                throw new ReadOnlyException(string.Format("Dane tego pracownika są właśnie edytowane \nprzez użytkownika: {0}", Uzytkownik.PobieranieNazwyUzytkownika(blokady.KtoBlokuje)));
                            }

                            string temp = string.Format("Zaznaczono pracownika:\n\n{0} {1}\n {2}\n {3}\n\nCzy napewno chcesz usunąć zaznaczony wpis?",
                                dgvEmployee.SelectedCells[1].Value, dgvEmployee.SelectedCells[2].Value, dgvEmployee.SelectedCells[3].Value,
                                dgvEmployee.SelectedCells[4].Value);
                            DialogResult result = MessageBox.Show(temp, "Usuwanie pracownika", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                //usuwanie
                                Polaczenia.BeginTransactionSerializable();
                                employee.DeleteRateRegular(ConnectionToDB.notDisconnect);
                                employee.DeleteRateOvertime(ConnectionToDB.notDisconnect);
                                employeeManager.Delete(employee.IdEmployee, ConnectionToDB.notDisconnect);
                                Polaczenia.CommitTransaction();

                                //odświerzanie
                                RefreshDgvEmployees();
                            }
                            else
                                MessageBox.Show("Usuwanie anulowane.", "Anulowanie...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (NullReferenceException)
                        {
                            MessageBox.Show("Nie możesz usunąć danych pracownika ponieważ tabela wyświetlająca pracowników jest pusta lub nie jest wybrany żaden pracownik.\nZmień opcje wyświetlania lub uzupełnij bazę pracowników.",
                                "Błąd podczas usuwania danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            MessageBox.Show("Musisz najpierw wybrać pracownika do usunięcia.",
                                "Błąd podczas usuwania pracownika z bazy.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (SqlException ex2)
                        {
                            //log
                            LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnUsun_Click()/n/n" + ex2.Message));
                            string temp = string.Format("Nie możesz usunąć pracownika, który ma wpisane godziny pracy\n lub udzielone pożyczki.");
                            MessageBox.Show(temp, "Błąd SQL podczas usuwania pracownika z bazy danych", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            Polaczenia.RollbackTransaction();
                            Polaczenia.OdlaczenieOdBazy();
                        }
                        catch (Exception ex2)
                        {
                            //log
                            LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnUsun_Click()/n/n" + ex2.Message));
                            MessageBox.Show(ex2.Message, "Błąd podczas usuwania pracownika z bazy danych", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                        throw new UnauthorizedAccessException("Nie masz uprawnień do usuwania danych pracowników!");
                }

                else if (btnLoan.Checked)//pożyczka
                {
                    //sprawdzenie uprawnien
                    if (Uprawnienie.PracownikPozyczkiForm == 'w')
                    {
                        try
                        {
                            Loan.correctLoan = false;
                            int id = Convert.ToInt32(dgvLoans.SelectedCells[0].Value);
                            Loan loan = LoanManager.GetLoan(id);
                            //sprawdzenie czy dany rekord nie jest zablokowany                
                            if (Blokady.SprawdzenieBlokady(NazwaTabeli.pozyczka, id) != null)
                            {
                                blokady = Blokady.SprawdzenieBlokady(NazwaTabeli.pozyczka, id);
                                throw new ReadOnlyException(string.Format("Dane tej pożyczki są właśnie edytowane \nprzez użytkownika: {0}", Uzytkownik.PobieranieNazwyUzytkownika(blokady.KtoBlokuje)));
                            }

                            string temp = string.Format("Zaznaczono pożyczkę nr: {0}\nPracownik: {1}\nNazwa: {2}\nKwota: {3}\nData wystawienia {4}\n\nCzy napewno chcesz usunąć zaznaczony wpis?",
                                dgvLoans.SelectedCells[0].Value, dgvLoans.SelectedCells[2].Value, dgvLoans.SelectedCells[3].Value, dgvLoans.SelectedCells[4].Value,
                                dgvLoans.SelectedCells[5].Value);
                            DialogResult result = MessageBox.Show(temp, "Usuwanie pożyczki", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                            if (result == DialogResult.Yes)
                            {
                                //usuwanie
                                Polaczenia.BeginTransactionSerializable();
                                loan.DeleteAllInstallmentLoan(ConnectionToDB.notDisconnect);
                                LoanManager.DeleteLoan(id, ConnectionToDB.notDisconnect);
                                Polaczenia.CommitTransaction();
                                Loan.correctLoan = true;
                            }
                            else
                                MessageBox.Show("Usuwanie anulowane.", "Anulowanie...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            if (Loan.correctLoan)
                            {
                                MessageBox.Show("Usunięto pożyczke z bazy danych.", "Potwierdzenie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                //odświerzanie grida
                                RefreshDgvLoan();
                            }
                        }
                        catch (NullReferenceException)
                        {
                            MessageBox.Show("Nie możesz usuwać danych o pożyczkach ponieważ tabela wyświetlająca pożyczki jest pusta.\nZmień opcje wyświetlania lub uzupełnij bazę danych.",
                                "Błąd podczas edycji pożyczek pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            MessageBox.Show("Musisz najpierw wybrać pożyczke z tabeli.",
                                "Błąd podczas usuwania pożyczk z bazy.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (System.Data.SqlClient.SqlException)
                        {
                            string temp = string.Format("Nie możesz usunąć pożyczki, którą zaczęto już spłacać.");
                            MessageBox.Show(temp, "Błąd SQL podczas usuwania pożyczki z bazy danych", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex3)
                        {
                            MessageBox.Show(ex3.Message, "Błąd podczas usuwania pożyczki pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //log
                            LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "MainForm.btnUsun_Click()/n/n" + ex3.Message));
                        }
                    }
                    else
                        throw new UnauthorizedAccessException("Nie masz uprawnień do usuwania zadań typu wewnętrzna!");
                }
                else
                    MessageBox.Show("Musisz najpierw wybrać rodzaj zadania, np. przyłącze.", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd autoryzacji", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ReadOnlyException ex3)
            {
                MessageBox.Show(ex3.Message, "Blokada rekordu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion


        #region PRACOWNICY

        /// <summary>
        /// Formatowanie dataGrid pracownicy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvPracownikCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            int dayOffDedlineYellowValue = 3;
            int medicalDedlineYellowValue = 30;
            int bhpDedlineYellowValue = 30;
            //zmienia kolor pracownikow zwolnionych
            bool isHired = Convert.ToBoolean(dgvEmployee[15, e.RowIndex].Value);

            if (!isHired)
            {
                e.CellStyle.BackColor = Color.LightGray;
                e.CellStyle.SelectionBackColor = Color.Gray;
            }
            else
            {
                //oznacza komórki z ujemnym urlopem kolorem czerwonym
                //oznacza komórki z <3 urlopem kolorem żółtym
                if (dgvEmployee.Columns[e.ColumnIndex].Name.Equals("_dgvPr_urlopPozostaly"))
                {
                    float dayOffLeft;
                    if (Single.TryParse(e.Value.ToString(), out dayOffLeft))
                    {

                        if (dayOffLeft <= dayOffDedlineYellowValue)
                        {
                            e.CellStyle.BackColor = Color.Yellow;
                            e.CellStyle.SelectionBackColor = Color.DarkGoldenrod;
                        }
                        if (dayOffLeft < 0)
                        {
                            e.CellStyle.BackColor = Color.Tomato;
                            e.CellStyle.SelectionBackColor = Color.DarkRed;
                            e.CellStyle.SelectionForeColor = Color.White;
                        }
                    }
                }
                //jeżeli data to "0001-01-01" to czyści komórke ""
                if (dgvEmployee.Columns[e.ColumnIndex].Name.Equals("_dgvPr_badLek"))
                {
                    if (e.Value.ToString() != DateTime.MinValue.ToShortDateString())
                    {
                        //jeżeli >30 to żółty, po terminie czarwony
                        TimeSpan dedline = Convert.ToDateTime(e.Value) - DateTime.Now;
                        if ((dedline.Days < medicalDedlineYellowValue) && (dedline.Days >= 0))
                        {
                            e.CellStyle.BackColor = Color.Yellow;
                            e.CellStyle.SelectionBackColor = Color.DarkGoldenrod;
                        }
                        if (Convert.ToDateTime(e.Value) < DateTime.Now.Date)
                        {
                            e.CellStyle.BackColor = Color.Tomato;
                            e.CellStyle.SelectionBackColor = Color.DarkRed;
                            e.CellStyle.SelectionForeColor = Color.White;
                        }
                    }
                }

                if (dgvEmployee.Columns[e.ColumnIndex].Name.Equals("_dgvPr_szkolenieBhp"))
                {
                    if (e.Value.ToString() != DateTime.MinValue.ToShortDateString())
                    {
                        //jeżeli >30 to żółty, po terminie czarwony
                        TimeSpan dedline = Convert.ToDateTime(e.Value) - DateTime.Now;
                        if ((dedline.Days < bhpDedlineYellowValue) && (dedline.Days >= 0))
                        {
                            e.CellStyle.BackColor = Color.Yellow;
                            e.CellStyle.SelectionBackColor = Color.DarkGoldenrod;
                        }
                        if (Convert.ToDateTime(e.Value) < DateTime.Now.Date)
                        {
                            e.CellStyle.BackColor = Color.Tomato;
                            e.CellStyle.SelectionBackColor = Color.DarkRed;
                            e.CellStyle.SelectionForeColor = Color.White;
                        }
                    }
                }
            }

            //zamienia niewpisane daty (minValue) na ""
            if (e.Value.ToString() == DateTime.MinValue.ToString("d", DateFormat.TakeDateFormat()))
                e.Value = "";
        }

        /// <summary>
        /// Odświerzanie grida na podstawie zaznaczenia przycisku CHECKED
        /// </summary>
        void RefreshDgvEmployees()
        {
            if (btnAll.Checked)
            {
                RefreshDgvEmployees(1);
            }
            if (btnHired.Checked)
            {
                RefreshDgvEmployees(2);
            }
            if (btnRealesed.Checked)
            {
                RefreshDgvEmployees(3);
            }
        }

        /// <summary>
        ///Wyświetla tabele pracowników i odblokowuje opcje 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEmployees_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikForm != 'x')
                {
                    UncheckAllButtons();
                    //zaznaczenie pracownika
                    btnEmployees.Checked = true;
                    //wybranie zatrudnionych
                    btnHired.Checked = true;

                    //wyświetlenie panelu pracownicy i schowanie pozostałych   
                    HideAllPanels();
                    panelEmployees.Show();

                    //zminan nazwy przycisków
                    btnAll.Text = "Wszyscy";
                    btnHired.Text = "Zatrudnieni";
                    btnRealesed.Text = "Zwolnieni";

                    //pobieranie ustawień kolumn grida
                    ustawieniaPracownik.GetSettings();
                    SetEmployee.InitialSettings();

                    //zaznaczenie wg ustawień
                    switch (ustawieniaPracownik.OptionDisplay)
                    {
                        case DisplayOptions.realesed:
                            btnRealesed.Checked = true;
                            break;
                        case DisplayOptions.all:
                            btnAll.Checked = true;
                            break;
                        case DisplayOptions.hired:
                            btnHired.Checked = true;
                            break;
                        default:
                            btnHired.Checked = true;
                            break;
                    }

                    //odblokowanie przycisków
                    BlockButtons(true);

                    //odswierzanie grida
                    RefreshDgvEmployees();
                }
                else
                    throw new Exception("Nie masz uprawnień do podglądu danych pracowników!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Pobieranie zamówień wszyscy(1), aktualni(2) i poprzedni(3) do 'dataGridPracownik'
        ///(0) wyswietlanie bez zapytania do bazy 
        /// </summary>
        /// <param name="wybor"></param>
        private void RefreshDgvEmployees(int wybor)
        {
            try
            {
                //zerowanie grida i listy
                dgvEmployee.Rows.Clear();

                switch (wybor)
                {
                    case 0://bez pobierania z bazy
                        break;
                    case 1://wszyscy
                        EmployeeManager.arrayEmployees.Clear();
                        employeeManager.GetEmployeesToList(TableView.view);
                        dgvEmployee.Focus();
                        break;
                    case 2://zatrudnieni
                        EmployeeManager.arrayEmployees.Clear();
                        employeeManager.GetEmployeesHiredRealesedToList(true, TableView.view);
                        dgvEmployee.Focus();
                        break;
                    case 3://zwolnieni
                        EmployeeManager.arrayEmployees.Clear();
                        employeeManager.GetEmployeesHiredRealesedToList(false, TableView.view);
                        dgvEmployee.Focus();
                        break;
                }

                if (EmployeeManager.arrayEmployees.Count > 0)
                {
                    //wyswietlanie w gridzie kolejnych wierszy
                    foreach (Employee p in EmployeeManager.arrayEmployees)
                    {

                        if (Uprawnienie.PracownikFinanse != 'x')
                        {
                            dgvEmployee.Rows.Add(p.IdEmployee, p.FirstName, p.LastName, p.Street, p.City, p.ZipCode,
                                p.RateRegular.IdRate, p.RateRegular.IsMonthlyOrHourly == RateType.monthly ? string.Format("{0:C}/mc", p.RateRegular.RateValue) : string.Format("{0:C}/h", p.RateRegular.RateValue), p.RateRegular.IsMonthlyOrHourly, p.RateOvertime.IdRate,
                                string.Format("{0:C}/h", p.RateOvertime.RateValue),
                                p.NumberDaysOffLeft, p.NumberDaysOffAnnually, p.TelNumber, p.OtherInfo, p.IsHired, p.NextMmedicalExaminationDate.ToString("d", DateFormat.TakeDateFormat()),
                                p.NextBhpTrainingDate.ToString("d", DateFormat.TakeDateFormat()), p.HiredDate.ToString("d", DateFormat.TakeDateFormat()), p.ReleaseDate.ToString("d", DateFormat.TakeDateFormat()), p.EMail);
                        }
                        else
                        {
                            dgvEmployee.Rows.Add(p.IdEmployee, p.FirstName, p.LastName, p.Street, p.City, p.ZipCode,
                                p.RateRegular.IdRate, p.RateRegular.IsMonthlyOrHourly == RateType.monthly ? string.Format("*******/mc") : string.Format("******/h"), p.RateRegular.IsMonthlyOrHourly, p.RateOvertime.IdRate,
                                string.Format("******/h", p.RateOvertime.RateValue),
                                p.NumberDaysOffLeft, p.NumberDaysOffAnnually, p.TelNumber, p.OtherInfo, p.IsHired, p.NextMmedicalExaminationDate.ToString("d", DateFormat.TakeDateFormat()),
                                p.NextBhpTrainingDate.ToString("d", DateFormat.TakeDateFormat()), p.HiredDate.ToString("d", DateFormat.TakeDateFormat()), p.ReleaseDate.ToString("d", DateFormat.TakeDateFormat()), p.EMail);
                        }
                    }
                }

                //TODO szerokość kolumn
                //ustwianie kolejności wyświetlania kolumn oraz sortowanie
                if (SetEmployee.listColumns.Count > 0)
                {
                    string nazwaKolumnySort = "";

                    foreach (Column k in SetEmployee.listColumns)
                    {
                        //dgvGazociag.Columns[k.Nazwa].Visible = k.Widocznosc;
                        //dgvWewnetrzna.Columns[k.Nazwa].Width = k.Szer;                        
                        //dgvGazociag.Columns[k.Nazwa].DisplayIndex = k.Index;

                        if (k.Index == ustawieniaPracownik.SortColumnIndex)
                            nazwaKolumnySort = k.Name;
                    }
                    //sortowanie
                    if (ustawieniaPracownik.SortTypeAscDesc == SortType.ascending)
                    {
                        dgvEmployee.Sort(dgvEmployee.Columns[nazwaKolumnySort], ListSortDirection.Ascending);
                    }
                    else
                        dgvEmployee.Sort(dgvEmployee.Columns[nazwaKolumnySort], ListSortDirection.Descending);
                }

                //czyszczenie wyboru
                dgvEmployee.ClearSelection();
            }
            catch (SqlException ex2)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.OdswierzanieGridaPracownicy()/n/n" + ex2.Message));
                MessageBox.Show(ex2.Message, "Błąd odczytu z tabeli pracownik (SQL)", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex3)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.OdswierzanieGridaPracownikcy()/n/n" + ex3.Message));
                MessageBox.Show(ex3.Message, "Błąd odczytu z tabeli pracownik", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Polaczenia.OdlaczenieOdBazy();
            }
        }


        #endregion
        #region Menu podręczne grida PRACOWNICY
        /// <summary>
        /// Sprawdza czy sa jakieś wpisy w tabeli i włącza menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuEmployee_Opening(object sender, CancelEventArgs e)
        {
            //podświetlenie wiersza ppm
            Point p = dgvEmployee.PointToClient(System.Windows.Forms.Cursor.Position);
            DataGridView.HitTestInfo hti = dgvEmployee.HitTest(p.X, p.Y);

            //jeżeli kliknięto w komórke
            if (hti.RowIndex > -1)
            {
                dgvEmployee.ClearSelection();
                dgvEmployee.Rows[hti.RowIndex].Selected = true;
                //zapisuje nr wiersza
                int rowNr = hti.RowIndex;
                int isHired = Convert.ToInt16(dgvEmployee[15, rowNr].Value);
                //jeżeli nie zatrudniony to umożliwia zatrudnienie
                if (isHired == 0)
                {
                    contextMenuEmployeeMarkAsRealese.Enabled = false;
                    contextMenuEmployeeMarkAsHired.Enabled = true;
                }
                //jeżeli zatrudniony to umożliwia zwolnienie
                if (isHired == 1)
                {
                    contextMenuEmployeeMarkAsHired.Enabled = false;
                    contextMenuEmployeeMarkAsRealese.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Oznacza pracownika jako ZATRUDNIONY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void markAsHiredMenuPracownik_Click(object sender, EventArgs e)
        {
            try
            {
                //jeżeli użytkownik ma uprawnienia do zapisu
                if (Uprawnienie.PracownikForm == 'w')
                {
                    int idEmpl = Convert.ToInt16(dgvEmployee.SelectedCells[0].Value);
                    //oznacza jako zatrudniony
                    employee.EmploymentEdition(idEmpl, true);
                    //odświerzanie grida
                    RefreshDgvEmployees();
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do wprowadzania zmian w danych pracowniczych!");
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Nie możesz oznaczyć pracownika jako zatrudnionego, ponieważ tabela pracowników jest pusta.\n\nSpróbuj innej opcji wyświetlania lub uzupełnij bazę danych.",
                    "Błąd podczas wyświetlania pracowników.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Musisz najpierw wybrać pracownika z tabeli.",
                    "Błąd podczas edycji danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SqlException ex2)
            {
                MessageBox.Show(ex2.Message, "Błąd podczas edycji danych pracownika", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Błąd podczas edycji danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.zaznaczJakoZatrudnionyMenuPracownik_Click()/n/n" + ex2.Message));
            }
        }

        /// <summary>
        /// Oznacza pracownika jako ZWOLNIONY
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void markAsRealeseMenuPracownik_Click(object sender, EventArgs e)
        {
            try
            {
                //jeżeli użytkownik ma uprawnienia do zapisu
                if (Uprawnienie.PracownikForm == 'w')
                {
                    int idEmpl = Convert.ToInt16(dgvEmployee.SelectedCells[0].Value);
                    //oznacza jako zatrudniony
                    employee.EmploymentEdition(idEmpl, false);
                    //odświerzanie grida
                    RefreshDgvEmployees();
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do wprowadzania zmian w danych pracowniczych!");
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Nie możesz oznaczyć pracownika jako zwolnionego, ponieważ tabela pracowników jest pusta.\n\nSpróbuj innej opcji wyświetlania lub uzupełnij bazę danych.",
                    "Błąd podczas wyświetlania pracowników.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Musisz najpierw wybrać pracownika z tabeli.",
                    "Błąd podczas edycji danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SqlException ex2)
            {
                MessageBox.Show(ex2.Message, "Błąd podczas edycji danych pracownika", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Błąd podczas edycji danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.zaznaczJakoZwolnionyMenuPracownik_Click()/n/n" + ex2.Message));
            }
        }
        #endregion


        #region OPCJE WYŚWIETLANIA

        private void btnAll_Click(object sender, EventArgs e)
        {
            //zaznaczanie wszystkie
            btnAll.Checked = true;

            //odznaczanie pozostałych (może być zaznaczone tylko jedno)
            btnHired.Checked = false;
            btnRealesed.Checked = false;

            //odświerzanie grida w przypadku zmiany opcji wyświetlania
            if (btnEmployees.CheckState == CheckState.Checked)
            {
                RefreshDgvEmployees();
            }
            if (btnLoan.CheckState == CheckState.Checked)
            {
                RefreshDgvLoan();
            }
        }

        private void btnHired_Click(object sender, EventArgs e)
        {
            //zaznaczanie aktualni
            btnHired.Checked = true;

            //odznaczanie pozostałych (może być zaznaczone tylko jedno)
            btnAll.Checked = false;
            btnRealesed.Checked = false;

            //odświerzanie grida w przypadku zmiany opcji wyświetlania
            if (btnEmployees.CheckState == CheckState.Checked)
            {
                RefreshDgvEmployees();
            }
            if (btnLoan.CheckState == CheckState.Checked)
            {
                RefreshDgvLoan();
            }
        }

        private void btnRealesed_Click(object sender, EventArgs e)
        {
            //zaznaczanie nieaktualni
            btnRealesed.Checked = true;

            //odznaczanie pozostałych (może być zaznaczone tylko jedno)
            btnHired.Checked = false;
            btnAll.Checked = false;

            //odświerzanie grida w przypadku zmiany opcji wyświetlania
            if (btnEmployees.CheckState == CheckState.Checked)
            {
                RefreshDgvEmployees();
            }
            if (btnLoan.CheckState == CheckState.Checked)
            {
                RefreshDgvLoan();
            }
        }

        #endregion

        #region GODZINY 

        /// <summary>
        /// otwiera formularz z wpisywaniem godzin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGodziny_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikWpisywanieGodzin != 'x')
                {
                    UncheckAllButtons();
                    //zaznaczenie godzin
                    btnGodziny.Checked = true;

                    //wyświetlenie panelu godziny i schowanie pozostałych   
                    HideAllPanels();
                    panelWork.Show();

                    //blokowanie przycisków
                    BlockButtons(false);

                    BindIllnessType();
                    BindDayOffType();

                    //jeżeli nie jest już przypisane to przypisuje
                    if (idEmployeeCB == -1)
                        idEmployeeCB = Convert.ToInt32(cbSelectEmployeeWork.SelectedValue);
                    else
                        //jeżeli jest to przypisuje do cb
                        cbSelectEmployeeWork.SelectedValue = idEmployeeCB;

                    mainDate = dtpWork.Value.Date;

                    //odświerzanie grida
                    RefreshDgvWork(Convert.ToInt32(cbSelectEmployeeWork.SelectedValue), dtpWork.Value.Date);

                    //ustawia zatwierdzanie enterem
                    this.AcceptButton = btnAddWork;
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do wpisywania godzin pracy pracowników!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas wyświetlania godzin.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnGodziny_Click()/n/n" + ex1.Message));
            }
        }

        /// <summary>
        /// formatowanie grida godziny
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvWorkCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //zmienia kolor wierszy w dni wolne
            bool isHoliday = Convert.ToBoolean(dgvWork[6, e.RowIndex].Value);

            if (isHoliday)
            {
                e.CellStyle.BackColor = Color.LightGray;// Gray;// PeachPuff;// LightGreen;
                e.CellStyle.SelectionForeColor = Color.LightGray;// LightGreen;
            }
        }

        /// <summary>
        /// Włącza lub wyłącza edycje pól pracy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbWork_CheckedChanged(object sender, EventArgs e)
        {
            MainFormWork.InputWorkTimeData(rbWork.Checked, this);
        }

        /// <summary>
        /// Włącza lub wyłącza edycje pól urlopowych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbDayOff_CheckedChanged(object sender, EventArgs e)
        {
            MainFormDayOff.InputWorkTimeData(rbDayOff.Checked, this);
        }

        /// <summary>
        /// Włącza lub wyłącza dostęp do wpisywania urlopów wielodniowych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbDayOffTo_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxDayOffMulti.Checked == true)
            {
                dtpDayOff.Enabled = true;
                dtpDayOff.Value = mainDate.Date;
            }
            else if (chBoxDayOffMulti.Checked == false)
                dtpDayOff.Enabled = false;
        }

        /// <summary>
        /// Włącza lub wyłącza edycje pól chorobowe
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rbIllness_CheckedChanged(object sender, EventArgs e)
        {
            MainFormIllness.InputWorkTimeData(rbIllness.Checked, this);
        }

        /// <summary>
        /// Włącza lub wyłącza dostęp do wpisywania zasiłków wielodniowych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbIllnessTo_CheckedChanged(object sender, EventArgs e)
        {
            if (chBoxIllnessMulti.Checked == true)
            {
                dtpIllness.Enabled = true;
                dtpIllness.Value = mainDate.Date;
            }
            else if (chBoxIllnessMulti.Checked == false)
                dtpIllness.Enabled = false;
        }

        /// <summary>
        /// Przy zmianie daty w głównym kalendarzu wpisuje nową date w wybrane textboxy i w główną zmienną daty 
        ///  oraz odświerza grida godziny
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpWork_ValueChanged(object sender, EventArgs e)
        {
            mainDate = dtpWork.Value.Date;
            if (rbWork.Checked)
                tbWorkDate.Text = dtpWork.Value.ToShortDateString();
            else if (rbDayOff.Checked)
                tbDayOffDate.Text = dtpWork.Value.ToShortDateString();
            else if (rbIllness.Checked)
                tbIllnessDate.Text = dtpWork.Value.ToShortDateString();
            if (doRefreshDgvWork)
            {
                RefreshDgvWork(Convert.ToInt32(cbSelectEmployeeWork.SelectedValue), dtpWork.Value.Date);

                //zaznacza date w gridzie
                foreach (DataGridViewRow wiersz in dgvWork.Rows)
                {
                    string temp1 = wiersz.Cells[0].Value.ToString();
                    string temp2 = dtpWork.Value.Date.ToString("d", DateFormat.TakeDateFormatDayFirst());
                    if (temp1 == temp2)
                        dgvWork.Rows[wiersz.Index].Selected = true;
                }
            }
            doRefreshDgvWork = true;
        }

        /// <summary>
        /// Odswierzanie grida godziny przy zmianie wyboru pracownika
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSelectEmployeeWork_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (panelWork.Visible)
            {
                idEmployeeCB = Convert.ToInt32(cbSelectEmployeeWork.SelectedValue);
                //odświerzanie grida
                RefreshDgvWork(idEmployeeCB, dtpWork.Value.Date);
            }
        }

        /// <summary>
        /// Odświerzanie grida GODZINY
        /// </summary>
        /// <param name="idEmployee"></param>
        /// <param name="date"></param>
        private void RefreshDgvWork(int idEmployee, DateTime date)//misiąc+rok     195->
        {
            //zmienne do wyświetlania nadgodzin na bieżąco
            int numberOfMinutesAll = 0;
            int numberOfMinutes50 = 0;
            int numberOfMinutes100 = 0;
            int numberOfMinutesIllness = 0;
            int numberOfMinutesDayOff = 0;
            int numberOfMinutesWork = 0;

            //zerowanie grida i listy
            dgvWork.Rows.Clear();
            WorkManager.arrayListWorkTime.Clear();

            stopWatch.Reset();
            stopWatch.Start();

            //to mierzymy
            //pobieranie danych z bazy i zapisanie ich do listyGodzin
            WorkManager.GetWorkToList(idEmployee, date, ConnectionToDB.notDisconnect);
            WorkManager.GetDayOffToList(idEmployee, date, ConnectionToDB.notDisconnect);
            WorkManager.GetIllnessToList(idEmployee, date, ConnectionToDB.notDisconnect);
            Holidays.GetAll(date);

            stopWatch.Stop();

            // Get the elapsed time as a TimeSpan value.
            ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                     ts.Hours, ts.Minutes, ts.Seconds,
                     ts.Milliseconds / 10);
            Console.WriteLine("Pobieranie z bazy danych pracy, urlopu, choroby " + elapsedTime);


      
            try
            {
                foreach (IWorkTime workTime in WorkManager.arrayListWorkTime)
                {
                    bool isHoliday = false;
                    if (workTime is Work)//wpisywanie godzin pracy do grida
                    {
                        Work w = (Work)workTime;

                        isHoliday = w.isHolidayOrWeekend();

                        dgvWork.Rows.Add(w.Date.ToString("d", DateFormat.TakeDateFormatDayFirst()), w.Date.ToString("dddd"), w.StartTime.ToString("t", DateFormat.TakeTimeFormat()), w.StopTime.ToString("t", DateFormat.TakeTimeFormat()),
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

                        dgvWork.Rows.Add(d.Date.ToString("d", DateFormat.TakeDateFormatDayFirst()), d.Date.ToString("dddd"), "Urlop", ChangeDayOffTypeToString.ChangeToString((DayOffType)d.IdTypeDayOff),
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

                        dgvWork.Rows.Add(i.Date.ToString("d", DateFormat.TakeDateFormatDayFirst()), i.Date.ToString("dddd"), "Zasiłek", ChangeIllnessTypeToString.ChangeToString((IllnessType)i.IdIllnessType), i.WorkTimeAll().ToString(DateFormat.TakeTimeSpanFormat()),
                          i.WorkTime50().ToString(DateFormat.TakeTimeSpanFormat()), isHoliday, i.IdEmployee, WorkType.illness,
                                i.WorkTime100().ToString(DateFormat.TakeTimeSpanFormat()));
                        numberOfMinutesIllness += (int)i.WorkTimeAll().TotalMinutes;
                    }
                }
                numberOfMinutesAll = numberOfMinutesWork + numberOfMinutes50 + numberOfMinutes100 + numberOfMinutesDayOff + numberOfMinutesIllness;
                FillingDgvWorkIfNotComplete();
                //czyszczenie wyboru
                dgvWork.ClearSelection();
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex2)
            {
                MessageBox.Show(ex2.Message);
            }
            catch (Exception ex3)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "PracownikForm.OdswierzanieGridaGodziny()/n/n" + ex3.Message));
                MessageBox.Show(ex3.Message);
            }

            // sortowanie malejące wg daty
            dgvWork.Sort(_dgvE_data, ListSortDirection.Ascending);

            //wpisanie ilości (nad)godzin
            lblHoursToWork.Text = Salary.GetHoursToWork(dtpWork.Value).ToString();
            lblWorkTime.Text = string.Format("{0}:{1:00}", numberOfMinutesWork / 60, numberOfMinutesWork % 60);
            lblWork50.Text = string.Format("{0}:{1:00}", numberOfMinutes50 / 60, numberOfMinutes50 % 60);
            lblWork100.Text = string.Format("{0}:{1:00}", numberOfMinutes100 / 60, numberOfMinutes100 % 60);
            lblWorkIllness.Text = string.Format("{0}:{1:00}", numberOfMinutesIllness / 60, numberOfMinutesIllness % 60);
            lblWorkDayOff.Text = string.Format("{0}:{1:00}", numberOfMinutesDayOff / 60, numberOfMinutesDayOff % 60);
            lblWorkAll.Text = string.Format("{0}:{1:00}", numberOfMinutesAll / 60, numberOfMinutesAll % 60);
        }

        private void FillingDgvWorkIfNotComplete()
        {
            DateTime tempDate;
            bool isFill = false;
            bool isHoliday = false;
            //dodanie dat tam grzie niema jeszcze wpisów
            for (int i = 1; i <= DateTime.DaysInMonth(dtpWork.Value.Year, dtpWork.Value.Month); i++)
            {
                //zerowanie zmiennej
                isHoliday = false;
                isFill = false;
                tempDate = new DateTime(dtpWork.Value.Year, dtpWork.Value.Month, i);
                for (int j = 0; j < dgvWork.RowCount; j++)
                {
                    //sprawda czy szukana data jest już w gridzie
                    if (dgvWork[0, j].Value.ToString() == tempDate.ToString("d", DateFormat.TakeDateFormatDayFirst()))
                    {
                        isFill = true;
                    }
                }
                //jeżeli nie ma to dodaje
                if (!isFill)
                {
                    isHoliday = Holidays.IsHoliday(tempDate.Date);
                    //dni wolne - weekwnd
                    if (tempDate.DayOfWeek == DayOfWeek.Saturday || tempDate.DayOfWeek == DayOfWeek.Sunday)
                    {
                        isHoliday = true;
                    }

                    dgvWork.Rows.Add(tempDate.ToString("d", DateFormat.TakeDateFormatDayFirst()), tempDate.ToString("dddd"), "", "", "", "", isHoliday, "", "");
                }
            }
        }

        /// <summary>
        /// Dodawanie godzin, urlopów i chorobowych do bazy danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddWork_Click(object sender, EventArgs e)//453
        {
            try
            {
                if (Uprawnienie.PracownikWpisywanieGodzin == 'w')
                {
                    DateTime we = DateTime.Now;
                    DateTime wy = DateTime.Now;
                    int dayCounter;
                    //zmienna do odejmowania urlopu z bazy danych
                    int idEmployee = Convert.ToInt32(cbSelectEmployeeWork.SelectedValue);

                    if (rbWork.Checked)//work hours
                    {
                        Work work = new Work();
                        //leżeli dane w polach tekstowych są poprawne
                        if (MainFormWork.AssignWorkData(work, this))
                        {
                            MainFormWork.CheckPossibilityToSaveWork(work);

                            MainFormWork.AddWork(work);
                            
                            //zeruje pole PracaDo, ustawia na nim focus i odświerza textboxPraca
                            tbWorkTimeTo.Text = "";
                            //ustawia pole PracaOd na 7:00
                            tbWorkTimeFrom.Text = "0700";
                            tbWorkTimeTo.Focus();
                            tbWorkDate.Text = mainDate.ToString("d", DateFormat.TakeDateFormatDayFirst());
                        }
                    }

                    if (rbDayOff.Checked)//day off hours
                    {
                        DayOff dayOff = new DayOff();
                        //jeżeli urlop wielodniowy
                        if (MainFormDayOff.AssignDayOffData(dayOff, out dayCounter, this))
                        {
                            for (int i = 1; i <= dayCounter; i++)
                            {
                                MainFormDayOff.CheckPossibilityToSaveDayOff(dayOff, dayCounter);
                                dayOff.Date = dayOff.Date.AddDays(1);//next day
                            }
                            dayOff.Date = Convert.ToDateTime(tbDayOffDate.Text);//reset date

                            MainFormDayOff.AddDayOff(dayOff, dayCounter);
                        }

                        chBoxDayOffMulti.Checked = false;
                        cbSelectEmployeeWork.Focus();
                        tbDayOffDate.Text = mainDate.ToShortDateString();
                    }

                    if (rbIllness.Checked)//illness
                    {
                        Illness illness = new Illness();
                        if (MainFormIllness.AssignIllnessData(illness, out dayCounter, this))
                        {
                            for (int i = 1; i <= dayCounter; i++)
                            {
                                MainFormIllness.CheckPossibilityToSaveIllness(illness, dayCounter);
                                illness.Date = illness.Date.AddDays(1);//next day
                            }
                            illness.Date = Convert.ToDateTime(tbIllnessDate.Text);//reset date

                            MainFormIllness.AddIllness(illness, dayCounter);
                        }

                        chBoxIllnessMulti.Checked = false;
                        cbSelectEmployeeWork.Focus();
                        tbIllnessDate.Text = mainDate.ToShortDateString();
                    }

                    //wyswietlanie bieżącej daty w głównym kalendarzu
                    doRefreshDgvWork = true;
                    dtpWork.Value = mainDate.Date;
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do wpisywania godzin pracy pracowników!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (AlreadyExistsException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (WrongDateTimeException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (CancelException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// Po kliknięciu zaznacza cały tekst
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbSelectAllText_MouseClick(object sender, MouseEventArgs e)
        {
            TextBoxBase tb = sender as MaskedTextBox;
            tb.SelectAll();
        }

        /// <summary>
        /// ustawia dtp DAY wg klikniecia o dgvGodziny
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvWork_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            doRefreshDgvWork = false;
            string temp = dgvWork["_dgvE_data", e.RowIndex].Value.ToString();
            dtpWork.Value = new DateTime(mainDate.Year, mainDate.Month, Convert.ToInt32(temp.Substring(0, 2)));
        }

        #endregion
        #region Menu podręczne grida GODZINY

        /// <summary>
        /// Menu podręczne (contextowe) grida godziny
        /// Włączanie wybranych przycisków w menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuWorkTime_Opening(object sender, CancelEventArgs e)
        {
            try
            {
                //podświetlenie wiersza ppm
                Point p = dgvWork.PointToClient(System.Windows.Forms.Cursor.Position);
                DataGridView.HitTestInfo hti = dgvWork.HitTest(p.X, p.Y);

                //jeżeli kliknięto w komórke
                if (hti.RowIndex > -1)
                {
                    dgvWork.ClearSelection();
                    dgvWork.Rows[hti.RowIndex].Selected = true;
                    doRefreshDgvWork = false;
                    string temp = dgvWork["_dgvE_data", hti.RowIndex].Value.ToString();
                    dtpWork.Value = new DateTime(mainDate.Year, mainDate.Month, Convert.ToInt32(temp.Substring(0, 2)));

                    WorkType rodzaj = (WorkType)Enum.Parse(typeof(WorkType), dgvWork.SelectedCells[8].Value.ToString());

                    switch (rodzaj)
                    {
                        case WorkType.work:
                            contextMenuWorkTimeChangeToWork.Enabled = false;
                            contextMenuWorkTimeChangeToIllness.Enabled = true;
                            contextMenuWorkTimeChangeToDayOff.Enabled = true;
                            contextMenuWorkTimeChangeWorkTime.Enabled = true;
                            break;
                        case WorkType.dayOff:
                            contextMenuWorkTimeChangeToDayOff.Enabled = false;
                            contextMenuWorkTimeChangeToWork.Enabled = true;
                            contextMenuWorkTimeChangeToIllness.Enabled = true;
                            contextMenuWorkTimeChangeWorkTime.Enabled = false;
                            break;
                        case WorkType.illness:
                            contextMenuWorkTimeChangeToIllness.Enabled = false;
                            contextMenuWorkTimeChangeToDayOff.Enabled = true;
                            contextMenuWorkTimeChangeToWork.Enabled = true;
                            contextMenuWorkTimeChangeWorkTime.Enabled = false;
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (ArgumentException)
            {
                contextMenuWorkTimeChangeToDayOff.Enabled = false;
                contextMenuWorkTimeChangeToWork.Enabled = false;
                contextMenuWorkTimeChangeToIllness.Enabled = false;
                contextMenuWorkTimeChangeWorkTime.Enabled = false;
            }
            catch (Exception exm1)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.contextMenuGodziny_Opening()/n/n" + exm1.Message));
                MessageBox.Show(exm1.Message, "Błąd menu Godziny.Open", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Zamiana urlopu lub godzin na zasiłek(chorobe) 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuWorkTimeChangeToIllness_Click(object sender, EventArgs e)
        {
            int rowIndex = (int)dgvWork.SelectedCells[7].RowIndex;
            try
            {
                int id = (int)dgvWork.SelectedCells[7].Value;
                WorkType changeForWhatWorkType = WorkType.illness;
                DateTime data = Convert.ToDateTime(dgvWork.SelectedCells[0].Value);
                WorkType currentWorkType = (WorkType)Enum.Parse(typeof(WorkType), dgvWork.SelectedCells[8].Value.ToString());

                switch (currentWorkType)
                {
                    case WorkType.work:
                        //sprawdzenie czy dany rekord nie jest zablokowany                
                        if (Blokady.SprawdzenieBlokady(NazwaTabeli.praca, id, data.ToShortDateString()) != null)
                        {
                            blokady = Blokady.SprawdzenieBlokady(NazwaTabeli.praca, id, data.ToShortDateString());
                            throw new ReadOnlyException(string.Format("Dane tego pracownika są właśnie edytowane \nprzez użytkownika: {0}", Uzytkownik.PobieranieNazwyUzytkownika(blokady.KtoBlokuje)));
                        }
                        //blokowanie rekordu
                        Blokady.UstawienieBlokady(NazwaTabeli.praca, id, data.ToShortDateString(), Polaczenia.idUser, DateTime.Now);
                        break;
                    case WorkType.dayOff:
                        //sprawdzenie czy dany rekord nie jest zablokowany                
                        if (Blokady.SprawdzenieBlokady(NazwaTabeli.urlop, id, data.ToShortDateString()) != null)
                        {
                            blokady = Blokady.SprawdzenieBlokady(NazwaTabeli.urlop, id, data.ToShortDateString());
                            throw new ReadOnlyException(string.Format("Dane tego pracownika są właśnie edytowane \nprzez użytkownika: {0}", Uzytkownik.PobieranieNazwyUzytkownika(blokady.KtoBlokuje)));
                        }
                        //blokowanie rekordu
                        Blokady.UstawienieBlokady(NazwaTabeli.urlop, id, data.ToShortDateString(), Polaczenia.idUser, DateTime.Now);
                        break;
                }
                ChangeWorkTypeForm changeForm = new ChangeWorkTypeForm(changeForWhatWorkType, currentWorkType, id, data);
                changeForm.ShowDialog();

                //odświerzanie grida
                RefreshDgvWork(id, data);
            }
            catch (ArgumentException)
            {

            }
            catch (ReadOnlyException ex3)
            {
                MessageBox.Show(ex3.Message, "Blokada rekordu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception exmC)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.GodzinyMenuChoroba_Click()/n/n" + exmC.Message));
                MessageBox.Show(exmC.Message, "Błąd menu Godziny.Choroba", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //usuwanie blokady
                Blokady.UsuwanieBlokady(Polaczenia.idUser, NazwaTabeli.urlop);
                Blokady.UsuwanieBlokady(Polaczenia.idUser, NazwaTabeli.praca);
                dgvWork.Rows[rowIndex].Selected = true;
            }
        }

        /// <summary>
        /// Zamiana zasiłku lub godziny na Urlop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuWorkTimeChangeToDayOff_Click(object sender, EventArgs e)
        {
            int rowIndex = (int)dgvWork.SelectedCells[7].RowIndex;
            try
            {
                int id = (int)dgvWork.SelectedCells[7].Value;
                WorkType changeForWhatWorkType = WorkType.dayOff;//urlop
                DateTime date = Convert.ToDateTime(dgvWork.SelectedCells[0].Value);
                WorkType currentWorkType = (WorkType)Enum.Parse(typeof(WorkType), dgvWork.SelectedCells[8].Value.ToString());

                switch (currentWorkType)
                {
                    case WorkType.work:
                        //sprawdzenie czy dany rekord nie jest zablokowany                
                        if (Blokady.SprawdzenieBlokady(NazwaTabeli.praca, id, date.ToShortDateString()) != null)
                        {
                            blokady = Blokady.SprawdzenieBlokady(NazwaTabeli.praca, id, date.ToShortDateString());
                            throw new ReadOnlyException(string.Format("Dane tego pracownika są właśnie edytowane \nprzez użytkownika: {0}", Uzytkownik.PobieranieNazwyUzytkownika(blokady.KtoBlokuje)));
                        }
                        //blokowanie rekordu
                        Blokady.UstawienieBlokady(NazwaTabeli.praca, id, date.ToShortDateString(), Polaczenia.idUser, DateTime.Now);
                        break;
                    case WorkType.illness:
                        //sprawdzenie czy dany rekord nie jest zablokowany                
                        if (Blokady.SprawdzenieBlokady(NazwaTabeli.choroba, id, date.ToShortDateString()) != null)
                        {
                            blokady = Blokady.SprawdzenieBlokady(NazwaTabeli.choroba, id, date.ToShortDateString());
                            throw new ReadOnlyException(string.Format("Dane tego pracownika są właśnie edytowane \nprzez użytkownika: {0}", Uzytkownik.PobieranieNazwyUzytkownika(blokady.KtoBlokuje)));
                        }
                        //blokowanie rekordu
                        Blokady.UstawienieBlokady(NazwaTabeli.choroba, id, date.ToShortDateString(), Polaczenia.idUser, DateTime.Now);
                        break;
                }
                ChangeWorkTypeForm changeForm = new ChangeWorkTypeForm(changeForWhatWorkType, currentWorkType, id, date);
                changeForm.ShowDialog();

                //odświerzanie grida
                RefreshDgvWork(id, date);
            }
            catch (ArgumentException)
            {

            }
            catch (ReadOnlyException ex3)
            {
                MessageBox.Show(ex3.Message, "Blokada rekordu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception exmC)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.godzinyMenuUrlop_Click()/n/n" + exmC.Message));
                MessageBox.Show(exmC.Message, "Błąd menu Godziny.Urlop", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //usuwanie blokady
                Blokady.UsuwanieBlokady(Polaczenia.idUser, NazwaTabeli.praca);
                Blokady.UsuwanieBlokady(Polaczenia.idUser, NazwaTabeli.choroba);
                dgvWork.Rows[rowIndex].Selected = true;
            }
        }

        /// <summary>
        /// Zamiana urlopu lub zasiłku na godziny
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuWorkTimeChangeToWork_Click(object sender, EventArgs e)
        {
            int rowIndex = (int)dgvWork.SelectedCells[7].RowIndex;
            try
            {
                int id = (int)dgvWork.SelectedCells[7].Value;
                WorkType changeForWhatWorkType = WorkType.work;
                DateTime date = Convert.ToDateTime(dgvWork.SelectedCells[0].Value);
                WorkType currentWorkType = (WorkType)Enum.Parse(typeof(WorkType), dgvWork.SelectedCells[8].Value.ToString());

                switch (currentWorkType)
                {
                    case WorkType.dayOff:
                        //sprawdzenie czy dany rekord nie jest zablokowany                
                        if (Blokady.SprawdzenieBlokady(NazwaTabeli.urlop, id, date.ToShortDateString()) != null)
                        {
                            blokady = Blokady.SprawdzenieBlokady(NazwaTabeli.urlop, id, date.ToShortDateString());
                            throw new ReadOnlyException(string.Format("Dane tego pracownika są właśnie edytowane \nprzez użytkownika: {0}", Uzytkownik.PobieranieNazwyUzytkownika(blokady.KtoBlokuje)));
                        }
                        //blokowanie rekordu
                        Blokady.UstawienieBlokady(NazwaTabeli.urlop, id, date.ToShortDateString(), Polaczenia.idUser, DateTime.Now);
                        break;
                    case WorkType.illness:
                        //sprawdzenie czy dany rekord nie jest zablokowany                
                        if (Blokady.SprawdzenieBlokady(NazwaTabeli.choroba, id, date.ToShortDateString()) != null)
                        {
                            blokady = Blokady.SprawdzenieBlokady(NazwaTabeli.choroba, id, date.ToShortDateString());
                            throw new ReadOnlyException(string.Format("Dane tego pracownika są właśnie edytowane \nprzez użytkownika: {0}", Uzytkownik.PobieranieNazwyUzytkownika(blokady.KtoBlokuje)));
                        }
                        //blokowanie rekordu
                        Blokady.UstawienieBlokady(NazwaTabeli.choroba, id, date.ToShortDateString(), Polaczenia.idUser, DateTime.Now);
                        break;
                }

                ChangeWorkTypeForm changeForm = new ChangeWorkTypeForm(changeForWhatWorkType, currentWorkType, id, date);
                changeForm.ShowDialog();

                //odświerzanie grida
                RefreshDgvWork(id, date);
            }
            catch (ArgumentException)
            {

            }
            catch (ReadOnlyException ex3)
            {
                MessageBox.Show(ex3.Message, "Blokada rekordu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception exmC)
            {
                MessageBox.Show(exmC.Message, "Błąd menu Godziny.Praca", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.godzinyMenuPraca_Click()/n/n" + exmC.Message));
            }
            finally
            {
                //usuwanie blokady
                Blokady.UsuwanieBlokady(Polaczenia.idUser, NazwaTabeli.urlop);
                Blokady.UsuwanieBlokady(Polaczenia.idUser, NazwaTabeli.choroba);
                dgvWork.Rows[rowIndex].Selected = true;
            }
        }

        /// <summary>
        /// Usuwanie wpisu z grida
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuWorkTimeDelete_Click(object sender, EventArgs e)
        {
            int rowIndex = (int)dgvWork.SelectedCells[7].RowIndex;
            try
            {
                int idEmployee = (int)dgvWork.SelectedCells[7].Value;
                DateTime date = Convert.ToDateTime(dgvWork.SelectedCells[0].Value);
                WorkType currentWorkType = (WorkType)Enum.Parse(typeof(WorkType), dgvWork.SelectedCells[8].Value.ToString());

                string temp = string.Format("Czy napewno chcesz usunąć zaznaczony wpis?");
                DialogResult result = MessageBox.Show(temp, "Pytanie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    switch (currentWorkType)
                    {
                        case WorkType.work:
                            WorkManager.DeleteWorkTime(WorkType.work, idEmployee, date);
                            RefreshDgvWork(idEmployee, date);
                            break;
                        case WorkType.dayOff:
                            Polaczenia.BeginTransactionSerializable();
                            DayOff dayOff = WorkManager.GetDayOff(idEmployee, date, ConnectionToDB.notDisconnect);
                            WorkManager.DeleteWorkTime(WorkType.dayOff, idEmployee, date, ConnectionToDB.notDisconnect);
                            //dodaje urlop do puli urlopów z bazy danych
                            if (dayOff.IdTypeDayOff == (int)DayOffType.halfDay)
                                dayOff.DayOffAddition("0.5", ConnectionToDB.notDisconnect);
                            if (dayOff.IdTypeDayOff == (int)DayOffType.rest)
                                dayOff.DayOffAddition("1", ConnectionToDB.notDisconnect);
                            Polaczenia.CommitTransaction();
                            RefreshDgvWork(idEmployee, date);
                            break;
                        case WorkType.illness:
                            WorkManager.DeleteWorkTime(WorkType.work, idEmployee, date);
                            RefreshDgvWork(idEmployee, date);
                            break;
                    }
                }
                else
                    MessageBox.Show("Usuwanie anulowane.", "Anulowanie...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exmC)
            {
                MessageBox.Show(exmC.Message, "Błąd menu Usuń", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.godzinyMenuUsun_Click()/n/n" + exmC.Message));
            }
            finally
            {
                dgvWork.Rows[rowIndex].Selected = true;
            }
        }

        /// <summary>
        /// Zmiana godzin przyjścia/wyjścia
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuWorkTimeChangeWork_Click(object sender, EventArgs e)
        {
            int rowIndex = (int)dgvWork.SelectedCells[7].RowIndex;
            try
            {
                int idEmployee = (int)dgvWork.SelectedCells[7].Value;
                DateTime date = Convert.ToDateTime(dgvWork.SelectedCells[0].Value);
                DateTime workFrom = Convert.ToDateTime(dgvWork.SelectedCells[2].Value);
                DateTime workTo = Convert.ToDateTime(dgvWork.SelectedCells[3].Value);

                //sprawdzenie czy dany rekord nie jest zablokowany                
                if (Blokady.SprawdzenieBlokady(NazwaTabeli.praca, idEmployee, date.ToShortDateString()) != null)
                {
                    blokady = Blokady.SprawdzenieBlokady(NazwaTabeli.praca, idEmployee, date.ToShortDateString());
                    throw new ReadOnlyException(string.Format("Dane tego pracownika są właśnie edytowane \nprzez użytkownika: {0}", Uzytkownik.PobieranieNazwyUzytkownika(blokady.KtoBlokuje)));
                }

                //blokowanie rekordu
                Blokady.UstawienieBlokady(NazwaTabeli.praca, idEmployee, date.ToShortDateString(), Polaczenia.idUser, DateTime.Now);

                ChangeWorkTypeForm changeForm = new ChangeWorkTypeForm(workFrom, workTo, idEmployee, date, false);
                changeForm.ShowDialog();

                //odświerzanie grida
                RefreshDgvWork(idEmployee, date);
            }
            catch (ReadOnlyException ex3)
            {
                MessageBox.Show(ex3.Message, "Blokada rekordu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception exmC)
            {
                MessageBox.Show(exmC.Message, "Błąd menu Godziny.Praca", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.praca, "MainForm.godzinyMenu|mienGodziny_Click()/n/n" + exmC.Message));
            }
            finally
            {
                //usuwanie blokady
                Blokady.UsuwanieBlokady(Polaczenia.idUser, NazwaTabeli.praca);
                dgvWork.Rows[rowIndex].Selected = true;
            }
        }
        #endregion
        //650
        #region WYPŁATY 

        #region propertysy

        /// <summary>
        /// Zamienia minuty przepracowane na gg:mm
        /// i wyświetla w textboxie
        /// </summary>
        public string LblGodzinyPrzepracowane
        {
            set
            {
                int temp = Convert.ToInt32(value);
                int godziny = (temp / 60);
                int minuty = (temp % 60);
                this.lblGodzinyPrzepracowane.Text = string.Format("{0}:{1:00}", godziny, minuty);
            }
            get { return this.lblGodzinyPrzepracowane.Text; }
        }
        public double LblZaPozyczke
        {
            set
            {
                lblPozyczka.Text = string.Format("{0:C}", value);
            }
        }
        public string LblGodzinyNadliczbowe
        {
            set
            {
                lblGodzNadliczbowe_0.Enabled = true;
                lblGodzNadliczbowe.Enabled = true;
                lblGodzNadliczbowe_50.Enabled = false;
                lblGodzNadliczbowe50.Enabled = false;
                lblGodzNadliczbowe_100.Enabled = false;
                lblGodzNadliczbowe100.Enabled = false;

                int temp = Convert.ToInt32(value);
                int godziny = (temp / 60);
                int minuty = (temp % 60);
                this.lblGodzNadliczbowe.Text = string.Format("{0}:{1:00}", godziny, minuty);
            }
        }
        public string LblGodzinyNadliczbowe50
        {
            set
            {
                lblGodzNadliczbowe_0.Enabled = false;
                lblGodzNadliczbowe.Enabled = false;
                lblGodzNadliczbowe_50.Enabled = true;
                lblGodzNadliczbowe50.Enabled = true;

                int temp = Convert.ToInt32(value);
                int godziny = (temp / 60);
                int minuty = (temp % 60);
                this.lblGodzNadliczbowe50.Text = string.Format("{0}:{1:00}", godziny, minuty);
            }
        }
        public string LblGodzinyNadliczbowe100
        {
            set
            {
                lblGodzNadliczbowe_0.Enabled = false;
                lblGodzNadliczbowe.Enabled = false;
                lblGodzNadliczbowe_100.Enabled = true;
                lblGodzNadliczbowe100.Enabled = true;

                int temp = Convert.ToInt32(value);
                int godziny = (temp / 60);
                int minuty = (temp % 60);
                this.lblGodzNadliczbowe100.Text = string.Format("{0}:{1:00}", godziny, minuty);
            }
        }
        public string LblGodzinyUrlopowePlatne
        {
            set 
            {
                int temp = Convert.ToInt32(value);
                int godziny = (temp / 60);
                int minuty = (temp % 60);
                this.lblGodzinyUrlopowePlatne.Text = string.Format("{0}:{1:00}", godziny, minuty); 
            }
        }
        public string LblIloscGodzinDoPrzepracowania
        {
            set { this.lblIloscGodzinDoPrzepracowania.Text = value; }
        }
        public string LblGodzinyUrlopoweBezplatne
        {
            set
            {
                int temp = Convert.ToInt32(value);
                int godziny = (temp / 60);
                int minuty = (temp % 60);
                this.lblGodzinyUrlopoweBezplatne.Text = string.Format("{0}:{1:00}", godziny, minuty);
            }
        }
        public string LblGodzinyChorobowe80
        {
            set
            {
                int temp = Convert.ToInt32(value);
                int godziny = (temp / 60);
                int minuty = (temp % 60);
                this.lblGodzinyChorobowe80.Text = string.Format("{0}:{1:00}", godziny, minuty);
            }
        }
        public string LblGodzinyChorobowe100
        {
            set
            {
                int temp = Convert.ToInt32(value);
                int godziny = (temp / 60);
                int minuty = (temp % 60);
                this.lblGodzinyChorobowe100.Text = string.Format("{0}:{1:00}", godziny, minuty);
            }
        }
        public string LblSumaGodzin
        {
            set
            {
                int temp = Convert.ToInt32(value);
                int godziny = (temp / 60);
                int minuty = (temp % 60);
                this.lblSumaGodzin.Text = string.Format("{0}:{1:00}", godziny, minuty);
            }
        }
        public float LblPozostaloUrlopu
        {
            set { this.lblPozostaloUrlopu.Text = value.ToString(); }
        }
        public double LblZaUrlopowe
        {
            set { this.lblZaUrlopowe.Text = string.Format("{0:C}", value); }
        }
        public double LblZaDodatek
        {
            set { this.lblDodatki.Text = string.Format("{0:C}", value); }
        }
        public double LblZaZaliczke
        {
            set { this.lblZaliczki.Text = string.Format("{0:C}", value); }
        }
        public double LblPozostaloPozyczki
        {
            set
            {
                lblPozostaloPozyczki.Text = string.Format("{0:C}", value);
            }
        }
        public double LblZaChorobowe80
        {
            set { this.lblZaChorobowe80.Text = string.Format("{0:C}", value); }
        }
        public double LblZaChorobowe100
        {
            set { this.lblZaChorobowe100.Text = string.Format("{0:C}", value); }
        }
        public double LblZaGodziny
        {
            set { this.lblZaPrzepracowane.Text = string.Format("{0:C}", value); }
        }
        public double LblZaNadgodziny
        {
            set
            {
                lblNadgodziny_0.Enabled = true;
                lblZaNadgodziny.Enabled = true;
                lblNadgodziny_50.Enabled = false;
                lblZaNadgodziny50.Enabled = false;
                lblNadgodziny_50_1.Enabled = false;
                lblZaNadgodziny100.Enabled = false;
                this.lblZaNadgodziny.Text = string.Format("{0:C}", value);
            }
        }
        public double LblZaNadgodziny50
        {
            set
            {
                lblNadgodziny_0.Enabled = false;
                lblZaNadgodziny.Enabled = false;
                lblNadgodziny_50.Enabled = true;
                lblZaNadgodziny50.Enabled = true;
                this.lblZaNadgodziny50.Text = string.Format("{0:C}", value);
            }
        }
        public double LblZaNadgodziny100
        {
            set
            {
                lblNadgodziny_0.Enabled = false;
                lblZaNadgodziny.Enabled = false;
                lblNadgodziny_50_1.Enabled = true;
                lblZaNadgodziny100.Enabled = true;
                this.lblZaNadgodziny100.Text = string.Format("{0:C}", value);
            }
        }
        public double LblZaWszystko
        {
            set { this.lblRazem.Text = string.Format("{0:C}", value); }
        }
        public double LblStawka
        {
            set
            {
                if (Uprawnienie.PracownikFinanse != 'x')
                    this.lblStawka.Text = string.Format("{0:C}", value);
                else
                    this.lblStawka.Text = string.Format("*****");
            }
        }
        public double LblStawkaNadgodzinowa
        {
            set
            {
                if (Uprawnienie.PracownikFinanse != 'x')
                    this.lblStawkaNadgodzinowa.Text = string.Format("{0:C}", value);
                else
                    this.lblStawkaNadgodzinowa.Text = string.Format("*****");
            }
        }
        public double LblDoWyplaty
        {
            set
            {
                if (Uprawnienie.PracownikFinanse != 'x')
                    this.lblDoWypłaty.Text = string.Format("{0:C}", value);
                else
                    this.lblDoWypłaty.Text = string.Format("*****");
            }
        }
        #endregion

        /// <summary>
        /// Wyświetla panel obliczania wypłat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSalary_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikObliczanieWyplatForm != 'x')
                {
                    UncheckAllButtons();
                    btnSalary.Checked = true;

                    HideAllPanels();
                    panelSalary.Show();

                    //blokowanie przycisków
                    BlockButtons(false);

                    //ustawia date na 1, zeby podczas przesuwania strzałakami 31->30 i error
                    dtpSalary.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                    //jeżeli nie jest już przypisane to przypisuje
                    if (idEmployeeCB == -1)
                        idEmployeeCB = Convert.ToInt32(cbSelectEmployeeSalary.SelectedValue);
                    else
                        //jeżeli jest to przypisuje do cb
                        cbSelectEmployeeSalary.SelectedValue = idEmployeeCB;
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do wpisywania godzin pracy pracowników!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas wyświetlania wypłat.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnWyplaty_Click()/n/n" + ex1.Message));
            }
        }

        /// <summary>
        /// formatowanie grida godziny
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvSalaryCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //zmienia kolor wierszy w dni wolne
            bool czyWolne = Convert.ToBoolean(dgvSalary[6, e.RowIndex].Value);

            if (czyWolne)
            {
                e.CellStyle.BackColor = Color.LightGray;// Gray;// PeachPuff;// LightGreen;
                e.CellStyle.SelectionForeColor = Color.LightGray;// LightGreen;
            }
        }

        /// <summary>
        /// Wypisuje dane z obliczeń w formularzu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalculateSalary_Click(object sender, EventArgs e)
        {
            try
            {
                //jezeli przed maj2018
                if (dtpSalary.Value.Date < new DateTime(2018, 5, 1))
                {
                    CalculateSalary.Calculate_OLD(this, Convert.ToInt32(cbSelectEmployeeSalary.SelectedValue), dtpSalary.Value.Date);
                }
                //jeżeli po maj 2018 (nadgodziny 50%)
                else
                {
                    CalculateSalary.Calculate(this, Convert.ToInt32(cbSelectEmployeeSalary.SelectedValue), dtpSalary.Value.Date);
                }
                //odswierzanie grida
                RefreshDgvSalary(Convert.ToInt32(cbSelectEmployeeSalary.SelectedValue), dtpSalary.Value.Date);

                //włącza możliwość wydruku karty pracy
                btnSalaryWorkCardPrint.Enabled = true;
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex1)
            {
                MessageBox.Show(ex1.Message, "Brak danych.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas obliczania wypłat.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnObliczWyplate_Click()/n/n" + ex1.Message));
            }
        }        

        /// <summary>
        /// Odświerzanie grida GODZINY
        /// </summary>
        /// <param name="idEmployee"></param>
        /// <param name="date"></param>
        private void RefreshDgvSalary(int idEmployee, DateTime date)
        {
            try
            {
                MainFormSalary.RefreshDgv(idEmployee, date, dgvSalary);
                // sortowanie malejące wg daty
                dgvSalary.Sort(_dgv0_data, ListSortDirection.Ascending);
            }
            catch (Exception ex3)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.RefreshDgvSalary()/n/n" + ex3.Message));
                MessageBox.Show(ex3.Message);
            }
        }

        private void cbSelestEmployeeSalary_SelectedIndexChanged(object sender, EventArgs e)
        {
            //zerowanie obliczen
            MainFormSalary.ClearLabel(this);
            //blokowanie możliwości wydruku karty godzin
            btnSalaryWorkCardPrint.Enabled = false;
            //zerowanie grida
            dgvSalary.Rows.Clear();
            idEmployeeCB = Convert.ToInt32(cbSelectEmployeeSalary.SelectedValue);
        }

        #endregion


        #region POZYCZKI_SPŁATY

        #region PROPERTYSY


        public double LblLoanAmount
        {
            set
            {
                lblLoanAmount.Text = string.Format("{0:C}", value);
            }
        }
        public double LblLoanInstallment
        {
            set
            {
                lblLoanInstallment.Text = string.Format("{0:C}", value);
            }
        }
        public double LblLoanInstallmentPaid
        {
            set
            {
                lblLoanInstallmentPaid.Text = string.Format("{0:C}", value);
            }
        }
        public double LblLoanToPay
        {
            set
            {
                lblLoanToPay.Text = string.Format("{0:C}", value);
            }
        }

        #endregion

        /// <summary>
        /// Wyświetla panel wpisywania rat
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoanPayment_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikWpisywaniePozyczek != 'x')
                {
                    UncheckAllButtons();
                    btnLoanPayment.Checked = true;

                    //wyświetlenie panelu pożyczki spłaty i schowanie pozostałych   
                    HideAllPanels();
                    panelPozyczkiSplaty.Show();

                    //blokowanie przycisków
                    BlockButtons(false);

                    //jeżeli nie jest już przypisane to przypisuje
                    if (idEmployeeCB == -1)
                        idEmployeeCB = Convert.ToInt32(cbLoanSelectEmployee.SelectedValue);
                    else
                        //jeżeli jest to przypisuje do cb
                        cbLoanSelectEmployee.SelectedValue = idEmployeeCB;

                    LoanManager.GetLoanIsPaidToList(Convert.ToInt32(cbLoanSelectEmployee.SelectedValue), Payment.toPay);
                    //jeżeli są pożyczki
                    if (LoanManager.arrayLoans.Count > 0)
                    {
                        //odblokowanie przycisku 'wyświetl szczególy' i dodaj                     
                        btnLoanDisplayPayment.Enabled = true;
                        btnLoanAdd.Enabled = true;

                        //odswierzanie cbWyborPozyczki
                        RefreshCbSelectLoan();

                        //przyjmuje pierwsza wartosc z listy
                        cbLoanSelectLoan.SelectedIndex = 0;
                    }
                    //jezeli nie ma pożyczek                                 
                    else
                    {
                        //przyjmuje ujemną wartosc w celu wyzerowania szczegółow ...
                        cbLoanSelectLoan.SelectedIndex = -1;
                        //blokujemy przycisk  dodawania i wyświetlania rat
                        btnLoanAdd.Enabled = false;
                        btnLoanDisplayPayment.Enabled = false;
                        cbLoanSelectLoan.Enabled = false;

                        RefreshLoanDetail();
                    }
                    mainDate = dtpLoanPaymentDate.Value.Date;

                    //odświerzanie grida
                    RefreshDgvLeanInstallment(Convert.ToInt32(cbLoanSelectEmployee.SelectedValue), dtpLoanPaymentDate.Value.Date);

                    //ustawia zatwierdzanie enterem
                    this.AcceptButton = btnLoanAdd;
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do wpisywania rat pożyczek pracowników!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas wyświetlania pożyczek.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnWPozyczkiSplaty_Click()/n/n" + ex1.Message));
            }
        }

        /// <summary>
        /// Metoda sprawdza czy dany pracownik ma jakieś niespłacone pożyczki.
        /// Jeżeli ma to wyświetla je w cbWyborPozyczki, jeżeli nie to blokuje cb
        /// oraz przycisk Dodaj, oraz wyświetl szczegóły
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbLoanSelectEmployee_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (panelPozyczkiSplaty.Visible)
            {
                idEmployeeCB = Convert.ToInt32(cbLoanSelectEmployee.SelectedValue);
                //odswierzanie cbWyborPozyczki
                RefreshCbSelectLoan();
                //odświerzanie grida
                RefreshDgvLeanInstallment(Convert.ToInt32(cbLoanSelectEmployee.SelectedValue), dtpLoanPaymentDate.Value.Date);
            }
        }

        /// <summary>
        /// Metoda wypisująca szczegóły pożyczki przy zmianie wyboru pożyczki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbLoanSelectLoan_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshLoanDetail();
        }

        /// <summary>
        /// Dodaje nową wpłate do bazy danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoanAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (Uprawnienie.PracownikWpisywaniePozyczek == 'w')
                {
                    if (tbLoanPaymentAmount.Text.Trim() == "")
                        throw new EmptyStringException("Musisz wypełnić pole \"Kwota wpłaty\".");

                    Loan loan = LoanManager.GetLoan(((Loan)cbLoanSelectLoan.SelectedItem).IdLoan);
                    LoanInstallment loanInstallment = new LoanInstallment();
                    loanInstallment.IdLoan = loan.IdLoan;
                    loanInstallment.Date = dtpLoanPaymentDate.Value.Date;
                    loanInstallment.InstallmentAmount = Convert.ToSingle(tbLoanPaymentAmount.Text.Replace('.', ','));
                    //oznacza rate jaki nie wplate wlasną i zostanie odciągnięta od wypłaty
                    loanInstallment.IsOwnRepeyment = false;

                    if (loan.PayInstallment(loanInstallment, loan))
                    {
                        RefreshCbSelectLoan();
                        RefreshLoanDetail();
                        RefreshDgvLeanInstallment(Convert.ToInt32(cbLoanSelectEmployee.SelectedValue), dtpLoanPaymentDate.Value);
                        tbLoanPaymentAmount.Text = "";
                    }
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do wpisywania spłat (rat) porzyczek pracowników!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (EmptyStringException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Musisz podać kwotę oddzieloną przecinkiem (np. 120,80).", "Błędne dane, popraw i spróbuj ponownie", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Błędne dane, popraw i spróbuj ponownie", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "MainForm.btnPozyczkaDodaj_Click()/n/n" + ex2.Message));
            }
        }

        /// <summary>
        /// Otwiera formularz który wyświetla wszystkie spłaty danej pożyczki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoanDisplayDetails_Click(object sender, EventArgs e)
        {
            Loan loan = LoanManager.GetLoan(((Loan)cbLoanSelectLoan.SelectedItem).IdLoan);
            LoanInstallmentDetailsForm loanDetailsForm = new LoanInstallmentDetailsForm(loan);
            loanDetailsForm.ShowDialog();
        }

        /// <summary>
        /// Metoda pobierająca wpłaty wszystkich pożyczek danego pracownika na dany miesiąc
        /// </summary>
        /// <param name="idEmployee">id pracownika</param>
        /// <param name="date">data</param>
        private void RefreshDgvLeanInstallment(int idEmployee, DateTime date)//misiąc+rok
        {
            dgvLoanPayment.Rows.Clear();
            //pobieranie wszystkich wpłat pracownika w danym miesiącu do listy
            Loan loan = new Loan();
            Loan loanForName = new Loan();
            float sumInstallment = 0;
            loan.GetListInstallmentLoan(idEmployee, date);
            try
            {
                foreach (LoanInstallment li in loan.ArrayInstallmentLoan)
                {
                    loanForName = LoanManager.GetLoan(li.IdLoan);
                    dgvLoanPayment.Rows.Add(li.Date.ToShortDateString(), string.Format("{0:C}", li.InstallmentAmount), loanForName.Name,
                        li.IdLoanInstallment.ToString(), //potrzebne do usuwania wierszy
                        li.IdLoan.ToString());//potrzebne do sprawdzania przy usuwaniu raty czy pozyczka jest splacona
                    sumInstallment += li.InstallmentAmount;
                }
                //jeżeli są jakieś wpisy w gridzie
                if (dgvLoanPayment.RowCount > 0)
                    dgvLoanPayment.Rows.Add("RAZEM:", string.Format("{0:C}", sumInstallment), "");
                //czyszczenie wyboru
                dgvLoanPayment.ClearSelection();
            }
            catch (Exception ex3)
            {
                MessageBox.Show(ex3.Message, "Odświerzanie grida pożyczki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "MainForm.OdswierzanieGridaRatyPozyczki()/n/n" + ex3.Message));
            }
        }

        /// <summary>
        /// Odświerzanie grida pozyczek po zmianie daty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpLoanPaymentDate_ValueChanged(object sender, EventArgs e)
        {
            RefreshDgvLeanInstallment(Convert.ToInt32(cbLoanSelectEmployee.SelectedValue), dtpLoanPaymentDate.Value);
        }

        /// <summary>
        /// Odświerza groupBox Szczegóły pożyczki
        /// </summary>
        private void RefreshLoanDetail()
        {
            Loan loan;
            if (cbLoanSelectLoan.SelectedIndex != -1)
            {
                int id = ((Loan)cbLoanSelectLoan.SelectedItem).IdLoan;
                loan = LoanManager.GetLoan(id);
                lblLoanName.Text = loan.Name;
                lblLoanOtherInfo.Text = loan.OtherInfo;
                lblLoanDate.Text = loan.Date.ToShortDateString();
                LblLoanAmount = loan.Amount;
                LblLoanInstallment = loan.InstallmentLoan;
                double sumaPaidInstallmentLoans = loan.GetSumPaidInstallmentLoan();
                LblLoanInstallmentPaid = sumaPaidInstallmentLoans;
                LblLoanToPay = loan.Amount - sumaPaidInstallmentLoans;
            }
            else//jeżeli nie ma pożyczek zeruje wyswietlanie
            {
                lblLoanName.Text = "...";
                lblLoanOtherInfo.Text = "...";
                lblLoanDate.Text = "...";
                lblLoanAmount.Text = "...";
                lblLoanInstallment.Text = "...";
                lblLoanInstallmentPaid.Text = "...";
                lblLoanToPay.Text = "...";
            }
        }

        /// <summary>
        /// Odświerzanie cbWyborPozyczki po spłaceniu całkowitym pożyczki
        /// </summary>
        private void RefreshCbSelectLoan()
        {
            //uzupełnienie cbPozyczki
            LoanManager.GetLoanIsPaidToList(Convert.ToInt32(cbLoanSelectEmployee.SelectedValue), Payment.toPay);
            //jeżeli są pożyczki
            if (LoanManager.arrayLoans.Count > 0)
            {
                //odblokowujemy przycisk dodawania (jeżęli są uprawnienia) i wyświetlania rat
                if (Uprawnienie.PracownikWpisywaniePozyczek == 'w')
                    btnLoanAdd.Enabled = true;
                btnLoanDisplayPayment.Enabled = true;
                cbLoanSelectLoan.Enabled = true;

                cbLoanSelectLoan.DataSource = null;
                cbLoanSelectLoan.DataSource = LoanManager.arrayLoans;
                cbLoanSelectLoan.DisplayMember = "Name";
                cbLoanSelectLoan.ValueMember = "IdLoan";

                //przyjmuje pierwsza wartosc z listy
                cbLoanSelectLoan.SelectedIndex = 0;
            }                                 
            else
            {
                cbLoanSelectLoan.SelectedIndex = -1;
                //blokujemy przycisk dodawania i wyświetlania rat
                btnLoanAdd.Enabled = false;
                btnLoanDisplayPayment.Enabled = false;
                cbLoanSelectLoan.Enabled = false;
            }
        }
        #endregion
        #region Menu podręczne grida POŻYCZKI

        /// <summary>
        /// Zachodzi podczas otwierania menu podręcznego grida Pozyczki
        /// Sprawdza czy są wpisy w gridzie pożyczki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuLoanPayment_Opening(object sender, CancelEventArgs e)
        {
            //podświetlenie wiersza ppm
            Point p = dgvLoanPayment.PointToClient(System.Windows.Forms.Cursor.Position);
            DataGridView.HitTestInfo hti = dgvLoanPayment.HitTest(p.X, p.Y);

            //jeżeli kliknięto w komórke
            if ((hti.RowIndex > -1) && (hti.RowIndex < dgvLoanPayment.RowCount - 1))
            {
                dgvLoanPayment.ClearSelection();
                dgvLoanPayment.Rows[hti.RowIndex].Selected = true;
            }
            else
            {
                dgvLoanPayment.ClearSelection();
                contextMenuLoanPayment.Enabled = false;
            }
        }

        /// <summary>
        /// Zmiana daty pożyczki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loanPaymentChangeDate_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikWpisywaniePozyczek == 'w')
                {
                    LoanInstallment.correctLoanInstallment = false;
                    LoanInstallment loanInstallment = Loan.GetInstallmentLoan(Convert.ToInt32(dgvLoanPayment.SelectedCells[3].Value));
                    //otwiera formularz do zmiany kwoty
                    ChangeAmountOrDateForm changeAmountOrDateForm = new ChangeAmountOrDateForm(loanInstallment, false, true);
                    changeAmountOrDateForm.ShowDialog();
                    //jeżeli edycja udana to wyświetlane jest potwierdzenie i odświerzany grid
                    if (LoanInstallment.correctLoanInstallment)
                    {
                        MessageBox.Show("Zmieniono datę wpłaty raty pożyczki.", "Potwierdzenie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshDgvLeanInstallment(Convert.ToInt32(cbLoanSelectEmployee.SelectedValue), dtpLoanPaymentDate.Value.Date);
                    }
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do edycji rat!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd podczas aktualizacji raty pożyczki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "MainForm.pozyczkiMenuZmienDate_Click()/n/n" + ex.Message));
            }
        }

        /// <summary>
        /// Zmiana kwoty wpłaty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loanPaymentChangeAmount_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikWpisywaniePozyczek == 'w')
                {
                    LoanInstallment.correctLoanInstallment = false;
                    LoanInstallment loanInstallment = Loan.GetInstallmentLoan(Convert.ToInt32(dgvLoanPayment.SelectedCells[3].Value));
                    //otwiera formularz do zmiany kwoty
                    ChangeAmountOrDateForm changeAmountOrDateForm = new ChangeAmountOrDateForm(loanInstallment, true, false);
                    changeAmountOrDateForm.ShowDialog();
                    //jeżeli edycja udana to wyświetlane jest potwierdzenie i odświerzany grid
                    if (LoanInstallment.correctLoanInstallment)
                    {
                        MessageBox.Show("Zmieniono kwotę wpłaty raty pożyczki.", "Potwierdzenie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshDgvLeanInstallment(Convert.ToInt32(cbLoanSelectEmployee.SelectedValue), dtpLoanPaymentDate.Value.Date);
                    }
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do edycji rat!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd podczas aktualizacji raty pożyczki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "MainForm.pozyczkiMenuZmienKwote_Click()/n/n" + ex.Message));
            }
        }

        /// <summary>
        /// Usuwanie wpisu z bazy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loanPaymentDelete_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikWpisywaniePozyczek == 'w')
                {
                    //sprawdzenie czy pozyczka jest splacona, jeżeli była to wyjątek
                    Loan loan = LoanManager.GetLoan(Convert.ToInt32(dgvLoanPayment.SelectedCells[4].Value));
                    if (loan.IsPaid == Payment.paidOff)
                        throw new ErrorException("Nie możesz usuwać rat z pożyczki, która została spłacona.");

                    //usuwanie raty z bazy danych
                    Loan.DeleteInstallmentLoan(Convert.ToInt32(dgvLoanPayment.SelectedCells[3].Value), ConnectionToDB.disconnect);

                    MessageBox.Show("Usunięto ratę pożyczki z bazy danych.", "Potwierdzenie", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //odświerzanie
                    RefreshDgvLeanInstallment(Convert.ToInt32(cbLoanSelectEmployee.SelectedValue), dtpLoanPaymentDate.Value);
                    RefreshLoanDetail();
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do edycji rat!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (ErrorException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex4)
            {
                MessageBox.Show(ex4.Message, "Błąd podczas usuwanie wpisu.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "MainForm.pozyczkiMenuUsunWpis_Click()/n/n" + ex4.Message));
            }
        }

        #endregion


        #region ZALICZKI

        /// <summary>
        /// Propertys
        /// </summary>
        float TbAdvanceAmount
        {
            set
            {
                tbAdcanceAmount.Text = string.Format("{0:N}", value);
            }
        }

        private void btnAdvance_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikWpisywanieZaliczek != 'x')
                {
                    UncheckAllButtons();
                    btnAdvance.Checked = true;

                    HideAllPanels();
                    panelAdvances.Show();

                    //blokowanie przycisków
                    BlockButtons(false);

                    //jeżeli nie jest już przypisane to przypisuje
                    if (idEmployeeCB == -1)
                        idEmployeeCB = Convert.ToInt32(cbAdvanceSelectEmployee.SelectedValue);
                    else
                        //jeżeli jest to przypisuje do cb
                        cbAdvanceSelectEmployee.SelectedValue = idEmployeeCB;

                    //odświerzanie grida
                    RefreshDgvAdvances(Convert.ToInt32(cbAdvanceSelectEmployee.SelectedValue), dtpAdvance.Value.Date);

                    //ustawia accept button na przycisku dodaj zaliczki
                    this.AcceptButton = this.btnAdvenceAdd;
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do wpisywania zaliczek pracowników!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas wyświetlania zaliczek.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnWZaliczki_Click()/n/n" + ex1.Message));
            }
        }


        /// <summary>
        /// Dodawanie zaliczek do bazy danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdvanceAdd_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikWpisywanieZaliczek == 'w')
                {
                    Advance a = new Advance();
                    a.IdEmployee = Convert.ToInt32(cbAdvanceSelectEmployee.SelectedValue);
                    a.Date = Convert.ToDateTime(dtpAdvance.Value.Date);
                    a.Amount = Convert.ToSingle(tbAdcanceAmount.Text.Replace('.', ','));
                    if (tbAdvancesInfo.TextLength > 100)
                        throw new WrongSizeStringException("Maksymalna ilość znaków w polu \"Dodatkowe informacje\" nie może przekraczać 100.");
                    a.OtherInfo = tbAdvancesInfo.Text;

                    //wprowadzanie danych do bazy
                    AdvanceManager.Add(a, ConnectionToDB.disconnect);

                    //ustawia focus na wybór pracownika
                    cbAdvanceSelectEmployee.Focus();

                    //zeruje pole kwota
                    tbAdcanceAmount.Text = "";
                    
                    //odświerzanie grida
                    RefreshDgvAdvances(Convert.ToInt32(cbAdvanceSelectEmployee.SelectedValue), dtpAdvance.Value.Date);
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do wpisywania zaliczek pracowników!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (WrongSizeStringException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Wpisałeś niepoprawną kwotę zaliczki.\nPopraw kwotę i spróbuj ponownie.", "Błąd przy wpisywaniu zaliczki.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Błąd podczas wprowadzania danych do tabeli Zaliczka.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.zaliczka, "Pracownicy.btnZaliczkaDodaj_Click()/n/n" + ex2.Message));
            }
        }

        /// <summary>
        /// Podczas zmiany pracownika wyswietla zaliczki (danego) za aktualny miesiac
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSelectedEmployeeAdvance_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (panelAdvances.Visible)
            {
                idEmployeeCB = Convert.ToInt32(cbAdvanceSelectEmployee.SelectedValue);
                
                //odświerzanie grida
                RefreshDgvAdvances(Convert.ToInt32(cbAdvanceSelectEmployee.SelectedValue), dtpAdvance.Value.Date);
            }
        }

        /// <summary>
        /// Odświerzanie grida zaliczki
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        private void RefreshDgvAdvances(int id, DateTime data)//misiąc+rok
        {
            //zerowanie grida i listy
            dgvAdvances.Rows.Clear();
            AdvanceManager.arrayListAdvances.Clear();

            try
            {
                //pobieranie zaliczek z bazy danych do listyZaliczek
                AdvanceManager.GetAdvancesByDate(id, data);
                float suma = 0;
                foreach (Advance a in AdvanceManager.arrayListAdvances)
                {
                    dgvAdvances.Rows.Add(a.Date.ToString("d",DateFormat.TakeDateFormatDayFirst()), string.Format("{0:C}", a.Amount), a.OtherInfo.ToString(), a.Id, a.Amount);//kwota potrzebna do menu context
                    suma += a.Amount;
                }

                //podlicza zaliczki i wypisuje na końcu tabeli
                if (suma != 0)
                    dgvAdvances.Rows.Add("RAZEM:", string.Format("{0:C}", suma), "");
                //czyszczenie wyboru
                dgvAdvances.ClearSelection();
            }
            catch (Exception ex3)
            {
                MessageBox.Show(ex3.Message, "Odświerzanie grida zaliczki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.zaliczka, "MainForm.OdswierzanieGridaZaliczek()/n/n" + ex3.Message));
            }
        }

        /// <summary>
        /// Odświerzanie grida zaliczki przy zmanie daty
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpAdvances_ValueChanged(object sender, EventArgs e)
        {
            //odświerzanie grida
            RefreshDgvAdvances(Convert.ToInt32(cbAdvanceSelectEmployee.SelectedValue), dtpAdvance.Value.Date);
        }

    
        #endregion
        #region Menu podręczne ZALICZKI

        /// <summary>
        /// Jeżeli jest pusty grid to blokuje menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuAdvance_Opening(object sender, CancelEventArgs e)
        {
            //podświetlenie wiersza ppm
            Point p = dgvAdvances.PointToClient(System.Windows.Forms.Cursor.Position);
            DataGridView.HitTestInfo hti = dgvAdvances.HitTest(p.X, p.Y);

            //jeżeli kliknięto w komórke
            if ((hti.RowIndex > -1) && (hti.RowIndex < dgvAdvances.RowCount - 1))
            {
                dgvAdvances.ClearSelection();
                dgvAdvances.Rows[hti.RowIndex].Selected = true;
            }
            else
            {
                dgvAdvances.ClearSelection();
                contextMenuAdvance.Enabled = false;
            }
        }

        /// <summary>
        /// Zmienia datę zaliczki w bazie danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuAdvanceChangeDate_Click(object sender, EventArgs e)
        {
            try
            {
                //jeżeli użytkownik ma uprawnienia do zapisu
                if (Uprawnienie.PracownikWpisywanieZaliczek == 'w')
                {
                    EmployeeFinanse.isCorrect = false;
                
                    //otwiera formularz do zmiany kwoty
                    EmployeesFinances.Forms.ChangeAmountDateInfoForm changeForm = new EmployeesFinances.Forms.ChangeAmountDateInfoForm(AssignAdvance(), ChangeOptions.date);
                    changeForm.ShowDialog();
                 
                    if (EmployeeFinanse.isCorrect)
                        RefreshDgvAdvances(Convert.ToInt32(cbAdvanceSelectEmployee.SelectedValue), dtpAdvance.Value.Date);
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do edycji zaliczek!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd podczas aktualizacji zaliczki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.zaliczka, "Pracownicy.zaliczkiMenuZmienDate_Click()/n/n" + ex.Message));
            }
        }

        private Advance AssignAdvance()
        {
            Advance advance = new Advance();
            advance.Id = Convert.ToInt32(dgvAdvances.SelectedCells[3].Value);
            advance.Amount = Convert.ToSingle(dgvAdvances.SelectedCells[4].Value);
            advance.OtherInfo = dgvAdvances.SelectedCells[2].Value.ToString();
            advance.Date = Convert.ToDateTime(dgvAdvances.SelectedCells[0].Value);
            return advance;
        }
        /// <summary>
        /// Zmienia kwotę zaliczki w bazie danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuAdvanceAmount_Click(object sender, EventArgs e)
        {
            try
            {
                //jeżeli użytkownik ma uprawnienia do zapisu
                if (Uprawnienie.PracownikWpisywanieZaliczek == 'w')
                {
                    EmployeeFinanse.isCorrect = false;

                    //otwiera formularz do zmiany kwoty
                    EmployeesFinances.Forms.ChangeAmountDateInfoForm changeForm = new EmployeesFinances.Forms.ChangeAmountDateInfoForm(AssignAdvance(), ChangeOptions.amount);
                    changeForm.ShowDialog();

                    if (EmployeeFinanse.isCorrect)
                        RefreshDgvAdvances(Convert.ToInt32(cbAdvanceSelectEmployee.SelectedValue), dtpAdvance.Value.Date);
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do edycji zaliczek!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd podczas aktualizacji zaliczki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.zaliczka, "Pracownicy.zaliczkiMenuZmienKwote_Click()/n/n" + ex.Message));
            }
        }

        /// <summary>
        /// Zmienia Info zaliczki w bazie danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuAdvanceChangeInfo_Click(object sender, EventArgs e)
        {
            try
            {
                //jeżeli użytkownik ma uprawnienia do zapisu
                if (Uprawnienie.PracownikWpisywanieZaliczek == 'w')
                {
                    EmployeeFinanse.isCorrect = false;

                    //otwiera formularz do zmiany kwoty
                    EmployeesFinances.Forms.ChangeAmountDateInfoForm changeForm = new EmployeesFinances.Forms.ChangeAmountDateInfoForm(AssignAdvance(), ChangeOptions.info);
                    changeForm.ShowDialog();

                    if (EmployeeFinanse.isCorrect)
                        RefreshDgvAdvances(Convert.ToInt32(cbAdvanceSelectEmployee.SelectedValue), dtpAdvance.Value.Date);
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do edycji zaliczek!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd podczas aktualizacji zaliczki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.zaliczka, "Pracownicy.zaliczkiMenuZmienInfo_Click()/n/n" + ex.Message));
            }
        }

        /// <summary>
        /// Usuwa zaliczke z bazy danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuAdvanceDelete_Click(object sender, EventArgs e)
        {
            try
            {
                //jeżeli użytkownik ma uprawnienia do zapisu
                if (Uprawnienie.PracownikWpisywanieZaliczek == 'w')
                {
                    DialogResult result = MessageBox.Show("Czy napewno chcesz usunąć zaznaczony wpis?", "Usuwanie...",
                         MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        AdvanceManager.Delete(Convert.ToInt32(dgvAdvances.SelectedCells[3].Value), ConnectionToDB.disconnect);
                        RefreshDgvAdvances(Convert.ToInt32(cbAdvanceSelectEmployee.SelectedValue), dtpAdvance.Value.Date);
                    }
                    else
                    {
                        MessageBox.Show("Anulowano usuwanie.", "Usuwanie...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do usuwania zaliczek!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd usuwania zaliczki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.zaliczka, "Pracownicy.zaliczkiMenuUsunWpis_Click()/n/n" + ex.Message));
            }
        }

        #endregion


        #region DODATKI

        float TbAdditionAmount
        {
            set
            {
                tbAdditionAmount.Text = string.Format("{0:N}", value);
            }
        }

        /// <summary>
        /// Wyświetla panel dodawania dodatków
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdditions_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikWpisywaniePozyczek != 'x')
                {
                    UncheckAllButtons();
                    btnAdditions.Checked = true;
                    
                    HideAllPanels();
                    panelAdditions.Show();

                    //blokowanie przycisków
                    BlockButtons(false);

                    //wyswietlanie rodzai dodatkow z bazy
                    BindAdditionType();

                    //jeżeli nie jest już przypisane to przypisuje
                    if (idEmployeeCB == -1)
                        idEmployeeCB = Convert.ToInt32(cbSelectEmployeeAddition.SelectedValue);
                    else
                        //jeżeli jest to przypisuje do cb
                        cbSelectEmployeeAddition.SelectedValue = idEmployeeCB;
                    
                    //odświerzanie grida
                    RefreshDgvAdditions(Convert.ToInt32(cbSelectEmployeeAddition.SelectedValue), dtpAdditions.Value.Date);

                    //ustawia zatwierdzanie enterem
                    AcceptButton = btnAdditionAdd;
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do wpisywania dodatków pracowników!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas wyświetlania dodatków.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnDodatki_Click()/n/n" + ex1.Message));
            }
        }

        /// <summary>
        /// Dodawanie dodatku do bazy danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdditionAdd_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikWpisywanieDodatkow == 'w')
                {
                    AdditionManager.Add(AssignNewAddition(), ConnectionToDB.disconnect);

                    //ustawia focus na wybór pracownika
                    cbSelectEmployeeAddition.Focus();

                    //zeruje pole kwota i dodatkowe informacje
                    tbAdditionAmount.Text = "";
                    tbAdditionInfo.Text = "";

                    //odświerzanie grida
                    RefreshDgvAdditions(Convert.ToInt32(cbSelectEmployeeAddition.SelectedValue), dtpAdditions.Value.Date);
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do wpisywania dodatków pracowników!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (EmptyStringException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (FormatException)
            {
                MessageBox.Show("Wpisałeś niepoprawną kwotę dodatku.", "Błąd przy wpisywaniu dodatku.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Błąd podczas wprowadzania danych do tabeli Dodatek.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.dodatek, "Pracownicy.btnDodajDodatek_Click()/n/n" + ex2.Message));
            }
        }
        private Addition AssignNewAddition()
        {
            Addition addition = new Addition();
            addition.AdditionType = new AdditionType();
            addition.IdEmployee = Convert.ToInt32(cbSelectEmployeeAddition.SelectedValue);
            if (cbAdditionType.SelectedItem == null)
                throw new Exception("Nie masz jeszcze rodzajów dodatków w bazie danych.\nDodaj rodzaj dodatku i spróbuj ponownie.");
            addition.AdditionType.Id = Convert.ToInt32(cbAdditionType.SelectedValue);
            addition.Date = Convert.ToDateTime(dtpAdditions.Value.Date);

            addition.Amount = Convert.ToSingle(tbAdditionAmount.Text.Replace('.', ','));
            if (tbAdditionInfo.TextLength > 100)
                throw new EmptyStringException("Maksymalna ilość znaków w polu \"Dodatkowe informacje\" nie może przekraczać 100.");
            addition.OtherInfo = tbAdditionInfo.Text;
            return addition;
        }
        /// <summary>
        /// Pobieranie informacji o dodatkach do grida GODZINY
        /// </summary>
        /// <param name="id"></param>
        /// <param name="data"></param>
        private void RefreshDgvAdditions(int id, DateTime data)//misiąc+rok
        {
            //zerowanie grida i listy
            dgvAdditions.Rows.Clear();
            AdditionManager.arrayListAddition.Clear();
            float suma = 0;
            try
            {
                AdditionManager.GetAdditionsByDate(id, data);
                foreach (Addition a in AdditionManager.arrayListAddition)
                {
                    //kwota bez :C potrzebna do zmiany kwoty w bazie
                    dgvAdditions.Rows.Add(a.Date.ToString("d", DateFormat.TakeDateFormatDayFirst()), string.Format("{0:C}", a.Amount), a.AdditionType.Name, a.Id.ToString(), a.Amount.ToString(), a.OtherInfo.ToString());
                    suma += a.Amount;
                }

                if (suma != 0)
                    dgvAdditions.Rows.Add("RAZEM:", string.Format("{0:C}", suma), "");
                //czyszczenie wyboru
                dgvAdditions.ClearSelection();
            }
            catch (Exception ex3)
            {
                MessageBox.Show(ex3.Message, "Błąd podczas podliczania dodatków.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.dodatek, "Pracownicy.OdswierzanieGridaDodatek()/n/n" + ex3.Message));
            }
        }

        /// <summary>
        /// Odswierzanie grida godziny przy zmianie wyboru pracownika
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbSelectEmployeeAddition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (panelAdditions.Visible)
            {
                idEmployeeCB = Convert.ToInt32(cbSelectEmployeeAddition.SelectedValue);
                //odświerzanie grida
                RefreshDgvAdditions(Convert.ToInt32(cbSelectEmployeeAddition.SelectedValue), dtpAdditions.Value.Date);
            }
        }

        /// <summary>
        /// Odswierzanie grida godziny przy zmianie daty w głównym kalendarzu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpAdditions_ValueChanged(object sender, EventArgs e)
        {
            RefreshDgvAdditions(Convert.ToInt32(cbSelectEmployeeAddition.SelectedValue), dtpAdditions.Value.Date);
        }

        /// <summary>
        /// Dodaje nowy dodatek do bazy np. premia, pranie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdditionNew_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikWpisywanieDodatkow == 'w')
                {
                    EmployeeFinanse.isCorrect = false;
                    AdditionNewForm nd = new AdditionNewForm();
                    nd.ShowDialog();
                    if (EmployeeFinanse.isCorrect)
                        MessageBox.Show("Dodano nowy rodzaj dodatku do bazy danych.", "Potwierdzenie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //odświerzenie listy rodzaj dodatku
                    BindAdditionType();
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do wpisywania dodatków pracowników!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Błąd podczas wprowadzania danych do tabeli Dodatek.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.dodatek, "Pracownicy.btnDodatekNowy_Click()/n/n" + ex2.Message));
            }
        }

        #endregion
        #region MENU PODRĘCZNE DODATKI

        /// <summary>
        /// Sprawdza przed włączeniem menu czy są jakieś wpisy w gridzie
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuAdditions_Opening(object sender, CancelEventArgs e)
        {
            //podświetlenie wiersza ppm
            Point p = dgvAdditions.PointToClient(System.Windows.Forms.Cursor.Position);
            DataGridView.HitTestInfo hti = dgvAdditions.HitTest(p.X, p.Y);

            //jeżeli kliknięto w komórke
            if ((hti.RowIndex > -1) && (hti.RowIndex < dgvAdditions.RowCount - 1))
            {
                dgvAdditions.ClearSelection();
                dgvAdditions.Rows[hti.RowIndex].Selected = true;
            }
            else
            {
                dgvAdditions.ClearSelection();
                contextMenuAdditions.Enabled = false;
            }
        }

        Addition AssignAddition()
        {
            Addition addition = new Addition();
            addition.Id = Convert.ToInt32(dgvAdditions.SelectedCells[3].Value);
            addition.Amount = Convert.ToSingle(dgvAdditions.SelectedCells[4].Value);
            addition.OtherInfo = dgvAdditions.SelectedCells[5].Value.ToString();
            addition.Date = Convert.ToDateTime(dgvAdditions.SelectedCells[0].Value);
            return addition;
        }
        /// <summary>
        /// Zmiana kwoty dodatku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuAdditionsChangeAmount_Click(object sender, EventArgs e)
        {
            try
            {
                //jeżeli użytkownik ma uprawnienia do zapisu
                if (Uprawnienie.PracownikWpisywanieDodatkow == 'w')
                {
                    EmployeeFinanse.isCorrect = false;

                    //otwiera formularz do zmiany kwoty
                    EmployeesFinances.Forms.ChangeAmountDateInfoForm changeForm = new EmployeesFinances.Forms.ChangeAmountDateInfoForm(AssignAddition(), ChangeOptions.amount);
                    changeForm.ShowDialog();

                    if (EmployeeFinanse.isCorrect)
                        RefreshDgvAdditions(Convert.ToInt32(cbSelectEmployeeAddition.SelectedValue), dtpAdditions.Value.Date);
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do edycji dodatków!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd aktualizacji dodatku", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.dodatek, "Pracownicy.dodatekmenuZmienKwote_Click()/n/n" + ex.Message));
            }
        }

        /// <summary>
        /// Zmiana daty dodatku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuAdditionsChangeDate_Click(object sender, EventArgs e)
        {
            try
            {
                //jeżeli użytkownik ma uprawnienia do zapisu
                if (Uprawnienie.PracownikWpisywanieDodatkow == 'w')
                {
                    EmployeeFinanse.isCorrect = false;

                    //otwiera formularz do zmiany kwoty
                    EmployeesFinances.Forms.ChangeAmountDateInfoForm changeForm = new EmployeesFinances.Forms.ChangeAmountDateInfoForm(AssignAddition(), ChangeOptions.date);
                    changeForm.ShowDialog();

                    if (EmployeeFinanse.isCorrect)
                        RefreshDgvAdditions(Convert.ToInt32(cbSelectEmployeeAddition.SelectedValue), dtpAdditions.Value.Date);
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do edycji dodatków!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd aktualizacji dodatku", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.dodatek, "Pracownicy.dodatekmenuZmienDate_Click()/n/n" + ex.Message));
            }
        }

        /// <summary>
        /// Zmiana info dodatku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuAdditionsChangeInfo_Click(object sender, EventArgs e)
        {
            try
            {
                //jeżeli użytkownik ma uprawnienia do zapisu
                if (Uprawnienie.PracownikWpisywanieDodatkow == 'w')
                {
                    EmployeeFinanse.isCorrect = false;

                    //otwiera formularz do zmiany kwoty
                    EmployeesFinances.Forms.ChangeAmountDateInfoForm changeForm = new EmployeesFinances.Forms.ChangeAmountDateInfoForm(AssignAddition(), ChangeOptions.info);
                    changeForm.ShowDialog();

                    if (EmployeeFinanse.isCorrect)
                        RefreshDgvAdditions(Convert.ToInt32(cbSelectEmployeeAddition.SelectedValue), dtpAdditions.Value.Date);
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do edycji dodatków!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd aktualizacji dodatku", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.dodatek, "Pracownicy.dodatekmenuZmienInfo_Click()/n/n" + ex.Message));
            }
        }

        /// <summary>
        /// Usuwanie dodatków
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuAdditionsDelete_Click(object sender, EventArgs e)
        {
            try
            {
                //jeżeli użytkownik ma uprawnienia do zapisu
                if (Uprawnienie.PracownikWpisywanieDodatkow == 'w')
                {
                    int idAddition = Convert.ToInt32(dgvAdditions.SelectedCells[3].Value);
                    DialogResult result = MessageBox.Show("Czy napewno chcesz usunąć zaznaczony wpis?", "Usuwanie...",
                         MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        AdditionManager.Delete(idAddition, ConnectionToDB.disconnect);
                        RefreshDgvAdditions(Convert.ToInt32(cbSelectEmployeeAddition.SelectedValue), dtpAdditions.Value.Date);
                    }
                    else
                    {
                        MessageBox.Show("Anulowano usuwanie.", "Usuwanie...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                    throw new UnauthorizedAccessException("Nie masz uprawnień do usuwania dodatków!");
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd usuwania dodatku", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.dodatek, "Pracownicy.dodatekmenuUsunWpis_Click()/n/n" + ex.Message));
            }
        }


        #endregion


        #region POŻYCZKI

        /// <summary>
        /// Formatowanie grida pożyczki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dgvLoan_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            Payment paid = (Payment)Enum.Parse(typeof(Payment), dgvLoans[9, e.RowIndex].Value.ToString());
            if (paid == Payment.paidOff)
            {
                e.CellStyle.BackColor = Color.LightGray;
                e.CellStyle.SelectionBackColor = Color.Gray;
            }
        }

        /// <summary>
        /// Wyświetla tabele pożyczek i odblokowyje opbje
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoan_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikPozyczkiForm != 'x')
                {
                    UncheckAllButtons();
                    //zaznaczenie pożyczki
                    btnLoan.Checked = true;

                    //wyświetlenie panelu pożyczki i schowanie pozostałych   
                    HideAllPanels();
                    panelLoan.Show();

                    //zminan nazwy przycisków
                    btnAll.Text = "Wszystkie";
                    btnHired.Text = "Niespłacone";
                    btnRealesed.Text = "Spłacone";

                    //pobieranie ustawień kolumn grida
                    ustawieniaPozyczka.GetSettings();
                    SetLoan.InitialSettings();

                    //zaznaczenie wg ustawień
                    switch (ustawieniaPozyczka.OptionDisplay)
                    {
                        case DisplayOptions.paid:
                            btnRealesed.Checked = true;
                            break;
                        case DisplayOptions.all:
                            btnAll.Checked = true;
                            break;
                        case DisplayOptions.notPaid:
                            btnHired.Checked = true;
                            break;
                        default:
                            btnHired.Checked = true;
                            break;
                    }

                    //odblokowanie przycisków
                    BlockButtons(true);

                    //odswierzanie grida
                    RefreshDgvLoan();
                }
                else
                    throw new NoAccessException("Nie masz uprawnień do podglądu pożyczek!");
            }
            catch (NoAccessException ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Odświerzanie grida na podstawie zaznaczenia przycisku CHECKED
        /// </summary>
        void RefreshDgvLoan()
        {
            if (btnAll.Checked)
            {
                RefreshDgvLoan(Payment.all);
            }
            if (btnRealesed.Checked)//spłacone
            {
                RefreshDgvLoan(Payment.paidOff);
            }
            if (btnHired.Checked)//nie spłacone
            {
                RefreshDgvLoan(Payment.toPay);
            }
        }

        /// <summary>
        /// Metoda odświerzająca wpisy w tabeli pożyczek
        /// </summary>
        public void RefreshDgvLoan(Payment select)
        {
            try
            {
                switch (select)
                {
                    case Payment.toPay://niesplacone
                        LoanManager.GetLoanIsPaidToList(Payment.toPay);
                        break;
                    case Payment.paidOff://splacone
                        LoanManager.GetLoanIsPaidToList(Payment.paidOff);
                        break;
                    case Payment.all://wszystkie
                        LoanManager.GetAllLoanToList();
                        break;
                }

                dgvLoans.Rows.Clear();

                //pasek postępu
                backgroundWorker1.RunWorkerAsync();
                progressLoading = 0;//zerowanie
                int divider = (int)Math.Round((double)(100 / LoanManager.arrayLoans.Count)) + 1;//+1 żeby osiągnąć 100 jeżeli jest np. 3.33 round do 3

                foreach (Loan l in LoanManager.arrayLoans)
                {
                    //zmienna do pobierania danych pracownika (imie nazwisko)
                    employee = employeeManager.GetEmployee(l.IdEmployee, TableView.table, ConnectionToDB.disconnect);
                    //zwoekszanie paska postepu
                    progressLoading += divider;

                    //dodanie nowego wiersza
                    dgvLoans.Rows.Add(l.IdLoan.ToString(), l.IdEmployee.ToString(), string.Format("{0} {1}", employee.LastName, employee.FirstName),
                        l.Name, string.Format("{0:C}", l.Amount), l.Date.ToShortDateString(), string.Format("{0:C}", l.InstallmentLoan), l.OtherInfo,
                        string.Format("{0:C}", l.Amount - l.GetSumPaidInstallmentLoan()), (int)Enum.Parse(typeof(Payment), l.IsPaid.ToString()));
                }

                //ustwianie kolejności wyświetlania kolumn oraz sortowanie
                //sprawdzenie czy nie jest pusty
                if (SetLoan.listColumn.Count > 0)
                {
                    string nazwaKolumnySort = "";

                    foreach (Column k in SetLoan.listColumn)
                    {
                        //dgvGazociag.Columns[k.Nazwa].Visible = k.Widocznosc;
                        //dgvWewnetrzna.Columns[k.Nazwa].Width = k.Szer;                        
                        //dgvGazociag.Columns[k.Nazwa].DisplayIndex = k.Index;

                        if (k.Index == ustawieniaPozyczka.SortColumnIndex)
                            nazwaKolumnySort = k.Name;
                    }
                    //sortowanie
                    if (ustawieniaPozyczka.SortTypeAscDesc == SortType.ascending)
                    {
                        dgvLoans.Sort(dgvLoans.Columns[nazwaKolumnySort], ListSortDirection.Ascending);
                    }
                    else
                        dgvLoans.Sort(dgvLoans.Columns[nazwaKolumnySort], ListSortDirection.Descending);
                }

                //czyszczenie wyboru
                dgvLoans.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Odświerzanie grida pożyczki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "MainForm.OdswierzaniegridaPozyczki()/n/n" + ex.Message));
            }
        }

        #endregion
        #region Menu podręczne grida pożyczki

        /// <summary>
        /// Zachodzi podczas otwierania menu podręcznego
        /// Sprawdza czy są wpisy w gridzie pożyczki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuLoans_Opening(object sender, CancelEventArgs e)
        {
            //podświetlenie wiersza ppm
            Point p = dgvLoans.PointToClient(System.Windows.Forms.Cursor.Position);
            DataGridView.HitTestInfo hti = dgvLoans.HitTest(p.X, p.Y);

            //jeżeli kliknięto w komórke
            if (hti.RowIndex > -1)
            {
                dgvLoans.ClearSelection();
                dgvLoans.Rows[hti.RowIndex].Selected = true;

                Payment paid = (Payment)Enum.Parse(typeof(Payment), dgvLoans.SelectedCells[9].Value.ToString());
                if (paid == Payment.paidOff)//blokuje splacone, odblokowuje niespłacone
                {
                    contextMenuLoansSelectAsPaidOff.Enabled = false;
                    contextMenuLoansSelectAsToPay.Enabled = true;
                }
                else//blokuje niespłacone, odblokowuje spłacone
                {
                    contextMenuLoansSelectAsPaidOff.Enabled = true;
                    contextMenuLoansSelectAsToPay.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Zachodzi podczas kliknięcia menu podrecznego Spłacona
        /// Oznacza pożyczke w bazie danych jako spłaconą
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuLoansSelectAsToPay_Click(object sender, EventArgs e)
        {
            try
            {
                Loan l = LoanManager.GetLoan(Convert.ToInt32(dgvLoans.SelectedCells[0].Value));
                l.SetLoanAsPaidOrNotPaid(Payment.toPay, ConnectionToDB.disconnect);
                RefreshDgvLoan();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Musisz najpierw wybrać pożyczke z tabeli.",
                    "Błąd podczas aktualizacji pożyczki.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd podczas aktualizacji pożyczki.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "Pracownicy.niesplaconaToolStripMenuItem_Click()/n/n" + ex.Message));
            }
        }

        /// <summary>
        /// Zachodzi podczas kliknięcia menu podrecznego Spłacona
        /// Oznacza pożyczke w bazie danych jako spłaconą
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuLoansPaidOff_Click(object sender, EventArgs e)
        {
            try
            {
                Loan l = LoanManager.GetLoan(Convert.ToInt32(dgvLoans.SelectedCells[0].Value));
                l.SetLoanAsPaidOrNotPaid(Payment.paidOff, ConnectionToDB.disconnect);
                RefreshDgvLoan();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Musisz najpierw wybrać pożyczke z tabeli.",
                    "Błąd podczas aktualizacji pożyczki.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd podczas aktualizacji pożyczki.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "Pracownicy.splaconaToolStripMenuItem_Click()/n/n" + ex.Message));
            }
        }

        /// <summary>
        /// Otwiera formularz który wyświetla wszystkie spłaty danej pożyczki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuLoansInstallmentDetails_Click(object sender, EventArgs e)
        {
            try
            {
                Loan l = LoanManager.GetLoan(Convert.ToInt32(dgvLoans.SelectedCells[0].Value));
                l.GetListInstallmentLoan();
                LoanInstallmentDetailsForm loanInstallmentDetailsForm = new LoanInstallmentDetailsForm(l);
                loanInstallmentDetailsForm.ShowDialog();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Musisz najpierw wybrać pożyczke z tabeli.",
                    "Błąd podczas wyświetlania szczegółów pożyczki.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd podczas wyświetlania szczegółów pożyczki.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "Pracownicy.szczegolyRatyToolStripMenuItem_Click()/n/n" + ex.Message));
            }
        }

        /// <summary>
        /// Wyświetla ostrzezenie a następnie otwiera formularz do przyjęcia spłaty własnej
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contextMenuLoansOwnRepeyment_Click(object sender, EventArgs e)
        {
            try
            {
                Loan l = LoanManager.GetLoan(Convert.ToInt32(dgvLoans.SelectedCells[0].Value));
                //wyświetla ostrzezenie
                DialogResult result = MessageBox.Show("'Wpłata własna' nie odejmuje się od wypłaty przy 'Obliczaniu wypłat'.\n\nCzy chcesz kontynuować?",
                    "Uwaga!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    OwnRepaymentForm ownRepaymentForm = new OwnRepaymentForm(l);
                    ownRepaymentForm.ShowDialog();
                }
                //odświerzanie grida
                RefreshDgvLoan();
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Musisz najpierw wybrać pożyczke z tabeli.",
                    "Błąd podczas otwierania formularza - Wplata własna.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd podczas otwierania formularza - Wplata własna.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "Pracownicy.wplataWlasnaToolStripMenuItem_Click()/n/n" + ex.Message));
            }
        }

        #endregion


        #region STATYSTYKI

        private void btnStatistics_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień
                if (Uprawnienie.PracownikStatystyki != 'x')
                {
                    UncheckAllButtons();
                    btnStatistics.Checked = true;

                    HideAllPanels();
                    panelStatistics.Show();

                    //odblokowanie przycisków
                    BlockButtons(true);

                    //odswierzanie
                    BindEmployeeStatistics();
                }
                else
                    throw new Exception("Nie masz uprawnień do podglądu statystyk pracowników!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStatisticsDisplay_Click(object sender, EventArgs e)
        {
            try
            {
                //wyłączanie przycsku eby nie klikali po kilka razy
                btnStatisticsDisplay.Enabled = false;

                //wł workera i otwiera formularz z paskiem postępu
                backgroundWorker1.RunWorkerAsync();

                WorkStatistic.DisplayChart(this);
                LoanStatistics.DisplayChart(this);

                //włączanie przycisku po załadowaniu
                btnStatisticsDisplay.Enabled = true;
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex1)
            {
                MessageBox.Show(ex1.Message, "Brak danych.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas wyświetlania statystyk.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnStatystykiWyswietl_Click()/n/n" + ex1.Message));
            }
        }


        #endregion


        #region WYDRUKI

        /// <summary>
        /// Drukuje kartki do wyplaty z datą z dtpObliczWyplate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSalaryPrintPaymentCards_Click(object sender, EventArgs e)
        {
            try
            {
                string temp = string.Format("Czy chcesz wydrukować kartki do wypłaty za\nokres: {0:MMMM} {1:yyyy} dla " +
                    "wszystkich pracowników?\n\n'Nie' - drukuje tylko dla jednego pracownika", dtpSalary.Value, dtpSalary.Value);
                DialogResult result = MessageBox.Show(temp, "Potwierdzenie", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    //drukowanie
                    PaymantCards paymantCards = new PaymantCards();
                    paymantCards.Print(dtpSalary.Value.Date);
                }
                if (result == DialogResult.No)
                {
                    //drukowanie
                    PaymantCards paymantCards = new PaymantCards();
                    paymantCards.Print(dtpSalary.Value.Date, Convert.ToInt32(cbSelectEmployeeSalary.SelectedValue));
                }
            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.Message);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnSalaryPrintPaymentCards_Click()/n/n" + e3.Message));
            }
        }

        /// <summary>
        /// Drukuje karte godzin z datą z dtpObliczWyplate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSalaryPrintSettlementTime_Click(object sender, EventArgs e)
        {
            try
            {
                SettlementTime st = new SettlementTime();
                st.Print(Convert.ToInt32(cbSelectEmployeeSalary.SelectedValue), dtpSalary.Value);
            }
            catch (WrongDateTimeException e3)
            {
                MessageBox.Show(e3.Message);
            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.Message);    
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnSalaryPrintSettlementTime_Click()/n/n" + e3.Message));
            }
        }



        /// <summary>
        /// Otwiera fomularz z możliwością wyboru daty
        /// i drukuje liste ewidencje czasu pracy dla wszystkich pracowników
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEmployeePrintTimeRecordSheet_Click(object sender, EventArgs e)
        {
            try
            {
                //otwieranie formularza w celu wybrania daty i wydrukowania danych
                TimeRecordSheetPrintForm ecp = new TimeRecordSheetPrintForm();
                ecp.ShowDialog();
            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnEmployeePrintTimeRecordSheet_Click()/n/n" + e3.Message));
            }
        }

        /// <summary>
        /// Otwiera fomularz z możliwością wyboru daty
        /// i drukuje liste obecnosci
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEmployeePrintAttendanceList_Click(object sender, EventArgs e)
        {
            try
            {
                //otwieranie formularza w celu wybrania daty i wydrukowania danych
                AttendanceListPrintForm lo = new AttendanceListPrintForm();
                lo.ShowDialog();
            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnEmployeePrintAttendanceList_Click()/n/n" + e3.Message));
            }
        }

        /// <summary>
        /// Otwiera dokument WORD z wnioskiem urlopowym
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEmployeePrintHolidayRequest_Click(object sender, EventArgs e)
        {
            try
            {
                Employees.Prints.HolidayRequest.Print();
            }
            catch (Exception e3)
            {
                MessageBox.Show(e3.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "MainForm.btnEmployeePrintHolidayRequest_Click()/n/n" + e3.Message));
            }
        }

        #endregion


        #region Bindowanie

        /// <summary>
        /// Bindowanie comboboxów
        /// </summary>
        private void BindEmployeeWorkTime()
        {
            try
            {
                employeeManager.GetEmployeesHiredRealesedToList(true, TableView.table);//zatrudnieni
                ArrayList lista = new ArrayList(EmployeeManager.arrayEmployees);

                cbSelectEmployeeWork.DisplayMember = "GetFullName";
                cbSelectEmployeeWork.ValueMember = "IdEmployee";
                cbSelectEmployeeWork.DataSource = lista;
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas bindowania danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "BindPracownik()/n/n" + ex1.Message));
            }
        }

        private void BindEmployeeAdditions()
        {
            try
            {
                ArrayList lista = new ArrayList(EmployeeManager.arrayEmployees);

                cbSelectEmployeeAddition.DisplayMember = "GetFullName";
                cbSelectEmployeeAddition.ValueMember = "IdEmployee";
                cbSelectEmployeeAddition.DataSource = lista;
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas bindowania danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "BindPracownik()/n/n" + ex1.Message));
            }
        }

        private void BindEmployeeAdvances()
        {
            try
            {
                ArrayList lista = new ArrayList(EmployeeManager.arrayEmployees);

                cbAdvanceSelectEmployee.DisplayMember = "GetFullName";
                cbAdvanceSelectEmployee.ValueMember = "IdEmployee";
                cbAdvanceSelectEmployee.DataSource = lista;
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas bindowania danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "BindPracownik()/n/n" + ex1.Message));
            }
        }

        private void BindEmployeeStatistics()
        {
            try
            {
                ArrayList lista = new ArrayList(EmployeeManager.arrayEmployees);

                cbStatisticSelectEmployee.DisplayMember = "GetFullName";
                cbStatisticSelectEmployee.ValueMember = "IdEmployee";
                cbStatisticSelectEmployee.DataSource = lista;
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas bindowania danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "BindPracownik()/n/n" + ex1.Message));
            }
        }

        private void BindEmployeeLoans()
        {
            try
            {
                ArrayList lista = new ArrayList(EmployeeManager.arrayEmployees);

                cbLoanSelectEmployee.DisplayMember = "GetFullName";
                cbLoanSelectEmployee.ValueMember = "IdEmployee";
                cbLoanSelectEmployee.DataSource = lista;
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas bindowania danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "BindPracownik()/n/n" + ex1.Message));
            }
        }

        /// <summary>
        /// Bindowanie comboboxów
        /// </summary>
        private void BindEmployeeSalary()
        {
            try
            {
                ArrayList lista = new ArrayList(EmployeeManager.arrayEmployees);

                cbSelectEmployeeSalary.DisplayMember = "GetFullName";
                cbSelectEmployeeSalary.ValueMember = "IdEmployee";
                cbSelectEmployeeSalary.DataSource = lista;
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas bindowania danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "BindPracownik()/n/n" + ex1.Message));
            }
        }

        /// <summary>
        /// Bindowanie comboboxów
        /// </summary>
        private void BindDayOffType()
        {
            try
            {
                cbDayOffType.DataSource = ChangeDayOffTypeToString.ArrayListDayOffType;
                cbDayOffType.DisplayMember = "Name";
                cbDayOffType.ValueMember = "Id";
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas bindowania danych rodzaju urlopu.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "BindRodzajUrlopu()/n/n" + ex1.Message));
            }
        }

        /// <summary>
        /// Bindowanie comboboxów
        /// </summary>
        private void BindIllnessType()
        {
            try
            {
                cbIllnessType.DataSource = ChangeIllnessTypeToString.ArrayListIllnessType;
                cbIllnessType.DisplayMember = "Name";
                cbIllnessType.ValueMember = "Id";
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas bindowania danych rodzaju choroby.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "BindRodzajChoroby()/n/n" + ex1.Message));
            }
        }

        /// <summary>
        /// Bindowanie comboboxów
        /// </summary>
        private void BindAdditionType()
        {
            try
            {
                Addition.GetAdditionType();

                cbAdditionType.DataSource = Addition.arrayListAdditionType;
                cbAdditionType.DisplayMember = "Name";
                cbAdditionType.ValueMember = "Id";
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas bindowania danych rodzaju dodatku.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "BindRodzajChoroby()/n/n" + ex1.Message));
            }
        }

        #endregion


        #region Podświetlenie przycisków przy wybraniu (cheked)

        private void btnWszyscy_CheckStateChanged(object sender, EventArgs e)
        {
            if (btnAll.CheckState == CheckState.Checked)
            {
                //wł podświetlenia
                btnAll.ForeColor = Color.Blue;
            }
            if (btnAll.CheckState == CheckState.Unchecked)
            {
                btnAll.ForeColor = SystemColors.ControlText;
            }
        }

        private void btnZatrudnieni_CheckStateChanged(object sender, EventArgs e)
        {
            if (btnHired.CheckState == CheckState.Checked)
            {
                //wł podświetlenia
                btnHired.ForeColor = Color.Blue;
            }
            if (btnHired.CheckState == CheckState.Unchecked)
            {
                btnHired.ForeColor = SystemColors.ControlText;
            }
        }

        private void btnPracownicy_CheckStateChanged(object sender, EventArgs e)
        {
            if (btnEmployees.CheckState == CheckState.Checked)
            {
                //wł podświetlenia
                btnEmployees.ForeColor = Color.Blue;
            }
            if (btnEmployees.CheckState == CheckState.Unchecked)
            {
                btnEmployees.ForeColor = SystemColors.ControlText;
            }
        }

        private void btnPozyczki_CheckStateChanged(object sender, EventArgs e)
        {
            if (btnLoan.CheckState == CheckState.Checked)
            {
                //wł podświetlenia
                btnLoan.ForeColor = Color.Blue;
            }
            if (btnLoan.CheckState == CheckState.Unchecked)
            {
                btnLoan.ForeColor = SystemColors.ControlText;
            }
        }

        private void btnZwolnieni_CheckStateChanged(object sender, EventArgs e)
        {
            if (btnRealesed.CheckState == CheckState.Checked)
            {
                //wł podświetlenia
                btnRealesed.ForeColor = Color.Blue;
            }
            if (btnRealesed.CheckState == CheckState.Unchecked)
            {
                btnRealesed.ForeColor = SystemColors.ControlText;
            }
        }

        private void btnGodziny_CheckStateChanged(object sender, EventArgs e)
        {
            if (btnGodziny.CheckState == CheckState.Checked)
            {
                //wł podświetlenia
                btnGodziny.ForeColor = Color.Blue;
            }
            if (btnGodziny.CheckState == CheckState.Unchecked)
            {
                btnGodziny.ForeColor = SystemColors.ControlText;
            }
        }

        private void btnZaliczki_CheckStateChanged(object sender, EventArgs e)
        {
            if (btnAdvance.CheckState == CheckState.Checked)
            {
                //wł podświetlenia
                btnAdvance.ForeColor = Color.Blue;
            }
            if (btnAdvance.CheckState == CheckState.Unchecked)
            {
                btnAdvance.ForeColor = SystemColors.ControlText;
            }
        }

        private void btnDodatki_CheckStateChanged(object sender, EventArgs e)
        {
            if (btnAdditions.CheckState == CheckState.Checked)
            {
                //wł podświetlenia
                btnAdditions.ForeColor = Color.Blue;
            }
            if (btnAdditions.CheckState == CheckState.Unchecked)
            {
                btnAdditions.ForeColor = SystemColors.ControlText;
            }
        }

        private void btnPozyczkiSplaty_CheckStateChanged(object sender, EventArgs e)
        {
            if (btnLoanPayment.CheckState == CheckState.Checked)
            {
                //wł podświetlenia
                btnLoanPayment.ForeColor = Color.Blue;
            }
            if (btnLoanPayment.CheckState == CheckState.Unchecked)
            {
                btnLoanPayment.ForeColor = SystemColors.ControlText;
            }
        }

        private void btnWyplaty_CheckStateChanged(object sender, EventArgs e)
        {
            if (btnSalary.CheckState == CheckState.Checked)
            {
                //wł podświetlenia
                btnSalary.ForeColor = Color.Blue;
                //wl przycisk drukowanie kartek do wypłaty
                btnWyplatyWydrukiKartki.Enabled = true;
            }
            if (btnSalary.CheckState == CheckState.Unchecked)
            {
                btnSalary.ForeColor = SystemColors.ControlText;
                //wl przycisk drukowanie kartek do wypłaty
                btnWyplatyWydrukiKartki.Enabled = false;
            }
        }

        #endregion

        /// <summary>
        /// wyświetla przyciski opcji i wyświetlania gdy wybrany jest
        /// pracownik lub pożyczka, o blokuje przy reszcie
        /// </summary>
        /// <param name="p"></param>
        private void BlockButtons(bool p)
        {
            btnNowy.Enabled = p;
            btnEdytuj.Enabled = p;
            btnUsun.Enabled = p;
            btnAll.Enabled = p;
            btnHired.Enabled = p;
            btnRealesed.Enabled = p;
        }

        /// <summary>
        /// otwiera formularz z ustawieniami                            
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUstawienia_Click(object sender, EventArgs e)
        {
            try
            {
                //sprawdzenie uprawnień - jeżeli ma do pracownikForm to ma też do ustawnien
                if (Uprawnienie.PracownikForm != 'x')
                {
                    Pracownicy.Settings.Forms.SettingsForm ustawieniaForm = new Pracownicy.Settings.Forms.SettingsForm();
                    ustawieniaForm.ShowDialog();
                }
                else
                    throw new Exception("Nie masz uprawnień do podglądu danych pracowników!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        /// <summary>
        /// Otwiera formularz z paskiem postępu w osobnym wątku
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadingForm loadingForm = new LoadingForm();
            loadingForm.ShowDialog();
        }
       

        private void HideAllPanels()
        {
            panelWork.Hide();
            panelAdvances.Hide();
            panelSalary.Hide();
            panelPozyczkiSplaty.Hide();
            panelAdditions.Hide();
            panelLoan.Hide();
            panelEmployees.Hide();
            panelStatistics.Hide();
            panelCalendar.Hide();
        }
        private void UncheckAllButtons()
        {
            btnGodziny.Checked = false;
            btnLoan.Checked = false;
            btnEmployees.Checked = false;
            btnAdvance.Checked = false;
            btnAdditions.Checked = false;
            btnLoanPayment.Checked = false;
            btnSalary.Checked = false;
            btnAll.Checked = false;
            btnHired.Checked = false;
            btnRealesed.Checked = false;
        }
      
        private void tbCheckAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox tb = sender as TextBox;

            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != Convert.ToChar(Keys.Back))
            {
                if (e.KeyChar == ',')
                {
                    e.Handled = (tb.Text.Contains(","));
                }
                else
                    e.Handled = true;
            }
        }

        #region Kalendarz

        private void btnCalendar_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            panelCalendar.Show();

            
            calendarDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DisplayCalendarDate();
        }

        private void btnCalendarPrevDate_Click(object sender, EventArgs e)
        {
            calendarDate = calendarDate.AddMonths(-1);
            DisplayCalendarDate();
        }

        private void btnCalendarNextDate_Click(object sender, EventArgs e)
        {
            calendarDate = calendarDate.AddMonths(1);
            DisplayCalendarDate();
        }

        private void DisplayCalendarDate()
        {
            tlpKalendarz.Visible = false;
            tlpKalendarz.SuspendLayout();
            lblCalendarDate.Text = calendarDate.ToString("MMMM yyyy");
            int controlIndexToRemove = 7;//usuwa wszystko po tym indeksie (po niedzieli)
            while (controlIndexToRemove != tlpKalendarz.Controls.Count)
                tlpKalendarz.Controls.RemoveAt(controlIndexToRemove);
            
            calendarManager.DisplayCalendar(this.tlpKalendarz, calendarDate);
            tlpKalendarz.ResumeLayout();
            tlpKalendarz.Visible = true;

        }
        #endregion

    }
}
