using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Events;

public class GameServer : MonoBehaviour
{
    class StartGameEvent : UnityEvent<int, int, string> { }
    class SendMoveEvent : UnityEvent<string, int, int> { }
    class GameData { public string gameId; public string suggestion; }
    private static string WEB_SERVER_URL = "http://127.0.0.1:5000/";

    private string gameId;
    private List<Color> colors = new List<Color>();
    private float minX, minY;

    [SerializeField]
    private Text gameIdText;

    [SerializeField]
    private GameObject blockPrefab;

    StartGameEvent startGameEvent = new StartGameEvent();
    SendMoveEvent sendMoveEvent;

    public void setData(int numberOfJobs, int numberOfMachines, string times, List<Color> colors, float minX, float minY)
    {
        startGameEvent.AddListener(StartGame);
        startGameEvent.Invoke(numberOfJobs, numberOfMachines, times);

        this.colors = colors;
        this.minX = minX;
        this.minY = minY;
    }

    void StartGame(int numberOfJobs, int numberOfMachines, string times) => StartCoroutine(StartGame_Coroutine(numberOfJobs, numberOfMachines, times));

    void DrawSuggestion(string suggestion) {
        string[] inds = suggestion.Split(' ');

        for (int i = 0; i < inds.Length; ++i)
        {
            var gameObject = GameObject.Instantiate(blockPrefab, new Vector3(minX + (2 + 2 * i) * blockPrefab.GetComponent<Renderer>().bounds.size.x, minY + blockPrefab.GetComponent<Renderer>().bounds.size.y, 0.0f), Quaternion.identity);
            gameObject.GetComponent<SpriteRenderer>().color = this.colors[Int32.Parse(inds[i])];
        }
    } 

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
                gameIdText.text = "Game ID: " + this.gameId;
                DrawSuggestion(gameData.suggestion);
            }
        }
    }

    void SendMove(string action, int jobId, int position) => StartCoroutine(SendMove_Coroutine(action, jobId, position));

    public void sendMove(string action, int jobId, int position)
    {
        sendMoveEvent = new SendMoveEvent();
        sendMoveEvent.AddListener(SendMove);
        sendMoveEvent.Invoke(action, jobId, position);
    }

    private IEnumerator SendMove_Coroutine(string action, int jobId, int position)
    {
        string uri = WEB_SERVER_URL + "move/" + this.gameId;

        WWWForm form = new WWWForm();
        form.AddField("action", action);
        form.AddField("jobId", jobId.ToString());
        form.AddField("index", position.ToString());

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
                DrawSuggestion(gameData.suggestion);
            }
        }
    }
}
