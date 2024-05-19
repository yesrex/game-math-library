using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulator.SlotUtility
{
    internal class RandomWeightArray
    {
        internal int[] accumulatedWeights { get; private set; }
        internal int[] items { get; private set; }
        internal RandomWeightArray(int[, ] weights)
        {
            if (weights.GetLength(1) != 2 || weights.GetLength(0) == 0)
                throw new ArgumentException("weights has a incorrect dimension");
            accumulatedWeights = new int[weights.GetLength(0)];
            items = new int[weights.GetLength(0)];

            int weight = 0;
            for(int i=0; i<weights.GetLength(0); i++)
            {
                if (weights[i, 1] <= 0)
                    throw new ArgumentException("each item must have a positive weight!");
                weight += weights[i, 1];
                accumulatedWeights[i] = weight;
                items[i] = weights[i, 0];
            }
        }

        internal int rollSingleItem(Random random, bool printDetails=false)
        {
            int rolledWeight = random.Next(accumulatedWeights.Last()) + 1;
            int index = 0;
            while (accumulatedWeights[index] < rolledWeight)
                index++;

            if (printDetails)
                Console.WriteLine($"item: {items[index]} with index: {index} ({rolledWeight}/{accumulatedWeights.Last()})");
            return items[index];
        }
    }
}
