using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using static MainData.Main;
using System.Linq;
using UnityEngine.UI;
using UI;
using System;
using static CharacterHand;

namespace Items
{
    public class Magasine : MonoBehaviour, IItemComponent
    {
        public string SystemName { get; private set; }
        public int MagasineSize { get; set; }
        public Stack<Ammunition> ContainedAmmo = new Stack<Ammunition>();
        public double Ergonomy { get; set; }
        public float Caliber_Weapon { get; set; }
        public float CartridgeSize_Weapon { get; set; }

        private AdvancedItem advancedItem;

        public Magasine(AdvancedItemStruct advancedItemStruct)
        {
            SystemName = advancedItemStruct.SystemName;
            MagasineSize = advancedItemStruct.MagasineSize;
            Ergonomy = advancedItemStruct.Ergonomy;
            Caliber_Weapon = advancedItemStruct.Caliber_Weapon;
            CartridgeSize_Weapon = advancedItemStruct.CartridgeSize_Weapon;
        }

        public IItemComponent CloneComponent()
        {
            throw new NotImplementedException();
        }

        public void Control(InputFrameData input)
        {
            Debug.LogWarning($"magasine in action {input.ReloadPressed && !advancedItem.IsReloading && !advancedItem.IsShooting && !advancedItem.IsUnloading}");
            if (input.ReloadPressed && !advancedItem.IsReloading && !advancedItem.IsShooting && !advancedItem.IsUnloading)
            {
                Debug.LogWarning("magasine in action");
               
                StartCoroutine(Reload());
            }
        }

        public void Inicialisation(AdvancedItem advancedItem)
        {
            this.advancedItem = advancedItem;
        }

        public IEnumerator Reload()
        {
            Debug.LogWarning("reload magasine");
            advancedItem.IsReloading = true;
            AudioSource audioSource;
            AudioClip audioClip;
            WeaponBody body = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(WeaponBody)).item_s_Part.Component as WeaponBody);
            Magasine magasine = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(Magasine)).item_s_Part.Component as Magasine);
            //ha van body
            if (body != null)
            {
                //ha a tar tele van
                if (magasine.MagasineSize == magasine.ContainedAmmo.Count)
                {
                    //ha a chamber nincs tele
                    if (body.Chamber.Count < body.ChamberSize)
                    {
                        body.Reload();
                    }
                }
                //Ha a tar nincs tele
                else
                {
                    //keresssen loszert
                    AdvancedItem[] possibleAmmoes = advancedItem.LevelManagerRef.Items.Where(item =>item.Components.TryGetValue(typeof(Ammunition), out var component) && component is Ammunition ammo && ammo.Caliber == Caliber_Weapon && ammo.CartridgeSize == CartridgeSize_Weapon).ToArray();
                    if (possibleAmmoes.Length>0)
                    {
                        audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
                        audioClip = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTUnload");
                        audioSource.PlayOneShot(audioClip);

                        yield return new WaitForSeconds(audioClip.length);

                        List<AdvancedItem> RemovableAdvancedItems = new List<AdvancedItem>();
                        List<AdvancedItem> AddableAdvancedItems = new List<AdvancedItem>();
                        int nessesaryAmmo = MagasineSize-ContainedAmmo.Count;
                        int index = 0;
                        while (nessesaryAmmo > 0 && index<possibleAmmoes.Length)
                        {
                            if (possibleAmmoes[index].Quantity - nessesaryAmmo > 0)
                            {
                                AddableAdvancedItems.Add(possibleAmmoes[index]);
                                possibleAmmoes[index].Quantity -= nessesaryAmmo;
                            }
                            else
                            {
                                nessesaryAmmo-=possibleAmmoes[index].Quantity;
                                AddableAdvancedItems.Add(possibleAmmoes[index]);
                                RemovableAdvancedItems.Add(possibleAmmoes[index]);
                            }
                        }

                        //int index2 = 0;
                        //for (int i = MagasineSize - ContainedAmmo.Count; i > 0; index2++ , i = MagasineSize - ContainedAmmo.Count)
                        //{
                        //    for (int j = 0; j < AddableAdvancedItems[index2].Quantity && MagasineSize - ContainedAmmo.Count > 0; j++)
                        //    {
                        //        ContainedAmmo.Push((AddableAdvancedItems[index2].Components.First(component=>component.Value.GetType() == typeof(Ammunition)).Value as Ammunition).CloneComponent() as Ammunition);
                        //    }
                        //}

                        int index2 = 0;
                        while (ContainedAmmo.Count < MagasineSize && index2 < AddableAdvancedItems.Count)
                        {
                            var item = AddableAdvancedItems[index2];

                            if (!item.Components.TryGetValue(typeof(Ammunition), out var comp))
                            {
                                index2++;
                                continue;
                            }

                            var ammo = comp as Ammunition;
                            if (ammo == null)
                            {
                                index2++;
                                continue;
                            }

                            while (item.Quantity > 0 && ContainedAmmo.Count < MagasineSize)
                            {
                                ContainedAmmo.Push(ammo.CloneComponent() as Ammunition);
                                item.Quantity--;
                            }

                            index2++;
                        }

                        foreach (AdvancedItem item in RemovableAdvancedItems)
                        {
                            InventorySystem.Delete(item);
                        }


                        audioClip = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTReload");
                        audioSource.PlayOneShot(audioClip);

                        yield return new WaitForSeconds(audioClip.length);

                        //ha a chamber nincs tele
                        if (body.Chamber.Count < body.ChamberSize)
                        {
                            body.Reload();
                        }
                    }
                }
            }
            advancedItem.IsReloading = false;
        }
    }
}
