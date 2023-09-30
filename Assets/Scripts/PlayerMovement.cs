using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Components
    [SerializeField] private GameObject         m_model;
    [SerializeField] private Animator           m_anim;

    //Movement variables
    [SerializeField] private float              _playerHorizontalSpeed;
    [SerializeField] private float              _jumpingSpeed;
    [SerializeField] private float              _fallingAcceleration;
    [SerializeField] private float              _dropSpeed;
    [SerializeField] private float              _slideDuration;
    private bool                                _sliding = false;
    private float                               _verticalVelocity;

    
    //Player inputs
    private float                               _playerHorizontalInput;

    //Players collisions from PlayerHitboxes
    private bool                                _touchingRight;
    private bool                                _touchingLeft;
    private bool                                _isGrounded    =   false;

    private Coroutine _slidingCoroutine;

    [SerializeField] private BoxCollider        m_boxCollider;
     
    // Update is called once per frame
    void Update()
    {
        CapturePlayerHorizontalInput(_sliding);
        CheckIfTouchingLimits();
        CalculateVerticalVelocity();
        m_model.transform.position=m_model.transform.position + new Vector3(_playerHorizontalInput*Time.deltaTime*_playerHorizontalSpeed,_verticalVelocity* Time.deltaTime, 0);
    }

    private void CapturePlayerHorizontalInput(bool isSliding)
    {
        if (isSliding)
        {
            _playerHorizontalInput = 0;
        }
        else
        {
            _playerHorizontalInput = -Input.GetAxis("Horizontal");
        }
    }

    private void CheckIfTouchingLimits()
    {
        if (_touchingRight)
        {
            _playerHorizontalInput=Mathf.Clamp(_playerHorizontalInput, 0, 1);
        } else if (_touchingLeft)
        {
            _playerHorizontalInput = Mathf.Clamp(_playerHorizontalInput, -1, 0);
        }
    }
    private void CalculateVerticalVelocity()
    {
        if (_isGrounded)
        {
            if(Input.GetKey(KeyCode.W)|| Input.GetKey(KeyCode.Space))
            {
                _verticalVelocity = 10f;
            } else
            {
                _verticalVelocity = 0f;
                if (Input.GetKeyDown(KeyCode.S))
                {
                    _slidingCoroutine=StartCoroutine(Sliding());
                }
            }

        } else if (Input.GetKeyDown(KeyCode.S))
        {

            _verticalVelocity= -_dropSpeed;
            m_anim.SetTrigger("playerDrop");

        } else
        {

            _verticalVelocity -= _fallingAcceleration*Time.deltaTime;
        }
    }

    public void PlayerHit()
    {
        StartCoroutine(PlayerHitCoroutine());
    }
    public void IsGrounded(bool isGrounded)
    {
        _isGrounded = isGrounded;
        m_anim.SetBool("grounded", isGrounded);
        if (!isGrounded)
        {
            if (_slidingCoroutine != null)
            {
                _sliding = false;
                StopCoroutine(_slidingCoroutine);
                m_anim.SetTrigger("slideEnd");
                _slidingCoroutine = null;
            }
        }

    }
    public void CheckTouchingRight(bool touching)
    {
        _touchingRight = touching;
    }

    public void CheckTouchingLeft(bool touching)
    {
        _touchingLeft = touching;
    }

    IEnumerator PlayerHitCoroutine()
    {
        m_boxCollider.enabled= false;
        _touchingLeft = _touchingRight = false;
        _verticalVelocity = _jumpingSpeed*2;
        _isGrounded = false;
        m_anim.SetTrigger("playerHit"); 
        yield return new WaitForSeconds(0.5f);
        m_anim.SetBool("grounded", _isGrounded);
        m_boxCollider.enabled = true;
    }

    IEnumerator Sliding()
    {
        _sliding = true;
        m_anim.SetTrigger("slideStart");
        _playerHorizontalInput = 0;
        yield return new WaitForSeconds(_slideDuration);
        _sliding = false;
        m_anim.SetTrigger("slideEnd");
        _slidingCoroutine = null;
    }
}
