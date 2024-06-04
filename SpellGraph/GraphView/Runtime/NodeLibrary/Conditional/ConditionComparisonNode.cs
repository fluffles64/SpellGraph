using System.Collections.Generic;
using UnityEngine.UIElements;

public class ComparisonNodeData : NodeData
{
    public string Comparison;
}

#if UNITY_EDITOR
[NodeInfo("Comparison", "Conditional", "Compares two floats")]
public class ConditionComparisonNode : ExtendedNode
{
    private string selectedOption = "<";
    private string[] options = new string[] { "<", ">", "<=", ">=", "==", "!=" };

    [InputPort("A")]
    public float valueA;

    [InputPort("B")]
    public float valueB;

    [InputPort("In")]
    public Trigger input;

    [OutputPort("Bool")]
    public bool output;

    public ConditionComparisonNode()
    {
        VisualElement dropdownContainer = DropdownContainer("Comparison", selectedOption, options, newValue => selectedOption = newValue);
        extensionContainer.Add(dropdownContainer);
    }

    #region INodeData
    public override void SetValues(NodeData data)
    {
        if (data is ComparisonNodeData nodeData)
        {
            selectedOption = nodeData.Comparison;
        }

        DropdownField dropdown = extensionContainer.Q<DropdownField>();
        if (dropdown != null)
            dropdown.SetValueWithoutNotify(selectedOption);
    }

    public override object GetNodeData()
    {
        return new ComparisonNodeData
        {
            Id = Id,
            Position = GetPosition().position,
            DataType = new SerializableSystemType(typeof(ComparisonNodeData)),
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames(),
            Comparison = selectedOption,
        };
    }
    #endregion
}
#endif

public class RuntimeConditionComparisonNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        var comparisonNodeData = (ComparisonNodeData)nodeData;

        float a = (float)(portValues[0] ?? 0f);
        float b = (float)(portValues[1] ?? 0f);

        switch (comparisonNodeData.Comparison)
        {
            case "<":
                return a < b;
            case ">":
                return a > b;
            case "<=":
                return a <= b;
            case ">=":
                return a >= b;
            case "==":
                return a == b;
            case "!=":
                return a != b;
            default:
                return false;
        }
    }
}