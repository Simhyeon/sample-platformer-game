using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{
	// Player Parameter
	[Header("Player Parameter")]
	[SerializeField] float movementSpeed = 5f;
	[SerializeField] float jumpSpeed = 15f;
	[SerializeField] float jumpAnimationOffset = 0.1f;
	[SerializeField] int jumpCount = 1;
	[SerializeField] int wallJumpCount = 1;
	[SerializeField] [Range(0.1f, 0.3f)] float jumpHoldForce = 0.1f;
	[SerializeField] float wallJumpSpeed = 5f;
	[SerializeField] float wallJumpDuration = 0.5f;
	[SerializeField] float dashAnimDuration = 1f;
	[SerializeField] float dashSpeed = 5f;
	[SerializeField] float dashDelay = 1f;
	[SerializeField] Transform pickPosition ;

	// Collider 
	[SerializeField] BoxCollider2D bodyCollider;
	[SerializeField] BoxCollider2D FootCollider;
	[SerializeField] BoxCollider2D WallDashCollider;

	// Particle objects
	[Header("Particle Objects")]
	[SerializeField] GameObject jumpBoostParticle;

	// Cached Components
	BoxCollider2D myCollider;
	Rigidbody2D myRigidbody;
	Animator animator;
	float originalGravity;

	//Statuses
	bool isDead = false;
	bool isTouchingWall = false;
	bool isTouchingLand = false;
	bool onWallJumpAnim = false;
	WallJumpDirection wallJumpDirection = WallJumpDirection.FORWARD;
	bool onDash = false;
	bool onFall = false;
	bool onGrab = false;
	bool onDialogue = false;
	float dashDelayLeft = 0f;
	PickableItem pickedItem;

	[SerializeField] Inventory inventoryIndicator; 

	enum WallJumpDirection 
	{ 
		FORWARD = 1,
		BACKWARD = -1
	};

	void Start()
	{
		myCollider = GetComponent<BoxCollider2D>();
		myRigidbody = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		originalGravity = myRigidbody.gravityScale;
	}

	void Update()
	{
		if (!isDead)
		{
			Move();
			Jump();
			WallJump();
			LandPlatforms();
			Fall();
			Dash();
			DropItem();
		}
	}		

	void FixedUpdate()
	{
		if (!isDead)
		{
			WallJumpForce();
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (!isDead)
		{
			// LandPlatforms(other);
			ObjectInteraction(other);
			GetDamage(other.gameObject);
		}
	}

	// Mostly Button Interaction
	void OnTriggerStay2D(Collider2D col) 
	{
		if (!isDead)
		{
			GrabItem(col);
			NpcInteraction(col);
		}

	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (!isDead)
		{
			WallJump();
		}
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (!isDead)
		{
			GetDamage(other.gameObject);
		}
	}

	void WallJump()
	{
		isTouchingWall = WallDashCollider.IsTouchingLayers(LayerMask.GetMask("Platforms"));
		if(isTouchingWall && !isTouchingLand)
		{
			if (onDash) { return; }
			if (onFall) {myRigidbody.gravityScale = originalGravity / 5;}
			if (wallJumpCount > 0 && CrossPlatformInputManager.GetButtonDown("Jump"))
			{
				if (isTouchingWall)
				{
					StartCoroutine(WallJumpAnim(transform.localScale.x));
				}
				myRigidbody.velocity = new Vector2(0f, 0f);
				Vector2 jumpVelocity = new Vector2(myRigidbody.velocity.x, jumpSpeed);
				myRigidbody.velocity = jumpVelocity;
				wallJumpCount = 0;
				jumpCount = 0;
				animator.SetTrigger("Jump");
			}
		}
		else if (!onDash)
		{
			myRigidbody.gravityScale = originalGravity;
		}
	}

	void LandPlatforms()
	{
		isTouchingLand = FootCollider.IsTouchingLayers(LayerMask.GetMask("Platforms")) || FootCollider.IsTouchingLayers(LayerMask.GetMask("PickablePlatform"));
		bool isPlayerJumping = Mathf.Abs(myRigidbody.velocity.y) > jumpAnimationOffset;
		if (isTouchingLand && !isPlayerJumping)
		{
			onWallJumpAnim = false;
			jumpCount = 1;
			wallJumpCount = 1;
			animator.SetBool("OnFall", false);
			
			LevelInteraction.levelInteraction.ResetLevelObjects();
		}
	}

	void ObjectInteraction(Collider2D collider)
	{
		if (collider.GetComponent<LevelObject>() == null) { return; }
		if (!collider.GetComponent<LevelObject>().Interact()) { return; }
		switch (collider.gameObject.tag)
		{
			case "JumpBoost":
				jumpCount = 1;
				wallJumpCount = 1;
				StartCoroutine(TriggerParticle(jumpBoostParticle, collider.gameObject.transform.position));
				break;

			case "DashBoost":
				dashDelayLeft = 0;
				UInteraction.uInteraction.SetDelay(UInteraction.DelayKinds.DASH, 0f);
				StartCoroutine(TriggerParticle(jumpBoostParticle, collider.gameObject.transform.position));
				break;

			//case "Key":
			//	inventoryIndicator.Add(collider.gameObject.tag, collider.GetComponent<SpriteRenderer>().sprite);
			//	Destroy(collider.gameObject);
			//	StartCoroutine(TriggerParticle(jumpBoostParticle, collider.gameObject.transform.position));
			//	break;

			case "Door":
				if ( !inventoryIndicator.UseItem("Key") ) { return; }
				Destroy(collider.gameObject);
				StartCoroutine(TriggerParticle(jumpBoostParticle, collider.gameObject.transform.position));
				break;

			default:
				inventoryIndicator.Add(collider.gameObject.tag, collider.GetComponent<SpriteRenderer>().sprite);
				Destroy(collider.gameObject);
				StartCoroutine(TriggerParticle(jumpBoostParticle, collider.gameObject.transform.position));
				break;
		}
	}

	void GetDamage(GameObject col)
	{
		if (!bodyCollider.IsTouchingLayers(LayerMask.GetMask("Obstacles")) && !bodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy"))) { return; }
		if (col.tag == "DamageDealer" || col.tag == "DeathCollider")
		{
			Debug.Log("Player Dead");
			isDead = true;
			myRigidbody.Sleep();
			animator.SetBool("OnFall", false);
			animator.SetBool("Run", false);
			animator.SetTrigger("Death");
			myRigidbody.velocity = new Vector2(0f, 0f);
			LevelManager.levelManager.PlayerDeath();
		}
	}

	IEnumerator TriggerParticle(GameObject particle, Vector3 position)
	{
		GameObject cachedTrigger = Instantiate(particle, position, Quaternion.identity);
		yield return new WaitForSeconds(0.3f);
		Destroy(cachedTrigger);
	}

	void Move()
	{
		if (onDash) { return; }
		if (onWallJumpAnim) { return; }
		float controlThrow = CrossPlatformInputManager.GetAxisRaw("Horizontal");
		Vector2 playerVelocity = new Vector2(controlThrow * movementSpeed, myRigidbody.velocity.y);

		myRigidbody.velocity = playerVelocity;

		bool playerHasVelocity = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
		if (playerHasVelocity)
		{
			transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
			animator.SetBool("Run", true);
		}
		else 
		{
			animator.SetBool("Run", false);
			if (pickedItem != null)
			{
				pickedItem.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 0f);
			}
		}

	}

	void Jump()
	{
		if (onDash) { return; }
		if (jumpCount > 0 && CrossPlatformInputManager.GetButtonDown("Jump"))
		{
			Vector2 jumpVelocity = new Vector2(myRigidbody.velocity.x, jumpSpeed);
			myRigidbody.velocity = jumpVelocity;
			jumpCount = 0;
			animator.SetTrigger("Jump");
		}
		else if (!onFall && !isTouchingWall && CrossPlatformInputManager.GetButton("Jump"))
		{
			myRigidbody.velocity += new Vector2(0f, jumpHoldForce);
		}
	}

	void WallJumpForce()
	{
		if (!onWallJumpAnim) { return; }
		myRigidbody.AddForce(new Vector2( (float) wallJumpDirection * wallJumpSpeed, 0f), ForceMode2D.Impulse);
	}

	IEnumerator WallJumpAnim(float xLocalScale)
	{
		transform.localScale = new Vector2( -Mathf.Sign(transform.localScale.x), 1f);
		wallJumpDirection = (WallJumpDirection)(-xLocalScale);
		onWallJumpAnim = true;
		yield return new WaitForSeconds(wallJumpDuration);
		onWallJumpAnim = false;
	}
	
	void Dash()
	{
		if (dashDelayLeft > 0) 
		{
			dashDelayLeft -= Time.deltaTime; 
			return;
		}
		// CrossPlatform이라고 되어 있지만 정작 콘솔 키가 입력은 안 되어 있다 왜냐면 뭐가 무슨 키인지 모르고 찾긴 귀찮다.
		if (CrossPlatformInputManager.GetButtonDown("Dash") && dashDelayLeft <= 0 )
		{
			if (onDash) { return; }
			StartCoroutine(DashAnim());
			dashDelayLeft = dashDelay;
		}
	}

	IEnumerator DashAnim()
	{
		onDash = true;
		animator.SetBool("Dash", true);
		myRigidbody.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * dashSpeed, 0f);
		myRigidbody.gravityScale = 0f;
		UInteraction.uInteraction.SetDelay(UInteraction.DelayKinds.DASH, dashDelay);
		yield return new WaitForSeconds(dashAnimDuration);

		myRigidbody.velocity = new Vector2(0f, myRigidbody.velocity.y);
		myRigidbody.gravityScale = originalGravity;
		animator.SetBool("Dash", false);
		onDash = false;
	}

	void Fall()
	{
		onFall = myRigidbody.velocity.y < -jumpAnimationOffset;
		if (onFall)
		{
			animator.SetBool("OnFall", true);
		}
		else 
		{
			animator.SetBool("OnFall", false);
		}
	}

	void GrabItem(Collider2D col)
	{
		if (onGrab) { return; }
		if (col.gameObject.tag == "Pickable" && 
				CrossPlatformInputManager.GetButtonDown("Grab")) 
		{
			Debug.Log("Grab Item");
			if (pickedItem == col.GetComponent<PickableItem>()) { return; }
			if (col.GetComponent<PickableItem>() == null) 
			{ 
				Debug.LogError("Pickable error"); 
				return; 
			}
			pickedItem = col.GetComponent<PickableItem>();
			pickedItem.GetComponent<PickableItem>().Grab(pickPosition);
			StartCoroutine(GrabDelay());
		}
	}

	IEnumerator GrabDelay()
	{
		yield return new WaitForSeconds(0.3f);
		onGrab = true;
	}

	void DropItem()
	{
		if (!onGrab) { return; }
		if (pickedItem == null) 
		{
			Debug.LogError("Inconsistent grab sequence detected");
			return;
		}
		// Change this to check if player has grabbed or not
		if (CrossPlatformInputManager.GetButtonDown("Grab") && !pickedItem.IsIntersecting()) 
		{
			pickedItem.Drop();
			// set gravity sacle to default or original 
			pickedItem = null;
			onGrab = false;
		}
	}

	void NpcInteraction(Collider2D col)
	{
		// COnsider removing onDialogue value
		if (onGrab) { return; }
		if (col.tag != "NPC") { return; }
		if (onDialogue) { return; }
		//Debug.Log("Detecting NPc Interaction");
		if (CrossPlatformInputManager.GetButtonDown("Grab"))
		{
			Debug.Log("----NPC Interaction----");
			onDialogue = true;
			myRigidbody.Sleep();
			FindObjectOfType<NpcDialogue>().StartDialogue(col.GetComponent<NonPlayerCharacter>().GetDialogue());
		}
	}

	public void DialogueExit()
	{
		onDialogue = false;
		myRigidbody.WakeUp();
	}
}
