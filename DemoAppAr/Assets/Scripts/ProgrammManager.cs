using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ProgrammManager : MonoBehaviour
{
    public GameObject PlaneMarkerPrefab;

    private ARRaycastManager ARRaycastManagerScript;

    private Vector2 TouchPosition;

    public GameObject ObjectToSpawn;
    private GameObject SelectedObject;
    [SerializeField] private Camera ARCamera;
    List<ARRaycastHit> hits = new List<ARRaycastHit>();
    void Start()
    {
        ARRaycastManagerScript = FindObjectOfType<ARRaycastManager>();

        PlaneMarkerPrefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
       ShowMarkerAndSetObject();
       MoveObject();
    }

    void ShowMarkerAndSetObject()
    {
        

        ARRaycastManagerScript.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);

        // show marker
        if (hits.Count > 0)
        {
            PlaneMarkerPrefab.transform.position = hits[0].pose.position;
            PlaneMarkerPrefab.SetActive(true);
        }
       // set object
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            Instantiate(ObjectToSpawn, hits[0].pose.position, ObjectToSpawn.transform.rotation);
            PlaneMarkerPrefab.SetActive(false);
        }
    }

    void MoveObject()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            TouchPosition = touch.position;
            
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = ARCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;

                if (Physics.Raycast(ray, out hitObject))
                {
                    if (hitObject.collider.CompareTag("UnSelected"))
                    {
                        hitObject.collider.gameObject.tag = "Selected";
                    }
                }
            }

            if (touch.phase == TouchPhase.Moved)
            {
                ARRaycastManagerScript.Raycast(TouchPosition, hits, TrackableType.Planes);
                SelectedObject = GameObject.FindWithTag("Selected");
                SelectedObject.transform.position = hits[0].pose.position;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                if (SelectedObject.CompareTag("Selected"))
                {
                    SelectedObject.tag = "UnSelected";
                }
            }

        }
    }    
    

    
}