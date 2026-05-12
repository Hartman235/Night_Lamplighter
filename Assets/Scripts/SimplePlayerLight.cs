using UnityEngine;

public class SimplePlayerLight : MonoBehaviour
{
    [Header("Light Settings")]
    public Light playerLight;
    public float lightRange = 60f;
    public float lightIntensity = 60f;
    
    [Header("Light Position")]
    public float heightAbovePlayer = 2f;      
    public float forwardOffset = 3f;          
    public float sideOffset = 0f;             
    public float smoothFollowSpeed = 5f;
    
    [Header("Follow Player Direction")]
    public bool followPlayerDirection = true; 
    public float rotationSmoothSpeed = 3f;    
    
    private Transform playerTransform;
    private Vector3 currentOffset;
    
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        if (playerLight == null)
        {
            playerLight = GetComponent<Light>();
        }
        
        if (playerLight != null)
        {
            playerLight.range = lightRange;
            playerLight.intensity = lightIntensity;
            playerLight.color = new Color(1f, 0.9f, 0.7f); 
            
            if (playerLight.type == LightType.Spot)
            {
                playerLight.spotAngle = 60f; 
            }
        }
        
        currentOffset = new Vector3(sideOffset, heightAbovePlayer, forwardOffset);
    }
    
    void Update()
    {
        if (playerTransform != null)
        {
            Vector3 targetPosition = CalculateTargetPosition();
            
            transform.position = Vector3.Lerp(
                transform.position, 
                targetPosition, 
                smoothFollowSpeed * Time.deltaTime
            );
            
            if (followPlayerDirection)
            {
                UpdateLightDirection();
            }
        }
        
        if (playerLight != null && Random.value < 0.05f)
        {
            playerLight.intensity = lightIntensity * Random.Range(0.95f, 1.05f);
        }
    }
    
    Vector3 CalculateTargetPosition()
    {
        Vector3 basePosition = playerTransform.position + Vector3.up * heightAbovePlayer;
        
        if (followPlayerDirection)
        {
            Vector3 forwardOffsetVec = playerTransform.forward * forwardOffset;
            Vector3 sideOffsetVec = playerTransform.right * sideOffset;
            
            return basePosition + forwardOffsetVec + sideOffsetVec;
        }
        else
        {
            return basePosition + new Vector3(sideOffset, 0, forwardOffset);
        }
    }
    
    void UpdateLightDirection()
    {
        if (playerLight == null) return;
        
        Vector3 lookDirection = playerTransform.forward;
        lookDirection.y = -0.2f; 
        lookDirection.Normalize();
        
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
        
        transform.rotation = Quaternion.Slerp(
            transform.rotation, 
            targetRotation, 
            rotationSmoothSpeed * Time.deltaTime
        );
    }
    
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
    
    public void SetLightOffset(float newForwardOffset, float newSideOffset = 0f)
    {
        forwardOffset = newForwardOffset;
        sideOffset = newSideOffset;
        currentOffset = new Vector3(sideOffset, heightAbovePlayer, forwardOffset);
    }
    
    public void SetLightHeight(float newHeight)
    {
        heightAbovePlayer = newHeight;
        currentOffset.y = newHeight;
    }
    
    void OnDrawGizmosSelected()
    {
        if (playerTransform != null)
        {
            Gizmos.color = Color.yellow;
            
            Gizmos.DrawSphere(transform.position, 0.3f);
            
            Gizmos.DrawLine(playerTransform.position + Vector3.up, transform.position);
            
            if (playerLight != null && playerLight.type == LightType.Spot)
            {
                Gizmos.color = Color.red;
                Vector3 lightForward = transform.forward * 5f;
                Gizmos.DrawRay(transform.position, lightForward);
            }
        }
    }
    
    void OnValidate()
    {
        currentOffset = new Vector3(sideOffset, heightAbovePlayer, forwardOffset);
    }
}