using System;
using UnityEngine;

namespace BossCortege.EventHolder
{
    public class BuyCarInfo
    {
        public uint Cost { get; private set; }

        public BuyCarInfo(uint cost)
        {
            Cost = cost;
        }
    }

    public class BuyPlaceInfo
    {
        public uint Cost { get; private set; }

        public BuyPlaceInfo(uint cost)
        {
            Cost = cost;
        }
    }

    public class RaceStartInfo
    {
        // no info
    }

    public class RaceStopInfo
    {
        // no info
    }

    public class MoneyChangeInfo
    {
        public uint Value { get; private set; }

        public MoneyChangeInfo(uint value)
        {
            Value = value;
        }
    }

    public class DistanceChangeInfo
    {
        public uint Value { get; private set; }

        public DistanceChangeInfo(uint value)
        {
            Value = value;
        }
    }

    public class BestDistanceChangeInfo
    {
        public uint Value { get; private set; }

        public BestDistanceChangeInfo(uint value)
        {
            Value = value;
        }
    }
}