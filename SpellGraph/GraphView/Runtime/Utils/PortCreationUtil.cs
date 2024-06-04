#if UNITY_EDITOR
using System.Reflection;

/// <summary>
/// Utility class responsible of creating the ports of a node when it is instantiated.
/// Uses reflection to get all of the input/output ports for each node type.
/// </summary>

public static class PortCreationUtil
{
    public static void CreateInputPortsFromAttributes(this ExtendedNode node)
    {
        var type = node.GetType();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            var inputPortAttribute = field.GetCustomAttribute<InputPortAttribute>();
            if (inputPortAttribute != null)
            {
                var fieldName = string.IsNullOrEmpty(inputPortAttribute.displayName) ? field.Name : inputPortAttribute.displayName;
                var fieldType = field.FieldType;
                node.SetInputPort(fieldName, fieldType, inputPortAttribute.capacity);
            }
        }

        foreach (var property in properties)
        {
            var inputPortAttribute = property.GetCustomAttribute<InputPortAttribute>();
            if (inputPortAttribute != null)
            {
                var propertyName = string.IsNullOrEmpty(inputPortAttribute.displayName) ? property.Name : inputPortAttribute.displayName;
                var propertyType = property.PropertyType;
                node.SetInputPort(propertyName, propertyType, inputPortAttribute.capacity);
            }
        }
    }

    public static void CreateOutputPortsFromAttributes(this ExtendedNode node)
    {
        var type = node.GetType();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            var outputPortAttribute = field.GetCustomAttribute<OutputPortAttribute>();
            if (outputPortAttribute != null)
            {
                var fieldName = string.IsNullOrEmpty(outputPortAttribute.displayName) ? field.Name : outputPortAttribute.displayName;
                var fieldType = field.FieldType;
                node.SetOutputPort(fieldName, fieldType, outputPortAttribute.capacity);
            }
        }

        foreach (var property in properties)
        {
            var outputPortAttribute = property.GetCustomAttribute<OutputPortAttribute>();
            if (outputPortAttribute != null)
            {
                var propertyName = string.IsNullOrEmpty(outputPortAttribute.displayName) ? property.Name : outputPortAttribute.displayName;
                var propertyType = property.PropertyType;
                node.SetOutputPort(propertyName, propertyType, outputPortAttribute.capacity);
            }
        }
    }
}
#endif