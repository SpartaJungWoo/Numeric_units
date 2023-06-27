using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Numerics;

public class ButtonManager : MonoBehaviour
{
    public TMP_InputField text;
    public TMP_Text gold;

    public void SetGold()
    {
        BigInteger value = BigInteger.Parse(text.text.ToString());
        
        gold.text = GoldManager.Instance.SetGold(value);
    }
}
