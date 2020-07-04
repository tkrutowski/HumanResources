//Musi być to samo co w bazie danych
using Konfiguracja;
using Pracownicy;
using System.Collections;
using System.Data.SqlClient;

enum IllnessType
{

    illness80 = 1,
    illnest100 = 2,
    incidental = 3,
    motherly = 4,
    care = 5
}


class ChangeIllnessTypeToString
{
    private int id;
    private string name;

    private static ArrayList arrayListDayOffType = new ArrayList();

    public static ArrayList ArrayListIllnessType
    {
        get
        {
            if (arrayListDayOffType.Count == 0)
            {
                string select = "select * from rodzaj_choroby";

                SqlDataReader dataReader = HumanResources.Database.GetData(select);

                while (dataReader.Read())
                {
                    ChangeIllnessTypeToString value = new ChangeIllnessTypeToString();
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
    public int Id { get => id; }

    public static string ChangeToString(IllnessType value)
    {
        string toReturn = "";
        switch (value)
        {
            case IllnessType.illness80:
                toReturn = "chorobowy 80%";
                break;
            case IllnessType.illnest100:
                toReturn = "chorobowy 100%";
                break;
            case IllnessType.incidental:
                toReturn = "wypadkowy";
                break;
            case IllnessType.motherly:
                toReturn = "macierzyński";
                break;
            case IllnessType.care:
                toReturn = "opiekuńczy";
                break;
            default:
                toReturn = "";
                break;
        }
        return toReturn;
    }
}