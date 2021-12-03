using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpState : State {
	float timer = 0;
	public override State Update(PlayerStates player) {
		timer += Time.deltaTime;
		int roundTime = (int) Mathf.Floor(timer);
		if (roundTime >= 1){
			return new NaturalState();	//FIX: Picked up item in hand
		}
		else return null;
	}
}

public class Item : MonoBehaviour
{
	public State PickUp(State state)
	{
		gameObject.SetActive(false);
		return new PickUpState();
	}
}
