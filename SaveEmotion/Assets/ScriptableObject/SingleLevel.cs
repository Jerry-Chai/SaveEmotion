using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SingleLevel", menuName = "ScriptableObjects/SingleLevel", order = 1)]
public class SingleLevel : ScriptableObject
{
    /// <summary>
    /// �ؿ�����
    /// </summary>
    public string name;
    /// <summary>
    /// ��Ϸʱ��
    /// </summary>
    public int gameTime = 90;

    SingleLevel()
    {
    }
}
