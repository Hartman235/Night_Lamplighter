using UnityEngine;
using UnityEngine.EventSystems; // добавлено для проверки UI

public class SwipeController : MonoBehaviour
{
    public static bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    public static bool wasSwipe = false;
    
    private bool isDraging = false;
    private Vector2 startTouch, swipeDelta, endTouch;
    private float touchStartTime;
    
    [Header("Swipe Settings")]
    public float minSwipeDistance = 70f;      // Минимальная дистанция для свайпа
    public float maxSwipeTime = 0.4f;          // Максимальное время для свайпа
    
    [Header("Double Tap Settings")]
    public float maxDoubleTapDistance = 20f;   // Максимальное расстояние для двойного тапа
    public float maxDoubleTapTime = 0.3f;       // Максимальное время между тапами
    
    private float lastTapTime = 0f;
    private Vector2 lastTapPosition = Vector2.zero;
    public static bool doubleTap = false;
    
    void Update()
    {
        // Сбрасываем флаги
        tap = swipeDown = swipeUp = swipeLeft = swipeRight = false;
        doubleTap = false;
        
        // Обработка мыши (ПК)
        if (Input.GetMouseButtonDown(0))
        {
            // Игнорируем, если касание по UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
            StartTouch(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndTouch(Input.mousePosition);
        }
        
        // Обработка тачей (мобильные)
        if (Input.touches.Length > 0)
        {
            Touch touch = Input.touches[0];
            // Игнорируем, если касание по UI
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return;
            
            if (touch.phase == TouchPhase.Began)
            {
                StartTouch(touch.position);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                EndTouch(touch.position);
            }
        }
        
        // Обработка свайпа во время движения
        if (isDraging)
        {
            Vector2 currentPos = GetCurrentPosition();
            swipeDelta = currentPos - startTouch;
            
            // Проверяем свайп
            if (swipeDelta.magnitude > minSwipeDistance && 
                Time.time - touchStartTime < maxSwipeTime)
            {
                DetermineSwipeDirection();
                wasSwipe = true;
                Reset();
            }
        }
    }
    
    void StartTouch(Vector2 position)
    {
        // Дополнительная страховка – если вдруг не сработало выше
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        
        isDraging = true;
        startTouch = position;
        touchStartTime = Time.time;
        wasSwipe = false;
    }
    
    void EndTouch(Vector2 position)
    {
        if (!isDraging) return;
        
        float touchDuration = Time.time - touchStartTime;
        float moveDistance = Vector2.Distance(startTouch, position);
        
        // Если это короткое касание с маленьким движением - это тап
        if (touchDuration < maxSwipeTime && moveDistance < minSwipeDistance)
        {
            tap = true;
            
            // Проверяем двойной тап
            float timeSinceLastTap = Time.time - lastTapTime;
            float distanceFromLastTap = Vector2.Distance(position, lastTapPosition);
            
            if (timeSinceLastTap < maxDoubleTapTime && 
                distanceFromLastTap < maxDoubleTapDistance)
            {
                doubleTap = true;
            }
            
            // Запоминаем этот тап для следующей проверки
            lastTapTime = Time.time;
            lastTapPosition = position;
        }
        
        wasSwipe = moveDistance >= minSwipeDistance;
        isDraging = false;
        Reset();
    }
    
    Vector2 GetCurrentPosition()
    {
        if (Input.touches.Length > 0)
            return Input.touches[0].position;
        return Input.mousePosition;
    }
    
    void DetermineSwipeDirection()
    {
        float x = swipeDelta.x;
        float y = swipeDelta.y;
        
        if (Mathf.Abs(x) > Mathf.Abs(y))
        {
            if (x < 0) swipeLeft = true;
            else swipeRight = true;
        }
        else
        {
            if (y < 0) swipeDown = true;
            else swipeUp = true;
        }
    }
    
    void Reset()
    {
        startTouch = swipeDelta = Vector2.zero;
        isDraging = false;
    }
}