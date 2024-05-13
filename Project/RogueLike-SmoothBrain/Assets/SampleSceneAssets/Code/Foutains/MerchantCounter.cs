using UnityEngine;

public class MerchantCounter : MonoBehaviour
{
    [SerializeField] private int bloodPrice = 10;
    [SerializeField] private int valueTrade = 1;

    public int BloodPrice => bloodPrice;
    public int ValueTrade => valueTrade;
    public Color Color => new Color(0.62f, 0.34f, 0.76f, 1.0f);
}
