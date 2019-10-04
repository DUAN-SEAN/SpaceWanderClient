using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Game.Actor;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MicroPhoneTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (var device in Microphone.devices)
        {
            _deviceName = device;


            Microphone.GetDeviceCaps(device,out var minFreq,out var maxFreq);
            Debug.Log("Name: " + device);
            Debug.Log("minFreq = "+minFreq);
            Debug.Log("maxFreq = " + maxFreq);

        }

        _audioSource = GetComponent<AudioSource>();

        var audioClip1 = Resources.Load<AudioClip>("test");

        {
            float[] data = new float[audioClip1.samples * audioClip1.channels];


            

            Debug.Log(audioClip1.samples);
            Debug.Log(audioClip1.channels);

            audioClip1.GetData(data, 0);
           
            AudioBlock audioBlock = new AudioBlock();
            audioBlock.data = data;
            audioBlock.channels = audioClip1.channels;
            audioBlock.samples = audioClip1.samples;
            audioBlock.frequency = audioClip1.frequency;

            MemoryStream memoryStream = new MemoryStream();
            _binaryFormatter.Serialize(memoryStream, audioBlock);

            //Debug.Log("序列化后的大小 = "+sizeof(byte)* memoryStream.ToArray().Length);
            memoryStream.Seek(0, SeekOrigin.Begin);


            var m2 =  new MemoryStream(memoryStream.ToArray());

            var audioBlock2 =  _binaryFormatter.Deserialize(m2) as AudioBlock;
            


            audioClip1 = AudioClip.Create("漆黑", audioBlock2.samples, audioBlock2.channels, audioBlock2.frequency, false);
            if (audioBlock.Equals(audioBlock2))
            {
                Debug.Log("前后相同");
            }
            else
            {
                if(audioBlock.data.Length!=audioBlock2.data.Length)
                    Debug.Log("序列化data长度不同");
            }
            audioClip1.SetData(audioBlock2.data, 0);
            AudioSource.PlayClipAtPoint(audioClip1,Vector3.zero);
            _audioSource.clip = audioClip1;
            _audioSource.clip.name = "漆黑";
            ////foreach (float f in data)
            ////{
            ////    Debug.Log(f);
            ////}
            //_audioSource.clip.SetData(data, 0);
        }
        Debug.Log("音频加载完毕");


        

        //StartRecord();
    }
    void Update()
    {

    }
    public IEnumerator LoadStreamingAssets(string url, Action<Byte[]> action = null)
    {
        UnityWebRequest wr = new UnityWebRequest(url);
        DownloadHandlerBuffer bufferHandler = new DownloadHandlerBuffer();
        wr.downloadHandler = bufferHandler;
        yield return wr.SendWebRequest();
        if (!wr.isNetworkError)
        {
            action?.Invoke(bufferHandler.data);
        }

    }

    public void StartRecord()
    {
        var button = Resources.Load<GameObject>("ButtonRecord");
        button = Instantiate(button);

        button.transform.parent = GameObject.Find("Canvas").transform;

        var buttonC =  button.GetComponent<Button>();
        buttonC.onClick.AddListener(() => { StopRecord(); });

       
        _audioSource.clip = Microphone.Start(_deviceName, false, 4, 44100);
        _audioSource.clip.name = "录音";

    }
    public void StopRecord()
    {
        Microphone.End(_deviceName);

    }

    public void LoadAndPlayFromResource(string name)
    {
        var clip = (AudioClip)Resources.Load(name, typeof(AudioClip));

        _audioSource.clip = clip;
        _audioSource.Play();
    }

    private void UpLoadAudio(AudioClip clip)
    {
        float[] data = new float[clip.channels*clip.samples];
        
    }
    // Update is called once per frame
    private AudioClip _clip;
    private string _deviceName;
    private AudioSource _audioSource;

    private BinaryFormatter _binaryFormatter = new BinaryFormatter();

}




