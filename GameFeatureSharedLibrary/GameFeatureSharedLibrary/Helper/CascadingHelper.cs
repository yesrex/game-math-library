using GameFeatureSharedLibrary.Slot;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameFeatureSharedLibrary.Helper
{
    internal class CascadingHelper
    {
        public static void SlotViewCascading(SlotView slotView, List<Reel> reels, List<List<bool>> isRemoved)
        {
            for(int reel = 0; reel < reels.Count; reel++)
            {
                ReelCascading(slotView.Reels[reel], reels[reel], isRemoved[reel]);
            }
        }

        private static void ReelCascading(List<Symbol> reelView, Reel reel, List<bool> isRemoved)
        {
            var emptyPoses = new Queue<int>();
            for (int pos = reelView.Count - 1; pos >= 0; pos--)
            {
                if (isRemoved[pos])
                {
                    emptyPoses.Enqueue(pos);
                    continue;
                }
                else
                {
                    if (emptyPoses.Count > 0)
                    {
                        reelView[emptyPoses.Dequeue()] = reelView[pos];
                        emptyPoses.Enqueue(pos);
                    }
                }
            }
            while(emptyPoses.Count > 0)
            {
                reelView[emptyPoses.Dequeue()] = GetRolledSymbol(reel);
            }

        }

        private static Symbol GetRolledSymbol(Reel reel)
        {
            reel.RollDown();
            return reel.GetSymbol();
        }
    }
}
