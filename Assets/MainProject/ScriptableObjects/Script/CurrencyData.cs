 using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Currency
{
    public string CurrencyType;

    public Sprite CurrencySprite;
}
[CreateAssetMenu(fileName = "CurrencyData")]
public class CurrencyData : ScriptableObject
{
   

    public Currency[]  currencies;
    public Dictionary<string, Currency> LookupDictionary = new Dictionary<string, Currency>();
    void OnEnable()
    {
        
  
        LookupDictionary = new Dictionary<string, Currency>();
        foreach (var currency in currencies)
        {
            string key = currency.CurrencyType.ToLower();
            if (!LookupDictionary.ContainsKey(key))
            {
                LookupDictionary.Add(key, currency);
            }
        }
    }
}