using AllIn1SpriteShader;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class EdgeCubeMovementController : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject leftdown;
    //public GameObject rightdown;
    //public GameObject upleft;
    //public GameObject upright;
    public GameObject startPos;
    public GameObject centerCube;
    public Color StartColor;
    public Color EndColor;


    public int currIndex;
    public int totalCubeNum;
    void Start()
    {
        totalCubeNum = transform.childCount;
        currIndex = -1;
    }

    // Update is called once per frame
    void Update()
    {
        float timeInterval = GameManager.Instance.TotalTimeLimit / (float)totalCubeNum;
        int currProgress = Mathf.FloorToInt(GameManager.Instance.currSpentTime / timeInterval);
        if (currProgress > currIndex)
        {
            while (currIndex < currProgress)
            {
                if (currIndex >= 0 && currIndex < totalCubeNum) 
                {
                    Debug.Log("Change Curr Index" + currIndex);
                    StartCoroutine(CubeColorChange(currIndex, 2.0f));
                };
                currIndex++;
            }
            currIndex = currProgress;
        }
    }

    public void GenCube() 
    {
       
        GameObject[] targets = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            targets[i] = transform.GetChild(i).gameObject;
        }
        var orderedTargets = targets.OrderBy(t => Vector3.SignedAngle(Vector3.Normalize(t.transform.position - centerCube.transform.position), Vector3.Normalize(centerCube.transform.position - startPos.transform.position) ,  Vector3.up)).ToList();

        GameObject newParent = new GameObject("OrganizedTargets");

        // 按顺序将 GameObjects 设置为新父节点的子对象
        foreach (GameObject target in orderedTargets)
        {
            target.transform.SetParent(newParent.transform);
        }
    }    
    
    public void RandScale() 
    {
       
        GameObject[] targets = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            targets[i] = transform.GetChild(i).gameObject;
        }

        foreach (GameObject child in targets)
        {
            var scale = new Vector3(Random.Range(0.99f, 1.01f), 1, Random.Range(0.99f, 1.01f));
            float scaleX = child.transform.localScale.x * scale.x;
            float scaleZ = child.transform.localScale.z * scale.z;
            child.transform.localScale = new Vector3(scaleX, 7.0f, scaleZ);
        }
        //for (int i = 0; i < transform.childCount; i++)
        //{
        //    targets[i] = transform.GetChild(i).gameObject;
        //}
        //var orderedTargets = targets.OrderBy(t => Vector3.SignedAngle(Vector3.Normalize(t.transform.position - centerCube.transform.position), Vector3.Normalize(centerCube.transform.position - startPos.transform.position) ,  Vector3.up)).ToList();

        //GameObject newParent = new GameObject("OrganizedTargets");

        //// 按顺序将 GameObjects 设置为新父节点的子对象
        //foreach (GameObject target in orderedTargets)
        //{
        //    target.transform.SetParent(newParent.transform);
        //}
        //if (testParent) 
        //{
        //    DestroyImmediate(testParent);
        //}
        //testParent = new GameObject("TestParent");
        //float startX = startPos.transform.position.x;
        //float endPosX = rightdown.transform.position.x;
        //while (startX < endPosX) 
        //{

        //}

    }

    IEnumerator CubeColorChange(int index, float time) 
    {
        //transform.GetChild(index).gameObject.SetActive(false);

        var Script = transform.GetChild(index)?.GetComponent<BackGroundValueSetter>();
        float originalTime = time;
        while (time > 0.0f)
        {
            time -= Time.deltaTime;
            var color = Color.Lerp(StartColor, EndColor, (originalTime - time) / originalTime);
            Script.SetColor(color);
            yield return null;
        }

        yield return null;
    }
}

[CustomEditor(typeof(EdgeCubeMovementController))]
public class EdgeCubeGeneratorEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI();
        var script = target as EdgeCubeMovementController;
        if (GUILayout.Button("Gen Cube")) 
        {
            script.GenCube();
        }

        if (GUILayout.Button("RandScale Cube"))
        {
            script.RandScale();
        }
    }
}
