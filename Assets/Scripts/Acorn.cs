using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acorn : MovingPiece, IPickable
{

    // Update is called once per frame
    protected override void Update()
    {
      base.Update();
    }

    public void PickUp()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().AcornPickUp();
        Destroy(this.gameObject);
    }

    protected override void CheckForDestruction()
    {
        if (transform.position.z >= 5 || transform.position.y >= 8)
        {
            m_gameManager.AcornLost();
            Destroy(this.gameObject);

        }
    }
}
