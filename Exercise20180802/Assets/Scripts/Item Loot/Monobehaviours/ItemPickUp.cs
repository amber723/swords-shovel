using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ItemPickUps_SO itemDefinition;

    GameObject foundStats;              //The Player obj
    public CharacterStats charStats;
    CharacterInventory charInventory;   //singleton instance

    #region Constructors

    public ItemPickUp()
    {
        //preset values
        charInventory = CharacterInventory.Instance;
    }

    #endregion

    void Start()
    {
        if(charStats == null)
        {
            foundStats = GameObject.FindGameObjectWithTag("Player");
            charStats = foundStats.GetComponent<CharacterStats>();
        }
    }

    void StoreItemInInventory()
    {
        charInventory.StoreItem(this);
    }

    public void UseItem()
    {
        switch (itemDefinition.itemType)
        {
            case ItemTypeDefinitions.HEALTH:
                charStats.ApplyHealth(itemDefinition.itemAmount);
                break;
            case ItemTypeDefinitions.MANA:
                charStats.ApplyMana(itemDefinition.itemAmount);
                break;
            case ItemTypeDefinitions.WEALTH:
                charStats.GiveWealth(itemDefinition.itemAmount);
                break;
            case ItemTypeDefinitions.WEAPON:
                charStats.ChangeWeapon(this);
                break;
            case ItemTypeDefinitions.ARMOR:
                charStats.ChangeArmor(this);
                break;
            default:
                Debug.LogError("Can't Use this type of item: "+ itemDefinition.itemType);
                break;
        }
    }

    //Function Description:
    //An item is going to be a rigidBody obj and it's going to have applied to 
    //it a collider. Set that collider to a trigger and anytime the player runs
    //through it, they'll pick that item up.
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (itemDefinition.isStorable)
            {
                Debug.Log("ITEM PICKED UP");
                StoreItemInInventory();
            }
            else
            {
                UseItem();
            }
        }
    }
}
