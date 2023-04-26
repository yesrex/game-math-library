using GameFeatureSharedLibrary.Slot;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibraryUnitTesting
{
    [TestClass]
    public class SlotUnitTest
    {
        [TestMethod]
        public void TestReel()
        {
            var reel = new Reel();
            reel.Add(new Symbol(0), 5);
            reel.Add(new Symbol(1), 0);
            reel.Add(new Symbol(0), 1);
            reel.Add(new Symbol(3), 5);
            reel.Add(new Symbol(1));
            Assert.AreEqual(3, reel.GetSymbol(-2).Id);
            Assert.AreEqual(3, reel.GetSymbol(3).Id);
            Assert.AreEqual(3, reel.GetSymbol(8).Id);
            Assert.AreEqual(0, reel.GetSymbol(0).Id);
            Assert.AreEqual(0, reel.GetSymbol(2).Id);
            Assert.AreEqual(1, reel.GetSymbol(-1).Id);
        }

        [TestMethod]
        public void TestSlotViewPrintOnConsole() 
        {
            var reel = new Reel();
            reel.Add(new Symbol(0), 5);
            reel.Add(new Symbol(1), 0);
            reel.Add(new Symbol(0), 1);
            reel.Add(new Symbol(3), 5);
            reel.Add(new Symbol(1));
            var reels = new List<Reel>();
            for (int i = 0;i < 6; i++) { reels.Add(reel); }
            var reelSize = new List<int>() { 2, 2, 6, 6, 4, 4};
            var slotView = new SlotView(reels, reelSize);
            slotView.PrintOnConsole();
        }
    }
}
