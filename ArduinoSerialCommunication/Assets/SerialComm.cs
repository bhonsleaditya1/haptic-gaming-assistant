using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.Linq;
using System.Text;

public class SerialComm : MonoBehaviour
{
    SerialPort sp;
    public GameObject Control,Action;
    string message;

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

    // Update is called once per frame
    void Update()
    {
        CheckForReceivedData();
        if(Input.GetKeyDown(KeyCode.Escape) && sp.IsOpen){
            sp.Write("STOP");
            sp.Close();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            Control.transform.Translate(-0.5f,0,0);
            message = "Left";
            if(sp.IsOpen){
                sp.Write(message);
                Debug.Log("Unity C#: " + message);
            }
            else{
                Debug.LogError("Serial Port: "+sp.PortName+" is unavaliable");
            }           
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            Control.transform.Translate(0.5f,0,0);
            message = "Right";
            if(sp.IsOpen){
                sp.Write(message);
                Debug.Log("Unity C#: " + message);
            }
            else{
                Debug.LogError("Serial Port: "+sp.PortName+" is unavaliable");
            }           
        }
        if(Input.GetKeyDown(KeyCode.UpArrow)){
            Control.transform.Translate(0,0,0.5f);
            message = "Up";
            if(sp.IsOpen){
                sp.Write(message);
                Debug.Log("Unity C#: " + message);
            }
            else{
                Debug.LogError("Serial Port: "+sp.PortName+" is unavaliable");
            }           
        }
        if(Input.GetKeyDown(KeyCode.DownArrow)){
            Control.transform.Translate(0,0,-0.5f);
            message = "Down";
            if(sp.IsOpen){
                sp.Write(message);
                Debug.Log("Unity C#: " + message);
            }
            else{
                Debug.LogError("Serial Port: "+sp.PortName+" is unavaliable");
            }           
        }
    }

    void CheckForReceivedData(){
        try{
            string inData = sp.ReadLine();
            int inSize = inData.Count();
            if (inSize > 0)
            {
                Debug.Log("ARDUINO->|| " + inData + " ||MSG SIZE:" + inSize.ToString());
                if(inData == "Left"){
                    Action.transform.Translate(-0.5f,0,0);
                    Debug.Log("Moved Left");
                }
                if(inData =="Right"){
                    Action.transform.Translate(0.5f,0,0);
                    Debug.Log("Moved Right");
                }
                if(inData =="Up"){
                    Action.transform.Translate(0,0,0.5f);
                    Debug.Log("Moved Up");
                }
                if(inData =="Down"){
                    Action.transform.Translate(0,0,-0.5f);
                    Debug.Log("Moved Down");
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

    /*void ProcessData(byte[] inData){
       
        //byte[] data = inData.ToByte();

    }*/
}
