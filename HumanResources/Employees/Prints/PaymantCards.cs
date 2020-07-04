using HumanResources.Employees;
using HumanResources.Exceptions;
using HumanResources.Salaries;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HumanResources.Employees.Prints
{
    class PaymantCards //450
    {
        DateTime date;
        int idEmployee;
        //licznik pracowników na strone
        int employeeCounter = 0;
        //zmienna określająca ilośc pozycji na wydruku kart pracy - po 5 (od zera)
        const int employeesPerPage = 4;
        //zmienna określająca ilośc pozycji na wydrukach kart pracy - 6, 12, 18....
        int sumEmployeesPerPage = employeesPerPage;
        int space = 40;
        EmployeeManager employeeManager = new EmployeeManager();
        int hoursToWork;
        int y;
        int x;

        /// <summary>
        /// Drukuje kartki do wypłaty
        /// </summary>
        /// <param name="data">data (miesiąc, rok)</param>
        public void Print(DateTime data, int idEmployee = -1)
        {
            this.date = data;
            this.idEmployee = idEmployee;

            //jeżeli po 4.2018 to wydruk 50% 
            if (data.Date < new DateTime(2018, 5, 1))
                throw new WrongDateTimeException("Aby otrzymać wydruk z okresu starszego niż kwiecień 2018/n skontaktuj się z adrministratorem.");

            if (idEmployee != -1)//jeden pracownik
            {
                EmployeeManager.arrayEmployees.Clear();
                EmployeeManager.arrayEmployees.Add(employeeManager.GetEmployee(idEmployee, TableView.view, ConnectionToDB.disconnect));
            }
            else//lub wszyscy
            {
                //pobieranie pracowników zatrudnionych z bazy
                employeeManager.GetEmployeesHiredRealesedToList(true, TableView.view);
            }
            hoursToWork = Salary.GetHoursToWork(date);
            // (1) Tworzy obiekt klasy PrintDocument.
            PrintDocument pd = new PrintDocument();
            // (2) Tworzy obiekt klasy PrintDialog.
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;
            // (3) Tworzy obiekt klasy PrintPreviewDialog.
            PrintPreviewDialog prevDialog = new PrintPreviewDialog();
            prevDialog.Document = pd;
            // (4) Przypisuje metodę obsługującą do zdarzenia PrintPage.
            pd.PrintPage += new PrintPageEventHandler(PrintPreparation);
            //podgląd danych
            PrintPreviewDialog podgladPrint = new PrintPreviewDialog();
            podgladPrint.Document = pd;
            // (5) Wyświetla okno dialogowe drukowania i drukuje w odpowiedzi na naciśnięcie przycisku OK.
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                //wyświetla podgląd
                //    podgladPrint.ShowDialog();
                //drukuje bez podglądu
                pd.Print(); // Wywołuje zdarzenie PrintPage.
            }
        }
        private void PrintPreparation(object sender, PrintPageEventArgs e)
        {
            //zmienne lewego górnego punktu
            y = 30;
            x = e.PageBounds.Left;

            //pętla drukująca 6 kartek na strone
            for (; employeeCounter < EmployeeManager.arrayEmployees.Count; employeeCounter++)
            {
                Employee employee = (Employee)EmployeeManager.arrayEmployees[employeeCounter];
                //FillPage(x,y,employee,e);

                if (FillPage(x, y, employee, e))
                {
                    e.HasMorePages = true;
                    break;
                }
                else
                    e.HasMorePages = false;
            }

            //OSTATNIA LINIA      
            Pen penBlack = new Pen(Brushes.Black);
            e.Graphics.DrawLine(penBlack, x, y, e.PageBounds.Right - 25, y);
            //zwiększenie licznika
            employeeCounter++;
            sumEmployeesPerPage += employeesPerPage + 1;//ponieważ zaczynał od zera...
        }

        private bool FillPage(int x, int y, Employee employee, PrintPageEventArgs e)
        {
            // utworzenie fonta
            Font font10 = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            Font font10Bold = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);
            Font font8 = new Font("Arial", 8, FontStyle.Regular, GraphicsUnit.Point);
            Font font8Bold = new Font("Arial", 8, FontStyle.Bold, GraphicsUnit.Point);

            //kolor pióra czarny
            Pen penBlack = new Pen(Brushes.Black);

            SalaryDayOff salaryDayOff = new SalaryDayOff(employee.IdEmployee, date);
            SalaryIllness salaryIllness = new SalaryIllness(employee.IdEmployee, date);
            SalaryWork salaryWork = new SalaryWork(employee.IdEmployee, date);
            SalaryLoanInstallment salaryLoanInstallment = new SalaryLoanInstallment();
            SalaryAdvance salaryAdvance = new SalaryAdvance(employee.IdEmployee, date);
            SalaryAddition salaryAddition = new SalaryAddition(employee.IdEmployee, date);

            StringFormat sf = new StringFormat();
            //ustawienie tekstu w prostokątach
            sf.Alignment = StringAlignment.Center;//środek

            //pierwszy prostokąt LEWA strona tabeli
            SizeF sF_Left;
            Rectangle r_Left = new Rectangle();

            //drugi prostokąt Kwota
            SizeF sF_right;
            Rectangle r_Right = new Rectangle();

            //PIERWSZA LINIA
            e.Graphics.DrawLine(penBlack, x, y, e.PageBounds.Right - 25, y);
            Rectangle rect = new Rectangle(x, y + 10, e.PageBounds.Right - 25, (int)Math.Ceiling(font10.GetHeight(e.Graphics)));
            e.Graphics.DrawString(string.Format("ROZLICZENIE CZASU PRACY - {0} {1}, {2}.{3}",
                employee.FirstName, employee.LastName, date.Month, date.Year), font10Bold, Brushes.Black, rect, sf);

            ///////////////////////////////////////////
            ///////////////////////////////////////////     LEFT COLUMN
            //////////////////////////////////////////////
            sf.Alignment = StringAlignment.Near;//do lewej

            //godziny przepracowane
            sF_Left = e.Graphics.MeasureString("Godziny przepracowane:", font8);
            r_Left.X = x + space;
            r_Left.Y = rect.Bottom;
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Height = (int)Math.Ceiling(sF_Left.Height);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny przepracowane:", font8, Brushes.Black, r_Left, sf);

            sF_right = e.Graphics.MeasureString("999:999", font8Bold);
            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            r_Right.Width = (int)Math.Ceiling(sF_right.Width);
            r_Right.Height = (int)Math.Ceiling(sF_right.Height);
            int tempWork = salaryWork.NumberOfMinutesRegular;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempWork / 60), (tempWork % 60)), font8Bold, Brushes.Black, r_Right, sf);


            //ilość godzin nadliczbowych 50%
            sF_Left = e.Graphics.MeasureString("Godziny przepracowane 50%:", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny przepracowane (50%):", font8, Brushes.Black, r_Left, sf);


            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempWork50 = salaryWork.NumberOfMinutes50;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempWork50 / 60), (tempWork50 % 60)), font8Bold, Brushes.Black, r_Right, sf);


            //ilość godzin nadliczbowych 100%
            sF_Left = e.Graphics.MeasureString("Godziny przepracowane (100%):", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny przepracowane (100%):", font8, Brushes.Black, r_Left, sf);


            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempWork100 = salaryWork.NumberOfMinutes100;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempWork100 / 60), (tempWork100 % 60)), font8Bold, Brushes.Black, r_Right, sf);


            //godziny urlopowe (płatne)
            sF_Left = e.Graphics.MeasureString("Godziny urlopowe (płatne):", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny urlopowe (płatne):", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int temp = salaryDayOff.NumberOfMinutesDayOffPaid;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (temp / 60), (temp % 60)), font8Bold, Brushes.Black, r_Right, sf);

            // godziny urlopowe (bezpłatne)
            sF_Left = e.Graphics.MeasureString("Godziny urlopowe (bezpłatne):", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny urlopowe (bezpłatne):", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempFree = salaryDayOff.NumberOfMinutesDayOffFree;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempFree / 60), (tempFree % 60)), font8Bold, Brushes.Black, r_Right, sf);

            //godziny chorobowe 80%
            sF_Left = e.Graphics.MeasureString("Godziny chorobowe (80%):", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny chorobowe (80%):", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempIlness80 = salaryIllness.NumberOfMinutesIllness80;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempIlness80 / 60), (tempIlness80 % 60)), font8Bold, Brushes.Black, r_Right, sf);

            //godziny chorobowe 100%
            sF_Left = e.Graphics.MeasureString("Godziny chorobowe (100%):", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny chorobowe (100%):", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempIlness100 = salaryIllness.NumberOfMinutesIllness100;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempIlness100 / 60), (tempIlness100 % 60)), font8Bold, Brushes.Black, r_Right, sf);

            //pogrubienie SUMA GODZIN
            sF_Left = e.Graphics.MeasureString("Suma godzin:", font8Bold);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Suma godzin:", font8Bold, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempWorkAll = salaryWork.NumberOfMinutesAll + salaryDayOff.NumberOfMinutesDayOffFree + salaryDayOff.NumberOfMinutesDayOffPaid +
            salaryIllness.NumberOfMinutesIllness80 + salaryIllness.NumberOfMinutesIllness100;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempWorkAll / 60), (tempWorkAll % 60)), font8Bold, Brushes.Black, r_Right, sf);

            //normatywny czas pracy
            sF_Left = e.Graphics.MeasureString("Normatywny czas pracy:", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height * 2;
            e.Graphics.DrawString("Normatywny czas pracy:", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0}", hoursToWork), font8, Brushes.Black, r_Right, sf);

            ////////////////////////////////
            ///////////////////////////////MIDDLE COLUMN
            ///////////////////////////////
            //za godziny
            sF_Left = e.Graphics.MeasureString("Za przepracowane:", font8);
            r_Left.X = r_Right.Right + space * 2;
            r_Left.Y = rect.Bottom + r_Left.Height;
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            e.Graphics.DrawString("Za przepracowane:", font8, Brushes.Black, r_Left, sf);

            sF_right = e.Graphics.MeasureString("300.524,00 zł", font8Bold);
            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            e.Graphics.DrawString(string.Format("{0:C}", salaryWork.CalculateSalaryRegular(employee.RateRegular, hoursToWork)), font8Bold, Brushes.Black, r_Right, sf);

            // za nadgodziny 50%
            sF_Left = e.Graphics.MeasureString("Za nadgodziny (50%):", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Za nadgodziny (50%):", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryWork.CalculateSalaryOvertime50(employee.RateOvertime)), font8Bold, Brushes.Black, r_Right, sf);

            //następna linia - za nadgodziny 100%
            sF_Left = e.Graphics.MeasureString("Za nadgodziny (100%):", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Za nadgodziny (100%):", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryWork.CalculateSalaryOvertime100(employee.RateOvertime)), font8Bold, Brushes.Black, r_Right, sf);

            //następna linia - za urlop
            sF_Left = e.Graphics.MeasureString("Za urlop:", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Za urlop:", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryDayOff.CalculateSalaryDayOff(employee.RateRegular, hoursToWork)), font8Bold, Brushes.Black, r_Right, sf);

            //następna linia - za chorobowe 80%
            sF_Left = e.Graphics.MeasureString("Za chorobowe (80%):", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Za chorobowe (80%):", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryIllness.CalculateSalaryIllness80(employee.RateRegular, hoursToWork)), font8Bold, Brushes.Black, r_Right, sf);

            //następna linia - za chorobowe 100%
            sF_Left = e.Graphics.MeasureString("Za chorobowe (100%):", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Za chorobowe (100%):", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryIllness.CalculateSalaryIllness100(employee.RateRegular, hoursToWork)), font8Bold, Brushes.Black, r_Right, sf);


            //następna linia - za wszystko
            sF_Left = e.Graphics.MeasureString("Razem:", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Razem:", font8Bold, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.X + r_Left.Width;//obok lewego
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryWork.ForAll + salaryIllness.ForAll + salaryDayOff.ForDayOff), font8Bold, Brushes.Black, r_Right, sf);

            //następna linia - pozostało urlopu
            sF_Left = e.Graphics.MeasureString("Pozostało urlopu:", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height * 2;
            e.Graphics.DrawString("Pozostało urlopu:", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.X + r_Left.Width;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0}", employee.NumberDaysOffLeft), font8Bold, Brushes.Black, r_Right, sf);

            //następna linia - pozostało pożyczki
            sF_Left = e.Graphics.MeasureString("Pozostało do oddania (pożyczka):", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Pozostało do oddania (pożyczka):", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.X + r_Left.Width;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryLoanInstallment.LoansRemianedToPay(employee.IdEmployee)), font8Bold, Brushes.Black, r_Right, sf);

            /////////////////////////////////////
            ////////////////////////////////////    RIGHT COLUMN
            ////////////////////////////////////
            //zaliczki
            sF_Left = e.Graphics.MeasureString("Zailczki:", font8);
            r_Left.X = r_Right.Right + space;
            r_Left.Y = rect.Bottom + r_Left.Height;
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            e.Graphics.DrawString("Zailczki:", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.X + r_Left.Width;//obok lewego
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryAdvance.ForAdvances), font8Bold, Brushes.Black, r_Right, sf);

            // pożyczki
            sF_Left = e.Graphics.MeasureString("Pożyczki:", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Pożyczki:", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.X + r_Left.Width;//obok lewego
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryLoanInstallment.AmountForPaidOffInstallmentInMonth(employee.IdEmployee, date)), font8Bold, Brushes.Black, r_Right, sf);

            // dodatki
            sF_Left = e.Graphics.MeasureString("Dodatki:", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Dodatki:", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.X + r_Left.Width;//obok lewego
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryAddition.ForAdditions), font8Bold, Brushes.Black, r_Right, sf);

            // (większy odstęp) - stawka
            sF_Left = e.Graphics.MeasureString("Stawka:", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += (r_Left.Height * 2);
            e.Graphics.DrawString("Stawka:", font8, Brushes.Black, r_Left, sf);

            sF_right = e.Graphics.MeasureString("9.999,999 zł/mc", font8Bold);
            r_Right.Width = (int)Math.Ceiling(sF_right.Width);
            r_Right.X = r_Left.X + r_Left.Width;
            r_Right.Y = r_Left.Y;
            if (employee.RateRegular.IsMonthlyOrHourly == RateType.hourly)//zł/h
                e.Graphics.DrawString(string.Format("{0} zł/h", employee.RateRegular.RateValue), font8Bold, Brushes.Black, r_Right, sf);
            else
                e.Graphics.DrawString(string.Format("{0} zł/mc", employee.RateRegular.RateValue), font8Bold, Brushes.Black, r_Right, sf);


            // stawka nadodziny
            sF_Left = e.Graphics.MeasureString("Stawka nadgodzinowa:", font8);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Stawka nadgodzinowa:", font8, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.X + r_Left.Width;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0} zł/h", employee.RateOvertime.RateValue), font8Bold, Brushes.Black, r_Right, sf);

            //następna linia - do wypłaty
            sF_Left = e.Graphics.MeasureString("Do wypłaty:", font10Bold);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Height = (int)Math.Ceiling(sF_Left.Height);
            r_Left.Y += r_Left.Height * 2;
            e.Graphics.DrawString("Do wypłaty:", font10, Brushes.Black, r_Left, sf);

            sF_right = e.Graphics.MeasureString("30.524,00 zł", font10Bold);
            r_Right.X = r_Left.X + r_Left.Width;//obok lewego
            r_Right.Y = r_Left.Y;
            r_Right.Width = (int)Math.Ceiling(sF_right.Width);//szerokość prostokąta wg SizeF zaokraglone
            r_Right.Height = (int)Math.Ceiling(sF_right.Height);//wysokość prostokąta wg SizeF
            e.Graphics.DrawString(string.Format("{0:C}", salaryWork.ForAll + salaryIllness.ForAll + salaryDayOff.ForDayOff - salaryAdvance.ForAdvances + salaryAddition.ForAdditions - salaryLoanInstallment.ForInstallment), font10Bold, Brushes.Black, r_Right, sf);

            //przypisanie zmiennych dla nowego pracownika
            this.y = r_Right.Bottom + r_Right.Height * 2;

            if (employeeCounter == sumEmployeesPerPage)
            {
                return true;
            }

            return false;
        }
    }
}
