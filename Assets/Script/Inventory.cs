using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;
    public readonly Dictionary<CardItemData, float> itemCheck = new Dictionary<CardItemData, float>();
    public InventoryItem item = new InventoryItem();
    void Awake()
    {
        if(Instance ==null) Instance = this;
        else Destroy(gameObject); 
    }

    void AddItem(CardItemData card)
    {
        float currentStack = itemCheck.ContainsKey(card) ? itemCheck[card] : 0;
        if (currentStack >= card.maxStack)
        {
            Debug.Log("최대 중복수 도달");
            return;
        }
        if(card.cardName == "SharpFangs")   itemCheck[card] = currentStack + card.scalePerStack;
        else itemCheck[card] = currentStack + 1;
        AddItemValue(card);
    }

    private void AddItemValue(CardItemData card)
    {
        float amount = itemCheck.ContainsKey(card) ? card.amount : card.scalePerStack;
        if (!item.invetory.ContainsKey(card))
        {
            item.invetory.Add(card, true);
        }
        switch (card.cardName)
        {
            case "DisposableShield":
                item.shield += amount;
                break;
            case "SharpFangs":
                item.sharpFangs += amount;
                break;
            case "DoT":
                item.dot += amount;
                break;
            case "RapidStrike":
                item.rapidStrike += amount;
                break;
            case "LuckySeven":
                item.luckySeven += amount;
                break;
            case "SpecialMove":
                item.specialMove = true;
                break;
            case "Magnet":
                item.magent = true;
                break;
            case "AdrenalineRush":
                item.adrenaline = true;
                break;
            case "SlowMotion":
                item.slowMotion = true;
                break;
            case "DealwithTheDevil":
                item.dealWithDevil = true;
                break;
        }
    }
}
