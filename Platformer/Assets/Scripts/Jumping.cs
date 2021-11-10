using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumping : MonoBehaviour
{
	public Transform ground;
	public float radius;
	public LayerMask whatIsGround;
	public float jumpForce;
	bool grounded;
	Rigidbody2D body;
	
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
        grounded = Physics2D.OverlapCircle(ground.position, radius, whatIsGround);
		if (Input.GetKeyDown(KeyCode.X))
		{
			Jump();
		}
		
		if (grounded)
		{
			rend.color = Color.green;
		}
		else
		{
			rend.color = Color.red;
		}
    }
	
	void Jump()
	{
		if (body && grounded)
		{
			Vector2 velocity = body.velocity;
			velocity.y = jumpForce;
			body.velocity = velocity;
		}
	}
}
