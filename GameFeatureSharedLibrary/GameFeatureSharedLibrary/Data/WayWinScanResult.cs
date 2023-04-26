using System;
using System.Collections.Generic;
using System.Text;

namespace GameFeatureSharedLibrary.Data
{
    internal class WayWinRecord
    {
        public int TypeId { get; private set; }
        public int TotalWay { get; private set; }
        public WayWinRecord(int typeId, int totalWay)
        {
            TypeId = typeId;
            TotalWay = totalWay;
        }
    }
    internal class WayWinScanResult
    {
        public List<WayWinRecord> WayWinRecords { get;private set; }
        public List<List<bool>> IsRemoved;
        public WayWinScanResult() 
        {
            WayWinRecords = new List<WayWinRecord>();
            IsRemoved = new List<List<bool>>();
    }
        public void Add(int typeId, int totalWay)
        {
            WayWinRecords.Add(new WayWinRecord(typeId, totalWay) );
        }
    }
}
