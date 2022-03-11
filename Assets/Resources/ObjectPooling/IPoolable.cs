﻿
/// <summary>
/// Interface for poolable objects
/// </summary>
public interface IPoolable
{
    IObjectPool Orgin { get; set; }
    void PrepareToUse();
    void ReturnToPool();
}
