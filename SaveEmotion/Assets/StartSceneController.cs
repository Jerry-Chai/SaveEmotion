using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class StartSceneController : MonoBehaviour
{
    GameObject partA;
    GameObject partB;
    GameObject partC;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator TriggerUltraSkill()
    {
        //if (currentEnergy > 3)
        //{
        //    currentEnergy = 0;
        //    //UIManager.Instance._uiList["UIManagement.UISkillPanel"].OnUpdate(currentEnergy / 4.0f);
        //    //GridManager.Instance.TriggerSkill(ball.transform.position, 2, 2);
        //    StartCoroutine(UpdateEnergyMat(0, 2.0f));
        //    TriggerSkillPos = ball.transform.position;
            //GameObject CollisionEffect = Instantiate(CollisionEffectPrefab);
            //CollisionEffect.transform.position = this.transform.position; ;
            //CollisionEffect.SetActive(true);
            //CollisionEffect.transform.localScale = 3.0f * Vector3.one;
            //CollisionEffect.GetComponent<ParticleSystem>().Play();
            //Debug.Log("Play Collision Effect");
            yield return new WaitForSeconds(0.5f);

            // create Sphere
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = this.transform.position;
            sphere.transform.localScale = 30.0f * Vector3.one;
            sphere.gameObject.name = "Collision";
            sphere.gameObject.tag = "SkillRange";
            sphere.GetComponent<MeshRenderer>().enabled = false;
            sphere.GetComponent<Collider>().isTrigger = true;

            yield return new WaitForSeconds(0.5f);
            Destroy(sphere);
            //yield return new WaitForSeconds(1.0f);
            //Destroy(CollisionEffect);

        //}
        // 1 means padding 1 grid;
        Debug.Log("Trigger Skill");

    }
}


//public class StartSceneController
