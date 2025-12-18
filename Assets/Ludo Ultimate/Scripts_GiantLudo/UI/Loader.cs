using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loader : MonoBehaviour
{
    #region Private_Variables
    [SerializeField] private RectTransform loader;
    [SerializeField] private float loadingSpeed;
    public Text Message;
    #endregion

    #region  Unity_Callback
    void OnEnable()
    {
        transform.SetAsLastSibling();
    }
    private void OnDisable()
    {
        Message.text = "";
    }
    void Update()
    {
        loader.Rotate(Vector3.back, loadingSpeed * Time.deltaTime, Space.Self);
    }
    #endregion
}
