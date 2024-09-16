using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Random = UnityEngine.Random;

public class AllInOne : MonoBehaviour
{
    // ROTATION VARIABLES
    private Rigidbody rb;
    private bool isDragging = false;      // K�ytt�j� k��nt�� objectia
    private bool controlDisabled;         // Onko ohjaus k�yt�ss�
    private Vector2 initialTouchPosition; // Kosketuksen alkuper�inen sijainti
    public float rotationSpeed = 20.0f;   // Kierron nopeus

    // ZOOM VARIABLES
    private float minZoom = 0.25f;        // Pienin mahdollinen zoom
    private float maxZoom = 2.0f;         // Suurin mahdollinen zoom
    private float initialPinchDistance;   // Alkuper�inen kosketuset�isyys
    private float initialScale;           // Alkuper�inen skaala

    // SHAKE VARIABLES
    private float shakeThreshold = 3.0f;  // Ravistuksen kynnysarvo
    private float rotationTime = 0.5f;    // Kierron kesto
    private Quaternion targetRotation = Quaternion.Euler(0, 180, 0); // Tavoiteasento

    // TEXTMESH VARIABLES
    public float enableDelayInSeconds = 1.0f;  // Viive sekunteina
    public float disableDelayInSeconds = 2.0f; // Viive sekunteina
    public TextMeshPro textMesh;               // Tekstikomponentti (TextMeshPro tai TMP_Text)

    // PREDICKTIONS
    public string[] predictionsEn = {
        "Hell Yeah",
        "It is certain",
        "It is decidedly so",
        "Without a doubt",
        "Yes definitely",
        "You may rely on it",
        "As I see it, yes",
        "Most likely",
        "Outlook good",
        "Yes",
        "Signs point to yes",
        "Reply hazy, try again",
        "Ask again later",
        "Better not tell you now",
        "Cannot predict now",
        "Concentrate and ask again",
        "Don't count on it",
        "My reply is no",
        "My sources say no",
        "Outlook not so good",
        "Very doubtful",
        "That's a yikes from me",
        "No way in hell"
    };

    public string[] predictionsFi = {
        "Se on varmaa",
        "Se on ehdottomasti niin",
        "Ei ep�ilyst�k��n",
        "Ehdottomasti kyll�",
        "Voit luottaa siihen",
        "Niin kuin min� sen n�en, kyll�",
        "Hyvin todenn�k�ist�",
        "N�kym�t ovat hyv�t",
        "Kyll�",
        "Merkit viittaavat my�nt�v�sti",
        "Vastaus on ep�selv�, kysy uudelleen",
        "Kysy my�hemmin uudelleen",
        "Parasta, etten kerro sit� juuri nyt",
        "En voi ennustaa nyt",
        "Keskity ja kysy uudelleen",
        "�l� luota siihen",
        "Vastaukseni on ei",
        "L�hteideni mukaan ei.",
        "N�kym�t eiv�t ole kovin hyv�t",
        "Se on Hyvin ep�todenn�k�ist�"
    };

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        textMesh.enabled = false; // Piilottaa tekstin (elementin) alussa
    }

    // Update is called once per frame
    void LateUpdate() // LateUpdate korjaa liikkeen glitsailun yms.
    {
        if (Input.touchCount == 0)
        {
            CheckShake();
        }
        else if (Input.touchCount == 1)
        {
            BallRotation();
        }
        else if (Input.touchCount == 2)
        {
            PinchZoom();
        }
    }

    private void BallRotation()
    {
        // Tarkistaa, onko vain yksi kosketus ja ohjaus ei ole poistettu k�yt�st�
        if (Input.touchCount == 1 && !controlDisabled)
        {
            // K�sitell��n yhden kosketuksen sy�tt�
            SingleTouchInput();
        }
        else
        {
            // Muissa tapauksissa
            isDragging = false;
        }
    }

    /*void SingleTouchInput()
    {
        // Hae ensimm�inen kosketus
        Touch touch = Input.GetTouch(0);

        // K�sittele kosketuksen vaiheet
        switch (touch.phase)
        {
            // Kosketus aloitettu
            case TouchPhase.Began:
                isDragging = true;
                rb.angularDrag = 10;
                initialTouchPosition = touch.position;
                break;

            // Kosketusta liikutetaan
            case TouchPhase.Moved:
                if (isDragging)
                {
                    // Hae nykyinen kosketuskohta
                    Vector2 currentTouchPosition = touch.position;

                    // Laske muutos alkuper�isest� kosketuskohdasta
                    Vector2 dragVector = currentTouchPosition - initialTouchPosition;

                    // Muuta py�rimissuunta muutosten perusteella
                    Vector3 rotationAxis = new Vector3(dragVector.y, -dragVector.x, 0).normalized;
                    float rotationAmount = dragVector.magnitude * rotationSpeed * Time.deltaTime * 10; // Lis�� nopeuskerroin

                    // Lis�� momenttia Rigidbodylle k��nt�miseen
                    rb.AddTorque(rotationAxis * rotationAmount);

                    initialTouchPosition = currentTouchPosition; // P�ivit� aloituskosketuspiste

                    rb.angularDrag = 4;
                }
                break;

            // Kosketus p��ttyy
            case TouchPhase.Ended:
                isDragging = false;
                rb.angularDrag = 1;
                break;
        }
    }*/

    void SingleTouchInput()
    {
        // Hae ensimm�inen kosketus
        Touch touch = Input.GetTouch(0);

        // K�sittele kosketuksen vaiheet
        switch (touch.phase)
        {
            // Kosketus aloitettu
            case TouchPhase.Began:
                isDragging = true;
                rb.angularDrag = 10;
                initialTouchPosition = touch.position;
                break;

            // Kosketusta liikutetaan
            case TouchPhase.Moved:
                if (isDragging)
                {
                    // Hae nykyinen kosketuskohta
                    Vector2 currentTouchPosition = touch.position;
                    // Laske vetovektori
                    Vector2 dragVector = currentTouchPosition - initialTouchPosition;

                    // Laske rotaatio x- ja y-akseleilla perustuen vetovektoriin
                    float rotationX = dragVector.y * rotationSpeed * Time.deltaTime;
                    float rotationY = -dragVector.x * rotationSpeed * Time.deltaTime;

                    // Lis�� momenttia Rigidbodylle k��nt�miseen
                    rb.AddTorque(Vector3.up * rotationY * rotationSpeed);
                    rb.AddTorque(Vector3.right * rotationX * rotationSpeed);

                    initialTouchPosition = currentTouchPosition; // P�ivit� aloituskosketuspiste

                    rb.angularDrag = 5;
                }
                break;

            // Kosketus p��ttyy
            case TouchPhase.Ended:
                isDragging = false;
                rb.angularDrag = 1;
                break;
        }
    }

    void PinchZoom()
    {
        // Tarkista, onko n�yt�ll� kaksi kosketusta
        if (Input.touchCount == 2)
        {
            // Hae tiedot kahdesta kosketuksesta
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Tarkista, onko jompikumpi kosketuksista juuri alkanut
            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                // Laske kahden kosketuksen v�linen alkuet�isyys
                initialPinchDistance = Vector2.Distance(touch0.position, touch1.position);

                // Tallenna kohteen alkuper�inen skaala
                initialScale = transform.localScale.x;
            }
            // Tarkista, liikkuuko jompikumpi kosketuksista juuri
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                // Laske kahden kosketuksen v�linen nykyinen et�isyys
                float currentPinchDistance = Vector2.Distance(touch0.position, touch1.position);

                // Laske skaalakerroin et�isyyden muutoksen perusteella
                float scaleFactor = currentPinchDistance / initialPinchDistance;

                // Laske uusi skaala arvo m��ritettyjen rajojen sis�ll�
                float newScale = Mathf.Clamp(initialScale * scaleFactor, minZoom, maxZoom);

                // Sovella uutta skaalaa kohteen muokkaamiseen
                transform.localScale = new Vector3(newScale, newScale, newScale);
            }
        }
    }

    // T�m� funktio tarkistaa laitteen t�rin�n/liikkeen ja k�ynnist�� funktion, jos kynnysarvo ylittyy.
    void CheckShake()
    {
        // Tarkista, onko laite kokenut riitt�v�n voimakasta liikett�
        if (Input.acceleration.sqrMagnitude >= shakeThreshold * shakeThreshold && !controlDisabled || Input.GetMouseButtonDown(1) && !controlDisabled)
        {
            
            StartCoroutine(RotateToTarget()); // K�ynnist�� funktion, joka k��nt�� pallon n�yt�n k�ytt�j�n suuntaan
            StartCoroutine(Prediction());     // K�ynnist�� ennustus funktion 
        }
    }

    // K��nt�� objektin n�yt�n oikeeseen suuntaan
    IEnumerator RotateToTarget()
    {
        // Asetetaan kontrolli pois p��lt�, jotta ohjaus ei h�iritse kiertoliikett�
        controlDisabled = true;

        // Pys�ytt�� fysiikan vaikutuksen palloon
        rb.isKinematic = true;

        // Tallennetaan aloitusaika
        float startTime = Time.time;

        // Tallennetaan alkuper�inen rotaatio
        Quaternion startRotation = transform.rotation;

        // K�yd��n l�pi liikeaika (rotationTime) ajan
        while (Time.time - startTime < rotationTime)
        {
            // Lasketaan, kuinka pitk�lle ollaan matkattu matkasta (0.0 - 1.0)
            float journeyLength = (Time.time - startTime) / rotationTime;

            // K�ytet��n Slerp-menetelm�� saavuttaaksemme tasaisen kiertoliikkeen
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, journeyLength);

            // Odota seuraava p�ivityskehys
            yield return null;

            // P�ivitet��n alkuper�inen rotaatio uusimmaksi rotaatioksi
            //initialRotation = transform.rotation;
        }
    }

    // Enumerator-funktio, joka k�sittelee ennustuksen
    IEnumerator Prediction()
    {
        // Valitsee satunnaisen indeksin ennustuksista
        int randomIndex = Random.Range(0, predictionsEn.Length);

        // Asetaa ennustuksen textMesh-tekstikomponenttiin
        textMesh.text = predictionsEn[randomIndex];

        // Viive ennen kuin kytketkee textMesh-komponentin p��lle
        yield return new WaitForSeconds(enableDelayInSeconds);
        textMesh.enabled = true;

        // Viive ennen kuin kytkee textMesh-komponentin pois p��lt�
        yield return new WaitForSeconds(disableDelayInSeconds);
        textMesh.enabled = false;

        // Viive ennen kuin kontolli palautuu k�ytt�j�lle
        //yield return new WaitForSeconds(enableDelayInSeconds / 2);
        controlDisabled = false;

        // Palauttaa fysiikan vaikutuksen palloon
        rb.isKinematic = false;
    }
}