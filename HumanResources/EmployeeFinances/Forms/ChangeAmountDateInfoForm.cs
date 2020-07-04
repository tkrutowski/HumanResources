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
using HumanResources.EmployeesFinances.Additions;
using HumanResources.EmployeesFinances.Advances;

namespace HumanResources.EmployeesFinances.Forms
{
    public partial class ChangeAmountDateInfoForm : Form
    {
        EmployeeFinanse employeeFinanse;
        ChangeOptions changeOptions;
        public ChangeAmountDateInfoForm(EmployeeFinanse ef, ChangeOptions changeOptions)
        {
            InitializeComponent();
            //pasek tytułowy
            this.Text = DaneFirmy.NazwaProgramu + "Edycja zaliczki";
            //ikona
            this.Icon = Properties.Resources.logo_firmy;
            
            this.employeeFinanse = ef;
            this.changeOptions = changeOptions;

            AssignData();
        }


        float TbZmianaKwoty
        {
            set
            {
                tbAmount.Text = string.Format("{0:N}", value);
            }
        }

        void AssignData()
        {
            switch (changeOptions)
            {
                case ChangeOptions.date:
                    dtpDate.Enabled = true;
                    dtpDate.Value = employeeFinanse.Date;
                    break;
                case ChangeOptions.amount:
                    tbAmount.Enabled = true;
                    tbAmount.Text = employeeFinanse.Amount.ToString();
                    break;
                case ChangeOptions.info:
                    tbInfo.Enabled = true;
                    tbInfo.Text = employeeFinanse.OtherInfo;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Przycisk wprowadzania danych do bazy
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dtpDate.Enabled == true)
                    employeeFinanse.Date = dtpDate.Value.Date;
                if (tbAmount.Enabled == true)
                {
                    employeeFinanse.Amount = Convert.ToSingle(tbAmount.Text.Replace('.', ','));
                }
                if (tbInfo.Enabled == true)
                {
                    if (tbInfo.TextLength > 100)
                        throw new FormatException("Pole Info nie może mieć więcej niż 100 znaków.");
                    employeeFinanse.OtherInfo = tbInfo.Text;
                }
                if (employeeFinanse is Addition)
                    AdditionManager.Edit((Addition)employeeFinanse, ConnectionToDB.disconnect);
                else
                    AdvanceManager.Edit((Advance)employeeFinanse, ConnectionToDB.disconnect);
                EmployeeFinanse.isCorrect = true;
                //zamknięcie formularza
                this.Close();
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message, "Błąd aktualizacji.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex2)
            {
                MessageBox.Show(ex2.Message, "Błąd aktualizacji.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //log
                LogErr.DodajLogErrorDoBazy(new LogErr(Polaczenia.idUser, DateTime.Now, Polaczenia.ip, 0, NazwaTabeli.zaliczka, "ZmianaKwotyDatyForm.btnZatwierdz_Click()/n/n" + ex2.Message));
            }          
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
    }
}
