using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyGame.Weapons.Components.ComponentData;

namespace MyGame.Weapons.weaponsStorage
{
    [CreateAssetMenu(fileName = "newWeaponData", menuName = "Data/Weapon Data/Basic Weapon Data", order = 0)]
    public class WeaponDataSO : ScriptableObject
    {
        [field: SerializeField] public int WeaponStats { get; private set; }

        public List<ComponentData> componentDatas { get; private set; }
    }

}



