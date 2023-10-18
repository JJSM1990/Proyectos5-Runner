using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 positionMenu = new Vector3(0, 8.4f, 6.5f);
    private Vector3 positionGameplay = new Vector3(0, 18.32f, 12.9f);
    private float durationChange = 1.0f;

    public void StartGame()
    {
        StartCoroutine(ChangeCameraPosition(positionGameplay));
    }

    public void EndGame()
    {
        StartCoroutine(ChangeCameraPosition(positionMenu));
    }

    IEnumerator ChangeCameraPosition(Vector3 newPosition)
    {
        Vector3 startingPosition = transform.position;
        float counter = 0f;
        while (counter<durationChange)
        {
            counter+= Time.deltaTime;
            transform.position=Vector3.Lerp(startingPosition, newPosition, counter / durationChange);
            yield return null;
        }
        transform.position = newPosition;
    }
}
