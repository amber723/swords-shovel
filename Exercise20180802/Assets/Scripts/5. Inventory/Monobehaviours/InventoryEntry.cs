using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//a bunch of  fields that are going to store info 
public class InventoryEntry
{
    public ItemPickUp invEntry;
    public int stackSize;
    public int inventorySlot;
    public int hotBarSlot;
    public Sprite hbSprite;

    public InventoryEntry(ItemPickUp invEntry, int stackSize, Sprite hbSprite)
    {
        this.invEntry = invEntry;
        this.stackSize = stackSize;
        inventorySlot = -1;
        hotBarSlot = -1;
        this.hbSprite = hbSprite;
    }
}
