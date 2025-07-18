using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    private Target[] targets;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else
            Destroy(gameObject); // make sure object not destroyed on scene load

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targets = FindObjectsByType<Target>(FindObjectsSortMode.None);
    }

    public void TargetHit()
    {
        if (AllTargetsCleared())
        {
            Debug.Log("Level done");

        }
    }

    private bool AllTargetsCleared()
    {
        foreach (Target t in targets)
        {
            if (!t.isHit)
            {
                return false;
            }
        }
        return true;
    }

    public void ResetLevel()
    {
        foreach (Target t in targets)
        {
            t.ResetTarget();
        }

        //Reset ball
        DragShoot ball = FindFirstObjectByType<DragShoot>();
        if (ball != null)
        {
            ball.ResetBall();
        }
    }


    
    // Update is called once per frame
    void Update()
    {
        
    }
}
