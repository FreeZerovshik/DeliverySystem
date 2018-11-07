using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliverySystem
{
    class SubProgramm
    {
        public string type;
        public bool IsPublic;
        public string name;
        public string result;

        

        internal class _Params
        {
            public string type;
            public bool IsRef;
            public string name;
            public string def_value;
            public bool in_par;
            public bool out_par;
        }

        public Dictionary<int, _Params> parameters = new Dictionary<int, _Params>();

    }
}
