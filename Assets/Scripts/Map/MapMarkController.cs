using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class MapMarkController : MonoBehaviour
{
    [SerializeField]
    Camera mapCamera;

    [Header("Map Markers")]
    [SerializeField]
    GameObject mapMarkerSafe;

    [SerializeField]
    GameObject mapMarkerLore;

    [SerializeField]
    GameObject mapMarkerDanger;


    //maybe get rid of this in favor of tagging only within room bounds
    [Header("Map Bounds")]
    int xMin = -500;
    int xMax = -450;
    int yMin = -500;
    int yMax = -450;


    private GameObject selectedPrefab;

    private MapMenuUI mapMenuUI;

    private ScoreController scoreController;

    private bool mapIsActive = false;


    void Awake()
    {
        mapMenuUI = GameObject.Find("MapUI").GetComponent<MapMenuUI>();
        selectedPrefab = mapMarkerSafe;
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();

    }

    private void OnEnable()
    {
        mapMenuUI.OnMarkerChange += Map_OnMarkerChange;
        mapMenuUI.OnMapToggle += Map_OnMapToggle;
    }

    private void OnDisable()
    {
        mapMenuUI.OnMarkerChange -= Map_OnMarkerChange;
        mapMenuUI.OnMapToggle -= Map_OnMapToggle;
    }

    // Update is called once per frame
    void Update()
    {
        //only perform map actions while in the map interface
        if (mapIsActive)
        {
            // add the markers
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                //RaycastHit hit;
                Ray ray = mapCamera.ScreenPointToRay(Input.mousePosition);
                //Debug.Log(ray.origin);

                if (ray.origin.x > xMin && ray.origin.x < xMax && ray.origin.y > yMin && ray.origin.y < yMax)
                {
                    Instantiate(selectedPrefab, new Vector3(ray.origin.x, ray.origin.y, 0f), Quaternion.identity);
                }
            }

            // delete the markers
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                RaycastHit hit;
                Ray ray = mapCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {

                    if (!hit.collider.gameObject.CompareTag("room"))
                    {

                        //Remove marker from score and Destroy it
                        scoreController.RemoveRoomMark(hit.collider.gameObject);

                        //Destroy(hit.collider.gameObject);
                    }

                }
            }
        }
    }

    private void Map_OnMapToggle(object sender, EventArgs e)
    {
        mapIsActive = !mapIsActive;
    }

    private void Map_OnMarkerChange(object sender, MapMenuUI.MarkerArgs e)
    {
        switch (e.markerId)
        {
            case 0:
                selectedPrefab = mapMarkerSafe;
                break;
            case 1:
                selectedPrefab = mapMarkerLore;
                break;
            case 2:
                selectedPrefab = mapMarkerDanger;
                break;
            default:
                break;

        }
    }

}
