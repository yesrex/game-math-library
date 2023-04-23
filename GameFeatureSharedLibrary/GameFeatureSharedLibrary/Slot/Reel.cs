using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using GameFeatureSharedLibrary.RandomNumber;

namespace GameFeatureSharedLibrary.Slot
{
    internal class Reel
    {
        private BaseRng ReelRng;
        private List<Symbol> Symbols;
        private int ReelLength;
        public Reel() 
        {
            ReelRng = new BaseRng();
            Symbols = new List<Symbol>();
        }
        public void Add(Symbol symbol, int weight=1)
        {
            Symbols.Add(symbol);
            ReelRng.Add(weight);
            ReelLength++;
        }
        public Symbol GetSymbol(int index)
        {
            if (ReelLength == 0) { throw new Exception("cannot get symbol from an empty reel"); }
            var shiftIndex = index % ReelLength;
            if (shiftIndex < 0) 
            {
                shiftIndex += ReelLength;
            }
            return Symbols[shiftIndex];
        }
    }
}
