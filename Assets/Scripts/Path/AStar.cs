using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour
{
  TilemapSetup tileMapSetup;

  int mapXSize;
  int mapYSize;

  GridPos start;
  GridPos goal;

  float centerHeight;

  List<GridNode> openList;
  List<GridNode> closedList;
  Dictionary<GridPos, GridPos> pathTracker;

  // Use this for initialization
  void Start()
  {
    tileMapSetup = GameObject.Find("GameManager").GetComponent<TilemapSetup>();


  }

  // Update is called once per frame
  void Update()
  {

  }

  public float ComputeTotalPathCost(Vector3 worldStart, Vector3 worldGoal)
  {
    float planningCost = 0f;

    mapXSize = tileMapSetup.GetMapWidth();
    mapYSize = tileMapSetup.GetMapHeight();

    // Get start and goal positions first
    start = tileMapSetup.WorldToGrid(worldStart);
    goal = tileMapSetup.WorldToGrid(worldGoal);

    // If start and goal tiles are the same
    if (start.Equals(goal))
    {
      return planningCost;
    }

    //request.path.push_back(request.start);

    // Straight line optimization
    if (NoWallInArea(start, goal) == true)
    {
      planningCost = tileMapSetup.GetEuclideanDistance(start, goal);
    }

    float hx = CalculateHeuristic(start.x, start.y, goal);

    openList = new List<GridNode>();
    closedList = new List<GridNode>();
    pathTracker = new Dictionary<GridPos, GridPos>();

    //openList.Add(new GridNode((int)start.x, (int)start.y, -1, -1, 0, hx));
    openList.Add(new GridNode(start.x, start.y, 0, 0, 0, hx));

    while (openList.Count > 0)
    {
      GridNode parentNode = RemoveCheapest(openList);

      int parX = parentNode.xPos;
      int parY = parentNode.yPos;

      // If node is the Goal Node, then path found (RETURN “COMPLETE”)
      if (parX == goal.x && parY == goal.y)
      {
        List<GridPos> pathVec = new List<GridPos>();
        pathVec.Add(goal);

        GridPos currPos = new GridPos();

        if (tileMapSetup.IsValidGridPosition(parentNode.xPos, parentNode.yPos) == false)
        {
          currPos.x = start.x;
          currPos.y = start.y;
        }
        else
        {
          currPos.x = parentNode.xParent;
          currPos.y = parentNode.yParent;
        }

        while (!currPos.Equals(start))
        {
          pathVec.Add(currPos);
          currPos = pathTracker[currPos];
        }

        if (!start.Equals(goal))
        {
          pathVec.Add(start);
        }

        pathVec.Reverse();


        if (pathVec.Count > 2)
        {
          PerformRubberbanding(pathVec);
        }

        for (int i = 1; i < pathVec.Count; ++i)
        {
          planningCost += tileMapSetup.GetEuclideanDistance(pathVec[i - 1], pathVec[i]);
        }

        ClearContainers();

        return planningCost;
      }

      for (int i = parX - 1; i <= parX + 1; ++i)
      {
        for (int j = parY - 1; j <= parY + 1; ++j)
        {
          // Indices should be within map range
          if (tileMapSetup.IsValidGridPosition(i, j) == true)
          {
            // exclude parent position and walls
            if (!(i == parX && j == parY)
              && (tileMapSetup.IsWall(i, j) == false))
            {
              if (IsOnList(i, j, closedList))
                continue;

              // Diagonal correctness
              bool isDiagonal = (i != parX) && (j != parY);

              if (isDiagonal)
              {
                if (SkipDiagonal(i, j, parX, parY))
                {
                  continue;
                }
              }

              //Compute its cost, f(x) = g(x) + h(x)

              float givenCost = parentNode.given;

              // If diagonal
              if (isDiagonal)
              {
                givenCost += Mathf.Sqrt(2);
              }
              // If vertical or horizontal
              else
              {
                givenCost += 1;
              }

              float heuristicCost = CalculateHeuristic(j, i, goal);
              float totalCost = givenCost + heuristicCost;

              /*
              If child node isn’t on Open or Closed list, put it on Open List.

              Else If child node is on Open or Closed List, AND this new one is cheaper,
              then take the old expensive one off both lists and put this new cheaper one on the Open List.
              */

              GridNode childNode = new GridNode(i, j, parX, parY, givenCost, totalCost);

              List<GridNode>.Enumerator openListIt = new List<GridNode>.Enumerator();
              List<GridNode>.Enumerator closedListIt = new List<GridNode>.Enumerator();

              bool isOnOpenList = IsOnList(childNode, openList, ref openListIt);
              bool isOnClosedList = IsOnList(childNode, closedList, ref closedListIt);

              if (!isOnOpenList && !isOnClosedList)
              {
                openList.Add(childNode);
              }
              else
              {
                if (isOnOpenList && openListIt.Current.total > childNode.total)
                {
                  openList.Remove(openListIt.Current);
                  openList.Add(childNode);
                }
                else if (isOnClosedList && closedListIt.Current.total > childNode.total)
                {
                  closedList.Remove(closedListIt.Current);
                  openList.Add(childNode);
                }
              }
            }
          }
        }
      }

      // Place parent node on the Closed List (we’re done with it)
      closedList.Add(parentNode);

      GridPos key = new GridPos(parentNode.xPos, parentNode.yPos);
      GridPos value = new GridPos(parentNode.xParent, parentNode.yParent);

      if (pathTracker.ContainsKey(key) == false)
        pathTracker.Add(key, value);
      else
        pathTracker[key] = value;
    }

    ClearContainers();

    return float.MaxValue;
  }

  public PathResult ComputePath(List<Vector3> path, Vector3 worldStart, Vector3 worldGoal, float centerHeight)
  {
    this.centerHeight = centerHeight;

    mapXSize = tileMapSetup.GetMapWidth();
    mapYSize = tileMapSetup.GetMapHeight();

    // Get start and goal positions first
    start = tileMapSetup.WorldToGrid(worldStart);
    goal = tileMapSetup.WorldToGrid(worldGoal);

    // If start and goal tiles are the same
    if (start == goal)
    {
      path.Add(worldGoal);
      return PathResult.COMPLETE;
    }

    //request.path.push_back(request.start);

    // Straight line optimization
    if (NoWallInArea(start, goal) == true)
    {
      path.Add(tileMapSetup.GridToWorld(start, centerHeight));
      path.Add(tileMapSetup.GridToWorld(goal, centerHeight));

      return PathResult.COMPLETE;
    }

    float hx = CalculateHeuristic(start.x, start.y, goal);

    openList = new List<GridNode>();
    closedList = new List<GridNode>();
    pathTracker = new Dictionary<GridPos, GridPos>();

    //openList.Add(new GridNode((int)start.x, (int)start.y, -1, -1, 0, hx));
    openList.Add(new GridNode(start.x, start.y, 0, 0, 0, hx));

    while (openList.Count > 0)
    {
      GridNode parentNode = RemoveCheapest(openList);

      int parX = parentNode.xPos;
      int parY = parentNode.yPos;

      // If node is the Goal Node, then path found (RETURN “COMPLETE”)
      if (parX == goal.x && parY == goal.y)
      {
        List<GridPos> pathVec = new List<GridPos>();
        pathVec.Add(goal);

        GridPos currPos = new GridPos();

        if (tileMapSetup.IsValidGridPosition(parentNode.xPos, parentNode.yPos) == false)
        {
          currPos.x = start.x;
          currPos.y = start.y;
        }
        else
        {
          currPos.x = parentNode.xParent;
          currPos.y = parentNode.yParent;
        }

        while (!currPos.Equals(start))
        {
          pathVec.Add(currPos);
          currPos = pathTracker[currPos];
        }

        if (!start.Equals(goal))
        {
          pathVec.Add(start);
        }

        pathVec.Reverse();


        if (pathVec.Count > 2)
        {
          PerformRubberbanding(pathVec);
        }

        for (int i = 0; i < pathVec.Count; ++i)
        {
          path.Add(tileMapSetup.GridToWorld(pathVec[i], centerHeight));
        }

        ClearContainers();

        return PathResult.COMPLETE;
      }

      for (int i = parX - 1; i <= parX + 1; ++i)
      {
        for (int j = parY - 1; j <= parY + 1; ++j)
        {
          // Indices should be within map range
          if (tileMapSetup.IsValidGridPosition(i, j) == true)
          {
            // exclude parent position and walls
            if (!(i == parX && j == parY)
              && (tileMapSetup.IsWall(i, j) == false))
            {
              if (IsOnList(i, j, closedList))
                continue;

              // Diagonal correctness
              bool isDiagonal = (i != parX) && (j != parY);

              if (isDiagonal)
              {
                if (SkipDiagonal(i, j, parX, parY))
                {
                  continue;
                }
              }

              //Compute its cost, f(x) = g(x) + h(x)

              float givenCost = parentNode.given;

              // If diagonal
              if (isDiagonal)
              {
                givenCost += Mathf.Sqrt(2);
              }
              // If vertical or horizontal
              else
              {
                givenCost += 1;
              }

              float heuristicCost = CalculateHeuristic(j, i, goal);
              float totalCost = givenCost + heuristicCost;

              /*
              If child node isn’t on Open or Closed list, put it on Open List.

              Else If child node is on Open or Closed List, AND this new one is cheaper,
              then take the old expensive one off both lists and put this new cheaper one on the Open List.
              */

              GridNode childNode = new GridNode(i, j, parX, parY, givenCost, totalCost);

              List<GridNode>.Enumerator openListIt = new List<GridNode>.Enumerator();
              List<GridNode>.Enumerator closedListIt = new List<GridNode>.Enumerator();

              bool isOnOpenList = IsOnList(childNode, openList, ref openListIt);
              bool isOnClosedList = IsOnList(childNode, closedList, ref closedListIt);

              if (!isOnOpenList && !isOnClosedList)
              {
                openList.Add(childNode);
              }
              else
              {
                if (isOnOpenList && openListIt.Current.total > childNode.total)
                {
                  openList.Remove(openListIt.Current);
                  openList.Add(childNode);
                }
                else if (isOnClosedList && closedListIt.Current.total > childNode.total)
                {
                  closedList.Remove(closedListIt.Current);
                  openList.Add(childNode);
                }
              }
            }
          }
        }
      }

      // Place parent node on the Closed List (we’re done with it)
      closedList.Add(parentNode);

      GridPos key = new GridPos(parentNode.xPos, parentNode.yPos);
      GridPos value = new GridPos(parentNode.xParent, parentNode.yParent);

      if (pathTracker.ContainsKey(key) == false)
        pathTracker.Add(key, value);
      else
        pathTracker[key] = value;
    }

    ClearContainers();
    
    return PathResult.IMPOSSIBLE;
  }

  public PathResult ComputePath(List<Vector3> path, Vector3 worldStart, Vector3 worldGoal, float centerHeight, ref float planningCost)
  {
    // TODO : Add rubberbanding

    this.centerHeight = centerHeight;

    mapXSize = tileMapSetup.GetMapWidth();
    mapYSize = tileMapSetup.GetMapHeight();

    // Get start and goal positions first
    start = tileMapSetup.WorldToGrid(worldStart);
    goal = tileMapSetup.WorldToGrid(worldGoal);

    // If start and goal tiles are the same
    if (start == goal)
    {
      path.Add(worldGoal);
      return PathResult.COMPLETE;
    }

    //request.path.push_back(request.start);

    // Straight line optimization
    if (NoWallInArea(start, goal) == true)
    {
      path.Add(tileMapSetup.GridToWorld(start, centerHeight));
      path.Add(tileMapSetup.GridToWorld(goal, centerHeight));

      planningCost = tileMapSetup.GetEuclideanDistance(start, goal);

      return PathResult.COMPLETE;
    }

    float hx = CalculateHeuristic(start.x, start.y, goal);

    openList = new List<GridNode>();
    closedList = new List<GridNode>();
    pathTracker = new Dictionary<GridPos, GridPos>();

    //openList.Add(new GridNode((int)start.x, (int)start.y, -1, -1, 0, hx));
    openList.Add(new GridNode(start.x, start.y, 0, 0, 0, hx));

    while (openList.Count > 0)
    {
      GridNode parentNode = RemoveCheapest(openList);

      int parX = parentNode.xPos;
      int parY = parentNode.yPos;

      // If node is the Goal Node, then path found (RETURN “COMPLETE”)
      if (parX == goal.x && parY == goal.y)
      {
        List<GridPos> pathVec = new List<GridPos>();
        pathVec.Add(goal);

        GridPos currPos = new GridPos();

        if (tileMapSetup.IsValidGridPosition(parentNode.xPos, parentNode.yPos) == false)
        {
          currPos.x = start.x;
          currPos.y = start.y;
        }
        else
        {
          currPos.x = parentNode.xParent;
          currPos.y = parentNode.yParent;
        }

        while (!currPos.Equals(start))
        {
          pathVec.Add(currPos);
          currPos = pathTracker[currPos];
        }

        if (!start.Equals(goal))
        {
          pathVec.Add(start);
        }

        pathVec.Reverse();


        if (pathVec.Count > 2)
        {
          PerformRubberbanding(pathVec);
        }

        for (int i = 0; i < pathVec.Count; ++i)
        {
          path.Add(tileMapSetup.GridToWorld(pathVec[i], centerHeight));

          if (i > 0)
            planningCost += tileMapSetup.GetEuclideanDistance(pathVec[i - 1], pathVec[i]);
        }

        ClearContainers();

        return PathResult.COMPLETE;
      }

      for (int i = parX - 1; i <= parX + 1; ++i)
      {
        for (int j = parY - 1; j <= parY + 1; ++j)
        {
          // Indices should be within map range
          if (tileMapSetup.IsValidGridPosition(i, j) == true)
          {
            // exclude parent position and walls
            if (!(i == parX && j == parY)
              && (tileMapSetup.IsWall(i, j) == false))
            {
              if (IsOnList(i, j, closedList))
                continue;

              // Diagonal correctness
              bool isDiagonal = (i != parX) && (j != parY);

              if (isDiagonal)
              {
                if (SkipDiagonal(i, j, parX, parY))
                {
                  continue;
                }
              }

              //Compute its cost, f(x) = g(x) + h(x)

              float givenCost = parentNode.given;

              // If diagonal
              if (isDiagonal)
              {
                givenCost += Mathf.Sqrt(2);
              }
              // If vertical or horizontal
              else
              {
                givenCost += 1;
              }

              float heuristicCost = CalculateHeuristic(j, i, goal);
              float totalCost = givenCost + heuristicCost;

              /*
              If child node isn’t on Open or Closed list, put it on Open List.

              Else If child node is on Open or Closed List, AND this new one is cheaper,
              then take the old expensive one off both lists and put this new cheaper one on the Open List.
              */

              GridNode childNode = new GridNode(i, j, parX, parY, givenCost, totalCost);

              List<GridNode>.Enumerator openListIt = new List<GridNode>.Enumerator();
              List<GridNode>.Enumerator closedListIt = new List<GridNode>.Enumerator();

              bool isOnOpenList = IsOnList(childNode, openList, ref openListIt);
              bool isOnClosedList = IsOnList(childNode, closedList, ref closedListIt);

              if (!isOnOpenList && !isOnClosedList)
              {
                openList.Add(childNode);
              }
              else
              {
                if (isOnOpenList && openListIt.Current.total > childNode.total)
                {
                  openList.Remove(openListIt.Current);
                  openList.Add(childNode);
                }
                else if (isOnClosedList && closedListIt.Current.total > childNode.total)
                {
                  closedList.Remove(closedListIt.Current);
                  openList.Add(childNode);
                }
              }
            }
          }
        }
      }

      // Place parent node on the Closed List (we’re done with it)
      closedList.Add(parentNode);

      GridPos key = new GridPos(parentNode.xPos, parentNode.yPos);
      GridPos value = new GridPos(parentNode.xParent, parentNode.yParent);

      if (pathTracker.ContainsKey(key) == false)
        pathTracker.Add(key, value);
      else
        pathTracker[key] = value;
    }

    ClearContainers();

    // Set planningCost to max value to avoid selecting this method
    planningCost = float.MaxValue;

    return PathResult.IMPOSSIBLE;
  }

  bool NoWallInArea(GridPos a, GridPos b)
  {
    int minX = GetMin(a.x, b.x);
    int maxX = GetMax(a.x, b.x);
    int minY = GetMin(a.y, b.y);
    int maxY = GetMax(a.y, b.y);

    for (int i = minX; i <= maxX; ++i)
    {
      for (int j = minY; j <= maxY; ++j)
      {
        if (tileMapSetup.IsWall(i, j))
        {
          return false;
        }
      }
    }

    return true;
  }

  int GetMin(int a, int b)
  {
    return a < b ? a : b;
  }

  int GetMax(int a, int b)
  {
    return a > b ? a : b;
  }

  bool NoWallInArea(GridPos a, GridPos b, GridPos c)
  {
    int minX = GetMin(a.x, b.x, c.x);
    int maxX = GetMax(a.x, b.x, c.x);
    int minY = GetMin(a.y, b.y, c.y);
    int maxY = GetMax(a.y, b.y, c.y);

    for (int i = minX; i <= maxX; ++i)
    {
      for (int j = minY; j <= maxY; ++j)
      {
        if (tileMapSetup.IsWall(i, j))
        {
          return false;
        }
      }
    }

    return true;
  }

  int GetMin(int a, int b, int c)
  {
    int min = a;

    if (min > b)
      min = b;
    if (min > c)
      min = c;

    return min;
  }

  int GetMax(int a, int b, int c)
  {
    int max = a;

    if (max < b)
      max = b;
    if (max < c)
      max = c;

    return max;
  }

  float CalculateHeuristic(int currX, int currY, GridPos destPos)
  {

    float xDiff = Mathf.Abs(destPos.x - currX);
    float yDiff = Mathf.Abs(destPos.y - currY);

    return Mathf.Sqrt(xDiff * xDiff + yDiff * yDiff);
  }

  GridNode RemoveCheapest(List<GridNode> nodeList)
  {
    GridNode nodeWithMinCost = nodeList[0];

    for (int i = 1; i < nodeList.Count; ++i)
    {
      if (nodeWithMinCost.total > nodeList[i].total)
      {
        nodeWithMinCost = nodeList[i];
      }
    }

    nodeList.Remove(nodeWithMinCost);

    return nodeWithMinCost;
  }

  bool IsOnList(int currX, int currY, List<GridNode> nodeList)
  {
    if (nodeList == null)
      return false;

    for (int i = 0; i < nodeList.Count; ++i)
    {
      if (nodeList[i].xPos == currX && nodeList[i].yPos == currY)
        return true;
    }

    return false;
  }

  bool IsOnList(GridNode node, List<GridNode> nodeList, ref List<GridNode>.Enumerator it)
  {
    if (nodeList == null)
      return false;

    it = nodeList.GetEnumerator();

    while (it.MoveNext())
    {
      if (it.Current.xPos == node.xPos && it.Current.yPos == node.yPos)
        return true;
    }

    return false;
  }

  bool SkipDiagonal(int i, int j, int parX, int parY)
  {
    if (i == parX - 1)
    {
      // Lower left
      if (j == parY - 1)
      {
        bool leftWall = tileMapSetup.IsWall(parX - 1, parY);
        bool bottomWall = tileMapSetup.IsWall(parX, parY - 1);

        if (leftWall || bottomWall)
          return true;
      }
      // Upper left
      else if (j == parY + 1)
      {
        bool leftWall = tileMapSetup.IsWall(parX - 1, parY);
        bool topWall = tileMapSetup.IsWall(parX, parY + 1);

        if (leftWall || topWall)
          return true;
      }
    }

    else if (i == parX + 1)
    {
      // Lower right
      if (j == parY - 1)
      {
        bool rightWall = tileMapSetup.IsWall(parX + 1, parY);
        bool bottomWall = tileMapSetup.IsWall(parX, parY - 1);

        if (rightWall || bottomWall)
          return true;
      }
      // Upper right
      else if (j == parY + 1)
      {
        bool rightWall = tileMapSetup.IsWall(parX + 1, parY);
        bool topWall = tileMapSetup.IsWall(parX, parY + 1);

        if (rightWall || topWall)
          return true;
      }
    }

    return false;
  }

  void ClearContainers()
  {
    openList.Clear();
    closedList.Clear();
    pathTracker.Clear();
  }
  void PerformRubberbanding(List<GridPos> pathList)
  {
    int it1 = pathList.Count - 3;
    int it2 = pathList.Count - 2;
    int it3 = pathList.Count - 1;

    while (it1 != 0)
    {
      if (NoWallInArea(pathList[it1], pathList[it2], pathList[it3]))
      {
        pathList.RemoveAt(it2);

        it3 = it2;
        it2 = it1;
        --it1;
      }
      else
      {
        --it1;
        --it2;
        --it3;
      }
    }

    if (NoWallInArea(pathList[it1], pathList[it2], pathList[it3]))
    {
      pathList.RemoveAt(it2);
      it3 = it2;
    }
  }
}