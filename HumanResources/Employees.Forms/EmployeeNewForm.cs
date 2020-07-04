using System;
using System.Drawing;
using System.Windows.Forms;
using Konfiguracja;
using Ustawienia.DaneFirmy;
using Logi;
using System.Data.SqlClient;
using Ustawienia.Konfiguracja;
using Ustawienia.Uprawnienia;
using HumanResources.Exceptions;

namespace HumanResources.Employees.Forms
{
    public partial class NewEmployeeForm : Form//737 ->680
    {
        EmployeeManager employeeManager = new EmployeeManager();
        Employee employee;
        
        //zmienne okreslające czy była zmiana danych w polach tekstowych
        bool editEmployee = false;
        bool editRateRegular = false;
        bool editRateOvertime = false;
                      
        bool isEdit;

        /// <summary>
        /// Konstruktor podstawowy 
        /// </summary>
        public NewEmployeeForm()
        {
            InitializeComponent();
            //podczas dodawania nowego pracownika
            //data stawek jest pobierana z dnia wstawiania wpisu
            dtpRegularRateFromDate.Enabled = false;
            dtpOvertimeRateFromDate.Enabled = false;
            isEdit = false;
            
            //pasek tytułowy
            this.Text = DaneFirmy.NazwaProgramu + "Pracownik - Nowy";
            //ikona
            this.Icon = Properties.Resources.logo_firmy;
            employee = new Employee();
        }

        private void PermissionsCheck()
        {
            if(Uprawnienie.PracownikFinanse =='x')
            {
                groupBoxRateRegular.Enabled = false;
                groupBoxRateOvertime.Enabled = false;
            }
        }

        /// <summary>
        /// Konstruktor przeciążony
        /// Służy do edycji danych pracownika
        /// </summary>
        /// <param name="idEmployee">id pracownika</param>
        public NewEmployeeForm(int idEmployee)
        {
            InitializeComponent();
            isEdit = true;
            //blokowanie wybierania dat
            dtpRegularRateFromDate.Enabled = false;
            dtpOvertimeRateFromDate.Enabled = false;
            //blokowanie przycisku zatwierdz
            btnSave.Enabled = false;

            //pasek tytułowy
            this.Text = DaneFirmy.NazwaProgramu + "Pracownik - Edycja";
            //ikona
            this.Icon = Properties.Resources.logo_firmy;
            employee = employeeManager.GetEmployee(idEmployee,TableView.view, ConnectionToDB.disconnect);
            
            PermissionsCheck();
            
            DisplayEmployee();

            //dodanie delegatow
            this.tbOthersInfo.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.cbNumberDaysOffAnnually.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbTelNumer.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbNumberDaysOffLeft.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbLastName.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbName.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbZipCode.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbCity.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbStreet.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbNextMmedicalExaminationDate.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbNextTrainingBhpDate.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbHiredDate.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbReleaseDate.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbMail.TextChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.chbPartTimeJob.CheckedChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.chbIsManagement.CheckedChanged += new System.EventHandler(this.editEmployee_ValueChanged);
            this.tbRateOvertimeValue.TextChanged += new System.EventHandler(this.editRateOvertime_ValueChanged);
            this.tbRateValue.TextChanged += new System.EventHandler(this.editRateRegular_ValueChanged);

            this.dtpHiredDate.ValueChanged += new System.EventHandler(this.dtpHiredDate_ValueChanged);
            this.dtpReleaseDate.ValueChanged += new System.EventHandler(this.dtpRealiseDate_ValueChanged);
            this.dtpNextMmedicalExaminationDate.ValueChanged += new System.EventHandler(this.dtpNextMmedicalExaminationDate_ValueChanged);
            this.dtpNextTrainingBhpDate.ValueChanged += new System.EventHandler(this.dtpNextBhpTrainingDate_ValueChanged);

            //ustawienie blokady na dany rekord
            Blokady.UstawienieBlokady(NazwaTabeli.pracownik, idEmployee, "", Polaczenia.idUser, DateTime.Now);
        }

        #region PROPERTYSY

        float TbNumberDaysOffLeft
        {
            set
            {
                tbNumberDaysOffLeft.Text = string.Format("{0:N1}", value);
            }
        }

        float TbRateValue
        {
            set
            {
                tbRateValue.Text = string.Format("{0:N}", value);
            }
        }

        float TbRateOvertimeValue
        {
            set
            {
                tbRateOvertimeValue.Text = string.Format("{0:N}", value);
            }
        }

        DateTime TbHiredDate
        {
            set
            {
                if (value != Convert.ToDateTime("0001-01-01"))
                {
                    tbHiredDate.Text = string.Format("{0}", value.ToShortDateString());
                    //włącza obrazek
                    pbHiredDate.Visible = true;
                }
            }
        }

        DateTime TbReleaseDate
        {
            set
            {
                if (value != Convert.ToDateTime("0001-01-01"))
                {
                    tbReleaseDate.Text = string.Format("{0}", value.ToShortDateString());
                    //włącza obrazek
                    pbReleaseDate.Visible = true;
                }
            }
        }

        DateTime TbNextMmedicalExaminationDate
        {
            set
            {
                if (value != Convert.ToDateTime("0001-01-01"))
                {
                    tbNextMmedicalExaminationDate.Text = string.Format("{0}", value.ToShortDateString());
                    //włącza obrazek
                    pbNextMmedicalExaminationDate.Visible = true;
                }
            }
        }

        DateTime TbNextTrainingBhpDate
        {
            set
            {
                if (value != Convert.ToDateTime("0001-01-01"))
                {
                    tbNextTrainingBhpDate.Text = string.Format("{0}", value.ToShortDateString());
                    //włącza obrazek
                    pbNextTrainingBhpDate.Visible = true;
                }
            }
        }

        #endregion


        private void DisplayEmployee()
        {
            try
            {
                this.tbName.Text = employee.FirstName;
                this.tbLastName.Text = employee.LastName;
                this.tbStreet.Text = employee.Street;
                this.tbCity.Text = employee.City;
                this.tbZipCode.Text = employee.ZipCode;
                this.tbNumberDaysOffLeft.Text = employee.NumberDaysOffLeft.ToString();
                this.tbMail.Text = employee.EMail;
                this.chbPartTimeJob.Checked = employee.PartTimeJob;
                this.chbIsManagement.Checked = employee.IsManagement;

                switch (employee.NumberDaysOffAnnually)
                {
                    case 20:
                        this.cbNumberDaysOffAnnually.SelectedIndex = 0;
                        break;
                    case 26:
                        this.cbNumberDaysOffAnnually.SelectedIndex = 1;
                        break;
                }
                this.tbTelNumer.Text = employee.TelNumber;
                this.tbOthersInfo.Text = employee.OtherInfo;

                if (UprawnieniePracownik.PracownikFinanse != 'x')
                {
                    //stawka
                    if (employee.RateRegular.IsMonthlyOrHourly == RateType.monthly)
                    {
                        //jezeli stawka miesieczna
                        rbRateMonthly.Checked = true;
                    }
                    else
                    {
                        //jeżeli stawka godzinowa
                        rbRateHourly.Checked = true;
                    }
                    tbRateValue.Text = employee.RateRegular.RateValue.ToString();
                    dtpRegularRateFromDate.Value = employee.RateRegular.DateFrom;

                    //stawka nadgodzinowa
                    this.tbRateOvertimeValue.Text = employee.RateOvertime.RateValue.ToString();
                    dtpOvertimeRateFromDate.Value = employee.RateOvertime.DateFrom;
                }
                //dane opcjonalne
                TbHiredDate = employee.HiredDate;
                TbReleaseDate = employee.ReleaseDate;
                TbNextMmedicalExaminationDate = employee.NextMmedicalExaminationDate;
                TbNextTrainingBhpDate = employee.NextBhpTrainingDate;
            }
            catch (Exception ex)
            {
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "PracownikNowyForm.WypisywanieDanychDoEdycji()/n/n" + ex.Message));
            }
        }

        /// <summary>
        /// Przycisk zatwierdzania zmian dokonanych w formularzu
        /// zapisuje zmiany w bazie danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CheckDataCorrectnessEmployee();
             
                DataAssignmentEmployee();

                //jeżeli zmienna "edycja" ma wartość true wpis o podanym idPracownwnika jest edytowany
                if (isEdit)
                {
                    //rozpoczęcie transakcji i blokowanie rekordu
                    Polaczenia.BeginTransactionSerializable();

                    //wprowadzanie zmian do bazy
                    if (editEmployee)
                        employeeManager.Edit(employee, ConnectionToDB.notDisconnect);

                    if (editRateRegular)
                    {
                        //sprawdzenie czy istnieje stawka z wybraną datą
                        //jeżeli istnieje to edycja stawki
                        if (employee.RateRegular.IsExist())
                        {
                            employee.RateRegular.DateFrom = dtpRegularRateFromDate.Value;
                            employee.RateRegular.RateValue = Convert.ToSingle(this.tbRateValue.Text);
                            employee.RateRegular.IsMonthlyOrHourly = (rbRateHourly.Checked ? RateType.hourly : RateType.monthly);
                            employee.EditRateRegular(ConnectionToDB.notDisconnect);
                        }
                        else
                        //jeżeli nie to dodanie nowej
                        {
                            employee.RateRegular = new RateRegular(dtpRegularRateFromDate.Value, Convert.ToSingle(this.tbRateValue.Text), rbRateHourly.Checked ? RateType.hourly : RateType.monthly);
                            employee.AddRateRegular(ConnectionToDB.notDisconnect);
                        }
                    }

                    if (editRateOvertime)
                    {
                        //sprawdzenie czy istnieje stawka z wybraną datą
                        //jeżeli istnieje to edycja stawki
                        if (employee.RateOvertime.IsExist())
                        {
                            employee.RateOvertime.DateFrom = dtpOvertimeRateFromDate.Value;
                            employee.RateOvertime.RateValue = Convert.ToSingle(this.tbRateOvertimeValue.Text);
                            employee.EditRateOvertime(ConnectionToDB.notDisconnect);
                        }
                        else
                        {
                            //jeżeli nie to dodanie nowej
                            employee.RateOvertime = new RateOvertime(dtpOvertimeRateFromDate.Value, Convert.ToSingle(this.tbRateOvertimeValue.Text));
                            employee.AddRateOvertime(ConnectionToDB.notDisconnect); 
                        }
                    }
                    Polaczenia.CommitTransaction();
                    Blokady.UsuwanieBlokady(Polaczenia.idUser, NazwaTabeli.pracownik);
                    Polaczenia.OdlaczenieOdBazy();
                    Employee.correctEmployee = true;
                    //zamykanie formularza
                    this.Close();
                }
                else//jeżeli nie edycja to dodaje nowego pracownika do bazy
                {
                    //rozpoczęcie transakcji
                    Polaczenia.BeginTransaction();
                    //dodanie pracownika
                    int idInsertedEmployee = employeeManager.AddReturnId(employee,ConnectionToDB.notDisconnect);
                    
                    employee = employeeManager.GetEmployee(idInsertedEmployee, TableView.table, ConnectionToDB.notDisconnect);
                    //pobranie i przypisanie id ostatniego pracownika   
                    employee.RateRegular = new RateRegular(idInsertedEmployee, dtpRegularRateFromDate.Value, Convert.ToSingle(this.tbRateValue.Text), rbRateHourly.Checked ? RateType.hourly : RateType.monthly);
                    employee.RateOvertime = new RateOvertime(idInsertedEmployee, dtpOvertimeRateFromDate.Value, Convert.ToSingle(this.tbRateOvertimeValue.Text));
                    //dodanie stawki
                    employee.AddRateRegular(ConnectionToDB.notDisconnect);
                    //dodanie stawki za nadgodziny
                    employee.AddRateOvertime(ConnectionToDB.notDisconnect);
                    Polaczenia.CommitTransaction();
                    Polaczenia.OdlaczenieOdBazy();
                    //zamykanie formularza
                    this.Close();
                }
            }
            catch (EmptyStringException ex)
            {
                MessageBox.Show(ex.Message, "Błąd danych", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (WrongSizeStringException ex)
            {
                MessageBox.Show(ex.Message, "Błąd danych", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (NoNullException ex)
            {
                MessageBox.Show(ex.Message, "Błąd danych", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (OverflowException)
            {
                MessageBox.Show("Wprowadziłeś niepoprawną stawkę lub urlop.\n\nPopraw pola zaznaczone na czerwono.", "Błąd danych", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (SqlException eSQL)
            {
                MessageBox.Show(eSQL.Message, "Błąd SQL podczas wprowadzania danych", MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                Polaczenia.RollbackTransaction();
                Polaczenia.OdlaczenieOdBazy();
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "PracownikNowyForm.btnDodaj_Click()/n/n" + eSQL.Message));
            }
            catch (System.IO.FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message, "Brak pliku konfiguracyjnego", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Błąd podczas sprawdzania danych", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Polaczenia.RollbackTransaction();
                Polaczenia.OdlaczenieOdBazy();
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pracownik, "PracownikNowyForm.btnDodaj_Click()/n/n" + ex2.Message));
            }
            finally
            {
                //jeżeli edycja danych to usuwa blokade rekordu
                if (isEdit)
                {
                    Blokady.UsuwanieBlokady(Polaczenia.idUser, NazwaTabeli.pracownik);
                    Polaczenia.OdlaczenieOdBazy();
                }
            }
        }

        private void DataAssignmentEmployee()
        {
            employee.FirstName = this.tbName.Text.Trim();
            employee.LastName = this.tbLastName.Text.Trim();
            employee.Street = this.tbStreet.Text.Trim();
            employee.ZipCode = this.tbZipCode.Text.Trim();
            employee.City = this.tbCity.Text.Trim();
            employee.NumberDaysOffLeft = Convert.ToSingle(this.tbNumberDaysOffLeft.Text.Replace('.', ','));
            employee.NumberDaysOffAnnually = Convert.ToInt32(this.cbNumberDaysOffAnnually.SelectedItem);
            employee.TelNumber = this.tbTelNumer.Text.Trim();
            employee.OtherInfo = this.tbOthersInfo.Text.Trim();
            employee.EMail = this.tbMail.Text.Trim();
            employee.IsHired = true;
            if (tbHiredDate.Text != "")
                employee.HiredDate = Convert.ToDateTime(tbHiredDate.Text);
            if (tbReleaseDate.Text != "")
                employee.ReleaseDate = Convert.ToDateTime(tbReleaseDate.Text);
            if (tbNextMmedicalExaminationDate.Text != "")
                employee.NextMmedicalExaminationDate = Convert.ToDateTime(tbNextMmedicalExaminationDate.Text);
            if (tbNextTrainingBhpDate.Text != "")
                employee.NextBhpTrainingDate = Convert.ToDateTime(tbNextTrainingBhpDate.Text);
            //zmienne do drukowania
            employee.PartTimeJob = chbPartTimeJob.Checked;
            employee.IsManagement = chbIsManagement.Checked;
        }

        private void CheckDataCorrectnessEmployee()
        {
            //imie
            if (tbName.Text.Trim() == "")
                throw new EmptyStringException("Pole 'Imie' nie może być puste.");
            if (tbName.Text.Trim().Length > 10)
                throw new EmptyStringException("Pole 'Imie' nie może zawierać więcej niż 10 znaków.");
           
            //nazwisko
            if (tbLastName.Text.Trim() == "")
                throw new EmptyStringException("Pole 'Nazwisko' nie może być puste.");
            if (tbLastName.Text.Trim().Length > 20)
                throw new WrongSizeStringException("Pole 'Nazwisko' nie może zawierać więcej niż 20 znaków.");
           
            //ulica
            if (tbStreet.Text.Trim() == "")
                throw new EmptyStringException("Pole 'Ulica' nie może być puste.");
            if (tbStreet.Text.Trim().Length > 50)
                throw new WrongSizeStringException("Pole 'Ulica' nie może zawierać więcej niż 50 znaków.");
            
            //kod
            if (tbZipCode.Text.Trim() == "__-___")
                throw new EmptyStringException("Pole 'Kod' nie może być puste.");

            //miasto
            if (tbCity.Text.Trim() == "")
                throw new EmptyStringException("Pole 'Miasto' nie może być puste.");
            if (tbCity.Text.Trim().Length > 50)
                throw new WrongSizeStringException("Pole 'Miasto' nie może zawierać więcej niż 50 znaków.");

            //urlop pozostały
            if (tbNumberDaysOffLeft.Text.Trim() == "")
                throw new EmptyStringException("Pole 'Urlop pozostały' nie może być puste.");

            if (cbNumberDaysOffAnnually.SelectedItem == null)
                throw new NoNullException("Musisz wybrać wymiar urlopu.");

            if (tbTelNumer.Text.Trim().Length > 20)
                throw new WrongSizeStringException("Pole 'Numer tel.' nie może zawierać więcej niż 20 znaków.");

            if (tbOthersInfo.Text.Trim().Length > 150)
                throw new WrongSizeStringException("Pole 'Inne' nie może zawierać więcej niż 150 znaków.");

            if (tbMail.Text.Trim().Length > 100)
                throw new WrongSizeStringException("Pole 'E-mail' nie może zawierać więcej niż 100 znaków.");
            //TODO nie sprawdzać gdy nie ma dostępu  (wtedy tb są puste)

            if (Uprawnienie.PracownikFinanse != 'x')
            {
                if (tbRateValue.Text.Trim() == string.Empty)
                    throw new NoNullException("Musisz wpisać stawkę.");

                if (tbRateOvertimeValue.Text.Trim() == string.Empty)
                    throw new NoNullException("Musisz wpisać stawkę nadgodzinową.");
            }
        }

        /// <summary>
        /// Anulowanie wpisu i zamknię cie formularza
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void anulujButton_Click(object sender, EventArgs e)
        {
            Blokady.UsuwanieBlokady(Polaczenia.idUser, NazwaTabeli.pracownik);
            Polaczenia.OdlaczenieOdBazy();
            this.Close();
        }

     
        /// <summary>
        /// Okresla czy była zmiana w polach stawki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editRateRegular_ValueChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            dtpRegularRateFromDate.Enabled = true;
            editRateRegular = true;
        }

        /// <summary>
        /// Okresla czy była zmiana w polach stawki nadgodzinowej
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editRateOvertime_ValueChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            dtpOvertimeRateFromDate.Enabled = true;
            editRateOvertime = true;
        }

        /// <summary>
        /// Okresla czy była zmiana w polach danych personalnych pracownika
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void editEmployee_ValueChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
            editEmployee = true;
        }

        /// <summary>
        /// Sprawdza poprawność wpisanych danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbDayOff_Leave(object sender, EventArgs e)
        {
            TbNumberDaysOffLeft = Convert.ToSingle(tbNumberDaysOffLeft.Text.Trim());         
        }

        
        /// <summary>
        /// Sprawdza poprawność wpisanych danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbRateRegular_Leave(object sender, EventArgs e)
        {
            if (tbRateValue.Text.Trim() != string.Empty)
                TbRateValue = Convert.ToSingle(tbRateValue.Text.Trim());
        }

        /// <summary>
        /// Sprawdza poprawność wpisanych danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbRateOvertime_Leave(object sender, EventArgs e)
        {
            if(tbRateOvertimeValue.Text.Trim() != string.Empty)
                TbRateOvertimeValue = Convert.ToSingle(tbRateOvertimeValue.Text.Trim());
        }
        private void tbRate_KeyPress(object sender, KeyPressEventArgs e)
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
        /// <summary>
        /// wyswietlanie daty w tb i wł gumki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpHiredDate_ValueChanged(object sender, EventArgs e)
        {
            tbHiredDate.Text = dtpHiredDate.Value.ToShortDateString();
            //włącza obrazek
            pbHiredDate.Visible = true;
        }

        /// <summary>
        /// wyswietlanie daty w tb i wł gumki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpRealiseDate_ValueChanged(object sender, EventArgs e)
        {
            tbReleaseDate.Text = dtpReleaseDate.Value.ToShortDateString();
            //włącza obrazek
            pbReleaseDate.Visible = true;
        }

        /// <summary>
        /// wyswietlanie daty w tb i wł gumki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpNextMmedicalExaminationDate_ValueChanged(object sender, EventArgs e)
        {
            tbNextMmedicalExaminationDate.Text = dtpNextMmedicalExaminationDate.Value.ToShortDateString();
            //włącza obrazek
            pbNextMmedicalExaminationDate.Visible = true;
        }

        /// <summary>
        /// wyswietlanie daty w tb i wł gumki
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dtpNextBhpTrainingDate_ValueChanged(object sender, EventArgs e)
        {
            tbNextTrainingBhpDate.Text = dtpNextTrainingBhpDate.Value.ToShortDateString();
            //włącza obrazek
            pbNextTrainingBhpDate.Visible = true;
        }

        /// <summary>
        /// usuwanie daty z tb
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbHiredDate_Click(object sender, EventArgs e)
        {
            tbHiredDate.Text = null;
            pbHiredDate.Visible = false;
        }

        /// <summary>
        /// usuwanie daty z tb
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbReleaseDate_Click(object sender, EventArgs e)
        {
            tbReleaseDate.Text = null;
            pbReleaseDate.Visible = false;
        }

        /// <summary>
        /// usuwanie daty z tb
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbNextMmedicalExaminationDate_Click(object sender, EventArgs e)
        {
            tbNextMmedicalExaminationDate.Text = null;
            pbNextMmedicalExaminationDate.Visible = false;
        }

        /// <summary>
        /// usuwanie daty z tb
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pbDataNastSzkoleniaBHP_Click(object sender, EventArgs e)
        {
            tbNextTrainingBhpDate.Text = null;
            pbNextTrainingBhpDate.Visible = false;
        }

        /// <summary>
        /// Zmienia text przycisku anuluj
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_EnabledChanged(object sender, EventArgs e)
        {
            if (btnSave.Enabled)
                btnCancel.Text = "&Anuluj";
            else
                btnCancel.Text = "&Wyjście";
        }
    }
}
