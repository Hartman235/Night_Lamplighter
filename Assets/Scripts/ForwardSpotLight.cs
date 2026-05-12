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
    public bool isBatteryActive = false; 
    private float normalRange;
    private float normalIntensity;
    private float normalSpotAngle;
    
    [Header("Battery Bonus Values")]
    public float batteryRange = 90f;
    public float batteryIntensity = 2000f;
    public float batterySpotAngle = 60f; 
    
    private Transform playerTransform;
    private bool isInitialized = false;
    
    void Start()
    {
        FindPlayer();
        InitializeLight();
        
        normalRange = lightRange;
        normalIntensity = lightIntensity;
        normalSpotAngle = spotAngle;
    }
    
    void InitializeLight()
    {
        if (playerLight == null)
        {
            playerLight = GetComponentInChildren<Light>();
            
            if (playerLight == null)
            {
                GameObject lightObj = new GameObject("PlayerSpotLight");
                lightObj.transform.SetParent(transform);
                playerLight = lightObj.AddComponent<Light>();
            }
        }
        
        playerLight.type = LightType.Spot;
        ApplyLightSettings();
        
        isInitialized = true;
    }
    
    void LateUpdate()
    {
        if (!isInitialized || playerLight == null || playerTransform == null)
            return;
            
        UpdateLightPosition();
        ApplyLightSettings(); 
        
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
        
        Vector3 lookDirection = playerTransform.forward;
        
        if (isBatteryActive)
        {
            Quaternion forwardRotation = Quaternion.LookRotation(playerTransform.forward);
        }
        else
        {
            lookDirection.y = -0.3f;
            playerLight.transform.rotation = Quaternion.LookRotation(lookDirection.normalized);
        }
    }
    
    public void SetLightRange(float newRange)
    {
        lightRange = newRange;
        if (playerLight != null)
            playerLight.range = newRange; 
    }
    
    public void SetSpotAngle(float newAngle)
    {
        spotAngle = newAngle;
        if (playerLight != null)
            playerLight.spotAngle = newAngle; 
    }
    
    public void SetLightIntensity(float newIntensity)
    {
        lightIntensity = newIntensity;
        if (playerLight != null)
            playerLight.intensity = newIntensity;
    }
    
    public void ActivateBatteryBonus(float duration)
    {
        if (isBatteryActive) return; 
        
        StopAllCoroutines(); 
        StartCoroutine(BatteryBonusCoroutine(duration));
    }
    
    private System.Collections.IEnumerator BatteryBonusCoroutine(float duration)
    {
        isBatteryActive = true;
        
        normalRange = lightRange;
        normalIntensity = lightIntensity;
        normalSpotAngle = spotAngle;
        
        lightRange = batteryRange;
        lightIntensity = batteryIntensity;
        spotAngle = batterySpotAngle;
        
        ApplyLightSettings();
        
        yield return new WaitForSeconds(duration);
        
        lightRange = normalRange;
        lightIntensity = normalIntensity;
        spotAngle = normalSpotAngle;
        
        ApplyLightSettings();
        
        isBatteryActive = false;
    }
        
    void OnValidate()
    {
        if (playerLight != null && Application.isPlaying == false)
        {
            ApplyLightSettings();
        }
    }
}