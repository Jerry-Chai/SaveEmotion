using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class CharectorController : MonoBehaviour
{
    public float moveSpeedVertical = 100.0f;
    public float moveSpeefHorizontal = 100.0f;
    public GameObject player;
    public GameObject leftBound;
    public GameObject rightBound;
    public GameObject upperBound;
    public GameObject lowerBound;
    public GameObject boardMountPoint;


    public GameObject ball;
    public GameObject ballMountPoint;

    public Camera cam;
    public bool faceRight = true;

    public bool isGameStart = false;
    public bool canShootBall = true;

    [Header("Shoot Param Settings")]
    public float punchTime = 0.5f;
    public float startAngle = 30.0f;
    public float endAngle = 60.0f;
    public float shootCoolDown = 1.0f;
    public float lastShootTillNow = 100.0f;
    // Start is called before the first frame update
    void Start()
    {
        ball.transform.position = new Vector3(ballMountPoint.transform.position.x, ballMountPoint.transform.position.y,
            ball.transform.position.z);

        ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        isGameStart = false;



    }

    // Update is called once per frame
    void Update()
    {


        Vector3 mountPointScreenPos = cam.WorldToScreenPoint(boardMountPoint.transform.position);
        Vector3 mousePointScreenPos = Input.mousePosition;
        mountPointScreenPos.z = 0.0f;
        mousePointScreenPos.z = 0.0f;
        Vector3 directionVector = Vector3.Normalize(mousePointScreenPos - mountPointScreenPos);
        directionVector.z = 0.0f;
        
        
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        
        bool shouldUpdate = false;
        if (inputX < -0.01f || inputX > 0.01f) shouldUpdate = true;
        if (inputY < -0.01f || inputY > 0.01f) shouldUpdate = true;
        
        //// board rotation:
        //if (faceRight && Mathf.Abs(Vector3.Angle(directionVector, Vector3.right)) < maxAngle)
        //{
        //    boardMountPoint.transform.rotation = Quaternion.LookRotation(directionVector) * Quaternion.FromToRotation(Vector3.right, Vector3.forward);
        //}
        //if (!faceRight && Mathf.Abs(Vector3.Angle(directionVector, Vector3.left)) < maxAngle)
        //{
        //    boardMountPoint.transform.rotation = Quaternion.LookRotation(directionVector) * Quaternion.FromToRotation(Vector3.left, Vector3.forward);
        //}

        if (Input.GetButtonDown("Fire1") && lastShootTillNow > shootCoolDown)
        {
            Debug.Log("Test!!");
            lastShootTillNow = 0.0f;
            StartCoroutine(DoRightPunch());
        }
        lastShootTillNow += Time.deltaTime;

        
        // face towards:
        if (inputX < -0.01f && faceRight)
        {
            faceRight = false;
            player.transform.localScale = new Vector3(player.transform.localScale.x * -1, player.transform.localScale.y, player.transform.localScale.z);
        }

        if (inputX > 0.01f && !faceRight)
        {
            faceRight = true;
            player.transform.localScale = new Vector3(player.transform.localScale.x * -1, player.transform.localScale.y, player.transform.localScale.z);
        }

        // move
        if (shouldUpdate)
        {
            Vector3 tempPos = player.transform.position;
            tempPos.x += inputX * Time.deltaTime * moveSpeefHorizontal;
            tempPos.y += inputY * Time.deltaTime * moveSpeedVertical;
            tempPos.x = tempPos.x < leftBound.transform.position.x ? leftBound.transform.position.x : tempPos.x;
            tempPos.x = tempPos.x > rightBound.transform.position.x ? rightBound.transform.position.x : tempPos.x;
            tempPos.y = tempPos.y < lowerBound.transform.position.y ? lowerBound.transform.position.y : tempPos.y;
            tempPos.y = tempPos.y > upperBound.transform.position.y ? upperBound.transform.position.y : tempPos.y;
            player.transform.position = tempPos;
        }
        
        if (!isGameStart)
        {
            OnGameWaitForStart();

        }
        
        if (Input.GetMouseButtonDown(0) && canShootBall)
        {
            StartShootBall();
            isGameStart = true;
            canShootBall = false;
            
        }


    }

    public void StartShootBall()
    {
        Rigidbody2D ballRb = ball.GetComponent<Rigidbody2D>();
        ball.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        float random = Random.Range(0, 1.0f);
        Debug.Log(random);
        ballRb.isKinematic = false;
        ballRb.velocity = Vector2.zero;
        if (random > 0.5f)
        {
                
            ballRb.AddForce(new Vector2(8000, 8000));
        }
        else
        {
            ballRb.AddForce(new Vector2(-8000, 8000));
        }
    }


    public void OnGameWaitForStart()
    {
        ball.transform.position = new Vector3(ballMountPoint.transform.position.x, ballMountPoint.transform.position.y,
            ball.transform.position.z);
    }

    IEnumerator DoRightPunch()
    {
        
        Vector3 directionVector = Vector3.Normalize(new Vector3(Mathf.Cos(2 * Mathf.PI * (endAngle / 360.0f)), Mathf.Sin(2 * Mathf.PI * (endAngle / 360.0f)), 0.0f));
        if (!faceRight)
        {
            directionVector = Vector3.Normalize(new Vector3(Mathf.Cos(2 * Mathf.PI * (endAngle / 360.0f)), -Mathf.Sin(2 * Mathf.PI * (endAngle / 360.0f)), 0.0f));
        }
        Quaternion endPos =  Quaternion.LookRotation(directionVector) * Quaternion.FromToRotation(Vector3.right, Vector3.forward);

        directionVector = Vector3.Normalize(new Vector3(Mathf.Cos(2 * Mathf.PI * (startAngle / 360.0f)), -Mathf.Sin(2 * Mathf.PI * (startAngle / 360.0f)), 0.0f));
        if (!faceRight)
        {
            directionVector = Vector3.Normalize(new Vector3(Mathf.Cos(2 * Mathf.PI * (startAngle / 360.0f)), Mathf.Sin(2 * Mathf.PI * (startAngle / 360.0f)), 0.0f));
        }
        Quaternion startPos =  Quaternion.LookRotation(directionVector) * Quaternion.FromToRotation(Vector3.right, Vector3.forward);



        float remainTime = punchTime;
        while (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            boardMountPoint.transform.rotation = Quaternion.Lerp(endPos, startPos, remainTime / punchTime);
            yield return null;
        }
        boardMountPoint.transform.rotation = startPos;
        yield return null;
    }
}
