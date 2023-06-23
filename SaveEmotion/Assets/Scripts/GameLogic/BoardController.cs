using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{

    public Collider2D boardCollider;
    // Start is called before the first frame update
    void Start()
    {
        boardCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Object collide!");
        StartCoroutine(Sleep());
    }

    IEnumerator Sleep()
    {
        boardCollider.isTrigger = true;
        yield return new WaitForSeconds(1.0f);
        boardCollider.isTrigger = false;

    }

}

