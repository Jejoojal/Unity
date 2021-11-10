using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
	//directionals Down
	public virtual State leftKey(PlayerStates player){return null;}
	public virtual State rightKey(PlayerStates player){return null;}
	public virtual State upKey(PlayerStates player){return null;}
	public virtual State downKey(PlayerStates player){return null;}
	//directionals Up
	public virtual State leftKeyUp(PlayerStates player){return null;}
	public virtual State rightKeyUp(PlayerStates player){return null;}
	public virtual State upKeyUp(PlayerStates player){return null;}
	public virtual State downKeyUp(PlayerStates player){return null;}
	//ZXC
	public virtual State zKey(PlayerStates player){return null;}
	public virtual State zKeyUp(PlayerStates player){return null;}
	public virtual State xKey(PlayerStates player){return null;}
	public virtual State xKeyUp(PlayerStates player){return null;}
	public virtual State cKey(PlayerStates player){return null;}
	public virtual State cKeyUp(PlayerStates player){return null;}
	//ASD
	public virtual State aKey(PlayerStates player){return null;}
	public virtual State aKeyUp(PlayerStates player){return null;}
	public virtual State sKey(PlayerStates player){return null;}
	public virtual State dKey(PlayerStates player){return null;}
	//other triggers
	public virtual State GetHit(PlayerStates player){return null;}
	
	//Movement
	public virtual Vector2 Move(Rigidbody2D rb){return rb.velocity;}
	public virtual State Aerial(PlayerStates player, bool grounded){return null;}
	
	//Timer
	public virtual State Timer(PlayerStates player, float time){ return null;}
}

public abstract class SubState : State
{
	public State state;
	public SubState(State baseState)
	{
		state = baseState;
	}
}

//Unarmed Idle
public class NaturalState : State
{
	public override State leftKey(PlayerStates player){return new NaturalRunningState();}
	public override State rightKey(PlayerStates player){return new NaturalRunningState();}
	public override State downKey(PlayerStates player){return new NaturalDuckState();}
	public override State aKey(PlayerStates player){return new EquipMenu(this);}
	public override State aKeyUp(PlayerStates player){return new UnarmedStance();}
	public override State zKey(PlayerStates player){return new PunchState();}
	public override State xKey(PlayerStates player)
	{
		Rigidbody2D body = player.GetComponent<Rigidbody2D>();
		Vector2 velocity = body.velocity;
		velocity.y = 5; //JUMP FORCE
		body.velocity = velocity;
		return new NaturalJump();
	}
	public override Vector2 Move(Rigidbody2D rb)
	{
		return new Vector2(Mathf.Lerp(rb.velocity.x, 0, Time.deltaTime * 5),
		rb.velocity.y);
	}
}

//Weapon Menu
public class EquipMenu : SubState
{
	public EquipMenu(State basestate) : base(basestate){}
	public override State upKeyUp(PlayerStates player){return player.shortCuts[0];}
	public override State leftKeyUp(PlayerStates player){return player.shortCuts[1];}
	public override State downKeyUp(PlayerStates player){return player.shortCuts[2];}
	public override State rightKeyUp(PlayerStates player){return player.shortCuts[3];}
	public override State aKeyUp(PlayerStates player){return state.aKeyUp(player);}
	public override Vector2 Move(Rigidbody2D rb){return new Vector2(0, rb.velocity.y);}
}

//Unarmed Running
public class NaturalRunningState : State
{
	public override State leftKeyUp(PlayerStates player){return new NaturalState();}
	public override State rightKeyUp(PlayerStates player){return leftKeyUp(player);}
	public override State downKey(PlayerStates player){return new NaturalRollState();}
	public override Vector2 Move(Rigidbody2D rb){return new Vector2(Input.GetAxis("Horizontal"), rb.velocity.y);}
}

//Unarmed Ducking
public class NaturalDuckState : State
{
	public override State leftKey(PlayerStates player){return new NaturalCrawlState();}
	public override State rightKey(PlayerStates player){return new NaturalCrawlState();}
	public override State downKeyUp(PlayerStates player){return new NaturalState();}
	public override Vector2 Move(Rigidbody2D rb){return new Vector2(0, rb.velocity.y);}
}

//Unarmed Crawling
public class NaturalCrawlState : State
{
	public override State downKeyUp(PlayerStates player){return new NaturalRunningState();}
	public override State leftKeyUp(PlayerStates player){return new NaturalDuckState();}
	public override State rightKeyUp(PlayerStates player){return leftKeyUp(player);}
	public override Vector2 Move(Rigidbody2D rb){return new Vector2(Input.GetAxis("Horizontal")/2, rb.velocity.y);}
}

//Unarmed Roll
public class NaturalRollState : State
{
	public override Vector2 Move(Rigidbody2D rb)
	{
		return new Vector2(3 * Mathf.Sign(rb.velocity.x), rb.velocity.y);
	}
	
	public override State Timer(PlayerStates player, float time)
	{
		int roundTime = (int) Mathf.Floor(time);
		switch(roundTime)
		{
			case 1:
				if (Input.GetKey(KeyCode.DownArrow))
					return new NaturalCrawlState();
				else
					return new NaturalRunningState();
			default:
				return null;
		}
	}
}

//Punch
public class PunchState : State
{
	public override Vector2 Move(Rigidbody2D rb)
	{
		return new Vector2(2 * Mathf.Sign(rb.velocity.x), rb.velocity.y);
	}
	
	public override State Timer(PlayerStates player, float time)
	{
		int roundTime = (int) Mathf.Floor(time);
		switch(roundTime)
		{
			case 1:
				return new UnarmedStance();
			default:
				return null;
		}
	}
}

//Unarmed Combat Stance
public class UnarmedStance : NaturalState
{
	public override State aKeyUp(PlayerStates player){return new NaturalState();}
}

//Unarmed Jumping
public class NaturalJump : State
{
	public override State Aerial(PlayerStates player, bool grounded)
	{
		if (grounded) return new NaturalState();
		return null;
	}
}

//Player Controller
public class PlayerStates : MonoBehaviour
{
	State currentState;
	State nextState;
	public string cstate;
	Rigidbody2D body;
	public State[] shortCuts = new State[4]; //up:0 - left - down - right:3
	public float timer = 0;
	
	public float radius;
	public LayerMask whatIsGround;
	
    // Start is called before the first frame update
    void Start()
    {
        currentState = new NaturalState();
		body = GetComponent<Rigidbody2D>();
		shortCuts[0] = new NaturalState();
		shortCuts[1] = new NaturalState();
		shortCuts[2] = new NaturalState();
		shortCuts[3] = new NaturalState();
    }

    // Update is called once per frame
    void Update()
    {
		//Check if grounded
		bool grounded = Physics2D.OverlapCircle(transform.position - Vector3.up * 0.5f, radius, whatIsGround);
		nextState = currentState.Aerial(this, grounded);
		if (nextState != null) { currentState = nextState; timer = 0; }
		
        if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			nextState = currentState.leftKey(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		else if (Input.GetKeyUp(KeyCode.LeftArrow))
		{
			nextState = currentState.leftKeyUp(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			nextState = currentState.rightKey(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		else if (Input.GetKeyUp(KeyCode.RightArrow))
		{
			nextState = currentState.rightKeyUp(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
        else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			nextState = currentState.downKey(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		else if (Input.GetKeyUp(KeyCode.DownArrow))
		{
			nextState = currentState.downKeyUp(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
        else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			nextState = currentState.upKey(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		else if (Input.GetKeyUp(KeyCode.UpArrow))
		{
			nextState = currentState.upKeyUp(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		else if (Input.GetKeyDown(KeyCode.Z))
		{
			nextState = currentState.zKey(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		else if (Input.GetKeyUp(KeyCode.Z))
		{
			nextState = currentState.zKeyUp(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		else if (Input.GetKeyDown(KeyCode.X))
		{
			nextState = currentState.xKey(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		else if (Input.GetKeyUp(KeyCode.X))
		{
			nextState = currentState.xKeyUp(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		else if (Input.GetKeyDown(KeyCode.A))
		{
			nextState = currentState.aKey(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		else if (Input.GetKeyUp(KeyCode.A))
		{
			nextState = currentState.aKeyUp(this);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		else
		{
			nextState = currentState.Timer(this, timer);
			if (nextState != null) { currentState = nextState; timer = 0; }
		}
		timer += Time.deltaTime;
		cstate = currentState.ToString();
		body.velocity = currentState.Move(body);
    }
}
