/*using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;

public class SerialControler : MonoBehaviour
{
    public float speed = 5.0f;
    SerialPort serialPort;
    bool portOpen = false;

    void Start()
    {
        serialPort = new SerialPort("COM3",9600);
        serialPort.ReadTimeout = 50;

        try
        {
            serialPort.Open();
            portOpen = true;
            Debug.Log("Puerto abierto");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("" + ex.Message);
        }
    }

    void Update()
    {
        if(portOpen)
        {
            try
            {
                string[] data = serialPort.ReadLine().Trim().Split('|');
                float x = float.Parse(data[0]);
                float z = float.Parse(data[1]);

                Debug.Log($"X: {x} Z: {z}");

                Vector3 movement = new Vector3(x, 0, z) * speed * Time.deltaTime;
                this.transform.Translate(movement);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error en la lectura"+ex.Message);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colision con" + other.name);
        serialPort.Write("1");
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Sale de colision con" + other.name);
        serialPort.Write("0");
    }
<<<<<<< HEAD
}*/
=======
}
*/
>>>>>>> e8b21de3aed2599d6ad1a53574cd368905eb6e68
