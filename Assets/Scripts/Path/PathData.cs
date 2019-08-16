using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathData : MonoBehaviour
{
  public List<Vector3> path;
  public Vector3 start;
  public Vector3 goal;
  public bool pathSet;
  public int currIt;

  // Use this for initialization
  void Start()
  {
    path = new List<Vector3>();
    start = new Vector3();
    goal = new Vector3();
    pathSet = false;
    currIt = 1;
  }

  // Update is called once per frame
  void Update()
  {

  }
}
