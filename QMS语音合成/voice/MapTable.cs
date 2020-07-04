using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace voice
{
    class MapTable
    {
        private string _key;
        private string _value;

        public string key {
            get {return _key; }
            set { _key = value; }
        }
        public string value {
            get { return _value; }
            set { _value = value; }
        }

    }
}
