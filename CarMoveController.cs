using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
public class CarMoveController : NetworkBehaviour {
    private float horizontal;
    private float vertical;
    public float speed=10000;
    private Rigidbody rg;

    [SyncVar]
    public bool isReady = false;

    [Command]
    private void CmdsetIsReady(bool r)
    {
        isReady = r;
    }
    
    private float timer=0;
    private bool isGrouned=true;
    private Queue<Vector3> queue;
    private Queue<Vector3> queue2;
    private Transform originPos;
    public Text error;
    public Text no;
    private GameObject gameController;
    private Color[] col = { Color.black, Color.blue, Color.red, Color.green, Color.yellow, Color.white, Color.gray, Color.grey };
    // Start is called before the first frame update
    public override void OnStartLocalPlayer()
    {
        Camera.main.transform.SetParent(transform);
        Camera.main.transform.localPosition = Vector3.zero;
        Camera.main.transform.localPosition = new Vector3(0, 3f, -4.8f);
        Camera.main.transform.localRotation  =Quaternion.Euler( new Vector3(13.01f, 0, 0));
        GameObject.Find("MapCamera").GetComponent<CameraController>().carPos = transform;
        originPos = GameObject.Find("OriginPos").transform;
        originPos.position = new Vector3(originPos.position.x, originPos.position.y, originPos.position.z - 1);
       
    }
    void Start()
    {
        gameController = GameObject.Find("GameManager");
        gameController.GetComponent<Gmanager>().addCar(transform.gameObject);
        rg = GetComponent<Rigidbody>();
        queue = new Queue<Vector3>();
        queue2 = new Queue<Vector3>();
        rg.centerOfMass += new Vector3(0.1f, 0.5f, 0);
        //rg.centerOfMass = new Vector3(rg.centerOfMass.x, -1.5F, rg.centerOfMass.z);
    }
    

    // Update is called once per frame
    void Update()
    {
        no.color = col[Random.Range(0, 7)];
        if (!isLocalPlayer)
        {
            return;
        }
        if (Input.GetButtonDown("Jump"))
        {
            CmdsetIsReady(true);
        }
        bool canStart = gameController.GetComponent<Gmanager>().startGame;
        if (!canStart)
        {
            rg.velocity = Vector3.zero;
            rg.angularVelocity = Vector3.zero;
            return;
        }
        setIsGrouned();      
        if (isGrouned)
        {
            judgeDir();
            queue.Enqueue(transform.position);
            queue2.Enqueue(transform.rotation.eulerAngles);
            if (queue.Count > 105)
            {
                queue.Dequeue();
            }
            if (queue2.Count > 105)
            {
                queue2.Dequeue();
            }
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            if (Mathf.Abs(Vector3.Dot(transform.forward.normalized, rg.velocity)) > 0 && Mathf.Abs(horizontal)>0.1f)
            {
                rg.angularVelocity = Vector3.zero;
                if (Vector3.Dot(rg.velocity, transform.forward) > 0)
                {                   
                    transform.Rotate(new Vector3(0, horizontal, 0));
                }
                else
                {
                    transform.Rotate(new Vector3(0,  -horizontal, 0));
                }
            }
            if (Input.GetKey(KeyCode.W))
            {
                if (Vector3.Dot(transform.forward.normalized,rg.velocity)< 40)
                {
                    rg.AddForce(transform.forward * 10000, ForceMode.Force);
                }
                else
                {
                    rg.velocity = new Vector3(rg.velocity.x, rg.velocity.y, rg.velocity.z);
                }

            }
            if (Input.GetKey(KeyCode.S))
            {
                if (Vector3.Dot(transform.forward.normalized, rg.velocity) >-20)
                {
                    rg.AddForce(-transform.forward * 10000, ForceMode.Force);
                }
                else
                {
                    rg.velocity = new Vector3(rg.velocity.x, rg.velocity.y, rg.velocity.z);
                }
            }
        }     
        if (!isGrouned)
        {
            timer += Time.deltaTime;
        }
        if (timer >= 3f)
        {
            timer = 0;
            reSetPosition();
        }

        if ((transform.eulerAngles.x > 0 && transform.eulerAngles.x < 360) || (transform.eulerAngles.z > 0 && transform.eulerAngles.z < 360))
        {
            Vector3 originAngel = new Vector3(0, transform.eulerAngles.y, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(originAngel), Time.deltaTime);
           // transform.rotation=Quaternion.FromToRotation(transform.rotation.eulerAngles, originAngel);

        }
    }

    

    private void reSetPosition()
    {
        rg.velocity = Vector3.zero;
        rg.angularVelocity = Vector3.zero;
        Vector3 tmp = queue.Dequeue();
        Vector3 tmp2 = queue2.Dequeue();
        transform.position = tmp;
        tmp2.x = 0;
        tmp2.z = 0;
        transform.rotation = Quaternion.Euler(tmp2);
        isGrouned = true;
    }
    private void setIsGrouned()
    {
        Ray myray = new Ray(transform.position+new Vector3(0,1,0), -transform.up);
        RaycastHit hit;
        if(Physics.Raycast(myray, out hit, 2f))
        {
            isGrouned = true;
            timer = 0;
        }
        else
        {
            isGrouned = false;
        }
    }

    private void judgeDir()
    {
        Vector3 tmp = transform.position-originPos.position;
        if (Vector3.Dot(transform.forward, tmp) < 0)
        {
            error.enabled = true;
        }
        else
        {
            error.enabled = false;
        }
    }
    public void setNo(int non)
    {
        no.text = "µÚ" + non.ToString() + "Ãû";
        no.enabled = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "End")
        {
            if (!gameController.GetComponent<Gmanager>().inList(transform.gameObject.name))
            {
                gameController.GetComponent<Gmanager>().addList(transform.gameObject.name);
                gameController.GetComponent<Gmanager>().CmdchangeNo(gameController.GetComponent<Gmanager>().getno() + 1);
                if (isLocalPlayer)
                {
                    setNo(gameController.GetComponent<Gmanager>().getno());
                }
            }
           
        }
    }
}

