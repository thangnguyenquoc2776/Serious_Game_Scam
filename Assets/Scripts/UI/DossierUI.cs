//using UnityEngine;
//using TMPro;
//using UnityEngine.UI;
//using System;

//public class DossierUI : MonoBehaviour
//{
//    public TextMeshProUGUI titleText;
//    public TextMeshProUGUI contentText;
//    public Button prevButton;
//    public Button nextButton;
//    public Button doneButton;

//    [TextArea(5, 10)]
//    public string[] dossiers;

//    private int index = 0;
//    private Action onDone;

//    public void Open(Action doneCallback)
//    {
//        onDone = doneCallback;
//        index = 0;
//        Refresh();

//        gameObject.SetActive(true);

//        var player = FindObjectOfType<SeriousGame.Runtime.PlayerController>();
//        player?.SetLockState(true);
//    }

//    void Refresh()
//    {
//        titleText.text = $"Hồ sơ {index + 1}";
//        contentText.text = dossiers[index];

//        prevButton.interactable = index > 0;
//        nextButton.interactable = index < dossiers.Length - 1;
//    }

//    void Start()
//    {
//        prevButton.onClick.AddListener(() =>
//        {
//            index--;
//            Refresh();
//        });

//        nextButton.onClick.AddListener(() =>
//        {
//            index++;
//            Refresh();
//        });

//        doneButton.onClick.AddListener(() =>
//        {
//            var player = FindObjectOfType<SeriousGame.Runtime.PlayerController>();
//            player?.SetLockState(false);

//            onDone?.Invoke();
//            Destroy(gameObject);
//        });
//    }
//}
