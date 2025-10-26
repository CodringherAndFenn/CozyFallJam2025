using UnityEngine;
using LeafPhysics.Code;

public class LeafBlowerInteraction : MonoBehaviour
{
    [SerializeField] private float maxRange = 2.5f;
    [SerializeField] private GPUInstancing leafSystem;
    [SerializeField] private Transform target;

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.forward, out hit, maxRange))
        {
            if (hit.collider.CompareTag("Ground"))
                target.position = hit.point;
        }
        else
        {
            target.position = transform.position - transform.forward * maxRange;
        }
        
        leafSystem.DoUpdate = Input.GetKey(KeyCode.Mouse0);
    }
}
