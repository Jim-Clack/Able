using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AbleStrategiesServices
{
    public class UserData
    {

        public UserData(string desc)
        {
            Desc = desc;
        }

        private string desc = "";

        public string Desc { get => desc; set => desc = value; }
    }
}
