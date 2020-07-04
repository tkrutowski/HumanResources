using HumanResources.Salaries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.MainForm
{
    class CalculateSalary
    {        
        internal static void Calculate(MainForm form, int idEmployee, DateTime date)
        {
            Employees.EmployeeManager employeeManager = new Employees.EmployeeManager();
            Employees.Employee employee =  employeeManager.GetEmployee(idEmployee, TableView.view, ConnectionToDB.disconnect);
            int hoursToWork = Salary.GetHoursToWork(date);
            int sumAllMinutes = 0;

            SalaryWork salaryWork = new SalaryWork(idEmployee, date);
            form.LblIloscGodzinDoPrzepracowania = hoursToWork.ToString();
            form.LblGodzinyPrzepracowane = salaryWork.NumberOfMinutesRegular.ToString();
            form.LblGodzinyNadliczbowe50 = salaryWork.NumberOfMinutes50.ToString();
            form.LblGodzinyNadliczbowe100 = salaryWork.NumberOfMinutes100.ToString();
            sumAllMinutes += salaryWork.NumberOfMinutesAll;
            form.LblZaGodziny = salaryWork.CalculateSalaryRegular(employee.RateRegular, hoursToWork);
            form.LblZaNadgodziny50 = salaryWork.CalculateSalaryOvertime50(employee.RateOvertime);
            form.LblZaNadgodziny100 = salaryWork.CalculateSalaryOvertime100(employee.RateOvertime);

            SalaryDayOff salaryDayOff = new SalaryDayOff(idEmployee, date);
            form.LblGodzinyUrlopowePlatne = salaryDayOff.NumberOfMinutesDayOffPaid.ToString();
            form.LblGodzinyUrlopoweBezplatne = salaryDayOff.NumberOfMinutesDayOffFree.ToString();
            sumAllMinutes += (salaryDayOff.NumberOfMinutesDayOffFree + salaryDayOff.NumberOfMinutesDayOffPaid);
            form.LblPozostaloUrlopu = employee.NumberDaysOffLeft;
            form.LblZaUrlopowe = salaryDayOff.CalculateSalaryDayOff(employee.RateRegular, hoursToWork);

            SalaryIllness salaryIllness = new SalaryIllness(idEmployee, date);
            form.LblGodzinyChorobowe80 = salaryIllness.NumberOfMinutesIllness80.ToString();
            form.LblGodzinyChorobowe100 = salaryIllness.NumberOfMinutesIllness100.ToString();
            sumAllMinutes += (salaryIllness.NumberOfMinutesIllness80 + salaryIllness.NumberOfMinutesIllness100);
            form.LblZaChorobowe80 = salaryIllness.CalculateSalaryIllness80(employee.RateRegular, hoursToWork);
            form.LblZaChorobowe100 = salaryIllness.CalculateSalaryIllness100(employee.RateRegular, hoursToWork);

            SalaryAddition salaryAddition = new SalaryAddition(idEmployee, date);
            form.LblZaDodatek = salaryAddition.ForAdditions;

            SalaryAdvance salaryAdvance = new SalaryAdvance(idEmployee, date);
            form.LblZaZaliczke = salaryAdvance.ForAdvances;
                       
            SalaryLoanInstallment salaryLoanInstallment = new SalaryLoanInstallment();
            form.LblPozostaloPozyczki = salaryLoanInstallment.LoansRemianedToPay(employee.IdEmployee);
            form.LblZaPozyczke = salaryLoanInstallment.AmountForPaidOffInstallmentInMonth(employee.IdEmployee, date);
                                 
            form.LblSumaGodzin = sumAllMinutes.ToString();
            form.LblZaWszystko = salaryWork.ForAll + salaryIllness.ForAll + salaryDayOff.ForDayOff;
            form.LblDoWyplaty = salaryWork.ForAll + salaryIllness.ForAll + salaryDayOff.ForDayOff - salaryAdvance.ForAdvances + salaryAddition.ForAdditions - salaryLoanInstallment.ForInstallment;
            form.LblStawka = employee.RateRegular.RateValue;
            form.LblStawkaNadgodzinowa = employee.RateOvertime.RateValue;
        }
        internal static void Calculate_OLD(MainForm form, int idEmployee, DateTime date)
        {
            Employees.EmployeeManager employeeManager = new Employees.EmployeeManager();
            Employees.Employee employee = employeeManager.GetEmployee(idEmployee, TableView.view, ConnectionToDB.disconnect);
            int hoursToWork = Salary.GetHoursToWork(date);
            int sumAllMinutes = 0;

            SalaryDayOff salatyDayOff = new SalaryDayOff(idEmployee, date);
            form.LblGodzinyUrlopowePlatne = salatyDayOff.NumberOfMinutesDayOffPaid.ToString();
            form.LblGodzinyUrlopoweBezplatne = salatyDayOff.NumberOfMinutesDayOffFree.ToString();
            sumAllMinutes += (salatyDayOff.NumberOfMinutesDayOffFree + salatyDayOff.NumberOfMinutesDayOffPaid);
            form.LblPozostaloUrlopu = employee.NumberDaysOffLeft;
            form.LblZaUrlopowe = salatyDayOff.CalculateSalaryDayOff(employee.RateRegular, hoursToWork);

            SalaryIllness salaryIllness = new SalaryIllness(idEmployee, date);
            form.LblGodzinyChorobowe80 = salaryIllness.NumberOfMinutesIllness80.ToString();
            form.LblGodzinyChorobowe100 = salaryIllness.NumberOfMinutesIllness100.ToString();
            sumAllMinutes += (salaryIllness.NumberOfMinutesIllness80 + salaryIllness.NumberOfMinutesIllness100);
            form.LblZaChorobowe80 = salaryIllness.CalculateSalaryIllness80(employee.RateRegular, hoursToWork);
            form.LblZaChorobowe100 = salaryIllness.CalculateSalaryIllness100(employee.RateRegular, hoursToWork);


            SalaryAddition salaryAddition = new SalaryAddition(idEmployee, date);
            form.LblZaDodatek = salaryAddition.ForAdditions;

            SalaryAdvance salaryAdvance = new SalaryAdvance(idEmployee, date);
            form.LblZaZaliczke = salaryAdvance.ForAdvances;

            SalaryWorkOLD salaryWork = new SalaryWorkOLD(idEmployee, date);
            form.LblIloscGodzinDoPrzepracowania = hoursToWork.ToString();
            form.LblStawka = employee.RateRegular.RateValue;
            form.LblStawkaNadgodzinowa = employee.RateOvertime.RateValue;
            salaryWork.NumberOfMinutesOver = sumAllMinutes;//musi być przed obliczeniami
            sumAllMinutes += salaryWork.NumberOfMinutesAll;
            form.LblGodzinyNadliczbowe = salaryWork.NumberOfMinutesOver.ToString();


            SalaryLoanInstallment salaryLoanInstallment = new SalaryLoanInstallment();
            form.LblPozostaloPozyczki = salaryLoanInstallment.LoansRemianedToPay(employee.IdEmployee);
            form.LblZaPozyczke = salaryLoanInstallment.AmountForPaidOffInstallmentInMonth(employee.IdEmployee, date);

            form.LblSumaGodzin = sumAllMinutes.ToString();
            form.LblIloscGodzinDoPrzepracowania = hoursToWork.ToString();


            form.LblZaGodziny = salaryWork.CalculateSalaryRegular(employee.RateRegular, hoursToWork);
            form.LblZaNadgodziny = salaryWork.CalculateSalaryOvertime_OLD(employee.RateOvertime);
            form.LblZaWszystko = salaryWork.ForAll + salaryIllness.ForAll + salatyDayOff.ForDayOff;
            form.LblDoWyplaty = salaryWork.ForAll + salaryIllness.ForAll + salatyDayOff.ForDayOff - salaryAdvance.ForAdvances + salaryAddition.ForAdditions - salaryLoanInstallment.ForInstallment;
            form.LblStawka = employee.RateRegular.RateValue;
            form.LblStawkaNadgodzinowa = employee.RateOvertime.RateValue;
        }
    }
}
