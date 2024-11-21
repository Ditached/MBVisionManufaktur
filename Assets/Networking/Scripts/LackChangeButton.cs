    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class LackChangeButton : MonoBehaviour
    {
        public LackConfigCollection globalConfig;
        public TMP_Text nameText;
        public Image colorImage;
        
        public int index;

        public void Set()
        {
            nameText.text = globalConfig.lackConfigs[index].name;
            colorImage.color = globalConfig.lackConfigs[index].mainColor;
            
            if(globalConfig.lackConfigs[index].isUnassigned) nameText.text = "Unassigned";
            
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            var lackChangeMenu = GetComponentInParent<LackChangeMenu>();
            lackChangeMenu.ChangeSelectedMacToIndex(index);
        }
    }