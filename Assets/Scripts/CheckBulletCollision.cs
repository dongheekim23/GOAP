using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckBulletCollision : MonoBehaviour
{
  GameObject[] m_bullets;
  AgentStats m_stats;

  // Use this for initialization
  void Start()
  {
    m_stats = gameObject.GetComponent<AgentStats>();
  }

  // Update is called once per frame
  void Update()
  {
    if (m_stats.health <= 0f)
    {
      //if (m_bt != null)
      //  Destroy(m_bt.textObject);
      if(gameObject.GetComponent<GOAP_base>() != null)
        Destroy(gameObject.GetComponent<GOAP_base>().textObject);
      Destroy(gameObject);

      return;
    }

    m_bullets = GameObject.FindGameObjectsWithTag("Bullet");

    if (m_bullets.Length == 0)
      return;

    float agentSize = gameObject.GetComponent<Renderer>().bounds.size.x;

    foreach (GameObject bullet in m_bullets)
    {
      float bulletSize = bullet.GetComponent<Renderer>().bounds.size.x;

      // If the agent is collided with the enemy, then agent dies (eaten)
      if (Vector3.Distance(gameObject.transform.position, bullet.transform.position)
        <= (agentSize + bulletSize) / 2f)
      {
        Destroy(bullet);
        m_stats.health -= bullet.GetComponent<BulletStats>().damage;

        return;
      }
    }
  }
}
