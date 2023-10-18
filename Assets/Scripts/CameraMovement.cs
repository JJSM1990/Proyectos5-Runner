using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 positionMenu = new Vector3(0, 3.94f, 6.5f);
    private Vector3 positionGameplay = new Vector3(0, 18.32f, 12.9f);
    private Vector3 rotationGameplay = new Vector3(35.9f, 180f, 0f);
    private float durationChange = 2.0f;

    public void StartGame()
    {
        StartCoroutine(ChangeCameraPosition(positionGameplay, rotationGameplay));
    }


    IEnumerator ChangeCameraPosition(Vector3 newPosition, Vector3 newRotation)
    {
        Vector3 startingPosition = transform.position;
        Vector3 startingRotation = transform.rotation.eulerAngles;
        float counter = 0f;
        while (counter<durationChange)
        {
            counter+= Time.deltaTime;
            transform.position=Vector3.Lerp(startingPosition, newPosition, counter / durationChange);
            transform.eulerAngles = Vector3.Lerp(startingRotation, newRotation, counter / durationChange);
            yield return null;
        }
        transform.position = newPosition;
    }
}
