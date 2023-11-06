using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EnterSubScene : MonoBehaviour, IPointerClickHandler
{

    public string EnterSubSceneName;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("RawImage被点击了！");
        SceneManager.LoadScene(EnterSubSceneName);
        // 在这里添加处理点击事件的逻辑
    }
}
