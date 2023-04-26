using System;
using System.Collections.Generic;
using System.Text;

namespace GameFeatureSharedLibrary.Slot
{
    internal class SlotView
    {
        internal List<int> ReelSize { get; private set; }
        internal List<List<Symbol>> Reels;
        public SlotView(List<Reel> reels, List<int> reelSize)
        {
            ReelSize = new List<int>();
            Reels = new List<List<Symbol>>();
            foreach (var size in reelSize)
            {
                ReelSize.Add(size);
                Reels.Add(new List<Symbol>());
            }
            
            for(int i = 0; i < ReelSize.Count; i++)
            {
                Reels[i] = reels[i].GetSymbols(ReelSize[i]);
            }
        }

        public void PrintOnConsole() 
        {
            bool ifRowExist;
            int rowIndex = 0;
            do
            {
                ifRowExist = false;
                foreach (var item in Reels)
                {
                    if (rowIndex < item.Count)
                    {
                        Console.Write("{0,2:N0} ", item[rowIndex].Id);
                        ifRowExist = true;
                    }
                    else
                    {
                        Console.Write("{0,2:N0} ", "");
                    }
                }
                rowIndex++;
                Console.Write("\n");
            } while(ifRowExist);     
        }
    }
}
