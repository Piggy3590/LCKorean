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
            resetButton = GameObject.Find("LCKRResetButton").GetComponent<Button>();
            resetCancelButton = GameObject.Find("LCKRNO").GetComponent<Button>();
            resetHoldButton = GameObject.Find("LCKRYES").GetComponent<Button>();
            resetHoldButton.gameObject.AddComponent<ResetHoldButton>();

            animPanel = GameObject.Find("LCKRAnimPanel").GetComponent<Animator>();

            MenuManager menuManager = FindObjectOfType<MenuManager>();
            
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
