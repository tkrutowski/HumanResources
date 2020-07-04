using HumanResources.Employees;
using HumanResources.WorkTimeRecords;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Ustawienia.DaneFirmy;

namespace HumanResources.WorkTimeRecords.Prints
{
    public class AttendanceList
    {
        DateTime date;
        EmployeeManager employeeManager = new EmployeeManager();
        DaneFirmy daneFirmy = new DaneFirmy();
        //licznik pracowników na strone
        int employeeCounterPerPage = 0;
        ArrayList tempList;
        string workTime = "";

        public void PrintAll(DateTime data)
        {
            date = data;
            daneFirmy = DaneFirmy.PobieranieDanychFirmy();
            //kadra
            employeeManager.GetEmployeesIsHiredIsManagementToList(true, true, TableView.table);
            tempList = new ArrayList(EmployeeManager.arrayEmployees);
            workTime = "Czas pracy 07:00-15:00";
            Print();
            //cały etat
            employeeManager.GetEmployeesIsHiredIsManagementIsFullTimeToList(true, false, true, TableView.table);
            tempList = new ArrayList(EmployeeManager.arrayEmployees);
            workTime = "Czas pracy 07:00-11:00";
            Print();
            //pół etatu
            employeeManager.GetEmployeesIsHiredIsManagementIsFullTimeToList(true, false, false, TableView.table);
            tempList = new ArrayList(EmployeeManager.arrayEmployees);
            workTime = "Czas pracy 07:00-15:00";
            Print();
        }
        private void Print()
        {
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

            Margins margins = new Margins(20, 50, 20, 20);
            pd.DefaultPageSettings.Margins = margins;

            // (5) Wyświetla okno dialogowe drukowania i drukuje w odpowiedzi na naciśnięcie przycisku OK.
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                //wyświetla podgląd
                prevDialog.ShowDialog();

                //drukuje bez podglądu
                //pd.Print(); // Wywołuje zdarzenie PrintPage.
            }
        }

        private void PrintPreparation(object sender, PrintPageEventArgs e)
        {
            //zerowanie iloci pracowników, żeby można było drukować kolejne 6 imion na nastepnej stronie
            employeeCounterPerPage = 0;
            // utworzenie fonta
            Font font12 = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            Font font10 = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            Font font10Bold = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);
            Font font12Bold = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
            Font font14 = new Font("Arial", 14, FontStyle.Regular, GraphicsUnit.Point);
            Font font14Bold = new Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Point);

            //kolor pióra czarny
            Pen penBlack = new Pen(Brushes.Black);

            //kolor wypełnienia
            Brush brushSzary = Brushes.LightGray;

            //zmienne lewego górnego punktu
            int y = 30;
            int x = e.MarginBounds.Left;
            int cellWidth = 0;
            int cellHeight = 0;

            //ustawienie tekstu w prostokątach
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;//wśrodkowanie
            sf.LineAlignment = StringAlignment.Center;

            //PIERWSZA LINIA
            e.Graphics.DrawLine(penBlack, x, y, e.MarginBounds.Right, y);
            Rectangle rect = new Rectangle(x, y, e.MarginBounds.Right, (int)Math.Ceiling(font14.GetHeight(e.Graphics)));
            e.Graphics.DrawString(string.Format("LISTA OBECNOŚCI - {0:MMMM} {1:yyy} - {2}", date.Date, date.Date, workTime), font14Bold, Brushes.Black, rect, sf);

            //DRUGA LINIA
            //zwiększenie y o wysokosc prostokata
            y += Convert.ToInt32(rect.Height);
            //przypisanie nowego Y do prostokąta
            rect.Y = y;
            e.Graphics.DrawLine(penBlack, x, y, e.MarginBounds.Right, y);//linia
            e.Graphics.DrawString(string.Format("{0}", daneFirmy.Nazwa), font14Bold, Brushes.Black, rect, sf);
            //zwiekszenie Y o wysokosc prostokoata
            y += Convert.ToInt32(rect.Height); ;
            e.Graphics.DrawLine(penBlack, x, y, e.MarginBounds.Right, y);//linia

            //dodatkowy odstęp
            y += 10;

            #region NAGŁÓWKI TABELI

            //pierwszy prostokąt DZIEŃ
            SizeF sF_Dzien = e.Graphics.MeasureString("#DNI#", font12Bold);
            Rectangle r_Dzien = new Rectangle();
            //prac1
            Rectangle r_Prac1 = new Rectangle();
            //prac2          
            Rectangle r_Prac2 = new Rectangle();
            //prac3
            Rectangle r_Prac3 = new Rectangle();
            //prac4
            Rectangle r_Prac4 = new Rectangle();
            //prac5
            Rectangle r_Prac5 = new Rectangle();
            //prac6;
            Rectangle r_Prac6 = new Rectangle();

            //zwiększa Y o wysokość prostokąta
            y += Convert.ToInt32(rect.Height);

            //
            //NAGŁÓWKI TABELI 
            //
            //Dni
            r_Dzien.X = x;
            r_Dzien.Y = y;
            r_Dzien.Width = (int)Math.Ceiling(sF_Dzien.Width);
            r_Dzien.Height = font12Bold.Height * 3;
            //rysowanie danych
            e.Graphics.DrawString("Dni", font12Bold, Brushes.Black, r_Dzien, sf);
            e.Graphics.DrawRectangle(penBlack, r_Dzien);

            //PRAC1
            r_Prac1.X = r_Dzien.Right;
            r_Prac1.Y = y;
            r_Prac1.Width = (e.MarginBounds.Width - r_Dzien.Width) / 6;
            r_Prac1.Height = r_Dzien.Height;

            //wypisuje nazwiska pracowników
            foreach (Employee emp in tempList)
            {
                //rysowanie danych
                e.Graphics.DrawString(string.Format("{0}\n {1}", emp.FirstName, emp.LastName), font12Bold, Brushes.Black, r_Prac1, sf);
                e.Graphics.DrawRectangle(penBlack, r_Prac1);

                //nastepny
                r_Prac1.X += r_Prac1.Width;

                //licznik pracownikow powyżej 6 przerywa foreach
                employeeCounterPerPage++;
                if (employeeCounterPerPage > 5)
                {
                    break;
                }
            }

            //dorysowanie pustych prostokątów (bez imion)
            for (int i = employeeCounterPerPage; i < 6; i++)
            {
                e.Graphics.DrawRectangle(penBlack, r_Prac1);
                r_Prac1.X += r_Prac1.Width;
            }

            #endregion

            #region tabela

            cellWidth = (e.MarginBounds.Width - r_Dzien.Width) / 6;
            cellHeight = font14Bold.Height;//wysokość prostokąta 
            //
            //przygotowanie zmiennych
            //
            y = r_Dzien.Bottom;
            //data potrzebna w pętli do wypisywania dni
            DateTime dataTemp;
            //DZIEŃ NR
            r_Dzien.X = x;
            r_Dzien.Y = y;
            r_Dzien.Height = cellHeight;

            //PRAC1
            r_Prac1.X = r_Dzien.Right;
            r_Prac1.Y = y;
            r_Prac1.Width = cellWidth;
            r_Prac1.Height = cellHeight;

            //PRAC2
            r_Prac2.X = r_Prac1.Right;
            r_Prac2.Y = y;
            r_Prac2.Width = cellWidth;
            r_Prac2.Height = font14Bold.Height;//wysokość prostokąta 

            //PRAC3
            r_Prac3.X = r_Prac2.Right;
            r_Prac3.Y = y;
            r_Prac3.Width = cellWidth;
            r_Prac3.Height = font14Bold.Height;//wysokość prostokąta 

            //PRAC4
            r_Prac4.X = r_Prac3.Right;
            r_Prac4.Y = y;
            r_Prac4.Width = cellWidth;
            r_Prac4.Height = font14Bold.Height;//wysokość prostokąta

            //PRAC5
            r_Prac5.X = r_Prac4.Right;
            r_Prac5.Y = y;
            r_Prac5.Width = cellWidth;
            r_Prac5.Height = font14Bold.Height;//wysokość prostokąta 

            //PRAC6
            r_Prac6.X = r_Prac5.Right;
            r_Prac6.Y = y;
            r_Prac6.Width = cellWidth;
            r_Prac6.Height = font14Bold.Height;//wysokość prostokąta 

            //ustawia date na 1 dzien danego miesiaca
            dataTemp = new DateTime(date.Year, date.Month, 1);

            //Tworzenie tabeli
            for (int i = 1; i <= DateTime.DaysInMonth(date.Year, date.Month); i++)
            {
                //wypełnienie prostokąta na szaro gdy weekend lub
                //dniWolne ma id (gdy nie ma to zanczy że to nie świeto tylko brak wpisu)
                if (Holidays.IsHolidayOrWeekend(dataTemp))
                {
                    e.Graphics.FillRectangle(brushSzary, r_Dzien);
                    e.Graphics.FillRectangle(brushSzary, r_Prac1);
                    e.Graphics.FillRectangle(brushSzary, r_Prac2);
                    e.Graphics.FillRectangle(brushSzary, r_Prac3);
                    e.Graphics.FillRectangle(brushSzary, r_Prac4);
                    e.Graphics.FillRectangle(brushSzary, r_Prac5);
                    e.Graphics.FillRectangle(brushSzary, r_Prac6);
                }

                //dzień NR
                e.Graphics.DrawString(i.ToString(), font12Bold, Brushes.Black, r_Dzien, sf);
                e.Graphics.DrawRectangle(penBlack, r_Dzien);

                //pozostałe
                e.Graphics.DrawRectangle(penBlack, r_Prac1);
                e.Graphics.DrawRectangle(penBlack, r_Prac2);
                e.Graphics.DrawRectangle(penBlack, r_Prac3);
                e.Graphics.DrawRectangle(penBlack, r_Prac4);
                e.Graphics.DrawRectangle(penBlack, r_Prac5);
                e.Graphics.DrawRectangle(penBlack, r_Prac6);

                //dodaje kolejny dzień
                dataTemp = dataTemp.AddDays(1);
                //kolejny wiersz
                r_Dzien.Y += r_Dzien.Height;
                r_Prac1.Y += r_Prac1.Height;
                r_Prac2.Y += r_Prac2.Height;
                r_Prac3.Y += r_Prac3.Height;
                r_Prac4.Y += r_Prac4.Height;
                r_Prac5.Y += r_Prac5.Height;
                r_Prac6.Y += r_Prac6.Height;
            }
            #endregion


            //sprawdzenie czy trzeba drukować więcej stron
            //if (EmployeeManager.arrayEmployees.Count > 6)
            if (tempList.Count > 6)
            {
                tempList.RemoveRange(0, 6);
                e.HasMorePages = true;
            }
            else
            {
                //przypisuje oryginalną liste do listyPracownikow do drukowania
                //żeby odświerzyć listę w przypadku drukowania z podglądem
                tempList = EmployeeManager.arrayEmployees;
            }

        }
    }
}
