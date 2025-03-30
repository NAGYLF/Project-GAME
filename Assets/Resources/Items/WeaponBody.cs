using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using static MainData.Main;
using System.Linq;
using UnityEngine.UI;

namespace Items
{
    public class WeaponBody : IItemComponent
    {
        public int MagasineSize { get; set; }
        public Stack<IItemComponent> ActualAmmo { get; set; }
        public double Spread { get; set; }
        public int Fpm { get; set; }
        public double Recoil { get; set; }
        public double Accturacy { get; set; }
        public double Range { get; set; }
        public double Ergonomy { get; set; }
        public string CompatibleCaliber { get; set; }

        private bool isShooting = false;
        private bool isReloading = false;
        private bool isUnloading = false;

        private AdvancedItem advancedItem;
        private Part selfPart;
        private WeaponBody()
        {

        }
        public WeaponBody(AdvancedItemStruct advancedItemStruct)
        {
            MagasineSize = advancedItemStruct.MagasineSize;
            ActualAmmo = new Stack<IItemComponent>();
            Spread = advancedItemStruct.Spread;
            Fpm = advancedItemStruct.Fpm;
            Recoil = advancedItemStruct.Recoil;
            Accturacy = advancedItemStruct.Accturacy;
            Range = advancedItemStruct.Range;
            Ergonomy = advancedItemStruct.Ergonomy;
            CompatibleCaliber = advancedItemStruct.CompatibleCaliber;
        }
        public IItemComponent CloneComponent()
        {
            return new WeaponBody()
            {
                MagasineSize = this.MagasineSize,
                ActualAmmo = new Stack<IItemComponent>(this.ActualAmmo.Select(ammo => ammo.CloneComponent()).Reverse()),
                Spread = this.Spread,
                Fpm = this.Fpm,
                Recoil = this.Recoil,
                Accturacy = this.Accturacy,
                Range = this.Range,
                Ergonomy = this.Ergonomy,
                CompatibleCaliber = this.CompatibleCaliber
            };
        }
        public void Inicialisation(AdvancedItem advancedItem, Part selfPart)
        {
            this.advancedItem = advancedItem;
            this.selfPart = selfPart;
        }
        public IEnumerator Control(bool Shoot, bool Reload, bool Use, bool Unload, bool Aim)
        {
            if (Shoot && Input.GetMouseButton(0) && !isShooting) // Bal klikk
            {
                Debug.LogWarning("FIre on");
                isShooting = true;

                var gg = advancedItem.Parts.SelectMany(part => part.SystemPoints);
                    
                var sp =   gg.LastOrDefault(sp => sp.SPData.PointName == "Fire");
                GameObject Bullet = new GameObject("Bullet");
                GameObject Fire = new GameObject("Fire", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));

                // Szülõ beállítása
                RectTransform rectTransform = Fire.GetComponent<RectTransform>();
                rectTransform.SetParent(sp.RefPoint1.transform.parent.transform,false);

                // Méret (sprite alapján, vagy fix érték)
                int randomIndex = Random.Range(1, 3); // 1 vagy 2 (a felsõ határ kizárva)
                string path = $"Textures/EffectTextures/fire{randomIndex}";
                Sprite sprite = Resources.Load<Sprite>(path);
                Vector2 size = sprite.rect.size;

                rectTransform.pivot = new Vector2(1f, 0.5f);
                rectTransform.sizeDelta = size*0.2f;
                rectTransform.anchoredPosition = sp.RefPoint1.transform.localPosition;

                rectTransform.SetParent(advancedItem.InGameSelfObject.GetComponent<InGameItemObject>().ItemCompound.GetComponent<ItemImgFitter>().fitter, true);
                // Kép beállítása
                Image img = Fire.GetComponent<Image>();
                img.sprite = sprite;
                img.preserveAspect = true;



                float delay = 60f /Fpm;



                yield return new WaitForSeconds(delay);


                GameObject.Destroy(Bullet);
                GameObject.Destroy(Fire);

                isShooting = false;
            }

            if (Reload && Input.GetKeyDown(KeyCode.R) && !isReloading) // R lenyomás
            {
                // Tölt újra
            }

            if (Unload && Input.GetKeyDown(KeyCode.U) &&!isUnloading) // U lenyomás
            {
                // Ürít
            }

            if (Aim && Input.GetMouseButton(1)) // Jobb klikk lenyomva
            {
                // Céloz
            }
            yield return null;
        }
        public IEnumerable Shoot()
        {
            yield return null;
        }
        public IEnumerable Reload(AdvancedItem advancedItem, Part selfPart)
        {
            SimpleItem KeyItem = selfPart.item_s_Part;
            PlayerInventoryClass.PlayerInventory.LevelManager playerInventoryItems = advancedItem.LevelManagerRef;



            //AudioSource audioComponent = advancedItem.SelfGameobject.GetComponent<AudioSource>();

            ////tar kivetele
            //AudioClip audioClip = Resources.Load<AudioClip>("Hangok/Loves");
            //audioComponent.PlayOneShot(audioClip);

            //inventoryba helyzi a megmaradt loszert ha van
            //Item RemaingAmmo = KeyItem

            //playerInventoryItems.Items.Find(item=>item.Caliber == KeyItem.Caliber);

            //yield return new WaitForSeconds(audioClip.length);

            ////tar behelyezese
            //audioClip = Resources.Load<AudioClip>("Hangok/Loves");
            //audioComponent.PlayOneShot(audioClip);

            //yield return new WaitForSeconds(audioClip.length);

            //inevntorybol eltavolitja a szukseges vagy talat menyiseget a loszerbol
            yield return null;
        }
    }
}
