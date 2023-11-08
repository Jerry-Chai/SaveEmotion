using System.Collections;
using System.Collections.Generic;
using JSAM;
using UIManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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
    public bool isGameStarted = false;
    public float currSpentTime;
    public float TotalTimeLimit = 90.0f;


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


    [Header("Plane")]
    private Plane plane;

    [Header("Liquid Mat Ctrl")]
    public Material liquidMat;
    
    [Header("Postprocessing control")]
    public SceneProjection _projectionScript;
    public GameObject _postProcessing;
    private Volume v;
    private Bloom b;
    private Vignette vg;    
    
    [Header("PrefabEffect GameObject")]
    public GameObject CollisionEffectPrefab;
    public Vector3 TriggerSkillPos;



   
    public enum BossType 
    {
        Snall,
        Gopher
    }
    [Header("Boss")]
    public BossType bossType;
    private bool isInBulletTime;
    public float enterBulletTime = 0.5f;
    private float bulletEnterTimeCount = 0.0f;

    [Header("Snall Info")]
    public GameObject Gophers;
    public GophersBehaviour gophersBehaviour;

    [Header("EndScene Image")]
    public GameObject EndSceneImage;


    string sceneName;
    void Start()
    {
        _projectionScript = GameObject.Find("ProjectionManager")?.GetComponent<SceneProjection>();

        AudioManager.PlayMusic(JSAMMusic.BackGroundMusic);
        // 获取当前激活的场景
        Scene currentScene = SceneManager.GetActiveScene();

        // 获取场景的名称
        sceneName = currentScene.name;
        if (sceneName == "IntroScene0000") return;
        gameState = GameState.NeedToReset;
        if (gameState == GameState.NeedToReset)
        {
            StartCoroutine("WaitForShoot");
            gameState = GameState.Inited;
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
        loadNewLevel = true;
        startCountDown = false;
        // currSpentTime = currLevelPlayTime;

        
        snall = GameObject.Find("Snall");
        if (snall) 
        {
            snallBehaviour = snall.GetComponent<SnallBehaviour>();
        }
        Gophers = GameObject.Find("Gophers");
        if (Gophers)
        {
            gophersBehaviour = Gophers.GetComponent<GophersBehaviour>();
        }

        plane = new Plane(Vector3.up, ball.transform.position);


        liquidMat.SetFloat("_WobbleX", 0.0f);
        StartCoroutine(UpdateEnergyMat(0, 2.0f));

        v = _postProcessing.GetComponent<Volume>();
        v.profile.TryGet(out b);
        v.profile.TryGet(out vg);

        if (EndSceneImage)
        {
            EndSceneImage.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneName == "IntroScene0000") return;
        //        Debug.Log(gameState.ToString());
        if (startCountDown)
        {
            currSpentTime += Time.deltaTime;
        }
        if (gameState == GameState.NeedToReset)
        {
            StartCoroutine("WaitForShoot");
            gameState = GameState.Inited;
        }

        //if (gameState == GameState.Start) 
        //{
        //    StopCoroutine("WaitForShoot");
        //}

        // motor = hinge.motor;	
        // motor = hinge.motor;														//		Flipper is desactivate. But you want him to go to the init position.
        // hinge.motor = motor;
        // hinge.useMotor = false;
        //
        Vector3 m_Input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        DetermineLiquidMovement(m_Input);
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

        if (Input.GetKeyDown("j") || Input.GetButtonDown("Fire1"))
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
            isInBulletTime = true;
            Debug.Log("isInBulletTime :" +　isInBulletTime);
        }

        if (Input.GetKeyUp("k"))
        {
            isInBulletTime = false;
            StartCoroutine(TriggerUltraSkill());
            Debug.Log("isInBulletTime　:" + isInBulletTime);
        }

        
        if (isInBulletTime)
        {
            if(bulletEnterTimeCount < 2.0f)bulletEnterTimeCount += Time.deltaTime / enterBulletTime;
            float tempVal = Mathf.Clamp(bulletEnterTimeCount / enterBulletTime, 0.0f, 1.0f);
            float timescale = Mathf.Lerp(1.0f, 0.5f, tempVal);
            Time.timeScale = timescale;

            float vgintensity = Mathf.Lerp(0.3f, 0.65f, tempVal);
            vg.intensity.value = vgintensity;
        }
        else
        {
            if (bulletEnterTimeCount > 0.0f) bulletEnterTimeCount -= Time.deltaTime / enterBulletTime;
            float tempVal = Mathf.Clamp(bulletEnterTimeCount / enterBulletTime, 0.0f, 1.0f);
            float timescale = Mathf.Lerp(1.0f, 0.5f, tempVal);
            Time.timeScale = timescale;

            float vgintensity = Mathf.Lerp(0.3f, 0.65f, tempVal);
            vg.intensity.value = vgintensity;
        }

        if (currSpentTime > TotalTimeLimit && EndSceneImage) 
        {
            EndSceneImage.SetActive(true);
            if(snallBehaviour)snallBehaviour.gameObject.SetActive(false);
            if(gophersBehaviour) gophersBehaviour.gameObject.SetActive(false);
            AudioManager.StopMusic(JSAMMusic.BackGroundMusic);
            AudioManager.StopMusic(JSAMSounds.AlarmTick);
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
                if (currentEnergy == 4) return;
                StartCoroutine(UpdateEnergyMat(currentEnergy, 2.0f));
                Debug.Log("Energy charging!");
                //UIManager.Instance._uiList["UIManagement.UISkillPanel"].OnUpdate(currentEnergy / 4.0f);
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
        ball.GetComponent<SphereCollider>().enabled = false;

        while (gameState == GameState.NeedToReset || gameState == GameState.Inited)
        {
            //{// this creates a horizontal plane passing through this object's center

            // create a ray from the mousePosition
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            // plane.Raycast returns the distance from the ray start to the hit point
            float distance;
            Vector3 hitPoint;
            if (plane.Raycast(ray, out distance)) {
                //                Debug.Log("Couroutine is runing");
                _projectionScript.SetLineRendererEnableState(true);
                // some point of the plane was hit - get its coordinates
                hitPoint = ray.GetPoint(distance);
                // use the hitPoint to aim your cannon
                Debug.DrawLine(ball.transform.position, hitPoint);
                var ballPos = ball.transform.position;
                Vector3 velocityDir = hitPoint - ballPos;
                ballPos.y = 0;
                hitPoint.y = 0.0f;
                float speed = 100.0f;
                velocityDir = hitPoint - ballPos;
                velocityDir.y = 0.03f;
                velocityDir = Vector3.Normalize(velocityDir);
                _projectionScript.SimulateTrajectory(ball_prefab, ball.transform.position, velocityDir, speed);
                if (Input.GetMouseButton(1))
                {
                    isGameStarted = true;
                    ShootBall(velocityDir, speed);
                    _projectionScript.SetLineRendererEnableState(false);
                }
            }
            //}

            ball.transform.position = ballInitPosGo.transform.position;

            yield return null;
        }
        yield return null;
        //gameState = GameState.WaitingForInput;

    }

    public void ShootBall(Vector3 dir, float speed)
    {
        var ballScript = ball.GetComponent<Ball>();
        ballScript.Shoot(dir, speed);
        Rigidbody ballRB = ball.GetComponent<Rigidbody>();

        gameState = GameState.Start;

        if (loadNewLevel)
        {
            loadNewLevel = false;
            StartGameStartBehaviour();
        }

    }

    public void StartGameStartBehaviour()
    {
        startCountDown = true;
        AudioManager.PlaySound(JSAMSounds.AlarmTick);
    }

    public void UpdateBrickNum(int num)
    {
        currBricksNum += num;
        UpdateProgressBar(1.0f - (float)currBricksNum / (float)totalBricksNum);
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
        if (!find)
        {
            Debug.LogError("Can't find level: " + levelName);
        }
        else
        {
            Debug.Log("Load level: " + levelName);
        }
    }

    IEnumerator TriggerUltraSkill()
    {
        if (currentEnergy > 3)
        {
            currentEnergy = 0;
            //UIManager.Instance._uiList["UIManagement.UISkillPanel"].OnUpdate(currentEnergy / 4.0f);
            //GridManager.Instance.TriggerSkill(ball.transform.position, 2, 2);
            StartCoroutine(UpdateEnergyMat(0, 2.0f));
            TriggerSkillPos = ball.transform.position;
            GameObject CollisionEffect = Instantiate(CollisionEffectPrefab);
            CollisionEffect.transform.position = TriggerSkillPos;
            CollisionEffect.SetActive(true);
            CollisionEffect.transform.localScale = 3.0f * Vector3.one;
            CollisionEffect.GetComponent<ParticleSystem>().Play();
            Debug.Log("Play Collision Effect");
            yield return new WaitForSeconds(0.5f);

            // create Sphere
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = TriggerSkillPos;
            sphere.transform.localScale = 15.0f * Vector3.one;
            sphere.gameObject.name = "Collision";
            sphere.gameObject.tag = "SkillRange";
            sphere.GetComponent<MeshRenderer>().enabled = false;
            sphere.GetComponent<Collider>().isTrigger = true;

            yield return new WaitForSeconds(0.5f);
            Destroy(sphere);
            yield return new WaitForSeconds(1.0f);
            Destroy(CollisionEffect);

        }
        // 1 means padding 1 grid;
        Debug.Log("Trigger Skill");

    }
    public float[] energyValueList = new float[4]{-4.0f, -2.0f, 1.0f, 4.0f};
    IEnumerator UpdateEnergyMat(int index, float time) 
    {
        Debug.Log((index + 3) % 4);
        Debug.Log(index);
        float fromValue = energyValueList[(index + 3) % 4];
        float toValue = energyValueList[index];
        float currValue = 0.0f;

        while (currValue <= 1.0f) 
        {
        
            currValue += Time.deltaTime * (1 / time);
            liquidMat.SetVector("_FillAmount", new Vector4(0.0f, 0.0f, fromValue*(1-currValue) + toValue * currValue, 0.0f));
            yield return null;
        }
        yield return null;
    }

    private float lastWaveCountDownValue = 0.0f;
    private float maxWaveWobble = 45.0f;
    private float waveCountDownValue = 0.0f;
    private float signValue = 1.0f;
    private  float decreaseSpeed = 4.0f;
    private float fromValue = 0.0f;
    private float toValue = 0.0f;
    public void DetermineLiquidMovement(Vector3 m_Input)
    {
        // -inputx 是因为惯性看起来是这样。
        float waveX = Mathf.Abs(m_Input.x) >= 0.0001f ? -m_Input.x : 0.01f;
        //Debug.Log("waveX: " + waveX + "waveCoundDown: " + waveCountDownValue);
        //这个地方应该是说如果我现在的这个值比我原来的值大，就重置。
        if (Mathf.Abs(waveX) > Mathf.Abs(waveCountDownValue) + 0.1f)
        {
            //Debug.Log("waveX: " + waveX + "waveCoundDown: " + waveCountDownValue);
            waveCountDownValue = waveX;
            float signValue = (waveCountDownValue + 0.0001f) / Mathf.Abs(waveCountDownValue + 0.0001f);
            fromValue = waveX;
            toValue = -signValue * (Mathf.Abs(waveX) - 0.1f);
            //Debug.Log("fromValue: " + fromValue + "  toValue: " + toValue);
        }
        else if(fromValue != toValue && (Mathf.Abs(Mathf.Abs(fromValue) - Mathf.Abs(toValue)) > 0.001f))
        {
            //Debug.Log("fromValue: " + fromValue + "  toValue: " + toValue);
            

            // 首先把现在的waveCount的正负值知道；
            if (signValue > 0.0f)
            {
                if (waveCountDownValue >= toValue)
                {
                    waveCountDownValue += -Time.deltaTime * decreaseSpeed;
                    float wobbleX = waveCountDownValue * maxWaveWobble;
                    liquidMat.SetFloat("_WobbleX", wobbleX);
                    //Debug.Log("WobleX: " + wobbleX);
                }
                else
                {
                    fromValue = toValue;
                    signValue = (fromValue + 0.0001f) / Mathf.Abs(fromValue + 0.0001f);
                    toValue = -signValue * (Mathf.Abs(fromValue) - 0.1f);
                    if (Mathf.Abs(toValue) <= 0.1f)
                    {
                        toValue = fromValue;
                    }
                    waveCountDownValue = fromValue;
                }
            }
            else
            {
                if (waveCountDownValue <= toValue)
                {
                    waveCountDownValue +=  Time.deltaTime * decreaseSpeed;
                    float wobbleX = waveCountDownValue * maxWaveWobble;
                    liquidMat.SetFloat("_WobbleX", wobbleX);
                    //Debug.Log("WobleX: " + wobbleX);
                }
                else
                {
                    fromValue = toValue;
                    signValue = (fromValue + 0.0001f) / Mathf.Abs(fromValue + 0.0001f);
                    toValue = -signValue * (Mathf.Abs(fromValue) - 0.1f);
                    if (Mathf.Abs(toValue) <= 0.01f)
                    {
                        toValue = fromValue;
                    }
                    waveCountDownValue = fromValue;
                }
            }
        }

    }

    public Vector2 GetBossDir() 
    {
        switch (bossType)
        {
            case BossType.Snall:
                return snallBehaviour.moveDir;
                //break;
            case BossType.Gopher:
                return gophersBehaviour.moveDir;
                //break;  
        }
        return new Vector2();
    }
    
    

    //async void WaitForShoot()
    //{
    //    //        Debug.Log("Async Task Started");
    //    float change = toValue - fromValue;
    //    change /= time;
    //    float dissove = fromValue;

    //    propertyBlock.SetVector("_DissolveDirection", objDir);
    //    while (time >= 0.0f)
    //    {
    //        //            Debug.Log("Async Task Running");
    //        time -= Time.deltaTime;
    //        dissove += Time.deltaTime * change;
    //        propertyBlock.SetFloat("_ControlValue", dissove);
    //        renderer.SetPropertyBlock(propertyBlock);
    //        await Task.Yield();
    //    }
    //    // This task will finish, even though it's object is destroyed
    //    //      Debug.Log("Async Task Ended");
    //    //GridManager.
    //    //lock
    //    if (fromValue > toValue)
    //    {
    //        GridManager.Instance.LockNormalGrid(this.gameObject.GetInstanceID(), this);
    //    }
    //    else
    //    {
    //        GridManager.Instance.UnlockNormalGrid(this.gameObject.GetInstanceID(), this);

    //    }
    //}
}
