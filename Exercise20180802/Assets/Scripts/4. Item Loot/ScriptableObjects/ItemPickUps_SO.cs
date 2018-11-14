using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemTypeDefinitions {
    HEALTH,
    MANA,
    WEALTH,
    WEAPON,
    ARMOR,
    BUFF,
    EMPTY
};

public enum ItemArmorSubType {
    None,
    Head,
    Chest,
    Hands,
    Legs,
    Boots
};

//Attribute Statement:
//it has to be right above the class line.
//it creates an asset menu entry based off the scriptable object.
[CreateAssetMenu(fileName = "NewItem", menuName = "Spawnable Item/New Pick-up", order = 1)]
public class ItemPickUps_SO : ScriptableObject
{
    #region Item Definitions Default Values

    public string itemName = "New Item";
    public ItemTypeDefinitions itemType = ItemTypeDefinitions.HEALTH;
    public ItemArmorSubType itemArmorSubType = ItemArmorSubType.None;
    public int itemAmount = 0;
    public int spawnChanceWeight = 0;

    public Material itemMaterial = null;
    public Sprite itemIcon = null;
    public Rigidbody itemSpawnObject = null;
    public Weapon weaponSlotObject = null;

    public bool isEquipped = false;
    public bool isInteractable = false;     //eg. non-interactable quest item(任务道具) 
    public bool isStorable = false;
    public bool isUnique = false;
    public bool isIndestructable = false;   //if it can't be destroyed
    public bool isQuestItem = false;
    public bool isStackable = false;
    public bool destroyOnUse = false;
    public float itemWeight = 0f;
    #endregion
}
