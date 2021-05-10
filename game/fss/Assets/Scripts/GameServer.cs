using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

public class GameServer : MonoBehaviour
{
    class StartGameEvent : UnityEvent<int, int, string> { }
    class GameData { public int gameId; }
    private static string WEB_SERVER_URL = "http://127.0.0.1:5000/";

    private int gameId;

    StartGameEvent startGameEvent = new StartGameEvent();
    UnityEvent unityEvent2 = new UnityEvent();

    public void setData(int numberOfJobs, int numberOfMachines, string times)
    {
        startGameEvent.AddListener(StartGame);
        startGameEvent.Invoke(numberOfJobs, numberOfMachines, times);
    }

    void StartGame(int numberOfJobs, int numberOfMachines, string times) => StartCoroutine(StartGame_Coroutine(numberOfJobs, numberOfMachines, times));

    IEnumerator StartGame_Coroutine(int numberOfJobs, int numberOfMachines, string times)
    {
        string uri = WEB_SERVER_URL + "game";

        WWWForm form = new WWWForm();
        form.AddField("numberOfJobs", numberOfJobs);
        form.AddField("numberOfMachines", numberOfMachines);
        form.AddField("times", times);

        using (UnityWebRequest request = UnityWebRequest.Post(uri, form))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("Network error!");
            }
            else
            {
                GameData gameData = JsonUtility.FromJson<GameData>(request.downloadHandler.text);
                this.gameId = gameData.gameId;
            }
        }
    }

    public void sendMove(int jobId, int position)
    {
        unityEvent2.AddListener(delegate { SendMove_Coroutine(jobId, position); });
        unityEvent2.Invoke();
    }

    private IEnumerator SendMove_Coroutine(int jobId, int position)
    {
        string uri = WEB_SERVER_URL + "move/" + this.gameId.ToString();
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        {
            yield return request.SendWebRequest();
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("Network error!");
            }
            else
            {

            }
        }
    }
}
