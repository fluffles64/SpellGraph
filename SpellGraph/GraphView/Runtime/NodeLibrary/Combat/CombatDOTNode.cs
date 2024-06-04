using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;

public class DOTNodeData : NodeData
{
    public string DamageType;
    public float Rate;
    public bool Stackable;
    public bool Synced;
    public int MaxStacks;
}

#if UNITY_EDITOR
[NodeInfo("DOT", "Combat", "Deals damage over time")]
public class CombatDOTNode : ExtendedNode
{
    private string selectedDamageType = "Physical";
    private string[] damageTypes = Enum.GetNames(typeof(DamageType));
    private float rate;
    private bool stackable;
    private bool synced;
    private int maxStacks;

    [InputPort("Damage")]
    public float ValueA;

    [InputPort("Duration")]
    public float Duration;

    [InputPort("In")]
    public Trigger Input;

    [OutputPort("Tick damage")]
    public float ValueB;

    [OutputPort("Out")]
    public Trigger Output;

    public CombatDOTNode()
    {
        var rateField = new FloatField("Rate (s)") { style = { width = 250 } };
        rateField.RegisterValueChangedCallback(e => rate = Mathf.Max(e.newValue, 0.05f));
        extensionContainer.Add(rateField);
        
        var syncedToggle = new Toggle("Synced [?]") { name = "Synced", value = true };
        var maxStacksField = new IntegerField("Max stacks") { style = { width = 250 }};

        var stackableToggle = new Toggle("Stackable [?]") { name = "Stackable" };
        stackableToggle.RegisterValueChangedCallback(e =>
        {
            stackable = e.newValue;
            syncedToggle.SetEnabled(stackable);
            maxStacksField.SetEnabled(stackable);
            maxStacks = stackable ? Mathf.Max(maxStacks, 0) : 0;
        });
        extensionContainer.Add(stackableToggle);

        syncedToggle.SetEnabled(stackable);
        syncedToggle.RegisterValueChangedCallback(e => synced = e.newValue);
        extensionContainer.Add(syncedToggle);

        maxStacksField.SetEnabled(stackable);
        maxStacksField.RegisterValueChangedCallback(e => maxStacks = Mathf.Max(e.newValue, 0));
        extensionContainer.Add(maxStacksField);

        VisualElement damageTypeDropdown = DropdownContainer("Type", selectedDamageType, damageTypes, newValue => selectedDamageType = newValue);
        extensionContainer.Add(damageTypeDropdown);
    }

    #region INodeData
    public override void SetValues(NodeData data)
    {
        if (data is DOTNodeData nodeData)
        {
            selectedDamageType = nodeData.DamageType;
            rate = Mathf.Max(nodeData.Rate, 0.05f);
            stackable = nodeData.Stackable;
            synced = nodeData.Synced;
            maxStacks = stackable ? Mathf.Max(nodeData.MaxStacks, 0) : 0;
        }

        var rateField = extensionContainer.Q<FloatField>();
        rateField?.SetValueWithoutNotify(rate);

        var stackableToggle = extensionContainer.Q<Toggle>("Stackable");
        stackableToggle?.SetValueWithoutNotify(stackable);

        var syncedToggle = extensionContainer.Q<Toggle>("Synced");
        syncedToggle?.SetValueWithoutNotify(synced);
        syncedToggle?.SetEnabled(stackable);

        var maxStacksField = extensionContainer.Q<IntegerField>();
        maxStacksField?.SetValueWithoutNotify(maxStacks);
        maxStacksField?.SetEnabled(stackable);

        DropdownField damageTypeDropdown = extensionContainer.Q<DropdownField>();
        damageTypeDropdown?.SetValueWithoutNotify(selectedDamageType);
    }

    public override object GetNodeData()
    {
        return new DOTNodeData
        {
            Id = Id,
            Position = GetPosition().position,
            DataType = new SerializableSystemType(typeof(DOTNodeData)),
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames(),
            DamageType = selectedDamageType,
            Rate = Mathf.Max(rate, 0.05f),
            Stackable = stackable,
            Synced = synced,
            MaxStacks = stackable ? Mathf.Max(maxStacks, 0) : 0
        };
    }
    #endregion
}
#endif

public class RuntimeCombatDOTNode : ActionNode
{
    private float damage;
    private float rate;
    private string damageType;
    
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        //if (TargetManager.Instance.Target != null && nodeData is DOTNodeData dotNodeData)
        //{
        //    damage = GetPortValue<float>(portValues[0]);
        //    float duration = GetPortValue<float>(portValues[1]);
        //    rate = dotNodeData.Rate;
        //    damageType = dotNodeData.DamageType;

        //    float delay = 0f;
        //    if (dotNodeData.Stackable && TargetManager.Instance.CoroutineRunning && TargetManager.Instance.CurrentStacks <= dotNodeData.MaxStacks)
        //    {
        //        // Get time for next tick in the current running coroutine
        //        float elapsedTimeSinceStart = Time.time - TargetManager.Instance.LastStartTime;
        //        float ticksElapsed = Mathf.Floor(elapsedTimeSinceStart / rate);
        //        float elapsedTimeSinceLastTick = elapsedTimeSinceStart % rate;
        //        delay = elapsedTimeSinceLastTick > 0 ? rate - elapsedTimeSinceLastTick : 0;
        //        TargetManager.Instance.StartDOTCoroutine(DealDamageOverTime(duration, delay), dotNodeData.Stackable);
        //    }
        //    else if (!dotNodeData.Stackable || !TargetManager.Instance.CoroutineRunning)
        //    {
        //        TargetManager.Instance.StartDOTCoroutine(DealDamageOverTime(duration, 0f), dotNodeData.Stackable);
        //    }
        //    //else if (!dotNodeData.Stackable && TargetManager.Instance.CoroutineRunning)
        //    //{
        //    //    TargetManager.Instance.StartDOTCoroutine(DealDamageOverTime(duration, 0f), dotNodeData.Stackable);
        //    //    TargetManager.Instance.CurrentStacks = 1;
        //    //}

        //    return CalculateDamage(damage, damageType, TargetManager.Instance.Target.GetComponent<TrainingDummy>().Stats);
        //}
        return null;
    }

    /*private IEnumerator<WaitForSeconds> DealDamageOverTime(float duration, float delay)
    {
        TargetManager.Instance.CoroutineRunning = true;
        TargetManager.Instance.LastStartTime = Time.time;
        TargetManager.Instance.CurrentStacks++;

        float elapsedTime = 0f;
        Stats stats = TargetManager.Instance.Target.GetComponent<TrainingDummy>().Stats;

        // Delay the coroutine start if it's synced
        if (delay > 0f)
        {
            delay -= Time.deltaTime;
            yield return new WaitForSeconds(delay);
        }

        while (elapsedTime < duration)
        {
            yield return new WaitForSeconds(rate);
            float damageDone = CalculateDamage(damage, damageType, stats);
            stats.Health -= damageDone;
            Debug.Log("Damage dealt");
            elapsedTime += rate;
        }

        TargetManager.Instance.CoroutineRunning = false;
        TargetManager.Instance.CurrentStacks--;
    }

    private float CalculateDamage(float damage, string damageType, Stats stats)
    {
        float damageDone = 0f;

        if (Enum.TryParse(damageType, out DamageType type))
        {
            switch (type)
            {
                case DamageType.Physical:
                    if (stats.Ar.Value >= 0f)
                        damageDone = damage * (100f / (100f + stats.Ar.Value));
                    else
                        damageDone = damage * 2 - ((100f / (100f - stats.Ar.Value)));
                    break;
                case DamageType.Magic:
                    if (stats.Mr.Value >= 0f)
                        damageDone = damage * (100f / (100f + stats.Mr.Value));
                    else
                        damageDone = damage * 2 - ((100f / (100f - stats.Mr.Value)));
                    break;
                case DamageType.True:
                    damageDone = damage;
                    break;
                default:
                    break;
            }
        }
        else
        {
            Debug.LogError($"Invalid damage type: {damageType}");
        }

        return damageDone;
    }*/
}