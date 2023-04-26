using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    // UI
    public GameObject pauseScreen;

    [SerializeField]GraphicRaycaster _graphicRaycaster;
    PointerEventData _pointerEventData;
    EventSystem _eventSystem;

    [SerializeField] private GameObject directionParent;
    private GameObject upArrow;
    private GameObject downArrow;
    private GameObject leftArrow;
    private GameObject rightArrow;
    private Color dpadColour;


    // MOVEMENT
    private List<Vector3> scanDirections = new List<Vector3>();
    public LayerMask nodeMask;
    private GameObject upNode;
    private GameObject downNode;
    private GameObject leftNode;
    private GameObject rightNode;

    private bool directionsChecked;
    private bool inputPressed = false;
    private bool moving = false;
    [SerializeField] private float speed = 4;
    private Vector3 currentDir;


    // INPUTS
    private PlayerInputActions playerInputActions;
    private InputAction movement;
    private InputAction pause;


    // EVENTS & DELEGATES
    public delegate void PauseGameDelegate();
    public event PauseGameDelegate PauseGameEvent = delegate { };


    // NODES
    public Node CurrentNode { get; private set; }
    public Node TargetNode { get; private set; }



    private void Awake()
    {
        //Debug.Log("awake bool");  //-- For Testing

        // PLAYER INPUTS
        playerInputActions = new PlayerInputActions();
        movement = playerInputActions.Player.Movement;
        movement.Enable();
        pause = playerInputActions.Player.Pause;
        pause.Enable();


        // DIRECTIONS
        scanDirections.Add(Vector3.forward);
        scanDirections.Add(Vector3.back);
        scanDirections.Add(Vector3.left);
        scanDirections.Add(Vector3.right);


        // UI ARROWS
        upArrow = directionParent.transform.Find("Up").gameObject;
        downArrow = directionParent.transform.Find("Down").gameObject;
        leftArrow = directionParent.transform.Find("Left").gameObject;
        rightArrow = directionParent.transform.Find("Right").gameObject;

        dpadColour = upArrow.gameObject.GetComponent<RawImage>().color;
    }



    // Start is called before the first frame update
    void Start()
    {

        _eventSystem = GetComponent<EventSystem>();
        directionsChecked = false;
        ResetDirections();

        foreach(Vector3 dir in scanDirections)
        {
            DirectionScan(dir);
            directionsChecked = true;
        }

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
        if (pause.WasPressedThisFrame())
        {
            PauseGameEvent.Invoke();
        }

        if (moving == false)
        {
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
                //Debug.Log("update true"); //-- For Testing
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
                //Debug.Log("reset");   //-- For Testing
                moving = false;
                inputPressed = false;
                CurrentNode = TargetNode;
            }
        }
    }



    public void ResetDirections()
    {
        //Debug.Log("resetting");   //-- For Testing
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

        if(Physics.Raycast(transform.position, direction, out hit, 10f, nodeMask))
        {
            // Debug.Log(hit.collider.gameObject.name); //-- For Testing
            switch (direction)
            {
                case var value when value == Vector3.forward:
                    // Debug.Log("Can Go Forward");     //-- For Testing
                    dpadColour.a = 1f;
                    upArrow.GetComponent<RawImage>().color = dpadColour;
                    upNode = hit.collider.gameObject;
                    break;
                case var value when value == Vector3.back:
                    // Debug.Log("Can Go backwards");   //-- For Testing
                    dpadColour.a = 1f;
                    downArrow.GetComponent<RawImage>().color = dpadColour;
                    downNode = hit.collider.gameObject;
                    break;
                case var value when value == Vector3.left:
                    // Debug.Log("Can Go left");        //-- For Testing
                    dpadColour.a = 1f;
                    leftArrow.GetComponent<RawImage>().color = dpadColour;
                    leftNode = hit.collider.gameObject;
                    break;
                case var value when value == Vector3.right:
                    // Debug.Log("Can Go right");       //-- For Testing
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
        if (_graphicRaycaster == null)
        {
            Debug.Log("Null Raycaster");
        }
        _graphicRaycaster.Raycast(_pointerEventData, results);
        if (results == null)
        {
            Debug.Log("No Results Found");
        }

        foreach (RaycastResult result in results)
        {
            //Debug.Log("Hit " + result.gameObject.name);   //-- For Testing
            switch (result.gameObject)
            {
                case var value when value == upArrow:
                    //Debug.Log("moving forward");  //-- For Testing
                    MoveToNode(upNode.GetComponent<Node>());
                    StartCoroutine(Flash(upArrow));
                    break;
                case var value when value == downArrow:
                    //Debug.Log("moving back");     //-- For Testing
                    MoveToNode(downNode.GetComponent<Node>());
                    StartCoroutine(Flash(downArrow));
                    break;
                case var value when value == leftArrow:
                    //Debug.Log("moving left");     //-- For Testing
                    MoveToNode(leftNode.GetComponent<Node>());
                    StartCoroutine(Flash(leftArrow));
                    break;
                case var value when value == rightArrow:
                    //Debug.Log("moving right");    //-- For Testing
                    MoveToNode(rightNode.GetComponent<Node>());
                    StartCoroutine(Flash(rightArrow));
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
                    StartCoroutine(Flash(upArrow));
                    break;
                }
                else
                {
                    //Debug.Log("Null");    //-- For Testing
                    inputPressed = false;
                }
                break;
            case var value when value == Vector2.down:
                if (downNode != null)
                {
                    MoveToNode(downNode.GetComponent<Node>());
                    StartCoroutine(Flash(downArrow));
                    break;
                }
                else
                {
                    //Debug.Log("Null");    //-- For Testing
                    inputPressed = false;
                }
                break;
            case var value when value == Vector2.left:
                if (leftNode != null)
                {
                    MoveToNode(leftNode.GetComponent<Node>());
                    StartCoroutine(Flash(leftArrow));
                    break;
                }
                else
                {
                    //Debug.Log("Null");    //-- For Testing
                    inputPressed = false;
                }
                break;
            case var value when value == Vector2.right:
                if (rightNode != null)
                {
                    MoveToNode(rightNode.GetComponent<Node>());
                    StartCoroutine(Flash(rightArrow));
                    break;
                }
                else
                {
                    //Debug.Log("Null");    //-- For Testing
                    inputPressed = false;
                }
                break;
            default:
                Debug.LogWarning("Default: Input Error");  // Failsafe. Shouldn't be possible.
                inputPressed = false;
                break;
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



    IEnumerator Flash(GameObject _arrow)
    {
        Color _colour;
        _colour = _arrow.GetComponent<RawImage>().color;

        _arrow.GetComponent<RawImage>().color = Color.green;
        yield return new WaitForSeconds(.05f);

        _arrow.GetComponent<RawImage>().color = dpadColour;

        yield return null;
    }
}
