using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.WorkTimeRecords
{
    public interface IWorkTime
    {
       TimeSpan WorkTimeAll();
       TimeSpan WorkTime50();
       TimeSpan WorkTime100();
        bool IsAlreadyInDataBase();
        int GetDay();
    }
}
