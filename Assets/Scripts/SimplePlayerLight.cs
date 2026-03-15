using UnityEngine;

public class SimplePlayerLight : MonoBehaviour
{
    [Header("Light Settings")]
    public Light playerLight;
    public float lightRange = 60f;
    public float lightIntensity = 60f;
    
    [Header("Light Position")]
    public float heightAbovePlayer = 2f;      // Высота над игроком
    public float forwardOffset = 3f;          // Смещение вперед (положительное = вперед)
    public float sideOffset = 0f;             // Смещение вбок (положительное = вправо)
    public float smoothFollowSpeed = 5f;
    
    [Header("Follow Player Direction")]
    public bool followPlayerDirection = true; // Следить за направлением игрока
    public float rotationSmoothSpeed = 3f;    // Плавность поворота
    
    private Transform playerTransform;
    private Vector3 currentOffset;
    
    void Start()
    {
        // Автоматически находим игрока
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        // Если свет не назначен, ищем его на этом объекте
        if (playerLight == null)
        {
            playerLight = GetComponent<Light>();
        }
        
        // Настраиваем свет
        if (playerLight != null)
        {
            playerLight.range = lightRange;
            playerLight.intensity = lightIntensity;
            playerLight.color = new Color(1f, 0.9f, 0.7f); // Теплый желтый
            
            // Если это Spot Light, настраиваем направление
            if (playerLight.type == LightType.Spot)
            {
                playerLight.spotAngle = 60f; // Угол конуса
            }
        }
        
        // Инициализируем смещение
        currentOffset = new Vector3(sideOffset, heightAbovePlayer, forwardOffset);
    }
    
    void Update()
    {
        if (playerTransform != null)
        {
            // Вычисляем целевую позицию с учетом смещения
            Vector3 targetPosition = CalculateTargetPosition();
            
            // Плавное следование за игроком
            transform.position = Vector3.Lerp(
                transform.position, 
                targetPosition, 
                smoothFollowSpeed * Time.deltaTime
            );
            
            // Если нужно следить за направлением игрока
            if (followPlayerDirection)
            {
                UpdateLightDirection();
            }
        }
        
        // Простая анимация мерцания (опционально)
        if (playerLight != null && Random.value < 0.05f)
        {
            // Легкое мерцание для атмосферы
            playerLight.intensity = lightIntensity * Random.Range(0.95f, 1.05f);
        }
    }
    
    Vector3 CalculateTargetPosition()
    {
        // Базовая позиция над игроком
        Vector3 basePosition = playerTransform.position + Vector3.up * heightAbovePlayer;
        
        // Если следим за направлением, смещаем относительно направления игрока
        if (followPlayerDirection)
        {
            // Вперед по направлению движения игрока
            Vector3 forwardOffsetVec = playerTransform.forward * forwardOffset;
            // Вбок относительно игрока
            Vector3 sideOffsetVec = playerTransform.right * sideOffset;
            
            return basePosition + forwardOffsetVec + sideOffsetVec;
        }
        else
        {
            // Просто смещение по мировым осям
            return basePosition + new Vector3(sideOffset, 0, forwardOffset);
        }
    }
    
    void UpdateLightDirection()
    {
        if (playerLight == null) return;
        
        // Направляем свет вперед по движению игрока (немного вниз для освещения земли)
        Vector3 lookDirection = playerTransform.forward;
        lookDirection.y = -0.2f; // Немного вниз, чтобы освещать дорогу
        lookDirection.Normalize();
        
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        
        // Плавный поворот
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            rotationSmoothSpeed * Time.deltaTime
        );
    }
    
    // Метод для изменения параметров света из других скриптов
    public void SetLightRange(float newRange)
    {
        if (playerLight != null)
        {
            playerLight.range = newRange;
        }
    }
    
    public void SetLightIntensity(float newIntensity)
    {
        if (playerLight != null)
        {
            playerLight.intensity = newIntensity;
        }
    }
    
    // Метод для изменения смещения
    public void SetLightOffset(float newForwardOffset, float newSideOffset = 0f)
    {
        forwardOffset = newForwardOffset;
        sideOffset = newSideOffset;
        currentOffset = new Vector3(sideOffset, heightAbovePlayer, forwardOffset);
    }
    
    // Метод для изменения высоты
    public void SetLightHeight(float newHeight)
    {
        heightAbovePlayer = newHeight;
        currentOffset.y = newHeight;
    }
    
    // Визуализация в редакторе (только при выделенном объекте)
    void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            
            // Текущая позиция света
            Gizmos.DrawSphere(transform.position, 0.3f);
            
            // Линия от игрока к свету
            Gizmos.DrawLine(playerTransform.position + Vector3.up, transform.position);
            
            // Направление света (если это Spot)
            if (playerLight != null && playerLight.type == LightType.Spot)
            {
                Gizmos.color = Color.red;
                Vector3 lightForward = transform.forward * 5f;
                Gizmos.DrawRay(transform.position, lightForward);
            }
        }
    }
    
    // Для отладки в редакторе
    void OnValidate()
    {
        // Обновляем смещение при изменении в инспекторе
        currentOffset = new Vector3(sideOffset, heightAbovePlayer, forwardOffset);
    }
}