using Konfiguracja;
using Logi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.WorkTimeRecords
{
    /// <summary>
    /// Klasa do 
    /// </summary>
    class Illness:WorkTime, IWorkTime
    {
        int idIllnessType;
        float percentTypeIllness;

        //lista rodzaj choroby
        public static ArrayList listIllnessType = new ArrayList();

        public int IdIllnessType { get => idIllnessType; set => idIllnessType = value; }
        public float PercentTypeIllness { get => percentTypeIllness; set => percentTypeIllness = value; }

        public TimeSpan WorkTimeAll()
        {
            return new TimeSpan(8, 0, 0);
        }

        public TimeSpan WorkTime50()
        {
            return new TimeSpan(0, 0, 0);
        }

        public TimeSpan WorkTime100()
        {
            return new TimeSpan(0, 0, 0);
        }

        public static bool IsAlreadyInDataBase(int idEmployee, DateTime data)
        {
            string select = "select * from choroba where id_pracownika=" + idEmployee +
                 " AND datepart(year,data)=" + data.Year + " AND datepart(month,data)=" + data.Month + " AND datepart(day,data)=" + data.Day;

            return Database.GetOneElementBool(select, ConnectionToDB.disconnect);
        }

       

        public bool IsAlreadyInDataBase()
        {
            string select = "select * from choroba where id_pracownika=" + IdEmployee +
                 " AND datepart(year,data)=" + Date.Year + " AND datepart(month,data)=" + Date.Month + " AND datepart(day,data)=" + Date.Day;

            return Database.GetOneElementBool(select, ConnectionToDB.disconnect);
        }

        public int GetDay()
        {
            return Date.Day;
        }
    }
}
