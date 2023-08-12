using System.Collections;
using System.Collections.Generic;
using UIManagement;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject Body;
    public GameObject Flipper;
    public float followDistance = 0.01f;


    public enum GameState
    {
        NeedToReset,
        Inited,
        Start,
        GameOver,
        WaitingForInput,
    }

    [Header("Input Manager")]
    public string shootKey = "r";



    [Header("Flipper Move params")]
    private HingeJoint hinge;
    public float flipperMoveSpeed = 0.5f;
    public GameObject leftBound;
    public GameObject rightBound;
    public GameObject upperBound;
    public GameObject lowerBound;
    public GameObject flipperConnectedBody;
    public bool faceRight = true;
    public GameObject flipperGo;
    public Rigidbody flipperRigidbody;

    [Header("Ball params")]
    public GameObject ball_prefab;
    public GameObject ball;
    public GameObject ballInitPosGo;
    public Ball ballScript;


    public GameState gameState;

    private Vector3 BodyFlipperDiff;


    [Header("Game Prams")]
    public int totalBricksNum;
    public int currBricksNum;



    [Header("Level Info")]
    public LevelData level;
    public string[] levelName;
    public int currLevelPlayTime;
    public bool loadNewLevel;
    public bool startCountDown = false;
    public float currSpentTime;


    [Header("Skill Param")]
    public bool isHitCount = true;
    public bool isHit = false;
    public bool isButtonPressed = false;
    public float hitDuration = 0.3f;
    public float buttonPressedDuration = 0.3f;
    public float currentHitTime = 0;
    public float currentButtonPressedTime = 0;
    public float currSkillCharge = 0;
    public float skillChargeCoolDown = 0.3f;
    public int currentEnergy = 0;


    [Header("Snall Info")]
    public GameObject snall;
    public SnallBehaviour snallBehaviour;

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameState.NeedToReset;
        if (gameState == GameState.NeedToReset) 
        {
            StartCoroutine("WaitForShoot");
        }

        hinge = flipperGo.GetComponent<HingeJoint>();

        BodyFlipperDiff = Body.transform.position - flipperConnectedBody.transform.position;

        //totalBricksNum = GameObject.Find("GridBase").transform.childCount;
        //currBricksNum = totalBricksNum;
        //if (gameState == GameState.Init)
        //{
        //    StartCoroutine("WaitForShoot");
        //}

        // level = GameObject.Find("LevelData").GetComponent<GameInfoContainer>().levelData;
        // LoadLevel(levelName[0]);
        // loadNewLevel = true;
        // startCountDown = false;
        // currSpentTime = currLevelPlayTime;

        snall = GameObject.Find("Snall");
        snallBehaviour = snall.GetComponent<SnallBehaviour>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startCountDown) 
        {
            currSpentTime -= Time.deltaTime;
            UIManager.Instance._uiList["UIManagement.UICountDownPanel"].OnUpdate(currSpentTime);
        }
        if (gameState == GameState.NeedToReset) 
        {
            StartCoroutine("WaitForShoot");
            gameState = GameState.Inited;
        }

        if (gameState == GameState.Start) 
        {
            StopCoroutine("WaitForShoot");
        }

        // motor = hinge.motor;	
        // motor = hinge.motor;														//		Flipper is desactivate. But you want him to go to the init position.
        // hinge.motor = motor;
        // hinge.useMotor = false;
        //
        Vector3 m_Input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        if (Input.GetButton("Vertical") || Input.GetButton("Horizontal"))
        {
            var motor = hinge.motor;                                                    // move the flipper to reach the init position
            hinge.motor = motor;
            hinge.useMotor = false;
            JointSpring hingeSpring = hinge.spring;
            hingeSpring.spring = 1000000.0f;
            hinge.spring = hingeSpring;
            //				Debug.Log(m_Input);
            Rigidbody flipperRigidbody = flipperConnectedBody.GetComponent<Rigidbody>();
            //Apply the movement vector to the current position, which is
            //multiplied by deltaTime and speed for a smooth MovePosition
            Vector3 newPos = flipperConnectedBody.transform.position + m_Input * Time.deltaTime * flipperMoveSpeed;
            newPos.x = Mathf.Min(rightBound.transform.position.x, newPos.x);
            newPos.x = Mathf.Max(leftBound.transform.position.x, newPos.x);
            newPos.z = Mathf.Min(upperBound.transform.position.z, newPos.z);
            newPos.z = Mathf.Max(lowerBound.transform.position.z, newPos.z);
            flipperRigidbody.MovePosition(newPos);

            Body.transform.position = flipperConnectedBody.transform.position + BodyFlipperDiff;

            isHitCount = false;
        }
        else 
        {
            isHitCount = true;
            JointSpring hingeSpring = hinge.spring;
            // 这个参数为了方便先写死，后面可以根据实际情况调整
            hingeSpring.spring = 3000.0f;
            hinge.spring = hingeSpring;
        }

        if (Input.GetKeyDown("j")) 
        {
            currentButtonPressedTime = buttonPressedDuration;
            isButtonPressed = true;
        }

        if (currentButtonPressedTime >= 0.0f)
        {
            currentButtonPressedTime -= Time.deltaTime;
        }
        else 
        {
            isButtonPressed = false;
        }
 

        if (currentHitTime >= 0.0f)
        {
            currentHitTime -= Time.deltaTime;
        }
        else
        {
            isHit = false;
        }

        if (currSkillCharge >= 0.0f) 
        {
            currSkillCharge -= Time.deltaTime;
        }


        if (Input.GetKeyDown("k")) 
        {
            TriggerUltSkill();
        
        }

    }


    public void TriggerHit()
    {
        if (isHitCount)
        {
            isHit = true;
            currentHitTime = hitDuration;

            if (isHit && isButtonPressed && currSkillCharge < 0.0f)
            {
                currSkillCharge = skillChargeCoolDown;
                currentEnergy += 1;
                currentEnergy = Mathf.Min(currentEnergy, 4);
                UIManager.Instance._uiList["UIManagement.UISkillPanel"].OnUpdate(currentEnergy / 4.0f);
            }
        }


    }

    IEnumerator WaitForShoot()
    {
        if (ball) 
        {
            Destroy(ball);
        }

        GameObject ball_instance = Instantiate(ball_prefab);
        ball_instance.transform.position = ballInitPosGo.transform.position;
        ball_instance.name = "ball_instance";
        ball_instance.SetActive(true);
        ball = ball_instance;
        ballScript = ball.GetComponent<Ball>();
        while (gameState == GameState.NeedToReset || gameState == GameState.Inited)
        {
            ball.GetComponent<SphereCollider>().enabled = false;
            ball.transform.position = ballInitPosGo.transform.position;
            if (Input.GetKeyDown(shootKey)) 
            {
                ShootBall();
            }
            yield return null;
        }
        //yield return new WaitForSeconds(2);
        //gameState = GameState.WaitingForInput;

    }

    public void ShootBall() 
    {
        Rigidbody ballRB = ball.GetComponent<Rigidbody>();
        ball.GetComponent<SphereCollider>().enabled = true;
        gameState = GameState.Start;
        ballRB.velocity = new Vector3(100, 0, 100);

        if (loadNewLevel) 
        {
            loadNewLevel = false;
            startCountDown = true;
        }

    }

    public void UpdateBrickNum(int num) 
    {
        currBricksNum += num;
        UpdateProgressBar(1.0f - (float)currBricksNum/(float)totalBricksNum);
    }

    public void UpdateProgressBar(float value)
    {
        
        UIManager.Instance._uiList["UIManagement.UIProgressPanel"].OnUpdate(value);
    
    }

    public void LoadLevel(string levelName) 
    {
        bool find = false;
        foreach (SingleLevel level in level.levelLists)
        {
            if (levelName == level.name) 
            {
                currLevelPlayTime = level.gameTime;
                find = true;
            }
        }
        if(!find)
        {
            Debug.LogError("Can't find level: " + levelName);
        }
        else
        {
            Debug.Log("Load level: " + levelName);
        }
    }

    public void TriggerUltSkill() 
    {
        if (currentEnergy >= 4) 
        {
            currentEnergy = 0;
            UIManager.Instance._uiList["UIManagement.UISkillPanel"].OnUpdate(currentEnergy / 4.0f);
            GridManager.Instance.TriggerSkill(ball.transform.position, 2, 2);
        }
        // 1 means padding 1 grid;
        Debug.Log("Trigger Skill");
    
    }

}
