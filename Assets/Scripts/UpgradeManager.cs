using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject upgradePanel;
    public Button optionButtonPrefab;
    public Transform optionsContainer;

    private Player player;

    private enum UpgradeType
    {
        MovementSpeed, 
        RotationSpeed, 
        FlatHealth,
        PickupRadius,
        CritChance,
        CritDamage,
        AttackSpeed,
        Damage
    }

    private void Start()
    {
        if (upgradePanel == null || optionButtonPrefab == null || optionsContainer == null)
        {
            Debug.LogError("NEED ALL UI REFERENCES");
        }
        
        upgradePanel.SetActive(false);
    }

    public void ShowUpgradeOptions(Player player)
    {
        //Debug.Log("ShowUpgradeOptions called");
        this.player = player;

        // Pause game
        Time.timeScale = 0f;
        
        // Toggle the upgrade panel on
        upgradePanel.SetActive(true);
        
        // Create a list of all the potential upgrades
        List<UpgradeType> allUpgrades = new List<UpgradeType>((UpgradeType[])System.Enum.GetValues(typeof(UpgradeType)));
        List<UpgradeType> selectedUpgrades = new List<UpgradeType>();
        
        // Shuffle the list of upgrades and take the first three of them
        for (int i = 0; i < 3; i++)
        {
            if (allUpgrades.Count == 0) break;
            int index = Random.Range(0, allUpgrades.Count);
            selectedUpgrades.Add(allUpgrades[index]);
            allUpgrades.RemoveAt(index);
        }

        //Debug.Log("Amount of upgrades to choose from (should usually be 3): " + selectedUpgrades.Count);
        
        foreach (var up in selectedUpgrades)
        {
            //Debug.Log("Upgrade to choose: " + up);
        }

        // Get rid of the existing buttons
        foreach (Transform child in optionsContainer)
        {
            Destroy(child.gameObject);
        }
        
        Vector3[] buttonPositions = {
            new(-300f, 0f, 0f), // Left button
            new(0f, 0f, 0f), // Middle button
            new(300f, 0f, 0f) // Right buttob
        };
        
        // Creating the upgrade buttons here
        for (int i = 0; i < selectedUpgrades.Count; i++)
        {
            UpgradeType upgrade = selectedUpgrades[i];
            Button btn = Instantiate(optionButtonPrefab, optionsContainer);
            btn.onClick.AddListener(() => OnUpgradeSelected(upgrade));
            btn.GetComponentInChildren<TextMeshProUGUI>().text = GetUpgradeDisplayName(upgrade);
            
            btn.GetComponent<RectTransform>().anchoredPosition = buttonPositions[i];
        }
        
        if (selectedUpgrades.Count < 3)
        {
            Debug.LogWarning("Potentially need to handle the case where we have less than 3 upgrade types available");
        }
    }

    private string GetUpgradeDisplayName(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.MovementSpeed:
                return "Movement speed";
            case UpgradeType.RotationSpeed:
                return "Rotation speed";
            case UpgradeType.FlatHealth:
                return "Health";
            case UpgradeType.PickupRadius:
                return "Pickup radius";
            case UpgradeType.CritChance:
                return "CritChance";
            case UpgradeType.CritDamage:
                return "CritDamage";
            case UpgradeType.AttackSpeed:
                return "Attack Speed";
            case UpgradeType.Damage:
                return "Damage";
            default:
                return "wat da catfish";
        }
    }

    private void OnUpgradeSelected(UpgradeType upgrade)
    {
        ApplyUpgrade(upgrade);
        CloseUpgradeMenu();
    }

    private void ApplyUpgrade(UpgradeType upgrade)
    {
        switch (upgrade)
        {
            case UpgradeType.MovementSpeed:
                player.UpgradeMovementSpeed(0.6f);
                break;
            case UpgradeType.RotationSpeed:
                player.UpgradeRotationSpeed(0.6f);
                break;
            case UpgradeType.FlatHealth:
                player.UpgradeFlatHealth(12);
                break;
            case UpgradeType.PickupRadius:
                player.UpgradePickupRadius(0.5f);
                break;
            case UpgradeType.CritChance:
                player.UpgradeCritChance(1f);
                break;
            case UpgradeType.CritDamage:
                player.UpgradeCritDamage(0.15f);
                break;
            case UpgradeType.AttackSpeed:
                player.UpgradeAttackSpeed(0.1f);
                break;
            case UpgradeType.Damage:
                player.UpgradeDamage(8);
                break;
            
        }

        Debug.Log("Player upgraded their " + upgrade);
    }

    private void CloseUpgradeMenu()
    {
        Time.timeScale = 1f;
        
        upgradePanel.SetActive(false);
    }
}
