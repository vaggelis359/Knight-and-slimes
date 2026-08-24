using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using MyGame.Weapons;


namespace MyGame.Weapons                                       // Φτιαχνουμε το namespace Weapons
{
    public class Weapon : MonoBehaviour
    {
        [Header("Ray Settings")]
        [SerializeField][Range(0.0f, 2.0f)] private float rayLenght;
        [SerializeField] private Vector3 rayOffset;
        [SerializeField] private LayerMask weaponMask;
        private RaycastHit topRayHitInfo;

        public Weapon currentWeapon;                                // Το οπλο που σηκωνουμε 
        private Rigidbody weaponBody;

        public enum WeaponType { Sword, AnvillHammer, Crossbow, Cub }
        public WeaponType type;

        // Start is called before the first frame update
        void Start()
        {
            weaponBody = GetComponent<Rigidbody>();

            if (weaponBody == null)
            {
                weaponBody.isKinematic = true;
            }
        }

        public virtual void EquipWeapon(WeaponHolder holder)
        {
            // Default συμπεριφορά για όλα τα όπλα
            transform.parent = holder.equipPos;
            transform.position = holder.equipPos.position;
            transform.rotation = holder.equipPos.rotation;

            holder.rightHandIK.weight = 1f;
            holder.rightHandTarget.position = holder.IKRightHandPos.position;
            holder.rightHandTarget.rotation = holder.IKRightHandPos.rotation;

        }
    }
}





