using Konfiguracja;
using Logi;
using Pracownicy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HumanResources.Employees
{
    public class Employee
    {
        private int id;
        private string name;
        private string lastName;
        private string street;
        private string city;
        private string zipCode;
        private float numberDaysOffLeft;
        private int numberDaysOffAnnually;
        private string telNumber;
        private string otherInfo;
        private bool isHired;
        private DateTime hiredDate;
        private DateTime releaseDate;
        private DateTime nextMmedicalExaminationDate;
        private DateTime nextBhpTrainingDate;

        //potrzebne do kierowców pojazdów/maszyn
        private string eMail;

        //potrzebne do drukowania formularzu czasu pracy (żeby nie trzeba było zwalniać żeby wydrukować)
        private bool partTimeJob;
        private bool isManagement;

        RateRegular rateRegular;
        RateOvertime rateOvertime;

        public static bool correctEmployee;
        
      
        public Employee()
        {

        }

        public string FirstName { get => name; set => name = value; }
        public int IdEmployee { get => id; set => id = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Street { get => street; set => street = value; }
        public string City { get => city; set => city = value; }
        public string ZipCode { get => zipCode; set => zipCode = value; }
        public float NumberDaysOffLeft { get => numberDaysOffLeft; set => numberDaysOffLeft = value; }
        public int NumberDaysOffAnnually { get => numberDaysOffAnnually; set => numberDaysOffAnnually = value; }
        public string TelNumber { get => telNumber; set => telNumber = value; }
        public string OtherInfo { get => otherInfo; set => otherInfo = value; }
        public bool IsHired { get => isHired; set => isHired = value; }
        public DateTime HiredDate { get => hiredDate; set => hiredDate = value; }
        public DateTime ReleaseDate { get => releaseDate; set => releaseDate = value; }
        public DateTime NextMmedicalExaminationDate { get => nextMmedicalExaminationDate; set => nextMmedicalExaminationDate = value; }
        public DateTime NextBhpTrainingDate { get => nextBhpTrainingDate; set => nextBhpTrainingDate = value; }
        public string EMail { get => eMail; set => eMail = value; }
        public bool PartTimeJob { get => partTimeJob; set => partTimeJob = value; }
        public bool IsManagement { get => isManagement; set => isManagement = value; }
        public RateOvertime RateOvertime { get => rateOvertime; set => rateOvertime = value; }
        public RateRegular RateRegular { get => rateRegular; set => rateRegular = value; }
        public string GetFullName { get { return lastName + " " + name; } }

        public void EmploymentEdition(int id, bool isHired, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = "update pracownik set zatrudniony = '"
                 + isHired + "'where id_pracownika = '" + id + "'";
            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.pracownik,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public void AddRateOvertime(ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            //wpisanie stawki 1 dnia miesiaca
            rateOvertime.DateFrom = new DateTime(rateOvertime.DateFrom.Year, rateOvertime.DateFrom.Month, 1);

            string select = "insert into stawka_nadgodziny values('" + IdEmployee + "','" +
                rateOvertime.DateFrom.ToString("d", DateFormat.TakeDateFormat()) + "','" + rateOvertime.RateValue.ToString().Replace(',', '.') + "')";

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.dodawanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.stawka_nadgodziny,
               select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public void EditRateOvertime(ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = "update stawka_nadgodziny set stawka_nadgodziny = '" + rateOvertime.RateValue.ToString().Replace(',', '.')
                + "'where id_stawki_nadgodziny = '" + rateOvertime.IdRate + "'";

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.stawka_nadgodziny,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        /// <summary>
        /// Usuwanie stawki nadgodzinowej pracownika z bazy
        /// </summary>
        /// <param name="id">id pracownika</param>
        public void DeleteRateOvertime(ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = "delete stawka_nadgodziny where id_pracownika=" + rateOvertime.IdEmployee;

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.usuwanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.stawka_nadgodziny,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }


        public void AddRateRegular(ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            //wpisanie stawki 1 dnia miesiaca
            rateRegular.DateFrom = new DateTime(rateRegular.DateFrom.Year, rateRegular.DateFrom.Month, 1);

            string select = "insert into stawka values('" + IdEmployee + "','" + (rateRegular.IsMonthlyOrHourly == RateType.hourly? false:true).ToString() + "','" +
                rateRegular.DateFrom.ToString("d", DateFormat.TakeDateFormat()) + "','" + rateRegular.RateValue.ToString().Replace(',', '.') + "')";

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.dodawanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.stawka,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public void EditRateRegular( ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = "update stawka set godz_mies='" + (rateRegular.IsMonthlyOrHourly == RateType.hourly ? false : true).ToString() + "',stawka = '" +
                  rateRegular.RateValue.ToString().Replace(',', '.') + "'where id_stawki = '" + rateRegular.IdRate + "'";

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.stawka,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public void DeleteRateRegular(ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = "delete stawka where id_pracownika=" + rateRegular.IdEmployee;

            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.dodawanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.stawka,
                select), disconnect == ConnectionToDB.disconnect ? true : false);
        }
    }
}
