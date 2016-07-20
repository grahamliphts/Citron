using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {
	// Movement keys (customizable in inspector)
    [SerializeField]
	private KeyCode upKey;
    [SerializeField]
    private KeyCode downKey;
    [SerializeField]
    private KeyCode rightKey;
    [SerializeField]
    private KeyCode leftKey;

    [SerializeField]
    private bool remoteMoving;

    [SerializeField]
    private Server m_Server;
	// Movement Speed
	public float speed = 16;

	// Wall Prefab
	public GameObject wallPrefab;

	// Current Wall
	Collider2D wall;

	// Last Wall's End
	Vector2 lastWallEnd;

    private bool playerIsAlive;
    private Rigidbody2D m_RigidBody;

    private bool UpdateIsWaiting;
    private Vector2 VelocityBuffer;

    private bool Standby;

    private int pendingMove = -1; 

	void Start () {
        m_RigidBody = GetComponent<Rigidbody2D>();
        /*m_RigidBody.velocity = Vector2.up * speed;
        spawnWall();*/
        playerIsAlive = true;
        UpdateIsWaiting = false;
        Standby = true;
        
    }
	
	// Update is called once per frame
	void Update () {
        if (Standby)
            return;
        if (!remoteMoving)
        {
            // Check for key presses
            if (Input.GetKeyDown(upKey))
            {
                //move(0); 
                m_Server.UpdateMove(0);
            }
            else if (Input.GetKeyDown(downKey))
            {
                //move(2);
                m_Server.UpdateMove(2);
            }
            else if (Input.GetKeyDown(rightKey))
            {
                //move(1);
                m_Server.UpdateMove(1);
            }
            else if (Input.GetKeyDown(leftKey))
            {
                //move(3);
                m_Server.UpdateMove(3);
            }
            
            if (pendingMove != -1)
                move(pendingMove);
        }
        else if(UpdateIsWaiting)
        {
            m_RigidBody.velocity = VelocityBuffer;
            spawnWall();
            UpdateIsWaiting = false;
        }

		fitColliderBetween (wall, lastWallEnd, transform.position);
	}

	void spawnWall() {
		// Save last wall's position
		lastWallEnd = transform.position;

		// Spawn a new Lightwall
		GameObject g = (GameObject)Instantiate (wallPrefab, transform.position, Quaternion.identity);
		wall = g.GetComponent<Collider2D>();
	}

	void fitColliderBetween(Collider2D co, Vector2 a, Vector2 b) {
		// Calculate the Center Position
		co.transform.position = a + (b - a) * 0.5f;

		// Scale it (horizontally or vertically)
		float dist = Vector2.Distance (a, b);
		if (a.x != b.x)
			co.transform.localScale = new Vector2 (dist + 1, 1);
		else
			co.transform.localScale = new Vector2 (1, dist + 1);
	}

	void OnTriggerEnter2D(Collider2D co) {
		if (co != wall) {
			print ("Player lost:" + name);
            playerIsAlive = false;
            Standby = true;

            //Destroy (gameObject);
		}
	}

    public bool isRemotePlayer()
    {
        return remoteMoving;
    }

    public bool isPlayerDead()
    {
        return !playerIsAlive;
    }

    public void remoteUpdate(int direction)
    {
        Debug.Log(direction);
       
        switch (direction)
            {
            case 0:
                VelocityBuffer = Vector2.up * speed;
                
                break;
            case 1:
                VelocityBuffer = Vector2.right * speed;
                break;
            case 2:
                VelocityBuffer = -Vector2.up * speed;
                break;
            case 3:
                VelocityBuffer = -Vector2.right * speed;
                break;
          
            }
        UpdateIsWaiting = true;
            
    }
    public void startMoving()
    {
        Standby = false;
        if (!remoteMoving)
        {
            m_RigidBody.velocity = Vector2.up * speed;
            spawnWall();
        }
        else
        {
            VelocityBuffer = Vector2.up * speed;
            UpdateIsWaiting = true;
        }
    }
   
    public void setRemote(bool arg)
    {
        remoteMoving = arg;
    }

    public void EndGame()
    {
        Destroy(gameObject);
    }

    public void move(int a_direction)
    {
        // Check for key presses
        if (a_direction == 0)
        {
            m_RigidBody.velocity = Vector2.up * speed;
            spawnWall();
        }
        else if (a_direction == 2)
        {
            m_RigidBody.velocity = -Vector2.up * speed;
            spawnWall();
        }
        else if (a_direction == 1)
        {
            m_RigidBody.velocity = Vector2.right * speed;
            spawnWall();
        }
        else if (a_direction == 3)
        {
            m_RigidBody.velocity = -Vector2.right * speed;
            spawnWall();
        }
        pendingMove = -1;
    }

    public void validateMove(int a_pendingMove)
    {
        pendingMove = a_pendingMove;
    }
}
