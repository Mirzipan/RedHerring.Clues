# Red Herring Clues

![Nuget](https://img.shields.io/nuget/v/RedHerring.Clues)

Simple definition system for your data-driven project.

## Accessing Definition

Use `Definitions` or `IDefinitions`, depending on how much access you need.

```csharp
IEnumerable<T> GetAll<T>() where T : ADefinition;
```
Returns all definitions of the given type.

```csharp
T Get<T>(CompositeId id) where T : ADefinition;
```
Returns the definition of the given type, with the specified `id`.

```csharp
bool TryGet<T>(CompositeId id, out T definition) where T : ADefinition;
```
Returns `true` if the definition of the given type, with the specified `id`, exists.

```csharp
bool Contains<T>(CompositeId id) where T : ADefinition;
```
Returns `true` if the definition of the given type, with the specified `id`, exists.

```csharp
T Default<T>() where T : ADefinition;
```
Returns the default definition of the given type. 
If no definition is marked as default, it will return `null`.

## Loading Definitions

To use Clues in your project, all you need to do is create an instance of `Definitions`. 
Loading and creation of definitions is up to you (you may want to deserialize from XML, JSON, etc.).
Once they are deserialized, you just need to feed them into the `Definitions` instance to 

```csharp
public class YourDefinitionLoaded : ILoadDefinitions
{
    // implementation of your loader   
}

var loader = new YourDefinitionLoader();
var definitions = new Definitions();
definitions.Load(loader);
```

There is currently no default loader implementation.

## CompositeId

Id that consists of 2 parts, `Primary` and `Secondary`, both of which are a `uint` internally. 
It is up to you, how you use this, even using all 8 bytes as a single `Id` is fine..
It comes with a property drawer, for easier editing.
Both parts of the id are visually represented by 4 characters, with values between `0-255`.

## A Definition

Basic definition type from which all of the other definitions need to inherit if `ADefinition`.
It contains some basic properties:
* **IsDefault** - It this the default definition for the type?
* **Id** - The `CompositeId` used to identify this definition.

You will need to override the `Init` method to create a game-ready definition from raw deserialized data.

## Definition Type Attribute

Attribute to use on your custom definition type, in order to have it registered as another definition type, in addition to its own.
The current types need to derived from the specified type.

