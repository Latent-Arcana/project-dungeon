using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraBehavior : MonoBehaviour
{

    public GameObject player;

    [Header("DEBUG")]
    public bool _CameraFollowPlayer;
    public bool _PlayerCameraZoom;

    public InputController input;

    private Camera mapCamera;
    private Camera mainCamera;


    void Awake()
    {
        player = GameObject.Find("Player");

        mainCamera = gameObject.GetComponent<Camera>();
        mapCamera = GameObject.Find("Map Camera").GetComponent<Camera>();


        input = GameObject.Find("InputController").GetComponent<InputController>();

        if (_PlayerCameraZoom)
        {
            this.GetComponent<Camera>().orthographicSize = 10;
        }

        if (_CameraFollowPlayer)
        {
            MoveCameraFollowPlayer();
        }

        mapCamera.enabled = false;

    }

    private void Input_OnMapEnter(object sender, System.EventArgs e)
    {
        mapCamera.enabled = !mapCamera.enabled;
        mainCamera.enabled = !mainCamera.enabled;
    }

    private void OnEnable()
    {
        input.OnMapEnter += Input_OnMapEnter;
    }

    private void OnDisable()
    {
        input.OnMapEnter -= Input_OnMapEnter;
    }

    public void MoveCameraFollowPlayer()
    {
        if (_CameraFollowPlayer)
        {
            float playerX = player.transform.position.x;
            float playerY = player.transform.position.y;
            float cameraZ = this.gameObject.transform.position.z;

            this.gameObject.transform.position = new Vector3(playerX, playerY, cameraZ);
        }
    }


}
