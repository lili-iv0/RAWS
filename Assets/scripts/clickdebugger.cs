using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class clickdebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PointerEventData data = new PointerEventData(EventSystem.current);
            data.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(data, results);

            foreach (var result in results)
                Debug.Log($"Hit: {result.gameObject.name}");
        }
    }
}