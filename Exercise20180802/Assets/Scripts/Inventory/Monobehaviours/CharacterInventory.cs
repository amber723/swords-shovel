using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInventory : Singleton<CharacterInventory>
{
    #region Variable Declaration

    public CharacterStats charStats;

    public Image[] hotBarDisplayHolders = new Image[4];
    public GameObject InventoryDisplayHolder;   //panel
    public Image[] inventoryDisplaySlots = new Image[30];

    //0-9 for equipments, 10-29 for items.
    int itemStartIndex = 10;
    int inventoryItemCap = 20;

    public Dictionary<int, InventoryEntry> itemsInInventory
        = new Dictionary<int, InventoryEntry>();

    public InventoryEntry itemEntry;    //temporary place to store an item that comes in

    #endregion

    #region Initializations
    void Start()
    {
        itemEntry = new InventoryEntry(null, 0, null);
        itemsInInventory.Clear();

        inventoryDisplaySlots = InventoryDisplayHolder.GetComponentsInChildren<Image>();

    }
    #endregion

    void Update()
    {
        #region Watch for Hotbar Keypresses - Called by Character Controller Later
        
        //Check for a hotbar key to be pressed
        if (Input.GetKeyDown(KeyCode.Alpha1))
            TriggerItemUse(100);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            TriggerItemUse(101);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            TriggerItemUse(102);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            TriggerItemUse(103);

        if (Input.GetKeyDown(KeyCode.I))
            ToggleDisplayInventory();

        #endregion
    }

    public void StoreItem(ItemPickUp ItemToStore)
    {
        //check bag storage capacity
        if ((charStats.characterDefinition.currentEncumbrance
            + ItemToStore.itemDefinition.itemWeight)
            <= charStats.characterDefinition.maxEncumbrance)
        {
            itemEntry.invEntry = ItemToStore;
            itemEntry.stackSize = 1;
            itemEntry.hbSprite = ItemToStore.itemDefinition.itemIcon;

            ItemToStore.gameObject.SetActive(false);

            TryPickUp();
        }
    }

    bool TryPickUp()
    {
        //Check if the item to be stored is not null
        if (!itemEntry.invEntry)
        {
            return false;
        }

        //check if any items exist in the inventory already - if not, add this item
        if (itemsInInventory.Count == 0)
        {
            return AddItemToInv();
        }

        bool isInInv = false;

        //items exist in inventory
        //check if the item is stackable
        if (itemEntry.invEntry.itemDefinition.isStackable)
        {
            foreach (KeyValuePair<int, InventoryEntry> ie in itemsInInventory)
            {
                //Does thsi tiem already exist in inventory
                if (itemEntry.invEntry.itemDefinition ==
                    ie.Value.invEntry.itemDefinition)
                {
                    //Add 1 to stack and destroy the new instance
                    ie.Value.stackSize += 1;
                    OnFinishAddingToInv(ie.Value);
                    isInInv = true;
                    break;
                }
            }
        }

        // if inventory full
        if (itemsInInventory.Count == inventoryItemCap)
        {
            itemEntry.invEntry.gameObject.SetActive(true);
            Debug.Log("Inventory is Full.");
        }
        else if (!isInInv)
        {
            return AddItemToInv();
        }

        return isInInv;
    }

    bool AddItemToInv()
    {
        int idCount = GetLowestUnusedID();
        itemsInInventory.Add(idCount, new InventoryEntry(Instantiate(itemEntry.invEntry),
            itemEntry.stackSize, itemEntry.hbSprite));

        charStats.characterDefinition.currentEncumbrance
            += itemEntry.invEntry.itemDefinition.itemWeight;

        FillInventoryDisplay();

        OnFinishAddingToInv(itemsInInventory[idCount]);

        return true;
    }

    int GetLowestUnusedID()
    {
        int newID = 1;

        while (newID <= itemsInInventory.Count)
        {
            if (!itemsInInventory.ContainsKey(newID))
                return newID;

            newID += 1;
        }

        return newID;
    }

    void AddItemToHotBar(InventoryEntry item)
    {
        bool resetCount = false;
        int hotBarCounter = GetOpenHotBarSlot();

        //the hotbar is full?
        if (hotBarCounter == -1)
            return;

        //If the item is not currently in a hotbar yet
        if (item.hotBarSlot == -1)
        {
            //Add item to open hotbar slot
            item.hotBarSlot = hotBarCounter;

            //Change hotbar sprite to show item
            hotBarDisplayHolders[hotBarCounter].sprite = item.hbSprite;
            resetCount = true;
        }
        //else if it's in the hotbar and stackable
        else if (item.invEntry.itemDefinition.isStackable)
        {
            resetCount = true;
        }
        //else it's in the hotbar but not stackable
        else
        {
            //do nothing
        }

        if (resetCount)
        {
            hotBarDisplayHolders[item.hotBarSlot].GetComponentInChildren<Text>().text =
                item.stackSize.ToString();
        }
    }

    // 0 means hotbar is full
    int GetOpenHotBarSlot()
    {

        int hotBarCounter = 0;

        //Check for open hotbar slot
        foreach (Image image in hotBarDisplayHolders)
        {
            if (image.sprite == null)
            {
                return hotBarCounter;
            }

            hotBarCounter += 1;
        }

        return -1;
    }

    void OnFinishAddingToInv(InventoryEntry item)
    {
        AddItemToHotBar(item);
        Destroy(itemEntry.invEntry.gameObject);

        #region Reset itemEntry
        itemEntry.invEntry = null;
        itemEntry.stackSize = 0;
        itemEntry.hbSprite = null;
        #endregion
    }

    void ToggleDisplayInventory()
    {
        InventoryDisplayHolder.SetActive(!InventoryDisplayHolder.activeSelf);
    }

    void FillInventoryDisplay()
    {
        int slotCounter = itemStartIndex;

        foreach (KeyValuePair<int, InventoryEntry> ie in itemsInInventory)
        {
            inventoryDisplaySlots[slotCounter].sprite = ie.Value.hbSprite;
            ie.Value.inventorySlot = slotCounter;
            slotCounter += 1;
        }

        while (slotCounter < itemStartIndex + inventoryItemCap)
        {
            inventoryDisplaySlots[slotCounter].sprite = null;
            slotCounter++;
        }
    }

    public void TriggerItemUse(int itemToUseID)
    {
        KeyValuePair<int, InventoryEntry> itemToUse = new KeyValuePair<int, InventoryEntry>();

        foreach (KeyValuePair<int, InventoryEntry> ie in itemsInInventory)
        {
            if (itemToUseID >= 100)
            {
                itemToUseID -= 100;

                if (ie.Value.hotBarSlot == itemToUseID)
                {
                    itemToUse = ie;
                    break;
                }
            }
            else if (ie.Value.inventorySlot == itemToUseID)
            {
                itemToUse = ie;
                break;
            }
        }

        if (itemToUse.Value != null)
        {
            itemToUse.Value.invEntry.UseItem();

            if (itemToUse.Value.stackSize == 1)
            {
                if (itemToUse.Value.invEntry.itemDefinition.isStackable)
                {
                    if (itemToUse.Value.hotBarSlot != -1)
                    {
                        hotBarDisplayHolders[itemToUse.Value.hotBarSlot].sprite = null;
                        hotBarDisplayHolders[itemToUse.Value.hotBarSlot]
                            .GetComponentInChildren<Text>().text = "0";
                    }

                    itemsInInventory.Remove(itemToUse.Key);
                }
                else if (!itemToUse.Value.invEntry.itemDefinition.isIndestructable)
                {
                    itemsInInventory.Remove(itemToUse.Key);
                }
            }
            else
            {
                itemToUse.Value.stackSize -= 1;

                if (itemToUse.Value.hotBarSlot != -1)
                {
                    hotBarDisplayHolders[itemToUse.Value.hotBarSlot]
                    .GetComponentInChildren<Text>().text = itemToUse.Value.stackSize.ToString();
                }
            }
        }

        FillInventoryDisplay();
    }
}