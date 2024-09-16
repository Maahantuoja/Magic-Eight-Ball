using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    private bool isShaking = false;
    private float shakeThreshold = 2.0f; // Muuta tarvittaessa

    private Quaternion targetRotation = Quaternion.Euler(0, 180, 0);
    private float rotationTime = 1.0f; // Aika, joka kuluu kääntymiseen

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shaking();
    }

    void Shaking()
    {
        // Puhelimen ravistustarkistus
        if (Input.acceleration.sqrMagnitude >= shakeThreshold * shakeThreshold)
        {
            if (!isShaking)
            {
                isShaking = true;
                StartCoroutine(RotateToTarget());
            }
        }
        else
        {
            isShaking = false;
        }
    }

    IEnumerator RotateToTarget()
    {
        float startTime = Time.time;
        Quaternion startRotation = transform.rotation;

        while (Time.time - startTime < rotationTime)
        {
            float journeyLength = (Time.time - startTime) / rotationTime;
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, journeyLength);
            yield return null;
        }

        transform.rotation = targetRotation; // Varmista, että kääntö on täysin asetettu asentoon
    }
}
