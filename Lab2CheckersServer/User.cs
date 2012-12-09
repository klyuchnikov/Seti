using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2CheckersServer
{
    [Serializable]
    class User
    {
        public string Name { get; set; }

        public string IP { get; set; }

        public int ID { get; set; }

        public string Opponent { get; set; }
    }
}
