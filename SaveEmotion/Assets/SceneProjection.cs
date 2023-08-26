using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneProjection : MonoBehaviour
{
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;

    public LineRenderer lineRenderer;
    public Transform ObstaclesParent;

    private void Start()
    {
        CreatePhysicsScene();
    }


    // Start is called before the first frame update
    void CreatePhysicsScene()
    {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();

        foreach (Transform child in ObstaclesParent)
        {
            var ghostObj = Instantiate(child.gameObject, child.position, child.rotation);
            ghostObj.GetComponent<Renderer>().enabled = false;
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
        }
    }


    [SerializeField]
    private int _maxPhysicsFrameIterations = 100;

    public void SimulateTrajectory(GameObject ballPrefab, UnityEngine.Vector3 pos, UnityEngine.Vector3 dir, float velocity)
    {
        var ghostObj = Instantiate(ballPrefab, pos, UnityEngine.Quaternion.identity);
        SceneManager.MoveGameObjectToScene(ghostObj.gameObject, _simulationScene);

        ghostObj.GetComponent<Ball>().Shoot(dir, velocity);

        lineRenderer.positionCount = _maxPhysicsFrameIterations;

        for (var i = 0; i < _maxPhysicsFrameIterations; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime * 5.0f);
            lineRenderer.SetPosition(i, ghostObj.transform.position);
        }

        Destroy(ghostObj.gameObject);
    }

    public void SetLineRendererEnableState(bool state)
    {
        if (lineRenderer.enabled == state) return;
        lineRenderer.enabled = state;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
