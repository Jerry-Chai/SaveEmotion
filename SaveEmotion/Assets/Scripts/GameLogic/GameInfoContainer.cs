using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfoContainer : MonoBehaviour
{

    public LevelData levelData;
    // Start is called before the first frame update
    void Start()
    {
        if(levelData == null)
        {
            Debug.LogError("LevelData is null");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
