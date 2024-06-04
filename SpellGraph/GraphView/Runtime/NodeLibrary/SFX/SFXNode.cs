using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;
using System.Linq;

public class SFXNodeData : NodeData
{
    public List<AudioClipSettings> AudioSettings;
}

[Serializable]
public class AudioClipSettings
{
    public AudioClip audioClip;
    public Vector2 pitchVariation;
    public float volume;
}

#if UNITY_EDITOR
[NodeInfo("SFX", "SFX", "SFX Manager")]
public class SFXNode : ExtendedNode
{
    private List<AudioClipSettings> audioClips = new List<AudioClipSettings>();
    private Foldout audioClipsFoldout = new Foldout { text = "Audio Clips" };

    [InputPort("In")]
    public Trigger Input;

    [OutputPort("Out")]
    public Trigger Output;

    public SFXNode()
    {
        var addButton = new Button(() => AddAudioClip(audioClipsFoldout)) { text = "Add" };
        audioClipsFoldout.Add(addButton);
        extensionContainer.Add(audioClipsFoldout);
    }

    private void AddAudioClip(Foldout audioClipsFoldout)
    {
        var clipSettings = new AudioClipSettings();
        clipSettings.volume = 100;
        audioClips.Add(clipSettings);

        var clipFoldout = CreateAudioClipFoldout(clipSettings);
        audioClipsFoldout.Add(clipFoldout);
    }

    private Foldout CreateAudioClipFoldout(AudioClipSettings clipSettings)
    {
        var clipFoldout = new Foldout { text = "SFX" };

        var audioClipField = new UnityEditor.UIElements.ObjectField("Audio Clip") { objectType = typeof(AudioClip) };
        audioClipField.style.width = 250;
        clipFoldout.Add(audioClipField);

        var pitchField = new Vector2Field("Pitch Variation");
        pitchField.style.width = 315;
        pitchField.style.marginRight = -100;
        clipFoldout.Add(pitchField);

        var volumeSlider = new Slider(0, 100) { label = "Volume", value = 100 };
        volumeSlider.style.width = 250;
        clipFoldout.Add(volumeSlider);

        var removeButton = new Button(() => RemoveAudioClip(audioClipsFoldout, clipFoldout)) { text = "Remove" };
        clipFoldout.Add(removeButton);

        audioClipField.value = clipSettings.audioClip;
        pitchField.value = clipSettings.pitchVariation;
        volumeSlider.value = clipSettings.volume;

        audioClipField.RegisterValueChangedCallback(evt =>
        {
            clipSettings.audioClip = evt.newValue as AudioClip;
        });

        pitchField.RegisterValueChangedCallback(evt =>
        {
            clipSettings.pitchVariation = evt.newValue;
        });

        volumeSlider.RegisterValueChangedCallback(evt =>
        {
            clipSettings.volume = evt.newValue;
        });

        return clipFoldout;
    }

    private void RemoveAudioClip(Foldout audioClipsFoldout, Foldout clipFoldout)
    {
        int indexToRemove = audioClipsFoldout.IndexOf(clipFoldout) - 1;
        if (indexToRemove >= 0 && indexToRemove < audioClips.Count)
        {
            audioClips.RemoveAt(indexToRemove);
            audioClipsFoldout.Remove(clipFoldout);
        }
    }

    #region INodeData
    public override void SetValues(NodeData data)
    {
        if (data is SFXNodeData nodeData)
        {
            audioClips = nodeData.AudioSettings;

            audioClipsFoldout.Clear();

            var addButton = new Button(() => AddAudioClip(audioClipsFoldout)) { text = "Add" };
            audioClipsFoldout.Add(addButton);

            foreach (var clipSettings in nodeData.AudioSettings)
            {
                var clipFoldout = CreateAudioClipFoldout(clipSettings);
                audioClipsFoldout.Add(clipFoldout);
            }
        }
    }

    public override object GetNodeData()
    {
        return new SFXNodeData
        {
            Id = Id,
            Position = GetPosition().position,
            DataType = new SerializableSystemType(typeof(SFXNodeData)),
            EditorType = new SerializableSystemType(GetType()),
            RuntimeType = new SerializableSystemType(GetRuntimeType()),
            InputPortNames = GetInputPortNames(),
            OutputPortNames = GetOutputPortNames(),
            AudioSettings = audioClips.Select(settings => new AudioClipSettings
            {
                audioClip = settings.audioClip,
                pitchVariation = settings.pitchVariation,
                volume = settings.volume
            }).ToList()
        };
    }
    #endregion
}
#endif

public class RuntimeSFXNode : ActionNode
{
    public override object Action(List<object> portValues, NodeData nodeData)
    {
        //var sfxNodeData = (SFXNodeData)nodeData;
        //SFXManager.Instance.PlayAudioClip(sfxNodeData.AudioSettings);
        return null;
    }
}