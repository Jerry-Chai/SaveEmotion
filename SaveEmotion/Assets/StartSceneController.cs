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
    public GameObject hintObjC;
    public GameObject hintObjB;
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

        hintCircle = hintObjC.transform.Find("HintCircle").gameObject;
        hintFinger = hintObjC.transform.Find("HintFinger").gameObject;        
        
        hintCircle = hintObjB.transform.Find("HintCircle").gameObject;
        hintFinger = hintObjB.transform.Find("HintFinger").gameObject;

        hintObjC.SetActive(false);
        hintObjB.SetActive(false);
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
    
    public void ChangeAreaCLock()
    {
        Sequence sequence = DOTween.Sequence();


        sequence.Append(partCLockImage.transform.DOScale(0.001f, 0.5f))
            .AppendCallback(() => {
            // 缩放动画完成后，这里会执行你的回调逻辑
            partCLockImage.sprite = lock_Off;
            });
        sequence.Append(partCLockImage.transform.DOScale(1.15f, 0.5f));
        sequence.Append(partCLockImage.transform.DOScale(1.1f, 0.5f))
            .AppendCallback(() => {
                // 缩放动画完成后，这里会执行你的回调逻辑
                hintObjC.SetActive(true);
                hintObjC.GetComponent<Animator>().SetTrigger("xxx");
            });;
        
        // ????????
        sequence.Play();
        
    }
    
    public void AreaCUnLock()
    {
        Sequence sequence = DOTween.Sequence();


        // sequence.Append(partCLockImage.transform.DOScale(0.001f, 0.5f))
        //     .AppendCallback(() => {
        //         // 缩放动画完成后，这里会执行你的回调逻辑
        //         partCLockImage.sprite = lock_Off;
        //     });
        sequence.Append(partCLockImage.transform.DOScale(0.01f, 0.5f))
            .AppendCallback(() => {
                // 缩放动画完成后，这里会执行你的回调逻辑
                hintObjC.SetActive(false);
                OnlockC();
            });;
        
        sequence.Play();
    }
    
    public void ChangeAreaBLock()
    {
        Sequence sequence = DOTween.Sequence();


        sequence.Append(partBLockImage.transform.DOScale(0.001f, 0.5f))
            .AppendCallback(() => {
                // 缩放动画完成后，这里会执行你的回调逻辑
                partBLockImage.sprite = lock_Off;
            });
        sequence.Append(partBLockImage.transform.DOScale(1.15f, 0.5f));
        sequence.Append(partBLockImage.transform.DOScale(1.1f, 0.5f))
            .AppendCallback(() => {
                // 缩放动画完成后，这里会执行你的回调逻辑
                hintObjB.SetActive(true);
                hintObjB.GetComponent<Animator>().SetTrigger("xxx");
            });;
        
        // ????????
        sequence.Play();
        
    }
    
    public void AreaBUnLock()
    {
        Sequence sequence = DOTween.Sequence();


        // sequence.Append(partCLockImage.transform.DOScale(0.001f, 0.5f))
        //     .AppendCallback(() => {
        //         // 缩放动画完成后，这里会执行你的回调逻辑
        //         partCLockImage.sprite = lock_Off;
        //     });
        sequence.Append(partBLockImage.transform.DOScale(0.01f, 0.5f))
            .AppendCallback(() => {
                // 缩放动画完成后，这里会执行你的回调逻辑
                hintObjB.SetActive(false);
                OnlockB();
            });;
        
        sequence.Play();  
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


        // ??????Ч??????????
        sequence.Append(partALockImage.transform.DOScale(1.25f, 0.5f));
        sequence.Append(partALockImage.transform.DOScale(1.2f, 0.5f));

        sequence.Append(partBLockImage.transform.DOScale(1.25f, 0.5f));
        sequence.Append(partBLockImage.transform.DOScale(1.2f, 0.5f));

        sequence.Append(partDLockImage.transform.DOScale(1.25f, 0.5f));
        sequence.Append(partDLockImage.transform.DOScale(1.2f, 0.5f));

        // ?????Ч??????????
        sequence.Append(partCLockImage.transform.DOScale(1.15f, 0.5f));
        sequence.Append(partCLockImage.transform.DOScale(1.1f, 0.5f));

        // ????????
        sequence.Play();

    }

    public void ShowHint()
    {
        hintObjC.SetActive(true);
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

        if (GUILayout.Button("Lock Area then show locks"))
        {
            myScript.LockAll();

        }
        
        if (GUILayout.Button("First Area Clickable"))
        {
            myScript.ChangeAreaCLock();

        }


        if (GUILayout.Button("First Area Unlock"))
        {
            myScript.AreaCUnLock();
        }
        
        
        if (GUILayout.Button("Second Area Clickable"))
        {
            myScript.ChangeAreaBLock();

        }


        if (GUILayout.Button("Second Area Unlock"))
        {
            myScript.AreaBUnLock();
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