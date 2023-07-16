using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnallBehaviour : MonoBehaviour
{

    public enum SnallState
    {
        DetermineMove,
        Move,
        Defend,
        Idle

    }
    public SnallState state;
    public float speed = 0.01f;
    public float moveMinRange;
    public float moveMaxRange;
    public GameObject upperLeft;
    public GameObject lowerRight;
    // Start is called before the first frame update

    private float distanceRatioToStopPoint;
    
    public float idleTime = 3.0f;
    public float defendTime = 3.0f;

    private float idleTimer = 0.0f;
    private float defendTimer = 0.0f;
    private Vector3 nextPos;
    private Vector3 originalPos;
    private float totalDistance;
    private float currDistance;
    void Start()
    {
        distanceRatioToStopPoint = 1.0f;
        state = SnallState.Idle;

        idleTimer = idleTime;
        defendTimer = defendTime;
    }

    // Update is called once per frame
    void Update()
    {
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
            nextPos = DetermineNextPos();
            state = SnallState.Move;
            originalPos = this.transform.position;
            totalDistance = Vector3.Distance(originalPos, nextPos);
            currDistance = 0.0f;
        }

        float distance = Vector3.Distance(this.transform.position, nextPos);
        if (state == SnallState.Move && distance >= 0.01f)
        {
            currDistance += speed * Time.deltaTime;
            float currRatio = currDistance  / totalDistance;
            this.transform.position = Vector3.Lerp(originalPos, nextPos, currRatio);
        }
        else if (state == SnallState.Move &&  distance <= 0.01f)
        {
            state = SnallState.Defend;
        }

        if (state == SnallState.Defend && defendTimer >= 0)
        {
            defendTimer -= Time.deltaTime;
        }
        else if (state == SnallState.Defend && defendTimer < 0)
        {
            state = SnallState.Idle;
            defendTimer = defendTime;
        }
        
    }

    public Vector3 DetermineNextPos()
    {
        float currX = this.transform.position.x;
        float currY = this.transform.position.z;
        
        // random dir:
        float x = Random.Range(-moveMinRange, moveMaxRange);
        float y = Random.Range(-moveMinRange, moveMaxRange);

        bool isXInBound = currX + x < lowerRight.transform.position.x && currX + x > upperLeft.transform.position.x;
        bool isYInBound = currY + y < upperLeft.transform.position.z && currY + y > lowerRight.transform.position.z;

        while (!isXInBound || !isYInBound) 
        {
            x = Random.Range(-moveMinRange, moveMaxRange);
            y = Random.Range(-moveMinRange, moveMaxRange);
            isXInBound = currX + x < lowerRight.transform.position.x && currX + x > upperLeft.transform.position.x;
            isYInBound = currY + y < upperLeft.transform.position.z && currY + y > lowerRight.transform.position.z;
        }


        return new Vector3(currX + x, this.transform.position.y, currY + y); ;
    }
}
