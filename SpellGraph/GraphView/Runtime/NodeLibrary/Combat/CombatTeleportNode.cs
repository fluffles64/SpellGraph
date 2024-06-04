using UnityEngine.UIElements;
using UnityEngine;
using System.Collections.Generic;
using System;

public enum TeleportType
{
    Target, BehindTarget, CurrentPlusValue, TargetPlusValue 
}

public class CombatTeleportNodeData : NodeData
{
    public string SelectedOption;
}

#if UNITY_EDITOR
[NodeInfo("Teleport", "Combat", "Teleports you to a position")]
public class CombatTeleportNode : ExtendedNode
{
    private string selectedOption = "Target";
    private string[] teleportTypes = Enum.GetNames(typeof(TeleportType));

    [InputPort("Value")]
    public Vector4 Value;

    [InputPort("In")]
    public Trigger Input;

    [OutputPort("New Value")]
    public Vector4 NewValue;

    [OutputPort("Old Value")]
    public Vector4 OldValue;

    [OutputPort("Out")]
    public Trigger Output;

    public CombatTeleportNode()
    {
        VisualElement dropdown = DropdownContainer("TP Type", selectedOption, teleportTypes, newValue => selectedOption = newValue);
        extensionContainer.Add(dropdown);
    }

    #region INodeData
    public override void SetValues(NodeData data)
    {
        if (data is CombatTeleportNodeData nodeData)
        {
            selectedOption = nodeData.SelectedOption;
        }

        DropdownField dropdown = extensionContainer.Q<DropdownField>();
        dropdown?.SetValueWithoutNotify(selectedOption);
    }

    public override object GetNodeData()
    {
        return new CombatTeleportNodeData
        {
            Id = Id,
            Position = GetPosition().position,
            DataType = new SerializableSystemType(typeof(CombatTeleportNodeData)),
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames(),
            SelectedOption = selectedOption,
        };
    }
    #endregion
}
#endif

public class RuntimeCombatTeleportNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        //CombatTeleportNodeData teleportNodeData = nodeData as CombatTeleportNodeData;
        //if (teleportNodeData == null) return null;

        //Hero hero = TargetManager.Instance.Player;
        //GameObject target = TargetManager.Instance.Target;

        //if (target == null) return null;

        //Vector4? teleportValue = portValues[0] as Vector4?;
        //Vector4 oldValue = new Vector4(hero.gameObject.transform.position.x, hero.gameObject.transform.position.y, hero.gameObject.transform.position.z, hero.gameObject.transform.rotation.eulerAngles.y);

        //hero.gameObject.GetComponent<CharacterController>().enabled = false;

        //switch (teleportNodeData.SelectedOption)
        //{
        //    case "Target":
        //        hero.gameObject.transform.position = target.transform.position;
        //        break;
        //    case "BehindTarget":
        //        hero.gameObject.transform.position = target.transform.position - new Vector3(0f, 0f, 2f);
        //        hero.gameObject.transform.forward = target.transform.forward;
        //        break;
        //    case "CurrentPlusValue":
        //        if (teleportValue != null)
        //        {
        //            hero.gameObject.transform.position += new Vector3(teleportValue.Value.x, teleportValue.Value.y, teleportValue.Value.z);
        //            hero.gameObject.transform.forward = Quaternion.Euler(0f, teleportValue.Value.w, 0f) * hero.gameObject.transform.forward;
        //        }
        //        break;
        //    case "TargetPlusValue":
        //        if (teleportValue != null)
        //        {
        //            hero.gameObject.transform.position = target.transform.position + new Vector3(teleportValue.Value.x, teleportValue.Value.y, teleportValue.Value.z);
        //            hero.gameObject.transform.forward = Quaternion.Euler(0f, teleportValue.Value.w, 0f) * hero.gameObject.transform.forward;
        //        }
        //        break;
        //    default:
        //        return null;
        //}

        //hero.gameObject.GetComponent<CharacterController>().enabled = true;

        //Vector3 newPosition = hero.gameObject.transform.position;
        //float newYRotation = hero.gameObject.transform.rotation.eulerAngles.y;

        //return new List<object> { new Vector4(newPosition.x, newPosition.y, newPosition.z, newYRotation), oldValue };
        
        return null;
    }
}