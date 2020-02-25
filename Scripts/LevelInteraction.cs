using UnityEngine;

public class LevelInteraction : MonoBehaviour
{
	public static LevelInteraction levelInteraction;

	void Awake()
	{
		levelInteraction = this;
	}

	public void ResetLevelObjects()
	{
		foreach (LevelObject item in GetComponentsInChildren<LevelObject>())
		 {
			 item.Reset();
		 } 
	}
}
