using System;
using UnityEngine;

namespace BossCortege.EventHolder
{
    public class InputSwipeInfo
    {
        public Vector2 Direction { get; private set; }

        public InputSwipeInfo(Vector2 direction)
        {
            Direction = direction;
        }
    }

    public class DragCarInfo
    {
        public bool IsDragging { get; private set; }

        public DragCarInfo(bool isDragging)
        {
            IsDragging = isDragging;
        }
    }

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

    public class SpawnStopInfo
    {
        // no info
    }

    public class RaceFinishInfo
    {
        // no info
    }

    public class RaceWinInfo
    {
        // no info
    }

    public class RaceLoseInfo
    {
        // no info
    }

    public class GoHomeInfo
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

    public class MergeCarInfo
    {
        public GuardCar FirstCar { get; private set; }
        public PlaceComponent FirstPlace { get; private set; }
        public GuardCar SecondCar { get; private set; }
        public PlaceComponent SecondPlace { get; private set; }

        public MergeCarInfo(GuardCar firstCar, PlaceComponent firstPlace, GuardCar secondCar, PlaceComponent secondPlace)
        {
            FirstCar = firstCar;
            FirstPlace = firstPlace;
            SecondCar = secondCar;
            SecondPlace = secondPlace;
        }
    }

    public class SwapCarInfo
    {
        public PlaceComponent FirstPlace { get; private set; }
        public PlaceComponent SecondPlace { get; private set; }

        public SwapCarInfo(PlaceComponent firstPlace, PlaceComponent secondPlace)
        {
            FirstPlace = firstPlace;
            SecondPlace = secondPlace;
        }
    }

    public class ReplaceCarInfo
    {
        // no info
    }

    public class RamInfo
    {
        //TODO: ram cooldown
    }

    public class BossDieInfo
    {
        // no info
    }

    public class EnemyDieInfo
    {
        public uint Money { get; private set; }

        public EnemyDieInfo(uint money)
        {
            Money = money;
        }
    }

    public class MenuOpenInfo
    {
        // no info
    }
}