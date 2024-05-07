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
        public Sound fountaineSFX;
        public Sound altarSFX;

        public FountainType Type => type;
        public int BloodPrice => bloodPrice;
        public int ValueTrade => type == FountainType.Corruption ? valueTrade : -valueTrade;
        public int AbsoluteValueTrade => valueTrade;
        public Color Color => type == FountainType.Corruption ? new Color(0.62f, 0.34f, 0.76f, 1.0f) : new Color(0.0f, 0.94f, 1.0f, 1.0f);
    }
}
