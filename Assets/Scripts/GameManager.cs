using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Los objetos iran hacia el jugador, pero realmente el jugador no va avanzar. Para que todos los objetos tenga la misma velocidad
    //no voy a darles velocidades individuales, lo cogeran todos de aqui usando la funcion GetPlayerSpeed().
    private float                       _levelSpeed = 0f;

    // Cuanto mas tiempo lleve el jugador jugando, mas rapido ira. m_timeToMaxSpeed determina el tiempo que va a tardar a llegar al maximo. m_MaxPlayerSpeed es el maximo. 
    // Cada vez que el jugador se de un golpe restamos al m_currentTime. Recibirimos un mensaje de DamagingObjects y todo lo que herede de esa clase.

    [SerializeField] private float                      _MaxLevelSpeed = 0f;
    [SerializeField] private float                      _initialSpeed = 0f;


    static private float                                _acornCounter = 0f;
    [SerializeField] private float                      _speedCounter = 0f;
    [SerializeField] private float                      _acornsToMaxSpeed;
    [SerializeField] public float                       _spawnCallTime;
    [SerializeField] private float                      _spawnExecutionTime;

    [SerializeField] private MonsterHandsBehaviour      m_handsBehaviour;
    [SerializeField] GameObject                         m_floorSpawner;
    [SerializeField] GameObject                         m_floor;

    [SerializeField] GameObject                         m_obstacleSpawnPoint;
    [SerializeField] private GameObject[]               m_obstacles;
    private Coroutine                                   _obstacleSpawnCoroutine;


    private bool                                        _gameOverEnabled=false;
    //Variables para el UI
    [SerializeField] TextMeshProUGUI                    m_UIcounter;
    void Start()
    {
        _levelSpeed = _initialSpeed;
        StartCoroutine(BlockGameOver());
    }

    void Update()
    {
        SpawnObstacle();
    }

    //Funciones privadas

    private void SpawnObstacle()
    {
        if (_obstacleSpawnCoroutine == null)
        {
            _obstacleSpawnCoroutine = StartCoroutine(ObstacleSpawn());
        }
    }

    private void UpdatePlayerSpeed()
    {

        _levelSpeed =  _speedCounter == _acornsToMaxSpeed ?
            _MaxLevelSpeed:
            Mathf.Lerp(_initialSpeed, _MaxLevelSpeed, _speedCounter / _acornsToMaxSpeed);

    }

    // Funciones Publicas


    // Los diferentes objetos que se mueven hacia el jugador pediran al gameManager la velocidad a la que iran de aqui.
    public float GetLevelSpeed()
    {
        return _levelSpeed;
    }

    //Las piezas sueltas del nivel le pediran al GameManager nuevas piezas con esta funcion.
    public void FloorSpawnRequired()
    {
        GameObject floorPiece=Instantiate(m_floor, m_floorSpawner.transform.position, m_floorSpawner.transform.rotation);
        floorPiece.GetComponent<MovingPiece>().GetStartSpeed(_levelSpeed);
    }

    public void AcornPickUp()
    {
        _acornCounter++;
        _speedCounter++;
        m_UIcounter.text = "Acorns: "+_acornCounter.ToString();
        UpdatePlayerSpeed();
        m_handsBehaviour.updateZ(_speedCounter, _acornsToMaxSpeed, _levelSpeed);
    }

    //Se le avisara al GameManager que el jugador se ha dado con un obstaculo con esta función.
    // El mensaje se recibe desde PLayerHitBox
    public void PlayerHit(float timeLost)
    {
        _speedCounter -= timeLost;
        _speedCounter = Mathf.Clamp(_speedCounter, 0f, _acornsToMaxSpeed);
        UpdatePlayerSpeed();
        m_handsBehaviour.updateZ(_speedCounter, _acornsToMaxSpeed, _levelSpeed);
    }
   
    public void HandsTouchingPlayer()
    {
        if (_gameOverEnabled)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    //Corutinas.

    IEnumerator ObstacleSpawn()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        GameObject spawnTarget= m_obstacles[Random.Range(0, m_obstacles.Length)];
        if (_speedCounter / _acornsToMaxSpeed >= Random.Range(0f, 1f))
        {
            Instantiate(spawnTarget, new Vector3(Random.Range(-9.5f, 9.5f), spawnTarget.GetComponent<DamagingObstacle>().GetCenter()-1.5f,m_obstacleSpawnPoint.transform.position.z), Quaternion.identity) ;
        }
        _obstacleSpawnCoroutine= null;
    }

    IEnumerator BlockGameOver()
    {
        yield return new WaitForSeconds(10f);
        _gameOverEnabled = true;
    }
}

