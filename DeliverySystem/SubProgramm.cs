using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliverySystem
{
    class _TreeCode
    {

        public Dictionary<int, _Vars> vars = new Dictionary<int, _Vars>(); // гл. переменные
        public Dictionary<int, _Macro> macros = new Dictionary<int, _Macro>(); // гл. макросы
        public Dictionary<int, SubProgramm> programms = new Dictionary<int, SubProgramm>(); // гл. макросы


        internal  class _Vars
        {
            public bool IsPublic;
            public string type;         // table, record
            public bool IsRef;
            public bool IsConst;
            public bool IsRec;
            public _Vars rec_struct;
            public bool IsType;
            public bool IsTable;
            public string name;
            public string def_value;
            public string IndexBy;
        }

        internal class _Macro
        {
            public string IsPublic;
            public string name;
            public string value;
            public string method;
        }

        internal class SubProgramm
        {
            public string type;
            public bool IsPublic;
            public string name;
            public _Vars result;


            internal class _Params
            {
                public string type;
                public bool IsRef;
                public string name;
                public string def_value;
                public bool in_par;
                public bool out_par;
            }

            internal class _Code
            {

            }

            public Dictionary<int, _Params> parameters = new Dictionary<int, _Params>();

        }
    }
}
