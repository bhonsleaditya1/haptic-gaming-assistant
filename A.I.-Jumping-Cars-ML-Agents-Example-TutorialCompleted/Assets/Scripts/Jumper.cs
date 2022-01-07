using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System.IO.Ports;
using System.Linq;

public class Jumper : Agent
{
    [SerializeField] private float jumpForce;
    [SerializeField] private KeyCode jumpKey;
    
    private bool jumpIsReady = true,recievedACK=false,sentData=false;
    private Rigidbody rBody;
    private Vector3 startingPosition;
    private int score = 0,i=0;
    SerialPort sp;
    string message;
    
    public event Action OnReset;

    void Start()
    {
        sp = new SerialPort("COM6",115200, Parity.None,8,StopBits.One);
        sp.DtrEnable = false;
        sp.ReadTimeout = 1;
        sp.WriteTimeout = 1;
        sp.Open();
        if(sp.IsOpen){
            sp.Write("Hello");
            Debug.Log("Unity C#: Hello");
        }
        else{
            Debug.LogError("Serial Port: "+sp.PortName+" is unavaliable");
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && sp.IsOpen){
            sp.Write("STOP");
            sp.Close();
        }
        try{
            string inData = sp.ReadLine();
            int inSize = inData.Length;
            if (inSize > 0)
            {
                Debug.Log("ARDUINO->|| " + inData + " ||MSG SIZE:" + inSize.ToString());
                if(sentData && inData.Contains(message)){
                    sentData = false;
                    recievedACK = true;                
                    Debug.Log("Received ACK");
                    AddJump();
                }
            }
            //Got the data. Flush the in-buffer to speed reads up.
            inSize = 0;
            sp.BaseStream.Flush();
            sp.DiscardInBuffer();
        }
        catch{
            Debug.Log("NO data");
        }
    }

    public override void Initialize()
    {
        rBody = GetComponent<Rigidbody>();
        startingPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if(sentData){
            if(jumpIsReady && recievedACK)
            RequestDecision();
        }else{
            if(jumpIsReady)
            RequestDecision();
        }
        
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        if (Mathf.FloorToInt(vectorAction[0]) == 1)
            Jump();
    }

    public override void OnEpisodeBegin()
    {
        Reset();
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = 0;
        
        if (Input.GetKey(jumpKey))
            actionsOut[0] = 1;
    }

    private void AddJump()
    {
        if(jumpIsReady && recievedACK)
        rBody.AddForce(new Vector3(0, jumpForce, 0), ForceMode.VelocityChange);
        jumpIsReady = false;
    }

    private void Jump()
    {
        if (!sentData && jumpIsReady)
        {
            switch (i)
            {
                case 0: message = "One"; break;
                case 1: message = "Two"; break;
                case 2: message = "Three"; break;
                case 3: message = "Four"; break;
            }
            i = (i+1)%4;
            if(sp.IsOpen){
                sp.Write(message);
                Debug.Log("Unity C#: " + message);
                sentData = true;
                recievedACK = false;
            }
            else{
                Debug.LogError("Serial Port: "+sp.PortName+" is unavaliable");
            }
        }
    }
    
    private void Reset()
    {
        score = 0;
        jumpIsReady = true;
        sentData = false;
        recievedACK=false;
        //Reset Movement and Position
        transform.position = startingPosition;
        rBody.velocity = Vector3.zero;
        OnReset?.Invoke();
    }

    private void OnCollisionEnter(Collision collidedObj)
    {
        if (collidedObj.gameObject.CompareTag("Street"))
            jumpIsReady = true;
        
        else if (collidedObj.gameObject.CompareTag("Mover") || collidedObj.gameObject.CompareTag("DoubleMover"))
        {
            AddReward(-1.0f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter(Collider collidedObj)
    {
        if (collidedObj.gameObject.CompareTag("score"))
        {
            AddReward(0.1f);
            score++;
            ScoreCollector.Instance.AddScore(score);
        }
    }
}
