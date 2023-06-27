using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Numerics;


public class GoldManager : MonoBehaviour
{
    static private BigInteger gold = 0;

    private static GoldManager instance = null;

    private void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static GoldManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }




    string FormatValue(BigInteger value)
    {
        if (value < 1000) return $"{value}";

        BigInteger thousand = BigInteger.Pow(10, 3);
        string[] units = { "", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M" }; // 단위 배열

        int unitIndex = 0;
        BigInteger reducedValue = value;

        while (reducedValue >= thousand)
        {
            reducedValue /= thousand;
            unitIndex++;
        }

        BigInteger divisor = BigInteger.Pow(thousand, unitIndex);
        BigInteger quotient = value / divisor;
        BigInteger remainder = value % divisor;

        double decimalPart = (double)remainder / (double)divisor;
        double formattedValue = (double)quotient + decimalPart;

        Debug.Log($"{formattedValue.ToString("F2")}{units[unitIndex]}");
        return $"{formattedValue.ToString("F2")}{units[unitIndex]}";
    }





    public string SetGold(BigInteger value)
    {
        gold += value;

        return FormatValue(gold);
    }

    public string GetGold()
    {
        return FormatValue(gold);
    }
}
