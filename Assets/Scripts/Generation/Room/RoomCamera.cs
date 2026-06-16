using UnityEngine;

public class RoomCamera : MonoBehaviour
{
    [Header("Настройки камеры")]
    [SerializeField] private float smoothSpeed = 10f;

    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }

    public void SetTargetPosition(Vector3 newPosition)
    {
        targetPosition = new Vector3(newPosition.x, newPosition.y, -10f);
    }

}
