using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GophersBehaviour : MonoBehaviour
{
    public GameObject EffectPrefab;


    public enum GophersState
    {
        DetermineMove,
        Move,
        Defend,
        Idle,
        ShootSkill
    }


    public GophersState state;
    public float speed = 0.01f;
    // public float moveMinRange;
    // public float moveMaxRange;
    // public GameObject upperLeft;
    // public GameObject lowerRight;
    // public GameObject ShootObjRoot;
    // // Start is called before the first frame update
    // // 表示蜗牛一次技能能改变多少个block的状态
    // public int revertBlocksNum;
    //
    // private float distanceRatioToStopPoint;
    //
    public float idleTime = 3.0f;
    public float defendTime = 3.0f;
    public float shootTime = 5.0f;
    public float shootObjectTime = 3.0f;
    //
    private float idleTimer = 0.0f;
    private float defendTimer = 0.0f;
    private float shootTimer = 0.0f;
    private Vector3 nextPos;
    private Vector3 originalPos;
    private float totalDistance;
    private float currDistance;
    //
    // public Animator animator;
    public Vector2 moveDir;
    //
    public GophersState lastState;
    //
    private float originalY;
    public bool shoottedInThisLoop = false;
    //
    // public int shootSkillNum = 6;
    // public List<GameObject> shootPos = new List<GameObject>();
    //
    // public GameObject head0;
    // public GameObject head1;
    // public GameObject head2;
    //
    // public Material head0_mat;
    // public Material head1_mat;
    // public Material head2_mat;
    //
    // [Header("位置限制")]
    // public GameObject rightLower;
    // public GameObject leftUpper;

    public GameObject gophersRoot;
    void Start()
    {

        //distanceRatioToStopPoint = 1.0f;
        state = GophersState.Idle;
        lastState = state;
        idleTimer = idleTime;
        defendTimer = defendTime;
        shootTimer = shootTime;

        // animator = GetComponent<Animator>();
        // animator.SetTrigger("Defend");
        moveDir = new Vector2(0.0f, 0.0f);
        originalY = this.transform.position.y;
        //
        shoottedInThisLoop = false;
        //
        // head0 = transform.Find("Eye").gameObject;
        // head1 = transform.Find("Eye.005").gameObject;
        // head2 = transform.Find("Eye.003").gameObject;
        //
        // head0_mat = head0.GetComponent<SkinnedMeshRenderer>().material;
        // head1_mat = head1.GetComponent<SkinnedMeshRenderer>().material;
        // head2_mat = head2.GetComponent<SkinnedMeshRenderer>().material;
        //
        // if (!head0 || !head1 || !head2 || !head0_mat || !head1_mat || !head2_mat) 
        // {
        //     Debug.LogError("Can't find head");
        // }
        //
        // StartCoroutine(SwitchAnimationState(GophersState.Defend));
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameManager.GameState.Start) return;
        if (lastState != state)
        {
            lastState = state;
            //StartCoroutine(SwitchAnimationState(lastState));

        }

        if (state == GophersState.Idle && idleTimer >= 0)
        {
            idleTimer -= Time.deltaTime;
        }
        else if (state == GophersState.Idle && idleTimer < 0)
        {
            state = GophersState.DetermineMove;
            idleTimer = idleTime;
        }


        if (state == GophersState.DetermineMove)
        {
            // 这个地方，最好做一个限制，太近的地方会影响技能释放的时间
            nextPos = DetermineNextPos();
            state = GophersState.Move;
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

        if (state == GophersState.Move && distance >= 0.01f)
        {
            currDistance += speed * Time.deltaTime;
            float currRatio = currDistance / totalDistance;
            this.transform.position = Vector3.Lerp(originalPos, nextPos, currRatio);
        }
        else if (state == GophersState.Move && distance <= 0.01f)
        {
            state = GophersState.Defend;
        }

        // 防御状态
        // 这个地方感觉可以设置一下， 蜗牛从移动到进入防御时间，应该有一个间隔
        if (state == GophersState.Defend && defendTimer >= 0)
        {
            defendTimer -= Time.deltaTime;
        }
        else if (state == GophersState.Defend && defendTimer < 0)
        {
            state = GophersState.ShootSkill;
            defendTimer = defendTime;
        }

        if (state == GophersState.ShootSkill && shootTimer >= 0)
        {
            shootTimer -= Time.deltaTime;
        }

        if (state == GophersState.ShootSkill && shootTimer <= shootObjectTime && !shoottedInThisLoop)
        {
            Debug.Log("Trigger Skill");
            shoottedInThisLoop = true;
            //TrigerGophersSkill(revertBlocksNum);
        }
        else if (state == GophersState.ShootSkill && shootTimer < 0)
        {
            state = GophersState.Idle;
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

        foreach (var VARIABLE in prefa)
        {
            
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player is in the Gophers's range");
    }

    // public void TrigerGophersSkill(int revertBlockNum)
    // {
    //     if (GridManager.Instance.UnlockedNormalGridDic.Count <= 0)
    //     {
    //         // 说明没有可以释放的地方了， 放空炮
    //         for (int i = 0; i < shootSkillNum; i++)
    //         {
    //             int index = i >= shootPos.Count ? i : shootPos.Count - 1;
    //             GameObject sphere = Instantiate(EffectPrefab);
    //             sphere.name = "Special Effect";
    //             sphere.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
    //             sphere.transform.position = ShootObjRoot.transform.position;
    //             sphere.transform.DOJump(shootPos[i].transform.position, 5, 1, 1.0f).OnComplete(() =>
    //             {
    //                 //script.LockThisGrid();
    //                 StartCoroutine(DelayDestroy(sphere));
    //             });
    //
    //         }
    //         return;
    //     }
    //     for (int i = 0; i < shootSkillNum; i++)
    //     {
    //         //GridManager.Instance.LockGridByGophersSkill(revertBlocksNum, this.gameObject.transform.position);
    //         GameObject sphere = Instantiate(EffectPrefab);
    //         sphere.name = "Special Effect";
    //         var script = GridManager.Instance.GetRandomUnlockedGrid();
    //         Debug.Log("Index " + i + " : " + script.gameObject.GetInstanceID());
    //         if (script == null)
    //         {
    //             // 如果有一个没回传， 说明后面的所有都不会回传
    //             return;
    //         }
    //         sphere.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
    //         sphere.transform.position = this.transform.position;
    //         sphere.transform.DOJump(script.transform.position, 5, 1, 1.0f).OnComplete(() =>
    //         {
    //             Vector2 dir = new Vector2(-(script.transform.position.x - this.transform.position.x), -(script.transform.position.z - this.transform.position.z)).normalized;
    //             script.LockThisGridByGophersSkill(dir);
    //             Destroy(sphere);
    //         });
    //     }
    //
    // }

    public void AniEvent_PlaySkillSound() 
    {
        //AudioManager.PlaySound(JSAMSounds.GophersSkill);
        Debug.Log("Play Sounds");
    }

    IEnumerator DelayDestroy(GameObject obj) 
    {
        yield return new WaitForSeconds(2.0f);
        Destroy(obj);
    }    
    
    // IEnumerator SwitchAnimationState(GophersState lastState) 
    // {
    //     switch (lastState)
    //     {
    //         case GophersState.Defend:
    //             animator.SetTrigger("Defend");
    //             break;
    //         case GophersState.Move:
    //             animator.SetTrigger("Extend");
    //             yield return  WaitFor.Frames(32);
    //             animator.SetTrigger("Move");
    //             break;
    //         case GophersState.ShootSkill:
    //             animator.SetTrigger("ShootSkill");
    //             break;
    //     }
    //
    //     yield return null;
    // }
}


