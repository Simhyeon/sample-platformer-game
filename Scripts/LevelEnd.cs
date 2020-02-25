using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
	bool levelClear = false;
	void OnTriggerEnter2D(Collider2D other)
	{
		if (levelClear) { return; }
		LevelManager.levelManager.LevelClear();
		levelClear = true;
	}
}
