using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public CharacterStats_SO characterDefinition_Template;
    public CharacterStats_SO characterDefinition;
    public CharacterInventory charInv;
    public GameObject characterWeaponSlot;

    #region Constructors
    public CharacterStats()
    {
        charInv = CharacterInventory.Instance;
    }
    #endregion

    #region Initializations
    private void Awake()
    {
        //make sure it's initialized before any other class tries to manipulate its values.
        if (characterDefinition_Template != null)
            characterDefinition = Instantiate(characterDefinition_Template);
    }

    void Start()
    {
        if (!characterDefinition.setManually)
        {
            characterDefinition.maxHealth = 500;
            characterDefinition.currentHealth = 500;

            characterDefinition.maxMana = 25;
            characterDefinition.currentMana = 10;

            characterDefinition.maxWealth = 500;
            characterDefinition.currentWealth = 0;

            characterDefinition.baseDamage = 2;
            characterDefinition.currentDamage = characterDefinition.baseDamage;

            characterDefinition.baseResistance = 0;
            characterDefinition.currentResistance = 0;

            characterDefinition.maxEncumbrance = 50f;
            characterDefinition.currentEncumbrance = 0f;

            characterDefinition.charExperience = 0;
            characterDefinition.charLevel = 1;
        }
    }
    #endregion

    #region SaveData
    private void Update()
    {
        //This should be triggered by the game manager during a save point
        if (Input.GetMouseButtonDown(2))
            characterDefinition.saveCharacterData();
    }
    #endregion

    #region Stat Increasers
    public void ApplyHealth(int healthAmount)
    {
        characterDefinition.ApplyHealth(healthAmount);
    }

    public void ApplyMana(int manaAmount)
    {
        characterDefinition.ApplyMana(manaAmount);
    }

    public void GiveWealth(int wealthAmount)
    {
        characterDefinition.GiveWealth(wealthAmount);
    }
    #endregion

    #region Stat Reducers
    public void TakeDamage(int amount)
    {
        characterDefinition.TakeDamage(amount);
    }

    public void TakeMana(int amount)
    {
        characterDefinition.TakeMana(amount);
    }
    #endregion

    #region Weapon and Armor Change
    public void ChangeWeapon(ItemPickUp weaponPickUp)
    {
        if (!characterDefinition.UnEquipWeapon(weaponPickUp, charInv, characterWeaponSlot))
        {
            characterDefinition.EquipWeapon(weaponPickUp, charInv, characterWeaponSlot);
        }
    }

    public void ChangeArmor(ItemPickUp armorPickUp)
    {
        if (!characterDefinition.UnEquipArmor(armorPickUp, charInv))
        {
            characterDefinition.EquipArmor(armorPickUp, charInv);
        }
    }
    #endregion

    #region Reporters
    public int GetHealth()
    {
        return characterDefinition.currentHealth;
    }

    public Weapon GetCurrentWeapon()
    {
        return characterDefinition.weapon.itemDefinition.weaponSlotObject;
    }

    public int GetDamage()
    {
        return characterDefinition.currentDamage;
    }

    public float GetResistance()
    {
        return characterDefinition.currentResistance;
    }

    #endregion

    #region Stat Initializers

    public void SetInitialHealth(int health)
    {
        characterDefinition.maxHealth = health;
        characterDefinition.currentHealth = health;
    }

    public void SetInitialResistance(int resistance)
    {
        characterDefinition.baseResistance = resistance;
        characterDefinition.currentResistance = resistance;
    }

    public void SetInitialDamage(int damage)
    {
        characterDefinition.baseDamage = damage;
        characterDefinition.currentDamage = damage;
    }

    #endregion
}