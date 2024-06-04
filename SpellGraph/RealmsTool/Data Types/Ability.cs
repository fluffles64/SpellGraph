using System;
using System.Collections.Generic;
using UnityEngine;

public enum Target { Enemy, Ally, Anyone }
public enum TargetType { None, Target, ChosenArea }
public enum CurrentTargetType { Player, Target }
public enum ActivationType { Active, Passive }
[Flags]
public enum CancelType { AutoAttacking = 1, Casting = 2, Stealth = 4 } // Backing == Casting?
public enum CastType { Instant, Casted, Channeled, Pressed }
public enum CostType { None, Flat, TotalPercentage, CurrentPercentage, MixedTotal, MixedCurrent }
public enum ChargeType { Constant, OneTime }
public enum RangeType { None, Melee, Ranged, Custom }
public enum TriggerType { None, OnCombatEnter, OnResourceTreshold, Custom }
public enum ResourceCostType { Resource, Health, Custom }

// Effect enums
public enum EffectType { Damage, Healing, Resource, Utility, CC } // Add Mana, energy, fury and all options inside resource
public enum ValueType { Flat, AP, AD, Mixed, FlatMixed } // Add Mana, energy, fury and all options inside resource
public enum AreaType { None, AreaAroundTarget }
public enum DurationType { Finite, Infinite }
public enum OverTimeEffectType { TPS, Gradual, Constant }

[Flags]
public enum CrowdControlType
{
    Stun = 1, Slow = 2, Root = 4, Ground = 8, Suppression = 16,
    Silence = 32, Polymorph = 64,
    Taunt = 128, Fear = 256, Charm = 512, Berserk = 1024,
    Petrify = 2048, Sleep = 4096, Kinematic = 8192
}

[Serializable]
public class Ability : ScriptableObject
{
    [SerializeField, ReadOnly] private string id = Guid.NewGuid().ToString();
    [SerializeField] private Sprite icon;
    [SerializeField] private string name = "New Ability";
    [SerializeField][TextAreaAttribute(0, 10)] private string description;

    [SerializeField] private ActivationType activationType;
    [SerializeField] private TargetType targetType;
    [SerializeField] private Target targets;
    [SerializeField] private bool needToFaceTarget;
    [SerializeField][Range(0f, 180f)] private float targetAngle = 110f;
    [SerializeField] private bool isTargetBackwards;
    [SerializeField] private CancelType cancelType;
    [SerializeField] private TriggerType trigger;
    [SerializeField] private CrowdControlType cantUseWhile;
    /*[SerializeField] private List<CrowdControlType> cantUseWhile = new List<CrowdControlType> { CrowdControlType.Silence, CrowdControlType.Polymorph, CrowdControlType.Stun, 
                                                                                                CrowdControlType.Suppression, CrowdControlType.Charm, CrowdControlType.Sleep, 
                                                                                                CrowdControlType.Fear, CrowdControlType.Petrify };*/
    [SerializeField] private CastType castType;
    [SerializeField] private float castTime;
    [SerializeField] private Vector2 pressSweetSpot;
    [SerializeField] private bool moveAndCast;
    [SerializeField] private bool slowsCaster;
    [SerializeField][Range(0, 100)] private int slowAmount;
    [SerializeField] private CurveType castSlowCurveType;
    [SerializeField] private EasingType castSlowEasingType;
    [SerializeField][AnimationCurve(0, 0, 1f, 1f)] private AnimationCurve castSlowCurve; //https://forum.unity.com/threads/changing-how-animation-curve-window-looks.488841/

    [SerializeField] private float cooldown;
    [SerializeField] private bool hasCharges;
    [SerializeField] private ChargeType chargeType;
    [SerializeField] private int numOfCharges;
    [SerializeField] private float chargeCooldown;

    [SerializeField] private CostType costType;
    [SerializeField] private ResourceCostType resource;
    [SerializeField] private float minimumResourceCost;
    [SerializeField] private bool isFromBaseResource;
    [SerializeField] private float resourceFlatCost;
    [SerializeField] [Range(0f, 100f)]private float resourcePercentageCost;
    [SerializeField] private float costPerSecond;

    [SerializeField] private RangeType rangeType;
    [SerializeField] private Vector2 range;

    [SerializeField] private List<Effect> effects;


    public string Id { get { if (string.IsNullOrEmpty(id)) { id = Guid.NewGuid().ToString(); } return id; } private set { id = value; } }
    public Sprite Icon { get { return icon; } set { icon = value; } }
    public string Name { get { return name; } set { name = value; } }
    public string Description { get { return description; } private set { description = value; } }

    public ActivationType ActivationType { get { return activationType; } private set { activationType = value; } }
    public TargetType TargetType { get { return targetType; } private set { targetType = value; } }
    public Target Targets { get { return targets; } private set { targets = value; } }   
    public bool NeedToFaceTarget { get { return needToFaceTarget; } private set { needToFaceTarget = value; } }
    public float TargetAngle { get { return targetAngle; } private set { targetAngle = value; } }
    public bool IsTargetBackwards { get { return isTargetBackwards; } private set { isTargetBackwards = value; } }
    public CancelType CancelType { get { return cancelType; } private set { cancelType = value; } }
    public TriggerType Trigger { get { return trigger; } private set { trigger = value; } }
    public CrowdControlType CantUseWhile { get { return cantUseWhile; } set { cantUseWhile = value; } }

    public CastType CastType { get { return castType; } private set { castType = value; } }
    public float CastTime { get { return castTime; } private set { castTime = value; } }
    public Vector2 PressSweetSpot { get { return pressSweetSpot; } private set { pressSweetSpot = value; } }
    public bool MoveAndCast { get { return moveAndCast; } private set { moveAndCast = value; } }
    public bool SlowsCaster { get { return slowsCaster; } private set { slowsCaster = value; } }
    public int SlowAmount { get { return slowAmount; } private set { slowAmount = value; } }
    public CurveType CastSlowCurveType { get { return castSlowCurveType; } private set { castSlowCurveType = value; } }
    public EasingType CastSlowEasingType { get { return castSlowEasingType; } private set { castSlowEasingType = value; } }
    public AnimationCurve CastSlowCurve { get { return castSlowCurve; } set { castSlowCurve = value; } }

    public float Cooldown { get { return cooldown; } private set { cooldown = value; } }
    public bool HasCharges { get { return hasCharges; } private set { hasCharges = value; } }
    public ChargeType ChargeType { get { return chargeType; } private set { chargeType = value; } }
    public int NumOfCharges { get { return numOfCharges; } private set { numOfCharges = value; } }
    public float ChargeCooldown { get { return chargeCooldown; } private set { chargeCooldown = value; } }

    public CostType CostType { get { return costType; } private set { costType = value; } }
    public ResourceCostType Resource { get { return resource; } private set { resource = value; } }
    public float MinimumResourceCost { get { return minimumResourceCost; } private set { minimumResourceCost = value; } }
    public bool IsFromBaseResource { get { return isFromBaseResource; } private set { isFromBaseResource = value; } }
    public float ResourceFlatCost { get { return resourceFlatCost; } private set { resourceFlatCost = value; } }
    public float ResourcePercentageCost { get { return resourcePercentageCost; } private set { resourcePercentageCost = value; } }
    public float CostPerSecond { get { return costPerSecond; } private set { costPerSecond = value; } }

    public RangeType RangeType { get { return rangeType; } private set { rangeType = value; } }
    public Vector2 Range { get { return range; } private set { range = value; } }

    public List<Effect> Effects { get { return effects; } private set { effects = value; } }
    
    public void Activate(Stats stats) 
    {
        foreach (var effect in effects)
        {
            ExecuteEffect execute = new ExecuteEffect();
            execute.Effect = effect;
            execute.StartExecution();
        }
    }
    public virtual void BeginCooldown(Stats stats) { }
}