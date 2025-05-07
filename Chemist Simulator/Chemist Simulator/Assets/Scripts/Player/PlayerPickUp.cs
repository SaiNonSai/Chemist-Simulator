using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerPickUp : MonoBehaviour
{
    [Header("Pick Up Settings")]
    public Transform PickUpSlot; // Slot to hold picked-up items
    public float PickUpRange = 2f; // Range within which items can be picked up
    public float SphereCastRadius = 1f; // Radius of the sphere cast for detecting items
    public KeyCode PickUpKey = KeyCode.E; // Key to pick up or drop items
    public KeyCode DropToSlotKey = KeyCode.F; // Key to drop items to a specific slot

    [HideInInspector] public GameObject Chemical;
    public static bool isHoldingItem = false;

    public TMP_Text HintText; // UI for hints
    public TMP_Text DropPromptText;

    private GameObject highlightedObject; // Currently highlighted object

    private void Start()
    {
        if (DropPromptText != null)
        {
            DropPromptText.text = ""; // Hide the prompt initially
        }
    }

    private void Update()
    {
        HighlightPickableObject(); // Highlight items in range

        ShowDropPrompt();

        if (Input.GetKeyDown(PickUpKey))
        {
            if (isHoldingItem)
            {
                DropItem();
            }
            else
            {
                TryPickUpItem();
            }
        }

        if (Input.GetKeyDown(DropToSlotKey) && isHoldingItem)
        {
            TryDropToSlot();
        }
    }

    private void HighlightPickableObject()
    {
        RaycastHit hit;
        // Use a SphereCast for better object detection (especially for floor-level objects)
        Vector3 origin = transform.position + Vector3.up * 0.5f; // Offset the SphereCast slightly upwards
        Vector3 direction = transform.forward;
        if (Physics.SphereCast(origin, SphereCastRadius, transform.forward, out hit, PickUpRange))
        {
            if (hit.collider.CompareTag("Chemical"))
            {
                if (highlightedObject != hit.collider.gameObject)
                {
                    if (highlightedObject != null)
                    {
                        RemoveHighlight(highlightedObject);
                    }
                    highlightedObject = hit.collider.gameObject;
                    ApplyHighlight(highlightedObject);
                }
            }
            else
            {
                RemoveHighlight(highlightedObject);
                highlightedObject = null;
            }
        }
        else
        {
            RemoveHighlight(highlightedObject);
            highlightedObject = null;
        }

        Debug.DrawRay(origin, transform.forward * PickUpRange, Color.green);
        Debug.DrawLine(hit.point, hit.point + Vector3.up * 0.5f, Color.red);
    }

    private void ShowDropPrompt()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        // Check if player is within range of a table
        if (Physics.SphereCast(origin, SphereCastRadius, transform.forward, out hit, PickUpRange))
        {
            Table_Script table = hit.collider.GetComponent<Table_Script>();
            if (table != null && isHoldingItem)
            {
                // Show the prompt to drop the item
                if (DropPromptText != null)
                {
                    DropPromptText.text = "Press F to drop item on table"; // Customize the message
                }
                return;
            }
        }

        // Hide the prompt if no table is in range or player isn't holding an item
        if (DropPromptText != null)
        {
            DropPromptText.text = ""; 
        }
    }
    private void TryPickUpItem()
    {
       
        RaycastHit hit;
        // Use SphereCast for picking up objects
        Vector3 origin = transform.position + Vector3.up * 0.5f; 
        if (Physics.SphereCast(origin, SphereCastRadius, transform.forward, out hit, PickUpRange))
        {
            if (hit.collider.CompareTag("Chemical"))
            {
                PickUpItem(hit.collider.gameObject);
            }
        }
    }

    private void PickUpItem(GameObject item)
    {
        if(isHoldingItem)
        { 
            return; 
        }

        Chemical = item;
        Chemical.transform.SetParent(PickUpSlot);
        Chemical.transform.localPosition = Vector3.zero;
        Chemical.transform.localRotation = Quaternion.identity;

        // Disable item's physics for holding
        Rigidbody rb = Chemical.GetComponent<Rigidbody>();
        if (rb != null)
        {
             rb.isKinematic = true;
        }

        Collider collider = Chemical.GetComponent<Collider>();
        if (collider != null)
        {
             collider.enabled = false;
        }

        isHoldingItem = true;

        // Update hint text
        string chemicalName = item.name;
        string hint = GetHintForChemical(chemicalName);
        HintText.text = hint;

        RemoveHighlight(Chemical);
    }

    private void DropItem()
    {
        if (!isHoldingItem || Chemical == null)
        {
            return;
        }

        // Detach the item
        Chemical.transform.SetParent(null);

        // Re-enable physics
        Rigidbody rb = Chemical.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        Collider collider = Chemical.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }

        // Drop the item slightly in front of the player
        Vector3 dropPosition = transform.position + transform.forward * 1.5f;
        Chemical.transform.position = dropPosition;

        Chemical = null;
        isHoldingItem = false;
    }

    private void TryDropToSlot()
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        if (Physics.SphereCast(origin, SphereCastRadius, transform.forward, out hit, PickUpRange))
        {
            Table_Script table = hit.collider.GetComponent<Table_Script>();
            if (table != null)
            {
                table.DropChemicalToSlot(this);
            }
        }
    }

    public string GetHintForChemical(string chemicalName)
    {
        // Return a hint based on the item's name
        switch (chemicalName)
        {
            case "H2": return "Mixable with Oxygen.";
            case "Hydrogen": return "Mixable with Hydrogen.";
            case "Methane": return "Mixable with Oxygen.";
            case "Oxygen": return "Mixable with Hydrogen, Methane, Magnesium, and more.";
            case "Sodium Hydroxide": return "Mixable with Hydrochloric Acid.";
            case "Hydrochloric Acid": return "Mixable with Sodium Hydroxide, Zinc.";
            case "Sulfuric Acid": return "Mixable with Sodium Carbonate.";
            case "Sodium Carbonate": return "Mixable with Sulfuric Acid.";
            case "Copper Sulfate": return "Mixable with Iron.";
            case "Iron": return "Mixable with Copper Sulfate.";
            case "Zinc": return "Mixable with Hydrochloric Acid.";
            case "Silver Nitrate": return "Mixable with Sodium Chloride.";
            case "Sodium Chloride": return "Mixable with Silver Nitrate.";
            case "Lead Nitrate": return "Mixable with Potassium Iodide.";
            case "Potassium Iodide": return "Mixable with Lead Nitrate.";
            case "Calcium Carbonate": return "Heating.";
            case "Heat": return "Heatable with Calcium Carbonate, Cobalt Chloride (Hydrated).";
            case "Hydrogen Peroxide": return "Mixable with Manganese Dioxide.";
            case "Manganese Dioxide": return "Mixable with Hydrogen Peroxide.";
            case "Magnesium": return "Mixable with Oxygen.";
            case "Cobalt Chloride (Hydrated)": return "Mixable with Heat.";
            case "Cobalt Chloride (Anhydrous)": return "Hint: No known mixes.";
            default: return "Hint: No specific hint for this chemical. Try experimenting!";
        }
    }

    private void ApplyHighlight(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.yellow;
        }
    }

    private void RemoveHighlight(GameObject obj)
    {
        if (obj == null) return;

        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material.color = Color.white;
        }
    }
}
