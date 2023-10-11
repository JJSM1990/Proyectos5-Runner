using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterHandsBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private GameManager m_gameManager;
    private float _currentXcoordinate;

    [SerializeField] private float _minZcoordinate;
    [SerializeField] private float _maxZcoordinate;
    private float _currentZcoordinate;

    [SerializeField] private float _updateDuration;
    private bool _updatingPosition;
    private Coroutine _updatingCoroutine;

    [SerializeField] private IndividualHandsBehaviour leftHand, rightHand;
    private bool GameEnding;






    private void Update()
    {
        transform.position = new Vector3(0, 14, _currentZcoordinate);
        if (_currentZcoordinate == _minZcoordinate)
        {
            m_gameManager.HandsTouchingPlayer();
        }
    }

    public void updateZ(float currentCounter, float maxCounter)
    {
        if (_updatingCoroutine != null)
        {
            StopCoroutine(_updatingCoroutine);
            _updatingCoroutine = null;
        }
        float targetZ = (currentCounter / maxCounter) * _maxZcoordinate;
        if (currentCounter <1)
        {
            GameEnding = true;
            leftHand.MoveHandsX(_updateDuration, GameEnding);
            rightHand.MoveHandsX(_updateDuration, GameEnding);
        }
        else if (GameEnding)
        {
            GameEnding = false;
            leftHand.MoveHandsX(_updateDuration, GameEnding);
            rightHand.MoveHandsX(_updateDuration, GameEnding);
        }
        _updatingCoroutine = StartCoroutine(UpdateZ(targetZ));
    }

    IEnumerator UpdateZ(float targetZ)
    {
        {
            float time = 0f;
            float startingZ = _currentZcoordinate;
            while (time < _updateDuration)
            {
                _currentZcoordinate = Mathf.Lerp(startingZ, targetZ, time / _updateDuration);
                time += Time.deltaTime;
                yield return null;
            }
            _currentZcoordinate = targetZ;
            _updatingCoroutine = null;
        }
    }
}
