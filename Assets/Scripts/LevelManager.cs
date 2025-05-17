using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.XR.ARFoundation;

public class LevelManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    [Tooltip("This is the AR plane manager that will be used to obtain the list of available planes")]
    private ARPlaneManager planeManager;

    [SerializeField]
    [Tooltip("This will be the prefab of the diary entries that will be scattered accross the scene.")]
    private GameObject diaryEntryPrefab;

    AudioSource audioSource;

    [SerializeField]
    [Tooltip("This is the text that displays the title.")]
    private TMP_Text titleText;

    private const int CURRENT_LEVEL = 1;
    private int currentPart;

    [SerializeField]
    [Tooltip("This is the audio clip that plays when the level starts.")]
    AudioClip startAudioClip;

    bool locked = false;

    void Start()
    {
        currentPart = 1;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!locked)
            titleText.text = $"Chapter {CURRENT_LEVEL}.{currentPart}";
    }

    public void StartPartOne()
    {
        audioSource.resource = startAudioClip;
        audioSource.PlayOneShot(startAudioClip);
        CreateDiaryEntriesInScene();
    }

    public void CreateDiaryEntriesInScene()
    {
        try
        {
            List<ARPlane> planes = new();
            foreach(ARPlane plane in planeManager.trackables)
            {
                planes.Add(plane);
            }
            if (planes.Count == 0)
                return;
            var chosenPlane = planes[Random.Range(0, planes.Count)];

            //NativeArray<Vector2> boundary2D = chosenPlane.boundary;
            //List<Vector3> boundaryWorld = new(boundary2D.Length);
            //foreach(var v in boundary2D)
            //{
            //    Vector3 localPoint = new(v.x, 0f, v.y);
            //    Vector3 worldPoint = chosenPlane.transform.TransformPoint(localPoint);
            //    boundaryWorld.Add(worldPoint);
            //}
            Vector2 sample2D = RandomPointInPlane(chosenPlane);
            Vector3 worldPos = chosenPlane.transform.TransformPoint(new Vector3(sample2D.x, 0f, sample2D.y));
            Instantiate(diaryEntryPrefab, worldPos, Quaternion.identity);
        }catch (System.Exception e)
        {
            locked = true;
            titleText.text = $"Error creating diary entries: {e.Message}";
        }
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
}
