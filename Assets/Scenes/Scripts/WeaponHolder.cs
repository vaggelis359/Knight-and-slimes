using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using MyGame.Weapons;
using static MyGame.Weapons.Weapon;

public class WeaponHolder : MonoBehaviour
{
    public Weapon currentWeapon;

    [SerializeField] public Transform equipPos; // Θέση εξοπλισμού
    [SerializeField] public Transform aimingPos;

    [SerializeField] public float pickupRange = 2f; // Απόσταση που μπορεί να "δει" τα όπλα
    [SerializeField] public LayerMask weaponMask;

    private bool isEquipped = false;

    public bool isAttacking;

    [Header("HandHeld Target")]
    [SerializeField] public TwoBoneIKConstraint rightHandIK;
    [SerializeField] public Transform rightHandTarget;
    [SerializeField] public Transform IKRightHandPos;

    [Header("Weapons positions")]
    [SerializeField] public Transform swordPosition;
    [SerializeField] public Transform anvilHammerPosition;
    [SerializeField] public Transform cubPosition;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!isEquipped)
            {
                EquipClosestWeapon(); // Αν δεν έχεις όπλο, εξόπλισε το κοντινότερο
            }
            else
            {
                UnEquip(); // Αν έχεις όπλο, απο-εξοπλίσου
            }
        }
    }

    private void EquipClosestWeapon()
    {
        // Βρες το κοντινότερο όπλο
        GameObject closestWeapon = FindClosestWeapon();
        if (closestWeapon == null)
        {
            Debug.Log("No weapon in range to equip.");
            return;
        }

        // Πάρε το component Weapon
        currentWeapon = closestWeapon.GetComponent<Weapon>();
        if (currentWeapon == null)
        {
            Debug.LogWarning("Object does not have a Weapon component!");
            return;
        }

        // Ορισμός θέσης εξοπλισμού ανάλογα με τον τύπο του όπλου
        switch (currentWeapon.type)
        {
            case WeaponType.Sword:
                equipPos = swordPosition;
                break;
            case WeaponType.AnvillHammer:
                equipPos = anvilHammerPosition;
                break;
            case WeaponType.Cub:
                equipPos = cubPosition;
                break;
            default:
                Debug.LogWarning("Unknown weapon type.");
                return;
        }

        // Εξόπλισε το όπλο
        currentWeapon.transform.SetParent(equipPos);
        currentWeapon.transform.localPosition = Vector3.zero;
        currentWeapon.transform.localRotation = Quaternion.identity;

        // Προαιρετικά: Ορισμός ότι κρατάμε το όπλο
        isEquipped = true;

        Debug.Log($"Equipped {currentWeapon.name}");
    }

    private void UnEquip()
    {
        if (!isEquipped || currentWeapon == null)
        {
            Debug.LogWarning("No weapon to unequip.");
            return;
        }

        // Αποσύνδεση του όπλου
        currentWeapon.transform.SetParent(null);
        currentWeapon.transform.position = transform.position + transform.forward * 0.5f; // Πετάξιμο λίγο μπροστά
        currentWeapon.GetComponent<Rigidbody>().isKinematic = false; // Ενεργοποίηση physics

        Debug.Log($"Unequipped {currentWeapon.name}");

        // Καθαρισμός δεδομένων
        currentWeapon = null;
        isEquipped = false;
    }

    private GameObject FindClosestWeapon()
    {

        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");                     //Δημιουργουμε μια λιστα με τα αντικειμενα-οπλα που εχουν tag weapon // αλλα εχουμε την λιστα απο το weapon με τα οπλα, μηπως μπορουμε να χρησιμοποιησουμε αυτην ως σημιο αναφορας τον αντικειμενων που σηκωνουμε ως οπλα
        GameObject closestWeapon = null;
        float closestDistance = Mathf.Infinity;

        foreach (GameObject weapon in weapons)
        {
            float distance = Vector3.Distance(transform.position, weapon.transform.position);
            if (distance < pickupRange && distance < closestDistance)
            {
                closestWeapon = weapon;
                closestDistance = distance;
            }
        }

        return closestWeapon;
    }
}
