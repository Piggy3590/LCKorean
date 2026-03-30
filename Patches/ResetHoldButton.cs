using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LCKR.Patches
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class ResetHoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private float holdTime = 2.5f;

        private float time;
        private bool isHolding;

        public void OnPointerDown(PointerEventData eventData)
        {
            isHolding = true;
            time = 0f;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isHolding = false;
            time = 0f;
        }

        private void Update()
        {
            if (!isHolding) return;
            time += Time.deltaTime;

            if (time >= holdTime)
            {
                TranslationManager.ResetTranslation();
                if (LCKRPanel.animPanel != null)
                    LCKRPanel.animPanel.SetBool("Open", false);

                isHolding = false;
                time = 0f;
            }
        }
    }
}
