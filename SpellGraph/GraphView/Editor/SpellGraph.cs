using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Linq;

/// <summary>
/// Base class of the whole SpellGraph tool. It inherits and implements the GraphView API.
/// Within the General region, the graph view is initialized on top of the SpellEditorWindow EditorWindow.
/// This class contains general functionalities like setting up the node library search window,
/// node copy/pasting, edge port rules, node deletion...
/// The blackboard from the GraphView API is also implemented, which allows for the creation
/// of instance variables, with drag/drop and automatic renaming capabilities via callbacks.
/// This setup is inspired by Mert Kirimgeri's amazing tutorial series.
/// See: https://www.youtube.com/watch?v=7KHGH0fPL84
/// </summary>

public class SpellGraph : GraphView
{
    private NodeLibrarySearchTree nodeLibrary;
    public Blackboard Blackboard = new Blackboard();
    private BlackboardSection varSection = new BlackboardSection() { title = "Instance Variables" };
    private string serializedGraphElementsData;
    public List<InstanceVariable> InstanceVariables = new List<InstanceVariable>();   
    public static string PropertyName;
    public static Type PropertyType;

    #region General
    public SpellGraph(SpellEditorWindow editorWindow)
    {
        styleSheets.Add(Resources.Load<StyleSheet>("SpellEditorStyle"));
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        AddElement(new RootNode());
        SetNodeLibrary(editorWindow);
        OnElementsDeleted();

        // Copy and paste
        OnCopy();
        AllowPaste();
        OnPaste();
    }

    private void SetNodeLibrary(SpellEditorWindow editorWindow)
    {
        nodeLibrary = ScriptableObject.CreateInstance<NodeLibrarySearchTree>();
        nodeLibrary.InitializeLibrary(editorWindow, this);
        nodeCreationRequest = context =>
            SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), nodeLibrary);
    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        List<Port> compatiblePorts = new List<Port>();
        Type startPortType = startPort.portType;

        ports.ForEach(port =>
        {
            // Check if the startPort and the current port are different and belong to different nodes
            if (startPort != port && startPort.node != port.node)
            {
                Type portType = port.portType;

                // Check if the port has a type and if it matches the startPort's type or satisfies CanConnectTypes
                if (portType != null && CanConnectTypes(startPortType, portType, startPort.direction, port.direction))
                {
                    // Don't give the option to connect to outputports
                    if (port.direction == Direction.Output)
                    {
                        return;
                    }
                    compatiblePorts.Add(port);
                }
            }
        });

        return compatiblePorts;
    }

    private bool CanConnectTypes(Type startPortType, Type portType, Direction startPortDirection, Direction portDirection)
    {
        // Check if the types are the same and the directions are the same
        if (startPortType == portType && startPortDirection == portDirection)
            return false;

        // Allow generics to connect with all other types except triggers
        if (startPortType != typeof(Trigger) && portType == typeof(Generic))
            return true;

        // Check if there's an implicit conversion between float and int
        if ((startPortType == typeof(float) && portType == typeof(int)) ||
            (startPortType == typeof(int) && portType == typeof(float)))
            return true;

        // Check if the types are explicitly allowed to connect
        if ((startPortType == typeof(Trigger) && portType == typeof(Trigger)) ||
            (startPortType == typeof(string) && portType == typeof(string)) ||
            (startPortType == typeof(float) && portType == typeof(float)) ||
            (startPortType == typeof(int) && portType == typeof(int)) ||
            (startPortType == typeof(bool) && portType == typeof(bool)) ||
            (startPortType == typeof(Vector2) && portType == typeof(Vector2)) ||
            (startPortType == typeof(Vector3) && portType == typeof(Vector3)) ||
            (startPortType == typeof(Vector4) && portType == typeof(Vector4)) ||
            (startPortType == typeof(GameObject) && portType == typeof(GameObject)))
            return true;

        return false;
    }

    private void OnElementsDeleted()
    {
        deleteSelection = (operationName, askUser) =>
        {
            Type groupType = typeof(NodeGroup);
            Type edgeType = typeof(Edge);

            List<NodeGroup> groupsToDelete = new List<NodeGroup>();
            List<ExtendedNode> nodesToDelete = new List<ExtendedNode>();
            List<Edge> edgesToDelete = new List<Edge>();

            foreach (GraphElement selectedElement in selection)
            {
                if (selectedElement is ExtendedNode node)
                {
                    nodesToDelete.Add(node);
                    continue;
                }

                if (selectedElement.GetType() == edgeType)
                {
                    Edge edge = (Edge)selectedElement;
                    edgesToDelete.Add(edge);
                    continue;
                }

                if (selectedElement.GetType() != groupType)
                {
                    continue;
                }

                NodeGroup group = (NodeGroup)selectedElement;

                groupsToDelete.Add(group);
            }

            foreach (NodeGroup groupToDelete in groupsToDelete)
            {
                List<ExtendedNode> groupNodes = new List<ExtendedNode>();

                foreach (GraphElement groupElement in groupToDelete.containedElements)
                {
                    if (!(groupElement is ExtendedNode))
                    {
                        continue;
                    }

                    ExtendedNode groupNode = (ExtendedNode)groupElement;

                    groupNodes.Add(groupNode);
                }

                groupToDelete.RemoveElements(groupNodes);

                //RemoveGroup(groupToDelete);
                RemoveElement(groupToDelete);
            }

            DeleteElements(edgesToDelete);

            foreach (ExtendedNode nodeToDelete in nodesToDelete)
            {
                nodeToDelete.DisconnectPorts(this);
                RemoveElement(nodeToDelete);
            }
        };
    }
    #endregion

    #region Copy/Paste
    private string OnCopy()
    {
        serializeGraphElements = elements =>
        {
            GraphDataJson graphData = new GraphDataJson();
            Dictionary<string, string> idMapping = new Dictionary<string, string>();

            foreach (var element in elements)
            {
                if (element is INodeData iNodeDataHandler)
                {
                    string newId = Guid.NewGuid().ToString();
                    var nodeData = (NodeData)iNodeDataHandler.GetNodeData();
                    idMapping[nodeData.Id] = newId;
                    nodeData.Id = newId;
                    string serializedNode = JsonUtility.ToJson(nodeData);
                    graphData.nodeDataJson.serializedNodes.Add(serializedNode);
                }
                else if (element is ExtendedNode node)
                {
                    string newId = Guid.NewGuid().ToString();
                    Type dataType = null;
                    if(node.GetNodeData() is NodeData nData)
                        dataType = nData.DataType.SystemType;
                    NodeData nodeData = new NodeData
                    {
                        Id = newId,
                        Position = element.GetPosition().position,
                        DataType = new SerializableSystemType(dataType),
                        EditorType = new SerializableSystemType(element.GetType()),
                        RuntimeType = new SerializableSystemType(node.GetRuntimeType()),
                        InputPortNames = node.GetInputPortNames(),
                        OutputPortNames = node.GetOutputPortNames(),
                    };
                    idMapping[node.Id] = newId;
                    string serializedNode = JsonUtility.ToJson(nodeData);
                    graphData.nodeDataJson.serializedNodes.Add(serializedNode);
                }
            }
            foreach (var element in elements)
            {
                if (element is Edge edge)
                {
                    var outputNode = edge.output.node as ExtendedNode;
                    var inputNode = edge.input.node as ExtendedNode;

                    if (outputNode != null && inputNode != null)
                    {
                        string baseId = idMapping.ContainsKey(outputNode.Id) ? idMapping[outputNode.Id] : outputNode.Id;
                        string targetId = idMapping.ContainsKey(inputNode.Id) ? idMapping[inputNode.Id] : inputNode.Id;

                        NodeLinkData edgeData = new NodeLinkData
                        {
                            BaseId = baseId,
                            TargetId = targetId,
                            BasePortName = edge.output.portName,
                            TargetPortName = edge.input.portName
                        };

                        string serializedEdge = JsonUtility.ToJson(edgeData);
                        graphData.nodeLinkDataJson.serializedEdges.Add(serializedEdge);
                    }
                }
            }
            serializedGraphElementsData = JsonUtility.ToJson(graphData);
            return serializedGraphElementsData;
        };
        return serializedGraphElementsData;
    }

    private bool AllowPaste()
    {
        canPasteSerializedData = (data) =>
        {
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    var parsedObject = JsonUtility.FromJson<GraphDataJson>(data);
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        };
        return false;
    }

    private void OnPaste()
    {
        unserializeAndPaste = (operationName, data) =>
        {
            if (!string.IsNullOrEmpty(data))
            {
                var graphData = JsonUtility.FromJson<GraphDataJson>(data);

                if (graphData != null)
                {
                    var nodes = new List<ExtendedNode>();
                    var edges = new List<Edge>();

                    // Create nodes from serialized data
                    foreach (var serializedNode in graphData.nodeDataJson.serializedNodes)
                    {
                        var concreteNode = JsonUtility.FromJson<NodeData>(serializedNode);

                        if (concreteNode != null)
                        {
                            Type nodeType = concreteNode.EditorType.SystemType;

                            if (nodeType != null && typeof(ExtendedNode).IsAssignableFrom(nodeType))
                            {
                                ExtendedNode node = (ExtendedNode)Activator.CreateInstance(nodeType);
                                AddElement(node);
                                node.SetPosition(new Rect(concreteNode.Position.x, concreteNode.Position.y, 100, 50));
                                node.Id = concreteNode.Id;

                                if (node is INodeData iNodeDataHandler)
                                    iNodeDataHandler.SetValues((NodeData)iNodeDataHandler.DeserializeJson(serializedNode));

                                node.RefreshNode();
                                nodes.Add(node);
                            }
                        }
                    }

                    // Create edges between nodes
                    foreach (var serializedEdge in graphData.nodeLinkDataJson.serializedEdges)
                    {
                        var edgeData = JsonUtility.FromJson<NodeLinkData>(serializedEdge);

                        if (edgeData != null)
                        {
                            var sourceNode = nodes.FirstOrDefault(x => x.Id == edgeData.BaseId);
                            var targetNode = nodes.FirstOrDefault(x => x.Id == edgeData.TargetId);

                            if (sourceNode != null && targetNode != null)
                            {
                                Port outputPort = null;
                                Port inputPort = null;

                                // Handle VarNode ports differently
                                if (sourceNode is VariableNode variableSourceNode)
                                {
                                    foreach (var port in variableSourceNode.titleContainer.Children())
                                    {
                                        if (port is Port p && p.portName == edgeData.BasePortName)
                                        {
                                            outputPort = p;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    outputPort = sourceNode.outputContainer.Children().OfType<Port>().FirstOrDefault(p => p.portName == edgeData.BasePortName);
                                }

                                inputPort = targetNode.inputContainer.Children().OfType<Port>().FirstOrDefault(p => p.portName == edgeData.TargetPortName);

                                // Link nodes
                                if (outputPort != null && inputPort != null)
                                {
                                    var tempEdge = new Edge()
                                    {
                                        output = outputPort,
                                        input = inputPort
                                    };

                                    tempEdge.input.Connect(tempEdge);
                                    tempEdge.output.Connect(tempEdge);
                                    Add(tempEdge);
                                    edges.Add(tempEdge);
                                }
                            }
                        }
                    }

                    // Add newly pasted elements to the current selection
                    ClearSelection();
                    foreach (var node in nodes)
                        AddToSelection(node);
                    foreach (var edge in edges)
                        AddToSelection(edge);
                }
            }
        };
    }
    #endregion

    #region Blackboard
    public void ShowBlackboardMenu()
    {
        var menu = new GenericMenu();
        menu.AddItem(new GUIContent("Int"), false, () => AddPropertyToBlackBoard(new InstanceVariableInt { Name = "Int", Value = 0 }));
        menu.AddItem(new GUIContent("Float"), false, () => AddPropertyToBlackBoard(new InstanceVariableFloat { Name = "Float", Value = 0f }));
        menu.AddItem(new GUIContent("String"), false, () => AddPropertyToBlackBoard(new InstanceVariableString { Name = "String", Value = "" }));
        menu.AddItem(new GUIContent("Bool"), false, () => AddPropertyToBlackBoard(new InstanceVariableBool { Name = "Bool", Value = false }));
        menu.AddItem(new GUIContent("Vector2"), false, () => AddPropertyToBlackBoard(new InstanceVariableVector2 { Name = "Vector2", Value = Vector2.zero }));
        menu.AddItem(new GUIContent("Vector3"), false, () => AddPropertyToBlackBoard(new InstanceVariableVector3 { Name = "Vector3", Value = Vector3.zero }));
        menu.AddItem(new GUIContent("Vector4"), false, () => AddPropertyToBlackBoard(new InstanceVariableVector4 { Name = "Vector4", Value = Vector4.zero }));
        menu.AddItem(new GUIContent("GameObject"), false, () => AddPropertyToBlackBoard(new InstanceVariableGameObject { Name = "GameObject", Value = new GameObject() }));
        menu.ShowAsContext();
    }

    public void ClearBlackboard()
    {
        InstanceVariables.Clear();
        Blackboard.Clear();
        varSection.Clear();
    }

    public void AddPropertyToBlackBoard(InstanceVariable variable)
    {
        // Make sure variable name can't be repeated
        String localPropertyName = variable.Name;
        while (InstanceVariables.Any(x => x.Name == localPropertyName))
        {
            localPropertyName = $"{localPropertyName}(1)";
        }
        variable.Name = localPropertyName;

        // Create new field and row
        BlackboardField blackBoardField = new BlackboardField { text = variable.Name, /*typeText = variable.Name*/ };
        blackBoardField.capabilities &= ~Capabilities.Deletable;
        BlackboardRow blackBoardRow = null;
        bool mouseIsOver = false;


        varSection.contentContainer.Add(blackBoardField);

        // Callbacks for drag/drop, delete, rename
        blackBoardField.RegisterCallback<MouseDownEvent>(evt =>
        {
            PropertyName = blackBoardField.text;
            PropertyType = variable.GetType();

            if (PropertyType == typeof(InstanceVariableInt))
            {
                PropertyType = typeof(int);
            }
            else if (PropertyType == typeof(InstanceVariableFloat))
            {
                PropertyType = typeof(float);
            }
            else if (PropertyType == typeof(InstanceVariableString))
            {
                PropertyType = typeof(string);
            }
            else if (PropertyType == typeof(InstanceVariableBool))
            {
                PropertyType = typeof(bool);
            }
            else if (PropertyType == typeof(InstanceVariableVector2))
            {
                PropertyType = typeof(Vector2);
            }
            else if (PropertyType == typeof(InstanceVariableVector3))
            {
                PropertyType = typeof(Vector3);
            }
            else if (PropertyType == typeof(InstanceVariableVector4))
            {
                PropertyType = typeof(Vector4);
            }
            else if (PropertyType == typeof(InstanceVariableGameObject))
            {
                PropertyType = typeof(GameObject);
            }
        });
        blackBoardField.RegisterCallback<MouseEnterEvent>(evt =>
        {
            blackBoardField.focusable = true;
            blackBoardField.Focus();
            mouseIsOver = true;
        });
        blackBoardField.RegisterCallback<MouseLeaveEvent>(evt =>
        {
            mouseIsOver = false;
        });
        blackBoardField.RegisterCallback<KeyDownEvent>(evt =>
        {
            if (evt.keyCode == KeyCode.Delete || evt.keyCode == KeyCode.Backspace)
            {
                if (blackBoardRow != null && (blackBoardRow.parent == varSection))
                {
                    if (blackBoardField.selected && mouseIsOver)
                    {
                        if (varSection.Contains(blackBoardRow))
                            varSection.Remove(blackBoardRow);

                        InstanceVariables.Remove(variable);

                        // Search for the associated VariableNode and remove it
                        string propertyNameToDelete = blackBoardField.text;

                        foreach (var node in nodes.ToList())
                        {
                            if (node is VariableNode variableNode)
                            {
                                if (variableNode.VarName == propertyNameToDelete)
                                {
                                    var port = variableNode.titleContainer.Q<Port>();
                                    if (port != null)
                                    {
                                        // Remove connected edges
                                        var edges = port.connections.ToList();
                                        foreach (var edge in edges)
                                        {
                                            edge.input.Disconnect(edge);
                                            edge.output.Disconnect(edge);
                                            RemoveElement(edge);
                                        }
                                        RemoveElement(variableNode);
                                        SetVarNode.UpdateDropdownOptions(InstanceVariables.Select(variable => variable.Name).ToArray(), InstanceVariables.Select(variable => variable.GetType()).ToArray());
                                    }
                                }
                            }
                        }
                    }
                }
            }
        });
        blackBoardField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            string newFieldName = evt.newValue;
            string oldFieldName = evt.previousValue;
            foreach (var node in nodes.ToList())
            {
                if (node is VariableNode variableNode)
                {
                    var port = node.titleContainer.Q<Port>();
                    if (port != null && port.portName == oldFieldName)
                    {
                        port.portName = newFieldName;
                        var nodeData = new VarNodeData() { VarName = newFieldName, VarType = new SerializableSystemType(variableNode.VarType) };
                        variableNode.SetValues(nodeData);
                        SetVarNode.UpdateDropdownOptions(InstanceVariables.Select(variable => variable.Name).ToArray(), InstanceVariables.Select(variable => variable.GetType()).ToArray());
                    }
                }
            }
        });
        
        // Row value fields
        #region Value Fields
        if (variable is InstanceVariableInt intVariable)
        {
            blackBoardField.typeText = "Int";

            var propertyValueTextField = new IntegerField()
            {
                value = intVariable.Value
            };

            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                intVariable.Value = evt.newValue;          
            });

            blackBoardRow = new BlackboardRow(blackBoardField, propertyValueTextField);
            varSection.Add(blackBoardRow);
        }
        else if (variable is InstanceVariableFloat floatVariable)
        {
            blackBoardField.typeText = "Float";
            
            var propertyValueTextField = new FloatField()
            {
                value = floatVariable.Value
            };

            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                floatVariable.Value = evt.newValue;
            });

            blackBoardRow = new BlackboardRow(blackBoardField, propertyValueTextField);
            varSection.Add(blackBoardRow);
        }
        else if (variable is InstanceVariableString stringVariable)
        {
            blackBoardField.typeText = "String";
            
            var propertyValueTextField = new TextField()
            {
                value = stringVariable.Value
            };

            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                stringVariable.Value = evt.newValue;
            });

            blackBoardRow = new BlackboardRow(blackBoardField, propertyValueTextField);
            varSection.Add(blackBoardRow);
        }
        else if (variable is InstanceVariableBool boolVariable)
        {
            blackBoardField.typeText = "Bool";
            
            var propertyValueTextField = new Toggle()
            {
                value = boolVariable.Value
            };

            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                boolVariable.Value = evt.newValue;             
            });

            blackBoardRow = new BlackboardRow(blackBoardField, propertyValueTextField);
            varSection.Add(blackBoardRow);
        }
        else if (variable is InstanceVariableVector2 vector2Variable)
        {
            blackBoardField.typeText = "Vector2";
            
            var propertyValueTextField = new Vector2Field()
            {
                value = vector2Variable.Value
            };

            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                vector2Variable.Value = evt.newValue;
            });

            blackBoardRow = new BlackboardRow(blackBoardField, propertyValueTextField);
            varSection.Add(blackBoardRow);
        }
        else if (variable is InstanceVariableVector3 vector3Variable)
        {
            blackBoardField.typeText = "Vector3";
            
            var propertyValueTextField = new Vector3Field()
            {
                value = vector3Variable.Value
            };

            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                vector3Variable.Value = evt.newValue;
            });

            blackBoardRow = new BlackboardRow(blackBoardField, propertyValueTextField);
            varSection.Add(blackBoardRow);
        }
        else if (variable is InstanceVariableVector4 vector4Variable)
        {
            blackBoardField.typeText = "Vector4";
            
            var propertyValueTextField = new Vector4Field()
            {
                value = vector4Variable.Value
            };

            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                vector4Variable.Value = evt.newValue;
            });

            blackBoardRow = new BlackboardRow(blackBoardField, propertyValueTextField);
            varSection.Add(blackBoardRow);
        }
        else if (variable is InstanceVariableGameObject gameObjectVariable)
        {
            blackBoardField.typeText = "GameObject";

            var propertyValueObjectField = new UnityEditor.UIElements.ObjectField()
            {
                objectType = typeof(GameObject),
                allowSceneObjects = true,
                value = gameObjectVariable.Value
            };

            propertyValueObjectField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue is GameObject newGameObject)
                {
                    gameObjectVariable.Value = newGameObject;
                }
            });

            blackBoardRow = new BlackboardRow(blackBoardField, propertyValueObjectField);
            varSection.Add(blackBoardRow);
        }
        #endregion

        Blackboard.Add(varSection);
        InstanceVariables.Add(variable);

        // Update SetVarNode dropdowns with the new instance variable
        SetVarNode.UpdateDropdownOptions(InstanceVariables.Select(variable => variable.Name).ToArray(), InstanceVariables.Select(variable => variable.GetType()).ToArray());

    }
    #endregion
}