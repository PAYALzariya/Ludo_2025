using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SlotReelIcon : MonoBehaviour
{
	#region PUBLIC_VARIABLES
	public string id;

	public Image imgSlotsReelIcon;
	#endregion

	#region PRIVATE_VARIABLES
	#endregion

	#region UNITY_CALLBACKS
	// Use this for initialization
	void OnEnable ()
	{
		imgSlotsReelIcon = GetComponent<Image> ();

//		GameManager.onResetMatchedIcon += HandleOnResetMatchedIcon;
	}

	void OnDisable()
	{
//		GameManager.onResetMatchedIcon -= HandleOnResetMatchedIcon;
	}
	#endregion

	#region DELEGATE_CALLBACKS
	private void HandleOnResetMatchedIcon ()
	{
		transform.localScale = Vector2.one;
	}
	#endregion

	#region PUBLIC_METHODS
	#endregion

	#region PRIVATE_METHODS
	#endregion

	#region COROUTINES
	#endregion
}