//https://www.patrykgalach.com/2019/04/01/how-to-implement-object-pooling-in-unity/
/// <summary>
/// Base interface for object pool used in poolable objects
/// </summary>
public interface IObjectPool
{
    void ReturnToPool(object instance);
}

/// <summary>
/// Generic interface with more methods for object pooling
/// </summary>
public interface IObjectPool<T> : IObjectPool where T : IPoolable
{
    T GetPrefabInstance();
    void ReturnToPool(T instance);
}