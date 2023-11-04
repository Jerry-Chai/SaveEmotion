using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StartSceneController : MonoBehaviour
{
    public GameObject partA;
    public GameObject partB;
    public GameObject partC;
    public GameObject partD;
    public GameObject allLocked;
    // Start is called before the first frame update
    void Start()
    {
        partA.GetComponent<Collider>().enabled = false;
        partB.GetComponent<Collider>().enabled = false;
        partC.GetComponent<Collider>().enabled = false;
        partD.GetComponent<Collider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnlockA()
    {
        StartCoroutine(UnlockCertainBlock(partA, 2.0f, 200.0f));
    }

    public void OnlockB()
    {
        StartCoroutine(UnlockCertainBlock(partB, 2.0f, 200.0f));
    }

    public void OnlockC()
    {
        StartCoroutine(UnlockCertainBlock(partC, 2.0f, 200.0f));
    }

    public void OnlockD()
    {
        StartCoroutine(UnlockCertainBlock(partD, 2.0f, 200.0f));
    }    
    
    public void LockAll()
    {
        StartCoroutine(UnlockCertainBlock(allLocked, 6.0f, 500.0f));
    }
    IEnumerator UnlockCertainBlock(GameObject sphere, float unlockTime, float destScale)
    {
        sphere.GetComponent<Collider>().enabled = true;
        float count = 0.0f;
        while (count < 1.0f) 
        {
            count += Time.deltaTime / unlockTime;
            float scale = Mathf.Lerp(1.0f, destScale, count);
            sphere.transform.localScale = scale * Vector3.one;
            yield return null;
        }
        yield return null;

    }
}



//public class StartSceneController
[CustomEditor(typeof(StartSceneController))]
public class StartSceneControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        StartSceneController myScript = target as StartSceneController;
        if (GUILayout.Button("Unlock A"))
        {
            myScript.OnlockA();
        }

        if (GUILayout.Button("Unlock B"))
        {
            myScript.OnlockB();
        }

        if (GUILayout.Button("Unlock C"))
        {
            myScript.OnlockC();
        }

        if (GUILayout.Button("Unlock D"))
        {
            myScript.OnlockD();
        }

        if (GUILayout.Button("Lock All"))
        {
            myScript.LockAll();
        }
    }

}