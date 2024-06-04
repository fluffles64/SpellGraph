using UnityEngine;

/// <summary>
/// Base class of the data manager tool "Realms Tool". Initializes sub-editors and draws all the GUI elements in OnGUI().
/// </summary>

public class RealmsTool : ExtendedEditorWindow
{
    private void OnEnable()
    {
        abilityEditor = CreateInstance<AbilityEditor>();
        resourceEditor = CreateInstance<ResourceEditor>();
        statsEditor = CreateInstance<StatsEditor>();
        Load();
        InitializeFoldouts();
    }

    private void OnGUI()
    {
        // GUIStyles can only be called inside OnGUI
        if (!styles.isInitialized)
        {
            styles = InitializeGUIStyles();
            styles.isInitialized = true;
        }
        if (!SubEditor.styles.isInitialized)
        {
            SubEditor.styles = InitializeGUIStyles();
            SubEditor.styles.isInitialized = true;
        }

        Load();

        GUILayout.BeginVertical();

        // Horizontal column at the top
        GUILayout.BeginHorizontal();
        WindowTopBar();
        GUILayout.EndHorizontal();

        // Menu OR editor
        GUILayout.BeginHorizontal();
        if (showMainMenu)
        {
            WindowMainMenu();
        }
        else
        {
            SubEditor.SetContentWidth();
            ShowEditor();
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }

    // Repaints window whenever an object changes in the hierarchy (right now used for stats)
    private void OnHierarchyChange()
    {
        Repaint();
    }

    private void OnDisable()
    {

    }
}