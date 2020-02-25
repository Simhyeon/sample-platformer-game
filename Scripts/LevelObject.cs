using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObject : MonoBehaviour
{
	bool interactable = true;

	public bool GetInteractable()
	{
		return interactable;
	}

	public bool Interact()
	{
		if (interactable == true)
		{
			interactable = false;
			return true;
		}
		else
		{
			return false;
		}
	}

	public void Reset()
	{
		interactable = true;
	}
}
