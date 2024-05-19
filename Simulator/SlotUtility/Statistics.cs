using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.SlotUtility
{
    internal class Distribution
    {
        internal double[] Axis;
        internal double[] SegmentAmounts;
        internal double[] SegmentValues;

        internal double[] WeightedValues
        {
            get
            {
                double totalAmount = SegmentAmounts.Sum();
                double[] values = new double[SegmentAmounts.Length];
                for (int i = 0; i < values.Length; i++)
                    values[i] = SegmentValues[i] / totalAmount;
                return values;
            }
        }
        internal double[] HitRates { get 
            {
                double totalAmount = SegmentAmounts.Sum();
                double[] values = new double[SegmentAmounts.Length];
                for(int i = 0; i<values.Length; i++)
                    values[i] = SegmentAmounts[i] / totalAmount;
                return values; 
            } }


        internal Distribution(double[] axis)
        {
            Axis = axis;
            SegmentAmounts = new double[axis.Length];
            SegmentValues = new double[axis.Length];
        }

        internal void Add(double value)
        {
            int index = 0;
            while (index+1<Axis.Length && Axis[index+1] <= value)
                index++;

            SegmentAmounts[index]++;
            SegmentValues[index] += value;
        }

        internal double[] GetCumulatedProba(bool reverse = true)
        {
            double[] values = HitRates;
            if (reverse)
            {
                for(int i=values.Length-2; i>=0; i--)
                {
                    values[i] += values[i + 1];
                }
            }
            else
            {
                for (int i = 1; i < values.Length; i++)
                {
                    values[i] += values[i - 1];
                }
            }
            return values;
        }

        internal void PrintValuesOnConsole()
        {
            double[] values = WeightedValues;
            for(int i=0; i<values.Length-1; i++)
                Console.WriteLine($"{Axis[i]}-{Axis[i + 1]}: {values[i]}");
            Console.WriteLine($"{Axis[values.Length - 1]}-: {values[values.Length - 1]}");
        }

        internal void PrintProbaOnConsole()
        {
            double[] values = HitRates;
            for (int i = 0; i < values.Length - 1; i++)
                Console.WriteLine($"{Axis[i]}-{Axis[i + 1]}: {values[i]}");
            Console.WriteLine($"{Axis[values.Length - 1]}-: {values[values.Length - 1]}");
        }

        internal void PrintCumulatedProbaOnConsole(bool reverse = true)
        {
            double[] values = GetCumulatedProba(reverse);
            if (reverse)
            {
                for (int i = 0; i < values.Length; i++)
                    Console.WriteLine($">={Axis[i]}: {values[i]}");
            }
            else
            {
                for (int i = 0; i < values.Length-1; i++)
                    Console.WriteLine($"{Axis[0]}-{Axis[i+1]}: {values[i]}");
                Console.WriteLine($"{Axis[0]}-: {values[values.Length - 1]}");
            }
            
        }
    }

    internal class Average
    {
        internal string Percentage => string.Format("{0}%", totalValue / amount * 100);
        internal double Value => totalValue / amount;
        double totalValue;
        long amount;

        internal Average()
        {
            totalValue = 0;
            amount = 0;
        }
        internal void Add(double value)
        {
            this.totalValue += value;
            amount++;
        }
        internal void Add(int value)
        {
            this.totalValue += (double)value;
            amount++;
        }
    }
    
    internal class Statistics
    {
    }
}
