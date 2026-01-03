using UnityEngine;
using UnityEngine.UI;
using SeriousGame.Content;
using System;
using TMPro;

namespace SeriousGame.Runtime
{
    public class DialogueUI : MonoBehaviour
{
    public TextMeshProUGUI promptText;
    public Transform choiceContainer;
    public Button choiceButtonPrefab;

    Action<ChoiceSO> onChoose;

    public void Show(
        InteractionSO interaction,
        Action<ChoiceSO> onChooseCallback)
    {
        gameObject.SetActive(true);
        onChoose = onChooseCallback;
        Cursor.lockState = CursorLockMode.None; // Mở khóa chuột
        Cursor.visible = true;                  // Hiện con trỏ

        promptText.text = interaction.prompt;

        foreach (Transform c in choiceContainer)
            Destroy(c.gameObject);

        foreach (var choice in interaction.choices)
        {
            var btn = Instantiate(choiceButtonPrefab, choiceContainer);
            btn.GetComponentInChildren<Text>().text = choice.text;
            btn.onClick.AddListener(() => Choose(choice));
        }
    }

    void Choose(ChoiceSO choice)
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked; // Khóa chuột lại
        Cursor.visible = false;                   // Ẩn con trỏ
        onChoose?.Invoke(choice);
    }
}
    
}
