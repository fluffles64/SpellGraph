using UnityEngine.UIElements;
using System.Collections.Generic;
using System;

public class StatNodeData : NodeData
{
    public string Target;
    public string Stat;
}

#if UNITY_EDITOR
[NodeInfo("Get Stat", "Stats", "Gets stats of a target")]
public class GetStatNode : ExtendedNode
{
    private string selectedTarget = "Player";
    private string selectedStat = "Health";
    private string[] targets = Enum.GetNames(typeof(CurrentTargetType));
    private string[] stats = Enum.GetNames(typeof(StatType));

    [OutputPort("Value")]
    public float Value;

    [InputPort("In")]
    public Trigger Input;

    [OutputPort("Out")]
    public Trigger Output;

    public GetStatNode()
    {
        VisualElement targetDropdown = DropdownContainer("Target", selectedTarget, targets, newValue => selectedTarget = newValue);
        titleContainer.Add(targetDropdown);

        VisualElement statDropdown = DropdownContainer("Stat", selectedStat, stats, newValue => selectedStat = newValue);
        extensionContainer.Add(statDropdown);
    }

    #region INodeData
    public override void SetValues(NodeData data)
    {
        if (data is StatNodeData nodeData)
        {
            selectedTarget = nodeData.Target;
            selectedStat = nodeData.Stat;
        }

        DropdownField targetDropdown = titleContainer.Q<DropdownField>();
        targetDropdown?.SetValueWithoutNotify(selectedTarget);

        DropdownField statDropdown = extensionContainer.Q<DropdownField>();
        statDropdown?.SetValueWithoutNotify(selectedStat);
    }

    public override object GetNodeData()
    {
        return new StatNodeData
        {
            Id = Id,
            Position = GetPosition().position,
            DataType = new SerializableSystemType(typeof(StatNodeData)),
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames(),
            Target = selectedTarget,
            Stat = selectedStat
        };
    }
    #endregion
}
#endif

public class RuntimeGetStatNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        //var statNodeData = (StatNodeData)nodeData;

        //if (statNodeData.Target == "Player")
        //{
        //    var hero = TargetManager.Instance.Player;
        //    float statValue = StatsUtil.GetStatValue((StatType)Enum.Parse(typeof(StatType), statNodeData.Stat), hero);
        //    return statValue;
        //}
        //else if(statNodeData.Target == "Target")
        //{
        //    // ...
        //}

        return null;
    }
}