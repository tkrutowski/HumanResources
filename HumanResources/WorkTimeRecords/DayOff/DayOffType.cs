//Musi być to samo co w bazie danych
using Konfiguracja;
using System.Collections;
using System.Data.SqlClient;

enum DayOffType
{
    halfDay = 1,
    rest = 2,
    free = 3,
    motherly = 4,
    educational = 5,
    care = 6,
    fatherly = 7,
    occasional = 8 
    

}

 class ChangeDayOffTypeToString
{
    private int  id;
    private string name;

    private static  ArrayList arrayListDayOffType = new ArrayList();

    public static ArrayList ArrayListDayOffType
    {
        get
        {
            if (arrayListDayOffType.Count == 0)
            {
                string select = "select * from rodzaj_urlopu";
                
                SqlDataReader dataReader = HumanResources.Database.GetData(select);
                                
                while (dataReader.Read())
                {
                    ChangeDayOffTypeToString value = new ChangeDayOffTypeToString();
                    //id potrzebne do grida
                    value.id = dataReader.GetInt32(0);
                    value.name = dataReader.GetString(1);
                    //ur.procentRodzajUrlop = dataReaderUrlop.GetInt32(2);

                    arrayListDayOffType.Add(value);
                }
                dataReader.Close();
                Polaczenia.OdlaczenieOdBazy();

                return arrayListDayOffType;
            }
            return arrayListDayOffType;
        }
    }

    public string Name { get => name; }
    public int Id { get => id;}

    public static string ChangeToString(DayOffType value)
    {
        string toReturn = "";
        switch (value)
        {
            case DayOffType.halfDay:
                toReturn = "1/2 dnia";
                break;
            case DayOffType.rest:
                toReturn = "wypoczynkowy";
                break;
            case DayOffType.free:
                toReturn = "bezpłatny";
                break;
            case DayOffType.motherly:
                toReturn = "macierzyński";
                break;
            case DayOffType.educational:
                toReturn = "wychowawczy";
                break;
            case DayOffType.care:
                toReturn = "opieka";
                break;
            case DayOffType.fatherly:
                toReturn = "ojcowski";
                break;
            case DayOffType.occasional:
                toReturn = "okolicznościowy";
                break;
            default:
                toReturn = "";
                break;
        }
        return toReturn;
    }
}