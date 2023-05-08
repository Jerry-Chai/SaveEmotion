using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAddForce : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("press this mouse has been clicked!");
            Rigidbody2D ballRb = gameObject.GetComponent<Rigidbody2D>();
            float random = Random.Range(0, 1.0f);
            Debug.Log(random);
            ballRb.isKinematic = false;
            ballRb.velocity = Vector2.zero;
            if (random > 0.5f)
            {
                
                ballRb.AddForce(new Vector2(5000, 3000));
            }
            else
            {
                ballRb.AddForce(new Vector2(-5000, 3000));
            }
        }
    }
}
