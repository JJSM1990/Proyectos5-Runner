using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcornSpawner : MonoBehaviour
{
    [SerializeField] private GameObject m_Acorn;
    [SerializeField] private float m_distanceBetweenSpawns;
    private float m_timeBetweenSpawns;
    [SerializeField] private GameManager m_GameManager;
    [SerializeField] private float m_areaToCheck;

    private float m_previousXcoordinate;
    private float m_newXcoordinate;
    private float m_yCoordinate;
    private Coroutine m_SpawnCoroutine;

    private void Start()
    {
        m_previousXcoordinate = 0f;
    }

    private void Update()
    {
        SpawnAcorn();
    }

    private void SpawnAcorn()
    {
        if(m_SpawnCoroutine == null)
        {
            m_SpawnCoroutine= StartCoroutine(SpawnCoroutine());
        }
    }

    private void CheckHeight(float xCoordinate, float areaToCheck)
    {
        Physics.SphereCast(new Vector3(xCoordinate, transform.position.y, transform.position.z), m_areaToCheck, Vector3.down, out RaycastHit hitInfo, 10);
        m_yCoordinate = hitInfo.point.y;
    }

    private void CalculateTimeBetweenSpawn()
    {
        m_timeBetweenSpawns = m_distanceBetweenSpawns / m_GameManager.GetLevelSpeed();
    }

    IEnumerator SpawnCoroutine()
    {
        CalculateTimeBetweenSpawn();
        yield return new WaitForSeconds(m_timeBetweenSpawns);
        m_newXcoordinate = m_previousXcoordinate + Random.Range(-3f, 3f);
        m_newXcoordinate = Mathf.Clamp(m_newXcoordinate, -9f, 9f);
        CheckHeight(m_newXcoordinate, m_areaToCheck);
        Instantiate(m_Acorn, new Vector3(m_newXcoordinate, m_yCoordinate, transform.position.z), Quaternion.identity);
        m_SpawnCoroutine = null;
    }
}
