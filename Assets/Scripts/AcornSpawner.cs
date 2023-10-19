using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcornSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] m_acornList;
    [SerializeField] private float m_distanceBetweenSpawns;
    private float m_timeBetweenSpawns;
    [SerializeField] private GameManager m_GameManager;
    [SerializeField] private float m_areaToCheck;

    private float m_previousXcoordinate;
    private float m_newXcoordinate;
    private float m_yCoordinate;
    private Coroutine m_SpawnCoroutine;
    private bool m_gameStart;

    private void Start()
    {
        m_previousXcoordinate = 0f;
    }

    private void Update()
    {
        if (m_gameStart)
        {
            SpawnAcorn();
        }
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
        Physics.SphereCast(new Vector3(xCoordinate, transform.position.y, transform.position.z), m_areaToCheck, Vector3.down, out RaycastHit hitInfo, 20);
        m_yCoordinate = hitInfo.point.y;
    }

    private void CalculateTimeBetweenSpawn()
    {
        m_timeBetweenSpawns = m_distanceBetweenSpawns / m_GameManager.GetLevelSpeed();
    }

    public void StartGame()
    {
        m_gameStart = true;
    }

    IEnumerator SpawnCoroutine()
    {
        CalculateTimeBetweenSpawn();
        yield return new WaitForSeconds(m_timeBetweenSpawns);
        m_newXcoordinate = m_previousXcoordinate + Random.Range(-3f, 3f);
        m_newXcoordinate = Mathf.Clamp(m_newXcoordinate, -9f, 9f);
        CheckHeight(m_newXcoordinate, m_areaToCheck);
        int diceRoll = Random.Range(0, 10);
        GameObject spawnTarget;
        switch (diceRoll)
        {
            case 0:
                spawnTarget = m_acornList[1];
                break;
            case 1:
                spawnTarget = m_acornList[2];
                break;
            default:
                spawnTarget= m_acornList[0];
                break;
        }
        Instantiate(spawnTarget, new Vector3(m_newXcoordinate, m_yCoordinate, transform.position.z), Quaternion.identity);
        m_SpawnCoroutine = null;
    }
}
