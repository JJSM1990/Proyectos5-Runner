using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObstacle : MovingPiece
{
    [SerializeField] public float m_damage;
    [SerializeField] public float m_height;

  
    protected override void Update()
    {
        base.Update();
    }

    protected override void CheckIfSpawnNeeded()
    {
        if (m_triggersSpawn && transform.position.z >= -81)
        {
            
        }
    }

    public float GetCenter()
    {
        return m_height / 2; 
    }
}

