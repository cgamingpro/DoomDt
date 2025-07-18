using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DungenGenrator : MonoBehaviour
{
    public static DungenGenrator Instance { get; private set; }

    [SerializeField] GameObject entrance;
    [SerializeField] List<GameObject> rooms = new List<GameObject>();
    [SerializeField] List<GameObject> specailroom = new List<GameObject>();
    [SerializeField] List<GameObject> alternateEntrance = new List<GameObject>();
    [SerializeField] List<GameObject> hallways = new List<GameObject>();
    [SerializeField] GameObject door;
    [SerializeField] int numberofrooms;
    [SerializeField] LayerMask roomLayer;
    [SerializeField] NavMeshSurface navMeshSurface; 

    List<DungenPart> genratedRoomSL;
    bool isgenrated = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        genratedRoomSL = new List<DungenPart>();
        StartGenration();
    }

    public void StartGenration()
    {
        Genrate();
        AlternateEgenrate();
        FillEmptyEntrance();
        isgenrated = true;

        navMeshSurface.BuildNavMesh();
       // Debug.Log("navmeshGenraes");
    }

    void Genrate()
    {
       // Debug.Log("genration started");
        for (int i = 0; i < numberofrooms - alternateEntrance.Count; i++)
        {
            if (genratedRoomSL.Count < 1)
            {
               // Debug.Log("base spawned");
                GameObject genratedroom = Instantiate(entrance, transform.position, transform.rotation);
                genratedroom.transform.parent = navMeshSurface.transform;//this
                if (genratedroom.TryGetComponent<DungenPart>(out DungenPart dungenPart))
                {
                    genratedRoomSL.Add(dungenPart);
                }
            }
            else
            {
                bool shouldPlaceHallway = Random.Range(0f, 1f) > 0.5f;
                DungenPart randomGenratedRoom = null;
                Transform romm1entrypoint = null;

                for (int retry = 0; retry < 800; retry++)
                {
                    int randomIndex = Random.Range(0, genratedRoomSL.Count);
                    if (genratedRoomSL[randomIndex].HasAvialableEntryPoint(out romm1entrypoint))
                    {
                        randomGenratedRoom = genratedRoomSL[randomIndex];
                        break;
                    }
                }

                if (randomGenratedRoom == null || romm1entrypoint == null)
                {
                   // Debug.LogWarning("Failed to find a valid entrypoint after retries. Skipping iteration.");
                    continue;
                }

                GameObject doorToAllign = Instantiate(door);
                doorToAllign.transform.parent = navMeshSurface.transform;//this
                doorToAllign.transform.position = romm1entrypoint.position;
                doorToAllign.transform.rotation = romm1entrypoint.rotation;

                if (shouldPlaceHallway)
                {
                    int randomIndex = Random.Range(0, hallways.Count);
                    GameObject genratedHallway = Instantiate(hallways[randomIndex]);
                    genratedHallway.transform.parent = navMeshSurface.transform;//this

                    if (!genratedHallway.TryGetComponent(out DungenPart dungenpart)) continue;
                    if (!dungenpart.HasAvialableEntryPoint(out Transform room2Entrypoint)) continue;

                    AlignRoom(randomGenratedRoom.transform, genratedHallway.transform, romm1entrypoint, room2Entrypoint);

                    if (HandelInterscetion(dungenpart))
                    {
                        dungenpart.UnuseEntryPoint(room2Entrypoint);
                        randomGenratedRoom.UnuseEntryPoint(romm1entrypoint);
                        Destroy(genratedHallway);
                        Destroy(doorToAllign);
                        continue;
                    }

                    genratedRoomSL.Add(dungenpart);
                }
                else
                {
                    GameObject roomPrefab = rooms[Random.Range(0, rooms.Count)];
                    if (specailroom.Count > 0 && Random.Range(0f, 1f) > 0.9f)
                    {
                        roomPrefab = specailroom[Random.Range(0, specailroom.Count)];
                    }

                    GameObject genratedRoom = Instantiate(roomPrefab);
                    genratedRoom.transform.parent = navMeshSurface.transform;//this
                    if (!genratedRoom.TryGetComponent(out DungenPart dungenPart)) continue;
                    if (!dungenPart.HasAvialableEntryPoint(out Transform room2Entrypoint)) continue;

                    AlignRoom(randomGenratedRoom.transform, genratedRoom.transform, romm1entrypoint, room2Entrypoint);

                    if (HandelInterscetion(dungenPart))
                    {
                        dungenPart.UnuseEntryPoint(room2Entrypoint);
                        randomGenratedRoom.UnuseEntryPoint(romm1entrypoint);
                        Destroy(genratedRoom);
                        Destroy(doorToAllign);
                        continue;
                    }

                    genratedRoomSL.Add(dungenPart);
                }
            }
        }
    }

    void AlternateEgenrate() { }

    void FillEmptyEntrance()
    {
        foreach (var item in genratedRoomSL)
        {
            item.FillEmptyWalls();
        }
    }

    private void RetryPlacment(GameObject itemToPlace, GameObject doorToPlace)
    {
        for (int i = 0; i < 100; i++)
        {
            int randomIndex = Random.Range(0, genratedRoomSL.Count);
            DungenPart baseRoom = genratedRoomSL[randomIndex];
            if (!baseRoom.HasAvialableEntryPoint(out Transform entry1)) continue;

            if (!itemToPlace.TryGetComponent(out DungenPart part)) return;
            if (!part.HasAvialableEntryPoint(out Transform entry2)) return;

            AlignRoom(baseRoom.transform, itemToPlace.transform, entry1, entry2);
            doorToPlace.transform.position = entry1.position;
            doorToPlace.transform.rotation = entry1.rotation;

            if (!HandelInterscetion(part))
            {
                genratedRoomSL.Add(part);
                return;
            }

            part.UnuseEntryPoint(entry2);
            baseRoom.UnuseEntryPoint(entry1);
        }

        Debug.LogWarning("RetryPlacement failed after 100 attempts.");
        Destroy(itemToPlace);
        Destroy(doorToPlace);
    }

    private bool HandelInterscetion(DungenPart dungenpart)
    {
        Collider[] hits = Physics.OverlapBox(dungenpart.collider.bounds.center, dungenpart.collider.bounds.size / 2, Quaternion.identity, roomLayer);
        foreach (Collider hit in hits)
        {
            if (hit != dungenpart.collider)
                return true;
        }
        return false;
    }

    private void AlignRoom(Transform room1, Transform room2, Transform room1entry, Transform room2entry)
    {
        float angle = Vector3.SignedAngle(room2entry.forward, -room1entry.forward, Vector3.up);
        room2.Rotate(Vector3.up, angle);
        Vector3 offset = room1entry.position - room2entry.position;
        room2.position += offset;
        Physics.SyncTransforms();
    }
}
