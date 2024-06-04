using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class VFXNodeData : NodeData
{
    public float Duration;
    public Object VFX;
    public Object Prefab;
    public string TransformName;
}

#if UNITY_EDITOR
[NodeInfo("VFX", "VFX", "Places particles in a transform for a set amount of seconds")]
public class VFXNode : ExtendedNode
{
    private float duration;
    private Object selectedVFX;
    private Object selectedPrefab;
    private string selectedBoneName = "None";
    private string[] bones = { };

    [InputPort("In")]
    public Trigger Input;

    [OutputPort("Out")]
    public Trigger Output;

    public VFXNode()
    {
        var durationField = new FloatField("Duration") { style = { width = 250 } };
        durationField.RegisterValueChangedCallback(e => duration = e.newValue);
        extensionContainer.Add(durationField);

        var vfxField = new UnityEditor.UIElements.ObjectField("VFX") { objectType = typeof(Object), name = "VFXField", style = { width = 250 } };
        vfxField.RegisterValueChangedCallback(e => selectedVFX = e.newValue);
        extensionContainer.Add(vfxField);

        var prefabField = new UnityEditor.UIElements.ObjectField("Prefab") { objectType = typeof(Object), name = "PrefabField", style = { width = 250 } };
        prefabField.RegisterValueChangedCallback(e => { selectedPrefab = e.newValue; UpdateBoneOptions(); });
        extensionContainer.Add(prefabField);

        VisualElement dropdownContainer = DropdownContainer("Bone", selectedBoneName, bones, newValue => selectedBoneName = newValue);
        extensionContainer.Add(dropdownContainer);
    }

    private void UpdateBoneOptions()
    {
        bones = new string[0];

        if (selectedPrefab != null && selectedPrefab is Object)
        {
            GameObject prefab = selectedPrefab as GameObject;

            if (prefab != null)
            {
                // Instantiate the selected prefab and check if it has the tag in its children
                GameObject instance = GameObject.Instantiate(prefab);
                Transform[] children = instance.GetComponentsInChildren<Transform>(true);
                List<string> tempList = new List<string>();
                foreach (Transform child in children)
                {
                    if (child.CompareTag("VFXTransform"))
                        tempList.Add(child.name);
                }

                // If no children with the tag were found, set selectedTransformName to "None"
                if (tempList.Count == 0)
                    selectedBoneName = "None";

                GameObject.DestroyImmediate(instance);
                bones = tempList.ToArray();
            }
        }

        // Update dropdown choices
        DropdownField dropdown = extensionContainer.Q<DropdownField>();
        dropdown.choices = new List<string>(bones);
        dropdown.SetValueWithoutNotify(selectedBoneName);
    }

    #region INodeData
    public override void SetValues(NodeData data)
    {
        if (data is VFXNodeData nodeData)
        {
            duration = nodeData.Duration;
            selectedVFX = nodeData.VFX;
            selectedPrefab = nodeData.Prefab;
            selectedBoneName = nodeData.TransformName;

            var durationField = extensionContainer.Q<FloatField>();
            durationField.SetValueWithoutNotify(duration);

            var vfxField = extensionContainer.Q<UnityEditor.UIElements.ObjectField>("VFXField");
            vfxField.SetValueWithoutNotify(selectedVFX);
            vfxField.RegisterValueChangedCallback(e => selectedVFX = e.newValue);         

            var prefabField = extensionContainer.Q<UnityEditor.UIElements.ObjectField>("PrefabField");
            prefabField.SetValueWithoutNotify(selectedPrefab);
            prefabField.RegisterValueChangedCallback(e => selectedPrefab = e.newValue);

            DropdownField dropdown = extensionContainer.Q<DropdownField>();
            dropdown?.SetValueWithoutNotify(selectedBoneName);

            UpdateBoneOptions();
        }
    }

    public override object GetNodeData()
    {
        return new VFXNodeData
        {
            Id = Id,
            Position = GetPosition().position,
            DataType = new SerializableSystemType(typeof(VFXNodeData)),
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames(),
            Duration = duration,
            VFX = selectedVFX,
            Prefab = selectedPrefab,
            TransformName = selectedBoneName,
        };
    }
    #endregion
}
#endif

public class RuntimeVFXNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        var vfxNodeData = (VFXNodeData)nodeData;

        if (vfxNodeData.VFX == null || vfxNodeData.Prefab == null) return null;

        GameObject[] objectsWithVFXTag = GameObject.FindGameObjectsWithTag("VFXTransform");
        Transform bone = objectsWithVFXTag.FirstOrDefault(obj => obj != null && obj.name == vfxNodeData.TransformName)?.transform;

        if (bone == null) return null;

        GameObject instantiatedPrefab = Object.Instantiate(vfxNodeData.VFX as GameObject, bone.position, bone.rotation);
        instantiatedPrefab.transform.SetParent(bone);
        Object.Destroy(instantiatedPrefab, vfxNodeData.Duration);

        return null;
    }
}