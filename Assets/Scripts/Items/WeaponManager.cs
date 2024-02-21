using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EldenRing.NT
{
    public class WeaponManager : MonoBehaviour
    {
        public MeleeWeaponDamageCollider meleeDamageCollider;

        private void Awake()
        {
            meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
        }

        public void SetWeaponDamage(CharacterManager characterWieldingWeapon, WeaponItem weaponItem)
        {
            meleeDamageCollider.characterCausingDamage = characterWieldingWeapon;
            meleeDamageCollider.physicalDamage = weaponItem.physicalDamage;
            meleeDamageCollider.magicDamage = weaponItem.magicDamage;
            meleeDamageCollider.fireDamage = weaponItem.fireDamage;
            meleeDamageCollider.lightningDamage = weaponItem.lightningDamage;
            meleeDamageCollider.holyDamage = weaponItem.holyDamage;

            meleeDamageCollider.light_Attack_01_Modifier = weaponItem.light_Attack_01_Modifier;
        }
    }
}