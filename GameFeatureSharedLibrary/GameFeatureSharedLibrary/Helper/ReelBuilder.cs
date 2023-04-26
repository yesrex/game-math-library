using System;
using System.Collections.Generic;
using System.Text;
using GameFeatureSharedLibrary.Slot;

namespace GameFeatureSharedLibrary.Helper
{
    internal class ReelBuilder
    {

        public static Reel GetReel(List<int> ids) { return GetReel(ids, Enumerable.Repeat(1, ids.Count()).ToList()); }
        public static Reel GetReel(List<int> ids, List<int> weights)
        {
            if(ids.Count != weights.Count) { throw new ArgumentException("lenghts of symbol id and weight do not match."); };
            var reel = new Reel();
            for(int i=0; i < ids.Count; i++)
            {
                reel.Add(new Symbol(ids[i]), weights[i]);
            }
            return reel;
        }
    }
}
