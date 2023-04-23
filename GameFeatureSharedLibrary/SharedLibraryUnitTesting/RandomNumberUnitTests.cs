using GameFeatureSharedLibrary.RandomNumber;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using System.Linq;

namespace SharedLibraryUnitTesting
{

    [TestClass]
    public class RandomNumberUnitTests : BaseRng
    {
        #region FindIndex
        [TestMethod]
        public void TestFindIndexEmptyWeightList()
        {
            Assert.AreEqual(-1, FindIndex(1));
        }
        [TestMethod]
        public void TestFindIndexZeroWeight()
        {
            Add(0);
            Assert.AreEqual(-1, FindIndex(1));
        }

        [TestMethod]
        public void TestFindIndexCheckIndex()
        {
            Add(0);
            Add(1);
            Add(2);
            Assert.AreEqual(1, FindIndex(1));
            Assert.AreEqual(2, FindIndex(2));
            Assert.AreEqual(2, FindIndex(3));
        }

        [TestMethod]
        public void TestFindIndexCheckIndex2()
        {
            Add(5);
            Add(0);
            Add(2);
            Add(3);
            Add(1);
            Assert.AreEqual(0, FindIndex(1));
            Assert.AreEqual(0, FindIndex(5));
            Assert.AreEqual(2, FindIndex(6));
            Assert.AreEqual(2, FindIndex(7));
            Assert.AreEqual(3, FindIndex(8));
            Assert.AreEqual(3, FindIndex(10));
            Assert.AreEqual(4, FindIndex(11));
            Assert.AreEqual(-1, FindIndex(12));
        }

        [TestMethod]
        public void TestFindIndexOverSize()
        {
            Add(0);
            Add(1);
            Add(2);
            Assert.AreEqual(-1, FindIndex(4));
        }
        #endregion

        [TestMethod]
        public void TestSpin()
        {
            Add(1);
            Add(2);
            Add(0);
            Add(2);
            int tmp;
            long upperBound = WeightList.Count() - 1;
            for (int i = 0; i < 10000; i++)
            {
                tmp = Spin();
                if (tmp  > upperBound || tmp < 0 || tmp == 2)
                {
                    Assert.Fail();
                }
            } 
        }

        [TestMethod]
        public void TestSpinAccuracy() 
        {
            int scope = 100;
            int scale = 10000;
            var testWeights = new List<int>()
            {
                1,
                scope,
                scope * 4,
                scope * 1,
                scope * 1,
                scope * 2,
                scope * 1,
                scope * 1,
                scope * 2,
                1,
                scope * 1,
                scope * 1,
                scope * 2,
                scope * 1,
            };
            int total = testWeights.Sum() * scale;
            foreach(var w in testWeights)
            {
                Add(w);
            }
            var result = (new int[WeightList.Count]).ToList();
            for (int i = 0; i < total; i++)
            {
                result[Spin()]++;
            }
            for(int i = 0; i < WeightList.Count; i++)
            {
                var estimate = (double)WeightList[i] * (double)total / (double)TotalWeight;
                //Console.WriteLine(result[i]);          
                var bias = Math.Abs(estimate - result[i]);
                Assert.IsFalse(bias > scale/2, $"Bias of {bias} is too big");
            }
            //Console.WriteLine(result.Sum());
        }
    }
}