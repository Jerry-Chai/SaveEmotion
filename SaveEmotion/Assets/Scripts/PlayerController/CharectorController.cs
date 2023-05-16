using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputY = Input.GetAxis("Vertical");
        
        bool shouldUpdate = false;
        if (inputX < -0.01f || inputX > 0.01f) shouldUpdate = true;
        if (inputY < -0.01f || inputY > 0.01f) shouldUpdate = true;
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
    }
}
