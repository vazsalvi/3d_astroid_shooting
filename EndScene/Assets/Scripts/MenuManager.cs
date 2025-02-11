using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public Transform levelContainer;
    public RectTransform menuContainer;
    public float transitionTime = 1f;
    private int screenWidth;

    public Transform shopButtonsParent;
    private GameObject currentSpaceshipPreview = null;
    public float rotationSpeed = 10f;

    public Text goldText;

    private void Start()
    {
        InitLevelButtons();
        screenWidth = Screen.width;
        InitShopButtons();
        UpdateSpaceshipPreview();
        UpdateGoldText();
    }

    private void Update()
    {
        if (currentSpaceshipPreview != null)
        {
            currentSpaceshipPreview.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }
    }

    private void UpdateSpaceshipPreview()
    {
        // Destroy the previous spaceship preview if it exists
        if (currentSpaceshipPreview != null)
        {
            Destroy(currentSpaceshipPreview);
        }

        // Get the current spaceship prefab from GameManager
        GameObject newSpaceshipPrefab = GameManager.Instance.currentSpaceship;
        if (newSpaceshipPrefab != null)
        {
            // Set the initial rotation for the spaceship preview
            Vector3 startRotationVector = new Vector3(0f, 180f, 0f);
            // Instantiate the new spaceship preview
            currentSpaceshipPreview = Instantiate(newSpaceshipPrefab, Vector3.zero, Quaternion.Euler(startRotationVector));
        }
    }

    private void InitShopButtons()
    {
        int i = 0;
        foreach (Transform btn in shopButtonsParent)
        {
            int currentIdx = i;

            //create sprites
            Texture2D texture = GameManager.Instance.spaceshipTextures[currentIdx];
            Rect newRect = new Rect(0f, 0f, texture.width, texture.height);
            Sprite newSprite = Sprite.Create(texture, newRect, new Vector2(0.5f, 0.5f));
            btn.GetComponent<Image>().sprite = newSprite;

            //onclick event
            Button button = btn.GetComponent<Button>();
            button.onClick.AddListener(() => OnShopButtonClicked(currentIdx));

            //check if we own the spaceship
            if (SaveManager.Instance.IsSpaceshipowned(currentIdx))
            {
                //disable the text element
                btn.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                //change the text element
                Text buttonText = btn.GetComponentInChildren<Text>();
                buttonText.text = GameManager.Instance.spaceshipPrices[currentIdx].ToString();

                //change image to gray
                button.image.color = Color.gray;
            }

            i++;
        }
    }

    private void OnShopButtonClicked(int idx)
    {
        if (SaveManager.Instance.IsSpaceshipowned(idx))
        {
            //own the spaceship
            GameManager.Instance.ChangeCurrentSpaceship(idx);
            UpdateSpaceshipPreview();
        }
        else
        {
            //check if we have enough gold
            int costOfSpaceship = GameManager.Instance.spaceshipPrices[idx];
            int currentGold = SaveManager.Instance.GetGold();
            if (currentGold >= costOfSpaceship)
            {
                //buy it
                SaveManager.Instance.RemoveGold(costOfSpaceship);
                SaveManager.Instance.PurchaseSpaceship(idx);

                //update the button
                Transform clickedBtn = shopButtonsParent.GetChild(idx);
                clickedBtn.GetChild(0).gameObject.SetActive(false); //disabling the text
                Button buttonComponent = clickedBtn.GetComponent<Button>();
                buttonComponent.image.color = Color.white;

                //select the spaceship
                GameManager.Instance.ChangeCurrentSpaceship(idx);
                UpdateSpaceshipPreview();

                //update the gold text
                UpdateGoldText();
            }
        }
    }

    private void InitLevelButtons()
    {
        int lastLevelCompleted = SaveManager.Instance.GetLevelsCompleted();

        int i = 0;
        foreach (Transform t in levelContainer)
        {
            int currentIdx = i;
            Button button = t.GetComponent<Button>();

            if (currentIdx <= lastLevelCompleted)
            {
                //completed level
                button.onClick.AddListener(() => OnLevelSelect(currentIdx));
                button.image.color = Color.white;
            }
            else if (currentIdx == lastLevelCompleted + 1)
            {
                //the current level to be completed
                button.onClick.AddListener(() => OnLevelSelect(currentIdx));
                button.image.color = new Color(0x00 / 255f, 0xEF / 255f, 0xFF / 255f, 1f); // Hex color #00EFFF
                Debug.Log($"Level {currentIdx} to be completed: setting color to #00EFFF.");
            }
            else
            {
                //not completed
                button.interactable = false;
                button.image.color = Color.gray;
            }

            i++;
        }
    }

    private void ChangeMenu(MenuType menuType)
    {
        Vector3 newPos;
        if (menuType == MenuType.Map1Menu)
        {
            newPos = new Vector3(-screenWidth, 0f, 0f);
        }
        else if (menuType == MenuType.ShopMenu)
        {
            newPos = new Vector3(screenWidth, 0f, 0f);
        }
        else if (menuType == MenuType.GameSetting)
        {
            newPos = new Vector3(0f, Screen.height * 1.1f, 0f);
        }
        else
        {
            newPos = Vector3.zero; // Default case
        }

        StopAllCoroutines();
        StartCoroutine(ChangeMenuAnimation(newPos));
    }

    private IEnumerator ChangeMenuAnimation(Vector3 newPos)
    {
        float elapsed = 0f;
        Vector3 oldPos = menuContainer.anchoredPosition3D;

        while (elapsed <= transitionTime)
        {
            elapsed += Time.deltaTime;
            Vector3 currentPos = Vector3.Lerp(oldPos, newPos, elapsed / transitionTime);
            menuContainer.anchoredPosition3D = currentPos;
            yield return null;
        }
    }

    private void OnLevelSelect(int idx)
    {
        GameManager.Instance.currentLevelIdx = idx;

        int levelIdx = idx + 1;
        string sceneName = "Level" + levelIdx.ToString();
        SceneManager.LoadScene(sceneName);
    }

    public void OnPlayButtonClicked()
    {
        Debug.Log("Play Button Clicked");
        ChangeMenu(MenuType.Map1Menu);
    }

    public void OnMainMenuButtonClicked()
    {
        Debug.Log("Clicked main button");
        ChangeMenu(MenuType.MainMenu);
    }

    public void OnNextMapButtonClicked()
    {
        Debug.Log("Next map clicked");
    }

    public void OnShopButtonClicked()
    {
        ChangeMenu(MenuType.ShopMenu);
    }

    public void OnSettingButtonClicked()
    {
        Debug.Log("Settings Button Clicked");
        ChangeMenu(MenuType.GameSetting);
    }

    public void OnResetGameButtonClicked()
    {
        Debug.Log("Reset Game Button Clicked");
        SaveManager.Instance.ResetValues();
        UpdateGoldText();
        InitLevelButtons();
        // Add other necessary UI updates here
    }

    private void UpdateGoldText()
    {
        goldText.text = SaveManager.Instance.GetGold().ToString();
    }

    private enum MenuType
    {
        MainMenu,
        Map1Menu,
        ShopMenu,
        GameSetting
    }
}
