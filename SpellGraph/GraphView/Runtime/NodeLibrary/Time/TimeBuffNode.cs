using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;

public class BuffNodeData : NodeData
{
    public bool IsDebuff;
    public float Duration;
    public Sprite Icon;
    public string Tooltip;
}

#if UNITY_EDITOR
[NodeInfo("Buff", "Time", "Buffs for x seconds")]
public class TimeBuffNode : ExtendedNode
{
    private bool isDebuff;
    private float duration;
    private Sprite icon;
    private string tooltip;

    [InputPort("In")]
    public Trigger Input;

    [OutputPort("Out")]
    public Trigger Output;

    public TimeBuffNode()
    {
        var iconImage = new Image { style = { maxWidth = 36, maxHeight = 36 } };
        titleContainer.Add(iconImage);

        var isDebuffToggle = new Toggle("Is Debuff [?]");
        isDebuffToggle.RegisterValueChangedCallback(e => isDebuff = e.newValue);
        extensionContainer.Add(isDebuffToggle);

        var durationField = new FloatField("Duration") { style = { width = 250 } };
        durationField.RegisterValueChangedCallback(e => duration = e.newValue);
        extensionContainer.Add(durationField);

        var spriteField = new UnityEditor.UIElements.ObjectField("Icon") { objectType = typeof(Sprite), style = { width = 250 } };
        spriteField.RegisterValueChangedCallback(e =>
        {
            icon = e.newValue as Sprite;
            iconImage.image = icon != null ? icon.texture : null;
        });
        extensionContainer.Add(spriteField);

        var tooltipField = new TextField("Tooltip") { style = { width = 250, flexDirection = FlexDirection.Column }, multiline = true };
        tooltipField.RegisterValueChangedCallback(e => tooltip = e.newValue);
        tooltipField.Q<Label>().style.marginBottom = 5;
        extensionContainer.Add(tooltipField);
    }

    #region INodeData
    public override void SetValues(NodeData data)
    {
        if (data is BuffNodeData nodeData)
        {
            isDebuff = nodeData.IsDebuff;
            duration = nodeData.Duration;
            icon = nodeData.Icon;
            tooltip = nodeData.Tooltip;

            var iconImage = titleContainer.Q<Image>();
            if (iconImage != null)
                iconImage.image = icon != null ? icon.texture : null;

            var debuffToggle = extensionContainer.Q<Toggle>();
            debuffToggle?.SetValueWithoutNotify(isDebuff);

            var durationField = extensionContainer.Q<FloatField>();
            durationField?.SetValueWithoutNotify(duration);

            var spriteField = extensionContainer.Q<UnityEditor.UIElements.ObjectField>();
            spriteField?.SetValueWithoutNotify(icon);

            var tooltipField = extensionContainer.Q<TextField>();
            tooltipField?.SetValueWithoutNotify(tooltip);
        }
    }

    public override object GetNodeData()
    {
        return new BuffNodeData
        {
            Id = Id,
            Position = GetPosition().position,
            DataType = new SerializableSystemType(typeof(BuffNodeData)),
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames(),
            IsDebuff = isDebuff,
            Duration = duration,
            Icon = icon,
            Tooltip = tooltip,
        };
    }
    #endregion
}
#endif

public class RuntimeTimeBuffNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        //if (nodeData is BuffNodeData buffNodeData)
        //    CanvasManager.Instance.AddBuff(buffNodeData.IsDebuff, buffNodeData.Duration, buffNodeData.Icon, buffNodeData.Tooltip);
        return null;
    }
}