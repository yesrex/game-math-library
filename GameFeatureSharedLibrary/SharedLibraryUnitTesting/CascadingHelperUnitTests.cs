using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameFeatureSharedLibrary.Helper;
using GameFeatureSharedLibrary.Slot;
using System.Threading.Tasks;

namespace SharedLibraryUnitTesting
{
    [TestClass]
    public class CascadingHelperUnitTests
    {
        [TestMethod]
        public void TestSlotViewCascading()
        {
            var reels = new List<Reel>();
            for(int i = 0; i < 6;i++)
            {
                reels.Add(ReelBuilder.GetReel(new List<int> { 1, 2, 3, 4, 5 }));
            }
            var reelSize = new List<int>() { 2, 2, 4, 4, 4, 4 };
            var slotView = new SlotView(reels, reelSize);
            var isRemoved = new List<List<bool>>();
            isRemoved.Add(new List<bool> { true, false });
            isRemoved.Add(new List<bool> { true, true });
            isRemoved.Add(new bool[4].ToList());
            isRemoved.Add(new List<bool> { true, true, true, true });
            isRemoved.Add(new List<bool> { true, false, true, false });
            isRemoved.Add(new List<bool> { false, true, false, true });
            slotView.PrintOnConsole();
            CascadingHelper.SlotViewCascading(slotView, reels, isRemoved);
            slotView.PrintOnConsole();
            var expected = new List<List<int>>
            {
                new List<int> { 5, 2},
                new List<int> { 4, 5},
                new List<int> { 1, 2, 3, 4},
                new List<int> { 2, 3, 4, 5},
                new List<int> { 4, 5, 2 ,4},
                new List<int> { 4, 5, 1, 3 }
            };
            for(int i = 0; i < expected.Count; i++)
            {
                for(int j = 0; j < expected[i].Count; j++)
                {
                    Assert.AreEqual(expected[i][j], slotView.Reels[i][j].Id);
                }
            }
        }
    }
}
