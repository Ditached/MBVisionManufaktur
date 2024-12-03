using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;


[Serializable]
public class Note
{
    public int chapter_index;
    [TextArea(3, 10)] // This allows for multiline text in the inspector
    public string text;
}

public class ModeratorNotes : MonoBehaviour
{
    public RectTransform scrollContent;
    // public StoryChapterSync storyChapterSync;
    public TMP_Text text;
    public TextAsset json_file;
    public List<Note> notes = new();

    public string jsonUrl = ""; // URL to fetch JSON from
    public float timeout = 5f; // Timeout in seconds

    private int lastChapterIndex = -2;

    private void Awake()
    {
        text.text = "";

/*
        var notes = JsonConvert.DeserializeObject<Note[]>(json_file.text);
        this.notes = notes.ToList(); 
*/
		//StartCoroutine(LoadNotes());

    }
    
    private IEnumerator LoadNotes()
    {
        if (!string.IsNullOrEmpty(jsonUrl))
        {
            using (UnityWebRequest request = UnityWebRequest.Get(jsonUrl))
            {
                request.timeout = Mathf.RoundToInt(timeout);
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    try
                    {

                        var notes = JsonConvert.DeserializeObject<Note[]>(request.downloadHandler.text);
                        this.notes = notes.ToList();
						Debug.Log("Loaded successfully!!");
                        text.text = "";
                        yield break; // Successfully loaded from URL, exit coroutine
                    }
                    catch (JsonException e)
                    {
                        Debug.LogWarning($"Failed to parse JSON from URL: {e.Message}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Failed to load JSON from URL: {request.error}");
                }
            }
        }

        // Fallback to local JSON file
        try
        {
            if (json_file != null)
            {
                var notes = JsonConvert.DeserializeObject<Note[]>(json_file.text);
                this.notes = notes.ToList();
                text.text = "";
                Debug.Log("Loaded notes from local JSON file");
            }
            else
            {
                Debug.LogError("No local JSON file assigned as fallback!");
                text.text = "Failed to load notes";
            }
        }
        catch (JsonException e)
        {
            Debug.LogError($"Failed to parse local JSON file: {e.Message}");
            text.text = "Failed to load notes";
        }
    }

    /*
    private void Update()
    {
        if(storyChapterSync.ChapterIndex.Value + 2 > notes.Count)
        {
            text.text = "No notes for this chapter";
            return;
        }
        else
        {
            if(lastChapterIndex != storyChapterSync.ChapterIndex.Value)
            {
                text.text = notes[storyChapterSync.ChapterIndex.Value + 1].text;
                scrollContent.transform.localPosition = Vector3.zero;
            } 
            
            lastChapterIndex = storyChapterSync.ChapterIndex.Value;
        }
    }
*/
    private void OnValidate()
    {
 #if UNITY_EDITOR
        if (json_file != null)
        {
            var notes = JsonConvert.DeserializeObject<Note[]>(json_file.text);
            this.notes = notes.ToList();
        }
        #endif
    }



}