using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
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
    private int part;

    [SerializeField]
    [Tooltip("This is the text that displays the skip narration button.")]
    private GameObject skipNarrationText;

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
        part = 0;
        audioSource = GetComponent<AudioSource>();
        taskManager = GetComponent<TaskManager>();
        touchDetector = GetComponent<TouchDetector>();
    }

    // Update is called once per frame
    void Update()
    {
        titleText.text = $"Chapter {currentLevel}.{part}";

        if(!audioSource.isPlaying && transitioning)
        {
            transitioning = false;
            StartPart();
        }
    }

    public void SkipNarration()
    {
        if(audioSource.isPlaying && transitioning)
        {
            audioSource.Stop();
            skipNarrationText.SetActive(false);
        }
    }

    public void StartPart()
    {
        part++;
        if (part > 4 && currentLevel < 2)
        {
            touchDetector.enabled = true;
            SceneManager.LoadScene("Level2");
            return;
        }
        audioSource.PlayOneShot(audioClips[currentPart - 1]);
        taskManager.StartTask(planeManager);
        touchDetector.enabled = true;
    }

    public void EndPart()
    {
        touchDetector.enabled = false;
        currentPart++;
        audioSource.PlayOneShot(audioClips[currentPart - 1]);
        skipNarrationText.SetActive(true);
        transitioning = true;
        currentPart++;
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("DiaryEntry");
        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
        gameObjects = GameObject.FindGameObjectsWithTag("Marker");
        foreach (GameObject gameObject in gameObjects)
        {
            Destroy(gameObject);
        }
    }
}
