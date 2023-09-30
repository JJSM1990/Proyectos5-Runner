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






    private void Update()
    {
        transform.position = new Vector3(0, 8, _currentZcoordinate);
        if (_currentZcoordinate == _minZcoordinate)
        {
            m_gameManager.HandsTouchingPlayer();
        }
    }

    private void CalculateX()
    {
        _currentXcoordinate = _playerTransform.position.x;
    }
    public void updateZ(float currentCounter, float maxCounter, float currentLevelSpeed)
    {
        if (_updatingCoroutine != null)
        {
            StopCoroutine(_updatingCoroutine);
            _updatingCoroutine = null;
        }
        float targetZ = (currentCounter / maxCounter) * _maxZcoordinate;
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
                Debug.Log(time / _updateDuration + " " + _currentZcoordinate);
                yield return null;
            }
            _currentZcoordinate = targetZ;
            _updatingCoroutine = null;
        }
    }
}
