using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameObject Body;
    public GameObject Flipper;
    public float followDistance = 0.01f;


    public enum GameState
    {
        Init,
        Start,
        GameOver,
        WaitingForInput
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
    public GameObject ball;
    public GameObject ballInitPosGo;


    public GameState gameState;

    private Vector3 BodyFlipperDiff;


    // Start is called before the first frame update
    void Start()
    {
        hinge = flipperGo.GetComponent<HingeJoint>();
        gameState = GameState.Init;

        BodyFlipperDiff = Body.transform.position - flipperConnectedBody.transform.position;
        ball.transform.position = ballInitPosGo.transform.position;

        if (gameState == GameState.Init)
        {
            StartCoroutine(WaitForShoot());
        }
    }

    // Update is called once per frame
    void Update()
    {

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
        }


    }


    IEnumerator WaitForShoot()
    {
        while (gameState == GameState.Init)
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
        gameState = GameState.Start;
        Rigidbody ballRB= ball.GetComponent<Rigidbody>();
        ball.GetComponent<SphereCollider>().enabled = true;
        ballRB.velocity = new Vector3(1, 0, 1);

    }

}
