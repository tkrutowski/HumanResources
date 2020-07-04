using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace HumanResources.Calendar
{
    public partial class CalendarControl : UserControl
    {
        private DateTime date;
        private int day;
        ArrayList arrayListEmployees;
    
        /// <summary>
        /// Konstruktor 
        /// Tworzy controlkę z wypisanym dniem miesiąca
        /// </summary>
        /// <param name="day"></param>
        public CalendarControl(int day)
        {
            InitializeComponent();

            this.day = day;

            DisplayEntryDay();
        }
        /// <summary>
        /// Konstruktor
        /// Tworzy kontrolkę z dniem tygodnia, oraz wypisuje pracowników
        /// którzy mają urlop danego dnia
        /// </summary>
        /// <param name="day">dzień miesiąca</param>
        /// <param name="arrayEmployeesFullName">tablica pracowników (lastName + ' ' + firstName)</param>
        public CalendarControl(int day, ArrayList arrayEmployeesFullName)
        {
            InitializeComponent();

            this.day = day;
            this.arrayListEmployees = arrayEmployeesFullName;
            DisplayEntryDay();

            DisplayEntryDayOff();
        }
     
        private void DisplayEntryDay()
        {
            this.lblDay.Text = day.ToString();
        }

        private void DisplayEntryDayOff()
        {
            if(arrayListEmployees.Count>0)
            {
               // this.flowLayoutPanel1.Controls.Add(new LabelDayOff()); //pusty wiersz
                this.flowLayoutPanel1.Controls.Add(new LabelDayOff().AddText("Pracownicy na urlopie:"));
            }
            foreach(string s  in arrayListEmployees)
            {
                this.flowLayoutPanel1.Controls.Add(new LabelDayOff().AddText(" - " +s));
            }
        }

        private void CalendarControl_Load(object sender, EventArgs e)
        {
            this.Dock = DockStyle.Fill;
        }

        
    }
}
