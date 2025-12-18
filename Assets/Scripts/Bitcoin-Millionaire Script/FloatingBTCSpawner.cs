using UnityEngine;

public class FloatingBTCSpawner : MonoBehaviour
{
    public static FloatingBTCSpawner Instance;

    [Header("Assign in Inspector")]
    public Canvas canvas;                 // your main Canvas
    public RectTransform spawnParent;     // full-screen panel under Canvas (RectTransform)
    public GameObject floatingPrefab;     // prefab with FloatingBTC or FloatingBTC_NoCanvasGroup

    [Header("Options")]
    public bool debugLogs = false;

    void Awake()
    {
        Instance = this;
    }
    public void SpawnAtRandomPointer(double gain)
    {

        SpawnAtScreenRandomPosition(new Vector2(Random.Range(175f, 1000f),1500f),gain);
    
    }

    /// <summary>
    /// Spawns a floating BTC at current pointer (mouse or first touch) position.
    /// Call from CurrencyManager.OnClickMine()
    /// </summary>
    public void SpawnAtPointer(double gain)
    {
#if (UNITY_EDITOR || UNITY_STANDALONE) 
        SpawnAtScreenPosition(Input.mousePosition, gain);
#else
        if (Input.touchCount > 0)
            SpawnAtScreenPosition(Input.GetTouch(0).position, gain);
#endif
    }

    /// <summary>
    /// Spawns floating prefab at a given screen point (in pixels).
    /// Works for ScreenSpace-Overlay and ScreenSpace-Camera; also usable for WorldSpace via helper below.
    /// </summary>
    public void SpawnAtScreenPosition(Vector2 screenPos, double gain)
    {
        if (floatingPrefab == null || spawnParent == null || canvas == null) return;

        // Choose camera param depending on canvas mode
        Camera cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;
        // For ScreenSpaceOverlay, cam stays null.

        // Convert screen point to local point in the spawnParent rect
        RectTransformUtility.ScreenPointToLocalPointInRectangle(spawnParent, screenPos, cam, out Vector2 localPoint);

        // Instantiate and place
        GameObject go = Instantiate(floatingPrefab, spawnParent);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchoredPosition = localPoint;
        rt.SetAsLastSibling();

        // Set text/value on the floating script if present
        var txtComp = go.GetComponentInChildren<TMPro.TMP_Text>(true);
        if (txtComp != null)
            txtComp.text = $"+{gain:0.##}";

        if (debugLogs) Debug.Log($"SpawnAtScreenPosition: screen {screenPos}, local {localPoint}, canvasMode {canvas.renderMode}, cam {(cam ? cam.name : "null")}");
    }

    /// <summary>
    /// Convenience: spawn at a UI button's RectTransform center.
    /// Example: call SpawnAtUIElement(buttonRect, gain)
    /// </summary>
    public void SpawnAtUIElement(RectTransform uiElement, double gain)
    {
        if (uiElement == null) { SpawnAtPointer(gain); return; }
        // World -> Screen
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera, uiElement.position);
        SpawnAtScreenPosition(screenPoint, gain);
    }

    /// <summary>
    /// Convenience: spawn at a world-space world position (e.g., spawn from world object).
    /// </summary>
    public void SpawnAtWorldPosition(Vector3 worldPos, double gain)
    {
        Camera cam = canvas.renderMode == RenderMode.ScreenSpaceCamera ? canvas.worldCamera : null;
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, worldPos);
        SpawnAtScreenPosition(screenPoint, gain);
    }
    /// <summary>
    /// Spawns floating prefab at a given screen point (in pixels).
    /// Works for ScreenSpace-Overlay and ScreenSpace-Camera; also usable for WorldSpace via helper below.
    /// </summary>
    public void SpawnAtScreenRandomPosition(Vector2 screenPos, double gain)
    {
        if (floatingPrefab == null || spawnParent == null || canvas == null) return;
        Debug.Log(screenPos);
        // Choose camera param depending on canvas mode
        Camera cam = null;
        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
            cam = canvas.worldCamera;
        // For ScreenSpaceOverlay, cam stays null.

        // Convert screen point to local point in the spawnParent rect
        RectTransformUtility.ScreenPointToLocalPointInRectangle(spawnParent, screenPos, cam, out Vector2 localPoint);

        // Instantiate and place
        GameObject go = Instantiate(floatingPrefab, spawnParent);
        RectTransform rt = go.GetComponent<RectTransform>();
        go.GetComponent<FloatingBTC>().isRandom = true;
        rt.anchoredPosition = localPoint;
        rt.SetAsLastSibling();

        // Set text/value on the floating script if present
        var txtComp = go.GetComponentInChildren<TMPro.TMP_Text>(true);
        if (txtComp != null)
            txtComp.text = $"+{gain:0.##}";

        if (debugLogs) Debug.Log($"SpawnAtScreenPosition: screen {screenPos}, local {localPoint}, canvasMode {canvas.renderMode}, cam {(cam ? cam.name : "null")}");
    }
}
