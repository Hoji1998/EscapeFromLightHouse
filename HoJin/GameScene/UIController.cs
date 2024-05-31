using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

namespace HoJin.GameScene
{
    [Serializable]
    public struct MessageData
    {
        [SerializeField] private string key;
        [SerializeField] private float time;

        public string Key { get => key; }
        public float Time { get => time; }
    }
    
    public class UIController : GameDirector
    {
        [SerializeField] private GameObject interactPanel;
        private RectTransform interactPanelTransform;
        private TextMeshProUGUI interactText;
        private GameObject mouseIcon;
        private GameObject interactIcon;
        private GameObject itemUsingUIs;

        [SerializeField] private GameObject examinePanel;
        private TextMeshProUGUI nameText;
        private TextMeshProUGUI explanationText;

        [SerializeField] private GameObject settingPanel;

        [SerializeField] private GameObject padLockPanel;
        private Button[] rotationButtons;

        [SerializeField] private GameObject selectPoint;

        [SerializeField] private GameObject inventoryPanel;
        private Animator inventoryAnimator;
        private List<Image> inventoryImages;
        private GameObject flashlightImage;

        [SerializeField] private GameObject paperPanel;
        private Button rightButton;
        private Button leftButton;

        [SerializeField] private TextMeshProUGUI message;
        [SerializeField] private TextMeshProUGUI log;
        private Animator logAnimator;
        public GameObject InteractPanel { get=> interactPanel; }
        public GameObject GetInteractUIsAfterSelect()
        {
            return examinePanel;
        }
        public GameObject GetSettingPanel()
        {
            return settingPanel;
        }
        public GameObject GetPadLockPanel()
        {
            return padLockPanel;
        }
        public Button[] GetRotationButtons()
        {
            return rotationButtons;
        }
        public GameObject GetSelectPoint()
        {
            return selectPoint;
        }
        public GameObject GetInventoryPanel()
        {
            return inventoryPanel;
        }
        public List<Image> InventoryImages { get => inventoryImages; }
        public Animator InventoryAnimator { get => inventoryAnimator; }
        public GameObject FlashlightImage { get => flashlightImage; }
        public GameObject GetPaperPanel()
        {
            return paperPanel;
        }
        public Button GetLeftButton()
        {
            return leftButton;
        }
        public Button GetRightButton()
        {
            return rightButton;
        }



        private void Awake()
        {
            interactPanel.TryGetComponent(out interactPanelTransform);
            interactText = interactPanel.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            mouseIcon = interactPanel.transform.GetChild(0).GetChild(1).gameObject;
            interactIcon = interactPanel.transform.GetChild(0).GetChild(2).gameObject;
            itemUsingUIs = interactPanel.transform.GetChild(1).gameObject;

            nameText = examinePanel.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
            explanationText = examinePanel.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();

            rotationButtons = padLockPanel.GetComponentsInChildren<Button>();

            inventoryPanel.TryGetComponent(out inventoryAnimator);
            inventoryImages = new List<Image>();
            for (int i = 0; i < inventoryPanel.transform.GetChild(2).childCount; i++)
            {
                inventoryImages.Add(inventoryPanel.transform.GetChild(2).GetChild(i).GetChild(0).GetComponent<Image>());
            }
            flashlightImage = inventoryPanel.transform.GetChild(3).gameObject;

            leftButton = paperPanel.transform.GetChild(0).GetComponent<Button>();
            rightButton = paperPanel.transform.GetChild(1).GetComponent<Button>();

            log.TryGetComponent(out logAnimator);
        }



        public void AppearLog(string text)
        {
            log.text = text;
            //logAnimator.Play(DataController.Instance.PreferencesData.NotificationPosition.ToString());
        }

        public void AppearMessage(params MessageData[] messageVariables)
        {
            IEnumerator AppearSequantially(params MessageData[] messageVariables)
            {
                for (int i = 0; i < messageVariables.Length; i++)
                {
                    //message.text = GetTextAtJsonFile(messageVariables[i].Key, KeyWord.defaultText);

                    while (message.color.a <= 1)
                    {
                        message.color += Color.black * Time.deltaTime;
                        yield return null;
                    }

                    yield return new WaitForSeconds(messageVariables[i].Time);

                    while (message.color.a >= 0)
                    {
                        message.color -= Color.black * Time.deltaTime;
                        yield return null;
                    }
                }
            }

            StopAllCoroutines();
            StartCoroutine(AppearSequantially(messageVariables));
        }
        public void AppearInteractUIsWithMouseIcon()
        {
            mouseIcon.SetActive(true);
            interactIcon.SetActive(false);
            //interactText.text = GetTextAtJsonFile("InteractText", KeyWord.defaultText);
            interactText.gameObject.SetActive(true);
        }
        public void AppearInteractUIsWithInteractIcon()
        {
            mouseIcon.SetActive(false);
            interactIcon.SetActive(true);
            //interactText.text = GetTextAtJsonFile("InteractText", KeyWord.defaultText);
            interactText.gameObject.SetActive(true);
        }
        public void SetInteractUIsToMousePosition()
        {
            //interactPanelTransform.anchoredPosition = new Vector2(Input.mousePosition.x - (DataController.Instance.PreferencesData.Resolution.Width * 0.5f) + 10f,
            //    Input.mousePosition.y - (DataController.Instance.PreferencesData.Resolution.Height * 0.5f));
        }
        public void DisappearInteractUIs()
        {
            interactText.gameObject.SetActive(false);
            mouseIcon.SetActive(false);
            interactIcon.SetActive(false);
        }
        public void DisappearInteractUIsAndItemUsingUIs()
        {
            interactText.gameObject.SetActive(false);
            mouseIcon.SetActive(false);
            interactIcon.SetActive(false);
            itemUsingUIs.SetActive(false);
        }
        public void AppearItemUsingUIs()
        {
            itemUsingUIs.SetActive(true);
        }
        public void DisappearItemUsingUIs()
        {
            itemUsingUIs.SetActive(false);
        }
        public void SetInventoryImage(Sprite image, int index)
        {
            inventoryImages[index].sprite = image;
            inventoryImages[index].transform.parent.gameObject.SetActive(true);
        }
        public void SetActiveFalseInventoryImage(int index)
        {
            inventoryImages[index].transform.parent.gameObject.SetActive(false);
        }
        public void SetExaminePanel(PickUpObjectController pickUpObject)
        {
            if (pickUpObject.ExamineData.IsAppearedExaminePanel == true)
            {
                examinePanel.SetActive(true);
                //nameText.text = pickUpObject.GetTextAtJsonFile(KeyWord.name);
                //explanationText.text = pickUpObject.GetTextAtJsonFile(KeyWord.explanation);
            }
        }
    }
}