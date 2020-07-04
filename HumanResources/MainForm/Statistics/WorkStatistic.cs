using HumanResources.Exceptions;
using HumanResources.Salaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.DataVisualization.Charting;

namespace HumanResources.MainForm
{
    class WorkStatistic
    {
        public static void DisplayChart(MainForm form)
        {
            //zmienne
            DateTime date = new DateTime(form.dtpStatystykiWyborDaty.Value.Date.Year, 1, 1);
            int idEmployee = Convert.ToInt32(form.cbStatisticSelectEmployee.SelectedValue);
            Employees.EmployeeManager employeeManager = new Employees.EmployeeManager();
            Employees.Employee employee = employeeManager.GetEmployee(idEmployee, TableView.view, ConnectionToDB.disconnect);
            int hoursToWork;// = Salary.GetHoursToWork(date);
            double salaryYearly = 0;
            double salaryMonthy = 0;

            //zerowanie punktów żeby przy wyświetlaniu kolejnego roku się nie dodawały
            foreach (Series s in form.chart1.Series)
                s.Points.Clear();

            form.lblStatystykiWyplatyRazem.Text = "...";
            form.lblStatystykiWyplatySrednio.Text = "...";
            MainForm.progressLoading = 0;

            //pętla wyświetlająca miesiące
            for (int i = 1; i < 13; i++)
            {
                hoursToWork = Salary.GetHoursToWork(date);
                try
                {
                    SalaryWork salaryWork = new SalaryWork(idEmployee, date);
                    salaryWork.CalculateAll(employee.RateRegular, hoursToWork, employee.RateOvertime);
                    form.chart1.Series["godziny"].Points.AddXY(i, salaryWork.ForAll);
                    salaryMonthy += salaryWork.ForAll;
                }
                catch (NoNullException)
                {
                    form.chart1.Series["godziny"].Points.AddXY(i, 0);
                }

                SalaryDayOff salaryDayOff = new SalaryDayOff(idEmployee, date);
                form.chart1.Series["urlop"].Points.AddXY(i, salaryDayOff.CalculateSalaryDayOff(employee.RateRegular, hoursToWork));

                SalaryIllness salaryIllness = new SalaryIllness(idEmployee, date);
                salaryIllness.CalculateAll(employee.RateRegular, hoursToWork);
                form.chart1.Series["choroba"].Points.AddXY(i, salaryIllness.ForAll);

                SalaryAddition salaryAddition = new SalaryAddition(idEmployee, date);
                form.chart1.Series["dodatki"].Points.AddXY(i, salaryAddition.ForAdditions);

                salaryMonthy += (salaryIllness.ForAll + salaryDayOff.ForDayOff + salaryAddition.ForAdditions);
                form.chart1.Series["godziny"].Points[i - 1].ToolTip = string.Format("{0:C}", salaryMonthy);
                form.chart1.Series["urlop"].Points[i - 1].ToolTip = string.Format("{0:C}", salaryMonthy);
                form.chart1.Series["choroba"].Points[i - 1].ToolTip = string.Format("{0:C}", salaryMonthy);
                form.chart1.Series["dodatki"].Points[i - 1].ToolTip = string.Format("{0:C}", salaryMonthy);

                //dodanie do podsumowania
                salaryYearly += salaryMonthy;

                date = date.AddMonths(1);
                //dodaje do paska postępu
                MainForm.progressLoading += 8;
                salaryMonthy = 0;
            }

            //wyświetlenie podliczeń
            form.lblStatystykiWyplatySrednio.Text = string.Format("{0:C}", salaryYearly / 12);
            form.lblStatystykiWyplatyRazem.Text = string.Format("{0:C}", salaryYearly);

            foreach (Series s in form.chart1.Series)
                s.Enabled = true;

            //nie wyświetla kwoty gdy =0.00
            foreach (Series s in form.chart1.Series)
                for (int i = 0; i < 12; i++)
                {
                    if (s.Points[i].YValues[0] == 0)
                        s.Points[i].Label = string.Format(" ");
                    else
                        s.Points[i].Label = string.Format("{0:C}", s.Points[i].YValues[0]);
                }
        }
    }
}
