using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using static MainData.Main;
using System.Linq;
using UnityEngine.UI;
using UI;
using Unity.VisualScripting;

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
            if (Shoot && Input.GetMouseButton(0) && !isReloading && !isShooting && !isUnloading) // Bal klikk
            {
                isShooting = true;
                Debug.LogWarning("FIre on");

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
                rectTransform.sizeDelta = size*0.2f;
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

                float delay = 60f /Fpm;



                AudioSource audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
                AudioClip audioClipShoot = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTShoot");
                audioSource.PlayOneShot(audioClipShoot);

                yield return new WaitForSeconds(delay);

                GameObject.Destroy(Fire);

                isShooting = false;

                float waitTime = (float)Mathf.Round(Random.Range(0.3f, 0.8f) * 10000f) / 10000f;
                yield return new WaitForSeconds(waitTime);

                AudioClip audioClipCasingDrop = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTCasingDrop");
                audioSource.PlayOneShot(audioClipCasingDrop);
            }

            if (Reload && Input.GetKeyDown(KeyCode.R) && !isReloading && !isShooting && !isUnloading) // R lenyomás
            {
                isReloading = true;

                AudioSource audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
                AudioClip audioClip = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTUnload");
                audioSource.PlayOneShot(audioClip);

                yield return new WaitForSeconds(audioClip.length);

                audioClip = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTReload");
                audioSource.PlayOneShot(audioClip);

                yield return new WaitForSeconds(audioClip.length);

                audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
                audioClip = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTChamber");
                audioSource.PlayOneShot(audioClip);

                yield return new WaitForSeconds(audioClip.length);

                isReloading = false;
            }

            if (Unload && Input.GetKeyDown(KeyCode.U) && !isReloading && !isShooting && !isUnloading) // U lenyomás
            {
                isUnloading = true;

                AudioSource audioSource = advancedItem.InGameSelfObject.GetComponent<AudioSource>();
                AudioClip audioClip = Resources.Load<AudioClip>("Sounds/WeaponTEST/TESTUnload");
                audioSource.PlayOneShot(audioClip);

                yield return new WaitForSeconds(audioClip.length);

                isUnloading = false;
            }

            if (Aim && Input.GetMouseButton(1)) // Jobb klikk lenyomva
            {
                // Céloz
            }
            yield return null;
        }
    }
}
