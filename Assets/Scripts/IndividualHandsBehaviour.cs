using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class IndividualHandsBehaviour : MonoBehaviour
{
    [SerializeField] private float startX;
    [SerializeField] private float endX;

    private Coroutine moveCoroutine;

    private float currentX;


    public void MoveHandsX(float timeToExecute, bool gameOver)
    {

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        float target = gameOver ? endX : startX;
        StartCoroutine(HandMovement(target, timeToExecute));
    }



    IEnumerator HandMovement(float targetPosition, float timeToExecute)
    {
        float currentTime = 0;
        float startingX = transform.position.x;
        while (currentTime<timeToExecute)
        {
            currentX = Mathf.Lerp(startingX, targetPosition, currentTime / timeToExecute);
            currentTime += Time.deltaTime;
            transform.position = new Vector3(currentX, transform.position.y, transform.position.z);
            yield return null;
        }
        moveCoroutine = null;
    }
}
