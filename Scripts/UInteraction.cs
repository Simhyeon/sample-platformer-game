using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UInteraction : MonoBehaviour
{
	public static UInteraction uInteraction;

	[SerializeField] DelayIndicator dashDelay;

	public enum DelayKinds {
		DASH
	}

	void Awake()
	{
		uInteraction = this;
	}

	public void SetDelay(DelayKinds kind, float delaySeconds)
	{
		switch (kind)
		{
			case DelayKinds.DASH:
				dashDelay.SetDelay(delaySeconds);
				break;

			default:
				break;
		}
	}
}
