using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Vector3 offset;
    void Start()
    {
        offset = transform.position - player.position;
    }

    void FixedUpdate()
    {
        Vector3 newPosition = new Vector3(
            offset.x + player.position.x / 2,  
            transform.position.y,
            offset.z + player.position.z
        );
        
        transform.position = newPosition;
    }
}
