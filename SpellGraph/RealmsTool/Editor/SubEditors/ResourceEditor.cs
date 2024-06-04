using UnityEditor;
using UnityEngine;
public class ResourceEditor : SubEditor
{
    private bool infoGroup, valueGroup = false;

    public override void ObjectEditor()
    {
        GUILayout.BeginVertical();
        if (selectedObjectIndex != -1 && selectedObjectIndex < objects.Count)
        {
            ScriptableObject selectedObject = objects[selectedObjectIndex];
            Resource selectedResource = selectedObject as Resource;
            SerializedObject serializedObject = new SerializedObject(selectedObject);

            //Display the options panel for the object
            if (ObjectOptions<Resource>(serializedObject))
                return;

            //Editor foldouts
            #region Resource info
            EditorGUILayout.BeginVertical("box");
            infoGroup = EditorGUILayout.BeginFoldoutHeaderGroup(infoGroup, "Resource Info", styles.foldoutStyle);
            if (infoGroup)
            {
                GUILayout.Space(styles.spaceBig);
                LabeledPropertyField("Name", serializedObject.FindProperty("name"));
                LabeledPropertyField("Color", serializedObject.FindProperty("color"));
                LabeledPropertyField("Is Global [?]", serializedObject.FindProperty("isGlobal"));
                GUILayout.Space(styles.spaceBig);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            #endregion

            #region Resource values
            if (selectedResource.IsGlobal)
            {
                EditorGUILayout.BeginVertical("box");
                valueGroup = EditorGUILayout.BeginFoldoutHeaderGroup(valueGroup, "Resource Values", styles.foldoutStyle);
                if (valueGroup)
                {
                    GUILayout.Space(styles.spaceBig);
                    LabeledPropertyField("Starting value", serializedObject.FindProperty("startValue"));
                    LabeledPropertyField("Base value", serializedObject.FindProperty("baseValue"));
                    LabeledPropertyField("Regeneration", serializedObject.FindProperty("regen"));
                    LabeledPropertyField("Regeneration rate", serializedObject.FindProperty("regenRate"));
                    GUILayout.Space(styles.spaceBig);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
            }
            #endregion

            serializedObject.ApplyModifiedProperties();
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("Select a resource to edit.");
        }
        GUILayout.EndVertical();
    }
    public override void SetFoldouts()
    {
        infoGroup = true; valueGroup = true;
    }
}