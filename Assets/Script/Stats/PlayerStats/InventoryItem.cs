using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

public class InventoryItem
{
    public float shield;
    public float sharpFangs;
    public float dot;
    public float rapidStrike;
    public float luckySeven;
    public bool magent;
    public bool adrenaline;
    public bool slowMotion;
    public bool dealWithDevil;
    public bool specialMove;
    public Dictionary<BaseCardData, bool> invetory = new Dictionary<BaseCardData, bool>();

    public void Reset()
    {
        shield = 0f;
        sharpFangs = 0f;
        dot = 0f;
        rapidStrike = 0f;
        luckySeven = 0f;
        magent = false;
        adrenaline = false;
        slowMotion = false;
        dealWithDevil = false;
        specialMove = false;
        invetory.Clear();
    }
}
