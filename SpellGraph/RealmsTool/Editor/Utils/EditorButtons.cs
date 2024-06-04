using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Ability), true)]
public class AbilityEditorButton : Editor
{
    public override void OnInspectorGUI()
    {
        Ability obj = (Ability)target;
        SerializedObject serializedObject = new SerializedObject(obj);

        GUILayout.Space(10);

        if (GUILayout.Button("Open in Editor"))
        {
            ExtendedEditorWindow.ShowWindow<Ability>(serializedObject);
        }
    }
}

[CustomEditor(typeof(Resource), true)]
public class ResourceEditorButton : Editor
{
    public override void OnInspectorGUI()
    {
        Resource obj = (Resource)target;
        SerializedObject serializedObject = new SerializedObject(obj);

        GUILayout.Space(10);

        if (GUILayout.Button("Open in Editor"))
        {
            ExtendedEditorWindow.ShowWindow<Resource>(serializedObject);
        }
    }
}

[CustomEditor(typeof(Stats), true)]
public class StatsEditorButton : Editor
{
    public override void OnInspectorGUI()
    {
        Stats obj = (Stats)target;
        SerializedObject serializedObject = new SerializedObject(obj);

        GUILayout.Space(10);

        if (GUILayout.Button("Open in Editor"))
        {
            ExtendedEditorWindow.ShowWindow<Stats>(serializedObject);
        }
    }
}