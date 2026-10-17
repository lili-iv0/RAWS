using UnityEngine;

public static class InputHelper
{
    private static Camera mainCam;

    public static bool GetInputDown(out Vector2 screenPos)
    {
        if (mainCam == null) mainCam = Camera.main;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        { screenPos = Input.GetTouch(0).position; return true; }
        if (Input.GetMouseButtonDown(0))
        { screenPos = Input.mousePosition; return true; }
        screenPos = Vector2.zero; return false;
    }

    public static bool GetInputHeld(out Vector2 screenPos)
    {
        if (mainCam == null) mainCam = Camera.main;
        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Moved || t.phase == TouchPhase.Stationary)
            { screenPos = t.position; return true; }
        }
        if (Input.GetMouseButton(0))
        { screenPos = Input.mousePosition; return true; }
        screenPos = Vector2.zero; return false;
    }

    public static bool GetInputUp(out Vector2 screenPos)
    {
        if (mainCam == null) mainCam = Camera.main;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        { screenPos = Input.GetTouch(0).position; return true; }
        if (Input.GetMouseButtonUp(0))
        { screenPos = Input.mousePosition; return true; }
        screenPos = Vector2.zero; return false;
    }

    public static Vector3 ScreenToWorld(Vector2 screenPos)
    {
        if (mainCam == null) mainCam = Camera.main;
        Vector3 pos = mainCam.ScreenToWorldPoint(screenPos);
        pos.z = 0f;
        return pos;
    }
}