using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FloorCoveringGeneration : MonoBehaviour
{

    [SerializeField]
    public float noiseScale;

    [SerializeField]
    GameObject[] mossPrefabs;


    List<FloorCoveringInstanceData> mossInstances = new List<FloorCoveringInstanceData>();

    public void GenerateGroundCover(List<GameObject> rooms)
    {
        foreach (GameObject roomObj in rooms)
        {
            // Get the room script
            Room room = roomObj.GetComponent<Room>();

            // Create a place in the heirarchy where we can put our floor cover
            GameObject floorCover = new GameObject("FloorCover");
            floorCover.transform.parent = room.gameObject.transform.GetChild(1).transform;

            for (int x = room.x; x < (room.x + room.width); x++)
            {
                for (int y = room.y; y < (room.y + room.height); y++)
                {
                    float sample = Mathf.PerlinNoise(x * noiseScale, y * noiseScale);

                    Vector3Int pos = new Vector3Int(x, y, 0);

                    if (sample > 0.5f)
                    {
                        GameObject selectedPrefab = mossPrefabs[UnityEngine.Random.Range(0, mossPrefabs.Length)];
                        Quaternion randomRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(0, 4) * 90);
                        mossInstances.Add(new FloorCoveringInstanceData(selectedPrefab, pos, randomRotation, floorCover.transform));
                        //Instantiate(selectedPrefab, pos, randomRotation);
                    }
                }
            }
        }

        // Batch instantiate moss
        foreach (FloorCoveringInstanceData instance in mossInstances)
        {
            PlaceFloorCover(instance);
        }

    }

    private void PlaceFloorCover(FloorCoveringInstanceData instance)
    {

        GameObject cover = Instantiate(instance.Prefab, instance.Position, instance.Rotation);

        cover.transform.parent = instance.Parent;

        LayerMask mask = LayerMask.GetMask("Default"); //we only care about colliding on default for now, but we should add in other layers here if needed

        SpriteRenderer sr = cover.GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.40f);
        
    }


    // Helper struct to store prefab, position, and rotation data
    private struct FloorCoveringInstanceData
    {
        public GameObject Prefab;
        public Vector3 Position;
        public Quaternion Rotation;
        public Transform Parent;

        public FloorCoveringInstanceData(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            Prefab = prefab;
            Position = position;
            Rotation = rotation;
            Parent = parent;
        }
    }
}



