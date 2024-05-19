using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.SlotUtility
{
    internal class DebugUtil
    {
        internal static void PrintReels(int[,] symbols)
        {
            for(int j=0; j<symbols.GetLength(1); j++)
            {
                for (int i = 0; i < symbols.GetLength(0); i++)
                {
                    Console.Write(String.Format("{0,2:D}", symbols[i, j]));
                }
                Console.WriteLine();
            }
        }
    }
}
