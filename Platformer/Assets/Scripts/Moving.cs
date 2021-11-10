using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
	public float moveSpeed;
	Rigidbody2D body;
	
    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float hor_axis = Input.GetAxis("Horizontal");
		Vector2 velocity = body.velocity;
		velocity.x = moveSpeed * hor_axis;
		body.velocity = velocity;
    }
}
