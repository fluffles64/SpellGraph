using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;

/// <summary>
/// This class manages the whole editor window that displays the graph view. It is closely
/// tied to the SpellGraph class. It basically adds the GraphView on top of itself. Also manages
/// visual elements such as buttons on the toolbar, the minimap, and the blackboard. It also adds
/// drag and drop functionalities so that instance variables from the blackboard can be dragged inside
/// the window, creating var nodes.
/// This setup is inspired by Mert Kirimgeri's amazing tutorial series.
/// See: https://www.youtube.com/watch?v=7KHGH0fPL84
/// </summary>

public class SpellEditorWindow : EditorWindow
{
    private SpellGraph graphView;
    private string assetName = "New effect";

    [MenuItem("Tools/SpellGraph")]
    public static void ShowWindow()
    {
        SpellEditorWindow window = GetWindow<SpellEditorWindow>("SpellGraph");
    }

    private void OnGUI()
    {
        if (Event.current.rawType == EventType.Layout)
        {
            MiniMap miniMap = graphView.contentContainer.Q<MiniMap>();
            miniMap.SetPosition(new Rect(position.width - 160, 30, 150, 100));

            Blackboard blackboard = graphView.contentContainer.Q<Blackboard>();
            blackboard.SetPosition(new Rect(5, 25, 250, position.height - 30));
        }
    }

    private void OnEnable()
    {
        SetGraphView();
        SetToolBar();
        SetMiniMap();
        SetBlackBoard();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }

    private void SetGraphView()
    {
        graphView = new SpellGraph(this);
        graphView.StretchToParentSize();
        graphView.viewport.RegisterCallback<MouseEnterEvent>(OnMouseEnter);
        graphView.viewport.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
        graphView.viewport.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
        graphView.viewport.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);     
        rootVisualElement.Add(graphView);
    }

    private void SetToolBar()
    {
        Toolbar toolbar = new Toolbar();

        ObjectField effectObjectField = new ObjectField("Effect:");
        effectObjectField.objectType = typeof(Effect);
        effectObjectField.labelElement.style.minWidth = 80;

        string[] assetGUIDs = AssetDatabase.FindAssets("t:Effect");
        string selectedEffectPath = assetGUIDs
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .FirstOrDefault(path => AssetDatabase.LoadAssetAtPath<Effect>(path)?.name == assetName);

        if (!string.IsNullOrEmpty(selectedEffectPath))
        {
            Effect selectedEffect = AssetDatabase.LoadAssetAtPath<Effect>(selectedEffectPath);
            effectObjectField.value = selectedEffect;
        }

        effectObjectField.RegisterValueChangedCallback(evt =>
        {
            Effect selectedEffect = evt.newValue as Effect;
            if (selectedEffect != null)
            {
                assetName = selectedEffect.name;
                Blackboard blackboard = graphView.contentContainer.Q<Blackboard>();
                if (blackboard != null)
                    blackboard.title = assetName;
                SaveOrLoad(false);
            }
        });

        toolbar.Add(effectObjectField);
        toolbar.Add(new Button(() => SaveOrLoad(true)) { text = "Save" });
        toolbar.Add(new Button(() => SaveOrLoad(false)) { text = "Load" });
        rootVisualElement.Add(toolbar);
    }

    private void SetMiniMap()
    {
        MiniMap miniMap = new MiniMap { anchored = true };
        miniMap.SetPosition(new Rect(10, 30, 200, 140));
        miniMap.elementTypeColor = Color.blue;
        graphView.Add(miniMap);
    }

    private void SetBlackBoard()
    {
        var blackboard = new Blackboard(graphView) { scrollable = true, title = assetName };
        blackboard.capabilities &= ~Capabilities.Deletable;
        blackboard.capabilities &= ~Capabilities.Selectable;

        blackboard.addItemRequested = board =>
        {
            graphView.ShowBlackboardMenu();
        };
        blackboard.editTextRequested = (_blackboard, element, newValue) =>
        {
            var oldPropertyName = ((BlackboardField)element).text;
            if (graphView.InstanceVariables.Any(x => x.Name == newValue))
            {
                return;
            }

            var propertyIndex = graphView.InstanceVariables.FindIndex(x => x.Name == oldPropertyName);
            graphView.InstanceVariables[propertyIndex].Name = newValue;
            ((BlackboardField)element).text = newValue;
        };
        blackboard.moveItemRequested = (_blackboard, posToMove, visualElement) =>
        {
            if (posToMove >= 0)
            {
                // Remove the 'visualElement' from its current position in the blackboard.
                //visualElement.RemoveFromHierarchy();
            }
        };
        blackboard.SetPosition(new Rect(10, 180, 300, 300));
        graphView.Add(blackboard);
        graphView.Blackboard = blackboard;
    }

    private void SaveOrLoad(bool save)
    {
        if (string.IsNullOrEmpty(assetName))
        {
            EditorUtility.DisplayDialog("Invalid file name", "Please enter a valid file name", "OK");
        }

        GraphSaveUtil saveUtility = GraphSaveUtil.GetInstance(graphView);
        if (save)
            saveUtility.SaveGraph(assetName);
        else
            saveUtility.LoadGraph(assetName);
    }

    #region DragAndDrop
    private bool canDrop = true;
    private bool isDragging = false;

    private void OnMouseEnter(MouseEnterEvent e)
    {
        if (isDragging)
            canDrop = false;
    }
    public void OnDragUpdatedEvent(DragUpdatedEvent e)
    {
        isDragging = DragAndDrop.objectReferences.Length > 0;

        // Need this check to avoid dragging files from file explorer
        bool containsFiles = DragAndDrop.paths != null && DragAndDrop.paths.Length > 0;

        if (containsFiles || !canDrop)
            DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
        else
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

        e.StopPropagation();
    }

    public void OnDragPerformEvent(DragPerformEvent e)
    {
        if (canDrop)
        {
            DragAndDrop.AcceptDrag();

            // Convert mouse pos to local coords of the graphView
            Vector2 graphMousePosition = graphView.contentViewContainer.WorldToLocal(e.mousePosition);

            var varNode = new VariableNode();
            var nodeData = new VarNodeData() { VarName = SpellGraph.PropertyName, VarType = new SerializableSystemType(SpellGraph.PropertyType) };

            varNode.SetValues(nodeData);
            varNode.SetPosition(new Rect(graphMousePosition.x, graphMousePosition.y, 100, 150));
            graphView.AddElement(varNode);
            e.StopPropagation();
        }
    }

    public void OnDragExitedEvent(DragExitedEvent e)
    {
        isDragging = false;
        canDrop = true;
    }
    #endregion
}