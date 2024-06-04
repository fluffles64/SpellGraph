using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Linq;

/// <summary>
/// This utility class is responisble for opening a small window with all of the available nodes
/// in the library. It displays each node inside color-coded categories, which are automatically
/// managed with the [NodeInfo()] custom attribute. The window is opened by pressing space
/// with the SpellGraph editor window open, and provides fuzzy finding. Clicking on one of the
/// options instantiates the actual node into the editor window.
/// </summary>

public class NodeLibrarySearchTree : ScriptableObject, ISearchWindowProvider
{
    private EditorWindow window;
    private SpellGraph graphView;

    public void InitializeLibrary(EditorWindow window, SpellGraph graphView)
    {
        this.window = window;
        this.graphView = graphView;
    }

    private Texture2D GetIcon(string category)
    {
        Color nodeColor = NodeColorUtil.GetColor(category);
        Texture2D icon = new Texture2D(1, 1);
        icon.SetPixel(0, 0, nodeColor);
        icon.Apply();
        return icon;
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        if (searchTreeEntry.userData is Node node)
        {
            Vector2 mousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent, context.screenMousePosition - window.position.position);
            Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);
            node.RefreshExpandedState();
            node.RefreshPorts();
            node.SetPosition(new Rect(graphMousePosition.x, graphMousePosition.y, 100, 150));
            graphView.AddElement(node);
            return true;
        }
        else if(searchTreeEntry.userData is NodeGroup group)
        {
            Vector2 mousePosition = window.rootVisualElement.ChangeCoordinatesTo(window.rootVisualElement.parent, context.screenMousePosition - window.position.position);
            Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(mousePosition);
            group.SetPosition(new Rect(graphMousePosition.x, graphMousePosition.y, 100, 150));
            graphView.AddElement(group);
            return true;
        }
        return false;
    }

    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry> { new SearchTreeGroupEntry(new GUIContent("Add Node"), 0) };
        int level = 0;

        var nodeTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(Node).IsAssignableFrom(p) || typeof(NodeGroup).IsAssignableFrom(p) && p.IsClass && !p.IsAbstract)
            .Select(type => (Type: type, Attributes: type.GetCustomAttributes(typeof(NodeInfoAttribute), false)))
            .Where(t => t.Attributes.Length > 0)
            .Select(t => (Type: t.Type, Attribute: (NodeInfoAttribute)t.Attributes[0]));

        foreach (var nodeType in nodeTypes)
        {
            var nodeInfo = nodeType.Attribute;

            // Check if the node has a libraryPath
            if (!string.IsNullOrEmpty(nodeInfo.libraryPath))
            {
                string[] categories = nodeInfo.libraryPath.Split('/');

                // Check if the main category has not been added before
                if (!tree.Any(entry => entry.content.text == categories[0]))
                {
                    // Add the main category as level 1
                    level = 1;
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(categories[0], GetIcon(categories[0])), level));
                }

                // If there are subfolders
                if(categories.Length > 1)
                {
                    for (int i = 1; i < categories.Length; i++)
                    {
                        if (!tree.Any(entry => entry.content.text == categories[i]))
                        {
                            level = i + 1;
                            tree.Add(new SearchTreeGroupEntry(new GUIContent(categories[i], GetIcon(categories[0])), level));
                        }
                    }

                    // Use title as name if available, otherwise use the last category
                    string nodeName = nodeInfo.title ?? categories.Last();
                    tree.Add(new SearchTreeEntry(new GUIContent(nodeName, GetIcon(categories[0]))) { level = level + 1, userData = Activator.CreateInstance(nodeType.Type) });
                }
                else
                {
                    string nodeName = nodeInfo.title ?? categories.Last();
                    tree.Add(new SearchTreeEntry(new GUIContent(nodeName, GetIcon(categories[0]))) { level = 2, userData = Activator.CreateInstance(nodeType.Type) });
                }
            }
        }
        return tree;
    }
}