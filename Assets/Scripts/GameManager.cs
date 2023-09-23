using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Los objetos iran hacia el jugador, pero realmente el jugador no va avanzar. Para que todos los objetos tenga la misma velocidad
    //no voy a darles velocidades individuales, lo cogeran todos de aqui usando la funcion GetPlayerSpeed().
    private float                       m_levelSpeed = 0f;

    // Cuanto mas tiempo lleve el jugador jugando, mas rapido ira. m_timeToMaxSpeed determina el tiempo que va a tardar a llegar al maximo. m_MaxPlayerSpeed es el maximo. 
    // Cada vez que el jugador se de un golpe restamos al m_currentTime. Recibirimos un mensaje de DamagingObjects y todo lo que herede de esa clase.

    private float                                       m_currentTime = 0f;
    [SerializeField] private float                      m_timeToMaxSpeed = 0f;
    private float m_acornPickUps = 0f;
    [SerializeField] private GameObject m_acornsToMaxSpeed;
    [SerializeField] private float                      m_MaxLevelSpeed = 0f;
    [SerializeField] private float                      m_initialSpeed = 0f;


    private float                                       m_acornCounter = 0f;
    private float                                       m_speedCounter = 0f;
    [SerializeField] private GameObject                 m_acornsToMaxAddedSpeed;
    public float                                        m_spawnCallTime;
    private float                                       m_spawnExecutionTime;


    [SerializeField] GameObject                         m_floorSpawner;
    [SerializeField] GameObject                         m_floor;

    [SerializeField] GameObject                         m_obstacleSpawnPoint;
    [SerializeField] private GameObject[]               m_obstacles;
    private Coroutine                                   m_obstacleSpawnCoroutine;
    void Start()
    {
        //Incremento la velocidad del jugador en base al tiempo que lleva jugando usando una corutina.
        StartCoroutine(IncreasePlayerSpeed());
    }

    void Update()
    {
        m_currentTime += Time.deltaTime;
        SpawnObstacle();
    }

    //Funciones privadas

    private void SpawnObstacle()
    {
        if (m_obstacleSpawnCoroutine == null)
        {
            m_obstacleSpawnCoroutine = StartCoroutine(ObstacleSpawn());
        }
    }

    // Funciones Publicas


    // Los diferentes objetos que se mueven hacia el jugador pediran al gameManager la velocidad a la que iran de aqui.
    public float GetLevelSpeed()
    {
        return m_levelSpeed;
    }

    //Las piezas sueltas del nivel le pediran al GameManager nuevas piezas con esta funcion.
    public void FloorSpawnRequired()
    {
       
       GameObject floorPiece=Instantiate(m_floor, m_floorSpawner.transform.position, m_floorSpawner.transform.rotation);
       m_spawnExecutionTime = Time.time;
        floorPiece.transform.position += Vector3.forward * (m_spawnExecutionTime - m_spawnCallTime) * m_levelSpeed;
    }

    //Se le avisara al GameManager que el jugador se ha dado con un obstaculo con esta función.
    // El mensaje se recibe desde PLayerHitBox
    public void PlayerHit(float timeLost)
    {
        m_currentTime-=timeLost;
        if (m_currentTime < 0)
        {
            m_currentTime = 0f;
        }
    }
    
    //Corutinas.
    IEnumerator IncreasePlayerSpeed() 
    {
        while (m_currentTime<m_timeToMaxSpeed)
        {

            m_levelSpeed = Mathf.Lerp(m_initialSpeed, m_MaxLevelSpeed, m_currentTime/ m_timeToMaxSpeed);
            yield return null;
        }
        m_levelSpeed = m_MaxLevelSpeed;
    }

    IEnumerator ObstacleSpawn()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        GameObject spawnTarget= m_obstacles[Random.Range(0, m_obstacles.Length)];
        if (m_currentTime / m_timeToMaxSpeed >= Random.Range(0f, 1f))
        {
            Instantiate(spawnTarget, new Vector3(Random.Range(-9.5f, 9.5f), m_obstacleSpawnPoint.transform.position.y, m_obstacleSpawnPoint.transform.position.z), Quaternion.identity);
        }
        m_obstacleSpawnCoroutine= null;
    }
}
