using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    private Target[] targets;
    public Canvas FinishMenu;

    private BadBlock bad;
    public GameObject Circle;

    public void NextLevel(int sceneIndex)
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
        targets = FindObjectsByType<Target>(FindObjectsSortMode.None);
    }

    public void TargetHit()
    {
        if (AllTargetsCleared())
        {
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

    private IEnumerator TargetGone()
    {
        float elapsed = 0f;
        float duration = 2f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            yield return null; // Wait for the next frame
        }
        bad = FindFirstObjectByType<BadBlock>();
        if (bad.HasTouchedBad == false)
        {
            LevelFinish(); // Finish the level
        }
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    

}
