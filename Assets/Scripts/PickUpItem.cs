using UnityEngine;

public class PickUpItem : MonoBehaviour
{

    public GameObject heldItem; // should be invisible, but attached to the player
    public GameObject itemToPickUp; // some item in the scene

    
    

    void Start()
    {
        heldItem.SetActive(false);
        itemToPickUp.SetActive(true);
    }

    
    void Update()
    {
        // this code might overlap with what stefan did
        if (Input.GetKeyDown(KeyCode.E))
        {
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 2f))
            {
                if (hit.transform.gameObject == itemToPickUp)
                {
                    itemToPickUp.SetActive(false);
                    heldItem.SetActive(true);
                }
            }
        }

    }
}
