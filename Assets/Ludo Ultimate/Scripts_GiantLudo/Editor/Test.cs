using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

public class Test : MonoBehaviour
{
}

//[CustomEditor(typeof(RectTransform))]
public class RectTransformEditor : Editor
{
    private Editor _editorInstance;
    private RectTransform rt;
    void OnEnable()
    {
        Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        Type rtEditor = assembly.GetType("UnityEditor.RectTransformEditor");
        _editorInstance = CreateEditor(target, rtEditor);
        rt = target as RectTransform;
    }

    public override void OnInspectorGUI()
    {
        _editorInstance.OnInspectorGUI();
        if (GUILayout.Button("Custom Anchors"))
        {
            SetCustomAnchors();
        }
    }

    private void SetCustomAnchors()
    {
        Debug.Log($"{rt.localPosition}||{rt.rect.size}");
    }
}