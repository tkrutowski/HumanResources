using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using  HumanResources.WorkTimeRecords;

namespace Employees.Tests
{
    [TestClass]
    public class WorkTimeTest
    {

        Work work = new Work();

       

        [TestMethod]
        public void WorkTimeAllTest()
        {

            //Arange
            work.StartTime = new DateTime(2019, 12, 1, 7, 0, 0);
            work.StopTime = new DateTime(2019, 12, 1, 16, 8, 0);


            //Act
            var result = work.WorkTimeAll();

            //Arrange
            Assert.AreEqual(new TimeSpan(9, 8, 0), result);
        }

        [TestMethod]
        public void WorkTime50Test()
        {

            //Arange
            work.StartTime = new DateTime(2019, 12, 1, 7, 0, 0);
            work.StopTime = new DateTime(2019, 12, 1, 16, 8, 0);


            //Act
            var result = work.WorkTime50();

            //Arrange
            Assert.AreEqual(new TimeSpan(1, 8, 0), result);
        }

        [TestMethod]
        public void WorkTime100WeekendTest()
        {

            //Arange
            work.StartTime = new DateTime(2019, 12, 1, 7, 0, 0);
            work.StopTime = new DateTime(2019, 12, 1, 15, 15, 0);
            work.Date = new DateTime(2019, 12, 28);//sobota
           // work.Date = new DateTime(2019, 12, 27);//dzień roboczy (piątek)

            //Act
            var result = work.WorkTime100();

            //Arrange
            Assert.AreEqual(new TimeSpan(8, 15, 0), result);//sobota
            //Assert.AreEqual(new TimeSpan(8, 15, 0), result);//piątek
        }

        [TestMethod]
        public void WorkTime100WorkDayTest()
        {

            //Arange
            work.StartTime = new DateTime(2019, 12, 1, 7, 0, 0);
            work.StopTime = new DateTime(2019, 12, 1, 15, 15, 0);
            //work.Date = new DateTime(2019, 12, 28);//sobota
            work.Date = new DateTime(2019, 12, 27);//dzień roboczy (piątek)

            //Act
            var result = work.WorkTime100();

            //Arrange
           // Assert.AreEqual(new TimeSpan(8, 15, 0), result);//sobota
            Assert.AreEqual(new TimeSpan(0, 0, 0), result);//piątek
        }


        [TestMethod]
        public void WorkTimeRegularWorkTest()
        {

            //Arange
            work.StartTime = new DateTime(2019, 12, 5, 7, 0, 0);
            work.StopTime = new DateTime(2019, 12, 5, 14, 8, 0);
            work.Date = new DateTime(2019, 12, 28);//sobota
            //work.Date = new DateTime(2019, 12, 27);//dzień roboczy (piątek)

            //Act
            var result = work.WorkTimeRegularWork();

            //Arrange
            Assert.AreEqual(new TimeSpan(7, 8, 0), result);
        }
    }
}
