using System;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;
using HumanResources.Employees;
using HumanResources.WorkTimeRecords;
using HumanResources.EmployeesFinances.Additions;
using HumanResources.EmployeesFinances.Advances;
using HumanResources.Loans;
using HumanResources.Salaries;
using HumanResources.Exceptions;

namespace HumanResources.Employees.Prints
{
    /// <summary>
    /// Przygotowuje wydruk 'Rozliczenia czasu pracy'
    /// </summary>
    public class SettlementTime //1160
    {
        Employee employee;
        EmployeeManager employeeManager = new EmployeeManager();
        int idEmployee;
        DateTime date;
        int xValueWorkEnd;
        int yValueWorkEnd;
        int yValueAdditionsEnd;
        int yValueLoansEnd;
        int yValueAdvancesEnd;
        int spaceFormRightSide = 40;

        /// <summary>
        /// Drukuje Karte pracy
        /// </summary>
        /// <param name="idEmployee">id pracownika</param>
        /// <param name="data">data</param>
        /// <param name="obl">zmienna Obliczenia z danymi na dany miesiąc</param>
        public void Print(int idEmployee, DateTime date)
        {
            this.idEmployee = idEmployee;
            this.date = date;
            //jeżeli po 4.2018 to wydruk 50% 
            if (date.Date < new DateTime(2018, 5, 1))
                throw new WrongDateTimeException("Aby otrzymać wydruk z okresu starszego niż kwiecień 2018/n skontaktuj się z adrministratorem.");
                
            PrintDocument pd = new PrintDocument();
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = pd;
            PrintPreviewDialog prevDialog = new PrintPreviewDialog();
            prevDialog.Document = pd;
            // (4) Przypisuje metodę obsługującą do zdarzenia PrintPage.
            pd.PrintPage += new PrintPageEventHandler(PrintPreparation);

            //podgląd danych
            PrintPreviewDialog podglad = new PrintPreviewDialog();
            podglad.Document = pd;

            // (5) Wyświetla okno dialogowe drukowania i drukuje w odpowiedzi na naciśnięcie przycisku OK.
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                //wyświetla podgląd
                podglad.ShowDialog();

                //drukuje bez podglądu
                //pd.Print(); // Wywołuje zdarzenie PrintPage.
            }
        }

        private void PrintPreparation(object sender, PrintPageEventArgs e)
        {
            // utworzenie fonta
            Font font14 = new Font("Arial", 14, FontStyle.Regular, GraphicsUnit.Point);
            Font font14Bold = new Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Point);

            //kolor pióra czarny
            Pen penBlack = new Pen(Brushes.Black);

            //zmienne lewego górnego punktu
            int y = 30;
            int x = e.PageBounds.Left;

            //pobieranie danych pracownika
            employee = employeeManager.GetEmployee(idEmployee, TableView.view,ConnectionToDB.disconnect);

            //ustawienie tekstu w prostokątach
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;//wśrodkowanie

            //PIERWSZA LINIA
            e.Graphics.DrawLine(penBlack, x, y, e.PageBounds.Right, y);
            Rectangle r_Header = new Rectangle(x, y, e.PageBounds.Right, (int)Math.Ceiling(font14.GetHeight(e.Graphics)));
            e.Graphics.DrawString(string.Format("ROZLICZENIE CZASU PRACY - {0:MMMM} {1:yyy}", date.Date, date.Date), font14Bold, Brushes.Black, r_Header, sf);

            //DRUGA LINIA
            //zwiększenie y o wysokosc prostokata
            y += r_Header.Height;
            //przypisanie nowego Y do prostokąta
            r_Header.Y = y;
            e.Graphics.DrawLine(penBlack, x, y, e.PageBounds.Width, y);//linia
            e.Graphics.DrawString(string.Format("{0} {1}", employee.FirstName, employee.LastName), font14Bold, Brushes.Black, r_Header, sf);
            //zwiekszenie Y o wysokosc prostokoata
            y += r_Header.Height;
            e.Graphics.DrawLine(penBlack, x, y, e.PageBounds.Width, y);//linia

            //dodatkowy odstęp
            y += 10;
            
            FillWorkTime(x,y,sf,e);
            FillAdditions(xValueWorkEnd,y,sf,e);
            FillAdvances(xValueWorkEnd,yValueAdditionsEnd,sf,e);
            FillLoans(xValueWorkEnd,yValueAdvancesEnd,sf,e);
            FillSalaryCalculations(x, yValueWorkEnd > yValueLoansEnd ? yValueWorkEnd : yValueLoansEnd, sf, e);

        }

        private void FillSalaryCalculations(int x, int y, StringFormat sf, PrintPageEventArgs e)
        {
            SalaryDayOff salaryDayOff = new SalaryDayOff(idEmployee, date);
            SalaryIllness salaryIllness = new SalaryIllness(idEmployee, date);
            SalaryWork salaryWork = new SalaryWork(idEmployee, date);
            SalaryLoanInstallment salaryLoanInstallment = new SalaryLoanInstallment();
            int hoursToWork = Salary.GetHoursToWork(date);
            SalaryAdvance salaryAdvance = new SalaryAdvance(idEmployee, date);
            SalaryAddition salaryAddition = new SalaryAddition(idEmployee, date);

            //ustawienie wyrównania go lewej
            sf.Alignment = StringAlignment.Near;

            //odstep miedzy tabelami w pionie
            y += 35;
            Font font12 = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            Font font10 = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            Font font10Bold = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);
            Font font12Bold = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
            Font font14 = new Font("Arial", 14, FontStyle.Regular, GraphicsUnit.Point);
            Font font14Bold = new Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Point);

            //kolor pióra czarny
            Pen penBlack = new Pen(Brushes.Black);

            //główny prostokąt                 
            Rectangle r_Main = new Rectangle(e.PageBounds.Left, y, e.PageBounds.Right - spaceFormRightSide, e.PageBounds.Height - y - spaceFormRightSide * 2);
            e.Graphics.DrawRectangle(penBlack, r_Main);

            //pierwszy prostokąt LEWA strona tabeli
            Rectangle r_Left = new Rectangle();
            SizeF sF_Left;

            //drugi prostokąt Kwota
            SizeF sF_Right;
            Rectangle r_Right = new Rectangle();

            int space = 50;

            //godziny przepracowane
            sF_Left = e.Graphics.MeasureString("Godziny urlopowe (płatne):", font10);
            r_Left.X = r_Main.X + space;
            r_Left.Y = r_Main.Y + space;
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);//szerokość prostokąta wg SizeF zaokraglone
            r_Left.Height = (int)Math.Ceiling(sF_Left.Height);//wysokość prostokąta wg SizeF
            e.Graphics.DrawString("Godziny urlopowe (płatne):", font10, Brushes.Black, r_Left, sf);

            sF_Right = e.Graphics.MeasureString("999:999", font10Bold);
            r_Right.X = r_Left.Right;//obok lewego
            r_Right.Y = r_Left.Y;
            r_Right.Width = (int)Math.Ceiling(sF_Right.Width);//szerokość prostokąta wg SizeF zaokraglone
            r_Right.Height = (int)Math.Ceiling(sF_Right.Height);//wysokość prostokąta wg SizeF
            int temp = salaryDayOff.NumberOfMinutesDayOffPaid;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (temp / 60), (temp % 60)), font10Bold, Brushes.Black, r_Right, sf);


            //godziny urlopowe (bezpłatne)
            sF_Left = e.Graphics.MeasureString("Godziny urlopowe (bezpłatne):", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny urlopowe (bezpłatne):", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempFree = salaryDayOff.NumberOfMinutesDayOffFree;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempFree / 60), (tempFree % 60)), font10Bold, Brushes.Black, r_Right, sf);

            
            //godziny chorobowe 80%
            sF_Left = e.Graphics.MeasureString("Godziny chorobowe (80%):", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny chorobowe (80%):", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempIlness80 = salaryIllness.NumberOfMinutesIllness80;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempIlness80 / 60), (tempIlness80 % 60)), font10Bold, Brushes.Black, r_Right, sf);

            
            //godziny chorobowe 100%
            sF_Left = e.Graphics.MeasureString("Godziny chorobowe (100%):", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny chorobowe (100%):", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempIlness100 = salaryIllness.NumberOfMinutesIllness100;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempIlness100 / 60), (tempIlness100 % 60)), font10Bold, Brushes.Black, r_Right, sf);

            //godziny przepracowane 
            sF_Left = e.Graphics.MeasureString("Godziny przepracowane:", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny przepracowane:", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempWork = salaryWork.NumberOfMinutesRegular;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempWork / 60), (tempWork % 60)), font10Bold, Brushes.Black, r_Right, sf);

            //godziny przepracowane 50
            sF_Left = e.Graphics.MeasureString("Godziny przepracowane (50%):", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny przepracowane (50%):", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempWork50 = salaryWork.NumberOfMinutes50;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempWork50 / 60), (tempWork50 % 60)), font10Bold, Brushes.Black, r_Right, sf);

            //godziny przepracowane 100
            sF_Left = e.Graphics.MeasureString("Godziny przepracowane (100%):", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Godziny przepracowane (100%):", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempWork100 = salaryWork.NumberOfMinutes100;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempWork100 / 60), (tempWork100 % 60)), font10Bold, Brushes.Black, r_Right, sf);


            //pogrubienie SUMA GODZIN
            sF_Left = e.Graphics.MeasureString("Suma godzin:", font10Bold);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Suma godzin:", font10Bold, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            int tempWorkAll = salaryWork.NumberOfMinutesAll + salaryDayOff.NumberOfMinutesDayOffFree + salaryDayOff.NumberOfMinutesDayOffPaid +
                salaryIllness.NumberOfMinutesIllness80 + salaryIllness.NumberOfMinutesIllness100;
            e.Graphics.DrawString(string.Format("{0}:{1:00}", (tempWorkAll / 60), (tempWorkAll % 60)), font10Bold, Brushes.Black, r_Right, sf);


            // normatywny czas pracy
            sF_Left = e.Graphics.MeasureString("Normatywny czas pracy:", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Normatywny czas pracy:", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0}", hoursToWork), font10, Brushes.Black, r_Right, sf);

                                                                        
            //(większy odstęp) - stawka
            sF_Left = e.Graphics.MeasureString("Stawka:", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += (r_Left.Height * 2);
            e.Graphics.DrawString("Stawka:", font10, Brushes.Black, r_Left, sf);

            sF_Right = e.Graphics.MeasureString("9.999,999 zł/mc", font10Bold);
            r_Right.Width = (int)Math.Ceiling(sF_Right.Width);
            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            if (employee.RateRegular.IsMonthlyOrHourly==RateType.hourly)//zł/h
                e.Graphics.DrawString(string.Format("{0} zł/h", employee.RateRegular.RateValue), font10Bold, Brushes.Black, r_Right, sf);
            else
                e.Graphics.DrawString(string.Format("{0} zł/mc", employee.RateRegular.RateValue), font10Bold, Brushes.Black, r_Right, sf);

            //stawka nadodziny
            sF_Left = e.Graphics.MeasureString("Stawka nadgodzinowa:", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Stawka nadgodzinowa:", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0} zł/h", employee.RateOvertime.RateValue), font10Bold, Brushes.Black, r_Right, sf);
                                                                        
            
            //następna linia - pozostało urlopu
            sF_Left = e.Graphics.MeasureString("Pozostało urlopu:", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Pozostało urlopu:", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0}", employee.NumberDaysOffLeft), font10Bold, Brushes.Black, r_Right, sf);


            //następna linia - pozostało pożyczki
            sF_Left = e.Graphics.MeasureString("Pozostało do oddania (pożyczka):", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Pozostało do oddania (pożyczka):", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryLoanInstallment.LoansRemianedToPay(employee.IdEmployee)), font10Bold, Brushes.Black, r_Right, sf);


            /////////////////////////////
            ///////////////////////////// NEXT COLUMN
            /////////////////////////////
            //za godziny
            sF_Left = e.Graphics.MeasureString("Za przepracowane:", font10);
            r_Left.X = (r_Main.Right /2) + space;
            r_Left.Y = r_Main.Y + space;
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            e.Graphics.DrawString("Za przepracowane:", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryWork.CalculateSalaryRegular(employee.RateRegular, hoursToWork)), font10Bold, Brushes.Black, r_Right, sf);

            //za nadgodziny 50
            sF_Left = e.Graphics.MeasureString("Za nadgodziny 50%:", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Za nadgodziny 50%:", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryWork.CalculateSalaryOvertime50(employee.RateOvertime)), font10Bold, Brushes.Black, r_Right, sf);

            //za nadgodziny 100
            sF_Left = e.Graphics.MeasureString("Za nadgodziny 100%:", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Za nadgodziny 100%:", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryWork.CalculateSalaryOvertime100(employee.RateOvertime)), font10Bold, Brushes.Black, r_Right, sf);


            //za urlop
            sF_Left = e.Graphics.MeasureString("Za urlop:", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Za urlop:", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryDayOff.CalculateSalaryDayOff(employee.RateRegular, hoursToWork)), font10Bold, Brushes.Black, r_Right, sf);

            //za chorobowe 80%
            sF_Left = e.Graphics.MeasureString("Za chorobowe (80%):", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Za chorobowe (80%):", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryIllness.CalculateSalaryIllness80(employee.RateRegular, hoursToWork)), font10Bold, Brushes.Black, r_Right, sf);

            //za chorobowe 100%
            sF_Left = e.Graphics.MeasureString("Za chorobowe (100%):", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Za chorobowe (100%):", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryIllness.CalculateSalaryIllness100(employee.RateRegular, hoursToWork)), font10Bold, Brushes.Black, r_Right, sf);

            //następna linia - za wszystko
            sF_Left = e.Graphics.MeasureString("Razem:", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Razem:", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryWork.ForAll + salaryIllness.ForAll + salaryDayOff.ForDayOff), font10Bold, Brushes.Black, r_Right, sf);

            //następna linia - zaliczki
            sF_Left = e.Graphics.MeasureString("Zailczki:", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height * 2;
            e.Graphics.DrawString("Zailczki:", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryAdvance.ForAdvances), font10Bold, Brushes.Black, r_Right, sf);

            //następna linia - pożyczki
            sF_Left = e.Graphics.MeasureString("Pożyczki:", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Pożyczki:", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryLoanInstallment.AmountForPaidOffInstallmentInMonth(employee.IdEmployee, date)), font10Bold, Brushes.Black, r_Right, sf);

            //następna linia - dodatki
            sF_Left = e.Graphics.MeasureString("Dodatki:", font10);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height;
            e.Graphics.DrawString("Dodatki:", font10, Brushes.Black, r_Left, sf);

            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            e.Graphics.DrawString(string.Format("{0:C}", salaryAddition.ForAdditions), font10Bold, Brushes.Black, r_Right, sf);

            //następna linia - do wypłaty
            sF_Left = e.Graphics.MeasureString("Do wypłaty:", font12Bold);
            r_Left.Width = (int)Math.Ceiling(sF_Left.Width);
            r_Left.Y += r_Left.Height * 2;
            e.Graphics.DrawString("Do wypłaty:", font12Bold, Brushes.Black, r_Left, sf);

            sF_Right = e.Graphics.MeasureString("30.524,00 zł", font14Bold);
            r_Right.X = r_Left.Right;
            r_Right.Y = r_Left.Y;
            r_Right.Width = (int)Math.Ceiling(sF_Right.Width);
            r_Right.Height = (int)Math.Ceiling(sF_Right.Height);
            e.Graphics.DrawString(string.Format("{0:C}", salaryWork.ForAll + salaryIllness.ForAll + salaryDayOff.ForDayOff - salaryAdvance.ForAdvances + salaryAddition.ForAdditions - salaryLoanInstallment.ForInstallment), font14Bold, Brushes.Black, r_Right, sf);
        }

        private void FillLoans(int x, int y, StringFormat sf, PrintPageEventArgs e)
        {
            LoanManager.GetLoanToList(idEmployee);

            Font font10 = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            Font font10Bold = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);
            //kolor pióra czarny
            Pen penBlack = new Pen(Brushes.Black);

            //HEADER
            Rectangle r_Header = new Rectangle();

            //DATA
            SizeF sF_Date = e.Graphics.MeasureString("12.34.56789", font10);
            Rectangle r_Date = new Rectangle();
            //Kwota
            SizeF sF_Amount = e.Graphics.MeasureString("20,00000zł", font10);
            Rectangle r_Amount = new Rectangle();
            //NAZWA
            SizeF sF_Other = e.Graphics.MeasureString("123456789012345678901234567890", font10);//nvarchar(30)
            Rectangle r_Odher = new Rectangle();

            //odstęp między tabelami w poziomie
            x += 15;
            //odstęp między tabelami w pionie
            y += 35;

            //tytuł tabeli DODATKI
            r_Header.Width = (e.PageBounds.Right - x - spaceFormRightSide);//max szerokość
            r_Header.Height = (int)Math.Ceiling(font10Bold.GetHeight(e.Graphics));//wysokość
            r_Header.X = x;//obok tabeli godziny
            r_Header.Y = y;
            e.Graphics.DrawString("Pożyczki", font10Bold, Brushes.Black, r_Header, sf);

            //
            //NAGŁÓWKI TABELI Zaliczki
            //
            //DATA
            r_Date.X = x;
            r_Date.Y = r_Header.Bottom;
            r_Date.Width = (int)Math.Ceiling(sF_Date.Width);//szerokość prostokąta wg SizeF zaokraglone
            r_Date.Height = (int)Math.Ceiling(sF_Date.Height);//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("Data", font10Bold, Brushes.Black, r_Date, sf);
            e.Graphics.DrawRectangle(penBlack, r_Date);

            //KWOTA
            r_Amount.X = r_Date.Right;
            r_Amount.Y = r_Date.Y;
            r_Amount.Width = (int)Math.Ceiling(sF_Amount.Width);//szerokość prostokąta wg SizeF
            r_Amount.Height = (int)Math.Ceiling(sF_Amount.Height);//wysokość prostokąta wg SizeF        
            //rysowanie danych
            e.Graphics.DrawString("Kwota", font10Bold, Brushes.Black, r_Amount, sf);
            e.Graphics.DrawRectangle(penBlack, r_Amount);

            //NAZWA
            r_Odher.X = r_Amount.Right;
            r_Odher.Y = r_Date.Y;
            r_Odher.Width = r_Header.Width - r_Date.Width - r_Amount.Width;//Max szerokość prostokąta
            r_Odher.Height = (int)Math.Ceiling(sF_Other.Height);//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("Nazwa", font10Bold, Brushes.Black, r_Odher, sf);
            e.Graphics.DrawRectangle(penBlack, r_Odher);

            float sumAllInstallmentLoans = 0;

            //drukowanie tabeli POZYCZKI
            foreach (Loan l in LoanManager.arrayLoans)
            {
                foreach (LoanInstallment li in l.ArrayInstallmentLoan)
                if(li.Date.Year==date.Year && li.Date.Month==date.Month)
                {
                    //DATA
                    r_Date.Y = r_Date.Bottom;
                    //rysowanie danych
                    e.Graphics.DrawString(string.Format("{0}", li.Date.ToString("d", DateFormat.TakeDateFormatDayFirst())), font10, Brushes.Black, r_Date, sf);
                    e.Graphics.DrawRectangle(penBlack, r_Date);

                    //KWOTA
                    r_Amount.Y = r_Amount.Bottom;
                    //rysowanie danych
                    e.Graphics.DrawString(string.Format("{0:C}", li.InstallmentAmount), font10, Brushes.Black, r_Amount, sf);
                    e.Graphics.DrawRectangle(penBlack, r_Amount);
                    sumAllInstallmentLoans += li.InstallmentAmount;

                    //NAZWA
                    r_Odher.Y = r_Odher.Bottom;
                    //rysowanie danych
                    e.Graphics.DrawString(string.Format("{0}", l.Name), font10, Brushes.Black, r_Odher, sf);
                    e.Graphics.DrawRectangle(penBlack, r_Odher);
                }                              
            }
            r_Header.Y = r_Odher.Bottom;
            e.Graphics.DrawString(string.Format("Suma pożyczek: {0:c}", sumAllInstallmentLoans), font10Bold, Brushes.Black, r_Header, sf);
            yValueLoansEnd = r_Header.Bottom;
        }

        private void FillAdvances(int x, int y, StringFormat sf, PrintPageEventArgs e)
        {
            AdvanceManager.GetAdvancesByDate(idEmployee, date);

            Font font10 = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            Font font10Bold = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);
            //kolor pióra czarny
            Pen penBlack = new Pen(Brushes.Black);

            //HEADER
            Rectangle r_Header = new Rectangle();

            //DATA
            SizeF sF_Date = e.Graphics.MeasureString("12.34.56789", font10);
            Rectangle r_Date = new Rectangle();
            //Kwota
            SizeF sF_Amount = e.Graphics.MeasureString("20,00000zł", font10);
            Rectangle r_Amount = new Rectangle();
            //NAZWA
            SizeF sF_Other = e.Graphics.MeasureString("123456789012345678901234567890", font10);//nvarchar(30)
            Rectangle r_Odher = new Rectangle();

            //odstęp między tabelami w poziomie
            x += 15;
            //odstęp między tabelami w pionie
            y += 35;

            //tytuł tabeli DODATKI
            r_Header.Width = (e.PageBounds.Right - x - spaceFormRightSide);//max szerokość
            r_Header.Height = (int)Math.Ceiling(font10Bold.GetHeight(e.Graphics));//wysokość
            r_Header.X = x;//obok tabeli godziny
            r_Header.Y = y;
            e.Graphics.DrawString("Zaliczki", font10Bold, Brushes.Black, r_Header, sf);

            //
            //NAGŁÓWKI TABELI Zaliczki
            //
            //DATA
            r_Date.X = x;
            r_Date.Y = r_Header.Bottom;
            r_Date.Width = (int)Math.Ceiling(sF_Date.Width);//szerokość prostokąta wg SizeF zaokraglone
            r_Date.Height = (int)Math.Ceiling(sF_Date.Height);//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("Data", font10Bold, Brushes.Black, r_Date, sf);
            e.Graphics.DrawRectangle(penBlack, r_Date);

            //KWOTA
            r_Amount.X = r_Date.Right;
            r_Amount.Y = r_Date.Y;
            r_Amount.Width = (int)Math.Ceiling(sF_Amount.Width);//szerokość prostokąta wg SizeF
            r_Amount.Height = (int)Math.Ceiling(sF_Amount.Height);//wysokość prostokąta wg SizeF        
            //rysowanie danych
            e.Graphics.DrawString("Kwota", font10Bold, Brushes.Black, r_Amount, sf);
            e.Graphics.DrawRectangle(penBlack, r_Amount);

            //NAZWA
            r_Odher.X = r_Amount.Right;
            r_Odher.Y = r_Date.Y;
            r_Odher.Width = r_Header.Width - r_Date.Width - r_Amount.Width;//Max szerokość prostokąta
            r_Odher.Height = (int)Math.Ceiling(sF_Other.Height);//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("Nazwa", font10Bold, Brushes.Black, r_Odher, sf);
            e.Graphics.DrawRectangle(penBlack, r_Odher);

            float sumAllAdvances = 0;

            //drukowanie tabeli ZALICZKI
            foreach (Advance adv in AdvanceManager.arrayListAdvances)
            {
                //DATA
                r_Date.Y = r_Date.Bottom;
                //rysowanie danych
                e.Graphics.DrawString(string.Format("{0}", adv.Date.ToString("d", DateFormat.TakeDateFormatDayFirst())), font10, Brushes.Black, r_Date, sf);
                e.Graphics.DrawRectangle(penBlack, r_Date);

                //KWOTA
                r_Amount.Y = r_Amount.Bottom;
                //rysowanie danych
                e.Graphics.DrawString(string.Format("{0:C}", adv.Amount), font10, Brushes.Black, r_Amount, sf);
                e.Graphics.DrawRectangle(penBlack, r_Amount);
                sumAllAdvances += adv.Amount;

                //NAZWA
                r_Odher.Y = r_Odher.Bottom;
                //rysowanie danych
                e.Graphics.DrawString(string.Format("{0}", adv.OtherInfo), font10, Brushes.Black, r_Odher, sf);
                e.Graphics.DrawRectangle(penBlack, r_Odher);
            }

            r_Header.Y = r_Odher.Bottom;
            e.Graphics.DrawString(string.Format("Suma zaliczek: {0:c}", sumAllAdvances), font10Bold, Brushes.Black, r_Header, sf);
            yValueAdvancesEnd = r_Header.Bottom;
        }

        private void FillAdditions(int x, int y, StringFormat sf, PrintPageEventArgs e)
        {
            AdditionManager.GetAdditionsByDate(idEmployee, date);

            Font font10 = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            Font font10Bold = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);
            //kolor pióra czarny
            Pen penBlack = new Pen(Brushes.Black);

            //HEADER
            Rectangle r_Header = new Rectangle();

            //DATA
            SizeF sF_Date = e.Graphics.MeasureString("12.34.56789", font10);
            Rectangle r_Date = new Rectangle();
            //Kwota
            SizeF sF_Amount = e.Graphics.MeasureString("20,00000zł", font10);
            Rectangle r_Amount = new Rectangle();
            //NAZWA
            SizeF sF_Other = e.Graphics.MeasureString("123456789012345678901234567890", font10);//nvarchar(30)
            Rectangle r_Odher = new Rectangle();

            //odstęp między tabelami w poziomie
            x += 15;

            //tytuł tabeli DODATKI
            r_Header.Width = (e.PageBounds.Right - x - spaceFormRightSide);//max szerokość
            r_Header.Height = (int)Math.Ceiling(font10Bold.GetHeight(e.Graphics));//wysokość
            r_Header.X = x;//obok tabeli godziny
            r_Header.Y = y;
            e.Graphics.DrawString("Dodatki", font10Bold, Brushes.Black, r_Header, sf);

            //
            //NAGŁÓWKI TABELI DODATKI
            //
            //DATA
            r_Date.X = x;
            r_Date.Y = r_Header.Bottom;
            r_Date.Width = (int)Math.Ceiling(sF_Date.Width);//szerokość prostokąta wg SizeF zaokraglone
            r_Date.Height = (int)Math.Ceiling(sF_Date.Height);//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("Data", font10Bold, Brushes.Black, r_Date, sf);
            e.Graphics.DrawRectangle(penBlack, r_Date);

            //KWOTA
            r_Amount.X = r_Date.Right;
            r_Amount.Y = r_Date.Y;
            r_Amount.Width = (int)Math.Ceiling(sF_Amount.Width);//szerokość prostokąta wg SizeF
            r_Amount.Height = (int)Math.Ceiling(sF_Amount.Height);//wysokość prostokąta wg SizeF        
            //rysowanie danych
            e.Graphics.DrawString("Kwota", font10Bold, Brushes.Black, r_Amount, sf);
            e.Graphics.DrawRectangle(penBlack, r_Amount);

            //NAZWA
            r_Odher.X = r_Amount.Right;
            r_Odher.Y = r_Date.Y;
            r_Odher.Width = r_Header.Width - r_Date.Width - r_Amount.Width;//Max szerokość prostokąta
            r_Odher.Height = (int)Math.Ceiling(sF_Other.Height);//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("Nazwa", font10Bold, Brushes.Black, r_Odher, sf);
            e.Graphics.DrawRectangle(penBlack, r_Odher);

            float sumAllAdditions = 0;
            //drukowanie tabeli DODATKI
            foreach (Addition d in AdditionManager.arrayListAddition)
            {
                //DATA
                r_Date.Y = r_Date.Bottom;
                //rysowanie danych
                e.Graphics.DrawString(string.Format("{0}", d.Date.ToString("d", DateFormat.TakeDateFormatDayFirst())), font10, Brushes.Black, r_Date, sf);
                e.Graphics.DrawRectangle(penBlack, r_Date);

                //KWOTA
                r_Amount.Y = r_Amount.Bottom;
                //rysowanie danych
                e.Graphics.DrawString(string.Format("{0:C}", d.Amount), font10, Brushes.Black, r_Amount, sf);
                e.Graphics.DrawRectangle(penBlack, r_Amount);
                sumAllAdditions += d.Amount;

                //NAZWA
                r_Odher.Y = r_Odher.Bottom;
                //rysowanie danych
                e.Graphics.DrawString(string.Format("{0}", d.OtherInfo), font10, Brushes.Black, r_Odher, sf);
                e.Graphics.DrawRectangle(penBlack, r_Odher);
            }
            r_Header.Y = r_Odher.Bottom;
            e.Graphics.DrawString(string.Format("Suma dodatków: {0:c}", sumAllAdditions), font10Bold, Brushes.Black, r_Header, sf);
            yValueAdditionsEnd = r_Header.Bottom;
        }

        private void FillWorkTime(int x, int y, StringFormat sf, PrintPageEventArgs e)
        {
            Font font10 = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            Font font10Bold = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);
            //kolor pióra czarny
            Pen penBlack = new Pen(Brushes.Black);
            //kolor wypełnienia
            Brush brushSzary = Brushes.LightGray;

            //pierwszy prostokąt DATA
            SizeF sF_Date = e.Graphics.MeasureString("2011-03-011", font10);
            Rectangle r_Date = new Rectangle();
            // OD
            SizeF sF_WorkTimeOthers = e.Graphics.MeasureString("Zasiłekk", font10);
            Rectangle r_WorkTimeOthers = new Rectangle();
            // DO
            SizeF sF_WorkTimeTo = e.Graphics.MeasureString("chorobowy 100%%", font10);
            Rectangle r_WorkTimeTo = new Rectangle();
            
            //tytuł tabeli GODZINY
            Rectangle r_Header = new Rectangle(x, y, (int)Math.Ceiling(sF_Date.Width + sF_WorkTimeTo.Width + sF_WorkTimeOthers.Width * 4), (int)Math.Ceiling(font10Bold.GetHeight(e.Graphics)));        
            e.Graphics.DrawString("Godziny", font10Bold, Brushes.Black, r_Header, sf);
            //zwiększa Y o wysokość prostokąta
            y += r_Header.Height;

            //
            //NAGŁÓWKI TABELI GODZINY
            //
            //DATA
            r_Date.X = x;
            r_Date.Y = y;
            r_Date.Width = (int)Math.Ceiling(sF_Date.Width);//szerokość prostokąta wg SizeF zaokraglone
            r_Date.Height = (int)Math.Ceiling(sF_Date.Height);//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("Data", font10Bold, Brushes.Black, r_Date, sf);
            e.Graphics.DrawRectangle(penBlack, r_Date);

            //OD
            r_WorkTimeOthers.X = r_Date.Right;//przesunięcie o szerokość prostokąta DATA
            r_WorkTimeOthers.Y = y;
            r_WorkTimeOthers.Width = (int)Math.Ceiling(sF_WorkTimeOthers.Width);//szerokość prostokąta wg SizeF
            r_WorkTimeOthers.Height = (int)Math.Ceiling(sF_WorkTimeOthers.Height);//wysokość prostokąta wg SizeF
            e.Graphics.DrawString("Od", font10Bold, Brushes.Black, r_WorkTimeOthers, sf);
            e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

            //DO
            r_WorkTimeTo.X = r_WorkTimeOthers.Right;//przesunięcie o szerokość prostokąta DATA + OD
            r_WorkTimeTo.Y = y;
            r_WorkTimeTo.Width = (int)Math.Ceiling(sF_WorkTimeTo.Width);//szerokość prostokąta wg SizeF
            r_WorkTimeTo.Height = (int)Math.Ceiling(sF_WorkTimeTo.Height);//wysokość prostokąta wg SizeF
            e.Graphics.DrawString("Do", font10Bold, Brushes.Black, r_WorkTimeTo, sf);
            e.Graphics.DrawRectangle(penBlack, r_WorkTimeTo);

            //ILOSC GODZIN ALL
            r_WorkTimeOthers.X = r_WorkTimeTo.Right;//przesunięcie o szerokość prostokąta DATA + OD +DO
            e.Graphics.DrawString("Ilość", font10Bold, Brushes.Black, r_WorkTimeOthers, sf);
            e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);


            //ILOSC NADGODZIN 50
            r_WorkTimeOthers.X += r_WorkTimeOthers.Width; //x + r_Date.Width + (r_WorkTimeOthers.Width * 2) + r_WorkTimeTo.Width;
            e.Graphics.DrawString("50%", font10Bold, Brushes.Black, r_WorkTimeOthers, sf);
            e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

            //ILOSC NADGODZIN 100
            r_WorkTimeOthers.X += r_WorkTimeOthers.Width;
            e.Graphics.DrawString("100%", font10Bold, Brushes.Black, r_WorkTimeOthers, sf);
            e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);
            
            WorkManager.arrayListWorkTime.Clear();
            //pobieranie danych z bazy i zapisanie ich do listyGodzin
            WorkManager.GetWorkToList(idEmployee, date, ConnectionToDB.notDisconnect);
            WorkManager.GetDayOffToList(idEmployee, date, ConnectionToDB.notDisconnect);
            WorkManager.GetIllnessToList(idEmployee, date, ConnectionToDB.notDisconnect);
            Holidays.GetAll(date);

            bool isDayPresent = false;
            //drukowanie tabeli GODZINY
            for (int i = 1; i <= DateTime.DaysInMonth(date.Year, date.Month); i++)
            {
                foreach (IWorkTime workTime in WorkManager.arrayListWorkTime)
                {
                    if (workTime is DayOff && workTime.GetDay() == i)
                    {
                        DayOff dayOff = (DayOff)workTime;
                        isDayPresent = true;
                        //DATA
                        r_Date.Y += r_Date.Height;
                        //rysowanie danych
                        if (dayOff.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_Date);
                        e.Graphics.DrawString(string.Format("{0}", dayOff.Date.ToString("d", DateFormat.TakeDateFormatDayFirst())), font10, Brushes.Black, r_Date, sf);
                        e.Graphics.DrawRectangle(penBlack, r_Date);

                        //OD
                        r_WorkTimeOthers.X = r_Date.Right;
                        r_WorkTimeOthers.Y = r_Date.Y;
                        //rysowanie danych
                        if (dayOff.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString("Urlop", font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                        //DO
                        r_WorkTimeTo.Y = r_Date.Y;
                        //rysowanie danych
                        if (dayOff.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeTo);
                        e.Graphics.DrawString(string.Format("{0}", ChangeDayOffTypeToString.ChangeToString((DayOffType)dayOff.IdTypeDayOff)), font10, Brushes.Black, r_WorkTimeTo, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeTo);

                        //ILOSC
                        r_WorkTimeOthers.X = r_WorkTimeTo.Right;
                        //rysowanie danych
                        if (dayOff.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString(dayOff.WorkTimeAll().ToString(DateFormat.TakeTimeSpanFormat()), font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                        //ILOSC 50
                        r_WorkTimeOthers.X += r_WorkTimeOthers.Width;
                        //rysowanie danych
                        if (dayOff.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString("0:00", font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                        //ILOSC 100
                        r_WorkTimeOthers.X += r_WorkTimeOthers.Width;
                        //rysowanie danych
                        if (dayOff.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString("0:00", font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                    }
                    if (workTime is Work && workTime.GetDay() == i)
                    {
                        Work work = (Work)workTime;
                        isDayPresent = true;

                        //DATA
                        r_Date.Y += r_Date.Height;
                        //rysowanie danych
                        if (work.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_Date);
                        e.Graphics.DrawString(string.Format("{0}", work.Date.ToString("d", DateFormat.TakeDateFormatDayFirst())), font10, Brushes.Black, r_Date, sf);
                        e.Graphics.DrawRectangle(penBlack, r_Date);

                        //OD
                        r_WorkTimeOthers.X = r_Date.Right;
                        r_WorkTimeOthers.Y = r_Date.Y;
                        //rysowanie danych
                        if (work.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString(work.StartTime.ToString("t", DateFormat.TakeTimeFormat()), font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                        //DO
                        r_WorkTimeTo.Y = r_Date.Y;
                        //rysowanie danych
                        if (work.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeTo);
                        e.Graphics.DrawString(work.StopTime.ToString("t", DateFormat.TakeTimeFormat()), font10, Brushes.Black, r_WorkTimeTo, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeTo);

                        //ILOSC ALL
                        r_WorkTimeOthers.X = r_WorkTimeTo.Right;
                        //rysowanie danych
                        if (work.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString(work.WorkTimeAll().ToString(DateFormat.TakeTimeSpanFormat()), font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                        //ILOSC 50
                        r_WorkTimeOthers.X += r_WorkTimeOthers.Width;
                        //rysowanie danych
                        if (work.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString(work.WorkTime50().ToString(DateFormat.TakeTimeSpanFormat()), font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                        //ILOSC 100
                        r_WorkTimeOthers.X += r_WorkTimeOthers.Width;
                        //rysowanie danych
                        if (work.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString(work.WorkTime100().ToString(DateFormat.TakeTimeSpanFormat()), font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                    }
                    if (workTime is Illness && workTime.GetDay() == i)
                    {
                        Illness illness = (Illness)workTime;
                        isDayPresent = true;

                        //DATA
                        r_Date.Y += r_Date.Height;
                        //rysowanie danych
                        if (illness.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_Date);
                        e.Graphics.DrawString(illness.Date.ToString("d", DateFormat.TakeDateFormatDayFirst()), font10, Brushes.Black, r_Date, sf);
                        e.Graphics.DrawRectangle(penBlack, r_Date);

                        //OD
                        r_WorkTimeOthers.X = r_Date.Right;
                        r_WorkTimeOthers.Y  = r_Date.Y;
                        //rysowanie danych
                        if (illness.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString("Zasiłek", font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                        //DO
                        r_WorkTimeTo.Y = r_Date.Y;
                        //rysowanie danych
                        if (illness.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeTo);
                        e.Graphics.DrawString(ChangeIllnessTypeToString.ChangeToString((IllnessType)illness.IdIllnessType), font10, Brushes.Black, r_WorkTimeTo, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeTo);

                        //ILOSC
                        r_WorkTimeOthers.X = r_WorkTimeTo.Right;
                        //rysowanie danych
                        if (illness.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString(illness.WorkTimeAll().ToString(DateFormat.TakeTimeSpanFormat()), font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                        //ILOSC NADGODZIN 50
                        r_WorkTimeOthers.X += r_WorkTimeOthers.Width;
                        //rysowanie danych
                        if (illness.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString("0:00", font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                        //ILOSC NADGODZIN 100
                        r_WorkTimeOthers.X += r_WorkTimeOthers.Width;
                        //rysowanie danych
                        if (illness.isHolidayOrWeekend()) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString("0:00", font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                    }
                }
                    if (!isDayPresent)
                    {
                        DateTime tempDate = new DateTime(date.Year, date.Month, i);
                        
                        //DATA
                        r_Date.Y += r_Date.Height;
                        //rysowanie danych
                        if (Holidays.IsHolidayOrWeekend(tempDate)) e.Graphics.FillRectangle(brushSzary, r_Date);
                        e.Graphics.DrawString(tempDate.ToString("d", DateFormat.TakeDateFormatDayFirst()), font10, Brushes.Black, r_Date, sf);
                        e.Graphics.DrawRectangle(penBlack, r_Date);

                        //OD
                        r_WorkTimeOthers.X = r_Date.Right;
                        r_WorkTimeOthers.Y = r_Date.Y;
                    //rysowanie danych
                    if (Holidays.IsHolidayOrWeekend(tempDate)) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString("", font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                        //DO
                        r_WorkTimeTo.Y = r_Date.Y;
                    //rysowanie danych
                    if (Holidays.IsHolidayOrWeekend(tempDate)) e.Graphics.FillRectangle(brushSzary, r_WorkTimeTo);
                        e.Graphics.DrawString("", font10, Brushes.Black, r_WorkTimeTo, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeTo);

                        //ILOSC
                        r_WorkTimeOthers.X = r_WorkTimeTo.Right;
                        //rysowanie danych
                        if (Holidays.IsHolidayOrWeekend(tempDate)) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString("", font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                        //ILOSC NADGODZIN 50
                        r_WorkTimeOthers.X += r_WorkTimeOthers.Width;
                        //rysowanie danych
                        if (Holidays.IsHolidayOrWeekend(tempDate)) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString("", font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);

                        //ILOSC NADGODZIN 100
                        r_WorkTimeOthers.X += r_WorkTimeOthers.Width;
                        //rysowanie danych
                        if (Holidays.IsHolidayOrWeekend(tempDate)) e.Graphics.FillRectangle(brushSzary, r_WorkTimeOthers);
                        e.Graphics.DrawString("", font10, Brushes.Black, r_WorkTimeOthers, sf);
                        e.Graphics.DrawRectangle(penBlack, r_WorkTimeOthers);
                    }
                    
                isDayPresent = false;
            }
            // XY końca tabeli godziny
            xValueWorkEnd = r_WorkTimeOthers.Right;
            yValueWorkEnd = r_WorkTimeOthers.Y;
        }
    }
}
