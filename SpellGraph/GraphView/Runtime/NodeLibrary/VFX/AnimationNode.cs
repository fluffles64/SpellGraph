using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationNodeData : NodeData
{
    public float Duration;
    public Object Controller;
    public string AnimationName;
}

#if UNITY_EDITOR
[NodeInfo("Animation", "VFX", "Overwrites the current animation and plays one shot of the selected animation")]
public class AnimationNode : ExtendedNode
{
    private float duration;
    private Object selectedAnimatorController;
    private string selectedAnimationName = "None";
    private string[] animationNames = { };

    [InputPort("In")]
    public Trigger Input;

    [OutputPort("Out")]
    public Trigger Output;

    public AnimationNode()
    {
        var durationField = new FloatField("Duration") { style = { width = 250 } };
        durationField.RegisterValueChangedCallback(e => duration = e.newValue);
        extensionContainer.Add(durationField);

        var controllerField = new UnityEditor.UIElements.ObjectField("Controller") { objectType = typeof(Object), style = { width = 250 } };
        controllerField.RegisterValueChangedCallback(e => { selectedAnimatorController = e.newValue; UpdateDropdownOptions(); });
        extensionContainer.Add(controllerField);

        VisualElement dropdownContainer = DropdownContainer("Animation", selectedAnimationName, animationNames, newValue => selectedAnimationName = newValue);
        extensionContainer.Add(dropdownContainer);
    }

    private void UpdateDropdownOptions()
    {
        animationNames = new string[0];

        if (selectedAnimatorController != null && selectedAnimatorController is Object)
        {
            GameObject prefab = selectedAnimatorController as GameObject;

            if (prefab != null)
            {
                GameObject instance = GameObject.Instantiate(prefab);
                var clips = instance.GetComponent<Animator>().runtimeAnimatorController.animationClips;
                List<string> names = new List<string>();
                foreach (var item in clips)
                    names.Add(item.name);

                if (names.Count == 0)
                    selectedAnimationName = "None";

                GameObject.DestroyImmediate(instance);
                animationNames = names.ToArray();
            }
        }

        DropdownField dropdown = extensionContainer.Q<DropdownField>();
        dropdown.choices = new List<string>(animationNames);
        dropdown.SetValueWithoutNotify(selectedAnimationName);
    }

    #region INodeData
    public override void SetValues(NodeData data)
    {
        if (data is AnimationNodeData nodeData)
        {
            duration = nodeData.Duration;
            selectedAnimatorController = nodeData.Controller;
            selectedAnimationName = nodeData.AnimationName;
        }

        var durationField = extensionContainer.Q<FloatField>();
        durationField.SetValueWithoutNotify(duration);

        var controllerField = extensionContainer.Q<UnityEditor.UIElements.ObjectField>();
        controllerField.SetValueWithoutNotify(selectedAnimatorController);
        controllerField.RegisterValueChangedCallback(e => selectedAnimatorController = e.newValue);

        DropdownField dropdown = extensionContainer.Q<DropdownField>();
        dropdown?.SetValueWithoutNotify(selectedAnimationName);

        UpdateDropdownOptions();
    }

    public override object GetNodeData()
    {
        return new AnimationNodeData
        {
            Id = Id,
            Position = GetPosition().position,
            DataType = new SerializableSystemType(typeof(AnimationNodeData)),
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames(),
            Duration = duration,
            Controller = selectedAnimatorController,
            AnimationName = selectedAnimationName,
        };
    }
    #endregion
}
#endif

public class RuntimeAnimationNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        //if(nodeData is AnimationNodeData animationNodeData && TargetManager.Instance.Player != null)
        //{
        //    float duration = animationNodeData.Duration;
        //    Object controller = animationNodeData.Controller;

        //    var animator = TargetManager.Instance.Player.GetComponent<Animator>();
        //    var clips = animator.runtimeAnimatorController.animationClips;

        //    var a = clips.FirstOrDefault(obj => obj != null && obj.name == animationNodeData.AnimationName);

        //    if (a != null)
        //    {
        //        animator.Play(a.name);
        //        Debug.Log(a.name);
        //        AnimationManager.Instance.StartCoroutine(WaitForAnimationEnd(animator, duration));
        //    }

        //    return null;
        //}
        return null;
    }

    /*private IEnumerator WaitForAnimationEnd(Animator animator, float duration)
    {
        Debug.Log("A");
        // Wait for the duration of the one-shot animation
        yield return new WaitForSeconds(duration);

        // Resume the normal animation
        animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0);
    }*/
}