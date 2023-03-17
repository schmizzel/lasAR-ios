using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class ARPLaserStart : MonoBehaviour
{
    public Material mat1, mat2;
    public bool laserActive;
    public Vector3 direction;
    public AudioSource laserSound;
    private float nextActionTime = 0.0f;
    public float periodSeconds = 0.1f;
    LaserNew beam;

    void Start() {}

    void Update(){
        if (laserActive) {
            Destroy(GameObject.Find("Laser Beam"));
            beam = new LaserNew(gameObject.transform.position, gameObject.transform.forward, mat1, mat2); 
        } else {
            Destroy(GameObject.Find("Laser Beam"));
        }

        if (Time.time > nextActionTime ) {
            nextActionTime = Time.time + periodSeconds;
            // TODO: Activate to use laser start from server
            // checkLaserRunning();
        }
    }

    public void SetLaserActive(bool input) {
        laserActive = input;
    }


    private void checkLaserRunning() {
        StartCoroutine(getRequest("http://192.168.1.1:3000/api/isStarted"));
    }

    IEnumerator getRequest(string uri) {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for result
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    bool newValue = Result.CreateFromJSON(webRequest.downloadHandler.text).isStarted;
                    if (newValue != laserActive) {
                        laserActive = newValue;
                    }                    
                    break;
            }
        }
    }
}

[System.Serializable]
class Result
{
    public bool isStarted;

    public static Result CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Result>(jsonString);
    }
}