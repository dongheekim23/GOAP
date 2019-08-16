using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
  Vector3 mapSize;
  Vector3 initPos;
  float range;

  GameObject[] obstacles;

  // Use this for initialization
  void Start()
  {
    initPos = gameObject.transform.position;
    //mapSize = GameObject.FindGameObjectWithTag("Map").GetComponent<Renderer>().bounds.size;
    mapSize = GameObject.Find("GameManager").GetComponent<TilemapSetup>().mapSize;
    range = 100f;

    obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
  }

  // Update is called once per frame
  void Update()
  {
    // If the bullet goes outside of the map, then destroy it
    if (gameObject.transform.position.x > mapSize.x / 2
      || gameObject.transform.position.x < -mapSize.x / 2
      || gameObject.transform.position.z > mapSize.z / 2
      || gameObject.transform.position.z < -mapSize.z / 2)
    {
      Destroy(gameObject);
    }
    // If the bullet travels over the maximum range, then destroy it
    else if (Vector3.Distance(initPos, gameObject.transform.position) > range)
    {
      Destroy(gameObject);
    }
    else
    {
      float bulletSize = gameObject.GetComponent<Renderer>().bounds.size.x;
      //obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

      foreach (GameObject obstacle in obstacles)
      {
        if (obstacle != null)
        {
          float obstacleSize = obstacle.GetComponent<Renderer>().bounds.size.x;

          if (Vector3.Distance(obstacle.transform.position, gameObject.transform.position)
            < (obstacleSize + bulletSize) / 2)
          {
            Destroy(gameObject);

            return;
          }
        }
      }
    }
  }
}
