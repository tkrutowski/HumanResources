using Konfiguracja;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HumanResources
{
    class Database
    {
        internal static void Save(string select, ConnectionToDB disconnect = ConnectionToDB.notDisconnect)
        {
            SqlCommand sqlInsert = new SqlCommand();
            sqlInsert.CommandText = select;
            sqlInsert.Connection = Polaczenia.PolaczenieDoBazy();
            //ExecuteNonQuery służy do wstawiania wierszy do tabeli
            sqlInsert.ExecuteNonQuery();
            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
        }
        internal static SqlDataReader GetData(string select)
        {
            SqlCommand sqlSelect = new SqlCommand();
            sqlSelect.Connection = Polaczenia.PolaczenieDoBazy();
            sqlSelect.CommandText = select;
            SqlDataReader dataReader = sqlSelect.ExecuteReader();
            return dataReader;
        }

        /// <summary>
        /// Pobiera  jedną komórkę z tabeli 
        /// /// </summary>
        /// <param name="select"></param>
        /// <returns>zwraca jako string</returns>
        internal static string GetOneElement(string select, ConnectionToDB disconnect = ConnectionToDB.notDisconnect)
        {
            string stringToReturn = string.Empty;
            SqlCommand sqlSelect = new SqlCommand();
            sqlSelect.Connection = Polaczenia.PolaczenieDoBazy();
            sqlSelect.CommandText = select;

            //odczyt dataReaderem zwróconego wiersza 
            SqlDataReader dataReader = sqlSelect.ExecuteReader();
            while (dataReader.Read())
            {
                if (!dataReader.IsDBNull(0))
                    stringToReturn = dataReader.GetValue(0).ToString();
            }
            dataReader.Close();
            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
            return stringToReturn;
        }
        /// <summary>
        /// Zwraca true jeżeli jest jakaś wartość w bazie danych
        /// </summary>
        /// <param name="select"></param>
        /// <returns></returns>
        internal static bool GetOneElementBool(string select, ConnectionToDB disconnect = ConnectionToDB.notDisconnect)
        {
            bool boolToReturn = false;
            SqlCommand sqlSelect = new SqlCommand();
            sqlSelect.Connection = Polaczenia.PolaczenieDoBazy();
            sqlSelect.CommandText = select;

            //odczyt dataReaderem zwróconego wiersza 
            SqlDataReader dataReader = sqlSelect.ExecuteReader();
            while (dataReader.Read())
            {
                if (!dataReader.IsDBNull(0))
                    if (!String.IsNullOrEmpty(dataReader.GetValue(0).ToString()))
                        boolToReturn = true;
            }
            dataReader.Close();
            if(disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
            return boolToReturn;
        }

        internal static int SaveReturnId(string select, ConnectionToDB disconnect = ConnectionToDB.notDisconnect)
        {
            int id = -1;
            SqlCommand sqlInsert = new SqlCommand();
            sqlInsert.CommandText = select;
            sqlInsert.Connection = Polaczenia.PolaczenieDoBazy();
            //ExecuteNonQuery służy do wstawiania wierszy do tabeli
            id = (int)sqlInsert.ExecuteScalar();
            if (disconnect == ConnectionToDB.disconnect)
                Polaczenia.OdlaczenieOdBazy();
            return id;
        }
    }
}
