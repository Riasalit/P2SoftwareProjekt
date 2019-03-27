using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship
{
    class Program
    {
        static void Main(string[] args)
        {
            AI ole = new AI("ole");
            ole.SetShips(new TestUI());
        }
    }
}
