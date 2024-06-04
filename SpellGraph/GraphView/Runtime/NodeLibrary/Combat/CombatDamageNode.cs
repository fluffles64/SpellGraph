using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;

public enum DamageType
{
    Physical, Magic, True
}

public class DamageNodeData : NodeData
{
    public string DamageType;
}

#if UNITY_EDITOR
[NodeInfo("Damage", "Combat", "Deals damage to target")]
public class CombatDamageNode : ExtendedNode
{
    private string selectedDamageType = "Physical";
    private string[] damageTypes = Enum.GetNames(typeof(DamageType));

    [InputPort("Value")]
    public float ValueA;

    [InputPort("In")]
    public Trigger Input;

    [OutputPort("Caused damage")]
    public float ValueB;

    [OutputPort("Out")]
    public Trigger Output;

    public CombatDamageNode()
    {
        VisualElement damageTypeDropdown = DropdownContainer("Type", selectedDamageType, damageTypes, newValue => selectedDamageType = newValue);
        extensionContainer.Add(damageTypeDropdown);
    }

    #region INodeData
    public override void SetValues(NodeData data)
    {
        if (data is DamageNodeData nodeData)
        {
            selectedDamageType = nodeData.DamageType;
        }

        DropdownField damageTypeDropdown = extensionContainer.Q<DropdownField>();
        damageTypeDropdown?.SetValueWithoutNotify(selectedDamageType);
    }

    public override object GetNodeData()
    {
        return new DamageNodeData
        {
            Id = Id,
            Position = GetPosition().position,
            DataType = new SerializableSystemType(typeof(DamageNodeData)),
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames(),
            DamageType = selectedDamageType,
        };
    }
    #endregion
}
#endif

public class RuntimeCombatDamageNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        //if (TargetManager.Instance.Target != null && nodeData is DamageNodeData damageNodeData)
        //{
        //    float damage = GetPortValue<float>(portValues[0]);
        //    Stats stats = TargetManager.Instance.Target.GetComponent<TrainingDummy>().Stats;
        //    float damageDone = 0f;

        //    if (Enum.TryParse(damageNodeData.DamageType, out DamageType damageType))
        //    {
        //        switch (damageType)
        //        {
        //            case DamageType.Physical:
        //                if (stats.Ar.Value >= 0f)
        //                    damageDone = damage * (100f / (100f + stats.Ar.Value));
        //                else
        //                    damageDone = damage * 2 - ((100f / (100f - stats.Ar.Value)));
        //                break;
        //            case DamageType.Magic:
        //                if (stats.Mr.Value >= 0f)
        //                    damageDone = damage * (100f / (100f + stats.Mr.Value));
        //                else
        //                    damageDone = damage * 2 - ((100f / (100f - stats.Mr.Value)));
        //                break;
        //            case DamageType.True:
        //                damageDone = damage;
        //                break;
        //            default:
        //                break;
        //        }
        //    }
        //    else
        //    {
        //        Debug.LogError($"Invalid damage type: {damageNodeData.DamageType}");
        //    }

        //    stats.Health -= damageDone;
        //    return damageDone;
        //}
        return null;
    }
}