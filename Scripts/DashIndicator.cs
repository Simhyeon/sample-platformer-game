using UnityEngine;

public class DashIndicator : DelayIndicator
{
	PlayerController player;

	void Start()
	{
		player = FindObjectOfType<PlayerController>();
	}

	protected override void EndDelay()
	{

	}
}
