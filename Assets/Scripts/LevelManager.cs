using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class LevelManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    AudioSource audioSource;

    [SerializeField]
    [Tooltip("This is the text that displays the title.")]
    private TMP_Text titleText;

    private const int CURRENT_LEVEL = 1;
    private int currentPart;

    [SerializeField]
    [Tooltip("This is the audio clip that plays when the level starts.")]
    AudioClip startAudioClip;

    void Start()
    {
        currentPart = 1;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        titleText.text = $"Chapter {CURRENT_LEVEL}.{currentPart}";
    }

    public void StartPartOne()
    {
        audioSource.resource = startAudioClip;
        audioSource.PlayOneShot(startAudioClip);
    }
}
