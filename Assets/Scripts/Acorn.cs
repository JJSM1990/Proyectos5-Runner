using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acorn : MovingPiece, IPickable
{
    [SerializeField] private float _acornValue;
    [SerializeField] private float _acornValueOnLoss;
    [SerializeField] private int _noiseOnPickUp;
    [SerializeField] private int _noiseOnDestroy;

    // Update is called once per frame
    protected override void Update()
    {
      base.Update();
    }

    public void PickUp()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().AcornPickUp(_acornValue, _noiseOnPickUp);
        Destroy(this.gameObject);
    }

    protected override void CheckForDestruction()
    {
        if (transform.position.z >= 5 || transform.position.y >= 8)
        {
            m_gameManager.AcornLost(_acornValueOnLoss, _noiseOnDestroy);
            Destroy(this.gameObject);

        }
    }
}
