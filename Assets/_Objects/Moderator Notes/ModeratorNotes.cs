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
    
    private void LoadNotes()
    {
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

    public ChipState chipState;

    
    private void Update()
    {
        var index = 0;
        
        if (chipState.IsAnySensorActive())
        {
            index = chipState.GetFirstActiveSensor() + 1;
        }

        if(index > notes.Count)
        {
            text.text = "No notes";
            return;
        }
        else
        {
            text.text = notes[index].text;
        }
    }

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