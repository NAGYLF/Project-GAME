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
    internal class AmmoToAdd
    {
        public AdvancedItem SourceItem;
        public Ammunition AmmoTemplate;
        public int Count;
    }
    public class Magasine : IItemComponent
    {
        public string SystemName { get; private set; }
        public int MagasineSize { get; set; }
        public Stack<Ammunition> ContainedAmmo = new Stack<Ammunition>();
        public double Ergonomy { get; set; }
        public float Caliber { get; set; }
        public float CartridgeSize { get; set; }

        private AdvancedItem advancedItem;

        public Magasine(AdvancedItemStruct advancedItemStruct)
        {
            SystemName = advancedItemStruct.SystemName;
            MagasineSize = advancedItemStruct.MagasineSize;
            Ergonomy = advancedItemStruct.Ergonomy;
            Caliber = advancedItemStruct.Caliber;
            CartridgeSize = advancedItemStruct.CartridgeSize;
        }

        public IItemComponent CloneComponent()
        {
            throw new NotImplementedException();
        }

        public IEnumerator Control(InputFrameData input)
        {
            Debug.LogWarning($"magasine in action {input.ReloadPressed && !advancedItem.IsReloading && !advancedItem.IsShooting && !advancedItem.IsUnloading}");
            if (input.ReloadPressed && !advancedItem.IsReloading && !advancedItem.IsShooting && !advancedItem.IsUnloading)
            {
                yield return Reload();
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
            if (body != null && magasine != null)
            {
                //ha a tar tele van
                Debug.LogWarning($"magaisne 1 {magasine.MagasineSize}  {magasine.ContainedAmmo.Count}");
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
                    Debug.LogWarning($"magasine datas: {Caliber}  {CartridgeSize}");
                    AdvancedItem[] possibleAmmoes = advancedItem.LevelManagerRef.Items.Where(item =>item.Components.TryGetValue(typeof(Ammunition), out var component) && component is Ammunition ammo && ammo.Caliber == Caliber && ammo.CartridgeSize == CartridgeSize).ToArray();
                    Debug.LogWarning(possibleAmmoes.Length);
                    if (possibleAmmoes.Length>0)
                    {
                        audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
                        audioClip = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTUnload");
                        audioSource.PlayOneShot(audioClip);

                        yield return new WaitForSeconds(audioClip.length);

                        List<AdvancedItem> RemovableAdvancedItems = new List<AdvancedItem>();
                        List<AdvancedItem> AddableAdvancedItems = new List<AdvancedItem>();
                        List<AmmoToAdd> ammoToAddList = new List<AmmoToAdd>();
                        int nessesaryAmmo = MagasineSize-ContainedAmmo.Count;
                        Debug.LogWarning($"nessesay ammo  {nessesaryAmmo}");
                        int index = 0;
                        while (nessesaryAmmo > 0 && index < possibleAmmoes.Length)
                        {
                            var item = possibleAmmoes[index];

                            if (!item.Components.TryGetValue(typeof(Ammunition), out var comp) || comp is not Ammunition ammo)
                            {
                                index++;
                                continue;
                            }

                            if (item.Quantity > nessesaryAmmo)
                            {
                                ammoToAddList.Add(new AmmoToAdd { SourceItem = item, AmmoTemplate = ammo, Count = nessesaryAmmo });
                                item.Quantity -= nessesaryAmmo;
                                nessesaryAmmo = 0;
                            }
                            else
                            {
                                ammoToAddList.Add(new AmmoToAdd { SourceItem = item, AmmoTemplate = ammo, Count = item.Quantity });
                                nessesaryAmmo -= item.Quantity;
                                RemovableAdvancedItems.Add(item);
                            }

                            index++;
                        }

                        //int index2 = 0;
                        //for (int i = MagasineSize - ContainedAmmo.Count; i > 0; index2++ , i = MagasineSize - ContainedAmmo.Count)
                        //{
                        //    for (int j = 0; j < AddableAdvancedItems[index2].Quantity && MagasineSize - ContainedAmmo.Count > 0; j++)
                        //    {
                        //        ContainedAmmo.Push((AddableAdvancedItems[index2].Components.First(component=>component.Value.GetType() == typeof(Ammunition)).Value as Ammunition).CloneComponent() as Ammunition);
                        //    }
                        //}

                        foreach (var ammoEntry in ammoToAddList)
                        {
                            for (int i = 0; i < ammoEntry.Count && ContainedAmmo.Count < MagasineSize; i++)
                            {
                                ContainedAmmo.Push(ammoEntry.AmmoTemplate.CloneComponent() as Ammunition);
                            }
                        }

                        foreach (AdvancedItem item in RemovableAdvancedItems)
                        {
                            InventorySystem.Delete(item);
                        }


                        audioClip = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTReload");
                        audioSource.PlayOneShot(audioClip);

                        yield return new WaitForSeconds(audioClip.length);

                        foreach (var item in AddableAdvancedItems)
                        {
                            if (item.SelfGameobject != null)
                            {
                                item.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                            }
                        }

                        //ha a chamber nincs tele
                        if (body.Chamber.Count < body.ChamberSize)
                        {
                            Debug.LogWarning($"body.Chamber.Count {body.Chamber.Count}       body.ChamberSize {body.ChamberSize}");
                            yield return body.Reload();
                        }
                    }
                }
            }
            advancedItem.IsReloading = false;
        }
    }
}
