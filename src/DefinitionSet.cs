using System.Reflection;
using RedHerring.Alexandria.Identifiers;

namespace RedHerring.Clues;

public sealed class DefinitionSet : IDisposable
{
    private readonly Dictionary<Type, Dictionary<ulong, ADefinition>> _data = new();
    
    #region Lifecycle

    public void Dispose()
    {
        Clear();
    }

    #endregion Lifecycle
    
    #region Manipulation
        
    /// <summary>
    /// Adds the specified definition. In case there already is one with the same indexed type and id, this will overwrite it
    /// </summary>
    /// <param name="definition"></param>
    public void Add(ADefinition definition)
    {
        Type type = definition.GetType();
        AddDefinition(definition, type);
                
        var attributes = type.GetCustomAttributes<DefinitionTypeAttribute>();
        foreach (var attribute in attributes)
        {
            if (attribute.IndexedType != null)
            {
                AddDefinition(definition, attribute.IndexedType);
            }
        }
    }

    /// <summary>
    /// Removes the specified definition.
    /// </summary>
    /// <param name="definition"></param>
    /// <returns></returns>
    public bool Remove(ADefinition definition)
    {
        Type type = definition.GetType();
        bool result = RemoveDefinition(definition, type);
            
        var attributes = type.GetCustomAttributes<DefinitionTypeAttribute>();
        foreach (var attribute in attributes)
        {
            if (attribute.IndexedType != null)
            {
                result |= RemoveDefinition(definition, attribute.IndexedType);
            }
        }

        return result;
    }

    public void Clear()
    {
        var definitionsByType = _data.Values.ToList();
        foreach (var definitionMap in definitionsByType)
        {
            definitionMap.Clear();
        }

        _data.Clear();
    }

    #endregion Manipulation

    #region Queries

    public IEnumerable<ADefinition> GetAll()
    {
        foreach (var outerPair in _data)
        {
            foreach (var innerPair in outerPair.Value)
            {
                yield return innerPair.Value;
            }
        }
    }

    /// <summary>
    /// Returns all definitions of a given type.
    /// </summary>
    /// <typeparam name="T">Definition type</typeparam>
    /// <returns>Definition of type, if found, null otherwise</returns>
    public IEnumerable<T> GetAll<T>() where T : ADefinition
    {
        var collection = GetOrCreateCollection(typeof(T));
        if (collection is null)
        {
            yield break;
        }

        foreach (var definition in collection)
        {
            yield return (T) definition.Value;
        }
    }

    /// <summary>
    /// Returns a definition with the given id.
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T">Definition type</typeparam>
    /// <returns>Definition of type and id, if found, null otherwise</returns>
    public T Get<T>(CompositeId id) where T : ADefinition
    {
        Type type = typeof(T);
        var collection = GetOrCreateCollection(type);
        if (collection is null)
        {
            return null;
        }

        collection.TryGetValue(id.Value, out var result);
        return (T) result;
    }

    /// <summary>
    /// Returns true if definition exists.
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool Contains<T>(CompositeId id) where T : ADefinition
    {
        Type type = typeof(T);
        return GetOrCreateCollection(type)?.ContainsKey(id.Value) ?? false;
    }

    #endregion Queries
    
    #region Private

    private Dictionary<ulong, ADefinition> GetOrCreateCollection(Type collectionType, bool canCreate = false)
    {
        if (!_data.TryGetValue(collectionType, out var innerDefinitions))
        {
            return _data[collectionType] = new Dictionary<ulong, ADefinition>();
        }

        return innerDefinitions;
    }
        
    private void AddDefinition(ADefinition definition, Type type)
    {
        GetOrCreateCollection(type, true).Add(definition.Id.Value, definition);
    }

    private bool RemoveDefinition(ADefinition definition, Type type)
    {
        return GetOrCreateCollection(type)?.Remove(definition.Id.Value) ?? false;
    }

    #endregion Private
}