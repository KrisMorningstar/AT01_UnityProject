using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    [SerializeField]GraphicRaycaster _raycaster;
    PointerEventData _pointerEventData;
    EventSystem _eventSystem;
    private List<Vector3> scanDirections = new List<Vector3>();
    public LayerMask layerMask;

    // NEEDS CLEANING
    private bool directionsChecked;

    [SerializeField]private GameObject directionParent;
    private GameObject upArrow;
    private GameObject downArrow;
    private GameObject leftArrow;
    private GameObject rightArrow;
    private Color uColour;
    private Color dColour;
    private Color lColour;
    private Color rColour;

    [SerializeField] private GameObject upNode;
    [SerializeField] private GameObject downNode;
    [SerializeField] private GameObject leftNode;
    [SerializeField] private GameObject rightNode;
    // END OF UGLY

    //Define delegate types and events here

    public Node CurrentNode { get; private set; }
    public Node TargetNode { get; private set; }

    [SerializeField] private float speed = 4;
    private bool moving = false;
    private Vector3 currentDir;

    private void Awake()
    {
        // UGLY, CLEAN UP
        scanDirections.Add(Vector3.forward);
        scanDirections.Add(Vector3.back);
        scanDirections.Add(Vector3.left);
        scanDirections.Add(Vector3.right);

        upArrow = directionParent.transform.Find("Up").gameObject;
        downArrow = directionParent.transform.Find("Down").gameObject;
        leftArrow = directionParent.transform.Find("Left").gameObject;
        rightArrow = directionParent.transform.Find("Right").gameObject;

        uColour = upArrow.gameObject.GetComponent<RawImage>().color;
        dColour = downArrow.gameObject.GetComponent<RawImage>().color;
        lColour = leftArrow.gameObject.GetComponent<RawImage>().color;
        rColour = rightArrow.gameObject.GetComponent<RawImage>().color;
        // END OF UGLY
    }

    // Start is called before the first frame update
    void Start()
    {
        directionsChecked = false;
        ResetDirections();

        foreach(Vector3 dir in scanDirections)
        {
            DirectionScan(dir);
            directionsChecked = true;
        }

        _eventSystem = GetComponent<EventSystem>();

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
        //Debug.DrawLine(transform.position, (transform.position + new Vector3(10,0,0)), Color.blue);

        if (Input.GetMouseButtonDown(0))
        {
            MouseTest();
        }

        if (moving == false)
        {
            //Implement inputs and event-callbacks here
            if (!directionsChecked)
            {
                foreach (Vector3 dir in scanDirections)
                {
                    DirectionScan(dir);
                    directionsChecked = true;
                }
            }
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
        
    }

    // call the input directional method
    // invoke change colour event

    public void ResetDirections()
    {
        Debug.Log("resetting");
        
        uColour.a = .5f;
        dColour.a = .5f;
        lColour.a = .5f;
        rColour.a = .5f;
        upArrow.GetComponent<RawImage>().color = uColour;
        downArrow.GetComponent<RawImage>().color = dColour;
        leftArrow.GetComponent<RawImage>().color = lColour;
        rightArrow.GetComponent<RawImage>().color = rColour;

        upNode = null;
        downNode = null;
        leftNode = null;
        rightNode = null;

        directionsChecked = false;
    }

    public void DirectionScan(Vector3 direction)
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, direction, out hit, 10f, layerMask))
        {
            //Debug.Log(hit.collider.gameObject.name);
            switch (direction)
            {
                case var value when value == Vector3.forward:
                    Debug.Log("Can Go Forward");
                    uColour.a = 1f;
                    upArrow.GetComponent<RawImage>().color = uColour;
                    upNode = hit.collider.gameObject;
                    break;
                case var value when value == Vector3.back:
                    Debug.Log("Can Go backwards");
                    dColour.a = 1f;
                    downArrow.GetComponent<RawImage>().color = dColour;
                    downNode = hit.collider.gameObject;
                    break;
                case var value when value == Vector3.left:
                    Debug.Log("Can Go left");
                    lColour.a = 1f;
                    leftArrow.GetComponent<RawImage>().color = lColour;
                    leftNode = hit.collider.gameObject;
                    break;
                case var value when value == Vector3.right:
                    Debug.Log("Can Go right");
                    rColour.a = 1f;
                    rightArrow.GetComponent<RawImage>().color = rColour;
                    rightNode = hit.collider.gameObject;
                    break;

            }
        }
    }

    public void MouseTest()
    {
        _pointerEventData = new PointerEventData(_eventSystem);
        _pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        if (_pointerEventData == null)
        {
            Debug.Log("Pointer is fucked dawg");
        }
        if (_raycaster == null)
        {
            Debug.Log("raycaster is fucked?");
        }
        if (results == null)
        {
            Debug.Log("No Results, Duh");
        }
        _raycaster.Raycast(_pointerEventData, results);
        if (results == null)
        {
            Debug.Log("uh oh fucky wucky");
        }

        foreach (RaycastResult result in results)
        {
            Debug.Log("Hit " + result.gameObject.name);
            switch (result.gameObject)
            {
                case var value when value == upArrow:
                    Debug.Log("moving forward");
                    MoveToNode(upNode.GetComponent<Node>());
                    break;
                case var value when value == downArrow:
                    Debug.Log("moving back");
                    MoveToNode(downNode.GetComponent<Node>());
                    break;
                case var value when value == leftArrow:
                    Debug.Log("moving left");
                    MoveToNode(leftNode.GetComponent<Node>());
                    break;
                case var value when value == rightArrow:
                    Debug.Log("moving right");
                    MoveToNode(rightNode.GetComponent<Node>());
                    break;
            }
        }
    }

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
            ResetDirections();
            moving = true;
        }
    }
}
