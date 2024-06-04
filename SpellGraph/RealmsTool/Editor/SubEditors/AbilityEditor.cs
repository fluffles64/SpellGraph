using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AbilityEditor : SubEditor
{
    private List<bool> effectFoldouts = new List<bool>();
    private bool infoGroup, typeGroup, timeGroup, cooldownGroup, costGroup, rangeGroup, effectsGroup = false;
    private bool displayRangeFoldout = false;

    public override void ObjectEditor()
    {
        GUILayout.BeginVertical();
        if (selectedObjectIndex != -1 && selectedObjectIndex < objects.Count)
        {
            ScriptableObject selectedObject = objects[selectedObjectIndex];
            Ability selectedAbility = selectedObject as Ability;
            SerializedObject serializedObject = new SerializedObject(selectedObject);

            //Display the options panel for the object
            if (ObjectOptions<Ability>(serializedObject))
                return;

            //Editor foldouts
            #region Ability info
            EditorGUILayout.BeginVertical("box");
            infoGroup = EditorGUILayout.BeginFoldoutHeaderGroup(infoGroup, "Ability Info", styles.foldoutStyle);
            if (infoGroup)
            {
                GUILayout.Space(styles.spaceBig);
                LabeledPropertyField("ID", serializedObject.FindProperty("id"));
                LabeledPropertyField("Name", serializedObject.FindProperty("name"));
                LabeledPropertyField("Icon", serializedObject.FindProperty("icon"));
                LabeledPropertyField("Description", serializedObject.FindProperty("description"));
                GUILayout.Space(styles.spaceBig);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            #endregion

            #region Activation
            EditorGUILayout.BeginVertical("box");
            typeGroup = EditorGUILayout.BeginFoldoutHeaderGroup(typeGroup, "Activation", styles.foldoutStyle);
            if (typeGroup)
            {
                GUILayout.Space(styles.spaceBig);
                LabeledPropertyField("Activation Type", serializedObject.FindProperty("activationType"));
                if (selectedAbility.ActivationType == ActivationType.Passive)
                {
                    LabeledPropertyField("Trigger", serializedObject.FindProperty("trigger"));
                }
                else
                {
                    LabeledPropertyField("Target Type", serializedObject.FindProperty("targetType"));
                    if (selectedAbility.TargetType != TargetType.None)
                    {
                        if (selectedAbility.TargetType == TargetType.Target)
                        {
                            LabeledPropertyField("Targets", serializedObject.FindProperty("targets"));
                            LabeledPropertyField("Need to face target [?]", serializedObject.FindProperty("needToFaceTarget"));
                            if (selectedAbility.NeedToFaceTarget)
                            {
                                LabeledPropertyField("Target angle [°]", serializedObject.FindProperty("targetAngle"));
                                LabeledPropertyField("Need target to be backwards [?]", serializedObject.FindProperty("isTargetBackwards"));
                            }
                        }
                        displayRangeFoldout = true;
                    }
                    else
                    {
                        displayRangeFoldout = false;
                    }
                    LabeledPropertyField("Cancels", serializedObject.FindProperty("cancelType"));
                    LabeledPropertyField("Can't use while", serializedObject.FindProperty("cantUseWhile"));
                    GUILayout.Space(styles.spaceBig);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            #endregion

            #region Casting
            if (selectedAbility.ActivationType == ActivationType.Active)
            {
                EditorGUILayout.BeginVertical("box");
                timeGroup = EditorGUILayout.BeginFoldoutHeaderGroup(timeGroup, "Casting", styles.foldoutStyle);
                if (timeGroup)
                {
                    GUILayout.Space(styles.spaceBig);
                    LabeledPropertyField("Cast type", serializedObject.FindProperty("castType"));
                    switch (selectedAbility.CastType)
                    {
                        case CastType.Instant:
                            break;
                        case CastType.Casted:
                            LabeledPropertyField("Cast time", serializedObject.FindProperty("castTime"));
                            break;
                        case CastType.Channeled:
                            LabeledPropertyField("Channel time", serializedObject.FindProperty("castTime"));
                            break;
                        case CastType.Pressed:
                            LabeledPropertyField("Press time", serializedObject.FindProperty("castTime"));
                            Vector2 pressSweetSpotValue = serializedObject.FindProperty("pressSweetSpot").vector2Value;
                            pressSweetSpotValue.x = Mathf.Clamp(pressSweetSpotValue.x, 0f, 100f);
                            pressSweetSpotValue.y = Mathf.Clamp(pressSweetSpotValue.y, pressSweetSpotValue.x, 100f);
                            serializedObject.FindProperty("pressSweetSpot").vector2Value = pressSweetSpotValue;
                            LabeledPropertyField("Press sweet spot", serializedObject.FindProperty("pressSweetSpot"));
                            break;
                        default:
                            break;
                    }

                    if (selectedAbility.CastType != CastType.Instant)
                    {
                        LabeledPropertyField("Move and cast [?]", serializedObject.FindProperty("moveAndCast"));
                        if (selectedAbility.MoveAndCast)
                        {
                            LabeledPropertyField("Slows caster [?]", serializedObject.FindProperty("slowsCaster"));
                            if (selectedAbility.SlowsCaster)
                            {
                                LabeledPropertyField("Slow amount [%]", serializedObject.FindProperty("slowAmount"));
                                LabeledPropertyField("Slow curve type", serializedObject.FindProperty("castSlowCurveType"));
                                if (selectedAbility.CastSlowCurveType != CurveType.Constant && selectedAbility.CastSlowCurveType != CurveType.Custom)
                                {
                                    LabeledPropertyField("Easing type", serializedObject.FindProperty("castSlowEasingType"));
                                    //EditorGUI.BeginDisabledGroup(true);
                                    selectedAbility.CastSlowCurve = SetCurve(selectedAbility.CastSlowCurveType, selectedAbility.CastSlowEasingType);
                                    LabeledPropertyField("Curve", serializedObject.FindProperty("castSlowCurve"));
                                    //EditorGUI.EndDisabledGroup();                     
                                }
                                else if (selectedAbility.CastSlowCurveType == CurveType.Custom)
                                {
                                    LabeledPropertyField("Curve", serializedObject.FindProperty("castSlowCurve"));
                                }
                            }
                        }
                    }

                    GUILayout.Space(styles.spaceBig);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
            }
            #endregion

            #region Cooldown
            EditorGUILayout.BeginVertical("box");
            cooldownGroup = EditorGUILayout.BeginFoldoutHeaderGroup(cooldownGroup, "Cooldown", styles.foldoutStyle);
            if (cooldownGroup)
            {
                GUILayout.Space(styles.spaceBig);
                LabeledPropertyField("Has charges [?]", serializedObject.FindProperty("hasCharges"));
                if (selectedAbility.HasCharges)
                {
                    LabeledPropertyField("Charge type", serializedObject.FindProperty("chargeType"));
                    switch (selectedAbility.ChargeType)
                    {
                        case ChargeType.Constant:
                            break;
                        case ChargeType.OneTime:
                            LabeledPropertyField("Cooldown", serializedObject.FindProperty("cooldown"));
                            break;
                    }
                    LabeledPropertyField("Num of charges", serializedObject.FindProperty("numOfCharges"));
                    LabeledPropertyField("Charge cooldown", serializedObject.FindProperty("chargeCooldown"));
                    LabeledPropertyField("Cooldown", serializedObject.FindProperty("cooldown"));
                }
                else
                {
                    LabeledPropertyField("Cooldown", serializedObject.FindProperty("cooldown"));
                }
                GUILayout.Space(styles.spaceBig);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            #endregion

            #region Cost
            EditorGUILayout.BeginVertical("box");
            costGroup = EditorGUILayout.BeginFoldoutHeaderGroup(costGroup, "Cost", styles.foldoutStyle);
            if (costGroup)
            {
                GUILayout.Space(styles.spaceBig);
                LabeledPropertyField("Cost type", serializedObject.FindProperty("costType"));
                switch (selectedAbility.CostType)
                {
                    case CostType.None:
                        break;
                    case CostType.Flat:
                        LabeledPropertyField("Resource", serializedObject.FindProperty("resource"));
                        LabeledPropertyField("Resource flat cost", serializedObject.FindProperty("resourceFlatCost"));
                        break;
                    case CostType.TotalPercentage:
                        LabeledPropertyField("Resource", serializedObject.FindProperty("resource"));
                        LabeledPropertyField("Is from base resource [?]", serializedObject.FindProperty("isFromBaseResource"));
                        LabeledPropertyField("Resource percentage cost [%]", serializedObject.FindProperty("resourcePercentageCost"));
                        break;
                    case CostType.CurrentPercentage:
                        LabeledPropertyField("Resource", serializedObject.FindProperty("resource"));
                        LabeledPropertyField("Minimum resource cost", serializedObject.FindProperty("minimumResourceCost"));
                        LabeledPropertyField("Resource percentage cost %", serializedObject.FindProperty("resourcePercentageCost"));
                        break;
                    case CostType.MixedTotal:
                        LabeledPropertyField("Resource", serializedObject.FindProperty("resource"));
                        LabeledPropertyField("Is from base resource [?]", serializedObject.FindProperty("isFromBaseResource"));
                        //LabeledPropertyField("", serializedObject.FindProperty(""));
                        LabeledPropertyField("Resource percentage cost [%]", serializedObject.FindProperty("resourcePercentageCost"));
                        break;
                    case CostType.MixedCurrent:
                        LabeledPropertyField("Resource", serializedObject.FindProperty("resource"));
                        LabeledPropertyField("Minimum resource cost", serializedObject.FindProperty("minimumResourceCost"));
                        LabeledPropertyField("Resource flat cost", serializedObject.FindProperty("resourceFlatCost"));
                        LabeledPropertyField("Resource percentage cost [%]", serializedObject.FindProperty("resourcePercentageCost"));
                        break;
                }
                if (selectedAbility.ActivationType == ActivationType.Passive && selectedAbility.CostType != CostType.None)
                {
                    LabeledPropertyField("Cost per cecond", serializedObject.FindProperty("costPerSecond"));
                }
                GUILayout.Space(styles.spaceBig);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.EndVertical();
            #endregion

            #region Range
            if (displayRangeFoldout)
            {
                EditorGUILayout.BeginVertical("box");
                rangeGroup = EditorGUILayout.BeginFoldoutHeaderGroup(rangeGroup, "Range", styles.foldoutStyle);
                if (rangeGroup)
                {
                    GUILayout.Space(styles.spaceBig);
                    LabeledPropertyField("Range Type", serializedObject.FindProperty("rangeType"));
                    switch (selectedAbility.RangeType)
                    {
                        case RangeType.None:
                            break;
                        case RangeType.Melee:
                            break;
                        case RangeType.Ranged:
                            break;
                        case RangeType.Custom:
                            Vector2 rangeValue = serializedObject.FindProperty("range").vector2Value;
                            rangeValue.x = Mathf.Abs(rangeValue.x);
                            rangeValue.y = Mathf.Clamp(rangeValue.y, rangeValue.x, rangeValue.y);
                            serializedObject.FindProperty("range").vector2Value = rangeValue;
                            LabeledPropertyField("Range", serializedObject.FindProperty("range"));
                            break;
                    }
                    GUILayout.Space(styles.spaceBig);
                }
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
            }
            #endregion

            #region Effects
            EditorGUILayout.BeginVertical("box");
            effectsGroup = EditorGUILayout.BeginFoldoutHeaderGroup(effectsGroup, "Effects", styles.foldoutStyle);
            if (effectsGroup)
            {
                GUILayout.Space(styles.spaceBig);
                if (serializedObject.FindProperty("effects").arraySize > 0)
                {
                    for (int i = 0; i < serializedObject.FindProperty("effects").arraySize; i++)
                    {
                        LabeledPropertyField("Effect " + i, serializedObject.FindProperty("effects").GetArrayElementAtIndex(i));
                    }
                    GUILayout.Space(styles.spaceBig);
                }

                // Add buttons to add/remove new effects at the end of the list
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Add"))
                {
                    serializedObject.FindProperty("effects").InsertArrayElementAtIndex(serializedObject.FindProperty("effects").arraySize);
                    effectFoldouts.Add(true);
                }
                if (GUILayout.Button("Remove"))
                {
                    if (serializedObject.FindProperty("effects").arraySize > 0)
                    {
                        serializedObject.FindProperty("effects").DeleteArrayElementAtIndex(serializedObject.FindProperty("effects").arraySize - 1);
                        effectFoldouts.RemoveAt(effectFoldouts.Count - 1);
                    }
                }
                EditorGUILayout.EndHorizontal();
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
            GUILayout.Label("Select an ability to edit.");
        }
        GUILayout.EndVertical();
    }

    public override void SetFoldouts()
    {
        infoGroup = true; typeGroup = true; timeGroup = true; cooldownGroup = true; costGroup = true; rangeGroup = true; effectsGroup = true;
    }
}