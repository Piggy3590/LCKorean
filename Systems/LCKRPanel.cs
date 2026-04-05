using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LCKR.Patches
{
    public class LCKRPanel : MonoBehaviour
    {
        public Button resetButton;
        public Button resetCancelButton;
        public Button resetHoldButton;
        public static Animator animPanel;
        private void Start()
        {
            MenuManager menuManager = FindObjectOfType<MenuManager>();
            resetButton = GameObject.Find("LCKRResetButton").GetComponent<Button>();
            
            if (Plugin.versionChanged)
            {
                menuManager.DisplayMenuNotification("LCKR 버전이 변경되었습니다.\n번역 재설정을 권장합니다.", "[ 닫기 ]");
                resetButton.GetComponentInChildren<TMP_Text>().text = "<size=15>(!) LCKR 번역 재설정";
            }
            
            resetCancelButton = GameObject.Find("LCKRNO").GetComponent<Button>();
            resetHoldButton = GameObject.Find("LCKRYES").GetComponent<Button>();
            resetHoldButton.gameObject.AddComponent<ResetHoldButton>();

            animPanel = GameObject.Find("LCKRAnimPanel").GetComponent<Animator>();
            
            resetButton.onClick.AddListener(() => TogglePanel(true));
            resetButton.onClick.AddListener(() => menuManager.PlayConfirmSFX());
            resetCancelButton.onClick.AddListener(() => TogglePanel(false));
            resetCancelButton.onClick.AddListener(() => menuManager.PlayCancelSFX());
        }

        public void TogglePanel(bool on)
        {
            animPanel.SetBool("Open", on);
        }
    }
}
