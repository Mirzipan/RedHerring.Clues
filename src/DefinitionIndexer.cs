using System.Reflection;
using RedHerring.Deduction;
using RedHerring.Extensions.Reflection;

namespace RedHerring.Clues;

public class DefinitionIndexer : IIndexMetadata, IDisposable
{
    private static readonly Type DefinitionType = typeof(ADefinition);
    private static readonly Type SerializedDefinitionType = typeof(ASerializedDefinition);
    
    private readonly Dictionary<Type, Type> _serializedDefinitionToDefinitionMap = new();

    #region Lifecycle

    public void Index(Type type)
    {
        if (!type.IsSubclassOf(DefinitionType) || !type.HasDefaultConstructor())
        {
            return;
        }

        var attributes = type.GetCustomAttributes<DeserializedFromAttribute>();
        foreach (var attribute in attributes)
        {
            if (attribute.SerializedType is null)
            {
                continue;
            }

            if (!attribute.SerializedType.IsSubclassOf(SerializedDefinitionType))
            {
                continue;
            }

            _serializedDefinitionToDefinitionMap[attribute.SerializedType] = type;
        }
    }
    
    public void Dispose()
    {
        _serializedDefinitionToDefinitionMap.Clear();
    }

    #endregion Lifecycle

    #region Queries

    public Type GetDefinitionType(Type serializedType)
    {
        return _serializedDefinitionToDefinitionMap.TryGetValue(serializedType, out var result) ? result : null;
    }

    #endregion Queries
}