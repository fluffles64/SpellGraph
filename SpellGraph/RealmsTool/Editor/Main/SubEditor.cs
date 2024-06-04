using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using static ExtendedEditorWindow;

/// <summary>
/// Base class all sub-editors in RealmsTool inherit from. Contains all of the necessary methods to display the elements in each sub-editor,
/// and some helper methods. To create a new editor, override both the ObjectEditor() and SetFoldouts() methods, or look at the already existing examples.
/// For the animation curve editor, see: https://www.youtube.com/watch?v=rdzqHK3Yu8Q
/// </summary>

public abstract class SubEditor : ScriptableObject
{
    public static GUIStyles styles;

    [SerializeField] protected Sprite defaultIcon;
    protected List<ScriptableObject> objects = new List<ScriptableObject>();
    protected int selectedObjectIndex = -1;
    protected ScriptableObject currentSelectedObject;
    [SerializeField] protected string scriptableObjectPath = "Assets/SpellGraph/Examples/";

    protected static bool debugMode = false;
    protected static Vector2 scrollPosition;
    protected static Vector2 scrollPosition2;
    protected static Vector2 scrollPosition4;
    protected static float contentWidth;
    protected static Func<float, float>[,] EasingFunctionList = new Func<float, float>[Enum.GetValues(typeof(CurveType)).Length, Enum.GetValues(typeof(EasingType)).Length];

    #region Editor Methods
    // Abstract methods
    public abstract void ObjectEditor();
    public abstract void SetFoldouts();

    //Generic methods
    public void ObjectList()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, styles.backgroundStyle, GUILayout.Width(styles.buttonBig), GUILayout.ExpandHeight(true));

        foreach (var obj in objects)
        {
            GUILayout.Space(styles.spaceTiny);
            EditorGUILayout.BeginHorizontal();

            Rect buttonRect = GUILayoutUtility.GetRect(styles.iconMedium + 2, styles.iconMedium + 2);

            if (GUI.Button(buttonRect, "", styles.buttonStyle))
            {
                selectedObjectIndex = objects.IndexOf(obj);
                currentSelectedObject = obj;
            }

            // Check if obj has a property called "Name" using reflection
            var nameProperty = obj.GetType().GetProperty("Name");
            if (nameProperty != null)
            {
                string objectName = (string)nameProperty.GetValue(obj);
                if (objectName != null)
                    GUI.Label(buttonRect, objectName, styles.clickableStyle);
            }

            // Check if obj has a property called "Icon" using reflection
            var iconProperty = obj.GetType().GetProperty("Icon");
            if (iconProperty != null)
            {
                Sprite objectIcon = (Sprite)iconProperty.GetValue(obj);
                if (objectIcon != null)
                    GUI.DrawTexture(new Rect(buttonRect.x, buttonRect.y, styles.iconMedium, styles.iconMedium), objectIcon.texture);
            }

            // Else use default icon of each editor type
            else if (defaultIcon != null)
            {
                GUI.DrawTexture(new Rect(buttonRect.x, buttonRect.y, styles.iconMedium, styles.iconMedium), defaultIcon.texture);
            }
            EditorGUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();
    }

    public bool ObjectOptions<T>(SerializedObject serializedObject) where T : ScriptableObject
    {
        // Display the selected object's icon
        if (currentSelectedObject != null)
        {
            GUILayout.BeginHorizontal("box", GUILayout.Height(styles.titleHeight), GUILayout.ExpandWidth(true));
            GUILayout.FlexibleSpace();

            // Button to save changes
            if (GUILayout.Button("Save", styles.optionsButtonStyle))
            {
                //serializedObject.ApplyModifiedProperties();
            }
            GUILayout.FlexibleSpace();

            // Button to create new object
            if (GUILayout.Button("New", styles.optionsButtonStyle))
            {
                T newObject = Create<T>();
                objects.Add(newObject);
                currentSelectedObject = newObject;
                selectedObjectIndex = objects.IndexOf(newObject);
            }
            GUILayout.FlexibleSpace();

            // Button to duplicate current object
            if (GUILayout.Button("Duplicate", styles.optionsButtonStyle))
            {

            }
            GUILayout.FlexibleSpace();

            // Object icon
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            if (serializedObject.FindProperty("icon") != null)
            {
                Texture tex = AssetPreview.GetAssetPreview(serializedObject.FindProperty("icon").objectReferenceValue);
                GUILayout.Label(tex, GUILayout.Width(styles.iconBig), GUILayout.Height(styles.iconBig));
            }
            else if (defaultIcon != null)
                GUILayout.Label(defaultIcon.texture, GUILayout.Width(styles.iconBig), GUILayout.Height(styles.iconBig));
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            // Display the selected object's name
            if (currentSelectedObject != null)
            {
                GUILayout.Space(styles.spaceBetweenIconAndTitle);
                var nameProperty = currentSelectedObject.GetType().GetProperty("Name");
                if (nameProperty != null)
                {
                    string objectName = (string)nameProperty.GetValue(currentSelectedObject);
                    if (objectName != null)
                        GUILayout.Label(objectName, styles.titleStyle);
                }
            }
            GUILayout.FlexibleSpace();

            // Button to delete object
            if (GUILayout.Button("Delete", styles.optionsButtonStyle))
            {
                if (selectedObjectIndex >= 0 && selectedObjectIndex < objects.Count)
                {
                    Delete<T>(currentSelectedObject);
                    objects.RemoveAt(selectedObjectIndex);

                    // After removing, adjust the selectedObjectIndex if necessary
                    if (objects.Count > 0)
                    {
                        selectedObjectIndex = Mathf.Clamp(selectedObjectIndex, 0, objects.Count - 1);
                        currentSelectedObject = objects[selectedObjectIndex];
                    }
                    else
                    {
                        selectedObjectIndex = -1;
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                    return true;
                }
                else
                {
                    Debug.LogWarning("No ability selected for removal.");
                }
            }
            GUILayout.FlexibleSpace();

            // Button to reset object
            if (GUILayout.Button("Reset", styles.optionsButtonStyle))
            {

            }
            GUILayout.FlexibleSpace();

            // Button to toggle debug mode
            if (GUILayout.Button(debugMode ? "Debug (On)" : "Debug (Off)", styles.optionsButtonStyle))
            {
                debugMode = !debugMode;
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2, styles.backgroundStyle, GUILayout.ExpandWidth(true));

        // Display all elements without style
        if (debugMode)
        {
            scrollPosition4 = GUILayout.BeginScrollView(scrollPosition4, styles.backgroundStyle, GUILayout.ExpandWidth(true));
            SerializedProperty property = serializedObject.GetIterator();
            bool enterChildren = true;

            while (property.NextVisible(enterChildren))
            {
                LabeledPropertyField(property.displayName, property);
                enterChildren = false;
            }
            serializedObject.ApplyModifiedProperties();

            GUILayout.EndScrollView();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            return true;
        }
        return false;
    }

    public void LoadObjects<T>() where T : ScriptableObject
    {
        objects.Clear();
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(T));
            if (asset is T scriptableObject)
            {
                objects.Add(scriptableObject);
            }
        }

        // Add any runtime-created instances to the list
        T[] runtimeInstances = FindObjectsOfType<T>();
        objects.AddRange(runtimeInstances);
    }

    public void LoadObject<T>(SerializedObject obj) where T : ScriptableObject
    {
        ScriptableObject scriptableObject = obj.targetObject as T;

        if (scriptableObject != null)
        {
            currentSelectedObject = scriptableObject;
            selectedObjectIndex = objects.IndexOf(scriptableObject);
        }
        else
        {
            Debug.Log($"Object of type {typeof(T).Name} couldn't be opened");
        }
    }

    protected T Create<T>() where T : ScriptableObject
    {
        T newObject = CreateInstance<T>();
        string folderName = GetTypeNames<T>();
        string typeName = typeof(T).Name;
        string path = AssetDatabase.GenerateUniqueAssetPath($"{scriptableObjectPath}{folderName}/New{typeName}.asset");
        try
        {
            AssetDatabase.CreateAsset(newObject, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        catch (Exception e)
        {
            Debug.Log($"Error creating asset: {e}");
            return null;
        }
        return newObject;
    }

    protected void Delete<T>(ScriptableObject obj) where T : ScriptableObject
    {
        string typeName = typeof(T).Name;

        if (obj != null)
        {
            string assetPath = AssetDatabase.GetAssetPath(obj);

            if (!string.IsNullOrEmpty(assetPath))
            {
                // Make sure the asset is in the right folder
                string subFolderName = GetTypeNames<T>();
                string folderName = $"{scriptableObjectPath}{subFolderName}/";

                if (assetPath.StartsWith(folderName))
                {
                    // Delete the asset & .meta files
                    File.Delete(assetPath);
                    File.Delete(assetPath + ".meta");
                    AssetDatabase.Refresh();
                }
                else
                {
                    Debug.Log($"The selected {typeName} is not in the {folderName} folder and cannot be deleted.");
                }
            }
            else
            {
                Debug.Log($"The selected {typeName} is not a valid asset and cannot be deleted.");
            }
        }
        else
        {
            Debug.Log($"The provided {typeName} object is null and cannot be deleted.");
        }
    }
    #endregion

    #region Helper Methods
    protected string GetTypeNames<T>()
    {
        switch (typeof(T).Name)
        {
            case "Ability":
                return "Abilities";
            case "Item":
                return "Items";
            case "Resource":
                return "Resources";
            case "Stats":
                return "Stats";
            case "NPC":
                return "NPCs";
            default:
                return "Error";
        }
    }

    public static void SetContentWidth()
    {
        contentWidth = EditorGUIUtility.currentViewWidth * styles.contentMaxWidth;
    }

    protected void LabeledPropertyField(string label, SerializedProperty property)
    {
        EditorGUILayout.Space(styles.spaceTiny);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (property != null)
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(styles.buttonBig - styles.spaceMedium));
            EditorGUILayout.PropertyField(property, GUIContent.none, GUILayout.Width(contentWidth));
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }

    protected void ReadonlyPropertyField(string label, SerializedProperty property)
    {
        EditorGUILayout.Space(styles.spaceTiny);
        Rect position = EditorGUILayout.GetControlRect();
        EditorGUI.BeginProperty(position, GUIContent.none, property);

        EditorGUI.PrefixLabel(position, 0, new GUIContent(label));
        EditorGUI.PropertyField(position, property, GUIContent.none);

        EditorGUI.EndProperty();
    }

    protected AnimationCurve SetCurve(CurveType curveType, EasingType easingType)
    {
        EasingFunctionList[(int)CurveType.Linear, (int)EasingType.In] = Easings.Linear;
        EasingFunctionList[(int)CurveType.Linear, (int)EasingType.Out] = Easings.Linear;
        EasingFunctionList[(int)CurveType.Linear, (int)EasingType.InOut] = Easings.Linear;

        EasingFunctionList[(int)CurveType.Sine, (int)EasingType.In] = Easings.Sinusoidal.In;
        EasingFunctionList[(int)CurveType.Sine, (int)EasingType.Out] = Easings.Sinusoidal.Out;
        EasingFunctionList[(int)CurveType.Sine, (int)EasingType.InOut] = Easings.Sinusoidal.InOut;

        EasingFunctionList[(int)CurveType.Quad, (int)EasingType.In] = Easings.Quadratic.In;
        EasingFunctionList[(int)CurveType.Quad, (int)EasingType.Out] = Easings.Quadratic.Out;
        EasingFunctionList[(int)CurveType.Quad, (int)EasingType.InOut] = Easings.Quadratic.InOut;

        EasingFunctionList[(int)CurveType.Cubic, (int)EasingType.In] = Easings.Cubic.In;
        EasingFunctionList[(int)CurveType.Cubic, (int)EasingType.Out] = Easings.Cubic.Out;
        EasingFunctionList[(int)CurveType.Cubic, (int)EasingType.InOut] = Easings.Cubic.InOut;

        EasingFunctionList[(int)CurveType.Quart, (int)EasingType.In] = Easings.Quartic.In;
        EasingFunctionList[(int)CurveType.Quart, (int)EasingType.Out] = Easings.Quartic.Out;
        EasingFunctionList[(int)CurveType.Quart, (int)EasingType.InOut] = Easings.Quartic.InOut;

        EasingFunctionList[(int)CurveType.Quint, (int)EasingType.In] = Easings.Quintic.In;
        EasingFunctionList[(int)CurveType.Quint, (int)EasingType.Out] = Easings.Quintic.Out;
        EasingFunctionList[(int)CurveType.Quint, (int)EasingType.InOut] = Easings.Quintic.InOut;

        EasingFunctionList[(int)CurveType.Expo, (int)EasingType.In] = Easings.Exponential.In;
        EasingFunctionList[(int)CurveType.Expo, (int)EasingType.Out] = Easings.Exponential.Out;
        EasingFunctionList[(int)CurveType.Expo, (int)EasingType.InOut] = Easings.Exponential.InOut;

        EasingFunctionList[(int)CurveType.Circ, (int)EasingType.In] = Easings.Circular.In;
        EasingFunctionList[(int)CurveType.Circ, (int)EasingType.Out] = Easings.Circular.Out;
        EasingFunctionList[(int)CurveType.Circ, (int)EasingType.InOut] = Easings.Circular.InOut;

        EasingFunctionList[(int)CurveType.Back, (int)EasingType.In] = Easings.Back.In;
        EasingFunctionList[(int)CurveType.Back, (int)EasingType.Out] = Easings.Back.Out;
        EasingFunctionList[(int)CurveType.Back, (int)EasingType.InOut] = Easings.Back.InOut;

        EasingFunctionList[(int)CurveType.Elastic, (int)EasingType.In] = Easings.Elastic.In;
        EasingFunctionList[(int)CurveType.Elastic, (int)EasingType.Out] = Easings.Elastic.Out;
        EasingFunctionList[(int)CurveType.Elastic, (int)EasingType.InOut] = Easings.Elastic.InOut;

        EasingFunctionList[(int)CurveType.Bounce, (int)EasingType.In] = Easings.Bounce.In;
        EasingFunctionList[(int)CurveType.Bounce, (int)EasingType.Out] = Easings.Bounce.Out;
        EasingFunctionList[(int)CurveType.Bounce, (int)EasingType.InOut] = Easings.Bounce.InOut;

        Dictionary<CurveType, int> NumKeyFramesMap = new Dictionary<CurveType, int>
        {
            { CurveType.Linear, 2 },
            { CurveType.Sine, 20 },
            { CurveType.Quad, 20 },
            { CurveType.Cubic, 20 },
            { CurveType.Quart, 20 },
            { CurveType.Quint, 20 },
            { CurveType.Expo, 20 },
            { CurveType.Circ, 30 },
            { CurveType.Back, 20 },
            { CurveType.Elastic, 20 },
            { CurveType.Bounce, 50 },
        };

        // Default to 20 if not found in the dictionary
        int NumKeyFrames = NumKeyFramesMap.ContainsKey(curveType) ? NumKeyFramesMap[curveType] : 20;
        float Time = 1;
        float TimeMax = 1.5f;
        float Scalar = 1.0f;
        AnimationCurve curve;

        if (curveType != CurveType.Constant)
        {
            List<Keyframe> keys = new List<Keyframe>(NumKeyFrames);
            Func<float, float> function = EasingFunctionList[(int)curveType, (int)easingType];

            for (int i = 0; i < NumKeyFrames; i++)
            {
                float timeFrac = (float)i / (NumKeyFrames - 1);
                float time = Time * timeFrac;
                float value = function(timeFrac) * Scalar;

                if (time <= TimeMax)
                {
                    Keyframe key = new Keyframe(time, value);
                    keys.Add(key);
                }
            }

            curve = new AnimationCurve(keys.ToArray());

            for (int i = 0; i < keys.Count; i++)
            {
                AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.ClampedAuto);
                AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.ClampedAuto);
            }
            return curve;
        }
        else
        {
            curve = new AnimationCurve();
            curve.AddKey(0f, 1f);
            curve.AddKey(1f, 1f);
            return curve;
        }
    }

    // Method to draw all properties of an object (unused)
    protected void DrawProperties(SerializedProperty prop, bool drawChildren)
    {
        string lastPropPath = string.Empty;
        foreach (SerializedProperty p in prop)
        {
            if (p.isArray && p.propertyType == SerializedPropertyType.Generic)
            {
                EditorGUILayout.BeginHorizontal();
                p.isExpanded = EditorGUILayout.Foldout(p.isExpanded, p.displayName);
                EditorGUILayout.EndHorizontal();

                if (p.isExpanded)
                {
                    EditorGUI.indentLevel++;
                    DrawProperties(p, drawChildren);
                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(lastPropPath) && p.propertyPath.Contains(lastPropPath)) { continue; }
                lastPropPath = p.propertyPath;
                EditorGUILayout.PropertyField(p, drawChildren);
            }
        }
    }
    #endregion
}