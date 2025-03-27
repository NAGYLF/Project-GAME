using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using DataHandler;

namespace Items
{
    public class AssaultRifles
    {
        public static IEnumerable Reload(Item MainItem, Part Part)
        {
            Item KeyItem = Part.item_s_Part;
            PlayerInventoryClass.PlayerInventory.LevelManager playerInventoryItems = MainItem.LevelManagerRef;
            AudioSource audioComponent = MainItem.SelfGameobject.GetComponent<AudioSource>();

            //tar kivetele
            AudioClip audioClip = Resources.Load<AudioClip>("Hangok/Loves");
            audioComponent.PlayOneShot(audioClip);

            //inventoryba helyzi a megmaradt loszert ha van
            //Item RemaingAmmo = KeyItem

            //playerInventoryItems.Items.Find(item=>item.Caliber == KeyItem.Caliber);

            yield return new WaitForSeconds(audioClip.length);

            //tar behelyezese
            audioClip = Resources.Load<AudioClip>("Hangok/Loves");
            audioComponent.PlayOneShot(audioClip);

            yield return new WaitForSeconds(audioClip.length);

            //inevntorybol eltavolitja a szukseges vagy talat menyiseget a loszerbol
        }
    }
}
