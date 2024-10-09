using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.UIElements;

public class MapMarker : MonoBehaviour
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

    private GameObject selectedPrefab;

    private MapMenuUI mapMenuUI;

    private ScoreController scoreController;

    public InputController input;


    void Awake()
    {
        mapMenuUI = GameObject.Find("MapUI").GetComponent<MapMenuUI>();
        selectedPrefab = mapMarkerSafe;
        scoreController = GameObject.Find("ScoreController").GetComponent<ScoreController>();
        input = GameObject.Find("InputController").GetComponent<InputController>();
    }

    private void OnEnable()
    {
        mapMenuUI.OnMarkerChange += Map_OnMarkerChange;
    }

    private void OnDisable()
    {
        mapMenuUI.OnMarkerChange -= Map_OnMarkerChange;
    }

    void Update()
    {
        //only perform map actions while in the map interface
        if (input.currentInputState == InputController.InputState.MapMenu)
        {
            // add the markers
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                RaycastHit hit;
                Ray ray = mapCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    //only place a map marker in a room or over an existing map marker
                    if (hit.collider.gameObject.CompareTag("room") || hit.collider.gameObject.CompareTag("mapMark"))
                    {
                        Instantiate(selectedPrefab, new Vector3(ray.origin.x, ray.origin.y, 0f), Quaternion.identity);
                    }
                }

            }

            // delete the markers
            if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                RaycastHit hit;
                Ray ray = mapCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    //only try and remove map markers, not other things you collide with
                    if (hit.collider.gameObject.CompareTag("mapMark"))
                    {
                        //Remove marker from score and Destroy it
                        scoreController.RemoveRoomMark(hit.collider.gameObject);
                    }

                }
            }
        }
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
