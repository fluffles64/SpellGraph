using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AnimationCurveAttribute))]
public class AnimationCurveDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        AnimationCurveAttribute animationCurve = attribute as AnimationCurveAttribute;
        if (property.propertyType == SerializedPropertyType.AnimationCurve)
        {      
            EditorGUI.LabelField(position, GUIContent.none);
            Rect curvePosition = EditorGUI.PrefixLabel(position, GUIContent.none);

            // Increase the height of the curvePosition rectangle
            curvePosition.height = EditorGUIUtility.singleLineHeight * 1.5f;

            AnimationCurve curveValue = EditorGUI.CurveField(curvePosition, property.animationCurveValue, Color.cyan, new Rect(animationCurve.x, animationCurve.y, animationCurve.width, animationCurve.height));

            if (GUI.changed)
            {
                property.animationCurveValue = curveValue;
            }
        }
    }
}