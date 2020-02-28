using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PickableItem : MonoBehaviour
{
	Transform playerPicker;
	[SerializeField] Collider2D mainCollider;
	public void Grab(Transform picker)
	{
		playerPicker = picker;
		transform.position = picker.position;
		GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		mainCollider.isTrigger = true;
	}

	public void Drop()
	{
		playerPicker = null;
		Debug.Log("Dropped");
		mainCollider.isTrigger = false;
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
		GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
	}

	public bool IsIntersecting()
	{
		if (mainCollider.IsTouchingLayers(LayerMask.GetMask("Platforms")))
		{
			return true;
		}
		else 
		{
			return false;
		}
	}

	void Update()
	{
		if (playerPicker == null) { return; }
		transform.position = playerPicker.position;
	}
}
