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
            meleeDamageCollider.light_Attack_02_Modifier = weaponItem.light_Attack_02_Modifier;

            meleeDamageCollider.heavy_Attack_01_Modifier = weaponItem.heavy_Attack_01_Modifier;
            meleeDamageCollider.heavy_Attack_02_Modifier = weaponItem.heavy_Attack_02_Modifier;

            meleeDamageCollider.charge_Attack_01_Modifier = weaponItem.charge_Attack_01_Modifier;
            meleeDamageCollider.charge_Attack_02_Modifier = weaponItem.charge_Attack_02_Modifier;

            meleeDamageCollider.running_Attack_01_Modifier = weaponItem.running_Attack_01_Modifier;
            meleeDamageCollider.rolling_Attack_01_Modifier = weaponItem.rolling_Attack_01_Modifier;
            meleeDamageCollider.backstep_Attack_01_Modifier = weaponItem.backstep_attack_01_Modifier;
        }
    }
}