using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitBox : MonoBehaviour
{
    private GameManager m_gameManager;
    [SerializeField] private PlayerMovement m_movement;

    private void Start()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "RightLimit")
        {
            Debug.Log("touching");
            m_movement.CheckTouchingRight(true);
        } else if (other.name == "LeftLimit")
        {
            m_movement.CheckTouchingLeft(true);
        }
        if (other.tag== "Obstacle")
        {
            Debug.Log("Contact");
            m_gameManager.PlayerHit(other.gameObject.GetComponent<DamagingObstacle>().m_damage);
            m_movement.PlayerHit();

        }
        IPickable ip = other.GetComponent<IPickable>();
        if (ip != null)
        {
            ip.PickUp();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag=="Walkable")
        {
            
            m_movement.IsGrounded(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "RightLimit")
        {
            m_movement.CheckTouchingRight(false);
        } else if (other.name == "LeftLimit")
        {
            m_movement.CheckTouchingLeft(false);
        }

        if (other.tag=="Walkable")
        {
            m_movement.IsGrounded(false);
        }

    }
}
