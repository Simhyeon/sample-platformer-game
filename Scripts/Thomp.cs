using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerDetect))]
public class Thomp : MonoBehaviour
{
	// SerializeField
	[SerializeField] Vector2 fallingVelocity = new Vector2(0f, -15f);
	[SerializeField] float resetDelay = 5f;

	// Caches
	Vector3 startPosition;
	Rigidbody2D myRigidbody;
	PlayerDetect detector;

	// Statues
	bool onTop = true;

	void Start()
	{
		startPosition = transform.position;
		myRigidbody = GetComponent<Rigidbody2D>();
		detector = GetComponentInChildren<PlayerDetect>();
	}

	void Update()
	{
		DetectPlayer();
	}

	void DetectPlayer()
	{
		if(detector.GetDetection() && onTop)
		{
			myRigidbody.velocity = fallingVelocity;
			onTop = false;
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.layer == LayerMask.NameToLayer("Platforms") || other.gameObject.layer == LayerMask.NameToLayer("Player")) 
		{
			StartCoroutine(ReturnToTop(transform, startPosition, resetDelay));
		}
	}

	IEnumerator ReturnToTop(Transform transform, Vector3 position, float timeToMove)
	{
		onTop = false;
		var currentPosition = transform.position;
		var t = 0f;
		while (t < 1)
		{
			t += Time.deltaTime / timeToMove;
			transform.position = Vector3.Lerp(currentPosition, position, t);
			yield return null;
		}
		onTop = true;
	}
}
