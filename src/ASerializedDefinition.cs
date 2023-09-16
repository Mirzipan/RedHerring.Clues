namespace RedHerring.Clues;

[Serializable]
public abstract class ASerializedDefinition
{
    public string PrimaryId;
    public string SecondaryId;
    
    public bool IsDefault;
}