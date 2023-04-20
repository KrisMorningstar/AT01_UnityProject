using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]GraphicRaycaster _raycaster;
    PointerEventData _pointerEventData;
    EventSystem _eventSystem;
    private List<Vector3> scanDirections = new List<Vector3>();
    public LayerMask layerMask;

    // NEEDS CLEANING
    [SerializeField] private bool inputPressed;
    private bool directionsChecked;
    private Color dpadColour;

    [SerializeField]private GameObject directionParent;
    private GameObject upArrow;
    private GameObject downArrow;
    private GameObject leftArrow;
    private GameObject rightArrow;

    [SerializeField] private GameObject upNode;
    [SerializeField] private GameObject downNode;
    [SerializeField] private GameObject leftNode;
    [SerializeField] private GameObject rightNode;
    // END OF UGLY

    //Define delegate types and events here
    private PlayerInputActions playerInputActions;
    private InputAction movement;

    public Node CurrentNode { get; private set; }
    public Node TargetNode { get; private set; }

    [SerializeField] private float speed = 4;
    [SerializeField] private bool moving = false;
    private Vector3 currentDir;

    private void Awake()
    {
        Debug.Log("awake bool");
        inputPressed = false;

        playerInputActions = new PlayerInputActions();
        movement = playerInputActions.Player.Movement;
        movement.Enable();

        // UGLY, CLEAN UP
        scanDirections.Add(Vector3.forward);
        scanDirections.Add(Vector3.back);
        scanDirections.Add(Vector3.left);
        scanDirections.Add(Vector3.right);

        upArrow = directionParent.transform.Find("Up").gameObject;
        downArrow = directionParent.transform.Find("Down").gameObject;
        leftArrow = directionParent.transform.Find("Left").gameObject;
        rightArrow = directionParent.transform.Find("Right").gameObject;

        dpadColour = upArrow.gameObject.GetComponent<RawImage>().color;
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

            if (Input.GetMouseButtonDown(0))
            {
                MouseInput();
            }

            if (!inputPressed && movement.IsPressed())
            {
                Debug.Log("update true");
                inputPressed = true;
                InputResult(movement.ReadValue<Vector2>());
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
                Debug.Log("reset");
                inputPressed = false;
                CurrentNode = TargetNode;
            }
        }
    }


    public void ResetDirections()
    {
        Debug.Log("resetting");
        
        dpadColour.a = .5f;
        upArrow.GetComponent<RawImage>().color = dpadColour;
        downArrow.GetComponent<RawImage>().color = dpadColour;
        leftArrow.GetComponent<RawImage>().color = dpadColour;
        rightArrow.GetComponent<RawImage>().color = dpadColour;

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
            // Debug.Log(hit.collider.gameObject.name); -- For Testing
            switch (direction)
            {
                case var value when value == Vector3.forward:
                    // Debug.Log("Can Go Forward");     -- For Testing
                    dpadColour.a = 1f;
                    upArrow.GetComponent<RawImage>().color = dpadColour;
                    upNode = hit.collider.gameObject;
                    break;
                case var value when value == Vector3.back:
                    // Debug.Log("Can Go backwards");   -- For Testing
                    dpadColour.a = 1f;
                    downArrow.GetComponent<RawImage>().color = dpadColour;
                    downNode = hit.collider.gameObject;
                    break;
                case var value when value == Vector3.left:
                    // Debug.Log("Can Go left");        -- For Testing
                    dpadColour.a = 1f;
                    leftArrow.GetComponent<RawImage>().color = dpadColour;
                    leftNode = hit.collider.gameObject;
                    break;
                case var value when value == Vector3.right:
                    // Debug.Log("Can Go right");       -- For Testing
                    dpadColour.a = 1f;
                    rightArrow.GetComponent<RawImage>().color = dpadColour;
                    rightNode = hit.collider.gameObject;
                    break;

            }
        }
    }

    // call the input directional method
    // invoke change colour event
    public void MouseInput()
    {
        _pointerEventData = new PointerEventData(_eventSystem);
        _pointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();

        if (_pointerEventData == null)
        {
            Debug.Log("Null Pointer Error");
        }
        if (_raycaster == null)
        {
            Debug.Log("Null Raycaster");
        }
        _raycaster.Raycast(_pointerEventData, results);
        if (results == null)
        {
            Debug.Log("No Results Found");
        }

        foreach (RaycastResult result in results)
        {
            Debug.Log("Hit " + result.gameObject.name);
            switch (result.gameObject)
            {
                case var value when value == upArrow:
                    // Debug.Log("moving forward"); -- For Testing
                    MoveToNode(upNode.GetComponent<Node>());
                    break;
                case var value when value == downArrow:
                    // Debug.Log("moving back");    -- For Testing
                    MoveToNode(downNode.GetComponent<Node>());
                    break;
                case var value when value == leftArrow:
                    // Debug.Log("moving left");    -- For Testing
                    MoveToNode(leftNode.GetComponent<Node>());
                    break;
                case var value when value == rightArrow:
                    // Debug.Log("moving right");   -- For Testing
                    MoveToNode(rightNode.GetComponent<Node>());
                    break;
            }
        }
    }

    public void InputResult(Vector2 _input)
    {
        switch (_input)
        {
            case var value when value == Vector2.up:
                if(upNode != null)
                {
                    MoveToNode(upNode.GetComponent<Node>());
                    break;
                }
                else
                {
                    Debug.Log("Null");
                    inputPressed = false;
                }
                break;
            case var value when value == Vector2.down:
                if (downNode != null)
                {
                    MoveToNode(downNode.GetComponent<Node>());
                    break;
                }
                else
                {
                    Debug.Log("Null");
                    inputPressed = false;
                }
                break;
            case var value when value == Vector2.left:
                if (leftNode != null)
                {
                    MoveToNode(leftNode.GetComponent<Node>());
                    break;
                }
                else
                {
                    Debug.Log("Null");
                    inputPressed = false;
                }
                break;
            case var value when value == Vector2.right:
                if (rightNode != null)
                {
                    MoveToNode(rightNode.GetComponent<Node>());
                    break;
                }
                else
                {
                    Debug.Log("Null");
                    inputPressed = false;
                }
                break;
            //TEST A DEFAULT INSTEAD OF ELSES
        }
    }

    public void NullResult()
    {
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
