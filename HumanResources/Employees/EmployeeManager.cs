using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Konfiguracja;
using Logi;
using Pracownicy;

namespace HumanResources.Employees
{
    public class EmployeeManager
    {
        //Employee employee;
        public static ArrayList arrayEmployees = new ArrayList();
        private TableView viewOrTable;

        public Employee GetEmployee(int idEmployee, TableView viewOrTable, ConnectionToDB disconnect)
        {
            Employee employee = new Employee();
            this.viewOrTable = viewOrTable;
            string select = "select * from " + (viewOrTable == TableView.view ? "pracownik_view" : "pracownik") + " where id_pracownika=" + idEmployee;
            SqlDataReader dataReader = Database.GetData(select);

            while (dataReader.Read())
            {
                employee = AssignEmployee(dataReader);
            }
            dataReader.Close();

            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();

            return employee;
        }

        public void Add(Employee employee, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = "insert into pracownik values('" +
                employee.FirstName + "','" + employee.LastName + "','" + employee.Street + "','" + employee.City + "','" + employee.ZipCode + "','" +
                employee.NumberDaysOffLeft + "','" + employee.NumberDaysOffAnnually + "','" + employee.TelNumber + "','" + employee.OtherInfo + "','" + employee.IsHired + "','" +
                employee.HiredDate.ToString("d", DateFormat.TakeDateFormat()) + "','" + employee.ReleaseDate.ToString("d", DateFormat.TakeDateFormat()) + "','" + employee.NextMmedicalExaminationDate.ToString("d", DateFormat.TakeDateFormat()) + "','"
                + employee.NextBhpTrainingDate.ToString("d", DateFormat.TakeDateFormat()) + "','" + employee.EMail + "','" + employee.PartTimeJob + "','" + employee.IsManagement + "')";

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.dodawanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.pracownik, select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public int AddReturnId(Employee employee, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            int id = -1; 
            string select = "insert into pracownik output inserted.id_pracownika values('" +
                employee.FirstName + "','" + employee.LastName + "','" + employee.Street + "','" + employee.City + "','" + employee.ZipCode + "','" +
                employee.NumberDaysOffLeft + "','" + employee.NumberDaysOffAnnually + "','" + employee.TelNumber + "','" + employee.OtherInfo + "','" + employee.IsHired + "','" +
                employee.HiredDate.ToString("d", DateFormat.TakeDateFormat()) + "','" + employee.ReleaseDate.ToString("d", DateFormat.TakeDateFormat()) + "','" + employee.NextMmedicalExaminationDate.ToString("d", DateFormat.TakeDateFormat()) + "','"
                + employee.NextBhpTrainingDate.ToString("d", DateFormat.TakeDateFormat()) + "','" + employee.EMail + "','" + employee.PartTimeJob + "','" + employee.IsManagement + "')";

            id = Database.SaveReturnId(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.dodawanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.pracownik, select), disconnect == ConnectionToDB.disconnect ? true : false);
            return id;
        }

        public void Edit(Employee employee, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = "update pracownik set imie='" + employee.FirstName + "',nazwisko = '" +
                  employee.LastName + "', ulica = '" + employee.Street + "', miasto = '" + employee.City + "', kod = '" + employee.ZipCode
                 + "', urlop_pozostaly = '" + employee.NumberDaysOffLeft.ToString().Replace(',', '.') + "', wymiar_urlopu = '" + employee.NumberDaysOffAnnually + "', numer = '" + employee.TelNumber
                 + "',inne = '" + employee.OtherInfo + "',data_zatrudnienia = '" + employee.HiredDate.ToString("d", DateFormat.TakeDateFormat()) + "',data_zwolnienia = '" + employee.ReleaseDate.ToString("d", DateFormat.TakeDateFormat())
                 + "',data_nast_badania_lek = '" + employee.NextMmedicalExaminationDate.ToString("d", DateFormat.TakeDateFormat()) + "',data_nast_szkolenia_BHP = '" + employee.NextBhpTrainingDate.ToString("d", DateFormat.TakeDateFormat())
                 + "',email = '" + employee.EMail + "',pol_etatu = '" + employee.PartTimeJob + "',czy_kadra = '" + employee.IsManagement + "' where id_pracownika = '" + employee.IdEmployee + "'";

            Database.Save(select, disconnect);

            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.pracownik,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }
        public void Delete(int id, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = "delete pracownik where id_pracownika=" + id;

            Database.Save(select, disconnect);

            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.usuwanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.pracownik,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public void GetEmployeesToList(TableView viewOrTable)
        {
            this.viewOrTable = viewOrTable;

            DownloadEmployeesToList("select * from " + (viewOrTable == TableView.view ? "pracownik_view" : "pracownik") + " order by nazwisko asc");
        }

        /// <summary>
        /// Pobiera dane o aktualnych\poprzednich pracownikach z bazy danych.
        /// </summary>
        /// <param name="isHired">zatrudniony - true, zwolniony - false</param>
        /// <param name="viewOrTable">view = true, table = false</param>
        public void GetEmployeesHiredRealesedToList(bool isHired, TableView viewOrTable)
        {
            this.viewOrTable = viewOrTable;
            DownloadEmployeesToList("select * from " + (viewOrTable == TableView.view ? "pracownik_view" : "pracownik") + " where zatrudniony='" + isHired + "' order by nazwisko asc");
        }
        public void GetEmployeesIsHiredIsManagementToList(bool isHired, bool isManagement, TableView viewOrTable)
        {
            this.viewOrTable = viewOrTable;
            DownloadEmployeesToList("select * from " + (viewOrTable == TableView.view ? "pracownik_view" : "pracownik") + " where zatrudniony='" + isHired + "' and czy_kadra = '" + isManagement + "' order by nazwisko asc");
        }
        
        public void GetEmployeesIsHiredIsManagementIsFullTimeToList(bool isHired, bool isManagement, bool isFullTime, TableView viewOrTable)
        {
            this.viewOrTable = viewOrTable;
            DownloadEmployeesToList("select * from " + (viewOrTable == TableView.view ? "pracownik_view" : "pracownik") + " where zatrudniony='" + isHired + "' and pol_etatu = '" + isFullTime + "' and czy_kadra = '" + isManagement + "' order by nazwisko asc");
        }


        private void DownloadEmployeesToList(string select, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            //odczyt dataReaderem zwróconego wiersza 
            SqlDataReader dataReader = Database.GetData(select);

            arrayEmployees.Clear();

            while (dataReader.Read())
            {
                arrayEmployees.Add(AssignEmployee(dataReader));
            }
            dataReader.Close();
            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
        }

        private Employee AssignEmployee(SqlDataReader dataReader)
        {
            Employee e = new Employee();
            e.IdEmployee = dataReader.GetInt32(0);
            e.FirstName = dataReader.GetString(1);
            e.LastName = dataReader.GetString(2);
            e.Street = dataReader.GetString(3);
            e.City = dataReader.GetString(4);
            e.ZipCode = dataReader.GetString(5);
            e.NumberDaysOffLeft = dataReader.GetFloat(6);
            e.NumberDaysOffAnnually = dataReader.GetInt32(7);
            e.TelNumber = dataReader.GetString(8);
            e.OtherInfo = dataReader.GetString(9);
            e.IsHired = dataReader.GetBoolean(10);
            if (!dataReader.IsDBNull(11))
                e.HiredDate = dataReader.GetDateTime(11);
            if (!dataReader.IsDBNull(12))
                e.ReleaseDate = dataReader.GetDateTime(12);
            if (!dataReader.IsDBNull(13))
                e.NextMmedicalExaminationDate = dataReader.GetDateTime(13);
            if (!dataReader.IsDBNull(14))
                e.NextBhpTrainingDate = dataReader.GetDateTime(14);
            if (!dataReader.IsDBNull(15))
                e.EMail = dataReader.GetString(15);
            else
                e.EMail = "";
            if (!dataReader.IsDBNull(16))
                e.PartTimeJob = dataReader.GetBoolean(16);
            if (!dataReader.IsDBNull(17))
                e.IsManagement = dataReader.GetBoolean(17);

            if (viewOrTable == TableView.view)//view
            {
                e.RateRegular = new RateRegular(dataReader.GetInt32(18), dataReader.GetDateTime(20),
                    dataReader.GetFloat(21), (RateType)Enum.Parse(typeof(RateType), Convert.ToInt16(dataReader.GetBoolean(19)).ToString()));

                e.RateOvertime = new RateOvertime(dataReader.GetInt32(22), dataReader.GetDateTime(23), dataReader.GetFloat(24));
            }
            return e;
        }
    }
}
