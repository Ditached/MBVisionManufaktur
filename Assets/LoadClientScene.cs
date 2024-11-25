using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class LoadClientScene : MonoBehaviour
{
    public TMP_Text downloadText;
    public Image loader;
    [SerializeField] private TMP_Text loadingText;
    private const string SceneKey = "Assets/__Client.unity";

    private IEnumerator Start()
    {
        // First download dependencies
        loadingText.text = "Checking for updates";
        var downloadHandle = Addressables.DownloadDependenciesAsync(SceneKey);

        while (!downloadHandle.IsDone)
        {
            float downloadProgress = downloadHandle.PercentComplete * 100f;

            if (downloadHandle.GetDownloadStatus().TotalBytes == 0)
            {
                downloadText.enabled = false;
                loader.fillAmount = 1f;
            }
            else
            {
                try
                {
                    downloadText.enabled = true;

                    var status = downloadHandle.GetDownloadStatus();

                    float myPercent = downloadHandle.GetDownloadStatus().TotalBytes > 0
                        ? (float) downloadHandle.GetDownloadStatus().DownloadedBytes /
                          downloadHandle.GetDownloadStatus().TotalBytes
                        : 0f;

                    float downloadedMB = status.DownloadedBytes / (1024f * 1024f);
                    float totalMB = status.TotalBytes / (1024f * 1024f);

                    downloadText.text = $"{downloadedMB:0.00}MB/{totalMB:0.00}MB";
                    loader.fillAmount = myPercent;
                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }

            loadingText.text = $"Downloading updates";
            yield return null;
        }

        downloadText.enabled = false;

        // Release the download handle
        Addressables.Release(downloadHandle);

        // Now load the scene
        loadingText.text = "Loading";

        var sceneHandle = Addressables.LoadSceneAsync(SceneKey, LoadSceneMode.Single);
        
        while (!sceneHandle.IsDone)
        {
            if (sceneHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogWarning("Scene load failed, retrying...");
                loadingText.text = "Retrying";
                yield return new WaitForSeconds(2f);
                StartCoroutine(Start());
                yield break;
            }
            
            loadingText.text = $"Loading Scene";
            yield return null;
        }

        if (sceneHandle.Status == AsyncOperationStatus.Succeeded)
        {
            loadingText.text = "Loading Complete!";
            
            // Check if we have a valid scene before activating
            if (sceneHandle.Result.Scene.IsValid())
            {
                sceneHandle.Result.ActivateAsync();
            }
            else
            {
                Debug.LogWarning("Invalid scene result, retrying...");
                loadingText.text = "Retrying";
                yield return new WaitForSeconds(2f);
                StartCoroutine(Start());
            }
        }
        else
        {
            Debug.LogWarning("Scene load failed with status: " + sceneHandle.Status);
            loadingText.text = "Retrying";
            yield return new WaitForSeconds(2f);
            StartCoroutine(Start());
        }
    }
}