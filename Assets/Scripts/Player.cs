using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //Define delegate types and events here

    public Node CurrentNode { get; private set; }
    public Node TargetNode { get; private set; }

    [SerializeField] private float speed = 4;
    private bool moving = false;
    private Vector3 currentDir;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Node node in GameManager.Instance.Nodes)
        {
            if(node.Parents.Length > 2 && node.Children.Length == 0)
            {
                CurrentNode = node;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            MouseInput();
        }

        if (moving == false)
        {
            //Implement inputs and event-callbacks here
        }
        else
        {
            if (Vector3.Distance(transform.position, TargetNode.transform.position) > 0.25f)
            {
                transform.Translate(currentDir * speed * Time.deltaTime);
            }
            else
            {
                moving = false;
                CurrentNode = TargetNode;
            }
        }
    }

    //Implement mouse interaction method here
    
    public void MouseInput()
    {
        RaycastHit ray;

        if (Physics.Raycast(Input.mousePosition, Vector3.forward, out ray,10f))
        {
            // if object in UI which mouse is ver is tagged "Button"
            if(ray.collider.gameObject.tag == "Button")
            {
                Debug.Log("It's ya boy...uh...skinny benis");
            }
        }
    }

    // call the input directional method
    // invoke change colour event



    /// <summary>
    /// Sets the players target node and current directon to the specified node.
    /// </summary>
    /// <param name="node"></param>
    public void MoveToNode(Node node)
    {
        if (moving == false)
        {
            TargetNode = node;
            currentDir = TargetNode.transform.position - transform.position;
            currentDir = currentDir.normalized;
            moving = true;
        }
    }
}
