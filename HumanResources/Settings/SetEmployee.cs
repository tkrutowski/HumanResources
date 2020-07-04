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
using HumanResources.Settings;

namespace Pracownicy.Settings
{
    class SetEmployee
    {
        //zmienne
        int idUstawienia;
        //int idUzytkownika;

        //
        //pracownicy
        //
        public static List<Column> listColumns = new List<Column>();
        int sortColumnIndex;//sortowanie wg kolumny - index
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

            listColumns.Clear();

            Column k0 = new Column();
            k0.Name = "__dgvPr_idPracownika";
            k0.Index = 0;
            k0.Wight = 100;
            k0.Visibility = false;
            k0.Description = "idPracownika";
            listColumns.Add(k0);

            Column k1 = new Column();
            k1.Name = "_dgvPr_imie";
            k1.Index = 1;
            k1.Wight = 80;
            k1.Visibility = true;
            k1.Description = "Imie";
            listColumns.Add(k1);

            Column k2 = new Column();
            k2.Name = "_dgvPr_nazwisko";
            k2.Index = 2;
            k2.Wight = 80;
            k2.Visibility = true;
            k2.Description = "Nazwisko";
            listColumns.Add(k2);

            Column k3 = new Column();
            k3.Name = "_dgvPr_ulica";
            k3.Index = 3;
            k3.Wight = 100;
            k3.Visibility = true;
            k3.Description = "Ulica";
            listColumns.Add(k3);

            Column k4 = new Column();
            k4.Name = "_dgvPr_miasto";
            k4.Index = 4;
            k4.Wight = 100;
            k4.Visibility = true;
            k4.Description = "Miasto";
            listColumns.Add(k4);

            Column k5 = new Column();
            k5.Name = "_dgvPr_kod";
            k5.Index = 5;
            k5.Wight = 50;
            k5.Visibility = true;
            k5.Description = "Kod";
            listColumns.Add(k5);

            Column k6 = new Column();
            k6.Name = "__dgvPr_idStawki";
            k6.Index = 6;
            k6.Wight = 311;
            k6.Visibility = false;
            k6.Description = "idStawki";
            listColumns.Add(k6);

            Column k7 = new Column();
            k7.Name = "_dgvPr_stawka";
            k7.Index = 7;
            k7.Wight = 100;
            k7.Visibility = true;
            k7.Description = "Stawka";
            listColumns.Add(k7);

            Column k8 = new Column();
            k8.Name = "__dgvPr_godzMies";
            k8.Index = 8;
            k8.Wight = 80;
            k8.Visibility = false;
            k8.Description = "godz czy miesiac";
            listColumns.Add(k8);

            Column k9 = new Column();
            k9.Name = "__dgvPr_idNadgodziny";
            k9.Index = 9;
            k9.Wight = 70;
            k9.Visibility = false;
            k9.Description = "idNadgodziny";
            listColumns.Add(k9);

            Column k10 = new Column();
            k10.Name = "_dgvPr_nadgodziny";
            k10.Index = 10;
            k10.Wight = 70;
            k10.Visibility = true;
            k10.Description = "Nadgodziny";
            listColumns.Add(k10);

            Column k11 = new Column();
            k11.Name = "_dgvPr_urlopPozostaly";
            k11.Index = 11;
            k11.Wight = 70;
            k11.Visibility = true;
            k11.Description = "Urlop pozostały";
            listColumns.Add(k11);

            Column k12 = new Column();
            k12.Name = "_dgvPr_wymiarUrlopu";
            k12.Index = 12;
            k12.Wight = 60;
            k12.Visibility = true;
            k12.Description = "Wymiar urlopu";
            listColumns.Add(k12);

            Column k13 = new Column();
            k13.Name = "_dgvPr_telefon";
            k13.Index = 13;
            k13.Wight = 80;
            k13.Visibility = true;
            k13.Description = "Telefon";
            listColumns.Add(k13);

            Column k14 = new Column();
            k14.Name = "_dgvPr_inne";
            k14.Index = 14;
            k14.Wight = 107;
            k14.Visibility = true;
            k14.Description = "Inne";
            listColumns.Add(k14);

            Column k15 = new Column();
            k15.Name = "__dgvPr_czyZatrudniony";
            k15.Index = 15;
            k15.Wight = 80;
            k15.Visibility = false;
            k15.Description = "czy zatrudniony";
            listColumns.Add(k15);

            Column k16 = new Column();
            k16.Name = "_dgvPr_badLek";
            k16.Index = 16;
            k16.Wight = 80;
            k16.Visibility = true;
            k16.Description = "Badanie lek.";
            listColumns.Add(k16);

            Column k17 = new Column();
            k17.Name = "_dgvPr_szkolenieBhp";
            k17.Index = 17;
            k17.Wight = 80;
            k17.Visibility = true;
            k17.Description = "Szkolenie BHP";
            listColumns.Add(k17);

            Column k18 = new Column();
            k18.Name = "_dgvPr_dataZatrudnienia";
            k18.Index = 18;
            k18.Wight = 80;
            k18.Visibility = true;
            k18.Description = "Zatrudniony";
            listColumns.Add(k18);

            Column k19 = new Column();
            k19.Name = "_dgvPr_dataZwolnienia";
            k19.Index = 19;
            k19.Wight = 80;
            k19.Visibility = true;
            k19.Description = "Zwolniony";
            listColumns.Add(k19);



            //return listaKolumnWewn;
        }

        /// <summary>
        /// Zapisuje ustawienia tabeli pracownik do bazy danych
        /// </summary>
        /// <param name="lista"></param>
        /// <param name="z"></param>
        public void SaveSettings(SetEmployee p, ConnectionToDB disconnect)
        {
            string select = "update ustawienia set sort_kolumna_prac='" + p.SortColumnIndex +
            "', sort_rodzaj_prac='" + (int)p.SortTypeAscDesc + "', opcje_wys_prac ='" + (int)p.OptionDisplay + "' where id_uzytkownika=" + Polaczenia.idUser;

            HumanResources.Database.Save(select, ConnectionToDB.disconnect);

            //log
            LogSys.DodanieLoguSystemu(new LogSys(Polaczenia.idUser, RodzajZdarzenia.edycja, DateTime.Now, Polaczenia.ip, NazwaTabeli.ustawienia, select), disconnect == ConnectionToDB.disconnect ? true : false);
        }

        /// <summary>
        /// Pobiera ustawienia tabeli pracowników danego uzytkownika
        /// </summary>
        /// <returns></returns>
        public void GetSettings()
        {
            string select = "select id_ustawien, sort_kolumna_prac, sort_rodzaj_prac, opcje_wys_prac from ustawienia where id_uzytkownika=" + Polaczenia.idUser;

            SqlDataReader dataReader = HumanResources.Database.GetData(select);

            while (dataReader.Read())
            {
                if (!dataReader.IsDBNull(0))
                    idUstawienia = dataReader.GetInt32(0);
                if (!dataReader.IsDBNull(1))
                    sortColumnIndex = dataReader.GetInt32(1);
                if (!dataReader.IsDBNull(2))
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
