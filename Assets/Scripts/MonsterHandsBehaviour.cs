using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MonsterHandsBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameManager m_gameManager;
    private float _currentXcoordinate;
    private bool _gameStarted = false;

    [SerializeField] private float _minZcoordinate;
    [SerializeField] private float _maxZcoordinate;
    private float _currentZcoordinate;

    [SerializeField] private float _updateDuration;
    private bool _updatingPosition;
    private Coroutine _updatingCoroutine;
    private bool _gameEnding;

    [SerializeField] private GameObject leftHand, rightHand;
    private Coroutine coroutineLH, coroutineRH;
    [SerializeField] private float startLHPosition, startRHPosition, endLHPosition, endRHPosition;


    private void Update()
    {
        Debug.Log(_currentZcoordinate);
        if (_gameStarted)
        {
            transform.position = new Vector3(0, 14, _currentZcoordinate);
            if (_currentZcoordinate == _minZcoordinate)
            {
                m_gameManager.HandsTouchingPlayer();
            }
        }
    }

    public void StartMovement()
    {
        _currentZcoordinate = _maxZcoordinate;
        _updatingCoroutine = StartCoroutine(UpdateZ(_minZcoordinate));
        _gameStarted=true;
    }
    public void updateZ(float currentCounter, float maxCounter)
    {
        if (_updatingCoroutine != null)
        {
            StopCoroutine(_updatingCoroutine);
            _updatingCoroutine = null;
        }
        float targetZ = _minZcoordinate +((currentCounter / maxCounter) * _maxZcoordinate);
        if (currentCounter <1)
        {
            UpdateHandsX(true);
        }
        else if (_gameEnding)
        {
            UpdateHandsX(false);
        }
        _updatingCoroutine = StartCoroutine(UpdateZ(targetZ));
    }

    IEnumerator UpdateZ(float targetZ)
    {
            float time = 0f;
            float startingZ = _currentZcoordinate;
            while (time < _updateDuration)
            {
                time += Time.deltaTime;
                _currentZcoordinate = Mathf.Lerp(startingZ, targetZ, time / _updateDuration);
                
                yield return null;
            }
            _currentZcoordinate = targetZ;
            _updatingCoroutine = null;
    }



    //Individual Hands

    private void UpdateHandsX(bool gameEnding)
    {
        _gameEnding = gameEnding;
        if (coroutineLH!=null|| coroutineRH != null)
        {
            StopCoroutine(coroutineLH);
            StopCoroutine(coroutineRH);
        }
        
        if (_gameEnding)
        {
            coroutineLH = StartCoroutine(UpdateIndividualX(leftHand, endLHPosition));
            coroutineRH = StartCoroutine(UpdateIndividualX(rightHand, endRHPosition));
        } else if(!_gameEnding)
        {
            coroutineLH = StartCoroutine(UpdateIndividualX(leftHand, startLHPosition));
            coroutineRH = StartCoroutine(UpdateIndividualX(rightHand, startRHPosition));
        }
    }
    IEnumerator UpdateIndividualX(GameObject hand, float targetX)
    {
        float time = 0f;
        float startingX=hand.transform.position.x;
        float currentX=startingX;
        while (time<_updateDuration)
        {
            time+= Time.deltaTime;
            Vector3 currentPosition = hand.transform.position;
            currentX =Mathf.Lerp(startingX, targetX, time / _updateDuration);
            hand.transform.position= new Vector3(currentX, currentPosition.y, currentPosition.z);
            yield return new WaitForEndOfFrame();
        }
        if (hand.name =="LHempty")
        {
            coroutineLH = null;
        }
        else if (hand.name == "RHempty")
        {
            coroutineRH= null;
        }
        else
        {
            Debug.Log("Error en los nombres de las manos");
        }
    }
}
