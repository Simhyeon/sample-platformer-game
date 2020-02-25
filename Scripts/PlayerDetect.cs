using UnityEngine;

public class PlayerDetect : MonoBehaviour
{
	bool isPlayerDetecting = false;

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			isPlayerDetecting = true;
		}
		else 
		{
			isPlayerDetecting = false;
		}
	}

	public bool GetDetection()
	{
		return isPlayerDetecting;
	}
}
