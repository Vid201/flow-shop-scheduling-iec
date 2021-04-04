using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class RequestsTest : MonoBehaviour
{
    UnityEvent unityEvent = new UnityEvent();
    UnityEvent unityEvent2 = new UnityEvent();

    void Start()
    {
        unityEvent.AddListener(GetData);
        unityEvent.Invoke();

        unityEvent2.AddListener(PostData);
        unityEvent2.Invoke();
    }

    void GetData() => StartCoroutine(GetData_Coroutine());

    void PostData() => StartCoroutine(PostData_Coroutine());

    IEnumerator GetData_Coroutine() {
        string uri = "https://jsonplaceholder.typicode.com/posts/1";
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError) {
                Debug.Log("Network error!");
            } else
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }

    IEnumerator PostData_Coroutine() {
        string uri = "https://httpbin.org/post";
        WWWForm form = new WWWForm();
        form.AddField("title", "test data");
        using (UnityWebRequest request = UnityWebRequest.Post(uri, form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("Network error!");
            }
            else
            {
                Debug.Log(request.downloadHandler.text);
            }
        }
    }
}
