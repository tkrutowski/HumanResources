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

namespace WorkTimeRecords.Prints
{
    class TimeRecordSheet
    {
        DateTime date;
        EmployeeManager employeeManager = new EmployeeManager();
        Employee employee;
        int idEmployee;
        int j;

        public void Print(DateTime data)
        {
            date = data;
            employeeManager.GetEmployeesHiredRealesedToList(true, TableView.table);

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
            //na początku ustawia id pracownika na pierwszą wartość na liście
            idEmployee = (EmployeeManager.arrayEmployees[j] as Employee).IdEmployee;
            //następnie zwiększa i o 1
            j++;

            // utworzenie fonta
            Font font12 = new Font("Arial", 12, FontStyle.Regular, GraphicsUnit.Point);
            Font font10 = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            Font font10Bold = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);
            Font font12Bold = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
            Font font14 = new Font("Arial", 14, FontStyle.Regular, GraphicsUnit.Point);
            Font font14Bold = new Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Point);

            //ustawienie wysokości fonta
            float wysokoscFonta10 = font10.GetHeight(e.Graphics);
            float wysokoscFonta12 = font12.GetHeight(e.Graphics);
            float wysokoscFonta14 = font14.GetHeight(e.Graphics);

            //kolor pióra czarny
            Pen penBlack = new Pen(Brushes.Black);

            //kolor wypełnienia
            Brush brushSzary = Brushes.LightGray;

            //zmienne lewego górnego punktu
            int y = 30;
            int x = e.MarginBounds.Left;

            //zmienna zawierająca 1/3 szerokosci strony
            int jednaTrzecia = (e.MarginBounds.Width / 3);

            //pobieranie danych pracownika
            foreach (Employee p in EmployeeManager.arrayEmployees)
            {
                if (p.IdEmployee == idEmployee)
                    employee = p;
            }

            //ustawienie tekstu w prostokątach
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;//wśrodkowanie
            sf.LineAlignment = StringAlignment.Center;

            //PIERWSZA LINIA
            e.Graphics.DrawLine(penBlack, x, y, e.MarginBounds.Right, y);
            Rectangle rect = new Rectangle(x, y, e.MarginBounds.Right, (int)Math.Ceiling(font14.GetHeight(e.Graphics)));
            e.Graphics.DrawString(string.Format("EWIDENCJA CZASU PRACY - {0:MMMM} {1:yyy}", date.Date, date.Date), font14Bold, Brushes.Black, rect, sf);

            //DRUGA LINIA
            //zwiększenie y o wysokosc prostokata
            y += rect.Height;
            //przypisanie nowego Y do prostokąta
            rect.Y = y;
            e.Graphics.DrawLine(penBlack, x, y, e.MarginBounds.Right, y);//linia
            e.Graphics.DrawString(string.Format("{0} {1}", employee.FirstName, employee.LastName), font14Bold, Brushes.Black, rect, sf);
            //zwiekszenie Y o wysokosc prostokoata
            y += rect.Height;
            e.Graphics.DrawLine(penBlack, x, y, e.MarginBounds.Right, y);//linia

            //dodatkowy odstęp
            y += 10;

            #region NAGŁÓWKI TABELI

            //pierwszy prostokąt DZIEŃ
            SizeF sF_Dzien = e.Graphics.MeasureString("poniedziałek", font10Bold);
            Rectangle r_Dzien = new Rectangle();
            //data
            Rectangle r_DzienNr = new Rectangle();
            //słownie
            Rectangle r_DzienTyg = new Rectangle();

            //drugi prostokąt PODPIS PRACOWNIKA
            SizeF sF_Podpis = e.Graphics.MeasureString("pracownika", font10Bold);
            Rectangle r_Podpis = new Rectangle();

            //trzeci prostokąt CZAS PRACY
            Rectangle r_CzasPracy = new Rectangle();
            //rozpoczecie
            Rectangle r_CzasPracyRozp = new Rectangle();
            //zakonczenie
            Rectangle r_CzasPracyZakon = new Rectangle();
            //ilosc godz
            Rectangle r_CzasPracyIlosc = new Rectangle();

            //czwarty prostokąt NIEOBECNOSCI
            Rectangle r_Nieobecnosci = new Rectangle();
            //urlop
            Rectangle r_NieobecnosciUrlop = new Rectangle();
            //L4
            Rectangle r_NieobecnosciL4 = new Rectangle();
            //inne
            Rectangle r_NieobecnosciInne = new Rectangle();

            //zwiększa Y o wysokość prostokąta
            y += rect.Height;

            //
            //NAGŁÓWKI TABELI 
            //DZIEŃ
            //
            r_Dzien.X = x;
            r_Dzien.Y = y;
            r_Dzien.Width = (int)((jednaTrzecia * 60) / 100);// szerokość prostokąta 60% z 1/3
            r_Dzien.Height = font10Bold.Height * 3;//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("dzień", font10Bold, Brushes.Black, r_Dzien, sf);
            e.Graphics.DrawRectangle(penBlack, r_Dzien);

            //PODPIS PRACOWNIKA
            r_Podpis.X = r_Dzien.Right;
            r_Podpis.Y = r_Dzien.Y;
            r_Podpis.Width = (int)((jednaTrzecia * 40) / 100);
            r_Podpis.Height = r_Dzien.Height;
            //rysowanie danych
            e.Graphics.DrawString("podpis pracownika", font10Bold, Brushes.Black, r_Podpis, sf);
            e.Graphics.DrawRectangle(penBlack, r_Podpis);

            //
            //CZAS PRACY
            //
            r_CzasPracy.X = r_Podpis.Right;
            r_CzasPracy.Y = r_Dzien.Y;
            r_CzasPracy.Width = jednaTrzecia;//szerokość prostokąta 1/3 strony
            r_CzasPracy.Height = r_Dzien.Height/ 2;//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("czas pracy", font10Bold, Brushes.Black, r_CzasPracy, sf);
            e.Graphics.DrawRectangle(penBlack, r_CzasPracy);

            //rozp
            r_CzasPracyRozp.X = r_Podpis.Right;
            r_CzasPracyRozp.Y = r_Dzien.Y + r_CzasPracy.Height;//+ wys CZAS PRACY
            r_CzasPracyRozp.Width = r_CzasPracy.Width / 3;//szerokość prostokąta 1/3 CZAS PRACY
            r_CzasPracyRozp.Height = r_Dzien.Height / 2;//wysokość prostokąta 
            //rysowanie danych
            e.Graphics.DrawString("rozp.", font10, Brushes.Black, r_CzasPracyRozp, sf);
            e.Graphics.DrawRectangle(penBlack, r_CzasPracyRozp);

            //zakonczenie
            r_CzasPracyZakon.X = r_CzasPracyRozp.Right;
            r_CzasPracyZakon.Y = r_CzasPracyRozp.Y;
            r_CzasPracyZakon.Width = r_CzasPracy.Width / 3;//szerokość prostokąta 1/3 CZAS PRACY
            r_CzasPracyZakon.Height = r_Dzien.Height / 2;//wysokość prostokąta 
             //rysowanie danych
            e.Graphics.DrawString("zakończ.", font10, Brushes.Black, r_CzasPracyZakon, sf);
            e.Graphics.DrawRectangle(penBlack, r_CzasPracyZakon);

            //ilosc godzin
            r_CzasPracyIlosc.X = r_CzasPracyZakon.Right;
            r_CzasPracyIlosc.Y = r_CzasPracyRozp.Y;
            r_CzasPracyIlosc.Width = r_CzasPracy.Width / 3;//szerokość prostokąta 1/3 CZAS PRACY
            r_CzasPracyIlosc.Height = r_Dzien.Height / 2;//wysokość prostokąta 
            //rysowanie danych
            e.Graphics.DrawString("ilość godz.", font10, Brushes.Black, r_CzasPracyIlosc, sf);
            e.Graphics.DrawRectangle(penBlack, r_CzasPracyIlosc);

            //
            //NIEOBECNOŚCI
            //
            r_Nieobecnosci.X = r_CzasPracy.Right;
            r_Nieobecnosci.Y = r_Dzien.Y;
            r_Nieobecnosci.Width = jednaTrzecia;//szerokość prostokąta wg SizeF
            r_Nieobecnosci.Height = r_Dzien.Height / 2;//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("nieobecności", font10Bold, Brushes.Black, r_Nieobecnosci, sf);
            e.Graphics.DrawRectangle(penBlack, r_Nieobecnosci);

            //urlop
            r_NieobecnosciUrlop.X = r_CzasPracyIlosc.Right;
            r_NieobecnosciUrlop.Y = r_Dzien.Y + r_Nieobecnosci.Height;
            r_NieobecnosciUrlop.Width = r_Nieobecnosci.Width / 3;// 1/3 szerokości NIEOBECNOSCI
            r_NieobecnosciUrlop.Height = r_Dzien.Height / 2;//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("urlop", font10, Brushes.Black, r_NieobecnosciUrlop, sf);
            e.Graphics.DrawRectangle(penBlack, r_NieobecnosciUrlop);

            //L4
            r_NieobecnosciL4.X = r_NieobecnosciUrlop.Right;
            r_NieobecnosciL4.Y = r_NieobecnosciUrlop.Y;
            r_NieobecnosciL4.Width = r_Nieobecnosci.Width / 3;// 1/3 szerokości NIEOBECNOSCI
            r_NieobecnosciL4.Height = r_Dzien.Height / 2;//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("zwoln. lek.", font10, Brushes.Black, r_NieobecnosciL4, sf);
            e.Graphics.DrawRectangle(penBlack, r_NieobecnosciL4);

            //inne
            r_NieobecnosciInne.X = r_NieobecnosciL4.Right;
            r_NieobecnosciInne.Y = r_NieobecnosciUrlop.Y;
            r_NieobecnosciInne.Width = r_Nieobecnosci.Width / 3;// 1/3 szerokości NIEOBECNOSCI
            r_NieobecnosciInne.Height = r_Dzien.Height / 2;//wysokość prostokąta wg SizeF
            //rysowanie danych
            e.Graphics.DrawString("inne", font10, Brushes.Black, r_NieobecnosciInne, sf);
            e.Graphics.DrawRectangle(penBlack, r_NieobecnosciInne);

            #endregion

            #region tabela

            //
            //przygotowanie zmiennych
            //
            y = r_Dzien.Bottom;
            //data potrzebna w pętli do wypisywania dni
            DateTime dataTemp;
            //DZIEŃ NR
            r_DzienNr.X = x;
            r_DzienNr.Y = y;
            r_DzienNr.Width = ((r_Dzien.Width * 25) / 100);// szerokość prostokąta 25% z 1/3 DZIEN
            r_DzienNr.Height = font14Bold.Height;//wysokość prostokąta 
                              
            //DZIEŃ TYG
            r_DzienTyg.X = r_DzienNr.Right;
            r_DzienTyg.Y = y;
            r_DzienTyg.Width = r_Dzien.Width - r_DzienNr.Width;
            r_DzienTyg.Height = font14Bold.Height;//wysokość prostokąta 

            //podpis
            r_Podpis.Y = y;
            r_Podpis.Height = font14Bold.Height;//wysokość prostokąta 

            //rozp
            r_CzasPracyRozp.Y = y;
            r_CzasPracyRozp.Height = font14Bold.Height;//wysokość prostokąta 

            //zakńcz
            r_CzasPracyZakon.Y = y;
            r_CzasPracyZakon.Height = font14Bold.Height;//wysokość prostokąta 

            //ilosc godzin
            r_CzasPracyIlosc.Y = y;
            r_CzasPracyIlosc.Height = font14Bold.Height;//wysokość prostokąta 

            //urlop
            r_NieobecnosciUrlop.Y = y;
            r_NieobecnosciUrlop.Height = font14Bold.Height;//wysokość prostokąta 

            //L4
            r_NieobecnosciL4.Y = y;
            r_NieobecnosciL4.Height = font14Bold.Height;//wysokość prostokąta 

            //inne
            r_NieobecnosciInne.Y = y;
            r_NieobecnosciInne.Height = font14Bold.Height;//wysokość prostokąta 

            //pobieranie godzin z bazy do listy            
            Holidays.GetAll(date);
            //ustawia date na 1 zien danego miesiaca
            dataTemp = new DateTime(date.Year, date.Month, 1);


            //Tworzenie tabeli
            for (int i = 1; i <= DateTime.DaysInMonth(date.Year, date.Month); i++)
            {
                //wypełnienie prostokąta na szaro gdy weekend lub
                //dniWolne ma id (gdy nie ma to zanczy że to nie świeto tylko brak wpisu)
                if (Holidays.IsHolidayOrWeekend(dataTemp))
                {
                    e.Graphics.FillRectangle(brushSzary, r_DzienNr);
                    e.Graphics.FillRectangle(brushSzary, r_DzienTyg);
                    e.Graphics.FillRectangle(brushSzary, r_Podpis);
                    e.Graphics.FillRectangle(brushSzary, r_CzasPracyRozp);
                    e.Graphics.FillRectangle(brushSzary, r_CzasPracyZakon);
                    e.Graphics.FillRectangle(brushSzary, r_CzasPracyIlosc);
                    e.Graphics.FillRectangle(brushSzary, r_NieobecnosciUrlop);
                    e.Graphics.FillRectangle(brushSzary, r_NieobecnosciL4);
                    e.Graphics.FillRectangle(brushSzary, r_NieobecnosciInne);
                }

                //dzień NR
                e.Graphics.DrawString(i.ToString(), font10, Brushes.Black, r_DzienNr, sf);
                e.Graphics.DrawRectangle(penBlack, r_DzienNr);
                //dzień TYG
                e.Graphics.DrawString(string.Format("{0:dddd}", dataTemp), font10, Brushes.Black, r_DzienTyg, sf);
                e.Graphics.DrawRectangle(penBlack, r_DzienTyg);
                //pozostałe
                e.Graphics.DrawRectangle(penBlack, r_Podpis);
                e.Graphics.DrawRectangle(penBlack, r_CzasPracyRozp);
                e.Graphics.DrawRectangle(penBlack, r_CzasPracyZakon);
                e.Graphics.DrawRectangle(penBlack, r_CzasPracyIlosc);
                e.Graphics.DrawRectangle(penBlack, r_NieobecnosciUrlop);
                e.Graphics.DrawRectangle(penBlack, r_NieobecnosciL4);
                e.Graphics.DrawRectangle(penBlack, r_NieobecnosciInne);

                //dodaje kolejny dzień
                dataTemp = dataTemp.AddDays(1);
                //kolejny wiersz
                r_DzienNr.Y += r_DzienNr.Height;
                r_DzienTyg.Y += r_DzienTyg.Height;
                r_Podpis.Y += r_Podpis.Height;
                r_CzasPracyRozp.Y += r_CzasPracyRozp.Height;
                r_CzasPracyZakon.Y += r_CzasPracyZakon.Height;
                r_CzasPracyIlosc.Y += r_CzasPracyIlosc.Height;
                r_NieobecnosciUrlop.Y += r_NieobecnosciUrlop.Height;
                r_NieobecnosciL4.Y += r_NieobecnosciL4.Height;
                r_NieobecnosciInne.Y += r_NieobecnosciInne.Height;
            }

            #endregion

            #region podliczenia
            
            //suma godzin
            rect.X = r_Dzien.Right;
            rect.Y = r_DzienNr.Y;
            rect.Height = font14Bold.Height;
            rect.Width = r_Podpis.Width + r_CzasPracyRozp.Width + r_CzasPracyZakon.Width;
            //wyrównanie do prawej
            sf.Alignment = StringAlignment.Far;
            //pozostałe prostokąty
            e.Graphics.DrawString("suma godzin przepracowanych:", font10Bold, Brushes.Black, rect, sf);
            e.Graphics.DrawRectangle(penBlack, r_CzasPracyIlosc);//rysowanie prostokąta
            e.Graphics.DrawRectangle(penBlack, r_NieobecnosciUrlop);//rysowanie prostokąta
            e.Graphics.DrawRectangle(penBlack, r_NieobecnosciL4);//rysowanie prostokąta
            e.Graphics.DrawRectangle(penBlack, r_NieobecnosciInne);//rysowanie prostokąta

            //normatywny czas pracy
            rect.Y += rect.Height;
            e.Graphics.DrawString("normatywny czas pracy:", font10Bold, Brushes.Black, rect, sf);
            //ilosc godz
            rect.X = r_CzasPracyIlosc.X;
            rect.Width = r_CzasPracyIlosc.Width;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString(string.Format("{0}h", HumanResources.Salaries.Salary.GetHoursToWork(date)), font10Bold, Brushes.Black, rect, sf);
            //pozostałe prostokąty
            r_CzasPracyIlosc.Y += r_CzasPracyIlosc.Height;//zmiana Y
            e.Graphics.DrawRectangle(penBlack, r_CzasPracyIlosc);//rysowanie prostokąta
            r_NieobecnosciUrlop.Y += r_NieobecnosciUrlop.Height;//zmiana Y
            e.Graphics.DrawRectangle(penBlack, r_NieobecnosciUrlop);//rysowanie prostokąta
            r_NieobecnosciL4.Y += r_NieobecnosciL4.Height;//zmiana Y
            e.Graphics.DrawRectangle(penBlack, r_NieobecnosciL4);//rysowanie prostokąta
            r_NieobecnosciInne.Y += r_NieobecnosciInne.Height;//zmiana Y
            e.Graphics.DrawRectangle(penBlack, r_NieobecnosciInne);//rysowanie prostokąta

            //liczba nadgodzin
            rect.X = r_Dzien.Right;
            rect.Y = r_CzasPracyIlosc.Bottom;
            rect.Width = r_Podpis.Width + r_CzasPracyRozp.Width + r_CzasPracyZakon.Width;
            //wyrównanie do prawej
            sf.Alignment = StringAlignment.Far;
            //pozostałe prostokąty
            e.Graphics.DrawString("liczba nadgodzin:", font10Bold, Brushes.Black, rect, sf);
            r_CzasPracyIlosc.Y += r_CzasPracyIlosc.Height;//zmiana Y
            e.Graphics.DrawRectangle(penBlack, r_CzasPracyIlosc);//rysowanie prostokąta
            r_NieobecnosciUrlop.Y += r_NieobecnosciUrlop.Height;//zmiana Y
            e.Graphics.DrawRectangle(penBlack, r_NieobecnosciUrlop);//rysowanie prostokąta
            r_NieobecnosciL4.Y += r_NieobecnosciL4.Height;//zmiana Y
            e.Graphics.DrawRectangle(penBlack, r_NieobecnosciL4);//rysowanie prostokąta
            r_NieobecnosciInne.Y += r_NieobecnosciInne.Height;//zmiana Y
            e.Graphics.DrawRectangle(penBlack, r_NieobecnosciInne);//rysowanie prostokąta
            #endregion

            //sprawdzenie czy trzeba drukować więcej stron
            if (EmployeeManager.arrayEmployees.Count > j)
            {
                e.HasMorePages = true;
            }

            //jeżeli jest wł podgląd wydruku, to trzeba wyzerować zmienną, ponieważ podgląd zwiększa o jeden i następnie drukowanie
            //zaczyna od zwiększonego j zamiast o zera i jest błąd indeksu
            if (EmployeeManager.arrayEmployees.Count == j)
                j = 0;
        }
    }
}
