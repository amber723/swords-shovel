using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "NewStats", menuName = "Character/Stats", order = 1)]
public class CharacterStats_SO : ScriptableObject
{
    public Events.EventIntegerEvent OnLevelUp;
    public Events.EventIntegerEvent OnHeroDamaged;
    public Events.EventIntegerEvent OnHeroGainedHealth;

    public UnityEvent OnHeroDeath;
    public UnityEvent OnHeroInitialized;

    [System.Serializable]
    public class CharLevel
    {
        public int maxHealth;
        public int maxMana;
        public int maxWealth;
        public int baseDamage;
        public float baseResistance;
        public float maxEncumbrance;
        public int requiredXP;
    }

    #region Fields
    //public bool setManually = false;
    //public bool saveDataOnClose = false;

    public bool isHero = false;

    public ItemPickUp weapon { get; private set; }
    public ItemPickUp headArmor { get; private set; }
    public ItemPickUp chestArmor { get; private set; }
    public ItemPickUp handArmor { get; private set; }
    public ItemPickUp legArmor { get; private set; }
    public ItemPickUp footArmor { get; private set; }
    public ItemPickUp misc1 { get; private set; }
    public ItemPickUp misc2 { get; private set; }

    public int maxHealth = 0;
    public int currentHealth = 0;

    public int maxWealth = 0;
    public int currentWealth = 0;

    public int maxMana = 0;
    public int currentMana = 0;

    public int baseDamage = 0;
    public int currentDamage = 0;

    public float baseResistance = 0;
    public float currentResistance = 0f;

    public float maxEncumbrance = 0f;
    public float currentEncumbrance = 0f;

    public int charExperience = 0;
    public int charLevel = 0;

    public CharLevel[] charLevels;
    #endregion

    #region Stat Increasers
    public void ApplyHealth(int healthAmount)
    {
        if ((currentHealth + healthAmount) > maxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += healthAmount;
        }

        if (isHero)
        {
            OnHeroGainedHealth.Invoke(healthAmount);
        }
    }

    public void ApplyMana(int manaAmount)
    {
        if ((currentMana + manaAmount) > maxMana)
        {
            currentMana = maxMana;
        }
        else
        {
            currentMana += manaAmount;
        }
    }

    public void GiveWealth(int wealthAmount)
    {
        if ((currentWealth + wealthAmount) > maxWealth)
        {
            currentWealth = maxWealth;
        }
        else
        {
            currentWealth += wealthAmount;
        }
    }

    public void GiveXP(int xp)
    {
        charExperience += xp;
        if (charLevel < charLevels.Length)
        {
            int levelTarget = charLevels[charLevel].requiredXP;

            if (charExperience >= levelTarget)
                SetCharacterLevel(charLevel + 1);
        }
    }

    public void EquipWeapon(ItemPickUp weaponPickUp, CharacterInventory charInventory,
        GameObject weaponSlot)
    {
        Rigidbody newWeapon;

        weapon = weaponPickUp;
        charInventory.inventoryDisplaySlots[2].sprite = weaponPickUp.itemDefinition.itemIcon;

        newWeapon = Instantiate(weaponPickUp.itemDefinition.weaponSlotObject.weaponPreb,
            weaponSlot.transform);

        currentDamage = baseDamage + weapon.itemDefinition.itemAmount;
    }

    public void EquipArmor(ItemPickUp armorPickUp, CharacterInventory charInventory)
    {
        switch (armorPickUp.itemDefinition.itemArmorSubType)
        {
            case ItemArmorSubType.Head:
                charInventory.inventoryDisplaySlots[3].sprite
                    = armorPickUp.itemDefinition.itemIcon;

                headArmor = armorPickUp;
                currentResistance += armorPickUp.itemDefinition.itemAmount;
                break;
            case ItemArmorSubType.Chest:
                charInventory.inventoryDisplaySlots[4].sprite
                    = armorPickUp.itemDefinition.itemIcon;

                chestArmor = armorPickUp;
                currentResistance += armorPickUp.itemDefinition.itemAmount;
                break;
            case ItemArmorSubType.Hands:
                charInventory.inventoryDisplaySlots[5].sprite
                    = armorPickUp.itemDefinition.itemIcon;

                handArmor = armorPickUp;
                currentResistance += armorPickUp.itemDefinition.itemAmount;
                break;
            case ItemArmorSubType.Legs:
                charInventory.inventoryDisplaySlots[6].sprite
                    = armorPickUp.itemDefinition.itemIcon;

                legArmor = armorPickUp;
                currentResistance += armorPickUp.itemDefinition.itemAmount;
                break;
            case ItemArmorSubType.Boots:
                charInventory.inventoryDisplaySlots[7].sprite
                    = armorPickUp.itemDefinition.itemIcon;

                footArmor = armorPickUp;
                currentResistance += armorPickUp.itemDefinition.itemAmount;
                break;
        }
    }
    #endregion

    #region Stat Reducers
    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (isHero)
        {
            OnHeroDamaged.Invoke(amount);
        }

        if (currentHealth <= 0)
        {
            Death();
        }
    }

    public void TakeMana(int amount)
    {
        currentMana -= amount;

        if (currentMana < 0)
        {
            currentMana = 0;
        }
    }

    public bool UnEquipWeapon(ItemPickUp weaponPickUp, CharacterInventory charInventory,
        GameObject weaponSlot)
    {
        bool previousWeaponSame = false;

        if (weapon != null)
        {
            if (weapon == weaponPickUp)
            {
                previousWeaponSame = true;
            }
            charInventory.inventoryDisplaySlots[2].sprite = null;
            Destroy(weaponSlot.transform.GetChild(0).gameObject);
            weapon = null;
            currentDamage = baseDamage;
        }

        return previousWeaponSame;
    }

    public bool UnEquipArmor(ItemPickUp armorPickUp, CharacterInventory charInventory)
    {
        switch (armorPickUp.itemDefinition.itemArmorSubType)
        {
            case ItemArmorSubType.Head:
                return UnEquipHelper(headArmor, armorPickUp, 3, charInventory);
            case ItemArmorSubType.Chest:
                return UnEquipHelper(chestArmor, armorPickUp, 4, charInventory);
            case ItemArmorSubType.Hands:
                return UnEquipHelper(handArmor, armorPickUp, 5, charInventory);
            case ItemArmorSubType.Legs:
                return UnEquipHelper(legArmor, armorPickUp, 6, charInventory);
            case ItemArmorSubType.Boots:
                return UnEquipHelper(footArmor, armorPickUp, 7, charInventory);
            default:
                return false;
        }
    }

    bool UnEquipHelper(ItemPickUp prevItem, ItemPickUp itemPickUp,
        int index, CharacterInventory charInventory)
    {
        bool previousItemSame = false;

        if (prevItem != null)
        {
            if (prevItem == itemPickUp)
            {
                previousItemSame = true;
            }
            charInventory.inventoryDisplaySlots[index].sprite = null;
            currentResistance -= itemPickUp.itemDefinition.itemAmount;
            prevItem = null;
        }

        return previousItemSame;
    }

    #endregion

    #region Character Level Up and Death
    void Death()
    {
        //Call to Game Manager for Death State to trigger respawn
        //Dispaly the Death visualization

        if (isHero)
        {
            OnHeroDeath.Invoke();
        }
    }

    public void SetCharacterLevel(int newLv)
    {
        charLevel = newLv;
        //Display Level Up Visualization

        maxHealth = charLevels[newLv - 1].maxHealth;
        currentHealth = charLevels[newLv - 1].maxHealth;

        maxMana = charLevels[newLv - 1].maxMana;
        currentMana = charLevels[newLv - 1].maxMana;

        maxWealth = charLevels[newLv - 1].maxWealth;

        baseDamage = charLevels[newLv - 1].baseDamage;
        currentDamage = charLevels[newLv - 1].baseDamage;
        if (weapon != null)
            currentDamage += weapon.itemDefinition.itemAmount;

        baseResistance = charLevels[newLv - 1].baseResistance;
        currentResistance = charLevels[newLv - 1].baseResistance;

        maxEncumbrance = charLevels[newLv - 1].maxEncumbrance;

        if (charLevel > 1)
            OnLevelUp.Invoke(charLevel);
    }
    #endregion

    #region SaveCharacterData
    public void saveCharacterData()
    {
        EditorUtility.SetDirty(this);
    }
    #endregion
}
