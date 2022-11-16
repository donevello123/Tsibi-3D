using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    public int MaxMana { get => _maxMana; set => _maxMana = value; }
    public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
    public int CurrentMana { get => _currentMana; set => _currentMana = value; }
    public int Level { get => _level; set { _level = value; UpdateStats(); } } // Affects Defense, Mana, Health, and Attack
    public float Distance { get => _distance; set => _distance = value; }      // Distance traveled
    public int Str { get => _str; set { _str = value; UpdateStats(); } }       // Affects Physical Attack, Physical Defense
    public int Agi { get => _agi; set { _agi = value; UpdateStats(); } }       // Affects ASPD, Dodge
    public int Dex { get => _dex; set { _dex = value; UpdateStats(); } }       // Affects Accuracy, Dodge, Magic Defense
    public int Chr { get => _chr; set { _chr = value; UpdateStats(); } }       // Affects Mana, Magic Attack
    public int Sta { get => _sta; set { _sta = value; UpdateStats(); } }       // Affects Health, Magic Defense
    public int PhysicalAttack { get => (int)_physATT; }
    public float AttackSpeed { get => _aspd; set { _aspd = value; UpdateStats(); } }

    public int statusPoints { get => _statusPoints; set => _statusPoints = value; }
    public float Exp { get => _exp; set => _exp = value; }

    private int _statusPoints;
    private int _str;
    private int _agi;
    private int _dex;
    private int _chr;
    private int _sta;
    private int _level;
    private int _maxHealth;
    private int _maxMana;
    private int _currentHealth;
    private int _currentMana;

    private float _distance;
    private float _exp;
    private float _physATT;
    private float _physDEF;
    private float _magDEF;
    private float _magATT;
    private float _accuracy;

    private float _aspd;
    private float _dodge;
    
    public Stats()
    {
        Level = 1;
        Str = 1;
        Agi = 1;
        Dex = 1;
        Chr = 1;
        Sta = 1;
        Exp = 0;
        Distance = 0.0f;
        statusPoints = 0;

       CurrentHealth = MaxHealth;
       CurrentMana = MaxMana;
    }
    public Stats(int level, int str, int agi, int dex, int chr, int sta, int exp)
    {
        Level = level;
        Str = str;
        Agi = agi;
        Dex = dex;
        Chr = chr;
        Sta = sta;
        Exp = exp;
        Distance = 0.0f;
        statusPoints = 0;

        CurrentHealth = MaxHealth;
        CurrentMana = MaxMana;
    }

    private void UpdateStats()
    {
        _maxHealth = 100 + (int)(Sta * 20.5f) + (50*Level);
        _maxMana = 40 + (Chr * 3) + (20*Level);
        _physATT = 1 + (0.5f * Dex) + (3 * Str) + (10*Level);
        _physDEF = 10 + (Str) * (Level * 10);
        _aspd = 0.65f + (0.01f * Agi);
    }
    public void LoadSavedStats(string characterName)
    {
        if (ES3.KeyExists(characterName + "_str"))
        {
            ES3.Load(characterName + "_str", 1);
            ES3.Load(characterName + "_agi", 1);
            ES3.Load(characterName + "_dex", 1);
            ES3.Load(characterName + "_chr", 1);
            ES3.Load(characterName + "_sta", 1);
            ES3.Load(characterName + "_level", 1);
            ES3.Load(characterName + "_exp", 0.0f);
            ES3.Load(characterName + "_statusPoints", 0);
            Debug.Log("Loaded character stats for " + characterName + "!");
            }else
            Debug.Log("No stats save file loaded for " + characterName + "!");


    }

    public void SaveStats(string characterName)
    {
        ES3.Save(characterName + "_str", Str);
        ES3.Save(characterName + "_agi", Agi);
        ES3.Save(characterName + "_dex", Dex);
        ES3.Save(characterName + "_chr", Chr);
        ES3.Save(characterName + "_sta", Sta);
        ES3.Save(characterName + "_level", Level);
        ES3.Save(characterName + "_exp", Exp);
        ES3.Save(characterName + "_statusPoints", statusPoints);

        Debug.Log("Saved character stats for " + characterName + "!");

    }
}
