using UnityEngine;

public class ForwardSpotLight : MonoBehaviour
{
    [Header("Light Settings")]
    public Light playerLight;
    
    [Header("Dynamic Parameters")]
    public float lightRange = 15f;
    public float lightIntensity = 70f;
    public float spotAngle = 60f;
    
    [Header("Position")]
    public float heightAbovePlayer = 3f;
    public float forwardOffset = 2f;
    
    [Header("Battery Bonus Settings")]
    public bool isBatteryActive = false; // Флаг активности батарейки
    private float normalRange;
    private float normalIntensity;
    private float normalSpotAngle;
    
    [Header("Battery Bonus Values")]
    public float batteryRange = 90f;
    public float batteryIntensity = 2000f;
    public float batterySpotAngle = 60f; // Угол оставляем тот же или меняем если нужно
    
    private Transform playerTransform;
    private bool isInitialized = false;
    
    void Start()
    {
        FindPlayer();
        InitializeLight();
        
        // Сохраняем нормальные значения
        normalRange = lightRange;
        normalIntensity = lightIntensity;
        normalSpotAngle = spotAngle;
    }
    
    void InitializeLight()
    {
        if (playerLight == null)
        {
            // Ищем Light компонент на этом объекте или дочерних
            playerLight = GetComponentInChildren<Light>();
            
            if (playerLight == null)
            {
                // Создаем новый если не найден
                GameObject lightObj = new GameObject("PlayerSpotLight");
                lightObj.transform.SetParent(transform);
                playerLight = lightObj.AddComponent<Light>();
            }
        }
        
        // Устанавливаем начальные значения ПРЯМО в компонент
        playerLight.type = LightType.Spot;
        ApplyLightSettings();
        
        isInitialized = true;
    }
    
    // Важно! Используем LateUpdate для гарантии применения настроек
    void LateUpdate()
    {
        if (!isInitialized || playerLight == null || playerTransform == null)
            return;
            
        UpdateLightPosition();
        ApplyLightSettings(); // Всегда применяем настройки
        
        // Опционально: мерцание (только если батарейка не активна)
        if (!isBatteryActive && Random.value < 0.03f)
        {
            playerLight.intensity = lightIntensity * Random.Range(0.95f, 1.05f);
        }
    }
    
    void ApplyLightSettings()
    {
        if (playerLight == null) return;
        
        playerLight.range = lightRange;
        playerLight.intensity = lightIntensity;
        playerLight.spotAngle = spotAngle;
        playerLight.color = new Color(1f, 0.85f, 0.6f);
    }
    
    void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
    }
    
    void UpdateLightPosition()
    {
        Vector3 targetPosition = playerTransform.position + 
                                Vector3.up * heightAbovePlayer + 
                                playerTransform.forward * forwardOffset;
        
        transform.position = targetPosition;
        
        // Направляем свет
        Vector3 lookDirection = playerTransform.forward;
        
        // Если батарейка активна - применяем дополнительный наклон вниз
        if (isBatteryActive)
        {
            // Поворачиваем свет вниз на batteryTiltAngle градусов
            Quaternion forwardRotation = Quaternion.LookRotation(playerTransform.forward);
        }
        else
        {
            // Обычное направление - немного вниз
            lookDirection.y = -0.3f;
            playerLight.transform.rotation = Quaternion.LookRotation(lookDirection.normalized);
        }
    }
    
    // Методы для изменения параметров
    public void SetLightRange(float newRange)
    {
        lightRange = newRange;
        if (playerLight != null)
            playerLight.range = newRange; // Применяем сразу
    }
    
    public void SetSpotAngle(float newAngle)
    {
        spotAngle = newAngle;
        if (playerLight != null)
            playerLight.spotAngle = newAngle; // Применяем сразу
    }
    
    public void SetLightIntensity(float newIntensity)
    {
        lightIntensity = newIntensity;
        if (playerLight != null)
            playerLight.intensity = newIntensity; // Применяем сразу
    }
    
    // НОВЫЙ МЕТОД: Активация бонуса от батарейки
    public void ActivateBatteryBonus(float duration)
    {
        if (isBatteryActive) return; // Если уже активна - игнорируем
        
        StopAllCoroutines(); // Останавливаем предыдущие корутины
        StartCoroutine(BatteryBonusCoroutine(duration));
    }
    
    private System.Collections.IEnumerator BatteryBonusCoroutine(float duration)
    {
        // Активируем бонус
        isBatteryActive = true;
        
        // Сохраняем текущие значения
        normalRange = lightRange;
        normalIntensity = lightIntensity;
        normalSpotAngle = spotAngle;
        
        // Устанавливаем бонусные значения
        lightRange = batteryRange;
        lightIntensity = batteryIntensity;
        spotAngle = batterySpotAngle;
        
        // ПРИМЕНЯЕМ сразу
        ApplyLightSettings();
        
        // Ждем указанное время
        yield return new WaitForSeconds(duration);
        
        // Возвращаем обычные значения
        lightRange = normalRange;
        lightIntensity = normalIntensity;
        spotAngle = normalSpotAngle;
        
        // ПРИМЕНЯЕМ сразу
        ApplyLightSettings();
        
        isBatteryActive = false;
    }
        
    // Для Editor: синхронизируем значения при изменении в инспекторе
    void OnValidate()
    {
        if (playerLight != null && Application.isPlaying == false)
        {
            ApplyLightSettings();
        }
    }
}