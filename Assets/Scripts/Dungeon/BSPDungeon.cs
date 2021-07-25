using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSPDungeon : MonoBehaviour
{
    [SerializeField]
    private int dungeonWidth;
    [SerializeField]
    private int dungeonHeight;
    [SerializeField]
    private int minDimension;
    [SerializeField]
    private int maxRooms;

    [Header("Dungeon Prefabs")]
    [SerializeField]
    private GameObject floorEntry;

    [Header("Dialogue")]
    [SerializeField]
    private GameObject dialogueUI;
    
    [Header("Debugging Tools")]
    [SerializeField]
    private GameObject player;

    private List<Room> rooms = new List<Room>();

    //private List<Node> leaves = new List<Node>();

    private class Node
    {
        public static int leafCount;
        public int x, y, width, height;
        public Node leftChild;
        public Node rightChild;

        public bool  horizontal;

        //public Transform topDoor, bottomDoor, leftDoor, rightDoor;

        public Node(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public void Split(int minDimension, int maxRooms)
        {
            this.horizontal = Random.Range(0, height + width) < height;
            int longer = this.horizontal ? height : width;

            if (longer < 2 * minDimension || leafCount >= maxRooms) return;

            int slice = Random.Range(0, longer);
            if (Mathf.Min(longer - slice, slice) < minDimension)
                slice = minDimension;


            leafCount++;
            if (this.horizontal)
            {
                this.leftChild = new Node(x, y, width, slice);
                this.rightChild = new Node(x, y + slice, width, height - slice);
            } else
            {
                this.leftChild = new Node(x, y, slice, height);
                this.rightChild = new Node(x + slice, y, width - slice, height);
            }

            this.leftChild.Split(minDimension, maxRooms);
            this.rightChild.Split(minDimension, maxRooms);
        }

        public float area() { return this.height * this.width; }
        public Vector3 middle() { return new Vector3(x + width / 2, 0, y + height / 2); }
    }

    private void Start()
    {
        WorldState.worldLevel++;
        //Build BSP and store resultant spaces in this.leaves
        Node.leafCount = 1;
        Room.Reset();

        Node root = new Node(0, 0, dungeonWidth, dungeonHeight);
        root.Split(minDimension, this.maxRooms);

        SpawnDungeon(root);

        Transform startDoor = Room.Get(0).bottomDoor;
        StartCoroutine(SpawnPlayer(startDoor));
    }

    IEnumerator SpawnPlayer(Transform door)
    {
        GameObject floorInstance = Instantiate(floorEntry, door.position + 40 * Vector3.up, door.rotation);

        if (PlayerController.instance == null) Instantiate(player, door.position, door.rotation);
        else
        {
            PlayerController.instance.transform.position = door.position;
            PlayerController.instance.transform.rotation = door.rotation;
        }

        PlayerController.ToggleAgent(false);

        yield return null;
        
        while (floorInstance.transform.position.y > 0)
        {
            floorInstance.transform.position -= 20 * Time.deltaTime * Vector3.up;
            PlayerState.player.position = floorInstance.transform.position + Vector3.up;

            yield return null;
        }

        float time = PlayerController.MoveTo(door.position + door.forward * 10);
        for (float elapsed = 0; elapsed <= time; elapsed += Time.deltaTime)
            yield return null;

        yield return null; //Wait 1 frame

        PlayerController.ToggleAgent(true);

        string dialoguePath = string.Format("Dialogue/Floor {0}", WorldState.worldLevel);
        Dialogue dialogue = Resources.Load<Dialogue>(dialoguePath);

        Instantiate(this.dialogueUI).GetComponent<DialogueManager>()
            .ParseDialogue(dialogue, rooms[0].SpawnEnemies);
    }

    private void SpawnDungeon(Node n)
    {
        if (n.leftChild == null)
        {
            GameObject roomObject = SearchRoom(n.width, n.height);
            roomObject = Instantiate(roomObject, n.middle(), roomObject.transform.rotation);
            Room room = roomObject.GetComponent<Room>();

            rooms.Add(room);
            //SpawnNode(n);
        }

        else
        {
            SpawnDungeon(n.leftChild);
            SpawnDungeon(n.rightChild);
        }
    }

    private GameObject SearchRoom(float width, float height)
    {
        int x  = ((int)width / 30) * 25;
        x = Mathf.Min(125, x);
        int z = ((int)height / 25) * 25;
        z = Mathf.Min(125, z);

        string name = string.Format("Rooms/{0}x{1}", x, z);

        return (GameObject)Resources.Load(name);
    }   


}
