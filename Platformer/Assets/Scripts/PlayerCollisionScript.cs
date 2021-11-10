using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
	void Push();
	void Pull();
}

public class PlayerCollisionScript : MonoBehaviour
{
	PlayerMovementScript pms;
	
    // Start is called before the first frame update
    void Start()
    {
        pms = GetComponent<PlayerMovementScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnCollisionEnter2D(Collision2D collision)
	{

	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.CompareTag("PickUp"))
		{
			other.gameObject.SetActive(false);
		}
		else if (other.gameObject.CompareTag("Hangable"))
		{
			pms.ConnectHangable(other.transform);
		}
	}
}
