using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public enum StatType
{
    Ad, Ap, AttackSpeed, CritChance, CritDamage, ArmorPen, FlatArmorPen, MagicPen, FlatMagicPen, LifeSteal,
    Ar, Mr, Tenacity, MoveSpeed, AbilityHaste, MeleeRange, RangedRange, GoldGeneration, Health, MaxHealth,
    HealthRegen, HealthRegenRate, Resource, MaxResource, ResourceRegen, ResourceRegenRate
}

/// <summary>
/// Class that, along with the StatModifier class, looks to provide stats with a base class and a modifier system
/// which always keeps track of all of the buffs/debuffs to a given stat.
/// See: https://forum.unity.com/threads/tutorial-character-stats-aka-attributes-system.504095/
/// </summary>

[Serializable]
public class Stat
{
    [SerializeField][ReadOnly] protected bool isDirty = true;
    [SerializeField][ReadOnly] protected float lastBaseValue = float.MinValue;
    [SerializeField][ReadOnly] protected float value;

    public float BaseValue;
    public virtual float Value { 
        get {
        if (isDirty || lastBaseValue != BaseValue)
        {
            lastBaseValue = BaseValue;
            value = CalculateFinalValue();
            isDirty = false;
        } return value; }
        /*set
        {
            Debug.Log("A");
            isDirty = true;
            this.value = value;
        }*/
    }

    protected List<StatModifier> statModifiers;
    public readonly ReadOnlyCollection<StatModifier> StatModifiers;

    public virtual void AddModifier(StatModifier mod)
    {
        isDirty = true;
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifierOrder);
    }

    public virtual bool RemoveModifier(StatModifier mod)
    {
        if (statModifiers.Remove(mod))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;

        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].Source == source)
            {
                isDirty = true;
                didRemove = true;
                statModifiers.RemoveAt(i);
            }
        }
        return didRemove;
    }

    protected virtual int CompareModifierOrder(StatModifier a, StatModifier b)
    {
        if (a.Order < b.Order)
            return -1;
        else if (a.Order > b.Order)
            return 1;
        return 0; // if (a.Order == b.Order)
    }

    protected virtual float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0; // This will hold the sum of our "PercentAdd" modifiers

        for (int i = 0; i < statModifiers.Count; i++)
        {
            StatModifier mod = statModifiers[i];

            if (mod.Type == StatModType.Flat)
            {
                finalValue += mod.Value;
            }
            else if (mod.Type == StatModType.PercentAdd) // When we encounter a "PercentAdd" modifier
            {
                sumPercentAdd += mod.Value; // Start adding together all modifiers of this type

                // If we're at the end of the list OR the next modifer isn't of this type
                if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != StatModType.PercentAdd)
                {
                    finalValue *= 1 + sumPercentAdd; // Multiply the sum with the "finalValue", like we do for "PercentMult" modifiers
                    sumPercentAdd = 0; // Reset the sum back to 0
                }
            }
            else if (mod.Type == StatModType.PercentMult)
            {
                finalValue *= 1 + mod.Value;
            }
            else if (mod.Type == StatModType.Equals)
            {
                finalValue = mod.Value;
            }
        }

        return (float)Math.Round(finalValue, 4);
    }

    public Stat()
    {
        statModifiers = new List<StatModifier>();
        StatModifiers = statModifiers.AsReadOnly();
    }

    public Stat(float baseValue) : this()
    {
        BaseValue = baseValue;
    }

}