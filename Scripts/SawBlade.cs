using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : MonoBehaviour
{
	// Route
	[SerializeField] Transform destination;
	[SerializeField] float loopTime = 3f;
	[SerializeField] bool isStatic = false;

	float proportion = 0f;
	bool isIncreasing = false;
	Vector3 startPosition;
	Vector3 targetPosition;

	void Start()
	{
		startPosition = transform.position;
		targetPosition = destination.position;
	}

	void Update()
	{
		if (!isStatic)
		{
			LoopPositions();
		}
	}

	void LoopPositions()
	{
		if (proportion <= 0)
		{
			isIncreasing = true;
		}
		else if (proportion >= 1) 
		{
			isIncreasing = false;
		}

		if (isIncreasing)
		{
			proportion += Time.deltaTime / loopTime;
		}
		else
		{
			proportion -= Time.deltaTime / loopTime;
		}

		transform.position = Vector3.Lerp(startPosition, targetPosition, proportion);
	}
}
