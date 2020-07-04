using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Xml;
using Logi;
using Konfiguracja;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace HumanResources.Settings
{
    class SetLoan
    {
        //zmienne
        int idUstawienia;

        //
        //pożyczki
        //
        public static List<Column> listColumn = new List<Column>();
        int sortColumnIndex;//sortowanie wg kolumny - index
        /// <summary>
        /// false malejaco, true rosnaco
        /// </summary>
        SortType sortTypeAscDesc;
        DisplayOptions optionDisplay;//zatwierdzone, wszystkie...


        #region PROPERTYSY
                   
        public int SortColumnIndex
        {
            set { sortColumnIndex = value; }
            get { return sortColumnIndex; }
        }

        public SortType SortTypeAscDesc
        {
            set { sortTypeAscDesc = value; }
            get { return sortTypeAscDesc; }
        }

        public DisplayOptions OptionDisplay
        {
            set { optionDisplay = value; }
            get { return optionDisplay; }
        }

        #endregion

        /// <summary>
        /// Jeżeli nie ma w bazie danych tabeli to przekazywane są do listy ustawienia początkowe
        /// </summary>
        public static void InitialSettings()
        {

            listColumn.Clear();

            Column k0 = new Column();
            k0.Name = "__dgvP_idPozyczki";
            k0.Index = 0;
            k0.Wight = 100;
            k0.Visibility = false;
            k0.Description = "idPozyczki";
            listColumn.Add(k0);

            Column k1 = new Column();
            k1.Name = "__dgvP_idPacownika";
            k1.Index = 1;
            k1.Wight = 100;
            k1.Visibility = false;
            k1.Description = "idPracownika";
            listColumn.Add(k1);

            Column k2 = new Column();
            k2.Name = "_dgvP_pracownik";
            k2.Index = 2;
            k2.Wight = 130;
            k2.Visibility = true;
            k2.Description = "Pracownik";
            listColumn.Add(k2);

            Column k3 = new Column();
            k3.Name = "_dgvP_nazwa";
            k3.Index = 3;
            k3.Wight = 100;
            k3.Visibility = true;
            k3.Description = "Nazwa";
            listColumn.Add(k3);

            Column k4 = new Column();
            k4.Name = "_dgvP_kwota";
            k4.Index = 4;
            k4.Wight = 80;
            k4.Visibility = true;
            k4.Description = "Kwota";
            listColumn.Add(k4);

            Column k5 = new Column();
            k5.Name = "_dgvP_data";
            k5.Index = 5;
            k5.Wight = 80;
            k5.Visibility = true;
            k5.Description = "Data";
            listColumn.Add(k5);

            Column k6 = new Column();
            k6.Name = "_dgvP_rata";
            k6.Index = 6;
            k6.Wight = 80;
            k6.Visibility = true;
            k6.Description = "Rata";
            listColumn.Add(k6);

            Column k7 = new Column();
            k7.Name = "_dgvP_inne";
            k7.Index = 7;
            k7.Wight = 100;
            k7.Visibility = true;
            k7.Description = "Inne";
            listColumn.Add(k7);

            Column k8 = new Column();
            k8.Name = "_dgvP_doSplaty";
            k8.Index = 8;
            k8.Wight = 80;
            k8.Visibility = true;
            k8.Description = "Do spłaty";
            listColumn.Add(k8);

            Column k9 = new Column();
            k9.Name = "__dgvP_czySplacona";
            k9.Index = 9;
            k9.Wight = 70;
            k9.Visibility = false;
            k9.Description = "czy spłacona";
            listColumn.Add(k9);

            //return listaKolumnWewn;
        }


        /// <summary>
        /// Zapisuje ustawienia tabeli pożyczki do bazy danych
        /// </summary>
        /// <param name="lista"></param>
        /// <param name="z"></param>
        public void SaveSettings(SetLoan p, ConnectionToDB disconnect)
        {
            string select = "update ustawienia set sort_kolumna_poz='" + p.SortColumnIndex +
            "', sort_rodzaj_poz='" + (int)p.SortTypeAscDesc + "', opcje_wys_poz ='" + (int)p.OptionDisplay + "' where id_uzytkownika=" + Polaczenia.idUser;

            Database.Save(select, ConnectionToDB.disconnect);

            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.ustawienia, select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        /// <summary>
        /// Pobiera ustawienia tabeli pożyczki danego uzytkownika
        /// </summary>
        /// <returns></returns>
        public void GetSettings()
        {
            string select = "select id_ustawien, sort_kolumna_poz, sort_rodzaj_poz, opcje_wys_poz from ustawienia where id_uzytkownika=" + Polaczenia.idUser;


            SqlDataReader dataReader = Database.GetData(select);

            while (dataReader.Read())
            {
                if (!dataReader.IsDBNull(0))
                    idUstawienia = dataReader.GetInt32(0);
                if (!dataReader.IsDBNull(1))
                    sortColumnIndex = dataReader.GetInt32(1);
                if (!dataReader.IsDBNull(2))
                    //sortTypeAscDesc = (SortType)Enum.Parse(typeof(SortType), dataReader.GetBoolean(2).ToString());
                    sortTypeAscDesc = (SortType)Enum.Parse(typeof(SortType), Convert.ToInt32(dataReader.GetBoolean(2)).ToString());
                if (!dataReader.IsDBNull(3))
                    optionDisplay = (DisplayOptions)Enum.Parse(typeof(DisplayOptions), dataReader.GetInt32(3).ToString());
            }
            dataReader.Close();

            //odłączenie od bazy
            Polaczenia.OdlaczenieOdBazy();
        }
    }
}
