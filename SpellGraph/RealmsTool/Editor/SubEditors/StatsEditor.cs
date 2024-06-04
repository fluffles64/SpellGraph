using UnityEditor;
using UnityEngine;
public class StatsEditor : SubEditor
{
    private bool infoGroup, offenseGroup, defenseGroup, utilityGroup, resourceGroup = false;

    public override void ObjectEditor()
    {
        GUILayout.BeginVertical();
        if (selectedObjectIndex != -1 && selectedObjectIndex < objects.Count)
        {
            ScriptableObject selectedObject = objects[selectedObjectIndex];
            Stats selectedStat = selectedObject as Stats;
            SerializedObject serializedObject = new SerializedObject(selectedObject);

            //Display the options panel for the object
            if (ObjectOptions<Stats>(serializedObject))
                return;

            //Editor foldouts
            #region Info
            EditorGUILayout.BeginVertical("box");
            infoGroup = EditorGUILayout.BeginFoldoutHeaderGroup(offenseGroup, "Stat info", styles.foldoutStyle);
            if (infoGroup)
            {
                GUILayout.Space(styles.spaceBig);
                LabeledPropertyField("Name", serializedObject.FindProperty("name"));
                GUILayout.Space(styles.spaceBig);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            #endregion

            #region Offense
            EditorGUILayout.BeginVertical("box");
            offenseGroup = EditorGUILayout.BeginFoldoutHeaderGroup(offenseGroup, "Offense", styles.foldoutStyle);
            if (offenseGroup)
            {
                GUILayout.Space(styles.spaceBig);
                StatPropertyField("Ad", serializedObject.FindProperty("ad").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("ad").FindPropertyRelative("value"));
                StatPropertyField("Ap", serializedObject.FindProperty("ap").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("ap").FindPropertyRelative("value"));
                StatPropertyField("Attack speed", serializedObject.FindProperty("attackSpeed").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("attackSpeed").FindPropertyRelative("value"));
                StatPropertyField("Crit chance [%]", serializedObject.FindProperty("critChance").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("critChance").FindPropertyRelative("value"));
                StatPropertyField("Crit damage", serializedObject.FindProperty("critDamage").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("critDamage").FindPropertyRelative("value"));
                StatPropertyField("Armor pen", serializedObject.FindProperty("armorPen").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("armorPen").FindPropertyRelative("value"));
                StatPropertyField("Magic pen", serializedObject.FindProperty("magicPen").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("magicPen").FindPropertyRelative("value"));
                StatPropertyField("Lifesteal", serializedObject.FindProperty("lifeSteal").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("lifeSteal").FindPropertyRelative("value"));
                GUILayout.Space(styles.spaceBig);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            #endregion

            #region Defense
            EditorGUILayout.BeginVertical("box");
            defenseGroup = EditorGUILayout.BeginFoldoutHeaderGroup(defenseGroup, "Defense", styles.foldoutStyle);
            if (defenseGroup)
            {
                GUILayout.Space(styles.spaceBig);
                StatPropertyField("Ar", serializedObject.FindProperty("ar").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("ar").FindPropertyRelative("value"));
                StatPropertyField("Mr", serializedObject.FindProperty("mr").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("mr").FindPropertyRelative("value"));
                StatPropertyField("Tenacity", serializedObject.FindProperty("tenacity").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("tenacity").FindPropertyRelative("value"));
                GUILayout.Space(styles.spaceBig);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            #endregion

            #region Utility
            EditorGUILayout.BeginVertical("box");
            utilityGroup = EditorGUILayout.BeginFoldoutHeaderGroup(utilityGroup, "Utility", styles.foldoutStyle);
            if (utilityGroup)
            {
                GUILayout.Space(styles.spaceBig);
                StatPropertyField("Move speed", serializedObject.FindProperty("moveSpeed").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("moveSpeed").FindPropertyRelative("value"));
                StatPropertyField("Ability haste", serializedObject.FindProperty("abilityHaste").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("abilityHaste").FindPropertyRelative("value"));
                StatPropertyField("Melee range", serializedObject.FindProperty("meleeRange").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("meleeRange").FindPropertyRelative("value"));
                StatPropertyField("Ranged range", serializedObject.FindProperty("rangedRange").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("rangedRange").FindPropertyRelative("value"));
                StatPropertyField("Gold regen", serializedObject.FindProperty("goldGeneration").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("goldGeneration").FindPropertyRelative("value"));
                GUILayout.Space(styles.spaceBig);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            #endregion

            #region Resources
            EditorGUILayout.BeginVertical("box");
            resourceGroup = EditorGUILayout.BeginFoldoutHeaderGroup(resourceGroup, "Resources", styles.foldoutStyle);
            if (resourceGroup)
            {
                GUILayout.Space(styles.spaceBig);
                LabeledPropertyField("Health", serializedObject.FindProperty("health"));
                StatPropertyField("Max Health", serializedObject.FindProperty("maxHealth").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("maxHealth").FindPropertyRelative("value"));
                StatPropertyField("Health regen", serializedObject.FindProperty("healthRegen").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("healthRegen").FindPropertyRelative("value"));
                StatPropertyField("Health regen rate", serializedObject.FindProperty("healthRegenRate").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("healthRegenRate").FindPropertyRelative("value"));

                LabeledPropertyField("Resource asset", serializedObject.FindProperty("resourceObj"));
                if(selectedStat.ResourceObj != null)
                {
                    if (!selectedStat.ResourceObj.IsGlobal)
                    {
                        LabeledPropertyField("Resource", serializedObject.FindProperty("resource"));
                        StatPropertyField("Max Resource", serializedObject.FindProperty("maxResource").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("maxResource").FindPropertyRelative("value"));
                        StatPropertyField("Resource regen", serializedObject.FindProperty("resourceRegen").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("resourceRegen").FindPropertyRelative("value"));
                        StatPropertyField("Resource regen rate", serializedObject.FindProperty("resourceRegenRate").FindPropertyRelative("BaseValue"), serializedObject.FindProperty("resourceRegenRate").FindPropertyRelative("value"));
                    }
                    else
                    {
                        LabeledPropertyField("Resource", serializedObject.FindProperty("resource"));
                        //LabeledPropertyField("Max Resource", serializedObject.FindProperty("maxResource").FindPropertyRelative("value"));
                        //LabeledPropertyField("Resource regen", serializedObject.FindProperty("resourceRegen").FindPropertyRelative("value"));
                        //LabeledPropertyField("Resource regen rate", serializedObject.FindProperty("resourceRegenRate").FindPropertyRelative("value"));
                    }

                }
                else if (selectedStat.ResourceObj != null)
                {
                    //selectedStat.Resource = selectedStat.ResourceObj.BaseValue;
                }
                GUILayout.Space(styles.spaceBig);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            #endregion

            serializedObject.ApplyModifiedProperties();
            GUILayout.EndScrollView();
        }
        else
        {
            GUILayout.Label("Select a stat to edit.");
        }
        GUILayout.EndVertical();
    }

    public override void SetFoldouts()
    {
        infoGroup = true; offenseGroup = true; defenseGroup = true; utilityGroup = true; resourceGroup = true;
    }

    private void StatPropertyField(string label, SerializedProperty property1, SerializedProperty property2)
    {
        contentWidth = EditorGUIUtility.currentViewWidth * styles.contentMaxWidth; // TODO: Move this to anywhere else its pretty inefficient

        EditorGUILayout.Space(styles.spaceTiny);
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (property1 != null && property2 != null)
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(styles.buttonBig - styles.spaceMedium));
            EditorGUILayout.PropertyField(property1, GUIContent.none, GUILayout.Width(contentWidth / 2));
            EditorGUILayout.PropertyField(property2, GUIContent.none, GUILayout.Width(contentWidth / 2));
        }
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
    }
}