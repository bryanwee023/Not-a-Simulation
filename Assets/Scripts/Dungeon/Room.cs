using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Room : MonoBehaviour
{
    private enum Direction { TOP, BOTTOM, LEFT, RIGHT, NONE }
    public enum Size { SMALL, MEDIUM , LARGE };

    private static Dictionary<Size, string> wavePath = new Dictionary<Size, string>()
    {
        [Size.SMALL] = "Waves/Floor {0}/Small",
        [Size.MEDIUM] = "Waves/Floor {0}/Medium",
        [Size.LARGE] = "Waves/Floor {0}/Large"
    };

    private static Dictionary<Size, float> rewardMultiplier = new Dictionary<Size, float>()
    {
        [Size.SMALL] = 1,
        [Size.MEDIUM] = 1.5f,
        [Size.LARGE] = 2
    };

    private static Direction OtherDir(Direction dir)
    {
        switch (dir)
        {
            case Direction.TOP:
                return Direction.BOTTOM;
            case Direction.BOTTOM:
                return Direction.TOP;
            case Direction.LEFT:
                return Direction.RIGHT;
            case Direction.RIGHT:
                return Direction.LEFT;
        }
        return Direction.TOP;;
    }

    public static List<Room> rooms = new List<Room>();

    public static Room Get(int index)
    {
        return rooms[index];
    }

    public static void Reset()
    {
        rooms.Clear();
    }

    public Size size;

    [Header("Door Transforms")]
    public Transform topDoor;
    public Transform bottomDoor;
    public Transform leftDoor;
    public Transform rightDoor;

    [SerializeField]
    private int length_x;
    [SerializeField]
    private int length_z;

    [HideInInspector]
    public bool cleared = false;
    private bool activated = false;
    private Direction enteredFrom = Direction.NONE;

    [Header("Dungeon Prefabs")]
    [SerializeField]
    private GameObject platform;
    [SerializeField]
    private GameObject floorExit;
    [SerializeField]
    private GameObject expReward;
    [SerializeField]
    private GameObject healthReward;
    [SerializeField]
    private GameObject skillReward;

    private EnemyWave wave;
    private bool terminalRoom;

    private void Awake()
    {
        rooms.Add(this);
    }
    
    private void Start()
    {
        this.LoadWaves();

        NavMeshSurface[] surfaces = this.GetComponentsInChildren<NavMeshSurface>();

        foreach (NavMeshSurface surface in surfaces)
        {
            surface.BuildNavMesh();
        }
    }

    private void LoadWaves()
    {
        string path = string.Format(wavePath[this.size], WorldState.worldLevel);
        EnemyWave[] waves = Resources.LoadAll<EnemyWave>(path);

        int index = Random.Range(0, waves.Length - 1);
        this.wave = waves[index];
        
    }


    private void Update()
    {
        if (activated && EnemyController.AllCleared())
        {
            
            this.cleared = true;
            this.activated = false;
            WorldState.roomsCleared++;

            Connect();

            this.SpawnReward();
            
        }
    }

    private void SpawnReward()
    {
        bool flipCoin = Random.Range(0f, 1f) < 0.75f;

        Vector3 spawnPoint = PlayerState.player.position + 6 * PlayerState.player.forward;
        if (this.terminalRoom || WorldState.roomsCleared == 1)
        {
            Instantiate(this.skillReward, spawnPoint, Quaternion.identity);
        }
        else if (flipCoin)
        {
            var reward = Instantiate(this.expReward, spawnPoint, Quaternion.identity);
            reward.GetComponent<ExpPickUp>().amount = (int)(50 * rewardMultiplier[this.size]);
        }
        else
        {
            var reward = Instantiate(this.healthReward, spawnPoint, Quaternion.identity);
            reward.GetComponent<HealthBoost>().amount = (int)(15 * rewardMultiplier[this.size]);
        }
    }
    public void SpawnEnemies()
    {
        activated = true;

        Invoke("PlaySpawnSFX", 1);
        foreach (GameObject enemy in wave.Get())
        {
            Vector3 pos = RandomPoint();
            if (pos != Vector3.zero)
                Instantiate(enemy, pos, Quaternion.identity);
        }
    }

    private void PlaySpawnSFX()
    {
        AudioManager.instance.spawn.Play();
    }

    // Returns a random point on the walkable mesh
    private Vector3 RandomPoint()
    {
        Vector3 randomPoint;
        NavMeshHit hit;

        int ctr = 0;
        do
        {
            float rand_x = Random.Range(- length_x, length_x) / 2f;
            float rand_z = Random.Range(- length_z, length_z) / 2f;

            rand_x += this.transform.position.x;
            rand_z += this.transform.position.z;

            randomPoint = new Vector3(rand_x, 0, rand_z);

            if (ctr++ > 5) { Debug.Log("failed to find point"); return Vector3.zero; }

        } while (!NavMesh.SamplePosition(randomPoint, out hit, 1f, NavMesh.AllAreas));  //TODO: Assign correct area

        return hit.position;
    }

    private Transform GetDoor(Direction dir)
    {
        switch (dir)
        {
            case Direction.TOP:
                return this.topDoor;
            case Direction.BOTTOM:
                return this.bottomDoor;
            case Direction.LEFT:
                return this.leftDoor;
            case Direction.RIGHT:
                return this.rightDoor;
        }
        return null;
    }

    #region Connect Hallways

    public void Connect()
    {
        bool connected = ConnectNeighbour(Direction.TOP);
        connected = ConnectNeighbour(Direction.BOTTOM) || connected;
        connected = ConnectNeighbour(Direction.LEFT) || connected;
        connected = ConnectNeighbour(Direction.RIGHT) || connected;

        if (!connected)
        {
            terminalRoom = true;
            Direction exitDir = OtherDir(this.enteredFrom);
            Transform exit = this.GetDoor(exitDir);
            Instantiate(floorExit, exit.position, exit.rotation);
        }
    }

    private bool ConnectNeighbour(Direction dir)
    {
        if (this.enteredFrom == dir) return false;

        Direction otherDir = OtherDir(dir);

        float min = float.MaxValue;
        Room nearest = null;
        foreach(Room r in rooms)
        {
            if (r == this) continue;

            float dist = Vector3.Distance(this.GetDoor(dir).position, r.GetDoor(otherDir).position);
            if (dist < min)
            {
                min = dist;
                nearest = r;
            }
        }

        //Debug.Log("Nearest neighbour on the " + dir + ": " + nearest.transform.position);

        if (ValidNeighbour(nearest, dir))
        {
            DrawHallway(this.GetDoor(dir), nearest.GetDoor(otherDir), nearest.SpawnEnemies);
            nearest.enteredFrom = otherDir;
            return true;

        } else return false;
    }

    private bool ValidNeighbour(Room neighbour, Direction dir)
    {
        if (neighbour == null || neighbour.cleared == true) return false;

        Vector3 pos1 = this.GetDoor(dir).position;
        Vector3 pos2 = neighbour.GetDoor(OtherDir(dir)).position;

        switch (dir)
        {
            case Direction.TOP:
                return pos1.z < pos2.z;
            case Direction.BOTTOM:
                return pos1.z > pos2.z;
            case Direction.LEFT:
                return pos1.x > pos2.x;
            case Direction.RIGHT:
                return pos1.x < pos2.x;
        }
        return false;
    }

    private void DrawHallway(Transform door1, Transform door2, FloatingPlatform.OnDestination onDest)
    {
        Vector3 pos1 = door1.position;
        Vector3 pos2 = door2.position;

        GameObject platformObject = Instantiate(platform, pos1, door1.transform.rotation);
        platformObject.GetComponent<FloatingPlatform>().SetPath(pos1, pos2, onDest);
    }

    #endregion
}
