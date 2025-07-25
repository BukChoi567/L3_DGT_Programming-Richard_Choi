using Unity.VisualScripting;
using UnityEngine;

public class Target : MonoBehaviour
{
    // Indicate if the target is been hit
    public bool isHit { get; private set; } = false; 

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isHit) return;

        if (collision.gameObject.CompareTag("Ball"))
        {
            DragShoot ball = collision.gameObject.GetComponent<DragShoot>();
            if (ball != null && ball.hasShot)
            {
                isHit = true;
                gameObject.SetActive(false);
                
                LevelManager.Instance.TargetHit(); // tell LevelManager that a target been hit
            }
            
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isHit = false;
    }

    public void ResetTarget()
    {
        isHit = false;
        gameObject.SetActive(true);
    }   
    
    // Update is called once per frame
    void Update()
    {    


    }
    
}
