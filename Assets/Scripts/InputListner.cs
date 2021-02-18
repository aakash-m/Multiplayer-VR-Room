using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class InputListner : MonoBehaviour
{
    List<InputDevice> inputDevices;

    InputDeviceCharacteristics deviceCharacteristic;

    public XRNode controllerNode;

    private void Awake()
    {
        inputDevices = new List<InputDevice>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //InputDevices.GetDevices(inputDevices);
        deviceCharacteristic = InputDeviceCharacteristics.HeldInHand | 
                               InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left;
        //InputDevices.GetDevicesWithCharacteristics(deviceCharacteristic, inputDevices);
        InputDevices.GetDevicesAtXRNode(controllerNode, inputDevices);

        foreach(InputDevice inputDevice in inputDevices)
        {
            Debug.Log("Device found with name:" + inputDevice.name);
            bool inputValue;
            if(inputDevice.TryGetFeatureValue(CommonUsages.primaryButton,out inputValue) && inputValue)
            {
                Debug.Log("You pressed the primary button");
            }
        }
    }
}
