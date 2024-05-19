using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.SlotUtility
{

    internal class MathUtil
    {
        internal static double GetProbaByValue(double[] thresholds, double[] probas, double value) 
        {
            double proba = 0;
            int index = 0;
            while (index + 1 < thresholds.Length && thresholds[index+1] <= value)
                index++;
            proba = probas[index];
            return proba;
        }
    }
}
