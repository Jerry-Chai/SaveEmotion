using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneController : MonoBehaviour
{
    public GameObject partA;
    public GameObject partB;
    public GameObject partC;
    public GameObject partD;
    public GameObject allLocked;

    public GameObject partALock;
    public GameObject partBLock;
    public GameObject partCLock;
    public GameObject partDLock;
    public GameObject hintObj;
    public GameObject hintCircle;
    public GameObject hintFinger;

    public Sprite lock_Off;
    public Sprite lock_On;

    public Image partALockImage;
    public Image partBLockImage;
    public Image partCLockImage;
    public Image partDLockImage;

    // Start is called before the first frame update
    void Start()
    {
        partA.GetComponent<Collider>().enabled = false;
        partB.GetComponent<Collider>().enabled = false;
        partC.GetComponent<Collider>().enabled = false;
        partD.GetComponent<Collider>().enabled = false;

        partALockImage = partALock.GetComponent<Image>();
        partBLockImage = partBLock.GetComponent<Image>();
        partCLockImage = partCLock.GetComponent<Image>();
        partDLockImage = partDLock.GetComponent<Image>();

        partALockImage.transform.DOScale(0.0f, 0.0f);
        partBLockImage.transform.DOScale(0.0f, 0.0f);
        partCLockImage.transform.DOScale(0.0f, 0.0f);
        partDLockImage.transform.DOScale(0.0f, 0.0f);

        partALock.SetActive(false);
        partBLock.SetActive(false);
        partCLock.SetActive(false);
        partDLock.SetActive(false);

        hintCircle = hintObj.transform.Find("HintCircle").gameObject;
        hintFinger = hintObj.transform.Find("HintFinger").gameObject;

        hintObj.SetActive(false);
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
        StartCoroutine(UnlockCertainBlock(allLocked, 6.0f, 500.0f, BompUpAllLock));
    }
    IEnumerator UnlockCertainBlock(GameObject sphere, float unlockTime, float destScale, Action callBack = null)
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
        if (callBack != null) 
        {
            callBack();
        }
    }

    private void BompUpAllLock()
    {
        partALock.SetActive(true);
        partBLock.SetActive(true);
        partCLock.SetActive(true);
        partDLock.SetActive(true);

        Sequence sequence = DOTween.Sequence();


        // 添加出现效果到序列中
        sequence.Append(partALockImage.transform.DOScale(1.25f, 0.5f));
        sequence.Append(partALockImage.transform.DOScale(1.2f, 0.5f));

        sequence.Append(partBLockImage.transform.DOScale(1.25f, 0.5f));
        sequence.Append(partBLockImage.transform.DOScale(1.2f, 0.5f));

        sequence.Append(partDLockImage.transform.DOScale(1.25f, 0.5f));
        sequence.Append(partDLockImage.transform.DOScale(1.2f, 0.5f));

        // 添加放大效果到序列中
        sequence.Append(partCLockImage.transform.DOScale(1.15f, 0.5f));
        sequence.Append(partCLockImage.transform.DOScale(1.1f, 0.5f));

        // 启动序列
        sequence.Play();

    }

    public void ShowHint()
    {
        hintObj.SetActive(true);
        //hintCircle.transform.DOScale(1.0f, 0.5f).SetLoops(-1, LoopType.Restart);
        //hintFinger.transform.DOLocalMoveY(-100.0f, 0.5f).SetLoops(-1, LoopType.Yoyo);
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

        if (GUILayout.Button("第一步 ———— 上锁所有地块，并出现锁"))
        {
            myScript.LockAll();

        }

        if (GUILayout.Button("第一步 ———— 上锁所有地块"))
        {
            myScript.LockAll();
        }


        GUILayout.Space(30);
        GUILayout.Space(30);

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