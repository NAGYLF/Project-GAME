using ItemHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;
using static MainData.Main;
using UnityEngine.UI;

namespace Items
{
    internal class Carbine : IItemComponent, IItemControl
    {
        public string ShootSoundPath { get; set; }
        public string ReloadSoundPath { get; set; }
        public string UnloadSoundPath { get; set; }
        public string ChamberSoundPath { get; set; }

        public string BulletTexturePath { get; set; }

        private AdvancedItem advancedItem { get; set; }
        public Carbine() 
        {

        }
        public Carbine(MainItem mainItem)
        {
            ShootSoundPath = mainItem.ShootSoundPath;
            ReloadSoundPath = mainItem.ReloadSoundPath;
            UnloadSoundPath = mainItem.UnloadSoundPath;
            ChamberSoundPath = mainItem.ChamberSoundPath;

            BulletTexturePath = mainItem.BulletTexturePath;
        }

        public IItemComponent CloneComponent()
        {
            return new Carbine()
            {
                ShootSoundPath = this.ShootSoundPath,
                ReloadSoundPath = this.ShootSoundPath,
                UnloadSoundPath = this.UnloadSoundPath,
                ChamberSoundPath = this.ChamberSoundPath,

                BulletTexturePath = this.BulletTexturePath,
            };
        }

        public IEnumerator Control(InputFrameData input)
        {
            if (input.ShootPressed && !advancedItem.IsReloading && !advancedItem.IsShooting && !advancedItem.IsUnloading) // Bal klikk
            {
                yield return Shoot();
            }

            if (input.ReloadPressed && !advancedItem.IsReloading && !advancedItem.IsShooting && !advancedItem.IsUnloading) // R lenyomás
            {
                yield return Reload();
            }

            if (input.UnloadPressed && !advancedItem.IsReloading && !advancedItem.IsShooting && !advancedItem.IsUnloading) // U lenyomás
            {
                advancedItem.IsUnloading = true;

                AudioSource audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
                AudioClip audioClip = Resources.Load<AudioClip>(this.UnloadSoundPath);
                audioSource.PlayOneShot(audioClip);

                yield return new WaitForSeconds(audioClip.length);

                advancedItem.IsUnloading = false;
            }

            if (input.AimPressed) // Jobb klikk lenyomva
            {
                // Céloz
            }
        }

        public IEnumerator Shoot()
        {
            Magasine magasine = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(Magasine))?.item_s_Part.Component as Magasine);
            WeaponBody weaponBody = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(WeaponBody))?.item_s_Part.Component as WeaponBody);

            if (weaponBody.Chamber.Count > 0)
            {
                advancedItem.IsShooting = true;

                var sp = advancedItem.Parts.SelectMany(part => part.SystemPoints).LastOrDefault(sp => sp.SPData.PointName == "Fire");



                //muzzle flash
                GameObject Fire = new GameObject("Fire", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

                RectTransform rectTransform = Fire.GetComponent<RectTransform>();
                rectTransform.SetParent(sp.InGameRefPoint1.transform.parent.transform, false);

                int randomIndex = UnityEngine.Random.Range(1, 3);
                string path = $"Textures/EffectTextures/fire{randomIndex}";
                Sprite sprite = Resources.Load<Sprite>(path);
                Vector2 size = sprite.rect.size;

                rectTransform.pivot = new Vector2(1f, 0.5f);
                rectTransform.sizeDelta = size * 0.2f;
                rectTransform.anchoredPosition = sp.InGameRefPoint1.transform.localPosition;

                Image img = Fire.GetComponent<Image>();
                img.sprite = sprite;
                img.preserveAspect = true;

                rectTransform.SetParent(advancedItem.InGameSelfObject.GetComponent<InGameItemObject>().ItemCompound.GetComponent<ItemImgFitter>().fitter, true);



                //bullet
                GameObject Bullet = new GameObject("Bullet");
                Bullet.transform.position = sp.InGameRefPoint1.transform.position;
                Bullet.transform.rotation = Quaternion.LookRotation(Vector3.forward, InGameUI.Player.GetComponent<Player>().Hand.transform.up); // irányhoz igazítva

                SpriteRenderer renderer = Bullet.AddComponent<SpriteRenderer>();
                renderer.sprite = Resources.Load<Sprite>(this.BulletTexturePath);
                renderer.sortingOrder = 10;
                renderer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // kisebb méret


                Ammunition ammo = weaponBody.Chamber.Pop();

                var bulletScript = Bullet.AddComponent<Bullet>();
                bulletScript.Initialize(ammo, InGameUI.Player.GetComponent<Player>().Hand.transform.right);


                if (magasine != null && magasine.ContainedAmmo.Count > 0)
                {
                    weaponBody.Chamber.Push(magasine.ContainedAmmo.Pop());
                }


                float delay = 60f / weaponBody.Fpm;



                AudioSource audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
                AudioClip audioClipShoot = Resources.Load<AudioClip>(this.ShootSoundPath);
                audioSource.PlayOneShot(audioClipShoot);

                yield return new WaitForSeconds(delay);

                GameObject.Destroy(Fire);

                advancedItem.IsShooting = false;

                advancedItem.InGameSelfObject.GetComponent<MonoBehaviour>().StartCoroutine(CaseDrop());
            }
        }
        private IEnumerator ReloadMagasine()
        {
            AudioSource audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
            AudioClip audioClip;

            Magasine magasine = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(Magasine))?.item_s_Part.Component as Magasine);
            WeaponBody weaponBody = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(WeaponBody))?.item_s_Part.Component as WeaponBody);

            audioClip = Resources.Load<AudioClip>(this.UnloadSoundPath);
            audioSource.PlayOneShot(audioClip);
            yield return new WaitForSeconds(audioClip.length);

            audioClip = Resources.Load<AudioClip>(this.ReloadSoundPath);
            audioSource.PlayOneShot(audioClip);
            yield return new WaitForSeconds(audioClip.length);

            int nessesaryAmmo = magasine.MagasineSize - magasine.ContainedAmmo.Count;
            List<AmmoToAdd> ammoToAddList = new();
            List<AdvancedItem> removableItems = new();

            var possibleAmmoes = advancedItem.LevelManagerRef.Items
                .Where(item => item.Components.TryGetValue(typeof(Ammunition), out var component)
                    && component is Ammunition ammo
                    && ammo.Caliber == weaponBody.Caliber
                    && ammo.CartridgeSize == weaponBody.CartridgeSize)
                .ToArray();

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
                    removableItems.Add(item);
                }

                index++;
            }

            foreach (var ammoEntry in ammoToAddList)
            {
                for (int i = 0; i < ammoEntry.Count && magasine.ContainedAmmo.Count < magasine.MagasineSize; i++)
                {
                    magasine.ContainedAmmo.Push(ammoEntry.AmmoTemplate.CloneComponent() as Ammunition);
                }
                if (ammoEntry.SourceItem.SelfGameobject != null)
                {
                    ammoEntry.SourceItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                }
            }

            foreach (var item in removableItems)
            {
                InventorySystem.Delete(item);
            }
        }
        private IEnumerator ReloadChamber()
        {
            AudioSource audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
            AudioClip audioClip;

            Magasine magasine = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(Magasine))?.item_s_Part.Component as Magasine);
            WeaponBody weaponBody = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(WeaponBody))?.item_s_Part.Component as WeaponBody);

            audioClip = Resources.Load<AudioClip>(this.ChamberSoundPath);
            audioSource.PlayOneShot(audioClip);
            yield return new WaitForSeconds(audioClip.length);

            for (int i = 0; magasine.ContainedAmmo.Count > 0 && weaponBody.Chamber.Count < weaponBody.ChamberSize; i++)
            {
                weaponBody.Chamber.Push(magasine.ContainedAmmo.Pop());
            }
        }
        private IEnumerator DirectReloadChamber()
        {
            AudioSource audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
            AudioClip audioClip;

            WeaponBody weaponBody = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(WeaponBody))?.item_s_Part.Component as WeaponBody);

            audioClip = Resources.Load<AudioClip>(this.ChamberSoundPath);
            audioSource.PlayOneShot(audioClip);
            yield return new WaitForSeconds(audioClip.length);

            int nessesaryAmmo = weaponBody.ChamberSize - weaponBody.Chamber.Count;

            List<AmmoToAdd> ammoToAddList = new();
            List<AdvancedItem> removableItems = new();

            var possibleAmmoes = advancedItem.LevelManagerRef.Items
                .Where(item => item.Components.TryGetValue(typeof(Ammunition), out var component)
                    && component is Ammunition ammo
                    && ammo.Caliber == weaponBody.Caliber
                    && ammo.CartridgeSize == weaponBody.CartridgeSize)
                .ToArray();

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
                    removableItems.Add(item);
                }

                index++;
            }

            foreach (var ammoEntry in ammoToAddList)
            {
                for (int i = 0; i < ammoEntry.Count && weaponBody.Chamber.Count < weaponBody.ChamberSize; i++)
                {
                    weaponBody.Chamber.Push(ammoEntry.AmmoTemplate.CloneComponent() as Ammunition);
                }
                if (ammoEntry.SourceItem.SelfGameobject != null)
                {
                    ammoEntry.SourceItem.SelfGameobject.GetComponent<ItemObject>().SelfVisualisation();
                }
            }

            foreach (var item in removableItems)
            {
                InventorySystem.Delete(item);
            }
        }
        public IEnumerator Reload()
        {
            advancedItem.IsReloading = true;

            Magasine magasine = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(Magasine))?.item_s_Part.Component as Magasine);
            WeaponBody weaponBody = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(WeaponBody))?.item_s_Part.Component as WeaponBody);

            if (weaponBody == null)
            {
                Debug.LogError("WeaponBody not found.");
                advancedItem.IsReloading = false;
                yield break;
            }

            // ✅ Ha van tár és benne van lőszer
            if (magasine != null)
            {
                //ha a magasineban van loszer
                if (magasine.ContainedAmmo.Count > 0)
                {
                    // ha a chamber nincs tele
                    if (weaponBody.Chamber.Count < weaponBody.ChamberSize)
                    {
                        yield return ReloadChamber();
                    }
                    // ha a chamebr tele van de a magasine-ben van hely
                    else if (magasine.ContainedAmmo.Count < magasine.MagasineSize)
                    {
                        yield return ReloadMagasine();
                    }
                }
                //ha a magasineban nincs loszer
                else
                {
                    yield return ReloadMagasine();

                    if (weaponBody.Chamber.Count < weaponBody.ChamberSize)
                    {
                        yield return ReloadChamber();
                    }
                }
            }
            //ha nincs magasine
            else if(weaponBody.ChamberSize > weaponBody.Chamber.Count)
            {
                yield return DirectReloadChamber();
            }

            advancedItem.IsReloading = false;
        }
        private IEnumerator CaseDrop()
        {
            float waitTime = (float)Mathf.Round(UnityEngine.Random.Range(0.3f, 0.8f) * 10000f) / 10000f;
            yield return new WaitForSeconds(waitTime);

            AudioSource audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
            AudioClip audioClipCasingDrop = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTCasingDrop");
            audioSource.PlayOneShot(audioClipCasingDrop, 0.1f);
        }
        public void Inicialisation(AdvancedItem advancedItem)
        {
            this.advancedItem = advancedItem;
        }
    }
}
