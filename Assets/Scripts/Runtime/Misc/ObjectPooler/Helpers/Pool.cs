using UnityEngine;
using static Pool;

[System.Serializable]
public class Pool
{
    public GameObject prefab;
    public PoolTag tag;
    public int size;
}

public enum PoolTag
{
    Cell,
    Enemy1,
    Enemy2,
    Enemy3,
    Player,
}