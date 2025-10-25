using UnityEngine;


public class playerMovement : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed = 10;
	public Transform orientation;
	private float horizontalIn;
	private float verticalIn;

	private Vector3 moveDir;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
		rb.freezeRotation = true;
    }

    // Update is called once per frame
    void Update()
    {
        horizontalIn = Input.GetAxisRaw("Horizontal") * speed;
        verticalIn = Input.GetAxisRaw("Vertical") * speed;
        
        //rb.linearVelocity = new Vector3(horizontalIn, 0 , verticalIn);
        
    }

    void FixedUpdate()
    {
	    move();
    }
    
	private void move()
	{
		moveDir = orientation.forward * verticalIn + orientation.right * horizontalIn;
		rb.AddForce(moveDir.normalized * speed, ForceMode.Force);
	}

}
