using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Animations;
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

    int selectedIndex;


    void Awake()
    {
        mapMenuUI = GameObject.Find("MapUI").GetComponent<MapMenuUI>();
        selectedPrefab = mapMarkerSafe;
        selectedIndex = 0;
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
                    bool isDiscoveredRoom = false;
                    //only place a map marker in a room or over an existing map marker

                    Room roomData = hit.collider.gameObject.GetComponentInParent<Room>(); // the room object is actually on the parent of the map object
                    Debug.Log("getting the parent of " + hit.collider.gameObject.name);

                    // We also only want to place the marker if the room has been discovered
                    if (roomData != null)
                    {
                        isDiscoveredRoom = roomData.discovered;
                    }

                    if (isDiscoveredRoom || hit.collider.gameObject.CompareTag("mapMark"))
                    {
                        GameObject placedMarker = Instantiate(selectedPrefab, new Vector3(ray.origin.x, ray.origin.y, 0f), Quaternion.identity);
                        Animator markerAnimator = placedMarker.GetComponent<Animator>();

                        string clipName;

                        if (markerAnimator != null)
                        {
                            switch (selectedIndex)
                            {
                                case 0:
                                    clipName = "safe-animation-mark";
                                    break;
                                case 1:
                                    clipName = "lore-animation-mark";
                                    break;
                                case 2:
                                    clipName = "danger-animation-mark";
                                    break;
                                default:
                                    clipName = null;
                                    break;
                            }

                            if (clipName != null)
                            {
                                markerAnimator.Play(clipName);
                                //StartCoroutine(ResetToIdle(markerAnimator, clipName));
                            }

                        }
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

    private IEnumerator ResetToIdle(Animator animator, string animationName)
    {
        // Wait for the duration of the animation
        float animationLength = animator.runtimeAnimatorController.animationClips.First(clip => clip.name == animationName).length / 3;
        yield return new WaitForSeconds(animationLength);

        // Play the idle state or any default animation
        animator.Play("None");
    }

    private void Map_OnMarkerChange(object sender, MapMenuUI.MarkerArgs e)
    {
        switch (e.markerId)
        {
            case 0:
                selectedPrefab = mapMarkerSafe;
                selectedIndex = 0;
                break;
            case 1:
                selectedPrefab = mapMarkerLore;
                selectedIndex = 1;
                break;
            case 2:
                selectedPrefab = mapMarkerDanger;
                selectedIndex = 2;
                break;
            default:
                break;

        }
    }

    public void PlacePresetMarker(Room room)
    {
        GameObject prefab;
        if (room.roomType == Enums.RoomType.Safe)
        {
            prefab = mapMarkerSafe;
        }
        else if (room.roomType == Enums.RoomType.Danger)
        {
            prefab = mapMarkerDanger;
        }
        else if (room.roomType == Enums.RoomType.Lore)
        {
            prefab = mapMarkerLore;
        }
        else
        {
            prefab = mapMarkerSafe;
        }

        int centerX = room.x + (room.width / 2);
        int centerY = room.y + (room.height / 2);

        GameObject placedMarker = Instantiate(prefab, new Vector3(centerX - 500, centerY - 500, 0f), Quaternion.identity);
        scoreController.SetRoomAsEntered(room.roomId);
    }


}
