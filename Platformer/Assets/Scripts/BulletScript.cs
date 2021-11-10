using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletScript : MonoBehaviour, IAttackable
{
	public Vector3 direction;
	public float speed;
	public float bounds;
	public float power;
	Vector2 source;
	
    // Start is called before the first frame update
    void Start()
    {
        source = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + direction * speed;
		if (Mathf.Abs(transform.position.x) > bounds || Mathf.Abs(transform.position.y) > bounds)
			transform.position = source;
    }
	
	void OnCollisionEnter2D(Collision2D collision)
	{
		transform.position = source;
		PlayerMovementScript pms = collision.gameObject.GetComponent<PlayerMovementScript>();
		if (pms)
		{
			pms.GetHit(power, Math.Sign(direction.x));
		}
	}
	
	public void Push()
	{
		direction = -direction;
	}
	
	public void Pull()
	{
		transform.position = source;
	}
}
