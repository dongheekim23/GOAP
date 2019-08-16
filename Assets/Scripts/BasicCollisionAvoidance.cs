using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicCollisionAvoidance : MonoBehaviour
{
  Rigidbody rb;
  GameObject[] obstacles;

  float seeAhead;
  float maxSpeed;

  // Use this for initialization
  void Start()
  {
    rb = GetComponent<Rigidbody>();
    obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
    maxSpeed = gameObject.GetComponent<AgentStats>().maxSpeed;
    seeAhead = 3f;
  }

  private void Update()
  {
    //obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
  }

  bool LineCircleIntersection(Vector3 ahead, Vector3 ahead2, GameObject obstacle)
  {
    Vector3 obstacleCenterPos = obstacle.transform.position;
    float obstacleRadius = obstacle.GetComponent<Renderer>().bounds.size.x / 2;
    float objectRadius = gameObject.GetComponent<Renderer>().bounds.size.x / 2;

    return (Vector3.Distance(obstacleCenterPos, ahead) <= obstacleRadius)
      || (Vector3.Distance(obstacleCenterPos, ahead2) <= obstacleRadius)
      || (Vector3.Distance(obstacleCenterPos, transform.position) <= obstacleRadius * 1f + objectRadius);
  }

  //bool LineAABBIntersection(Vector3 ahead, Vector3 ahead2, GameObject obstacle)
  //{
  //  Vector3 obstacleCenterPos = obstacle.transform.position;
  //  float obstacleRadius = obstacle.GetComponent<Renderer>().bounds.size.x / 2;
  //  float objectRadius = gameObject.GetComponent<Renderer>().bounds.size.x / 2;

  //  return Vector3.Distance()
  //}

  GameObject FindPriorityObstacle(Vector3 ahead, Vector3 ahead2)
  {
    GameObject priorityObstacle = null;

    foreach (GameObject obstacle in obstacles)
    {
      if (obstacle != null)
      {
        bool isCollided = LineCircleIntersection(ahead, ahead2, obstacle);

        if (isCollided == true)
        {
          if ((priorityObstacle == null)
            || (Vector3.Distance(transform.position, obstacle.transform.position))
            < Vector3.Distance(transform.position, priorityObstacle.transform.position))
          {
            priorityObstacle = obstacle;
          }
        }
      }
    }

    return priorityObstacle;
  }

  public Vector3 ComputeCollisionAvoidance()
  {
    if (rb == null)
      return Vector3.zero;

    float dynamicLength = rb.velocity.magnitude / maxSpeed;
    dynamicLength *= 2;

    Vector3 ahead = transform.position + rb.velocity.normalized * dynamicLength;
    Vector3 ahead2 = transform.position + rb.velocity.normalized * dynamicLength * 0.5f;

    //Vector3 ahead = transform.position + rb.velocity.normalized * seeAhead;
    //Vector3 ahead2 = transform.position + rb.velocity.normalized * seeAhead * 0.5f;

    GameObject priorityObstacle = FindPriorityObstacle(ahead, ahead2);
    Vector3 avoidance = new Vector3(0, 0, 0);

    if (priorityObstacle != null)
    {
      avoidance.x = ahead.x - priorityObstacle.transform.position.x;
      avoidance.z = ahead.z - priorityObstacle.transform.position.z;

      //avoidance.Normalize();
      //avoidance *= avoidForce;
    }
    else
    {
      //avoidance *= 0;
      avoidance = Vector3.zero;
    }

    return avoidance;
  }

  public Vector3 ComputeEvade(/*int t = 3*/GameObject target)
  {
    //Vector3 futurePos = goalPoint.transform.position + t * goalPoint.GetComponent<Rigidbody>().velocity;
    //return CalculateFlee(futurePos);

    float dist = Vector3.Distance(target.transform.position, transform.position);
    int t = (int)(dist / maxSpeed);

    Vector3 futurePos = target.transform.position + t * target.GetComponent<Rigidbody>().velocity;

    return CalculateFlee(futurePos);
  }
  public Vector3 CalculateFlee(Vector3 target)
  {
    Vector3 desiredVelocity = maxSpeed * (transform.position - target).normalized;
    Vector3 steering = desiredVelocity - GetComponent<Rigidbody>().velocity;

    return steering;
  }
}
