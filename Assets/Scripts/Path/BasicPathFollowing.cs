using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPathFollowing : MonoBehaviour
{
  TilemapSetup m_tm;
  AStar m_aStar;
  PathData m_pd;
  Vector3 objectSize;
  BasicMovement m_bm;

  GameObject goalPoint;

  public float nodeRadius;

  float maxSpeed;

  // Use this for initialization
  void Start()
  {
    m_aStar = gameObject.GetComponent<AStar>();
    m_tm = GameObject.Find("GameManager").GetComponent<TilemapSetup>();

    m_pd = gameObject.GetComponent<PathData>();
    m_bm = gameObject.GetComponent<BasicMovement>();

    objectSize = gameObject.GetComponent<Renderer>().bounds.size;

    nodeRadius = 5;

    maxSpeed = gameObject.GetComponent<AgentStats>().maxSpeed;
  }

  public void SetNextDestination(Vector3 goalPos)
  {
    m_pd.start = transform.position;
    m_pd.goal = new Vector3(goalPos.x, 0, goalPos.z);
    
    PathResult pathResult = m_aStar.ComputePath(m_pd.path, m_pd.start, m_pd.goal, objectSize.y / 2);

    if (pathResult == PathResult.COMPLETE)
    {
      m_pd.pathSet = true;
    }
  }

  public PathResult SetNextDestinationRandomly()
  {
    GridPos currPos = m_tm.WorldToGrid(transform.position);

    int it = Random.Range(0, m_tm.walkableTiles.Count - 1);
    // Get destination other than the one identical to start
    while (m_tm.walkableTiles[it].Equals(currPos) == true
      || (m_tm.GetEuclideanDistance(currPos, m_tm.walkableTiles[it]) < 10))
    {
      it = Random.Range(0, m_tm.walkableTiles.Count - 1);
    }

    m_pd.start = transform.position;
    m_pd.goal = m_tm.GridToWorld(m_tm.walkableTiles[it]);
    
    PathResult pathResult = m_aStar.ComputePath(m_pd.path, m_pd.start, m_pd.goal, objectSize.y / 2);

    if (pathResult == PathResult.COMPLETE)
    {
      m_pd.pathSet = true;
    }

    return pathResult;
  }
  public void ResetPath()
  {
    m_pd.path.Clear();
    m_pd.pathSet = false;
    m_pd.currIt = 1;
  }

  public Vector3 ComputePathFollowing()
  {
    Vector3 target = new Vector3();

    if (m_pd.path != null)
    {
      target = m_pd.path[m_pd.currIt];

      if (Vector3.Distance(transform.position, target) <= nodeRadius)
      {
        ++m_pd.currIt;

        // If the agent has reached the destination cell
        if (m_pd.currIt >= m_pd.path.Count)
        {
          //currIt = path.Count - 1;

          ResetPath();

          //m_bt.textMesh.text = "Destination reached";

          return Vector3.zero;
        }
      }
    }

    //m_bt.textMesh.text = "Following path to (" + goalGridPos.x + "," + goalGridPos.y + ")";

    //return target != null ? CalculateSeek(target) : new Vector3();
    return m_pd.path != null ? m_bm.ComputeSeek(target, maxSpeed) : new Vector3();
  }
}
