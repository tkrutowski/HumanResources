using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HumanResources.Settings
{
    [Serializable]
    public class Column
    {
        public string Name { get; set; }//jak się zaczyna od __ to nie może być widoczna w dostępne kolumny
        public int Index { get; set; }
        public int Wight { get; set; }
        public bool Visibility { get; set; }
        //public TabelaGrid Tabela { get; set; }
        public string Description { get; set; }
    }
}
