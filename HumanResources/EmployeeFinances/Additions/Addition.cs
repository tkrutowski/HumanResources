using Konfiguracja;
using Logi;
using Pracownicy;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HumanResources.EmployeesFinances.Additions
{
    public class Addition : EmployeeFinanse
    {
        AdditionType additionType;

        public static ArrayList arrayListAdditionType = new ArrayList();

        public AdditionType AdditionType { get => additionType; set => additionType = value; }

        public static void AddAdditionType(string name, ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = "insert into rodzaj_dodatku values('" + name + "')";
            Database.Save(select, disconnect);
            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.dodawanie, DateTime.Now, Polaczenia.ip, NazwaTabeli.rodzaj_dodatku, select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        public static void GetAdditionType(ConnectionToDB disconnect = ConnectionToDB.disconnect)
        {
            string select = "select * from rodzaj_dodatku";
            
            SqlDataReader dataReader = Database.GetData(select);

            arrayListAdditionType.Clear();

            while (dataReader.Read())
            {
                AdditionType d = new AdditionType();
                d.Id = dataReader.GetInt32(0);
                d.Name = dataReader.GetString(1);

                arrayListAdditionType.Add(d);
            }
            dataReader.Close();
            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
        }
    }
}
