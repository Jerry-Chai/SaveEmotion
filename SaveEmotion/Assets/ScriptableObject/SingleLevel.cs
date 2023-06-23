using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SingleLevel", menuName = "ScriptableObjects/SingleLevel", order = 1)]
public class SingleLevel : ScriptableObject
{
    /// <summary>
    /// 关卡名字
    /// </summary>
    public string name;
    /// <summary>
    /// 游戏时长
    /// </summary>
    public int gameTime = 90;

    SingleLevel()
    {
    }
}
