using JSAM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellAnimationEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShellClose()
    {
        AudioManager.PlaySound(JSAMSounds.ShellClose);
    }
}
