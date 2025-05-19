using UnityEngine;

public class TouchDetector : MonoBehaviour
{
    private TaskManager taskManager;
    private ScoreboardManager scoreboardManager;

    [SerializeField]
    [Tooltip("This is the light prefab that will be displayed on top of the diaries and markers")]
    private GameObject lightPrefab;
    [SerializeField]
    [Tooltip("This is the prefab of the diary entries that will be instantiated once we tap the marker.")]
    private GameObject diaryEntryPrefab;

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
                    }else if (hit.collider != null && hit.collider.CompareTag("Marker"))
                    {
                        // Instantiate the diary entry prefab at the marker's position
                        GameObject diaryEntry = Instantiate(diaryEntryPrefab, hit.collider.transform.position, diaryEntryPrefab.transform.rotation);
                        Destroy(hit.collider.gameObject);
                        taskManager.IncrementCurrentlyCollected();
                        scoreboardManager.AddScore(10);
                    }else if (hit.collider != null && hit.collider.CompareTag("Character") && scoreboardManager.GetCurrentNumberOfTaskSkipsUsed() > 0)
                    {
                        taskManager.SkipTask();
                    }else if (hit.collider != null && hit.collider.CompareTag("Detective") && scoreboardManager.GetCurrentHintsUsed() > 0)
                    {
                        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("DiaryEntry");
                        if(gameObjects.Length == 0)
                        {
                            gameObjects = GameObject.FindGameObjectsWithTag("Marker");
                        }
                        GameObject selectedObject = gameObjects[Random.Range(0, gameObjects.Length)];
                        GameObject light = Instantiate(lightPrefab, selectedObject.transform.position, Quaternion.identity);
                        Destroy(light, 5.0f);
                        scoreboardManager.DecrementHintsUsed();
                    }
                }
            }
        }
    }
}
