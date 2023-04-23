using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace GameFeatureSharedLibrary.RandomNumber
{
    public class BaseRng
    {
        private Random Rnd;
        public List<int> WeightList { get; private set; }
        public int TotalWeight { get; private set; }
        public BaseRng() 
        {
            Rnd = new Random();
            WeightList = new List<int>();
            TotalWeight = 0;
        }
        
        public void Add(int weight)
        {
            if (weight < 0)
            {
                throw new ArgumentException("weight needs to be non-negative!!");
            }
            WeightList.Add(weight);
            TotalWeight += weight;
        }

        protected int FindIndex(int weight)
        {
            if (weight == 0) { throw new ArgumentException("looked-up weight needs to be positive!!"); }
            int index = 0;
            int cumulatedWeight = 0;
            foreach(var w in WeightList)
            {
                cumulatedWeight += w;
                if (weight <= cumulatedWeight) { return index; }
                index++;
            }
            return -1;
        }
        public virtual int Spin() 
        {
            return FindIndex(Rnd.Next(TotalWeight) + 1);
        }
    }
}
