using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Helper classes that are used throughout the whole SpellGraph ecosystem.
/// </summary>

public class Trigger
{
    public string Name { get; private set; }

    public Trigger(string name)
    {
        Name = name;
    }
}

public class Generic
{

}

public interface INodeData
{
    public object GetNodeData();
    public void SetValues(NodeData data);
    public object DeserializeJson(string data);
}

public class RuntimeNode
{
    protected T GetPortValue<T>(object value, T defaultValue = default) where T : struct
    {
        if (value == null)
        {
            return defaultValue;
        }

        Type targetType = typeof(T);

        if (value.GetType() == targetType)
        {
            return (T)value;
        }

        if (targetType == typeof(int))
        {
            if (value is float floatValue)
            {
                return (T)Convert.ChangeType(Mathf.RoundToInt(floatValue), typeof(T));
            }
            else if (value is double doubleValue)
            {
                return (T)Convert.ChangeType((int)Math.Round(doubleValue), typeof(T));
            }
            else
            {
                throw new InvalidCastException($"Cannot convert value of type {value.GetType()} to int.");
            }
        }
        else if (targetType == typeof(float))
        {
            if (value is int intValue)
            {
                return (T)Convert.ChangeType(intValue, typeof(T));
            }
            else if (value is double doubleValue)
            {
                return (T)Convert.ChangeType((float)doubleValue, typeof(T));
            }
            else
            {
                throw new InvalidCastException($"Cannot convert value of type {value.GetType()} to float.");
            }
        }
        else if (targetType == typeof(string))
        {
            return (T)Convert.ChangeType(value.ToString(), typeof(T));
        }
        else if (targetType == typeof(bool))
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        else if (targetType == typeof(Vector2))
        {
            if (value is Vector2 vector2Value)
            {
                return (T)Convert.ChangeType(vector2Value, typeof(T));
            }
            else
            {
                throw new InvalidCastException($"Cannot convert value of type {value.GetType()} to Vector2.");
            }
        }
        else if (targetType == typeof(Vector3))
        {
            if (value is Vector3 vector3Value)
            {
                return (T)Convert.ChangeType(vector3Value, typeof(T));
            }
            else
            {
                throw new InvalidCastException($"Cannot convert value of type {value.GetType()} to Vector3.");
            }
        }
        else if (targetType == typeof(Vector4))
        {
            if (value is Vector4 vector4Value)
            {
                return (T)Convert.ChangeType(vector4Value, typeof(T));
            }
            else
            {
                throw new InvalidCastException($"Cannot convert value of type {value.GetType()} to Vector4.");
            }
        }
        else if (targetType == typeof(GameObject))
        {
            if (value is GameObject gameObjectValue)
            {
                return (T)Convert.ChangeType(gameObjectValue, typeof(T));
            }
            else
            {
                throw new InvalidCastException($"Cannot convert value of type {value.GetType()} to GameObject.");
            }
        }
        else
        {
            throw new InvalidCastException($"Unsupported target type: {targetType}");
        }
    }
}

public abstract class StateNode : RuntimeNode
{
    public virtual async Task<object> State(List<object> portValues, NodeData nodeData) { return Task.FromResult<object>(null); }
}

public abstract class ActionNode : RuntimeNode
{
    public abstract object Action(List<object> portValues, NodeData nodeData);
}

public abstract class ConditionNode : RuntimeNode
{
    public abstract Trigger Condition(List<object> portValues, NodeData nodeData);
}

public abstract class EventNode : RuntimeNode
{
    public abstract void Subscribe(List<object> portValues, NodeData nodeData);
    protected abstract void Event();
}