using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPiece : MonoBehaviour
{
    [SerializeField] private Rigidbody  m_rigidbody;
    protected GameManager               m_gameManager;
    private float                       m_levelSpeed = 0f;


    [SerializeField] protected bool m_triggersSpawn = true;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_gameManager==null)
        {
            m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        }
        m_levelSpeed = m_gameManager.GetLevelSpeed();
        CheckIfSpawnNeeded();
        CheckForDestruction();
    }

    private void FixedUpdate()
    {
        UpdateFloorVelocity();
    }
    virtual protected void CheckIfSpawnNeeded()
    {
        if (m_triggersSpawn && transform.position.z >= -81)
        {
            UnityEngine.Debug.Log("Spawning");
            m_triggersSpawn = false;
            m_gameManager.m_spawnCallTime= Time.time;
            m_gameManager.FloorSpawnRequired();
        }
    }

    private void CheckForDestruction()
    {
        if (transform.position.z >= 40)
        {
            Destroy(this.gameObject);
        }
    }
    private void UpdateFloorVelocity()
    {
        m_rigidbody.velocity = Vector3.forward*m_levelSpeed;
    }
}
