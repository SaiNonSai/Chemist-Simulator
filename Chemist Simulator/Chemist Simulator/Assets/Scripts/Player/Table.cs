using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Table_Script : MonoBehaviour
{
    public Transform Chemical_1_Slot;
    public Transform Chemical_2_Slot;
    public Transform Result_Slot;
    public GameObject H2Prefab;
    public GameObject WaterPrefab;
    public GameObject CO2Prefab;
    public GameObject SodiumChloridePrefab;
    public GameObject SodiumSulfatePrefab;
    public GameObject IronSulfatePrefab;
    public GameObject CopperPrefab;
    public GameObject ZincChloridePrefab;
    public GameObject SodiumNitratePrefab;
    public GameObject PotassiumNitratePrefab;
    public GameObject AnhydrousCobaltPrefab;
    public GameObject SilverChloridePrefab;
    public GameObject LeadIodidePrefab;
    public GameObject OxygenPrefab;
    public GameObject CalciumOxidePrefab;
    public GameObject MagnesiumOxidePrefab;
    private ObjectiveManager objectiveManager;
    private PlayerPickUp pickUpScript;  // Reference to the PlayerPickUp script
    public TMP_Text HintText;  // Reference to the HintText from PlayerPickUp script

    private Dictionary<string, List<string>> possibleMixes = new Dictionary<string, List<string>>();

    private List<string> objectives = new List<string>
    {
        "Great! Now mix Methane and Oxygen to create Carbon Dioxide.",
        "Now mix Hydrogen and Oxygen to create Water.",
        "Next, mix Sulfuric Acid and Sodium Carbonate to create Sodium Sulfate.",
        "Great! Now mix Copper Sulfate and Iron to create Iron Sulfate.",
        "Now, mix Zinc and Hydrochloric Acid to create Zinc Chloride.",
        "Nice! Mix Lead Nitrate and Potassium Iodide to create Lead Iodide.",
        "Awesome! Mix Sodium Chloride and Silver Nitrate to create Silver Chloride.",
        "Good! Now try mixing Hydrogen Peroxide with Manganese Dioxide.",
        "Great job! Mix Magnesium and Oxygen to create Magnesium Oxide.",
        "Excellent! Continue exploring chemical combinations."
    };
    private int objectiveIndex = 0;

    private void Start()
    {
        InitializeMixes();
        UpdateObjectiveText();

        GameObject chemicalObject = GameObject.FindGameObjectWithTag("Chemical");
        if (chemicalObject != null)
        {
            pickUpScript = chemicalObject.GetComponent<PlayerPickUp>();
        }
        GameObject managerObject = GameObject.FindGameObjectWithTag("ObjectiveManager");
        if (managerObject != null)
        {
            objectiveManager = managerObject.GetComponent<ObjectiveManager>();
        }

        // Make sure the hint text is empty at the start
        if (HintText != null)
        {
            HintText.text = "";
        }
    }
    
    private void InitializeMixes()
    {
        // Define possible mixes for each chemical
        possibleMixes["H2"] = new List<string> { "Oxygen" };
        possibleMixes["Oxygen"] = new List<string> { "H2", "Methane", "Magnesium", "Hydrogen Peroxide", "Heat", "Copper Sulfate" };
        possibleMixes["Methane"] = new List<string> { "Oxygen" };
        possibleMixes["Hydrogen"] = new List<string> { "H2" };
        possibleMixes["Hydrogen (Clone)"] = new List<string> { "Hydrogen" };
        possibleMixes["Sodium Hydroxide"] = new List<string> { "Hydrochloric Acid" };
        possibleMixes["Hydrochloric Acid"] = new List<string> { "Sodium Hydroxide", "Zinc", "Silver Nitrate" };
        possibleMixes["Sulfuric Acid"] = new List<string> { "Sodium Carbonate" };
        possibleMixes["Sodium Carbonate"] = new List<string> { "Sulfuric Acid" };
        possibleMixes["Copper Sulfate"] = new List<string> { "Iron" };
        possibleMixes["Iron"] = new List<string> { "Copper Sulfate" };
        possibleMixes["Zinc"] = new List<string> { "Hydrochloric Acid" };
        possibleMixes["Silver Nitrate"] = new List<string> { "Sodium Chloride" };
        possibleMixes["Sodium Chloride"] = new List<string> { "Silver Nitrate" };
        possibleMixes["Lead Nitrate"] = new List<string> { "Potassium Iodide" };
        possibleMixes["Potassium Iodide"] = new List<string> { "Lead Nitrate" };
        possibleMixes["Calcium Carbonate"] = new List<string> { "Heat" };
        possibleMixes["Heat"] = new List<string> { "Calcium Carbonate", "Cobalt Chloride (Hydrated)" };
        possibleMixes["Hydrogen Peroxide"] = new List<string> { "Manganese Dioxide" };
        possibleMixes["Manganese Dioxide"] = new List<string> { "Hydrogen Peroxide" };
        possibleMixes["Magnesium"] = new List<string> { "Oxygen" };
        possibleMixes["Cobalt Chloride (Hydrated)"] = new List<string> { "Heat" };
        possibleMixes["Cobalt Chloride (Anhydrous)"] = new List<string> { };
    }

    private void Update()
    {
        if (AreBothSlotsFull() && Input.GetKeyDown(KeyCode.Return))
        {
            MixChemicals();
        }
    }

    private bool AreBothSlotsFull()
    {
        return (Chemical_1_Slot.childCount > 0 && Chemical_2_Slot.childCount > 0);
    }

    private void MixChemicals()
    {
        string chemical1 = Chemical_1_Slot.GetChild(0).name;
        string chemical2 = Chemical_2_Slot.GetChild(0).name;

        if (possibleMixes.ContainsKey(chemical1) && possibleMixes[chemical1].Contains(chemical2) || possibleMixes.ContainsKey(chemical2) && possibleMixes[chemical2].Contains(chemical1))
        {
            ProcessCombination(chemical1, chemical2);
        }
        else
        {
            ShowHintText("These chemicals cannot be mixed.");
        }
    }

    private void ProcessCombination(string chemical1, string chemical2)
    {
         //Handle individual valid combinations
        if ((chemical1 == "H2" && chemical2 == "Oxygen") || 
            (chemical1 == "Oxygen" && chemical2 == "H2"))
        {
            CreateResultingChemical(WaterPrefab);
            MoveToNextObjective();
            ShowHintText("You created Water by mixing Hydrogen and Oxygen!");
        }
        else if ((chemical1 == "Methane" && chemical2 == "Oxygen") || (chemical1 == "Oxygen" && chemical2 == "Methane"))
        {
            // Only create the correct number of chemicals
            CreateResultingChemical(CO2Prefab);
            CreateResultingChemical(WaterPrefab);
            MoveToNextObjective();
            ShowHintText("You created Carbon Dioxide and Water!");
        }
        else if ((chemical1 == "Hydrochloric Acid" && chemical2 == "Sodium Hydroxide") || (chemical1 == "Sodium Hydroxide" && chemical2 == "Hydrochloric Acid"))
        {
            CreateResultingChemical(SodiumChloridePrefab);
            CreateResultingChemical(WaterPrefab);
            MoveToNextObjective();
            ShowHintText("You created Sodium Chloride and Water!");
        }
        else if ((chemical1 == "Sulfuric Acid" && chemical2 == "Sodium Carbonate") || (chemical1 == "Sodium Carbonate" && chemical2 == "Sulfuric Acid"))
        {
            CreateResultingChemical(SodiumSulfatePrefab);
            CreateResultingChemical(CO2Prefab);
            CreateResultingChemical(WaterPrefab);
            MoveToNextObjective();
            ShowHintText("You created Sodium Sulfate, Carbon Dioxide, and Water!");
        }
        else if ((chemical1 == "Copper Sulfate" && chemical2 == "Iron") || (chemical1 == "Iron" && chemical2 == "Copper Sulfate"))
        {
            CreateResultingChemical(IronSulfatePrefab);
            CreateResultingChemical(CopperPrefab);
            MoveToNextObjective();
            ShowHintText("You created Iron Sulfate and Copper!");
        }
        else if ((chemical1 == "Zinc" && chemical2 == "Hydrochloric Acid") || (chemical1 == "Hydrochloric Acid" && chemical2 == "Zinc"))
        {
            CreateResultingChemical(ZincChloridePrefab);
            CreateResultingChemical(H2Prefab);
            MoveToNextObjective();
            ShowHintText("You created Zinc Chloride and Hydrogen!");
        }
        else if ((chemical1 == "Lead Nitrate" && chemical2 == "Potassium Iodide") || (chemical1 == "Potassium Iodide" && chemical2 == "Lead Nitrate"))
        {
            CreateResultingChemical(LeadIodidePrefab);
            MoveToNextObjective();
            ShowHintText("You created Lead Iodide!");
        }
        else if ((chemical1 == "Sodium Chloride" && chemical2 == "Silver Nitrate") || (chemical1 == "Silver Nitrate" && chemical2 == "Sodium Chloride"))
        {
            CreateResultingChemical(SilverChloridePrefab);
            MoveToNextObjective();
            ShowHintText("You created Silver Chloride!");
        }
        else if ((chemical1 == "Calcium Carbonate" && chemical2 == "Heat") || (chemical1 == "Heat" && chemical2 == "Calcium Carbonate"))
        {
            CreateResultingChemical(CalciumOxidePrefab);
            CreateResultingChemical(CO2Prefab);
            MoveToNextObjective();
            ShowHintText("You created Calcium Oxide and Carbon Dioxide!");
        }
        else if ((chemical1 == "Hydrogen Peroxide" && chemical2 == "Manganese Dioxide") || (chemical1 == "Manganese Dioxide" && chemical2 == "Hydrogen Peroxide"))
        {
            CreateResultingChemical(OxygenPrefab);
            CreateResultingChemical(WaterPrefab);
            MoveToNextObjective();
            ShowHintText("You created Oxygen and Water!");
        }
        else if ((chemical1 == "Magnesium" && chemical2 == "Oxygen") || (chemical1 == "Oxygen" && chemical2 == "Magnesium"))
        {
            CreateResultingChemical(MagnesiumOxidePrefab);
            MoveToNextObjective();
            ShowHintText("You created Magnesium Oxide!");
        }
        else if ((chemical1 == "Cobalt Chloride (Hydrated)" && chemical2 == "Heat") || (chemical1 == "Heat" && chemical2 == "Cobalt Chloride (Hydrated)"))
        {
            CreateResultingChemical(AnhydrousCobaltPrefab);
            MoveToNextObjective();
            ShowHintText("You created Anhydrous Cobalt Chloride!");
        }
        else
        {
            ShowHintText("These chemicals cannot be mixed.");
        }
    }

    private void CreateResultingChemical(GameObject prefab)
    {
        Instantiate(prefab, Result_Slot.position, Quaternion.identity, Result_Slot);
    }

    private void ShowHintText(string message)
    {
        if (HintText != null)
        {
            HintText.text = message;
        }
    }

    private void ClearSlots()
    {
        foreach(Transform child in Chemical_1_Slot)
        {
            Destroy(child.gameObject);
        }
        foreach(Transform child in Chemical_2_Slot)
        {
            Destroy(child.gameObject);
        }
    }

    private void PlaceChemicalInSlot(GameObject chemical, Transform slot, PlayerPickUp pickUpScript)
    {
        chemical.transform.SetParent(slot);
        chemical.transform.position = slot.position;
        chemical.transform.rotation = slot.rotation;

        // Re-enable physics for dropped chemical
        Rigidbody rb = chemical.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;

        Collider collider = chemical.GetComponent<Collider>();
        if (collider != null) collider.enabled = true;

        // Clear the pick-up script
        pickUpScript.Chemical = null;
        PlayerPickUp.isHoldingItem = false;

        Debug.Log($"Placed {chemical.name} in {slot.name}");
    }

    public void DropChemicalToSlot(PlayerPickUp pickUpScript)
    {
        if (pickUpScript == null || pickUpScript.Chemical == null)
        {
            Debug.LogError("No chemical to drop!");
            return;
        }

        if (Chemical_1_Slot.childCount == 0)
        {
            PlaceChemicalInSlot(pickUpScript.Chemical, Chemical_1_Slot, pickUpScript);
        }
        else if (Chemical_2_Slot.childCount == 0)
        {
            PlaceChemicalInSlot(pickUpScript.Chemical, Chemical_2_Slot, pickUpScript);
        }
        else
        {
            Debug.Log("Both chemical slots are full!");
        }
    }

    private void MoveToNextObjective()
    {
        objectiveIndex = Mathf.Clamp(objectiveIndex + 1, 0, objectives.Count - 1);
        UpdateObjectiveText();
    }
    
    private void UpdateObjectiveText()
    {
        if (objectiveManager != null && objectiveIndex < objectives.Count)
        {
            objectiveManager.SetObjective(objectives[objectiveIndex]);
        }
    }
}
