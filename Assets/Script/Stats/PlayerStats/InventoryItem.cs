using System.Collections.Generic;
using UnityEngine;

public class InventoryItem
{
    public float shield;
    public float sharpFangs;
    public float dot;
    public float rapidStrike;
    public float luckySeven;
    public float specialMove = 1f;
    public bool magnet;
    public bool adrenaline;
    public bool slowMotion;
    public bool dealWithDevil;
    public Dictionary<BaseCardData, bool> invetory = new Dictionary<BaseCardData, bool>();

    public void Reset()
    {
        shield = 0f;
        sharpFangs = 0f;
        dot = 0f;
        rapidStrike = 0f;
        luckySeven = 0f;
        specialMove = 0f;
        magnet = false;
        adrenaline = false;
        slowMotion = false;
        dealWithDevil = false;
        invetory.Clear();
    }
}
