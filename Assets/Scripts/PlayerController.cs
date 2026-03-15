using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private StatisticsManager statsManager;
    private CharacterController controller;
    private Vector3 dir;
    private int lineToMove = 1;
    public float lineDistance = 4;
    private Animator anim;
    private Score score;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float gravity;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private int coins;
    [SerializeField] private Text coinsText;
    [SerializeField] private float coinMagnetRadius = 1.5f;
    [SerializeField] private LayerMask coinLayer;
    [SerializeField] private LayerMask batteryLayer;
    [SerializeField] private Score scoreScript;
    
    // Поля для батареек
    [SerializeField] private int batteriesCollected;
    [SerializeField] private Text batteriesText;
    [SerializeField] private ForwardSpotLight playerSpotLight;
    
    private const float maxSpeed = 150;
    private bool isSliding = false;
    private bool isImmortal;
    
    // Для двойного тапа
    private float lastTapTime = 0f;
    private float doubleTapThreshold = 0.3f;
    
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        if (anim == null)
            Debug.LogWarning("Animator not found in children!");

        // Подписываемся на событие смены скина
        SkinLoader.OnSkinChanged += UpdateAnimatorReference;

        controller = GetComponent<CharacterController>();
        if (controller == null)
            Debug.LogError("CharacterController not found!");

        score = scoreText?.GetComponent<Score>();
        if (score == null)
            Debug.LogWarning("Score component not found!");

        StartCoroutine(SpeedIncrease());
        Time.timeScale = 1;

        if (score != null)
            score.scoreMultiplier = 1;

        coins = PlayerPrefs.GetInt("coins");
        if (coinsText != null)
            coinsText.text = coins.ToString();
        else
            Debug.LogWarning("coinsText is not assigned!");

        isImmortal = false;
        
        batteriesCollected = 0; 
        if (batteriesText != null)
        {
            batteriesText.text = "0";
        }
        else
        {
            Debug.LogWarning("batteriesText is not assigned!");
        }
        
        if (playerSpotLight == null)
        {
            playerSpotLight = FindFirstObjectByType<ForwardSpotLight>();
            if (playerSpotLight == null)
                Debug.LogWarning("ForwardSpotLight not found in scene!");
        }

        statsManager = FindFirstObjectByType<StatisticsManager>();
        if (statsManager != null)
        {
            statsManager.StartNewGame();
        }
    }

    void OnDestroy()
    {
        // Важно отписаться от события, чтобы избежать утечек памяти
        SkinLoader.OnSkinChanged -= UpdateAnimatorReference;
    }

    void Update()
    {
        if (statsManager != null)
        {
            statsManager.UpdateDistance(transform.position.z);
        }
        if (SwipeController.doubleTap)
        {
            TryActivateBattery();
        }
        // Управление движением
        if (SwipeController.swipeRight)
        {
            if (lineToMove < 2) lineToMove++;
        }
        if (SwipeController.swipeLeft)
        {
            if (lineToMove > 0) lineToMove--;
        }
        if (SwipeController.swipeUp && controller != null && controller.isGrounded)
        {
            Jump();
        }

        // Свайп вниз = подкат
        if (SwipeController.swipeDown)
        {
            StartCoroutine(Slide());
        }
        
        // ТАП - проверяем двойной тап
        if (SwipeController.tap && !SwipeController.wasSwipe)
        {
            CheckForDoubleTap();
        }

        // Анимация
        if (anim != null && controller != null)
        {
            anim.SetBool("Running", controller.isGrounded && !isSliding);
        }

        // Движение по линиям
        if (controller != null)
        {
            Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up;
            if (lineToMove == 0)
            {
                targetPosition += Vector3.left * lineDistance;
            }
            else if (lineToMove == 2)
            {
                targetPosition += Vector3.right * lineDistance;
            }

            CoinMagnet();
            BatteryMagnet();

            if (transform.position != targetPosition)
            {
                Vector3 diff = targetPosition - transform.position;
                Vector3 moveDir = diff.normalized * 30 * Time.deltaTime;
                controller.Move(moveDir.sqrMagnitude < diff.sqrMagnitude ? moveDir : diff);
            }
        }

        if (statsManager != null)
        {
            statsManager.UpdateDistance(transform.position.z);
        }
    }

    void FixedUpdate()
    {
        if (controller != null)
        {
            dir.z = speed;
            dir.y = controller.isGrounded ? (dir.y < 0 ? -1f : dir.y) : dir.y + gravity * Time.fixedDeltaTime;
            controller.Move(dir * Time.fixedDeltaTime);
        }
    }

    // Проверка двойного тапа
    private void CheckForDoubleTap()
    {
        float currentTime = Time.time;
        
        if (currentTime - lastTapTime <= doubleTapThreshold)
        {
            TryActivateBattery();
            lastTapTime = 0f;
        }
        else
        {
            lastTapTime = currentTime; 
        }
    }

    private void Jump()
    {
        dir.y = jumpForce;
        if (anim != null)
        {
            anim.SetTrigger("Jump");
        }
        else
        {
            Debug.LogWarning("Animator is null in Jump!");
        }
    }

    private IEnumerator Slide()
    {
        if (isSliding) yield break;
        
        isSliding = true;

        if (anim != null)
        {
            anim.SetTrigger("Slide");
        }
        else
        {
            Debug.LogWarning("Animator is null in Slide!");
        }

        if (controller != null)
        {
            controller.height = 1.35f;
            controller.center = new Vector3(0, -1.42f, 0);
        }

        yield return new WaitForSeconds(0.67f);

        if (controller != null)
        {
            controller.height = 3f;
            controller.center = new Vector3(0, -0.6f, 0);
        }
        
        isSliding = false;
    }

    // Сбор монет и батареек
    private void CollectCoin(GameObject coin)
    {
        if (coin == null) return;

        coins++;
        PlayerPrefs.SetInt("coins", coins);
        if (coinsText != null)
            coinsText.text = coins.ToString();
        statsManager?.AddCoin();
        Destroy(coin);
    }
    
    private void CollectBattery(GameObject battery)
    {
        if (battery == null) return;

        batteriesCollected++;
        if (batteriesText != null)
            batteriesText.text = batteriesCollected.ToString();
        statsManager?.AddBattery();
        Destroy(battery);
    }

    void CoinMagnet() 
    {
        if (controller == null) return;

        foreach (Collider coin in Physics.OverlapSphere(transform.position, coinMagnetRadius, coinLayer))
            CollectCoin(coin.gameObject);
    }
    
    void BatteryMagnet()
    {
        if (controller == null) return;

        foreach (Collider battery in Physics.OverlapSphere(transform.position, coinMagnetRadius, batteryLayer))
            CollectBattery(battery.gameObject);
    }
    
    // Активация батарейки
    private void TryActivateBattery()
    {
        if (batteriesCollected <= 0) return;
        if (playerSpotLight != null && playerSpotLight.isBatteryActive) return;
        
        StartCoroutine(ActivateBattery());
    }
    
    IEnumerator ActivateBattery()
    {
        batteriesCollected--;
        if (batteriesText != null)
        {
            batteriesText.text = batteriesCollected.ToString();
        }
        else
        {
            Debug.LogWarning("batteriesText is null in ActivateBattery!");
        }
        
        if (playerSpotLight != null)
        {
            playerSpotLight.ActivateBatteryBonus(20f);
        }
        else
        {
            Debug.LogWarning("playerSpotLight is null in ActivateBattery! Battery consumed but no effect.");
        }
        
        yield return null;
    }

    void OnControllerColliderHit(ControllerColliderHit hit) 
    {
        if (hit.gameObject.tag == "obstacle")
        {
            if (isImmortal)
            {
                Destroy(hit.gameObject);
            }
            else
            {
                // Получаем текущий счет
                int currentScore = 0;
                if (scoreScript != null && scoreScript.scoreText != null)
                {
                    int.TryParse(scoreScript.scoreText.text, out currentScore);
                }
                
                currentScore++; // Увеличиваем на 1 (как в оригинале)
                PlayerPrefs.SetInt("lastRunScore", currentScore);
                
                // Завершаем игру в статистике
                statsManager?.EndGame(currentScore, transform.position.z);
                
                GameOverManager gameOverManager = FindAnyObjectByType<GameOverManager>();
                if (gameOverManager != null)
                {
                    gameOverManager.ProcessGameOver();
                }
                else
                {
                    Debug.LogWarning("GameOverManager not found! Showing lose panel directly.");
                    if (losePanel != null)
                        losePanel.SetActive(true);
                }
                
                Time.timeScale = 0;
            }    
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null || other.gameObject == null) return;

        if (other.gameObject.tag == "BonusStar")
        {
            StartCoroutine(StarBonus());
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "BonusShield")
        {
            StartCoroutine(ShieldBonus());
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Battery")
        {
            CollectBattery(other.gameObject);
        }
    }

    private IEnumerator SpeedIncrease()
    {
        yield return new WaitForSeconds(6);
        if (speed < maxSpeed)
        {
            speed += 1;
            StartCoroutine(SpeedIncrease());
        }
    }

    private IEnumerator StarBonus()
    {
        if (score != null)
        {
            score.scoreMultiplier = 2;
            yield return new WaitForSeconds(20);
            score.scoreMultiplier = 1;
        }
        else
        {
            yield break;
        }
    }

    private IEnumerator ShieldBonus()
    {
        isImmortal = true;
        yield return new WaitForSeconds(10);
        isImmortal = false;
    }
    
    public int GetBatteriesCollected() => batteriesCollected;
    
    public void ResetBatteries()
    {
        batteriesCollected = 0;
        if (batteriesText != null)
            batteriesText.text = "0";
    }

    public float GetCurrentSpeed()
    {
        return speed;
    }

    void UpdateAnimatorReference()
    {
        // Ищем новый аниматор среди детей
        anim = GetComponentInChildren<Animator>();
        
        if (anim != null)
        {
            Debug.Log($"Animator reference updated. New animator: {anim.gameObject.name}");
        }
        else
        {
            Debug.LogWarning("Animator not found after skin change!");
        }
    }
}