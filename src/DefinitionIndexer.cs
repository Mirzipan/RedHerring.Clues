using System.Reflection;
using RedHerring.Extensions.Reflection;

namespace RedHerring.Clues;

public class DefinitionIndexer : IDisposable
{
    private static readonly Type DefinitionType = typeof(ADefinition);
    private static readonly Type SerializedDefinitionType = typeof(ASerializedDefinition);
    
    private readonly HashSet<Assembly> _knownAssemblies = new();
    private readonly Dictionary<Type, Type> _serializedDefinitionToDefinitionMap = new();

    public IReadOnlyCollection<Assembly> KnownAssemblies => _knownAssemblies;

    #region Lifecycle

    public void Index()
    {
        foreach (var type in GetAllTypes())
        {
            Index(type);
        }
    }
    
    public void Dispose()
    {
        _knownAssemblies.Clear();
        _serializedDefinitionToDefinitionMap.Clear();
    }

    #endregion Lifecycle

    #region Manipulation

    public void Add(IEnumerable<Assembly> assemblies)
    {
        foreach (Assembly entry in assemblies)
        {
            Add(entry);
        }
    }

    public void Add(params Assembly[] assemblies)
    {
        Add((IEnumerable<Assembly>)assemblies);
    }

    public void Add(Assembly assembly)
    {
        if (_knownAssemblies.Contains(assembly))
        {
            return;
        }

        _knownAssemblies.Add(assembly);
    }

    #endregion Manipulation

    #region Queries

    public Type GetDefinitionType(Type serializedType)
    {
        return _serializedDefinitionToDefinitionMap.TryGetValue(serializedType, out var result) ? result : null;
    }

    #endregion Queries

    #region Private

    private IEnumerable<Type> GetAllTypes()
    {
        foreach (var assembly in _knownAssemblies)
        {
            foreach (var type in assembly.GetTypes())
            {
                yield return type;
            }
        }
    }

    private void Index(Type type)
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

    #endregion Private
}