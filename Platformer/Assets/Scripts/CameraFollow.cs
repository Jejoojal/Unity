using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	public Transform target;
	public float camSpeed;
	public Vector3 targetPos;
	public bool follow = false;
	
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if (target && !follow)
		{
			Vector3 viewPos = RoundVector(Camera.main.WorldToViewportPoint(target.position));
			Vector3 campos = RoundVector(transform.position);
			if (0.4f < Mathf.Abs(0.5f - viewPos.x)) campos.x = target.position.x;
			if (0.3f < Mathf.Abs(0.5f - viewPos.y) - 0.1f) campos.y = target.position.y;
			targetPos = campos;
		}
		if (RoundVector(transform.position) != RoundVector(targetPos))
		{
			follow = true;
			transform.position = Vector3.Lerp(transform.position, targetPos, camSpeed);
		}
		else follow = false;
    }
	
	Vector3 RoundVector(Vector3 roundable)
	{
		float x = Mathf.Round(roundable.x * 100) / 100.0f;
		float y = Mathf.Round(roundable.y * 100) / 100.0f;
		return new Vector3(x, y, roundable.z);
	}
}
