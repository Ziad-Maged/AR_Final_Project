using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.XR.ARFoundation;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    [Tooltip("This is the AR plane manager that will be used to obtain the list of available planes")]
    private ARPlaneManager planeManager;

    [SerializeField]
    [Tooltip("This is the text that displays the title.")]
    private TMP_Text titleText;

    [SerializeField]
    [Tooltip("This is the text that displays the current level.")]
    private int currentLevel;
    private int currentPart;

    AudioSource audioSource;

    [SerializeField]
    [Tooltip("This is the list of audio clips in each part of each level")]
    private List<AudioClip> audioClips;

    private TaskManager taskManager;
    private TouchDetector touchDetector;

    private bool transitioning = false;

    void Start()
    {
        currentPart = 1;
        audioSource = GetComponent<AudioSource>();
        taskManager = GetComponent<TaskManager>();
        touchDetector = GetComponent<TouchDetector>();
    }

    // Update is called once per frame
    void Update()
    {
        titleText.text = $"Chapter {currentLevel}.{currentPart}";

        if(!audioSource.isPlaying && transitioning)
        {
            transitioning = false;
            StartPart();
        }
    }

    public void StartPart()
    {
        audioSource.PlayOneShot(audioClips[currentPart - 1]);
        taskManager.StartTask(planeManager);
        touchDetector.enabled = true;
    }

    public void EndPart()
    {
        touchDetector.enabled = false;
        currentPart++;
        audioSource.PlayOneShot(audioClips[currentPart - 1]);
        transitioning = true;
        currentPart++;
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("DiaryEntry");
        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
    }
}
