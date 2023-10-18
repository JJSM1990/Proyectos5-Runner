using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] GameManager                gameManager;
    //Components
    [SerializeField] private GameObject         m_model;
    [SerializeField] private Animator           m_anim;


    //Movement variables
    [Header("Player Movement")]
    [SerializeField] private float              _playerHorizontalSpeed;
    [SerializeField] private float              _jumpingSpeed;
    [SerializeField] private float              _onHitVerticalSpeed;
    [SerializeField] private float              _fallingAcceleration;
    [SerializeField] private float              _dropSpeed;
    [SerializeField] private float              _slideDuration;
    [SerializeField] private float              _slideCooldown;
    private bool                                _gameRunning = false;
    private float                               _verticalVelocity;


    // Sliding Variables
    private bool                                _removePlayerControl = true;
    private bool                                _slideReady=true;
    private Vector3                             _normalHitboxSize = new Vector3(0.67f, 2.7f, 0.46f);
    private Vector3                             _normalHitboxCenter = new Vector3(-0.06f, 1.38f, 0.24f);
    private Vector3                             _slidingHitboxSize = new Vector3(0.67f, 1.75f, 0.46f);
    private Vector3                             _slidingHitboxCenter = new Vector3(-0.06f, 0.9134351f, 0.24f);
    
    //Player inputs
    private float                               _playerHorizontalInput;

    //Players collisions from PlayerHitboxes
    private bool                                _touchingRight;
    private bool                                _touchingLeft;
    private bool                                _isGrounded    =   false;

    private Coroutine _slidingCoroutine;

    [SerializeField] private BoxCollider        m_boxCollider;

    [SerializeField] private AudioSource        m_runningSource;

    [SerializeField] private List<AudioClip>    m_footSteplist;
    // Update is called once per frame
    void Update()
    {
        CalculateVerticalMovement();
        CapturePlayerHorizontalInput();
        CheckIfTouchingLimits();
      
        m_model.transform.position=m_model.transform.position + new Vector3(_playerHorizontalInput*Time.deltaTime*_playerHorizontalSpeed,_verticalVelocity* Time.deltaTime, 0);
    }

    private void CapturePlayerHorizontalInput()
    {
        if (_removePlayerControl || !_gameRunning)
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
    private void CalculateVerticalMovement()
    {
        if (_isGrounded)
        {
            if(!_removePlayerControl && (Input.GetKey(KeyCode.W)|| Input.GetKey(KeyCode.Space)))
            {
                _verticalVelocity = 10f;
                
                SoundManager.Instance.PlaySound(1);
            } else
            {
                _verticalVelocity = 0f;
                if (Input.GetKeyDown(KeyCode.S)&& _gameRunning&& _slideReady)
                {
                    _slidingCoroutine=StartCoroutine(Sliding());
                }
            }

        } else if (Input.GetKey(KeyCode.S) && _gameRunning && !_removePlayerControl)
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
                _removePlayerControl = false;
                ChangeHitBoxSliding(false);
                StopCoroutine(_slidingCoroutine);
                m_anim.SetTrigger("slideEnd");
                _slidingCoroutine = null;
            }

        }

    }
    public void PlayFootStep()
    {
        if (_isGrounded)
        {
            m_runningSource.PlayOneShot(m_footSteplist[Random.Range(0, m_footSteplist.Count)]);
        }
        
    }

    private void ChangeHitBoxSliding(bool isSliding)
    {
        BoxCollider bc = m_model.GetComponent<BoxCollider>();
        if (isSliding)
        {
            bc.center = _slidingHitboxCenter;
            bc.size = _slidingHitboxSize;
        }
        else
        {
            Debug.Log("HitBox changed to normal");
            bc.center = _normalHitboxCenter;
            bc.size = _normalHitboxSize;
        }
    } 
    public void ChangeGameState(bool running)
    {
        _gameRunning = running;
        _removePlayerControl = !running;
        if (running)
        {
            m_anim.SetTrigger("Start");
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

    private void EndSlide()
    {
        ChangeHitBoxSliding(false);
        StopCoroutine(_slidingCoroutine);
        _slidingCoroutine = null;
        SoundManager.Instance.StopSound();
    }
    IEnumerator PlayerHitCoroutine()
    {
        m_boxCollider.enabled= false;
        _removePlayerControl = true;
        _touchingLeft = _touchingRight = false;
        _verticalVelocity = _onHitVerticalSpeed;
        _isGrounded = false;
        m_anim.SetTrigger("playerHit");
        if (_slidingCoroutine!=null)
        {
            EndSlide();
        }
        yield return new WaitForSeconds(0.5f);
        _removePlayerControl = false;
        m_anim.SetBool("grounded", _isGrounded);
        m_boxCollider.enabled = true;
    }

    IEnumerator Sliding()
    {
        StartCoroutine(SliderCooldown());
        SoundManager.Instance.PlaySound(3);
        ChangeHitBoxSliding(true);
        gameManager.PlayerSliding(_slideDuration);
        _removePlayerControl = true;
        m_anim.SetTrigger("slideStart");
        _playerHorizontalInput = 0;
        yield return new WaitForSeconds(_slideDuration);
        _removePlayerControl = false;
        m_anim.SetTrigger("slideEnd");
        EndSlide();
    }

    IEnumerator SliderCooldown()
    {
        _slideReady = false;
        yield return new WaitForSeconds(_slideCooldown);
        _slideReady = true;
    }
}
