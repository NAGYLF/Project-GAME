using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using static MainData.Main;
using System.Linq;
using UnityEngine.UI;
using UI;
using static CharacterHand;

namespace Items
{
    public class WeaponBody : IItemComponent
    {
        public int ChamberSize { get; set; }
        public Stack<Ammunition> Chamber { get; set; }
        public double Spread { get; set; }
        public int Fpm { get; set; }
        public double Recoil { get; set; }
        public double Accturacy { get; set; }
        public double Range { get; set; }
        public double Ergonomy { get; set; }
        public float Caliber_Weapon { get; set; }
        public float CartridgeSize_Weapon { get; set; }

        private AdvancedItem advancedItem;
        private WeaponBody()
        {

        }
        public WeaponBody(AdvancedItemStruct advancedItemStruct)
        {
            ChamberSize = advancedItemStruct.MagasineSize;
            Chamber = new Stack<Ammunition>();
            Spread = advancedItemStruct.Spread;
            Fpm = advancedItemStruct.Fpm;
            Recoil = advancedItemStruct.Recoil;
            Accturacy = advancedItemStruct.Accturacy;
            Range = advancedItemStruct.Range;
            Ergonomy = advancedItemStruct.Ergonomy;
            Caliber_Weapon = advancedItemStruct.Caliber_Weapon;
            CartridgeSize_Weapon = advancedItemStruct.CartridgeSize_Weapon;
        }
        public IItemComponent CloneComponent()
        {
            return new WeaponBody()
            {
                ChamberSize = this.ChamberSize,
                Chamber = new Stack<Ammunition>((IEnumerable<Ammunition>)this.Chamber.Select(ammo => ammo.CloneComponent()).Reverse()),
                Spread = this.Spread,
                Fpm = this.Fpm,
                Recoil = this.Recoil,
                Accturacy = this.Accturacy,
                Range = this.Range,
                Ergonomy = this.Ergonomy,
                Caliber_Weapon= this.Caliber_Weapon,
                CartridgeSize_Weapon= this.CartridgeSize_Weapon,
            };
        }
        public void Inicialisation(AdvancedItem advancedItem)
        {
            this.advancedItem = advancedItem;
        }
        public IEnumerator Control(InputFrameData input)
        {
            if (input.ShootPressed && !advancedItem.IsReloading && !advancedItem.IsShooting && !advancedItem.IsUnloading) // Bal klikk
            {
                advancedItem.IsShooting = true;

                var sp = advancedItem.Parts.SelectMany(part => part.SystemPoints).LastOrDefault(sp => sp.SPData.PointName == "Fire");



                //muzzle flash
                GameObject Fire = new GameObject("Fire", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

                RectTransform rectTransform = Fire.GetComponent<RectTransform>();
                rectTransform.SetParent(sp.InGameRefPoint1.transform.parent.transform, false);

                int randomIndex = Random.Range(1, 3);
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
                renderer.sprite = Resources.Load<Sprite>("Textures/EffectTextures/BulletTEST");
                renderer.sortingOrder = 10;
                renderer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // kisebb méret

                SimpleItem ammunitionTest = new SimpleItem(DataHandler.GetAdvancedItemData("7.62x39FMJ"));
                var bulletScript = Bullet.AddComponent<Bullet>();
                bulletScript.Initialize(ammunitionTest.Component as Ammunition, InGameUI.Player.GetComponent<Player>().Hand.transform.right);

                float delay = 60f / Fpm;



                AudioSource audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
                AudioClip audioClipShoot = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTShoot");
                audioSource.PlayOneShot(audioClipShoot);

                yield return new WaitForSeconds(delay);

                GameObject.Destroy(Fire);

                advancedItem.IsShooting = false;

                float waitTime = (float)Mathf.Round(Random.Range(0.3f, 0.8f) * 10000f) / 10000f;
                yield return new WaitForSeconds(waitTime);

                AudioClip audioClipCasingDrop = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTCasingDrop");
                audioSource.PlayOneShot(audioClipCasingDrop, 0.1f);
            }

            if (input.ReloadPressed && !advancedItem.IsReloading && !advancedItem.IsShooting && !advancedItem.IsUnloading) // R lenyomás
            {
                yield return Reload();
            }

            if (input.UnloadPressed && !advancedItem.IsReloading && !advancedItem.IsShooting && !advancedItem.IsUnloading) // U lenyomás
            {
                advancedItem.IsUnloading = true;

                AudioSource audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
                AudioClip audioClip = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTUnload");
                audioSource.PlayOneShot(audioClip);

                yield return new WaitForSeconds(audioClip.length);

                advancedItem.IsUnloading = false;
            }

            if (input.AimPressed) // Jobb klikk lenyomva
            {
                // Céloz
            }
            yield return null;
        }

        public IEnumerator Reload()
        {
            Debug.LogWarning("reload body");
            advancedItem.IsReloading = true;
            AudioSource audioSource;
            AudioClip audioClip;
            WeaponBody body = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(WeaponBody)).item_s_Part.Component as WeaponBody);
            Magasine magasine = (advancedItem.Parts.FirstOrDefault(part => part.item_s_Part.ItemType == nameof(Magasine)).item_s_Part.Component as Magasine);
            //ha van tar
            if (magasine != null)
            {
                //ha a tarban van loszer
                if (magasine.MagasineSize > magasine.ContainedAmmo.Count)
                {
                    //ha a chamber nincs tele
                    if (body.Chamber.Count < body.ChamberSize)
                    {
                        //chamber tarazas
                        audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
                        audioClip = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTChamber");
                        audioSource.PlayOneShot(audioClip);

                        yield return new WaitForSeconds(audioClip.length);

                        for (int i = 0; 0 < magasine.ContainedAmmo.Count && i < body.ChamberSize; i++)
                        {
                            Ammunition ammunition = magasine.ContainedAmmo.Pop();
                            body.Chamber.Push(ammunition);
                        }
                    }
                }
            }
            advancedItem.IsReloading = false;
        }
    }
}
