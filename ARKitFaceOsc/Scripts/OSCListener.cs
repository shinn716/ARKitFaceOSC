using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OSCListener : MonoBehaviour
{
    public Transform head;
    public SkinnedMeshRenderer targetSkinnedMeshRenderer;
    public BlendshapesMapper blendshapesMapper;

    OSC osc = null;
    Dictionary<string, int> blendshapesList { get; set; } = new Dictionary<string, int>();

    // Start is called before the first frame update
    private IEnumerator Start()
    {
        blendshapesMapper.Init();
        blendshapesList = shinn.AR.Utils.GetBlendShapeNamesReturnDict(targetSkinnedMeshRenderer, true);

        yield return null;
        osc = OSCManager.instance.CurrentOsc;

        //foreach (var i in blendshapesMapper.dict)
        //    print(i.Key + "  " + i.Value);

        //foreach (var i in blendshapesList)
        //    print(i.Key + "  " + i.Value);

        osc.SetAllMessageHandler(OnReceive);
        osc.SetAddressHandler("/transform", OnReceiveTransform);
        osc.SetAddressHandler("/test", OnReceiveTest);
    }

    private void OnReceive(OscMessage message)
    {
        for (int i=0; i< blendshapesMapper.contents.Count; i++)
        {
            if(message.address == "/blendShape2." + blendshapesMapper.contents[i].blendshapesName.ToString())
            {
                string[] sliceName = message.address.Split('/');
                int index = shinn.AR.Utils.GetBlendShapeByIndex(blendshapesMapper.dict[sliceName[1]], blendshapesList, true);
                if (index != -1)
                {
                    float value = message.GetInt(0);
                    targetSkinnedMeshRenderer.SetBlendShapeWeight(index, value);
                }
            }
        }
    }

    private void OnReceiveTransform(OscMessage message)
    {
        Vector3 pos = new Vector3(message.GetFloat(0), message.GetFloat(1), message.GetFloat(2));
        Quaternion rot = new Quaternion(message.GetFloat(3), message.GetFloat(4), message.GetFloat(5), message.GetFloat(6));
        head.rotation = rot;
    }

    [ContextMenu("Test")]
    public void Test()
    {
        OscMessage message = new OscMessage();
        message.address = "/test";
        message.values.Add(456);
        osc.Send(message);
    }

    private void OnReceiveTest(OscMessage message)
    {
        print(message);
    }
}