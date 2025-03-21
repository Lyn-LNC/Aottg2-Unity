using ApplicationManagers;
using Cameras;
using Characters;
using GameManagers;
using Settings;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace CustomLogic
{
    class CustomLogicHumanBuiltin : CustomLogicCharacterBuiltin
    {
        public Human Human;

        public CustomLogicHumanBuiltin(Human human) : base(human, "Human")
        {
            Human = human;
        }

        public override object CallMethod(string methodName, List<object> parameters)
        {
            if (methodName == "Refill")
            {
                if (Human.IsMine() && Human.NeedRefill(true))
                    return Human.Refill();
                return false;
            }
            if (methodName == "RefillImmediate")
            {
                if (Human.IsMine())
                    Human.FinishRefill();
                return null;
            }
            if (methodName == "ClearHooks")
            {
                if (Human.IsMine())
                {
                    Human.HookLeft.DisableAnyHook();
                    Human.HookRight.DisableAnyHook();
                }
                return null;
            }
            if (methodName == "ClearLeftHook")
            {
                if (Human.IsMine())
                    Human.HookLeft.DisableAnyHook();
                return null;
            }
            if (methodName == "ClearRightHook")
            {
                if (Human.IsMine())
                    Human.HookRight.DisableAnyHook();
                return null;
            }
            if (methodName == "MountMapObject")
            {
                if (Human.IsMine())
                {
                    Vector3 positionOffset = ((CustomLogicVector3Builtin)parameters[1]).Value;
                    Vector3 rotationOffset = ((CustomLogicVector3Builtin)parameters[2]).Value;
                    Human.Mount(((CustomLogicMapObjectBuiltin)parameters[0]).Value, positionOffset, rotationOffset);
                }
                return null;
            }
            if (methodName == "MountTransform")
            {
                if (Human.IsMine())
                {
                    Vector3 positionOffset = ((CustomLogicVector3Builtin)parameters[1]).Value;
                    Vector3 rotationOffset = ((CustomLogicVector3Builtin)parameters[2]).Value;
                    Human.Mount(((CustomLogicTransformBuiltin)parameters[0]).Value, positionOffset, rotationOffset);
                }
                return null;
            }
            if (methodName == "Unmount")
            {
                if (Human.IsMine())
                    Human.Unmount(true);
                return null;
            }
            if (methodName == "SetSpecial")
            {
                if (Human.IsMine())
                    Human.SetSpecial((string)parameters[0]);
                return null;
            }
            if (methodName == "ActivateSpecial")
            {
                if (Human.IsMine() && Human.Special != null)
                {
                    Human.Special.SetInput(true);
                    Human.Special.SetInput(false);
                }
                return null;
            }
            if (methodName == "SetWeapon")
            {
                if (!Human.IsMine())
                    return null;
                var gameManager = (InGameManager)SceneLoader.CurrentGameManager;
                string weapon = (string)parameters[0];
                if (weapon == "Blades")
                    weapon = "Blade";
                else if (weapon == "Thunderspears")
                    weapon = "Thunderspear";
                if (gameManager.CurrentCharacter != null && gameManager.CurrentCharacter is Human && Human.IsMine())
                {
                    var miscSettings = SettingsManager.InGameCurrent.Misc;
                    if (!Human.Dead)
                    {
                        List<string> loadouts = new List<string>();
                        if (miscSettings.AllowBlades.Value)
                            loadouts.Add(HumanLoadout.Blade);
                        if (miscSettings.AllowAHSS.Value)
                            loadouts.Add(HumanLoadout.AHSS);
                        if (miscSettings.AllowAPG.Value)
                            loadouts.Add(HumanLoadout.APG);
                        if (miscSettings.AllowThunderspears.Value)
                            loadouts.Add(HumanLoadout.Thunderspear);
                        if (loadouts.Count == 0)
                            loadouts.Add(HumanLoadout.Blade);

                        if (loadouts.Contains(weapon) && weapon != SettingsManager.InGameCharacterSettings.Loadout.Value)
                        {
                            SettingsManager.InGameCharacterSettings.Loadout.Value = weapon;
                            var manager = (InGameManager)SceneLoader.CurrentGameManager;
                            Human = (Human)gameManager.CurrentCharacter;
                            Human.ReloadHuman(manager.GetSetHumanSettings());
                        }
                    }
                }
                return null;
            }
            if (methodName == "DisablePerks")
            {
                if (Human.IsMine())
                    Human.Stats.DisablePerks();
                return null;
            }
            return base.CallMethod(methodName, parameters);
        }

        public override object GetField(string name)
        {
            BladeWeapon bladeWeapon = null;
            AmmoWeapon ammoWeapon = null;
            if (Human.Weapon is BladeWeapon)
                bladeWeapon = (BladeWeapon)Human.Weapon;
            else if (Human.Weapon is AmmoWeapon)
                ammoWeapon = (AmmoWeapon)Human.Weapon;
            if (name == "Weapon")
                return Human.Setup.Weapon.ToString();
            if (name == "CurrentSpecial")
                return Human.CurrentSpecial;
            if (name == "SpecialCooldown")
                return Human.Special == null ? 0f : Human.Special.Cooldown;
            if (name == "ShifterLiveTime")
            {
                if (Human.Special != null && Human.Special is ShifterTransformSpecial special)
                    return special.LiveTime;

                return 0f;
            }
            if (name == "SpecialCooldownRatio")
                return Human.Special == null ? 0f : Human.Special.GetCooldownRatio();
            if (name == "CurrentGas")
                return Human.Stats.CurrentGas;
            if (name == "MaxGas")
                return Human.Stats.MaxGas;
            if (name == "Acceleration")
                return Human.Stats.Acceleration;
            if (name == "Speed")
                return Human.Stats.Speed;
            if (name == "HorseSpeed")
                return Human.Stats.HorseSpeed;
            if (name == "CurrentBladeDurability")
            {
                if (bladeWeapon != null)
                    return bladeWeapon.CurrentDurability;
                return 0f;
            }
            if (name == "MaxBladeDurability")
            {
                if (bladeWeapon != null)
                    return bladeWeapon.MaxDurability;
                return 0f;
            }
            if (name == "CurrentBlade")
            {
                if (bladeWeapon != null)
                    return bladeWeapon.BladesLeft;
                return 0;
            }
            if (name == "MaxBlade")
            {
                if (bladeWeapon != null)
                    return bladeWeapon.MaxBlades;
                return 0;
            }
            if (name == "CurrentAmmoRound")
            {
                if (ammoWeapon != null)
                    return ammoWeapon.RoundLeft;
                return 0;
            }
            if (name == "MaxAmmoRound")
            {
                if (ammoWeapon != null)
                    return ammoWeapon.MaxRound;
                return 0;
            }
            if (name == "CurrentAmmoLeft")
            {
                if (ammoWeapon != null)
                    return ammoWeapon.AmmoLeft;
                return 0;
            }
            if (name == "MaxAmmoTotal")
            {
                if (ammoWeapon != null)
                    return ammoWeapon.MaxAmmo;
                return 0;
            }
            if (name == "LeftHookEnabled")
                return Human.HookLeft.Enabled;
            if (name == "RightHookEnabled")
                return Human.HookRight.Enabled;
            if (name == "IsMounted")
                return Human.MountState == HumanMountState.MapObject;
            if (name == "MountedMapObject")
            {
                if (Human.MountedMapObject == null)
                    return null;
                return new CustomLogicMapObjectBuiltin(Human.MountedMapObject);
            }
            if (name == "MountedTransform")
            {
                if (Human.MountedTransform == null)
                    return null;
                return new CustomLogicTransformBuiltin(Human.MountedTransform);
            }
            if (name == "AutoRefillGas")
            {
                if (Human != null && Human.IsMine())
                    return SettingsManager.InputSettings.Human.AutoRefillGas.Value;
                return false;
            }
            if (name == "State")
                return Human.State.ToString();
            if (name == "CanDodge")
                return Human.CanDodge;
            if (name == "IsInvincible")
                return Human.IsInvincible;
            if (name == "InvincibleTimeLeft")
                return Human.InvincibleTimeLeft;
            if (name == "IsCarried")
                return Human.CarryState == HumanCarryState.Carry;
            return base.GetField(name);
        }

        public override void SetField(string name, object value)
        {
            if (name == "Name")
                Character.Name = (string)value;
            else if (name == "Guild")
                Character.Guild = (string)value;
            if (!Human.IsMine())
                return;
            BladeWeapon bladeWeapon = null;
            AmmoWeapon ammoWeapon = null;
            if (Human.Weapon is BladeWeapon)
                bladeWeapon = (BladeWeapon)Human.Weapon;
            else if (Human.Weapon is AmmoWeapon)
                ammoWeapon = (AmmoWeapon)Human.Weapon;
            if (name == "SpecialCooldown")
            {
                if (Human.Special == null) return;

                var v = Mathf.Max(0f, value.UnboxToFloat());
                Human.Special.Cooldown = v;
            }
            else if (name == "ShifterLiveTime")
            {
                if (Human.Special != null && Human.Special is ShifterTransformSpecial special)
                    special.LiveTime = value.UnboxToFloat();
            }
            else if (name == "CurrentGas")
                Human.Stats.CurrentGas = Mathf.Min(Human.Stats.MaxGas, value.UnboxToFloat());
            else if (name == "MaxGas")
                Human.Stats.MaxGas = value.UnboxToFloat();
            else if (name == "Acceleration")
                Human.Stats.Acceleration = value.UnboxToInt();
            else if (name == "Speed")
                Human.Stats.Speed = value.UnboxToInt();
            else if (name == "HorseSpeed")
                Human.Stats.HorseSpeed = value.UnboxToFloat();
            else if (name == "CurrentBladeDurability")
            {
                if (bladeWeapon != null)
                {
                    bool bladeWasEnabled = bladeWeapon.CurrentDurability > 0f;
                    bladeWeapon.CurrentDurability = Mathf.Max(Mathf.Min(bladeWeapon.MaxDurability, value.UnboxToFloat()), 0);
                    if (bladeWeapon.CurrentDurability == 0f)
                    {
                        Human.ToggleBlades(false);
                        if (bladeWasEnabled)
                            Human.PlaySound(HumanSounds.BladeBreak);
                    }
                }
            }
            else if (name == "MaxBladeDurability")
            {
                if (bladeWeapon != null)
                    bladeWeapon.MaxDurability = value.UnboxToFloat();
            }
            else if (name == "CurrentBlade")
            {
                if (bladeWeapon != null)
                    bladeWeapon.BladesLeft = Mathf.Min(value.UnboxToInt(), bladeWeapon.MaxBlades);
            }
            else if (name == "MaxBlade")
            {
                if (bladeWeapon != null)
                    bladeWeapon.MaxBlades = value.UnboxToInt();
            }
            else if (name == "CurrentAmmoRound")
            {
                if (ammoWeapon != null)
                    ammoWeapon.RoundLeft = Mathf.Min(ammoWeapon.MaxRound, value.UnboxToInt());
            }
            else if (name == "MaxAmmoRound")
            {
                if (ammoWeapon != null)
                    ammoWeapon.MaxRound = value.UnboxToInt();
            }
            else if (name == "CurrentAmmoLeft")
            {
                if (ammoWeapon != null)
                    ammoWeapon.AmmoLeft = Mathf.Min(ammoWeapon.MaxAmmo, value.UnboxToInt());
            }
            else if (name == "MaxAmmoTotal")
            {
                if (ammoWeapon != null)
                    ammoWeapon.MaxAmmo = value.UnboxToInt();
            }
            else if (name == "LeftHookEnabled")
                Human.HookLeft.Enabled = (bool)value;
            else if (name == "RightHookEnabled")
                Human.HookRight.Enabled = (bool)value;
            else if (name == "Position")
            {
                Human.IsChangingPosition();
                base.SetField(name, value);
            }
            else if (name == "CanDodge")
                Human.CanDodge = (bool)value;
            else if (name == "IsInvincible")
                Human.IsInvincible = (bool)value;
            else if (name == "InvincibleTimeLeft")
                Human.InvincibleTimeLeft = (float)value;
            else
                base.SetField(name, value);
            Human.Stats.UpdateStats();
        }
    }
}
