using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Runtime class that orderly triggers all of the nodes within an Abilities' Effect.
/// In the Activate() method in the Ability class, whenever a player presses an ability,
/// a new instance of this class is created, and execution starts by calling StartExecution().
/// Remember to call SubscribeToEventNodes() whenever a player is created, or in any Start() method of your liking.
/// 
/// The node traversing algorithm is a recursive graph algorithm that:
/// 1) Starts from the root node
/// 2) Looks for any connections to the output port of the current node. If there is one, it travels to the next node.
/// 3) It looks for all of its input port values. If the nodes connected as inputs haven't been executed, it will execute them.
/// If they have already been executed, it will get their stored values from a dictionary.
/// 4) With all of the necessary information (input values), it will execute the current node, store its result in the dictionary
/// and mark it as executed in the hash set.
/// 5) Repeat this process until no nodes are left.
/// 
/// Executing a node depends on its runtime type; the algorithm uses System.Threading.Tasks to account for returned objects
/// of type Task (State nodes).
/// 
/// WARNING: Be aware that this is by no means the most elegant approach to do this. While functional, if executing
/// hundreds of effects at once, it is notably slower than manually coded solutions, as it adds a
/// considerable amount of GC Alloc and Time ms to the frame of execution. If optimization is a priority,
/// the traversal algorithm should be revisited with strong graph theory and multithreading knowledge.
/// Anyways, it is a very viable approach if hundreds of abilities aren't triggered at once,
/// and should be a fitting solution for many single player projects.
/// </summary>

public class ExecuteEffect
{
    private Effect effect;
    private HashSet<string> executedNodes = new HashSet<string>();
    private Dictionary<string, object> executedNodeResults = new Dictionary<string, object>();

    public Effect Effect { get { return effect; } set { effect = value; } }

    public void SubscribeToEventNodes()
    {
        var eventNodes = effect.Nodes.Where(node => node.RuntimeType.SystemType.IsSubclassOf(typeof(EventNode)));
        foreach (var eventNode in eventNodes)
        {
            TraverseAndExecute(eventNode.Id);
        }
    }

    public void StartExecution()
    {
        if (effect != null)
        {
            string rootNodeId = effect.Nodes.FirstOrDefault(node => node.RuntimeType.SystemType == typeof(RuntimeRootNode))?.Id;
            if (string.IsNullOrEmpty(rootNodeId)) return;
            TraverseAndExecute(rootNodeId);
        }
    }

    private async void TraverseAndExecute(string currentNodeId)
    {
        // Hash set
        if (executedNodes.Contains(currentNodeId)) return;

        // Execute node
        NodeData nodeData = effect.Nodes.FirstOrDefault(node => node.Id == currentNodeId);
        if (nodeData == null)
        {
            Debug.LogError("Node not found!");
            return;
        }

        var result = ExecuteNode(nodeData);
        executedNodes.Add(currentNodeId);
        executedNodeResults[currentNodeId] = result;

        // If a condition node was executed, check what node to trigger next
        if (result is Trigger triggerResult)
        {
            string triggerName = triggerResult.Name;
            var outgoingLinks = effect.Links.Where(link => link.BaseId == currentNodeId && link.TargetPortName == "In" && link.BasePortName == triggerName);
            foreach (var link in outgoingLinks)
                TraverseAndExecute(link.TargetId);
        }
        else if (result is Task<object> taskResult)
        {
            var outgoingLinks = effect.Links.Where(link => link.BaseId == currentNodeId && link.TargetPortName == "In");
            foreach (var link in outgoingLinks) // TODO: FIX BUG "InvalidOperationException: Collection was modified; enumeration operation may not execute" when changing var in real time
            {
                await taskResult;
                TraverseAndExecute(link.TargetId);
            }
        }
        else
        {
            var outgoingLinks = effect.Links.Where(link => link.BaseId == currentNodeId && link.TargetPortName == "In");
            foreach (var link in outgoingLinks)
                TraverseAndExecute(link.TargetId);
        }
    }

    private object ExecuteNode(NodeData nodeData)
    {
        if (nodeData != null && typeof(RuntimeNode).IsAssignableFrom(nodeData.RuntimeType.SystemType))
        {
            RuntimeNode node = (RuntimeNode)Activator.CreateInstance(nodeData.RuntimeType.SystemType);

            if (node is INodeData iNode)
            {
                iNode.SetValues(nodeData);
            }

            if (node is RuntimeVariableNode varNode)
            {
                // Set varName before accessing it
                varNode.Action(null, nodeData);
                InstanceVariable instanceVariable = effect.InstanceVariables.FirstOrDefault(var => var.Name == varNode.VarName);
                if (instanceVariable != null)
                {
                    switch (instanceVariable)
                    {
                        case InstanceVariableInt intVariable:
                            return intVariable.Value; // TODO: CHECK IF THIS IS A BETTER SOLUTION THAN CAST NODES
                        case InstanceVariableFloat floatVariable:
                            return floatVariable.Value;
                        case InstanceVariableString stringVariable:
                            return stringVariable.Value;
                        case InstanceVariableBool boolVariable:
                            return boolVariable.Value;
                        case InstanceVariableVector2 vectorVariable:
                            return vectorVariable.Value;
                        case InstanceVariableVector3 vectorVariable:
                            return vectorVariable.Value;
                        case InstanceVariableVector4 vectorVariable:
                            return vectorVariable.Value;
                        case InstanceVariableGameObject gameObjectVariable:
                            return gameObjectVariable.Value;
                    }
                }
            }

            if (node is RuntimeSetVarNode setVarNode)
            {
                setVarNode.Effect = Effect;
            }

            if (node is ActionNode actionNode || node is ConditionNode conditionNode || node is StateNode stateNode || node is EventNode eventNode)
            {
                List<object> objects = new List<object>();
                
                // Sorted inputports
                List<string> inputPortDisplayNames = nodeData.InputPortNames.ToList();

                foreach (string displayName in inputPortDisplayNames)
                {
                    var link = effect.Links.FirstOrDefault(link => link.TargetId == nodeData.Id && link.TargetPortName == displayName);

                    // If a inputport isn't connected, add a null object to the list so that we know the exact order of the results when executing nodes
                    if (link == null) 
                    {
                        objects.Add(null);
                        continue;
                    }

                    NodeData connectedNodeData = effect.Nodes.FirstOrDefault(n => n.Id == link.BaseId);
                    if (connectedNodeData == null || connectedNodeData.RuntimeType.SystemType == typeof(RuntimeRootNode)) continue;

                    // If result is a list, add the values of the list instead of the list itself
                    if (executedNodeResults.TryGetValue(connectedNodeData.Id, out object result) && result is List<object> listResult && link.TargetPortName != "In")
                    {
                        // Get all output ports connected to this node
                        var connectedNodeOutputPortNames = effect.Links
                            .Where(l => l.BaseId == connectedNodeData.Id && l.TargetId == nodeData.Id && l.BasePortName != "Out")
                            .Select(l => l.BasePortName)
                            .ToList();

                        NodeData connectedNode = effect.Nodes.FirstOrDefault(n => n.Id == connectedNodeData.Id);
                        if (connectedNode == null) continue;

                        // Sorted output ports from the node we are connected to
                        List<string> outputPortDisplayNames = connectedNode.OutputPortNames.ToList();

                        // Initialize the connected port indices array with -1
                        int[] connectedPortIndices = new int[connectedNodeOutputPortNames.Count];
                        for (int i = 0; i < connectedPortIndices.Length; i++)
                        {
                            connectedPortIndices[i] = -1;
                        }

                        // Compare the list with all the output ports and the ones that are actually connected, and get indices of the ones that are connected
                        for (int i = 0; i < connectedNodeOutputPortNames.Count; i++)
                        {
                            string portName = connectedNodeOutputPortNames[i];
                            int indexInOutputPorts = outputPortDisplayNames.IndexOf(portName);
                            if (indexInOutputPorts != -1)
                            {
                                connectedPortIndices[i] = indexInOutputPorts;
                            }
                        }

                        // Add the values from the list with the correct index
                        foreach (var portNum in connectedPortIndices)
                        {
                            if (portNum != -1)
                            {
                                objects.Add(listResult[portNum]);
                            }
                        }
                    }
                    else
                    {
                        objects.Add(ExecuteNode(connectedNodeData));
                    }
                }

                if (node is ActionNode)
                {
                    return ((ActionNode)node).Action(objects, nodeData);
                }
                else if (node is ConditionNode)
                {
                    return ((ConditionNode)node).Condition(objects, nodeData);
                }
                else if (node is StateNode)
                {
                    return ((StateNode)node).State(objects, nodeData);
                }
                else if (node is EventNode)
                {
                    ((EventNode)node).Subscribe(objects, nodeData);
                    return null;
                }
            }
        }
        return null;
    }
}