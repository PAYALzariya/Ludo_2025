using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Country
{
    public string countryname;
    public string countryinitials;
    public Sprite flagSprite;
}
[CreateAssetMenu(fileName = "CountryData")]
public class CountryData : ScriptableObject
{
    public Country[] countries;
    public Dictionary<string, Country> countryLookup = new Dictionary<string, Country>();
    void OnEnable()
    {
        
  
        countryLookup = new Dictionary<string, Country>();
        foreach (var country in countries)
        {
            string key = country.countryinitials.ToLower();
            if (!countryLookup.ContainsKey(key))
            {
                countryLookup.Add(key, country);
            }
        }
    }
}