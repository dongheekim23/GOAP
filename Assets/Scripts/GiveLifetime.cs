using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveLifetime : MonoBehaviour
{
  float elapsedTime;
  float lifeTime;

  // Use this for initialization
  void Start()
  {
    elapsedTime = 0f;
    lifeTime = 0.3f;
  }

  // Update is called once per frame
  void Update()
  {
    if (elapsedTime >= lifeTime)
      Destroy(gameObject);

    elapsedTime += Time.deltaTime;
  }
}
