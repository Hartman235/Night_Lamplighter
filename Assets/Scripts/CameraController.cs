using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Vector3 offset;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.position - player.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Новая позиция: X = половина от позиции игрока, Z = как раньше
        Vector3 newPosition = new Vector3(
            offset.x + player.position.x / 2,  // Половина смещения игрока
            transform.position.y,
            offset.z + player.position.z
        );
        
        transform.position = newPosition;
    }
}
