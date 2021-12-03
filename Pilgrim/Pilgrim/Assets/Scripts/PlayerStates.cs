using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Abstract class for all states
public abstract class State {
	PlayerStates player;
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
	public virtual Vector2 Move(PlayerStates player){
		Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
		return rb.velocity;
	}
	
	//Update
	public virtual State Update(PlayerStates player){ return null;}
}

//Abstract class for substates
public abstract class SubState : State {
	public State state;
	public SubState(State baseState)
	{
		state = baseState;
	}
}

//Basic Idle
public class NaturalState : State {
	public override State leftKey(PlayerStates player){return new NaturalRunningState();}
	public override State rightKey(PlayerStates player){return new NaturalRunningState();}
	public override State downKey(PlayerStates player){return new NaturalDuckState();}
	public override State upKey(PlayerStates player){return new Interact(this);}
	public override State aKey(PlayerStates player){return new EquipMenu(this);}
	public override State aKeyUp(PlayerStates player){return new BasicStance();}
	public override State zKey(PlayerStates player){return new PunchState();}
	public override State xKey(PlayerStates player){return new NaturalJumpState();}
	public override State Update(PlayerStates player)
	{
		if (!player.grounded) return new NaturalFallState();
		return null;
	}
	public override Vector2 Move(PlayerStates player)
	{
		Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
		return new Vector2(Mathf.Lerp(rb.velocity.x, 0, Time.deltaTime * 5),
			rb.velocity.y);
	}
}

//Weapon Menu
public class EquipMenu : SubState {
	public EquipMenu(State basestate) : base(basestate){}
	public override State upKeyUp(PlayerStates player){return player.shortCuts[0];}
	public override State leftKeyUp(PlayerStates player){return player.shortCuts[1];}
	public override State downKeyUp(PlayerStates player){return player.shortCuts[2];}
	public override State rightKeyUp(PlayerStates player){return player.shortCuts[3];}
	public override State aKeyUp(PlayerStates player){return state.aKeyUp(player);}
	public override Vector2 Move(PlayerStates player){
		Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
		return new Vector2(0, rb.velocity.y);}
}

//Basic Running
public class NaturalRunningState : State {
	public override State leftKeyUp(PlayerStates player){return new NaturalState();}
	public override State rightKeyUp(PlayerStates player){return leftKeyUp(player);}
	public override State downKey(PlayerStates player){return new NaturalRollState();}
	public override State xKey(PlayerStates player){return new NaturalJumpState();}
	public override Vector2 Move(PlayerStates player){
		Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
		return new Vector2(Input.GetAxis("Horizontal") * 2, rb.velocity.y);
	}
}

//Basic Ducking
public class NaturalDuckState : State {
	public override State leftKey(PlayerStates player){return new NaturalCrawlState();}
	public override State rightKey(PlayerStates player){return new NaturalCrawlState();}
	public override State downKeyUp(PlayerStates player){return new NaturalState();}
	public override State upKey(PlayerStates player){return new Interact(this);}
	public override Vector2 Move(PlayerStates player){
		Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
		return new Vector2(0, rb.velocity.y);
	}
}

//Basic Crawling
public class NaturalCrawlState : State {
	public override State downKeyUp(PlayerStates player){return new NaturalRunningState();}
	public override State leftKeyUp(PlayerStates player){return new NaturalDuckState();}
	public override State rightKeyUp(PlayerStates player){return leftKeyUp(player);}
	public override Vector2 Move(PlayerStates player){
		Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
		return new Vector2(Input.GetAxis("Horizontal")/2, rb.velocity.y);
	}
}

//Basic Roll
public class NaturalRollState : State {
	float timer = 0;
	
	public override Vector2 Move(PlayerStates player)
	{
		Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
		return new Vector2(3 * Mathf.Sign(rb.velocity.x), rb.velocity.y);
	}
	
	public override State Update(PlayerStates player)
	{
		timer += Time.deltaTime;
		int roundTime = (int) Mathf.Floor(timer);
		if (roundTime >= 1){
			if (Input.GetKey(KeyCode.DownArrow))
				return new NaturalCrawlState();
			else if (Input.GetAxis("Horizontal") > 0)
				return new NaturalRunningState();
			else
				return new NaturalState();
		}
		else return null;
	}
}

//Punch
public class PunchState : State {
	float timer = 0;
	public override Vector2 Move(PlayerStates player)
	{
		Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
		return new Vector2(2 * Mathf.Sign(rb.velocity.x), rb.velocity.y);
	}
	
	public override State Update(PlayerStates player)
	{
		timer += Time.deltaTime;
		int roundTime = (int) Mathf.Floor(timer);
		switch(roundTime)
		{
			case 1:
				return new BasicStance();
			default:
				return null;
		}
	}
}

//Basic Combat Stance
public class BasicStance : NaturalState {
	public override State aKeyUp(PlayerStates player){return new NaturalState();}
}

//Basic Jumping
public class NaturalJumpState : State {
	float timer = 0;
	public override State Update(PlayerStates player)
	{
		timer += Time.deltaTime;
		Rigidbody2D body = player.GetComponent<Rigidbody2D>();
		if (timer <= 0.1f)
		{
			Vector2 velocity = body.velocity;
			velocity.y = 5; //JUMP FORCE (replace with player stats)
			body.velocity = velocity;
		}
		else if (body.velocity.y <= 0)
		{
			return new NaturalFallState();
		}
		return null;
	}
}

//Basic Controlled falling (after jumping or simple falling while moving)
public class NaturalFallState : State {
	public override State Update(PlayerStates player)
	{
		if (player.grounded)
		{
			if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0) return new NaturalRunningState();
			else return new NaturalState();
		}
		return null;
	}
	
	//public override Vector2 Move(PlayerStates player)
	//{
	//	return new Vector2(Mathf.Max(Input.GetAxis("Horizontal"), rb.velocity.x), rb.velocity.y);
	//}
}

//Interacting with different objects
public class Interact : SubState {
	public Interact(State basestate) : base(basestate){}
	public override State Update(PlayerStates player){
		Collider2D interact =
			Physics2D.OverlapCircle(player.transform.position, player.radius, player.whatIsInteractable);
		if (interact){
			Item item = interact.GetComponent<Item>();
			if (item){
				return item.PickUp(this);
			}
			else return state;
		}
		else
			return state;
	}
}


//Player Controller
public class PlayerStates : MonoBehaviour {
	State currentState;		//Current player State
	State nextState;		//The next state to become the Current State
	public string cstate;	//Temporary state indicator (From Editor)
	Rigidbody2D body;		//Character's Rigidbody2D
	
	public State[] shortCuts = new State[4]; //up:0 - left - down - right:3 (Temporary test for weapon equiping)
	public float radius;					//Radius of the ground detection
	public LayerMask whatIsGround;			//LayerMask for ground detection
	public LayerMask whatIsInteractable;	//LayerMask for interactable objects
	
	public bool grounded;			//Boolean for ground detection
	BoxCollider2D box;
	
	public float playerSpeed = 2;	//Temporary player speed
	
    // Start is called before the first frame update
    void Start()
    {
        currentState = new NaturalState();
		body = GetComponent<Rigidbody2D>();
		box = GetComponent<BoxCollider2D>();
		shortCuts[0] = new NaturalState();
		shortCuts[1] = new NaturalState();
		shortCuts[2] = new NaturalState();
		shortCuts[3] = new NaturalState();
    }

    // Update is called once per frame
    void Update()
    {
		//Check if grounded
		grounded = Physics2D.OverlapBox(transform.position - Vector3.up * 0.25f, box.size, 0, whatIsGround);

		//Inputs & Other States
        if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			nextState = currentState.leftKey(this);
			if (nextState != null) { currentState = nextState;}
		}
		else if (Input.GetKeyUp(KeyCode.LeftArrow))
		{
			nextState = currentState.leftKeyUp(this);
			if (nextState != null) { currentState = nextState;}
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			nextState = currentState.rightKey(this);
			if (nextState != null) { currentState = nextState;}
		}
		else if (Input.GetKeyUp(KeyCode.RightArrow))
		{
			nextState = currentState.rightKeyUp(this);
			if (nextState != null) { currentState = nextState;}
		}
        else if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			nextState = currentState.downKey(this);
			if (nextState != null) { currentState = nextState;}
		}
		else if (Input.GetKeyUp(KeyCode.DownArrow))
		{
			nextState = currentState.downKeyUp(this);
			if (nextState != null) { currentState = nextState;}
		}
        else if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			nextState = currentState.upKey(this);
			if (nextState != null) { currentState = nextState;}
		}
		else if (Input.GetKeyUp(KeyCode.UpArrow))
		{
			nextState = currentState.upKeyUp(this);
			if (nextState != null) { currentState = nextState;}
		}
		else if (Input.GetKeyDown(KeyCode.Z))
		{
			nextState = currentState.zKey(this);
			if (nextState != null) { currentState = nextState;}
		}
		else if (Input.GetKeyUp(KeyCode.Z))
		{
			nextState = currentState.zKeyUp(this);
			if (nextState != null) { currentState = nextState;}
		}
		else if (Input.GetKeyDown(KeyCode.X))
		{
			nextState = currentState.xKey(this);
			if (nextState != null) { currentState = nextState;}
		}
		else if (Input.GetKeyUp(KeyCode.X))
		{
			nextState = currentState.xKeyUp(this);
			if (nextState != null) { currentState = nextState;}
		}
		else if (Input.GetKeyDown(KeyCode.A))
		{
			nextState = currentState.aKey(this);
			if (nextState != null) { currentState = nextState;}
		}
		else if (Input.GetKeyUp(KeyCode.A))
		{
			nextState = currentState.aKeyUp(this);
			if (nextState != null) { currentState = nextState;}
		}
		else
		{
			nextState = currentState.Update(this);
			if (nextState != null) { currentState = nextState;}
		}
		
		//Movement
		cstate = currentState.ToString();
		body.velocity = currentState.Move(this);
    }
    private void OnDrawGizmos()
    {
        Vector2 origin = transform.position - Vector3.up * 0.25f;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(origin, Vector2.one);
        Gizmos.DrawSphere(transform.position, radius);
    }
	
}
