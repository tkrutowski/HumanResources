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
using Logi;
using Ustawienia.DaneFirmy;
using System.Data.SqlClient;
using System.Collections;
using Ustawienia.Konfiguracja;
using HumanResources.Loans;
using HumanResources.Exceptions;
using HumanResources.Employees;

namespace HumanResources.Loans.Forms
{
    public partial class LoanNewForm : Form
    {
        Loan loan;
        bool isEdit;
        LoanManager loanManager = new LoanManager();

        /// <summary>
        /// Konstruktor podstawowy
        /// </summary>
        public LoanNewForm()
        {
            InitializeComponent();
            isEdit = false;

            //pasek tytułowy
            this.Text = DaneFirmy.NazwaProgramu + "Nowa pożyczka";
            //ikona
            this.Icon = Properties.Resources.logo_firmy;
            //bindowanie
            BindEmployee();
            loan = new Loan();
        }

        /// <summary>
        /// Konstruktor do edycji
        /// </summary>
        /// <param name="id">id pożyczki</param>
        public LoanNewForm(int id) //280 ->
        {
            InitializeComponent();
            isEdit = true;
            //bindowanie
            BindEmployee();
            AssignLoan(id);
            //blokowanie zmiany pracownika
            cbEmployee.Enabled = false;
            btnSave.Enabled = false;

            //eventy
            this.tbName.TextChanged += new System.EventHandler(this.loan_TextChanged);
            this.dtpData.ValueChanged += new System.EventHandler(this.loan_TextChanged);
            this.tbAmount.TextChanged += new System.EventHandler(this.loan_TextChanged);
            this.tbInstallmentLoan.TextChanged += new System.EventHandler(this.loan_TextChanged);
            this.tbOther.TextChanged += new System.EventHandler(this.loan_TextChanged);

            //pasek tytułowy
            this.Text = DaneFirmy.NazwaProgramu + "Edycja pożyczki";
            //ikona
            this.Icon = Properties.Resources.logo_firmy;

            //ustawienie blokady na dany rekord
            Blokady.UstawienieBlokady(NazwaTabeli.pozyczka, id, "", Polaczenia.idUser, DateTime.Now);
        }

        //propertys
        float TbAmount
        {
            set
            {
                tbAmount.Text = string.Format("{0:N}", value);
            }
        }
        //propertys
        float TbInstallmentLoan
        {
            set
            {
                tbInstallmentLoan.Text = string.Format("{0:N}", value);
            }
        }
        
        void AssignLoan(int id)
        {
            //pobieranie danych o pożyczce z bazy danych
            loan = LoanManager.GetLoan(id);

            //wyświetlenie ich w polach textowych
            cbEmployee.SelectedValue = loan.IdEmployee;
            tbName.Text = loan.Name;
            dtpData.Value = loan.Date.Date;
            tbAmount.Text = loan.Amount.ToString();
            tbInstallmentLoan.Text = loan.InstallmentLoan.ToString();
            tbOther.Text = loan.OtherInfo;
        }

        /// <summary>
        /// Zapisuje pożyczke w bazie danych
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CheckDataCorrectness();

                DataAssignment();
                               
                if (isEdit)
                {
                    LoanManager.EditLoan(loan, ConnectionToDB.notDisconnect);                    
                    Blokady.UsuwanieBlokady(Polaczenia.idUser, NazwaTabeli.pozyczka);
                }
                else
                {
                    LoanManager.AddLoan(loan,ConnectionToDB.disconnect);
                }
                //jeżeli nie było błedów ustawia poprawność na true
                Loan.correctLoan = true;

                //zamykanie formularza
                this.Close();

            }
            catch (FormatException)
            {
                MessageBox.Show("Musisz podać kwotę oddzieloną przecinkiem (np. 120,80)", "Błędne dane, popraw i spróbuj ponownie", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (EmptyStringException ex1)
            {
                MessageBox.Show(ex1.Message, "Błędne dane, popraw i spróbuj ponownie", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (WrongSizeStringException ex2)
            {
                MessageBox.Show(ex2.Message, "Błędne dane, popraw i spróbuj ponownie", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas edycji pożyczki", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "PozyczkaNowaForm.btnDodaj_Click()/n/n" + ex1.Message));
                //zamykanie formularza
            }
            finally
            {
                Polaczenia.OdlaczenieOdBazy();
            }
        }

        private void DataAssignment()
        {
            loan.Name = tbName.Text;
            loan.Date = dtpData.Value.Date;
            loan.Amount = Convert.ToSingle(tbAmount.Text.Replace('.', ','));
            loan.InstallmentLoan = Convert.ToSingle(tbInstallmentLoan.Text);
            loan.OtherInfo = tbOther.Text;

            if (!isEdit)
            {
                loan.IdEmployee = Convert.ToInt32(cbEmployee.SelectedValue);
                loan.IsPaid = Payment.toPay;
            }
        }

        private void CheckDataCorrectness()
        {
            if (tbName.Text.Trim() == "")
                throw new EmptyStringException("Musisz wypełnić pole nazwa.");
            if (tbName.Text.Length > 50)
                throw new WrongSizeStringException("Nazwa nie może mieć więcej niż 50 liter.");
            if (tbAmount.Text.Trim() == "")
                throw new EmptyStringException("Musisz wypełnić pole kwota.");
            if (tbInstallmentLoan.Text == "")
                throw new EmptyStringException("Musisz wypełnić pole wysokość raty.");            
        }


        /// <summary>
        /// anulowanie - zamykanie formulerza i usuwanie blokady
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAnuluj_Click(object sender, EventArgs e)
        {
            if (isEdit)
            {
                Blokady.UsuwanieBlokady(Polaczenia.idUser, NazwaTabeli.pozyczka);
                Polaczenia.OdlaczenieOdBazy();
            }
            this.Close();
        }

        /// <summary>
        /// Jeżeli zmieni się tekst w którymś z pól to odblokowany zostanie przycisk zatwierdz
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loan_TextChanged(object sender, EventArgs e)
        {
            btnSave.Enabled = true;
        }

        private void tbAmount_Leave(object sender, EventArgs e)
        {
            try
            {
                TbAmount = Convert.ToSingle(tbAmount.Text.Replace('.', ','));
            }
            catch (FormatException)
            {
                //występuje gdy kontrolka traci focus i nie ma wpisanej żadnej wartości
            }
        }

        private void tbInstellment_Leave(object sender, EventArgs e)
        {
            try
            {
                TbInstallmentLoan = Convert.ToSingle(tbInstallmentLoan.Text.Replace('.', ','));
            }
            catch (FormatException)
            {
                //występuje gdy kontrolka traci focus i nie ma wpisanej żadnej wartości
            }
        }

        private void tb_KeyPress(object sender, KeyPressEventArgs e)
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

        /// <summary>
        /// Bindowanie comboboxów
        /// </summary>
        private void BindEmployee()
        {
            try
            {
                EmployeeManager em = new EmployeeManager();
                em.GetEmployeesHiredRealesedToList(true, TableView.table);
                
                ArrayList lista = new ArrayList(EmployeeManager.arrayEmployees);

                cbEmployee.DataSource = lista;
                cbEmployee.DisplayMember = "GetFullName";
                cbEmployee.ValueMember = "IdEmployee";
            }
            catch (Exception ex1)
            {
                MessageBox.Show(ex1.Message, "Błąd podczas bindowania danych pracownika.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.pozyczka, "BindPracownik()/n/n" + ex1.Message));
            }
        }
    }
}
