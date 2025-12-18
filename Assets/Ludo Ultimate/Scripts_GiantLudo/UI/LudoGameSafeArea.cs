using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LudoGameSafeArea : MonoBehaviour
{
    //	public RectTransform forEditorIphoneXImage;
    public RectTransform Panel;
    Rect LastSafeArea = new Rect(0, 0, 0, 0);

    void OnEnable()
    {
        Panel = GetComponent<RectTransform>();
        //		Debug.Log ("LudoGameSafeArea");
        Refresh();
    }

    void Update()
    {
        Refresh();
    }
    public void Refresh()
    {
        Rect safeArea;
        safeArea = GetSafeArea();
        if (safeArea != LastSafeArea)
            ApplySafeArea(safeArea);
    }

    Rect GetSafeArea()
    {
        return Screen.safeArea;
    }

    void ApplySafeArea(Rect r)
    {
        LastSafeArea = r;

        Vector2 anchorMin = r.position;
        Vector2 anchorMax = r.position + r.size;
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        Panel.anchorMin = anchorMin;
        Panel.anchorMax = anchorMax;
    }
}
