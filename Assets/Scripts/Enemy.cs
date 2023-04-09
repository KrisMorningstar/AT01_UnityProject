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

    [SerializeField] private List<Node> unsearchedNodes;
    [SerializeField] private Node searchingNode;
    [SerializeField] private Node playerCurrent;
    [SerializeField] private Node playerTarget;

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
                else
                {
                    DepthFirstSearch();
                }
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
    public void DepthFirstSearch()
    {
        playerCurrent = GameManager.Instance.Player.CurrentNode;
        playerTarget = GameManager.Instance.Player.TargetNode;
        if(unsearchedNodes.Count < 1)
        {
            unsearchedNodes.Add(GameManager.Instance.Nodes[0]);
        }
        else
        {
            searchingNode = unsearchedNodes[unsearchedNodes.Count-1];
            if(searchingNode == playerCurrent || searchingNode == playerTarget)
            {
                Debug.Log("Found it");
                currentNode = searchingNode;
                currentDir = currentNode.transform.position - transform.position;
                currentDir = currentDir.normalized;
            }
            else if(searchingNode != playerCurrent && searchingNode != playerTarget)
            {
                unsearchedNodes.Remove(searchingNode);
                foreach(Node child in searchingNode.Children)
                {
                    unsearchedNodes.Add(child);
                }
            }
        }
    }

}
