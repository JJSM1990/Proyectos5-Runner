using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagingObstacle : MovingPiece
{
    [SerializeField] public float m_damage;

  
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
}

