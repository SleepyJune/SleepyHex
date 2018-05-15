using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputManager : MonoBehaviour
{
    public static TouchInputManager instance = null;

    public Dictionary<int, TouchInput> inputs;

    public delegate void Callback(Touch touch);
    public event Callback touchStart;
    public event Callback touchMove;
    public event Callback touchEnd;

    bool useMouse = true;

    Vector3 lastMousePosition;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (Application.isMobilePlatform)
        {
            useMouse = false;
        }

        if (!useMouse)
        {
            Input.simulateMouseWithTouches = false;
        }

        inputs = new Dictionary<int, TouchInput>();
    }

    void Update()
    {
        /*if (GameManager.instance.isGamePaused || GameManager.instance.isGameOver)
        {
            return;
        }*/

        if (useMouse && Input.mousePresent)
        {
            if (Input.GetMouseButtonDown(0))
            {                
                var touchData = new Touch();
                touchData.fingerId = -1;
                touchData.position = Input.mousePosition;
                touchData.phase = TouchPhase.Began;
                                
                lastMousePosition = Input.mousePosition;

                if (true)//!touchData.IsPointerOverUI() && !CheckClickableObjects(touchData))
                {
                    TouchInput newInput;
                    if (inputs.TryGetValue(touchData.fingerId, out newInput))
                    {
                        newInput.startPosition = touchData.position;
                    }
                    else
                    {
                        newInput = new TouchInput(touchData);
                        inputs.Add(newInput.id, newInput);
                    }

                    if (touchStart != null)
                    {
                        touchStart(touchData);
                    }
                }
            }

            if (Input.GetMouseButton(0))// && Input.mousePosition != lastMousePosition)
            {
                var touchData = new Touch();
                touchData.fingerId = -1;
                touchData.position = Input.mousePosition;
                touchData.phase = TouchPhase.Moved;
                                
                lastMousePosition = Input.mousePosition;

                if (UpdateTouchPosition(touchData.fingerId, touchData.position) && touchMove != null)
                {
                    touchMove(touchData);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                var touchData = new Touch();
                touchData.fingerId = -1;
                touchData.position = Input.mousePosition;
                touchData.phase = TouchPhase.Ended;
                                
                lastMousePosition = Input.mousePosition;

                if (UpdateTouchPosition(touchData.fingerId, touchData.position) && touchEnd != null)
                {
                    touchEnd(touchData);
                }

                inputs.Remove(touchData.fingerId);
            }
        }

        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    if (true) //!touch.IsPointerOverUI() && !CheckClickableObjects(touch))
                    {
                        TouchInput newInput;
                        if (inputs.TryGetValue(touch.fingerId, out newInput))
                        {
                            newInput.startPosition = touch.position;
                        }
                        else
                        {
                            newInput = new TouchInput(touch);
                            inputs.Add(newInput.id, newInput);
                        }

                        if (touchStart != null)
                        {
                            touchStart(touch);
                        }
                    }
                }
                                
                if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
                {
                    if (UpdateTouchPosition(touch.fingerId, touch.position) && touchMove != null)
                    {
                        touchMove(touch);
                    }
                }

                if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                {                    
                    if (UpdateTouchPosition(touch.fingerId, touch.position) && touchEnd != null)
                    {
                        touchEnd(touch);
                    }

                    inputs.Remove(touch.fingerId);
                }
            }
        }
    }

    bool UpdateTouchPosition(int fingerId, Vector2 position)
    {
        TouchInput newInput;
        if (inputs.TryGetValue(fingerId, out newInput))
        {
            newInput.previousPosition = newInput.position;
            newInput.position = position;

            return true;
        }

        return false;
    }
}
