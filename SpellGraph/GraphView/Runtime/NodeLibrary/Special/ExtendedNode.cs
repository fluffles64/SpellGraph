using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using UnityEngine.UIElements;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;

/// <summary>
/// Base class all nodes extend from. Node instantiation (ports, styling...) is done inside the constructor.
/// Contains several helper methods and methods to streamline the styling process of custom nodes.
/// Inherits from INodeData. Nodes containing custom additional information must override the SetValues() and GetNodeData()
/// methods so that s.o. saving/loading works correctly. ExtendedNode is an editor class, and is not to be confused with
/// runtime nodes. This class only handles the editor part of things and heavily relies on the GraphView API.
/// </summary>

public class ExtendedNode : Node, INodeData
{
    private Dictionary<string, Port> inputPorts = new Dictionary<string, Port>();
    private Dictionary<string, Port> outputPorts = new Dictionary<string, Port>();
    public string Id;

    public ExtendedNode()
    {
        var nodeType = GetType();
        var nodeInfoAttribute = nodeType.GetCustomAttribute<NodeInfoAttribute>();
        string libraryPath = "";
        if (nodeInfoAttribute != null)
        {
            title = nodeInfoAttribute.title;
            libraryPath = nodeInfoAttribute.libraryPath;
            tooltip = nodeInfoAttribute.tooltip;
        }
        Id = Guid.NewGuid().ToString();

        // Node color
        if (libraryPath != null)
        {
            Color originalColor = NodeColorUtil.GetColor(libraryPath);
            Color defaultColor = NodeColorUtil.GetColor("Default");

            if (originalColor != defaultColor)
            {
                Color.RGBToHSV(originalColor, out float h, out float s, out float v);

                // Saturation
                s *= 0.75f;

                // Value
                v = Mathf.Clamp01(v - 0.5f);

                Color adjustedColor = Color.HSVToRGB(h, s, v);
                elementTypeColor = adjustedColor;
                titleContainer.style.backgroundColor = elementTypeColor;

                var labelElement = titleContainer.Q<Label>();
                if (labelElement != null)
                {
                    labelElement.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic;
                    labelElement.style.color = new Color(0.95f, 0.95f, 0.95f, 0.9f);
                }
            }
        }

        styleSheets.Add(Resources.Load<StyleSheet>("Node"));
        extensionContainer.AddToClassList("extension-container");

        this.CreateInputPortsFromAttributes();
        this.CreateOutputPortsFromAttributes();

        RefreshNode();
    }

    public void SetInputPort(string portName, Type type, Port.Capacity capacity)
    {
        Port port = InstantiatePort(Orientation.Horizontal, Direction.Input, capacity, type);
        port.portName = portName;
        if (type == typeof(Trigger))
            port.portColor = Color.green;
        inputContainer.Add(port);
        inputPorts[portName] = port;
    }

    public void SetOutputPort(string portName, Type type, Port.Capacity capacity)
    {
        Port port = InstantiatePort(Orientation.Horizontal, Direction.Output, capacity, type);
        port.portName = portName;
        if (type == typeof(Trigger))
            port.portColor = Color.green;
        outputContainer.Add(port);
        outputPorts[portName] = port;
    }

    public void DisconnectPorts(GraphView graph)
    {
        VisualElement[] containers = new VisualElement[] { inputContainer, outputContainer, titleContainer };
        foreach (var container in containers)
        {
            foreach (var visualElement in container.Children())
            {
                if (visualElement is Port port && port.connected)
                {
                    graph.DeleteElements(port.connections);
                }
            }
        }
    }

    public string[] GetInputPortNames()
    {
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        List<string> inputPortDisplayNames = new List<string>();
        foreach (FieldInfo field in fields)
        {
            InputPortAttribute inputPortAttribute = Attribute.GetCustomAttribute(field, typeof(InputPortAttribute)) as InputPortAttribute;
            if (inputPortAttribute != null && inputPortAttribute.displayName != "In")
            {
                inputPortDisplayNames.Add(inputPortAttribute.displayName);
            }
        }
        return inputPortDisplayNames.ToArray();
    }

    public string[] GetOutputPortNames()
    {
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
        List<string> outputPortDisplayNames = new List<string>();
        foreach (FieldInfo field in fields)
        {
            OutputPortAttribute outputPortAttribute = Attribute.GetCustomAttribute(field, typeof(OutputPortAttribute)) as OutputPortAttribute;
            if (outputPortAttribute != null && outputPortAttribute.displayName != "Out")
            {
                outputPortDisplayNames.Add(outputPortAttribute.displayName);
            }
        }
        return outputPortDisplayNames.ToArray();
    }

    public Type GetRuntimeType()
    {
        Type runtimeType = Type.GetType("Runtime" + GetType().Name, throwOnError: false);
        if (runtimeType != null)
            return runtimeType;
        else
            Debug.LogError("Failed to find or assign runtime type for node: " + GetType().Name);
        return null;
    }

    public VisualElement DropdownContainer(string labelText, string defaultSelection, string[] options, Action<string> onValueChanged)
    {
        var dropdownContainer = new VisualElement();
        dropdownContainer.style.flexDirection = FlexDirection.Row;
        dropdownContainer.style.alignItems = Align.Center;
        dropdownContainer.style.justifyContent = Justify.SpaceBetween;

        var label = new Label(labelText);
        label.style.marginRight = 4;
        label.style.marginLeft = 4;
        label.style.marginTop = 4;
        label.style.marginBottom = 4;
        label.style.unityTextAlign = TextAnchor.MiddleLeft;

        var dropdown = new DropdownField();
        dropdown.choices = options.ToList();
        dropdown.SetValueWithoutNotify(defaultSelection);
        dropdown.style.width = 75;
        dropdown.style.unityTextAlign = TextAnchor.MiddleRight;

        dropdown.RegisterValueChangedCallback(evt =>
        {
            onValueChanged?.Invoke(evt.newValue);
        });

        dropdownContainer.Add(label);
        dropdownContainer.Add(dropdown);       

        return dropdownContainer;
    }

    public void RefreshNode()
    {
        RefreshExpandedState();
        RefreshPorts();
        ToggleCollapse();
        ToggleCollapse();
    }

    #region INodeData
    public virtual void SetValues(NodeData data)
    {

    }

    public virtual object GetNodeData()
    {
        return new NodeData
        {
            Id = Id,
            Position = GetPosition().position,
            DataType = new SerializableSystemType(typeof(NodeData)),
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames()
        };
    }

    public virtual object DeserializeJson(string data)
    {
        var jsonData = GetNodeData();
        if(jsonData is NodeData nodeData)
        {
            if (nodeData.DataType != null)
            {
                Debug.Log(nodeData.DataType.SystemType);
                MethodInfo method = typeof(JsonUtility).GetMethod("FromJson", new[] { typeof(string) });
                MethodInfo genericMethod = method.MakeGenericMethod(nodeData.DataType.SystemType);
                return genericMethod.Invoke(null, new object[] { data });
            }
            else
            {
                Debug.LogError("Failed to find appropriate data type for deserialization.");
                return null;
            }
        }
        return null;
    }
    #endregion
}
#endif