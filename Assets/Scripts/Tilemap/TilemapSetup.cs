using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TilemapSetup : MonoBehaviour
{
  public int mapNumber;

  string filePath;
  int lineCount;

  // Prefabs
  public GameObject Map;
  public GameObject Wall;
  public GameObject PrisonTile;

  public GameObject Police;
  public GameObject Robber;

  public GameObject Bullet;
  public GameObject Baton;

  public GameObject Ammo;

  public GameObject Door;
  public GameObject SteelDoor;
  public GameObject Key;
  public GameObject Goal;

  public GameObject HitEffect;

  GameObject mainCamera;
  public GameObject map;
  GameObject civilian;
  GameObject goalPoint;

  public List<GridPos> walkableTiles;

  int mapHeight;
  int mapWidth;

  public Vector3 mapSize;
  float tileHeight;
  float tileWidth;

  TileData[,] tileMap;
  public List<GridPos> wallList;
  public List<GridPos> prisonTileList;

  class TileData
  {
    public bool m_isWall;
    public bool m_isPrisonTile;
    public GameObject m_wall;
    public int m_tileType;

    public TileData()
    {
      m_isWall = false;
      m_isPrisonTile = false;
      m_wall = null;
      m_tileType = 0;
    }
  }

  void SetFilePath()
  {
    filePath = "Assets/Resources/TileMap0" + mapNumber + ".txt";
  }

  void ReadMapData()
  {
    //FileInfo sourceFile = new FileInfo(filePath);
    StreamReader reader = new StreamReader(filePath);//sourceFile.OpenText();

    string text = reader.ReadLine();
    int rowIt = lineCount - 1;

    while (text != null)
    {
      int it = 0;
      int colIt = 0;

      while (it != text.Length)//text[it] != null)
      {
        if (text[it] != ' ')
        {
          // Walkable tile
          if (text[it] == '0')
          {
            tileMap[colIt, rowIt] = new TileData();

            walkableTiles.Add(new GridPos(colIt, rowIt));
          }
          // Wall
          else if (text[it] == '1')
          {
            tileMap[colIt, rowIt] = new TileData();
            RegisterWall(wallList, colIt, rowIt);
          }
          // Prison Tile
          else if (text[it] == '2')
          {
            tileMap[colIt, rowIt] = new TileData();
            RegisterPrisonTile(prisonTileList, colIt, rowIt);

            walkableTiles.Add(new GridPos(colIt, rowIt));
          }
          ++colIt;
        }
        ++it;
      }

      text = reader.ReadLine();
      --rowIt;
    }
  }

  // Use this for initialization
  void Awake()
  {
    //TextAsset txtAssets = (TextAsset)Resources.Load(txtFile);
    //txtContents = txtAssets.text;

    SetFilePath();

    //filePath = "Assets/Resources/TileMap01.txt";

    lineCount = File.ReadAllLines(filePath).Length;

    mapWidth = lineCount;
    mapHeight = lineCount;

    map = GameObject.Instantiate(Map, new Vector3(0, 0, 0), Quaternion.identity);
    map.transform.localScale = new Vector3(mapWidth, 1, mapHeight);
    mapSize = map.GetComponent<Renderer>().bounds.size;
    //map.transform.position = new Vector3(mapSize.x / 2, 0, mapSize.z / 2);

    tileMap = new TileData[mapWidth, mapHeight];

    mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    // Set camera position
    mainCamera.transform.position = new Vector3(map.transform.position.x, 180f * mapHeight / 20f, map.transform.position.z - 3);

    tileWidth = mapSize.x / mapWidth;
    tileHeight = mapSize.z / mapHeight;

    wallList = new List<GridPos>();
    prisonTileList = new List<GridPos>();

    // Make a list of walkable tiles
    walkableTiles = new List<GridPos>();

    ReadMapData();

    //SetListOfWalkableTiles();

    SetUpEnvironment();
  }

  // Deploy AI agents
  void SetUpEnvironment()
  {
    if (mapNumber == 1)
    {
      // Deploy police officers and robbers

      float agentHeight = Police.GetComponent<Renderer>().bounds.size.y / 2;

      // Deploy police officers
      GameObject obj = GameObject.Instantiate(Police,
        GridToWorld(new GridPos(14, 11), agentHeight), Quaternion.identity);
      AgentCondition ac = obj.AddComponent<AgentCondition>();
      ac.hasBaton = true;

      obj = GameObject.Instantiate(Police,
        GridToWorld(new GridPos(14, 14), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();
      ac.hasBaton = true;
      ac.ammoCount = 8;

      obj = GameObject.Instantiate(Police,
        GridToWorld(new GridPos(17, 11), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();
      ac.hasBaton = true;
      ac.ammoCount = 8;

      obj = GameObject.Instantiate(Police,
        GridToWorld(new GridPos(11, 16), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();
      ac.hasBaton = true;
      ac.ammoCount = 9;

      obj = GameObject.Instantiate(Police,
        GridToWorld(new GridPos(20, 16), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();
      ac.hasBaton = true;

      // Deploy robbers
      obj = GameObject.Instantiate(Robber,
              GridToWorld(new GridPos(25, 7), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();
      ac.hasBaton = true;

      obj = GameObject.Instantiate(Robber,
        GridToWorld(new GridPos(22, 5), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();
      ac.ammoCount = 3;

      obj = GameObject.Instantiate(Robber,
        GridToWorld(new GridPos(25, 25), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();

      obj = GameObject.Instantiate(Robber,
        GridToWorld(new GridPos(5, 5), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();

      obj = GameObject.Instantiate(Robber,
        GridToWorld(new GridPos(10, 5), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();
      ac.hasBaton = true;

      obj = GameObject.Instantiate(Robber,
        GridToWorld(new GridPos(12, 23), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();

      // Deploy ammo boxes
      obj = GameObject.Instantiate(Ammo,
        GridToWorld(new GridPos(28, 5), 3), Quaternion.identity);

      obj = GameObject.Instantiate(Ammo,
        GridToWorld(new GridPos(15, 28), 3), Quaternion.identity);
    }
    else
    {
      float agentHeight = Police.GetComponent<Renderer>().bounds.size.y / 2;

      GameObject goal = GameObject.Instantiate(Goal,
        GridToWorld(new GridPos(0, 11), 0.01f), Quaternion.identity);

      // Right Up : Get Key
      GameObject obj = GameObject.Instantiate(Robber,
          GridToWorld(new GridPos(25, 25), agentHeight), Quaternion.identity);
      AgentCondition ac = obj.AddComponent<AgentCondition>();

      GameObject keyObj = GameObject.Instantiate(Key,
          GridToWorld(new GridPos(28, 28), agentHeight), Quaternion.identity);

      GameObject doorObj = GameObject.Instantiate(Door,
          GridToWorld(new GridPos(19, 26), agentHeight), Quaternion.identity);

      obj.GetComponent<SecondRobberGOAP>().targetDoor = doorObj;
      obj.GetComponent<SecondRobberGOAP>().targetKey = keyObj;
      obj.GetComponent<SecondRobberGOAP>().goal = goal;

      // Left Down : Already has key
      obj = GameObject.Instantiate(Robber,
          GridToWorld(new GridPos(8, 3), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();
      ac.hasKey = true;

      //keyObj = GameObject.Instantiate(Key,
      //    GridToWorld(new GridPos(1, 6), agentHeight), Quaternion.identity);

      doorObj = GameObject.Instantiate(Door,
          GridToWorld(new GridPos(10, 4), agentHeight), Quaternion.identity);
      obj.GetComponent<SecondRobberGOAP>().targetDoor = doorObj;
      obj.GetComponent<SecondRobberGOAP>().goal = goal;

      // Left Up : Smash door
      obj = GameObject.Instantiate(Robber,
          GridToWorld(new GridPos(7, 25), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();

      //keyObj = GameObject.Instantiate(Key,
      //    GridToWorld(new GridPos(1, 17), agentHeight), Quaternion.identity);

      doorObj = GameObject.Instantiate(Door,
          GridToWorld(new GridPos(10, 25), agentHeight), Quaternion.identity);
      obj.GetComponent<SecondRobberGOAP>().targetDoor = doorObj;
      obj.GetComponent<SecondRobberGOAP>().targetKey = keyObj;
      obj.GetComponent<SecondRobberGOAP>().goal = goal;

      // Right Down : Steel door, no key
      obj = GameObject.Instantiate(Robber,
          GridToWorld(new GridPos(25, 7), agentHeight), Quaternion.identity);
      ac = obj.AddComponent<AgentCondition>();

      //keyObj = GameObject.Instantiate(Key,
      //    GridToWorld(new GridPos(28, 13), agentHeight), Quaternion.identity);

      doorObj = GameObject.Instantiate(SteelDoor,
          GridToWorld(new GridPos(19, 4), agentHeight), Quaternion.identity);
      obj.GetComponent<SecondRobberGOAP>().targetDoor = doorObj;
      doorObj.GetComponent<AgentStats>().steelDoor = true;
      obj.GetComponent<SecondRobberGOAP>().goal = goal;

      // Deploy guards
      obj = GameObject.Instantiate(Police,
          GridToWorld(new GridPos(14, 28), agentHeight), Quaternion.identity);
      obj.GetComponent<SecondCopGOAP>().goal = goal;
      ac = obj.AddComponent<AgentCondition>();
      ac.ammoCount = 8;

      obj = GameObject.Instantiate(Police,
          GridToWorld(new GridPos(3, 11), agentHeight), Quaternion.identity);
      obj.GetComponent<SecondCopGOAP>().goal = goal;
      ac = obj.AddComponent<AgentCondition>();
      ac.ammoCount = 8;
    }
  }


  // Update is called once per frame
  void Update()
  {

  }

  // Store all the grid positions that are not walls
  // to the list
  void SetListOfWalkableTiles()
  {
    for (int i = 0; i < mapWidth; ++i)
    {
      for (int j = 0; j < mapHeight; ++j)
      {
        if (tileMap[i, j].m_isWall == false)
        {
          walkableTiles.Add(new GridPos(i, j));
        }
      }
    }
    //Debug.Log(walkableTiles.Count);
  }

  // Deploy walls and register the information on tileMap
  void RegisterWall(List<GridPos> wallList, int x, int y)
  {
    GridPos wallPos = new GridPos(x, y);
    wallList.Add(wallPos);
    GameObject wallObj = GameObject.Instantiate(Wall, GridToWorld(wallPos), Quaternion.identity);
    tileMap[x, y].m_isWall = true;
    tileMap[x, y].m_tileType = 1;
    tileMap[x, y].m_wall = wallObj;
  }

  void RegisterPrisonTile(List<GridPos> prisonTileList, int x, int y)
  {
    float tileHeight = 0.01f;

    GridPos prisonTilePos = new GridPos(x, y);
    prisonTileList.Add(prisonTilePos);
    GameObject.Instantiate(PrisonTile, GridToWorld(prisonTilePos, tileHeight), Quaternion.identity);
    tileMap[x, y].m_isPrisonTile = true;
    tileMap[x, y].m_tileType = 2;
  }

  // Instantiate wall objects
  void BuildWalls()
  {
    foreach (GridPos wall in wallList)
    {
      GameObject wallObj = GameObject.Instantiate(Wall, GridToWorld(wall), Quaternion.identity);
    }
  }

  // Not used
  void DrawBorders()
  {
    //LineRenderer lineRenderer = floor.AddComponent<LineRenderer>();
    //lineRenderer.positionCount = (int)(tileWidth + tileHeight) - 2;

    //// Vertical Lines
    //for (int i = 1; i < mapWidth; ++i)
    //{

    //  lineRenderer.SetPosition(0, new Vector3(tileWidth * i, 1, tilePos.z - tileHeight / 2));
    //}
  }

  // Convert grid position to world position
  public Vector3 GridToWorld(GridPos gridPos, float objHeight = 0)
  {
    float worldX = -mapSize.x / 2 + tileWidth / 2 + tileWidth * gridPos.x;
    float worldY = -mapSize.z / 2 + tileHeight / 2 + tileHeight * gridPos.y;

    return new Vector3(worldX, objHeight, worldY);
  }

  // Convert world position to grid position
  public GridPos WorldToGrid(Vector3 worldPos)
  {
    GridPos gridPos = new GridPos();

    gridPos.x = (int)((worldPos.x + mapSize.x / 2) / mapSize.x * (float)mapWidth);
    gridPos.y = (int)((worldPos.z + mapSize.z / 2) / mapSize.z * (float)mapHeight);

    return gridPos;
  }

  public int GetMapWidth()
  {
    return mapWidth;
  }

  public int GetMapHeight()
  {
    return mapHeight;
  }

  public bool IsWall(int x, int y)
  {
    return tileMap[x, y].m_isWall;
  }

  public bool IsWall(GridPos pos)
  {
    return tileMap[pos.x, pos.y].m_isWall;
  }

  // Determine whether given [x,y] position is located
  // within the map
  public bool IsValidGridPosition(int x, int y)
  {
    return x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;
  }

  public float GetEuclideanDistance(GridPos p1, GridPos p2)
  {
    return Mathf.Sqrt((p2.x - p1.x) * (p2.x - p1.x) + (p2.y - p1.y) * (p2.y - p1.y));
  }
}
