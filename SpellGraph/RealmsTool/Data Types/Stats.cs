using UnityEngine;

[CreateAssetMenu]
public class Stats : ScriptableObject
{
    private bool resourcesSet = false;

    [SerializeField] private string name = "New Stat";

    [SerializeField] private Stat ad;
    [SerializeField] private Stat ap;
    [SerializeField] private Stat attackSpeed;
    [SerializeField] private Stat critChance;
    [SerializeField] private Stat critDamage;
    [SerializeField] private Stat armorPen;
    [SerializeField] private Stat flatArmorPen;
    [SerializeField] private Stat magicPen;
    [SerializeField] private Stat flatMagicPen;
    [SerializeField] private Stat lifeSteal;

    [SerializeField] private Stat ar;
    [SerializeField] private Stat mr;
    [SerializeField] private Stat tenacity;

    [SerializeField] private Stat moveSpeed;
    [SerializeField] private Stat abilityHaste;
    [SerializeField] private Stat meleeRange;
    [SerializeField] private Stat rangedRange;
    [SerializeField] private Stat goldGeneration;

    [SerializeField] private float health;
    [SerializeField] private Stat maxHealth;
    [SerializeField] private Stat healthRegen;
    [SerializeField] private Stat healthRegenRate;

    [SerializeField] private Resource resourceObj;
    [SerializeField] private float resource;
    [SerializeField] private Stat maxResource;
    [SerializeField] private Stat resourceRegen;
    [SerializeField] private Stat resourceRegenRate;

    public string Name { get { return name; } set { name = value; } }
    public Stat Ad { get { return ad; } set { ad = value; } }
    public Stat Ap { get { return ap; } set { ap = value; } }
    public Stat AttackSpeed { get { return attackSpeed; } set { attackSpeed = value; } }
    public Stat CritChance { get { return critChance; } set { critChance = value; } }
    public Stat CritDamage { get { return critDamage; } set { critDamage = value; } }
    public Stat ArmorPen { get { return armorPen; } set { armorPen = value; } }
    public Stat MagicPen { get { return magicPen; } set { magicPen = value; } }
    public Stat LifeSteal { get { return lifeSteal; } set { lifeSteal = value; } }

    public Stat Ar { get { return ar; } set { ar = value; } }
    public Stat Mr { get { return mr; } set { mr = value; } }
    public Stat Tenacity { get { return tenacity; } set { tenacity = value; } }

    public Stat MoveSpeed { get { return moveSpeed; } set { moveSpeed = value; } }
    public Stat AbilityHaste { get { return abilityHaste; } set { abilityHaste = value; } }
    public Stat MeleeRange { get { return meleeRange; } set { meleeRange = value; } }
    public Stat RangedRange { get { return rangedRange; } set { rangedRange = value; } }
    public Stat GoldGeneration { get { return goldGeneration; } set { goldGeneration = value; } }

    public float Health { get { return health; } set { health = value; } }
    public Stat MaxHealth { get { return maxHealth; } set { maxHealth = value; } }
    public Stat HealthRegen { get { return healthRegen; } set { healthRegen = value; } }
    public Stat HealthRegenRate { get { return healthRegenRate; } set { healthRegenRate = value; } }

    public Resource ResourceObj { get { return resourceObj; } set { resourceObj = value; } }
    public float Resource { get { return resource; } set { resource = value; } }
    public Stat MaxResource { get { if (!resourcesSet) SetResources(); return maxResource; } set { maxResource = value; } }
    public Stat ResourceRegen { get { if (!resourcesSet) SetResources(); return resourceRegen; } set { resourceRegen = value; } }
    public Stat ResourceRegenRate { get { if (!resourcesSet) SetResources(); return resourceRegenRate; } set { resourceRegenRate = value; } }

    private void SetResources()
    {
        if (resourceObj.IsGlobal)
        { 
            maxResource.BaseValue = resourceObj.BaseValue;
            resourceRegen.BaseValue = resourceObj.Regen;
            resourceRegenRate.BaseValue = resourceObj.RegenRate;
        }
        resourcesSet = true;
    }

    private void OnValidate()
    {
        resourcesSet = false;
    }
}