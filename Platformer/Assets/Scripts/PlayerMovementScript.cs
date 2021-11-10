using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
	public Transform ground;
	public Transform overhead;
	public Transform front;
	public Transform attack;
	public float radius;
	public LayerMask whatIsGround;
	public LayerMask whatIsWater;
	public LayerMask whatIsWall;
	public LayerMask whatIsAttackable;
	public float moveSpeed;
	public float jumpForce;
	public float speedLimit;
	public float waterMod;
	public bool unlockHover;
	public bool unlockWallJump;
	public bool unlockDive;
	
	float coolDownTimer = 0;
	public float coolDownDuration;
	public float attackRange;
	
	bool canMove = true;
	bool hurt = false;
	bool hovering;
	bool grounded;
	bool underwater;
	bool emerged;
	bool frontBlocked;
	bool crouching;
	Transform hang;
	Rigidbody2D body;
	
	Color stupidAnimationReplacement;

	
	//COLORDEBUG
	SpriteRenderer rend;
	
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
		rend = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
		//overlapping booleans
		grounded = Physics2D.OverlapCircle(ground.position, radius, whatIsGround);
		underwater = Physics2D.OverlapCircle(transform.position, radius, whatIsWater);
		emerged = Physics2D.OverlapCircle(overhead.position, radius, whatIsWater);
		frontBlocked = Physics2D.OverlapCircle(front.position, radius, whatIsWall);
		
		//Axes
        float hor_axis = Input.GetAxis("Horizontal");
        float ver_axis = Input.GetAxis("Vertical");
		
		//Crouching
		if (grounded && Input.GetKey(KeyCode.DownArrow)) crouching = true;
		else crouching = false;
		
		//Swap direction
		if (Mathf.Abs(hor_axis) > 0 && canMove)
		{
			Swap(hor_axis);
		}
		
		//MOVEMENTS
		if (canMove)
		{
			if (emerged && underwater) body.velocity = new Vector2(hor_axis * moveSpeed * waterMod, ver_axis * moveSpeed * waterMod);	//UNDERWATER MOVEMENT
			else if (underwater)
			{
				float ySpeed = Mathf.Max(body.velocity.y, 0);
				if (ver_axis < 0 && unlockDive) ySpeed = ver_axis * moveSpeed * waterMod;
				body.velocity = new Vector2(hor_axis * moveSpeed * waterMod, ySpeed);
			}
			else if (hang) //HANGING FROM LOOP
			{
				body.velocity = Vector3.zero;
				Vector3 hangpos = new Vector3(hang.position.x, hang.position.y-0.5f, transform.position.z);
				transform.position = Vector3.Lerp(transform.position, hangpos, 0.5f);
			}
			else if (hovering) body.velocity = new Vector2(hor_axis * moveSpeed, body.velocity.y); //HOVERING
			else body.velocity = new Vector2(hor_axis * moveSpeed, body.velocity.y); //NORMAL MOVEMENT
		}
		else if ((grounded && !hurt) || (body.velocity.y < 0 && Input.GetButtonDown("Horizontal")))
		{
			canMove = true;
		}
		
		//disable hurt
		if (!grounded) hurt = false;
		
		//Jump
		if (Input.GetKeyDown(KeyCode.X))
		{
			Jump();
		}
		
		//Fall
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			if (hang) hang = null;
		}
		
		//Limit vertical speed
		if (Mathf.Abs(body.velocity.y) > speedLimit)
		{
			body.velocity = new Vector2(body.velocity.x, speedLimit * Mathf.Sign(body.velocity.y));
			rend.color = Color.white;
		}
		
		hovering = Input.GetKey(KeyCode.X) && body.velocity.y < 0.5f && !grounded && !underwater && canMove;
		
		//Hover
		if (hovering && unlockHover)
		{
			body.gravityScale = 0.2f;
			rend.color = Color.magenta;
		}
		else
		{
			if (underwater || hang) body.gravityScale = 0;
			else body.gravityScale = 1f;
		}
		
		//Attack Timing
		if (coolDownTimer <= 0)
		{
			if (Input.GetKeyDown(KeyCode.Z))
			{
				Attack();
				coolDownTimer = coolDownDuration;
			}
			attack.localScale = Vector3.zero;
		}
		else
		{
			coolDownTimer -= Time.deltaTime;
			attack.localScale = Vector3.one * (coolDownTimer / coolDownDuration);
		}
		
		//
		if (stupidAnimationReplacement != rend.color)
		{
			stupidAnimationReplacement = rend.color;
		}
    }
	
	void FixedUpdate()
	{
		if (crouching)
		{
			rend.color = (Color.green + Color.clear) / 2;
		}
		else if (grounded)
		{
			rend.color = Color.green;
		}
		else if (hang)
		{
			rend.color = Color.yellow;
		}
		else if (!canMove)
		{
			rend.color = Color.grey;
		}
		else if (emerged)
		{
			rend.color = Color.blue;
		}
		else if (underwater && !emerged)
		{
			rend.color = Color.blue + Color.yellow * 0.75f;
		}
		else
		{
			rend.color = Color.red;
		}
	}
	
	//Jump
	void Jump()
	{
		bool canJump = grounded || (underwater && !emerged) || hang;
		if (body && canJump)
		{
			if (hang)
			{
				transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
				hang = null;
			}
			body.velocity = new Vector2(body.velocity.x, jumpForce);
		}
		else if (body && frontBlocked && !underwater && unlockWallJump)	//Wall Jump
		{
			canMove = false;
			body.velocity = new Vector2(jumpForce * -Mathf.Sign(transform.localScale.x), jumpForce);
			Swap(-Mathf.Sign(transform.localScale.x));
		}
	}
	
	//Attack
	void Attack()
	{
		Collider2D[] attackables = Physics2D.OverlapCircleAll(attack.position, attackRange, whatIsAttackable);
		attack.localScale = Vector3.one;
		foreach(Collider2D atk in attackables)
		{
			IAttackable iatk = atk.gameObject.GetComponent<IAttackable>();
			if (crouching) iatk.Pull();
			else iatk.Push();
		}
	}
	
	public void GetHit(float hitForce, float sign)
	{
		hang = null;
		canMove = false;
		body.velocity = new Vector2(hitForce * sign, hitForce);
		hurt = true;
	}
	
	public void ConnectHangable(Transform hangable)
	{
		if (!hang)
		{
			hang = hangable;
			canMove = true;
		}
	}
	
	void Swap(float sign)
	{
		Vector2 scale = transform.localScale;
		scale.x = Mathf.Abs(scale.x) * Mathf.Sign(sign);
		transform.localScale = scale;
	}
	
	void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(attack.position, attackRange);
	}
}
