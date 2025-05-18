using UnityEngine;

public class TouchDetector : MonoBehaviour
{
    private TaskManager taskManager;
    private ScoreboardManager scoreboardManager;

    private void Start()
    {
        taskManager = GetComponent<TaskManager>();
        scoreboardManager = GetComponent<ScoreboardManager>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if there is at least one touch
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Check if the touch just began
            if (touch.phase == TouchPhase.Began)
            {
                // Create a ray from the touch position
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;

                // Perform the raycast
                if (Physics.Raycast(ray, out hit))
                {
                    // Check if the hit object has the desired tag
                    if (hit.collider != null && hit.collider.CompareTag("DiaryEntry"))
                    {
                        Destroy(hit.collider.gameObject);
                        taskManager.IncrementCurrentlyCollected();
                        scoreboardManager.AddScore(10);
                    }else if (hit.collider != null && hit.collider.CompareTag("Character") && scoreboardManager.GetCurrentNumberOfTaskSkipsUsed() >= 0)
                    {
                        taskManager.SkipTask();
                    }
                }
            }
        }
    }
}
