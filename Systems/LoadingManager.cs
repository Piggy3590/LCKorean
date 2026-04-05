using TMPro;
using UnityEngine;

namespace LCKR.Patches;

public class LoadingManager : MonoBehaviour
{
    private readonly char[] spinner = { '|', '/', '-', '\\' };
    private int spinnerIndex = 0;
    private float timer = 0f;
    private float interval = 0.1f;
    private Vector3 textPos;
    private Vector3 buttonPos;

    public TMP_Text text;
    public GameObject buttons;
    private CanvasGroup buttonsCanvasGroup;
    private GameObject transition;

    void Start()
    {
        buttonsCanvasGroup = buttons.AddComponent<CanvasGroup>();
        buttonsCanvasGroup.alpha = 0f;
        transition = GameObject.Find("Transition");
        transition.SetActive(false);
        
        textPos = text.transform.position;
        buttonPos = buttons.transform.position; 
        text.transform.position = new Vector3(textPos.x, 603, textPos.z);
        buttons.transform.position = new Vector3(buttonPos.x - 40, buttonPos.y, buttonPos.z);
    }
    
    void Update()
    {
        if (TranslationManager.isDownloadingFiles)
        {
            timer += Time.deltaTime;

            if (timer >= interval)
            {
                spinnerIndex = (spinnerIndex + 1) % spinner.Length;
                timer = 0f;
            }

            text.text = $"{spinner[spinnerIndex]} LCKR 시작 중\n{TranslationManager.downloadingFileText}";
            
            buttons.SetActive(false);
        }
        else
        {
            transition.SetActive(true);
            text.text = "실행 모드";
            text.transform.position = Vector3.Lerp(text.transform.position, textPos, Time.deltaTime * 5);
            
            buttons.SetActive(true);
            buttons.transform.position = Vector3.Lerp(buttons.transform.position, buttonPos, Time.deltaTime * 5);
            buttonsCanvasGroup.alpha = Mathf.Lerp(buttonsCanvasGroup.alpha, 1f, Time.deltaTime * 5);
        }
    }
}