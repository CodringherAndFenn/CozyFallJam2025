using UnityEngine;


public class playerMovement : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private float speed = 10;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalIn = Input.GetAxisRaw("Horizontal") * speed;
        float verticalIn = Input.GetAxisRaw("Vertical") * speed;
        
        rb.linearVelocity = new Vector3(horizontalIn, 0 , verticalIn);
    }
}
