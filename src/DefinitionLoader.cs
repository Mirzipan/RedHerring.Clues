using RedHerring.Extensions.Collections;

namespace RedHerring.Clues;

public class DefinitionLoader : ILoadDefinitions, IDisposable
{
    private List<ASerializedDefinition> _serializedData = new();
    private DefinitionIndexer _indexer;
    
    #region Lifecycle

    public DefinitionLoader(DefinitionIndexer indexer)
    {
        _indexer = indexer;
    }

    public void Dispose()
    {
        _serializedData.Clear();
        _indexer = null;
    }

    #endregion Lifecycle

    #region ILoadDefinition

    public void AddSerialized(ASerializedDefinition definition) => _serializedData.Add(definition);
    public void AddSerialized(IEnumerable<ASerializedDefinition> definitions) => _serializedData.AddRange(definitions);

    public void Process(DefinitionSet set)
    {
        if (_serializedData.IsNullOrEmpty())
        {
            return;
        }

        for (int i = 0; i < _serializedData.Count; i++)
        {
            var entry = _serializedData[i];
            var instance = CreateInstance(entry.GetType());
            try
            {
                instance.Init(entry);
            }
            catch (FailedToInitializeDefinitionException de)
            {
                // TODO: logging
                continue;
            }
            catch (Exception e)
            {
                // TODO: logging
                continue;
            }
            
            set.Add(instance);
        }
        
        _serializedData.Clear();
    }

    #endregion ILoadDefinition

    #region Private

    private ADefinition CreateInstance(Type serializedType)
    {
        var type = _indexer.GetDefinitionType(serializedType);
        if (type is null)
        {
            return null;
        }

        return Activator.CreateInstance(type) as ADefinition;
    }

    #endregion Private
}