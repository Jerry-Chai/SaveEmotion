using DG.Tweening;
using JSAM;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class SnallBehaviour : MonoBehaviour
{
    public GameObject EffectPrefab;


    public enum SnallState
    {
        DetermineMove,
        Move,
        Defend,
        Idle,
        ShootSkill
    }


    public SnallState state;
    public float speed = 0.01f;
    public float moveMinRange;
    public float moveMaxRange;
    public GameObject upperLeft;
    public GameObject lowerRight;
    public GameObject ShootObjRoot;
    // Start is called before the first frame update
    // 表示蜗牛一次技能能改变多少个block的状态
    public int revertBlocksNum;

    private float distanceRatioToStopPoint;

    public float idleTime = 3.0f;
    public float defendTime = 3.0f;
    public float shootTime = 5.0f;
    public float shootObjectTime = 3.0f;

    private float idleTimer = 0.0f;
    private float defendTimer = 0.0f;
    private float shootTimer = 0.0f;
    private Vector3 nextPos;
    private Vector3 originalPos;
    private float totalDistance;
    private float currDistance;

    public Animator animator;
    public Vector2 moveDir;

    public SnallState lastState;

    private float originalY;
    public bool shoottedInThisLoop = false;

    public int shootSkillNum = 6;
    public List<GameObject> shootPos = new List<GameObject>();

    public GameObject head0;
    public GameObject head1;
    public GameObject head2;
    
    public Material head0_mat;
    public Material head1_mat;
    public Material head2_mat;

    [Header("位置限制")]
    public GameObject rightLower;
    public GameObject leftUpper;
    void Start()
    {

        distanceRatioToStopPoint = 1.0f;
        state = SnallState.Idle;
        lastState = state;
        idleTimer = idleTime;
        defendTimer = defendTime;
        shootTimer = shootTime;

        animator = GetComponent<Animator>();
        animator.SetTrigger("Defend");
        moveDir = new Vector2(0.0f, 0.0f);
        originalY = this.transform.position.y;

        shoottedInThisLoop = false;

        head0 = transform.Find("Eye").gameObject;
        head1 = transform.Find("Eye.005").gameObject;
        head2 = transform.Find("Eye.003").gameObject;

        head0_mat = head0.GetComponent<SkinnedMeshRenderer>().material;
        head1_mat = head1.GetComponent<SkinnedMeshRenderer>().material;
        head2_mat = head2.GetComponent<SkinnedMeshRenderer>().material;

        if (!head0 || !head1 || !head2 || !head0_mat || !head1_mat || !head2_mat) 
        {
            Debug.LogError("Can't find head");
        }

        StartCoroutine(SwitchAnimationState(SnallState.Defend));
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.Start) return;
        if (lastState != state)
        {
            lastState = state;
            StartCoroutine(SwitchAnimationState(lastState));

        }

        if (state == SnallState.Idle && idleTimer >= 0)
        {
            idleTimer -= Time.deltaTime;
        }
        else if (state == SnallState.Idle && idleTimer < 0)
        {
            state = SnallState.DetermineMove;
            idleTimer = idleTime;
        }


        if (state == SnallState.DetermineMove)
        {
            // 这个地方，最好做一个限制，太近的地方会影响技能释放的时间
            nextPos = DetermineNextPos();
            state = SnallState.Move;
            originalPos = this.transform.position;
            originalPos.y = originalY;
            nextPos.y = originalY;
            totalDistance = Vector3.Distance(originalPos, nextPos);
            currDistance = 0.0f;
        }

        // 移动状态
        float distance = Vector3.Distance(this.transform.position, nextPos);
        moveDir = new Vector2(-(nextPos.x - this.transform.position.x), -(nextPos.z - this.transform.position.z)).normalized;
        if (moveDir.x * this.transform.localScale.x > 0.0f)
        {
            this.transform.localScale = new Vector3(-this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
        }

        if (state == SnallState.Move && distance >= 0.01f)
        {
            currDistance += speed * Time.deltaTime;
            float currRatio = currDistance / totalDistance;
            this.transform.position = Vector3.Lerp(originalPos, nextPos, currRatio);
        }
        else if (state == SnallState.Move && distance <= 0.01f)
        {
            state = SnallState.Defend;
        }

        // 防御状态
        // 这个地方感觉可以设置一下， 蜗牛从移动到进入防御时间，应该有一个间隔
        if (state == SnallState.Defend && defendTimer >= 0)
        {
            defendTimer -= Time.deltaTime;
        }
        else if (state == SnallState.Defend && defendTimer < 0)
        {
            state = SnallState.ShootSkill;
            defendTimer = defendTime;
        }

        if (state == SnallState.ShootSkill && shootTimer >= 0)
        {
            shootTimer -= Time.deltaTime;
        }

        if (state == SnallState.ShootSkill && shootTimer <= shootObjectTime && !shoottedInThisLoop)
        {
            Debug.Log("Trigger Skill");
            shoottedInThisLoop = true;
            TrigerSnallSkill(revertBlocksNum);
        }
        else if (state == SnallState.ShootSkill && shootTimer < 0)
        {
            state = SnallState.Idle;
            defendTimer = defendTime;
            shoottedInThisLoop = false;
            shootTimer = shootTime;
        }

    }

    public Vector3 DetermineNextPos()
    {
        // 根据蜗牛的位置， 以及gridManager得到的下个点的位置， 去判断，
        // 如果这个点符合要求， 就走过去。

        float currX = this.transform.position.x;
        float currY = this.transform.position.z;

        //// random dir:
        //float x = Random.Range(-moveMinRange, moveMaxRange);
        //float y = Random.Range(-moveMinRange, moveMaxRange);

        //bool isXInBound = currX + x < lowerRight.transform.position.x && currX + x > upperLeft.transform.position.x;
        //bool isYInBound = currY + y < upperLeft.transform.position.z && currY + y > lowerRight.transform.position.z;

        //while (!isXInBound || !isYInBound) 
        //{
        //    x = Random.Range(-moveMinRange, moveMaxRange);
        //    y = Random.Range(-moveMinRange, moveMaxRange);
        //    isXInBound = currX + x < lowerRight.transform.position.x && currX + x > upperLeft.transform.position.x;
        //    isYInBound = currY + y < upperLeft.transform.position.z && currY + y > lowerRight.transform.position.z;
        //}
        Vector3 nextPos = GridManager.Instance.GetRandomGrid();
        do
        {
            nextPos = GridManager.Instance.GetRandomGrid();
        } while(nextPos.x > rightLower.transform.position.x || nextPos.x < leftUpper.transform.position.x || nextPos.z > leftUpper.transform.position.z || nextPos.z < rightLower.transform.position.z);
        Debug.Log(nextPos);
        return nextPos;
        //return new Vector3(currX + x, this.transform.position.y, currY + y); ;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player is in the snall's range");
    }

    public void TrigerSnallSkill(int revertBlockNum)
    {
        if (GridManager.Instance.UnlockedNormalGridDic.Count <= 0)
        {
            // 说明没有可以释放的地方了， 放空炮
            for (int i = 0; i < shootSkillNum; i++)
            {
                int index = i >= shootPos.Count ? i : shootPos.Count - 1;
                GameObject sphere = Instantiate(EffectPrefab);
                sphere.name = "Special Effect";
                sphere.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
                sphere.transform.position = ShootObjRoot.transform.position;
                sphere.transform.DOJump(shootPos[i].transform.position, 5, 1, 1.0f).OnComplete(() =>
                {
                    //script.LockThisGrid();
                    StartCoroutine(DelayDestroy(sphere));
                });

            }
            return;
        }
        for (int i = 0; i < shootSkillNum; i++)
        {
            //GridManager.Instance.LockGridBySnallSkill(revertBlocksNum, this.gameObject.transform.position);
            GameObject sphere = Instantiate(EffectPrefab);
            sphere.name = "Special Effect";
            var script = GridManager.Instance.GetRandomUnlockedGrid();
            Debug.Log("Index " + i + " : " + script.gameObject.GetInstanceID());
            if (script == null)
            {
                // 如果有一个没回传， 说明后面的所有都不会回传
                return;
            }
            sphere.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
            sphere.transform.position = this.transform.position;
            sphere.transform.DOJump(script.transform.position, 5, 1, 1.0f).OnComplete(() =>
            {
                Vector2 dir = new Vector2(-(script.transform.position.x - this.transform.position.x), -(script.transform.position.z - this.transform.position.z)).normalized;
                script.LockThisGridBySnallSkill(dir);
                Destroy(sphere);
            });
        }

    }

    public void AniEvent_PlaySkillSound() 
    {
        AudioManager.PlaySound(JSAMSounds.SnallSkill);
        Debug.Log("Play Sounds");
    }

    IEnumerator DelayDestroy(GameObject obj) 
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(obj);
    }    
    
    IEnumerator SwitchAnimationState(SnallState lastState) 
    {
        switch (lastState)
        {
            case SnallState.Defend:
                animator.SetTrigger("Defend");
                break;
            case SnallState.Move:
                animator.SetTrigger("Extend");
                yield return  WaitFor.Frames(32);
                animator.SetTrigger("Move");
                break;
            case SnallState.ShootSkill:
                animator.SetTrigger("ShootSkill");
                break;
        }

        yield return null;
    }
}


public static class WaitFor
{
    public static IEnumerator Frames(int frameCount)
    {
        if (frameCount <= 0)
        {
            //throw new ArgumentOutOfRangeException("frameCount", "Cannot wait for less that 1 frame");
            yield break;
        }

        while (frameCount > 0)
        {
            frameCount--;
            yield return null;
        }
    }
}
