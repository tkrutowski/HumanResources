using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HumanResources.Salaries;
using HumanResources.Employees;

namespace Employees.Tests
{
    [TestClass]
    public class SalaryTest
    {
        EmployeeManager employeeManager = new EmployeeManager();
            Employee employee = new Employee();
            int hoursToWork = 168;
        DateTime date = DateTime.Now;

        [TestMethod]
        public void CalculateSalaryRegularHourlyTest()
        {
            //Arange
            RateRegular rateRegular = new RateRegular(DateTime.Now, 20.3f, RateType.hourly);
            employee.RateRegular = rateRegular;
            SalaryWork salaryWork = new SalaryWork(employee.IdEmployee, date);

            salaryWork.NumberOfMinutesAll = 10080 + 1590 + 1955;//168+...
            salaryWork.NumberOfMinutes100 = 1590;//26:30
            salaryWork.NumberOfMinutes50 = 1955;//32:35
            
            double should = 3410.40d;

            //Act
            var result = salaryWork.CalculateSalaryRegular(rateRegular, hoursToWork);

            //Arrange
            Assert.AreEqual(should, result);
        }
        public void CalculateSalaryRegularMonthlyTest()
        {
            //Arange
            RateRegular rateRegular = new RateRegular(DateTime.Now, 3333f, RateType.monthly);
            employee.RateRegular = rateRegular;
            SalaryWork salaryWork = new SalaryWork(employee.IdEmployee, date);

            salaryWork.NumberOfMinutesAll = 9120 + 1590 + 1955;//152+...
            salaryWork.NumberOfMinutes100 = 1590;//26:30
            salaryWork.NumberOfMinutes50 = 1955;//32:35

            double should = 3015.58d;

            //Act
            var result = salaryWork.CalculateSalaryRegular(rateRegular, hoursToWork);

            //Arrange
            Assert.AreEqual(should, result);
        }

        [TestMethod]
        public void CalculateSalaryOverTime50Test()
        {
            //Arange
            RateOvertime rateOvertime = new RateOvertime(DateTime.Now, 20.9f);
            employee.RateOvertime =rateOvertime;
            SalaryWork salaryWork = new SalaryWork(employee.IdEmployee, date);

            salaryWork.NumberOfMinutes100 = 1590;//26:30
            salaryWork.NumberOfMinutes50 = 1955;//32:35
            
            double should = 1021.49d;

            //Act
            var result = salaryWork.CalculateSalaryOvertime50(rateOvertime);

            //Arrange
            Assert.AreEqual(should, result);
        }

        [TestMethod]
        public void CalculateSalaryOverTime100Test()
        {
            //Arange
            RateOvertime rateOvertime = new RateOvertime(DateTime.Now, 20.9f);
            employee.RateOvertime = rateOvertime;
            SalaryWork salaryWork = new SalaryWork(employee.IdEmployee, date);

            salaryWork.NumberOfMinutes100 = 1590;//26:30
            salaryWork.NumberOfMinutes50 = 1955;//32:35

            double should = 1107.70d;

            //Act
            var result = salaryWork.CalculateSalaryOvertime100(rateOvertime);

            //Arrange
            Assert.AreEqual(should, result);
        }

        [TestMethod]
        public void CalculateSalaryDayOffHourlyTest()
        {
            //Arange
            RateRegular rateRegular = new RateRegular(DateTime.Now, 20.3f, RateType.hourly);
            SalaryDayOff salary = new SalaryDayOff(employee.IdEmployee, date);

            salary.NumberOfMinutesDayOffPaid = 960;//16

            double should = 324.80d;

            //Act
            var result = salary.CalculateSalaryDayOff(rateRegular, hoursToWork);

            //Arrange
            Assert.AreEqual(should, result);
        }
        [TestMethod]
        public void CalculateSalaryDayOffMonthlyTest()
        {
            //Arange
            RateRegular rateRegular = new RateRegular(DateTime.Now, 3333f, RateType.monthly);
            SalaryDayOff salary = new SalaryDayOff(employee.IdEmployee, date);

            salary.NumberOfMinutesDayOffPaid = 960;//16

            double should = 317.43d;

            //Act
            var result = salary.CalculateSalaryDayOff(rateRegular, hoursToWork);

            //Arrange
            Assert.AreEqual(should, result);
        }

        [TestMethod]
        public void CalculateSalaryIllness80HourlyTest()
        {
            //Arange
            RateRegular rateRegular = new RateRegular(DateTime.Now, 20.3f, RateType.hourly);
            SalaryIllness salary = new SalaryIllness(employee.IdEmployee, date);

            salary.NumberOfMinutesIllness80 = 960;//16

            double should = 259.84d;

            //Act
            var result = salary.CalculateSalaryIllness80(rateRegular, hoursToWork);

            //Arrange
            Assert.AreEqual(should, result);
        }
        [TestMethod]
        public void CalculateSalaryIllnes80MonthlyTest()
        {
            //Arange
            RateRegular rateRegular = new RateRegular(DateTime.Now, 3333f, RateType.monthly);
            SalaryIllness salary = new SalaryIllness(employee.IdEmployee, date);

            salary.NumberOfMinutesIllness80 = 960;//16

            double should = 253.94d;

            //Act
            var result = salary.CalculateSalaryIllness80(rateRegular, hoursToWork);

            //Arrange
            Assert.AreEqual(should, result);
        }
        [TestMethod]
        public void CalculateSalaryIllness100HourlyTest()
        {
            //Arange
            RateRegular rateRegular = new RateRegular(DateTime.Now, 20.3f, RateType.hourly);
            SalaryIllness salary = new SalaryIllness(employee.IdEmployee, date);

            salary.NumberOfMinutesIllness100 = 960;//16

            double should = 324.8d;

            //Act
            var result = salary.CalculateSalaryIllness100(rateRegular, hoursToWork);

            //Arrange
            Assert.AreEqual(should, result);
        }
        [TestMethod]
        public void CalculateSalaryIllnes100MonthlyTest()
        {
            //Arange
            RateRegular rateRegular = new RateRegular(DateTime.Now, 3333f, RateType.monthly);
            SalaryIllness salary = new SalaryIllness(employee.IdEmployee, date);

            salary.NumberOfMinutesIllness100 = 960;//16

            double should = 317.43d;

            //Act
            var result = salary.CalculateSalaryIllness100(rateRegular, hoursToWork);

            //Arrange
            Assert.AreEqual(should, result);
        }
    }
}
