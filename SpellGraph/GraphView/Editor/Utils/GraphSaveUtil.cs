using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;

/// <summary>
/// Utility class that adds save/load capabilities to the SpellGraph class, through the toolbar buttons
/// present in the SpellEditorWindow class. Essentially it serializes nodes, blackboard instance
/// variables and the edges connecting each node into a scriptable object, or loads them into the
/// existing editor window.
/// This setup is inspired by Mert Kirimgeri's amazing tutorial series.
/// See: https://www.youtube.com/watch?v=7KHGH0fPL84
/// </summary>

public class GraphSaveUtil
{
    private SpellGraph targetGraphView;
    private Effect cachedEffect;
    [SerializeField] private string effectsPath = "Assets/SpellGraph/Examples/Effects/";
    private List<Edge> Edges => targetGraphView.edges.ToList();
    private List<ExtendedNode> Nodes => targetGraphView.nodes.OfType<ExtendedNode>().ToList();
    private List<InstanceVariable> Properties => targetGraphView.InstanceVariables.ToList();
    private List<NodeGroup> Groups => targetGraphView.graphElements.ToList().Where(x => x is NodeGroup).Cast<NodeGroup>().ToList();

    private static readonly Dictionary<Type, Func<InstanceVariable, InstanceVariable>> instanceVariableTypeMapping = new Dictionary<Type, Func<InstanceVariable, InstanceVariable>>
    {
        { typeof(InstanceVariableInt), var => new InstanceVariableInt { Name = var.Name, Value = ((InstanceVariableInt)var).Value } },
        { typeof(InstanceVariableFloat), var => new InstanceVariableFloat { Name = var.Name, Value = ((InstanceVariableFloat)var).Value } },
        { typeof(InstanceVariableString), var => new InstanceVariableString { Name = var.Name, Value = ((InstanceVariableString)var).Value } },
        { typeof(InstanceVariableBool), var => new InstanceVariableBool { Name = var.Name, Value = ((InstanceVariableBool)var).Value } },
        { typeof(InstanceVariableVector2), var => new InstanceVariableVector2 { Name = var.Name, Value = ((InstanceVariableVector2)var).Value } },
        { typeof(InstanceVariableVector3), var => new InstanceVariableVector3 { Name = var.Name, Value = ((InstanceVariableVector3)var).Value } },
        { typeof(InstanceVariableVector4), var => new InstanceVariableVector4 { Name = var.Name, Value = ((InstanceVariableVector4)var).Value } },
        { typeof(InstanceVariableGameObject), var => new InstanceVariableGameObject { Name = var.Name, Value = ((InstanceVariableGameObject)var).Value } },
    };

    public static GraphSaveUtil GetInstance(SpellGraph graphView)
    {
        return new GraphSaveUtil { targetGraphView = graphView };
    }

    #region Saving
    public void SaveGraph(string fileName)
    {
        if (!Edges.Any()) return;

        string filePath = $"{effectsPath}{fileName}.asset";

        if (!AssetDatabase.IsValidFolder(effectsPath))
            AssetDatabase.CreateFolder("Assets/SpellGraph/Examples", "Effects");

        Effect effect = AssetDatabase.LoadAssetAtPath<Effect>(filePath);
        if (effect == null)
        {
            effect = ScriptableObject.CreateInstance<Effect>();
            AssetDatabase.CreateAsset(effect, filePath);
        }
        else
        {
            EditorUtility.CopySerializedIfDifferent(effect, ScriptableObject.CreateInstance<Effect>());
        }

        effect.InstanceVariables.Clear();
        effect.Groups.Clear();
        effect.Nodes.Clear();
        effect.Links.Clear();

        SaveNodes(effect);
        SaveInstanceVariables(effect);
        SaveGroups(effect);

        // Mark the asset and instance as dirty to ensure changes are saved
        EditorUtility.SetDirty(effect);
        AssetDatabase.SaveAssets();
    }

    private void SaveNodes(Effect effect)
    {
        var connectedPorts = Edges.Where(edge => edge.input.node != null && edge.output.node != null).ToArray();
        effect.Links = new List<NodeLinkData>();

        for (int i = 0; i < connectedPorts.Length; i++)
        {
            var outputNode = connectedPorts[i].output.node as ExtendedNode;
            var inputNode = connectedPorts[i].input.node as ExtendedNode;

            // Check if either the outputNode or inputNode is null before adding data.
            if (outputNode != null && inputNode != null)
            {
                effect.Links.Add(new NodeLinkData
                {
                    BaseId = outputNode.Id,
                    TargetId = inputNode.Id,
                    BasePortName = connectedPorts[i].output.portName,
                    TargetPortName = connectedPorts[i].input.portName
                });
            }
        }

        foreach (var node in Nodes)
        {
            if (node is INodeData iNodeDataHandler)
                effect.Nodes.Add((NodeData)iNodeDataHandler.GetNodeData());
            else
            {
                effect.Nodes.Add(new NodeData
                {
                    Id = node.Id,
                    Position = node.GetPosition().position,
                    EditorType = new SerializableSystemType(node.GetType()),
                });
            }                    
        }
    }

    private void SaveInstanceVariables(Effect effect)
    {
        foreach (var property in Properties)
        {
            Type propType = property.GetType();
            if (instanceVariableTypeMapping.TryGetValue(propType, out var createInstanceVariable))
            {
                effect.InstanceVariables.Add(createInstanceVariable(property));
            }
        }
    }

    private void SaveGroups(Effect effect)
    {
        foreach (var group in Groups)
        {
            var nodes = group.containedElements.Where(x => x is ExtendedNode).Cast<ExtendedNode>().Select(x => x.Id)
                .ToList();

            effect.Groups.Add(new GroupData
            {
                Nodes = nodes,
                Title = group.title,
                Position = group.GetPosition().position
            });
        }
    }
    #endregion

    #region Loading
    public void LoadGraph(string fileName)
    {
        cachedEffect = AssetDatabase.LoadAssetAtPath<Effect>($"{effectsPath}{fileName}.asset");
        if (cachedEffect == null)
        {
            EditorUtility.DisplayDialog("File not found", "Effect doesn't exist", "Ok");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
        SetInstanceVariables();
        SetGroups();
    }

    public void ClearGraph()
    {
        foreach (var node in Nodes)
        {
            targetGraphView.RemoveElement(node);
        }
        foreach (var edge in Edges)
        {
            targetGraphView.RemoveElement(edge);
        }

        targetGraphView.ClearBlackboard();     
        Nodes.Clear();
        Edges.Clear();
        Properties.Clear();
        Groups.Clear();
    }

    public void CreateNodes()
    {
        foreach (var nodeData in cachedEffect.Nodes)
        {
            if (nodeData is NodeData nodeDataConcrete)
            {
                Type nodeType = nodeDataConcrete.EditorType.SystemType;

                if (nodeType != null && typeof(ExtendedNode).IsAssignableFrom(nodeType))
                {
                    ExtendedNode node = (ExtendedNode)Activator.CreateInstance(nodeType);
                    node.RefreshNode();
                    targetGraphView.AddElement(node);
                    node.SetPosition(new Rect(nodeDataConcrete.Position.x, nodeDataConcrete.Position.y, 100, 50));
                    node.Id = nodeDataConcrete.Id;

                    if (node is INodeData iNodeDataHandler)
                        iNodeDataHandler.SetValues(nodeData);

                    Nodes.Add(node);
                }
                else
                {
                    Debug.LogWarning($"Node type '{nodeType?.ToString()}' not found or doesn't inherit from ExtendedNode.");
                }
            }
        }
    }

    public void ConnectNodes()
    {
        foreach (var node in Nodes)
        {
            var connections = cachedEffect.Links.Where(x => x.BaseId == node.Id).ToList();
            foreach (var connection in connections)
            {
                var targetNodeGUID = connection.TargetId;
                var targetPortName = connection.TargetPortName;

                var targetNode = Nodes.First(x => x.Id == targetNodeGUID);

                Port outputPort = null;
                Port inputPort = null;

                // OutputPorts (titleContainer & normal container)
                if(node.GetType() == typeof(VariableNode))
                {
                    foreach (var port in node.titleContainer.Children())
                    {
                        if (port is Port p && p.portName == connection.BasePortName)
                        {
                            outputPort = p;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (var port in node.outputContainer.Children())
                    {
                        if (port is Port p && p.portName == connection.BasePortName)
                        {
                            outputPort = p;
                            break;
                        }
                    }
                }

                // InputPorts
                foreach (var port in targetNode.inputContainer.Children())
                {
                    if (port is Port p && p.portName == targetPortName)
                    {
                        inputPort = p;
                        break;
                    }
                }

                if (outputPort != null && inputPort != null)
                {
                    LinkNodes(outputPort, inputPort);
                }
            }
        }
    }

    private void LinkNodes(Port outputSocket, Port inputSocket)
    {
        var tempEdge = new Edge()
        {
            output = outputSocket,
            input = inputSocket
        };
        tempEdge?.input.Connect(tempEdge);
        tempEdge?.output.Connect(tempEdge);
        targetGraphView.Add(tempEdge);
    }

    private void SetInstanceVariables()
    {
        // Clone the instance variables from the cachedEffect
        foreach (var property in cachedEffect.InstanceVariables)
        {
            Type propType = property.GetType();
            if (instanceVariableTypeMapping.TryGetValue(propType, out var createInstanceVariable))
            {
                var clonedProperty = createInstanceVariable(property);
                targetGraphView.AddPropertyToBlackBoard(clonedProperty);
            }
        }
    }

    private void SetGroups()
    {
        foreach (var group in Groups)
        {
            targetGraphView.RemoveElement(group);
        }

        foreach (var groupData in cachedEffect.Groups)
        {
            var group = new NodeGroup();
            group.AddElements(Nodes.Where(x => groupData.Nodes.Contains(x.Id)));
            group.title = groupData.Title;
            targetGraphView.AddElement(group);
        }
    }
    #endregion
}