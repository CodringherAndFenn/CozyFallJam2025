using UnityEngine;

public class PickUpItem : MonoBehaviour
{

    public GameObject heldItem;
    public GameObject itemToPickUp;
    
    

    void Start()
    {
        heldItem.SetActive(false);
        itemToPickUp.SetActive(true);
    }

    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            float distance = Vector3.Distance(transform.position, itemToPickUp.transform.position);
            //float direction = Vector3.Angle();
            if (distance < 2f) 
            {
                itemToPickUp.SetActive(false); 
                heldItem.SetActive(true);    
            }
        }
    }
}
