using UnityEngine;

public class Rotation : MonoBehaviour
{
    private bool isDragging = false;
    private Vector2 initialTouchPosition;
    private Quaternion initialRotation;
    public float rotationSpeed = 1.0f;

    private Vector3 originalRotation;
    private float initialDistance;

    void Start()
    {
        //initialRotation = transform.rotation;
        //originalRotation = transform.eulerAngles;
    }

    void Update()
    {
        BallRotation();
    }

    void BallRotation()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    isDragging = true;
                    initialTouchPosition = touch.position;
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        Vector2 currentTouchPosition = touch.position;
                        Vector2 dragVector = currentTouchPosition - initialTouchPosition;

                        // K‰‰nn‰ palloa suhteessa dragVectorin liikkeeseen
                        float rotationX = dragVector.y * rotationSpeed * Time.deltaTime;
                        float rotationY = -dragVector.x * rotationSpeed * Time.deltaTime;

                        Quaternion xRotation = Quaternion.AngleAxis(rotationX, Vector3.right);
                        Quaternion yRotation = Quaternion.AngleAxis(rotationY, Vector3.up);

                        transform.rotation = initialRotation * xRotation * yRotation;
                    }
                    break;

                case TouchPhase.Ended:
                    isDragging = false;
                    initialRotation = transform.rotation; // Muista uusi asento
                    break;
            }
        }
        else
        {
            isDragging = false;
        }
    }
}

