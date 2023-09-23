using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private GameObject         m_model;

    [SerializeField] private float              m_playerHorizontalSpeed;
    [SerializeField] private float              m_jumpingSpeed;
    [SerializeField] private float              m_fallingSpeed;
    private float                               m_verticalVelocity;

    private float                               m_playerHorizontalInput;
    private bool                                m_playerVerticalInput;

    private bool                                m_touchingRight;
    private bool                                m_touchingLeft;
    private bool                                m_isGrounded    =   false;

    [SerializeField] private BoxCollider        m_boxCollider;
     
    // Update is called once per frame
    void Update()
    {
        m_playerHorizontalInput = -Input.GetAxis("Horizontal");
        CheckIfTouchingLimits();
        CalculateVerticalVelocity();
        m_model.transform.position=m_model.transform.position + new Vector3(m_playerHorizontalInput*Time.deltaTime*m_playerHorizontalSpeed,m_verticalVelocity* Time.deltaTime, 0);
    }

    private void CheckIfTouchingLimits()
    {
        if (m_touchingRight)
        {
            m_playerHorizontalInput=Mathf.Clamp(m_playerHorizontalInput, 0, 1);
        } else if (m_touchingLeft)
        {
            m_playerHorizontalInput = Mathf.Clamp(m_playerHorizontalInput, -1, 0);
        }
    }
    private void CalculateVerticalVelocity()
    {
        if (m_isGrounded)
        {
            if(Input.GetKey(KeyCode.W)|| Input.GetKey(KeyCode.Space))
            {
                m_verticalVelocity = 10f;
            } else
            {
                m_verticalVelocity = 0f;
            }
        } else
        {
            m_verticalVelocity -= m_fallingSpeed*Time.deltaTime;
        }
    }

    public void PlayerHit()
    {
        StartCoroutine(PlayerHitCoroutine());
    }
    public void IsGrounded(bool isGrounded)
    {
        m_isGrounded = isGrounded;
    }
    public void CheckTouchingRight(bool touching)
    {
        m_touchingRight = touching;
    }

    public void CheckTouchingLeft(bool touching)
    {
        m_touchingLeft = touching;
    }

    IEnumerator PlayerHitCoroutine()
    {
        m_boxCollider.enabled= false;
        m_verticalVelocity = m_jumpingSpeed;
        m_isGrounded = false;
        yield return new WaitForSeconds(0.5f);
        m_boxCollider.enabled = true;
    }
}
