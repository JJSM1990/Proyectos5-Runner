using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [SerializeField] private AcornSpawner               _acornSpawner;
    [SerializeField] private MonsterHandsBehaviour      m_handsBehaviour;
    [SerializeField] private PlayerMovement             m_player;
    [SerializeField] private GameObject                 m_floorSpawner;
    [SerializeField] private GameObject                 m_floor;
    [SerializeField] private GameObject                 _previousFloorPiece;

    [SerializeField] GameObject                         m_obstacleSpawnPoint;
    [SerializeField] private GameObject[]               m_obstacles;
    private Coroutine                                   _obstacleSpawnCoroutine;
    private bool                                        _gameRunning=false;

    Vector3 startPosition;


    private bool                                        _gameOverEnabled=false;
    //Variables para el UI
    [SerializeField] TextMeshProUGUI                    m_UIcounter;
    [SerializeField] GameObject                         m_mainMenu;
    [SerializeField] Slider                             m_speedBar;
    [SerializeField] AudioSource                        m_audioSource;
    
    void Start()
    {
        _levelSpeed = 0f;
        
    }

    void Update()
    {
        if (_gameRunning)
        {
            SpawnObstacle();
        }
    }

    //Funciones privadas

    public void StartGame()
    {
        _gameRunning = true;
        StartCoroutine(BlockGameOver());
        Cursor.visible = false;
        _levelSpeed = _initialSpeed;
        m_handsBehaviour.StartMovement();
        _acornSpawner.StartGame();
        m_player.GameStart();
        m_audioSource.Play();
        m_mainMenu.SetActive(false);
    }

    public void EndGame()
    {
        Application.Quit();
    }

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
        floorPiece.transform.position = new Vector3(_previousFloorPiece.transform.position.x, _previousFloorPiece.transform.position.y, _previousFloorPiece.transform.position.z - 40);
        _previousFloorPiece= floorPiece;
    }


    public void PlayerSliding(float slideTime)
    {
        StartCoroutine(PlayerSlidingCoroutine(slideTime));
    }

    // Funciones para las bellotas
    public void AcornPickUp()
    {
        _acornCounter++;
        _speedCounter++;
        UpdateAcorns();
    }

    public void AcornLost()
    {
        _acornCounter--;
        _speedCounter-=2;
        UpdateAcorns();
    }

    private void UpdateAcorns()
    {
        _speedCounter = Mathf.Clamp(_speedCounter, 0f, _acornsToMaxSpeed);
        m_UIcounter.text = _acornCounter.ToString();
        UpdatePlayerSpeed();
        m_speedBar.value = _speedCounter / _acornsToMaxSpeed;
        m_handsBehaviour.updateZ(_speedCounter, _acornsToMaxSpeed);
    }



    //Se le avisara al GameManager que el jugador se ha dado con un obstaculo con esta función.
    // El mensaje se recibe desde PLayerHitBox
    public void PlayerHit(float timeLost)
    {
        _speedCounter -= timeLost;
        UpdateAcorns();
    }
   
    public void HandsTouchingPlayer()
    {
        if (_gameOverEnabled)
        {
            Cursor.visible = true;
            m_audioSource.Stop();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    //Corutinas.

    IEnumerator ObstacleSpawn()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        GameObject spawnTarget = m_obstacles[Random.Range(0, m_obstacles.Length)];
        if (_speedCounter / _acornsToMaxSpeed >= Random.Range(0f, 1f))
        {
            Vector3 spawnPosition = new Vector3(Random.Range(-9.5f, 9.5f), m_obstacleSpawnPoint.transform.position.y, m_obstacleSpawnPoint.transform.position.z);       
            if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hitInfo, 11f))
            {
                switch (spawnTarget.name)
                {
                    case "WalkableObstacle":
                        Instantiate(spawnTarget, new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z - 20f), Quaternion.identity);
                        break;
                    case "SlidingObstacle":
                        Instantiate(spawnTarget, new Vector3(0f, hitInfo.point.y, hitInfo.point.z), Quaternion.identity);
                        break;
                    default:
                        Instantiate(spawnTarget, hitInfo.point, Quaternion.identity);
                        break;
                }
            }
            
        }
        _obstacleSpawnCoroutine = null;
    }

    IEnumerator BlockGameOver()
    {
        yield return new WaitForSeconds(10f);
        _gameOverEnabled = true;
    }

    IEnumerator PlayerSlidingCoroutine(float slideTime)
    {
        _speedCounter += 5f;
        UpdateAcorns();
        yield return new WaitForSeconds(slideTime);
        _speedCounter -= 5f;
        UpdateAcorns();

    }
}

