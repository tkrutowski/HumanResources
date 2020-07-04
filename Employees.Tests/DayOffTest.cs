using System;
using HumanResources.WorkTimeRecords;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Employees.Tests
{
    [TestClass]
    public class DayOffTest

    {
        DayOff work = new DayOff();
        [TestMethod]
        public void WorkTimeAllTest()
        {

            //Arange
            work.IdTypeDayOff = 1;
            

            //Act
            var result = work.WorkTimeAll();

            //Arrange
            Assert.AreEqual(new TimeSpan(4, 0, 0), result);
        }

        [TestMethod]
        public void WorkTimeAllFreeTest()
        {

            //Arange
            work.IdTypeDayOff = 3;


            //Act
            var result = work.WorkTimeAll();

            //Arrange
            Assert.AreEqual(new TimeSpan(0, 0, 0), result);
        }
        [TestMethod]
        public void WorkTimeAllEduTest()
        {

            //Arange
            work.IdTypeDayOff = 5;


            //Act
            var result = work.WorkTimeAll();

            //Arrange
            Assert.AreEqual(new TimeSpan(0, 0, 0), result);
        }
        [TestMethod]
        public void WorkTimeAllOtherTest()
        {

            //Arange
            work.IdTypeDayOff = 2;


            //Act
            var result = work.WorkTimeAll();

            //Arrange
            Assert.AreEqual(new TimeSpan(8, 0, 0), result);
        }
    }
}
