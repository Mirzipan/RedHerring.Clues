using RedHerring.Alexandria.Identifiers;

namespace RedHerring.Clues;

public sealed class Definitions : IDefinitions, IDisposable
{
    internal readonly DefinitionSet _data = new();
    internal readonly Dictionary<Type, ADefinition> _defaults = new();

    #region Lifecycle

    public void Load(ILoadDefinitions loader)
    {
        loader.Process(_data);

        PopulateDefaults();
    }

    public void Dispose()
    {
        _data.Clear();
        _defaults.Clear();
    }

    #endregion Lifecycle

    #region Queries

    /// <summary>
    /// Returns all definitions of a given type.
    /// </summary>
    /// <typeparam name="T">Definition type</typeparam>
    /// <returns>Definition of type, if found, null otherwise</returns>
    public IEnumerable<T> GetAll<T>() where T : ADefinition
    {
        return _data.GetAll<T>();
    }

    /// <summary>
    /// Returns a definition with the given id.
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T">Definition type</typeparam>
    /// <returns>Definition of type and id, if found, null otherwise</returns>
    public T Get<T>(CompositeId id) where T : ADefinition
    {
        return _data.Get<T>(id);
    }

    /// <summary>
    /// Returns true if definition exists and assigns it to `definition`.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="definition"></param>
    /// <typeparam name="T">Definition type</typeparam>
    /// <returns>True if definition exists</returns>
    public bool TryGet<T>(CompositeId id, out T definition) where T : ADefinition
    {
        return (definition = _data.Get<T>(id)) is not null;
    }

    /// <summary>
    /// Returns true if definition exists.
    /// </summary>
    /// <param name="id"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool Contains<T>(CompositeId id) where T : ADefinition
    {
        return _data.Contains<T>(id);
    }

    /// <summary>
    /// Returns the default definition for the type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T Default<T>() where T : ADefinition
    {
        Type type = typeof(T);
        return _defaults.TryGetValue(type, out var result) ? (T)result : null;
    }

    #endregion Queries

    #region Private

    private void PopulateDefaults()
    {
        foreach (var entry in _data.GetAll())
        {
            if (!entry.IsDefault)
            {
                continue;
            }

            var type = entry.GetType();
            _defaults[type] = entry;
        }
    }

    #endregion Private
}