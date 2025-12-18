using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RedTraingles : MonoBehaviour
{
    #region PUBLIC_VARIABLES

    public Image imgTriangle;

    public List<Sprite> spTrianglesList;

    [Range(0f, 1f)]
    public float startingFrom;

    #endregion

    #region PRIVATE_VARIABLES

    #endregion

    #region UNITY_CALLBACKS

    void OnEnable()
    {
        StartCoroutine(AnimateTriangles());
    }

    void OnDisable()
    {

    }

    #endregion

    #region DELEGATE_CALLBACKS

    #endregion

    #region PUBLIC_METHODS

    #endregion

    #region PRIVATE_METHODS

    #endregion

    #region COROUTINES

    private IEnumerator AnimateTriangles()
    {
        yield return new WaitForSeconds(startingFrom);

        while (true)
        {
            for (int i = 0; i < spTrianglesList.Count; i++)
            {
                imgTriangle.sprite = spTrianglesList[i];

                yield return new WaitForSeconds(.1f);
            }
        }
    }

    #endregion
}