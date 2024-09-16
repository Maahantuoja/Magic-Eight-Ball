using UnityEngine;

public class Zoom : MonoBehaviour
{
    private float minZoom = 0.25f;  // Pienin mahdollinen zoom
    private float maxZoom = 2.0f;   // Suurin mahdollinen zoom

    //private Vector2 touchStartPos;
    private float initialPinchDistance;
    private float initialScale;

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            // Kosketusn�yt�n kahden sormen zoom
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Began || touch1.phase == TouchPhase.Began)
            {
                // Tallenna kosketuksen aloituspiste ja alkuper�inen et�isyys
                //touchStartPos = (touch0.position + touch1.position) / 2;
                initialPinchDistance = Vector2.Distance(touch0.position, touch1.position);
                initialScale = transform.localScale.x;
            }
            else if (touch0.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Moved)
            {
                // Laske zoom-tekij� ja rajoita se minZoom ja maxZoom v�lille
                float currentPinchDistance = Vector2.Distance(touch0.position, touch1.position);
                float scaleFactor = currentPinchDistance / initialPinchDistance;
                float newScale = Mathf.Clamp(initialScale * scaleFactor, minZoom, maxZoom);

                // P�ivit� objektin skaala
                transform.localScale = new Vector3(newScale, newScale, newScale);
            }
        }
        else if (Input.touchCount == 1)
        {
            // Yksitt�inen kosketus, voit lis�t� objektin py�ritt�misen tai siirt�misen t�h�n
        }
    }
}
