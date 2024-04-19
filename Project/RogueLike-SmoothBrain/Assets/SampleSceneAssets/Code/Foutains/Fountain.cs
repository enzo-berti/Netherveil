using UnityEngine;

namespace Fountain
{
    public enum FountainType
    {
        Blessing,
        Corruption
    }

    public class Fountain : MonoBehaviour
    {
        [SerializeField] private FountainType type = FountainType.Blessing;
        [SerializeField] private int bloodPrice = 10;
        [SerializeField] private int valueTrade = 1;

        public FountainType Type => type;
        public int BloodPrice => bloodPrice;
        public int ValueTrade => type == FountainType.Corruption ? valueTrade : -valueTrade;
        public int AbsoluteValueTrade => valueTrade;
    }
}
