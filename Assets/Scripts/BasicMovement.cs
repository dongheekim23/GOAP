using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
  Rigidbody rb;

  //public float maxSpeed;
  //public float slowingRadius;

  void Start()
  {
    rb = gameObject.GetComponent<Rigidbody>();
  }
  
  public Vector3 ComputeSeek(Vector3 target, float maxSpeed)
  {
    Vector3 desiredVelocity = maxSpeed * (target - transform.position).normalized;
    Vector3 steering = desiredVelocity - GetComponent<Rigidbody>().velocity;

    return steering;
  }

  public Vector3 ComputeArrive(Vector3 targetPos, float maxSpeed = 10, float slowingRadius = 2)
  {
    Vector3 desiredVelocity = targetPos - transform.position;
    float distance = desiredVelocity.magnitude;

    if (distance < slowingRadius)
    {
      desiredVelocity = desiredVelocity.normalized * maxSpeed * (distance / slowingRadius);
    }
    else
    {
      desiredVelocity = desiredVelocity.normalized * maxSpeed;
    }

    return desiredVelocity - rb.velocity;
  }
  
  public Vector3 ComputeSeparation(string objectTag)
  {
    GameObject[] agents = GameObject.FindGameObjectsWithTag(objectTag);

    Vector3 v = new Vector3(0, 0, 0);
    int neighborCount = 0;

    float objRadius = gameObject.GetComponent<Renderer>().bounds.size.x;

    foreach (GameObject agent in agents)
    {
      if (gameObject != agent)
      {
        if (Vector3.Distance(gameObject.GetComponent<Transform>().position, agent.GetComponent<Transform>().position) <= objRadius)
        {
          v += agent.GetComponent<Transform>().position - gameObject.GetComponent<Transform>().position;
          ++neighborCount;
        }
      }
    }

    if (neighborCount != 0)
    {
      v /= neighborCount;
      v *= -1;
      v.Normalize();
    }

    return v;
  }
}
