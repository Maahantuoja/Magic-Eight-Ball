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
    private bool isDragging = false;      // Käyttäjä kääntää objectia
    private bool controlDisabled;         // Onko ohjaus käytössä
    private Vector2 initialTouchPosition; // Kosketuksen alkuperäinen sijainti
    public float rotationSpeed = 20.0f;   // Kierron nopeus

    // ZOOM VARIABLES
    private float minZoom = 0.25f;        // Pienin mahdollinen zoom
    private float maxZoom = 2.0f;         // Suurin mahdollinen zoom
    private float initialPinchDistance;   // Alkuperäinen kosketusetäisyys
    private float initialScale;           // Alkuperäinen skaala

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
        "Ei epäilystäkään",
        "Ehdottomasti kyllä",
        "Voit luottaa siihen",
        "Niin kuin minä sen näen, kyllä",
        "Hyvin todennäköistä",
        "Näkymät ovat hyvät",
        "Kyllä",
        "Merkit viittaavat myöntävästi",
        "Vastaus on epäselvä, kysy uudelleen",
        "Kysy myöhemmin uudelleen",
        "Parasta, etten kerro sitä juuri nyt",
        "En voi ennustaa nyt",
        "Keskity ja kysy uudelleen",
        "Älä luota siihen",
        "Vastaukseni on ei",
        "Lähteideni mukaan ei.",
        "Näkymät eivät ole kovin hyvät",
        "Se on Hyvin epätodennäköistä"
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
        // Tarkistaa, onko vain yksi kosketus ja ohjaus ei ole poistettu käytöstä
        if (Input.touchCount == 1 && !controlDisabled)
        {
            // Käsitellään yhden kosketuksen syöttö
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
        // Hae ensimmäinen kosketus
        Touch touch = Input.GetTouch(0);

        // Käsittele kosketuksen vaiheet
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

                    // Laske muutos alkuperäisestä kosketuskohdasta
                    Vector2 dragVector = currentTouchPosition - initialTouchPosition;

                    // Muuta pyörimissuunta muutosten perusteella
                    Vector3 rotationAxis = new Vector3(dragVector.y, -dragVector.x, 0).normalized;
                    float rotationAmount = dragVector.magnitude * rotationSpeed * Time.deltaTime * 10; // Lisää nopeuskerroin

                    // Lisää momenttia Rigidbodylle kääntämiseen
                    rb.AddTorque(rotationAxis * rotationAmount);

                    initialTouchPosition = currentTouchPosition; // Päivitä aloituskosketuspiste

                    rb.angularDrag = 4;
                }
                break;

            // Kosketus päättyy
            case TouchPhase.Ended:
                isDragging = false;
                rb.angularDrag = 1;
                break;
        }
    }*/

    void SingleTouchInput()
    {
        // Hae ensimmäinen kosketus
        Touch touch = Input.GetTouch(0);

        // Käsittele kosketuksen vaiheet
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

                    // Lisää momenttia Rigidbodylle kääntämiseen
                    rb.AddTorque(Vector3.up * rotationY * rotationSpeed);
                    rb.AddTorque(Vector3.right * rotationX * rotationSpeed);

                    initialTouchPosition = currentTouchPosition; // Päivitä aloituskosketuspiste

                    rb.angularDrag = 5;
                }
                break;

            // Kosketus päättyy
            case TouchPhase.Ended:
                isDragging = false;
                rb.angularDrag = 1;
                break;
        }
    }

    void PinchZoom()
    {
        // Tarkista, onko näytöllä kaksi kosketusta
        if (Input.touchCount == 2)
        {
            // Hae tiedot kahdesta kosketuksesta
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            // Tarkista, onko jompikumpi kosketuksista juuri alkanut
            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                // Laske kahden kosketuksen välinen alkuetäisyys
                initialPinchDistance = Vector2.Distance(touch0.position, touch1.position);

                // Tallenna kohteen alkuperäinen skaala
                initialScale = transform.localScale.x;
            }
            // Tarkista, liikkuuko jompikumpi kosketuksista juuri
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                // Laske kahden kosketuksen välinen nykyinen etäisyys
                float currentPinchDistance = Vector2.Distance(touch0.position, touch1.position);

                // Laske skaalakerroin etäisyyden muutoksen perusteella
                float scaleFactor = currentPinchDistance / initialPinchDistance;

                // Laske uusi skaala arvo määritettyjen rajojen sisällä
                float newScale = Mathf.Clamp(initialScale * scaleFactor, minZoom, maxZoom);

                // Sovella uutta skaalaa kohteen muokkaamiseen
                transform.localScale = new Vector3(newScale, newScale, newScale);
            }
        }
    }

    // Tämä funktio tarkistaa laitteen tärinän/liikkeen ja käynnistää funktion, jos kynnysarvo ylittyy.
    void CheckShake()
    {
        // Tarkista, onko laite kokenut riittävän voimakasta liikettä
        if (Input.acceleration.sqrMagnitude >= shakeThreshold * shakeThreshold && !controlDisabled || Input.GetMouseButtonDown(1) && !controlDisabled)
        {
            
            StartCoroutine(RotateToTarget()); // Käynnistää funktion, joka kääntää pallon näytön käyttäjän suuntaan
            StartCoroutine(Prediction());     // Käynnistää ennustus funktion 
        }
    }

    // Kääntää objektin näytön oikeeseen suuntaan
    IEnumerator RotateToTarget()
    {
        // Asetetaan kontrolli pois päältä, jotta ohjaus ei häiritse kiertoliikettä
        controlDisabled = true;

        // Pysäyttää fysiikan vaikutuksen palloon
        rb.isKinematic = true;

        // Tallennetaan aloitusaika
        float startTime = Time.time;

        // Tallennetaan alkuperäinen rotaatio
        Quaternion startRotation = transform.rotation;

        // Käydään läpi liikeaika (rotationTime) ajan
        while (Time.time - startTime < rotationTime)
        {
            // Lasketaan, kuinka pitkälle ollaan matkattu matkasta (0.0 - 1.0)
            float journeyLength = (Time.time - startTime) / rotationTime;

            // Käytetään Slerp-menetelmää saavuttaaksemme tasaisen kiertoliikkeen
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, journeyLength);

            // Odota seuraava päivityskehys
            yield return null;

            // Päivitetään alkuperäinen rotaatio uusimmaksi rotaatioksi
            //initialRotation = transform.rotation;
        }
    }

    // Enumerator-funktio, joka käsittelee ennustuksen
    IEnumerator Prediction()
    {
        // Valitsee satunnaisen indeksin ennustuksista
        int randomIndex = Random.Range(0, predictionsEn.Length);

        // Asetaa ennustuksen textMesh-tekstikomponenttiin
        textMesh.text = predictionsEn[randomIndex];

        // Viive ennen kuin kytketkee textMesh-komponentin päälle
        yield return new WaitForSeconds(enableDelayInSeconds);
        textMesh.enabled = true;

        // Viive ennen kuin kytkee textMesh-komponentin pois päältä
        yield return new WaitForSeconds(disableDelayInSeconds);
        textMesh.enabled = false;

        // Viive ennen kuin kontolli palautuu käyttäjälle
        //yield return new WaitForSeconds(enableDelayInSeconds / 2);
        controlDisabled = false;

        // Palauttaa fysiikan vaikutuksen palloon
        rb.isKinematic = false;
    }
}