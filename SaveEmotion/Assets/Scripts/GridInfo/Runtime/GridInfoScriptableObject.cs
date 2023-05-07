using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GridInfoScriptableObject", order = 1)]
public class GridInfoScriptableObject : ScriptableObject
{
    public string prefabName;

    public int numberOfPrefabsToCreate;
    public SingleGridInfo[] gridInfoList;
}


public enum GridType
{
    // Ball not pass
    Disable = 0,
    // Ball pass
    Enable = 1,
    // Ice Block
    Locked = 2,
    // Barrier
    Inactive = 3,
}

public class SingleGridInfo : ScriptableObject
{
    public GridType currType;

    public SingleGridInfo()
    {
        currType = GridType.Disable;
    }
}