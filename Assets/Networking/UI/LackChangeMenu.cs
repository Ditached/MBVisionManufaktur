using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class LackChangeMenu : MonoBehaviour
{
    public LackConfigCollection globalConfig;
    public string selectedMacAdress = "00:00:00:00:00:00";
    public TMP_Text selectedMacAdressText;

    public GameObject lackChangeBtnPrefab;
    public Transform lackChangeBtnParent;

    private void Awake()
    {
        SpawnInButtons();
        Hide();
    }

    private void Update()
    {
        selectedMacAdressText.text = selectedMacAdress;
    }

    private void SpawnInButtons()
    {
        for (var index = 0; index < globalConfig.lackConfigs.Length; index++)
        {
            var lackChangeBtn = Instantiate(lackChangeBtnPrefab, lackChangeBtnParent);
            var btn = lackChangeBtn.GetComponent<LackChangeButton>();
            
            btn.globalConfig = globalConfig;
            btn.index = index;
            btn.Set();
        }
    }

    public void ChangeSelectedMacToIndex(int index)
    {
        if(selectedMacAdress == "00:00:00:00:00:00") return;
        var exists = globalConfig.lackConfigs.Contains(globalConfig.lackConfigs.First(lack => lack.macAdress == selectedMacAdress));
        if (!exists) return;
        
        var currentLack = globalConfig.lackConfigs.First(lack => lack.macAdress == selectedMacAdress);
        
        var targetLack = globalConfig.lackConfigs[index];
        var targetLackMac = targetLack.macAdress;
        
        currentLack.macAdress = targetLackMac;
        targetLack.macAdress = selectedMacAdress;
        
        Hide();
        globalConfig.OnLackConfigChanged.Invoke();
    }

    public void Hide()
    {
        GetComponent<CanvasGroup>().alpha = 0;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        GetComponent<CanvasGroup>().interactable = false;
    }
    
    public void Show()
    {
        GetComponent<CanvasGroup>().alpha = 1;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        GetComponent<CanvasGroup>().interactable = true;
    }
}
