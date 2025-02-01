using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

public static class StaticExtensions
{  
    
    public static void CheckPublicMembersForNull_Node<ExcludeType, T>(this T node) 
        where ExcludeType : Node
        where T: ExcludeType
    {       
        GD.Print($"Вызвана проверка для {node.Name} типа {node.GetType()}");
		Type type = typeof(T);
        PropertyInfo[] allProperties = type.GetProperties();

        PropertyInfo[] propertiesToExclude = typeof(ExcludeType).GetProperties();

        IEnumerable<PropertyInfo> properties = allProperties.Where(sample => !propertiesToExclude.Any(exclude => exclude.Name == sample.Name));

        foreach (PropertyInfo property in properties)
        {
			if (property.GetValue(node) == null || IsFloatAndEqualsZero(property, node)) {
				GD.PushWarning($"В {node.Name} типа {type} свойство {property.Name} является null");
			}
        }

        foreach (FieldInfo field in type.GetFields())
        {
            if (field.GetValue(node) == null || IsFloatAndEqualsZero(field, node)){
				GD.PushWarning($"В {node.Name} типа {type} поле {field.Name} является null");
			}
        }
	}
    
    public static void CheckPublicMembersForNull_GodotObject<ExcludeType, T>(T godotObject) 
        where ExcludeType : GodotObject
        where T: ExcludeType
	{       
		Type type = typeof(T);
        PropertyInfo[] allProperties = type.GetProperties();

        PropertyInfo[] propertiesToExclude = typeof(ExcludeType).GetProperties();

        IEnumerable<PropertyInfo> properties = allProperties.Where(sample => !propertiesToExclude.Any(exclude => exclude.Name == sample.Name));

        foreach (PropertyInfo property in properties)
        {
			if (property.GetValue(godotObject) == null || IsFloatAndEqualsZero(property, godotObject)) {
				GD.PushWarning($"В {godotObject.ToString()} типа {type} свойство {property.Name} является null");
			}
        }

        foreach (FieldInfo field in type.GetFields())
        {
            if (field.GetValue(godotObject) == null || IsFloatAndEqualsZero(field, godotObject)){
				GD.PushWarning($"В {godotObject.ToString()} типа {type} поле {field.Name} является null");
			}
        }
	}

    private static bool IsFloatAndEqualsZero(PropertyInfo pInfo, GodotObject godotObject){
        return (pInfo.PropertyType == typeof(float) && (float)pInfo.GetValue(godotObject) == 0.0f);
    }

    private static bool IsFloatAndEqualsZero(FieldInfo fInfo, GodotObject godotObject){
        return (fInfo.FieldType == typeof(float) && (float)fInfo.GetValue(godotObject) == 0.0f);
    }

    /* ChatGPT's suggested code for debugging itself
    public static void CheckFieldsForNull<ExcludeType, T>(this T node)
        where ExcludeType : Node
        where T : Node2D
    {
        // Проверка на тип Node2D
        if (typeof(T) == typeof(Node2D))
            throw new InvalidOperationException("Node2D is not allowed as a generic type parameter.");

        // Получаем все свойства узла
        Type type = typeof(T);
        PropertyInfo[] allProperties = type.GetProperties();

        // Получаем свойства для исключения
        PropertyInfo[] propertiesToExclude = typeof(ExcludeType).GetProperties();

        GD.Print($"Checking node of type: {type.Name}");
        GD.Print($"All properties count: {allProperties.Length}");
        GD.Print($"Excluded properties count: {propertiesToExclude.Length}");

        // Исключаем свойства базового типа
        IEnumerable<PropertyInfo> filteredProperties = allProperties
            .Where(sample => !propertiesToExclude.Any(exclude => exclude.Name == sample.Name));

        GD.Print($"Filtered properties count: {filteredProperties.Count()}\n\n");

        // Проверяем каждое свойство
        foreach (PropertyInfo property in filteredProperties)
        {
            // Проверяем, можно ли читать свойство
            if (!property.CanRead)
            {
                GD.Print($"Skipping property {property.Name} (not readable).");
                continue;
            }

            try
            {
                object value = property.GetValue(node);

                if (value == null || IsFloatAndEqualsZero(property, value))
                {
                    GD.PushWarning($"Node '{node.Name}' of type '{type.Name}': Property '{property.Name}' is null or has invalid value.");
                }
            }
            catch (Exception ex)
            {
                GD.PushWarning($"Error checking property '{property.Name}' in node '{node.Name}': {ex.Message}");
            }
        }
    }

    private static bool IsFloatAndEqualsZero(PropertyInfo property, object value)
    {
        return property.PropertyType == typeof(float) && value is float floatValue && Math.Abs(floatValue) < float.Epsilon;
    }
*/
}
