using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic; 

public class SwipeController : MonoBehaviour
{
    public static bool tap, swipeLeft, swipeRight, swipeUp, swipeDown;
    public static bool wasSwipe = false;
    
    private bool isDraging = false;
    private bool isIgnoringTouch = false; 
    private Vector2 startTouch, swipeDelta;
    private float touchStartTime;
    
    [Header("Swipe Settings")]
    public float minSwipeDistance = 70f;      
    public float maxSwipeTime = 0.4f;          
    
    [Header("Double Tap Settings")]
    public float maxDoubleTapDistance = 20f;   
    public float maxDoubleTapTime = 0.3f;       
    
    private float lastTapTime = 0f;
    private Vector2 lastTapPosition = Vector2.zero;
    public static bool doubleTap = false;
    
    void Update()
    {
        tap = swipeDown = swipeUp = swipeLeft = swipeRight = false;
        doubleTap = false;
        
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI(Input.mousePosition))
                isIgnoringTouch = true;
            else
            {
                isIgnoringTouch = false;
                StartTouch(Input.mousePosition);
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!isIgnoringTouch) EndTouch(Input.mousePosition);
            isIgnoringTouch = false; 
        }
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            if (touch.phase == TouchPhase.Began)
            {
                if (IsPointerOverUI(touch.position))
                {
                    isIgnoringTouch = true;
                }
                else
                {
                    isIgnoringTouch = false;
                    StartTouch(touch.position);
                }
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                if (!isIgnoringTouch)
                {
                    EndTouch(touch.position);
                }
                isIgnoringTouch = false; 
            }
        }
        
        if (isDraging && !isIgnoringTouch)
        {
            Vector2 currentPos = GetCurrentPosition();
            swipeDelta = currentPos - startTouch;
            
            if (swipeDelta.magnitude > minSwipeDistance && 
                Time.time - touchStartTime < maxSwipeTime)
            {
                DetermineSwipeDirection();
                wasSwipe = true;
                Reset();
            }
        }
    }
    
    private bool IsPointerOverUI(Vector2 screenPosition)
    {
        if (EventSystem.current == null) return false;
        
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = screenPosition;
        
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        return results.Count > 0;
    }
    
    void StartTouch(Vector2 position)
    {
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
        
        if (touchDuration < maxSwipeTime && moveDistance < minSwipeDistance)
        {
            tap = true;
            float timeSinceLastTap = Time.time - lastTapTime;
            float distanceFromLastTap = Vector2.Distance(position, lastTapPosition);
            
            if (timeSinceLastTap < maxDoubleTapTime && 
                distanceFromLastTap < maxDoubleTapDistance)
            {
                doubleTap = true;
            }
            
            lastTapTime = Time.time;
            lastTapPosition = position;
        }
        
        wasSwipe = moveDistance >= minSwipeDistance;
        isDraging = false;
        Reset();
    }
    
    Vector2 GetCurrentPosition()
    {
        if (Input.touchCount > 0)
            return Input.GetTouch(0).position;
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