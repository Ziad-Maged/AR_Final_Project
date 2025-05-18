using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class TaskManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private int currentTaskIndex = 0;

    [SerializeField]
    [Tooltip("This is the list of the types of tasks that will be used in the game.")]
    private List<TaskType> typesOfTasks;
    [SerializeField]
    [Tooltip("This is the maximum number of collectibles or in case of interaction tasks the maximum number of objects")]
    private List<int> maxNumberOfObjects;

    [SerializeField]
    [Tooltip("This will be the prefab of the diary entries that will be scattered accross the scene.")]
    private GameObject diaryEntryPrefab;

    private int currentlyCollected;

    [SerializeField]
    [Tooltip("This is the text that displays the collected items or placed items")]
    private GameObject collectedText;

    private bool taskActive;

    private LevelManager levelManager;
    private ScoreboardManager scoreboardManager;



    void Start()
    {
        currentlyCollected = 0;
        taskActive = false;
        levelManager = GetComponent<LevelManager>();
        scoreboardManager = GetComponent<ScoreboardManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (collectedText.activeSelf)
        {
            switch (typesOfTasks[currentTaskIndex])
            {
                case TaskType.Collection:
                    collectedText.GetComponent<TMP_Text>().text = $"Collected: {currentlyCollected}/{maxNumberOfObjects[currentTaskIndex]}";
                    break;
                case TaskType.Interaction:
                    collectedText.GetComponent<TMP_Text>().text = $"Placed: {currentlyCollected}/{maxNumberOfObjects[currentTaskIndex]}";
                    break;
            }
            if(currentlyCollected >= maxNumberOfObjects[currentTaskIndex])
            {
                taskActive = false;
                collectedText.SetActive(false);
                currentlyCollected = 0;
                currentTaskIndex++;
                levelManager.EndPart();
                if (currentTaskIndex >= typesOfTasks.Count)
                {
                    currentTaskIndex = 0; // Reset to the first task
                }
            }
        }
    }

    public void SkipTask()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("DiaryEntry");
        foreach (GameObject obj in objects)
        {
            Destroy(obj);
        }
        currentlyCollected = maxNumberOfObjects[currentTaskIndex];
        scoreboardManager.DecrementTaskSkips();
    }

    public void StartTask(ARPlaneManager planeManager)
    {
        // Start the task based on the current task index
        switch (typesOfTasks[currentTaskIndex])
        {
            case TaskType.Collection:
                List<ARPlane> planes = new();
                foreach (ARPlane plane in planeManager.trackables)
                {
                    planes.Add(plane);
                }
                if (planes.Count == 0)
                    return;
                for(int i = 0; i < maxNumberOfObjects[currentTaskIndex]; i++)
                {
                    var chosenPlane = planes[Random.Range(0, planes.Count)];
                    Vector2 sample2D = RandomPointInPlane(chosenPlane);
                    Vector3 worldPos = chosenPlane.transform.TransformPoint(new Vector3(sample2D.x, 0f, sample2D.y));
                    Instantiate(diaryEntryPrefab, worldPos, diaryEntryPrefab.transform.rotation);
                }
                break;
            case TaskType.Interaction:
                // Start interaction task
                break;
        }
        collectedText.SetActive(true);
        taskActive = true;
    }

    private Vector2 RandomPointInPlane(ARPlane plane)
    {
        var verts = plane.boundary;
        Vector2 min = verts[0], max = verts[0];
        foreach (var v in verts)
        {
            min = Vector2.Min(min, v);
            max = Vector2.Max(max, v);
        }

        Vector2 sample;
        int tries = 0;
        do
        {
            sample = new Vector2(
                Random.Range(min.x, max.x),
                Random.Range(min.y, max.y)
            );
            tries++;
        }
        while (!IsPointInPolygon(sample, verts) && tries < 30);

        return sample;
    }

    private bool IsPointInPolygon(Vector2 p, NativeArray<Vector2> poly)
    {
        bool inside = false;
        int j = poly.Length - 1;
        for (int i = 0; i < poly.Length; j = i++)
        {
            Vector2 pi = poly[i], pj = poly[j];
            bool intersect = ((pi.y > p.y) != (pj.y > p.y)) &&
                             (p.x < (pj.x - pi.x) * (p.y - pi.y) / (pj.y - pi.y) + pi.x);
            if (intersect)
                inside = !inside;
        }
        return inside;
    }

    public void IncrementCurrentlyCollected()
    {
        if (currentlyCollected < maxNumberOfObjects[currentTaskIndex])
        {
            currentlyCollected++;
        }
    }

    public enum TaskType
    {
        Collection,
        Interaction,
    }
}
