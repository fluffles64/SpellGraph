using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
#endif

public class SetVarNodeData : NodeData
{
    public string SelectedVariable;
    public string[] Variables;
    public SerializableSystemType SelectedVariableType;
    public SerializableSystemType[] VariableTypes;
}

#if UNITY_EDITOR
[NodeInfo("SetVar", "General", "Sets a variable from the blackboard")]
public class SetVarNode : ExtendedNode
{
    private string selectedVariable = "None";
    private Type selectedVariableType = typeof(float);
    private static string[] variables = new string[] { };
    private static Type[] variableTypes = new Type[] { };
    private Dictionary<Type, Type> variableTypeMap = new Dictionary<Type, Type>
    {
        { typeof(InstanceVariableInt), typeof(int) },
        { typeof(InstanceVariableFloat), typeof(float) },
        { typeof(InstanceVariableString), typeof(string) },
        { typeof(InstanceVariableBool), typeof(bool) },
        { typeof(InstanceVariableVector2), typeof(Vector2) },
        { typeof(InstanceVariableVector3), typeof(Vector3) },
        { typeof(InstanceVariableVector4), typeof(Vector4) },
        { typeof(InstanceVariableGameObject), typeof(GameObject) },
    }; // TODO: Make this a util so i dont have 500 of them
    
    [InputPort("Value")]
    public Port value;

    [InputPort("In")]
    public Trigger input;

    [OutputPort("New Value")]
    public Port newValue;

    [OutputPort("Old Value")]
    public Port oldValue;

    [OutputPort("Out")]
    public Trigger output;

    public SetVarNode()
    {
        VisualElement dropdownContainer = DropdownContainer("", selectedVariable, variables, newValue => UpdatePortTypes(newValue));
        titleContainer.Add(dropdownContainer);
    }

    private void UpdatePortTypes(string newVar)
    {
        selectedVariable = newVar;
        int selectedIndex = Array.IndexOf(variables, selectedVariable);

        if (selectedIndex != -1 && variableTypes != null && selectedIndex < variableTypes.Length)
        {
            selectedVariableType = variableTypes[selectedIndex];

            if (variableTypeMap.TryGetValue(selectedVariableType, out Type targetType))
            {
                selectedVariableType = targetType;

                foreach (Port port in inputContainer.Children().Concat(outputContainer.Children()))
                {
                    if (port.portName != "In" && port.portName != "Out")
                    {
                        port.portType = selectedVariableType;
                        var typeLabel = port.Q<Label>("typeLabel");
                        if (typeLabel != null)
                            typeLabel.text = selectedVariableType.Name;
                    }
                }

                RefreshPorts();
            }
            else
            {
                Debug.LogError($"No mapping found for variable type '{selectedVariableType}'.");
            }
        }
    }

    public static void UpdateDropdownOptions(string[] instanceVariableNames, Type[] instanceVariableTypes)
    {
        variables = instanceVariableNames;
        variableTypes = instanceVariableTypes;
    }

    #region INodeData
    public override void SetValues(NodeData data)
    {
        if (data is SetVarNodeData nodeData)
        {
            selectedVariable = nodeData.SelectedVariable;
            variables = nodeData.Variables;
            selectedVariableType = nodeData.SelectedVariableType.SystemType;
            // Convert array of SerializableSystemType to Type
            variableTypes = new Type[nodeData.VariableTypes.Length];
            for (int i = 0; i < nodeData.VariableTypes.Length; i++)
            {
                variableTypes[i] = nodeData.VariableTypes[i].SystemType;
            }

            // Add the selected variable to the array if it doesn't exist
            if (!variables.Contains(selectedVariable))
            {
                variables = variables.Append(selectedVariable).ToArray();
            }

            DropdownField variableDropdown = titleContainer.Q<DropdownField>();
            if (variableDropdown != null)
            {
                variableDropdown.choices = variables.ToList();
                variableDropdown.SetValueWithoutNotify(selectedVariable);
            }

            // Update ports when setting values
            UpdatePortTypes(selectedVariable);
        }
    }

    public override object GetNodeData()
    {
        // Convert each Type object in the variableTypes array to SerializableSystemType
        SerializableSystemType[] serializableVariableTypes = new SerializableSystemType[variableTypes.Length];
        for (int i = 0; i < variableTypes.Length; i++)
        {
            serializableVariableTypes[i] = new SerializableSystemType(variableTypes[i]);
        }

        return new SetVarNodeData
        {
            Id = Id,
            Position = GetPosition().position,
            DataType = new SerializableSystemType(typeof(SetVarNodeData)),
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames(),
            SelectedVariable = selectedVariable,
            Variables = variables,
            SelectedVariableType = new SerializableSystemType(selectedVariableType),
            VariableTypes = serializableVariableTypes,
        };
    }
    #endregion
}
#endif

public class RuntimeSetVarNode : ActionNode
{
    public Effect Effect;

    public override object Action(List<object> portValues, NodeData nodeData)
    {
        object value = portValues[0];
        var setVarNodeData = (SetVarNodeData)nodeData;
        Type selectedVariableType = setVarNodeData.SelectedVariableType.SystemType;

        if (!selectedVariableType.IsAssignableFrom(value.GetType()) || Effect == null || portValues[0] == null) return null;

        foreach (var variable in Effect.InstanceVariables)
        {
            if (variable.Name == setVarNodeData.SelectedVariable)
            {
                object oldValue;

                switch (variable)
                {
                    case InstanceVariableFloat floatVar:
                        oldValue = floatVar.Value;
                        floatVar.Value = (float)Convert.ChangeType(value, typeof(float));
                        break;
                    case InstanceVariableInt intVar:
                        oldValue = intVar.Value;
                        intVar.Value = (int)Convert.ChangeType(value, typeof(int));
                        break;
                    case InstanceVariableString stringVar:
                        oldValue = stringVar.Value;
                        stringVar.Value = (string)Convert.ChangeType(value, typeof(string));
                        break;
                    case InstanceVariableBool boolVar:
                        oldValue = boolVar.Value;
                        boolVar.Value = (bool)Convert.ChangeType(value, typeof(bool));
                        break;
                    case InstanceVariableVector2 vector2Var:
                        oldValue = vector2Var.Value;
                        vector2Var.Value = (Vector2)Convert.ChangeType(value, typeof(Vector2));
                        break;
                    case InstanceVariableVector3 vector3Var:
                        oldValue = vector3Var.Value;
                        vector3Var.Value = (Vector3)Convert.ChangeType(value, typeof(Vector3));
                        break;
                    case InstanceVariableVector4 vector4Var:
                        oldValue = vector4Var.Value;
                        vector4Var.Value = (Vector4)Convert.ChangeType(value, typeof(Vector4));
                        break;
                    case InstanceVariableGameObject gameObjectVar:
                        oldValue = gameObjectVar.Value;
                        gameObjectVar.Value = (GameObject)Convert.ChangeType(value, typeof(GameObject));
                        break;
                    default:
                        Debug.LogError($"Unsupported instance variable type: {variable.GetType().Name}");
                        return null;
                }
                return new List<object> { value, oldValue };
            }
        }
        return null;
    }
}