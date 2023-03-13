using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Tooltip("Movement speed modifier.")]
    [SerializeField] private float speed = 3;
    private Node currentNode;
    private Vector3 currentDir;
    private bool playerCaught = false;

    public delegate void GameEndDelegate();
    public event GameEndDelegate GameOverEvent = delegate { };

    // Start is called before the first frame update
    void Start()
    {
        InitializeAgent();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCaught == false)
        {
            if (currentNode != null)
            {
                //If within 0.25 units of the current node.
                if (Vector3.Distance(transform.position, currentNode.transform.position) > 0.25f)
                {
                    transform.Translate(currentDir * speed * Time.deltaTime);
                }
                //Implement path finding here
            }
            else
            {
                Debug.LogWarning($"{name} - No current node");
            }

            Debug.DrawRay(transform.position, currentDir, Color.cyan);
        }
    }

    //Called when a collider enters this object's trigger collider.
    //Player or enemy must have rigidbody for this to function correctly.
    private void OnTriggerEnter(Collider other)
    {
        if (playerCaught == false)
        {
            if (other.tag == "Player")
            {
                playerCaught = true;
                GameOverEvent.Invoke(); //invoke the game over event
            }
        }
    }

    /// <summary>
    /// Sets the current node to the first in the Game Managers node list.
    /// Sets the current movement direction to the direction of the current node.
    /// </summary>
    void InitializeAgent()
    {
        currentNode = GameManager.Instance.Nodes[0];
        currentDir = currentNode.transform.position - transform.position;
        currentDir = currentDir.normalized;
    }

    //Implement DFS algorithm method here

    // VARIABLE for node currently being searched
    // BOOLEAN for targetfound
    // LIST of type "node" storing unsearched nodes (this is the stack)

    // set targetfound to false

    // set GameManager.Instance.nodes[0] to unsearched nodes
    
    // LOOP STARTS HERE
    // WHILE targetfound is false, continue the loop

    // 1. take last item in unsearched list and assign it to node currently being searched
    
    // 2. check if node currently being searched is the same as either:
        //the target node of the player
        //the current node of the player
    // IF this is true (the node being search is the target):
        // assign node being searched as currentNode
        // break the loop and continue
    // IF not true continue loop

    // 3. use a loop to add children of node being searched to unsearched nodes

    // 4. remove node currently being searched from unsearched nodes list

    // 5. return to start of loop





    // access the nodes on gamemanager
    // add GameManager.Instance.Nodes[0] to a list of unsearched nodes (root node)
    // check if root node is the same as GameManager.Instance.Player.TargetNode/CurrentNode
    // 74 if it is the same return that as the new destination for this enemy
    // add the children of the node being searched to the list of unsearched nodes
    // remove the node being searched from list of unsearched nodes
    // assign the node at the "top" (last position of the unsearched list) as the node being searched
    // go back to line 74
}
