using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ModelPlacement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    [Tooltip("This is the prefab of the character model we want to place in the scene.")]
    private GameObject characterModelPrefab;
    [SerializeField]
    [Tooltip("This is the prefab of the detective model we want to place in the scene.")]
    private GameObject detectiveModelPrefab;
    private ARRaycastManager arRaycastManager;
    private readonly List<ARRaycastHit> hits = new();
    [SerializeField]
    [Tooltip("This defines whether or not the models are facing the player.")]
    private bool facePlayerOnPlacement = true;

    private bool isDetectiveModelPlaced = false;
    private bool isCharacterModelPlaced = false;

    [SerializeField]
    [Tooltip("This is the text on the canvas header to change the title.")]
    private TMP_Text canvasHeaderText;
    [SerializeField]
    [Tooltip("This is the canvas.")]
    private GameObject canvas;
    [SerializeField]
    [Tooltip("This is the scoreboard canvas")]
    private GameObject scoreboardCanvas;


    private void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0 && !isCharacterModelPlaced && !isDetectiveModelPlaced)
        {
            //We place the character model when the user touches the screen
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    Quaternion rotation = facePlayerOnPlacement
                        ? Quaternion.LookRotation(Camera.main.transform.position - hitPose.position) * Quaternion.Euler(0, 180f, 0)
                        : Quaternion.identity;
                    //rotation.y = 0;
                    Instantiate(characterModelPrefab, hitPose.position, rotation);
                    isCharacterModelPlaced = true;
                    //Change the canvas header text to "Detective"
                    canvasHeaderText.text = "Place the Detective";
                }
            }
        }
        else if (Input.touchCount > 0 && isCharacterModelPlaced && !isDetectiveModelPlaced)
        {
            //We place the detective model when the user touches the screen
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    Pose hitPose = hits[0].pose;
                    Quaternion rotation = facePlayerOnPlacement
                        ? Quaternion.LookRotation(Camera.main.transform.position - hitPose.position) * Quaternion.Euler(0, 180f, 0)
                        : Quaternion.identity;
                    //rotation.y = 1;
                    Instantiate(detectiveModelPrefab, hitPose.position, rotation);
                    isDetectiveModelPlaced = true;
                    canvas.SetActive(false); // Hide the canvas after placing the detective model
                    scoreboardCanvas.SetActive(true); // Show the scoreboard canvas
                    enabled = false; // Disable this script to prevent further placements
                }
            }
        }
    }
}
