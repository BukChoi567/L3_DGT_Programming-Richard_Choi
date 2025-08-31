using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    // Array to hold all targets in the level
    private Target[] targets;
    // Reference to finish menu canvas
    public Canvas FinishMenu;
    // Reference to ball
    public GameObject Circle;

    public void NextLevel(int sceneIndex) // Load next level
    {
        SceneManager.LoadScene(sceneIndex);
        Time.timeScale = 1f; // Reset time scale to normal
    }
    

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
        // Find all targets in the scene and store in array
        targets = FindObjectsByType<Target>(FindObjectsSortMode.None);
    }

    public void TargetHit()
    {
        if (AllTargetsCleared())
        {
            // if all targets are hit, start coroutine to wait for 2 seconds before finishing level
            StartCoroutine(TargetGone());
            StopCoroutine(TargetGone());
        }
    }
    

    private void LevelFinish()
    {
        Circle.SetActive(false); //Hide circle
        FinishMenu.gameObject.SetActive(true); // Show finish menu
        Time.timeScale = 0f; // Stop time to pause the game
        
    }

    public bool AllTargetsCleared()
    {
        // Check if all targets have been hit, return false if any target is not hit, true if all are hit
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
        // Reset all targets
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

    // Coroutine to wait for 2 seconds before finishing level
    private IEnumerator TargetGone()
    {
        // Wait for 2 seconds
        float elapsed = 0f;
        float duration = 2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        // check if ball has touched any bad blocks
        DragShoot ball = FindFirstObjectByType<DragShoot>();
        if (ball.HasTouchedBad == false)
        {
            LevelFinish(); // Finish the level
        }

    }
    

}
