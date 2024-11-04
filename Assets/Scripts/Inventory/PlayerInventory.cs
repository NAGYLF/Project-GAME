using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemHandler;
using System.IO;
using Newtonsoft.Json;
using Unity.VisualScripting;
using Assets.Scripts;
using System;
using Weapons;
using Backpacks;
using Vests;
using Armors;
using System.Reflection;
using System.Linq;
using System.Drawing;
using MainData;
using static MainData.SupportScripts;
using PlayerInventoryVisualBuild;
using PlayerInventoryClass;
using static PlayerInventoryClass.PlayerInventory;



namespace PlayerInventoryClass
{
    public class PlayerInventory : MonoBehaviour
    {
        public static PlayerInventory playerInventoryData;//a player mindenhol elerheto inventoryja ezzel tortenik meg a mentes is

        public Equipmnets equipments;

        public class Equipmnets
        {
            public List<EquipmnetStruct> equipmnetsData;
            public Equipmnets()
            {
                //1.az equpmentek adatlist�j�t p�d�nyos�tjuk
                equipmnetsData = new List<EquipmnetStruct>();
                Transform transform = Resources.Load<GameObject>("GameElements/Equipment-Inventory").transform;
                for (int i = 0; i < transform.childCount; i++)
                {
                    //2. az equipment adatlist�t felt�ltj�k az er�re megalkotot prefab adataival, tov�bb� l�trehoz egy ures item p�ld�nyt
                    equipmnetsData.Add(new EquipmnetStruct(transform.GetChild(i).GetComponent<EquipmentSlot>().SlotName, transform.GetChild(i).GetComponent<EquipmentSlot>().SlotType,new Item()));
                }
            }
            public class EquipmnetStruct
            {
                public string EquipmentSlotName;
                public string EquipmnetSlotType;
                public Item EquipmnetItem;
                public EquipmnetStruct(string name, string type, Item item)
                {
                    this.EquipmentSlotName = name;
                    this.EquipmnetSlotType = type;
                    this.EquipmnetItem = item;
                }
            }
        }

        private void Awake()//----------------------------------------------------------- ELSO LEPES AZ INVENTORY MEGALKOTASAKOR ---------------------------------------------------------------------------
        {
            //0. egy uj pedlanyt hozunk letre az equipmnetkenek
            equipments = new Equipmnets();
            //3. ha van inventory akkor betoltj�k azt
            InventoryLoad();
            //5. a player megkapja a vizualiz�ci�t az inventoryhoz
            gameObject.AddComponent<PlayerInventoryVisual>().DataSynch(ref playerInventoryData);
        }
        public void InventorySave()
        {

        }
        public void InventoryLoad()//kelelne egy save manager script ami a be ovasat es a kiirast kezelni ezzel lehet idot lehetni sporolni
        {
            if (File.Exists("UserSave.json"))
            {
                string jsonString = File.ReadAllText("PlayerSave.json");
                equipments = JsonConvert.DeserializeObject<Equipmnets>(jsonString);
            }
            //4. ha nincs elmentett inventory akkor az eddig letrehozott ures inventory adat�t a f� publikus playerInventoryData v�ltoz� megkapja
            InventoryUpdate();
        }

        private void InventoryUpdate()
        {
            playerInventoryData = this;
        }




        public void InventoryAdd(Item item)
        {
            item.SetItem(item.ItemName);
            Debug.Log($"Addad item: {item.ItemName}");
            bool ItemAdded = false;
            for (int i = 0; i < equipments.equipmnetsData.Count; i++)//equipment
            {
                Debug.Log($"Adding into equipmnets...    {i}   equipments.equipmnetsData[i].EquipmnetType:{equipments.equipmnetsData[i].EquipmnetSlotType} =?= item.ItemType:{item.ItemType}   equipments.equipmnetsData[i].EquipmnetItem.ItemName:{equipments.equipmnetsData[i].EquipmnetItem.ItemName == null}");
                if (equipments.equipmnetsData[i].EquipmnetSlotType.Contains(item.ItemType) && equipments.equipmnetsData[i].EquipmnetItem.ItemName == null)
                {
                    item.SlotUse = new string[] {equipments.equipmnetsData[i].EquipmentSlotName};
                    equipments.equipmnetsData[i].EquipmnetItem = item;
                    ItemAdded = true;
                    InventoryUpdate();
                    Debug.Log($"item: {item.ItemName} added in equipment: {equipments.equipmnetsData[i].EquipmentSlotName}");
                    break;
                }
            }
            if (!ItemAdded)//container
            {
                for (int equpmentIndex = 0; equpmentIndex < equipments.equipmnetsData.Count; equpmentIndex++)//vegig iter�lunk az osszes equipmenten
                {
                    if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem==null) { Debug.LogError($"EquipmnetItem == null");};
                    if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container != null)//ha az equipmnetnek nincs containerje akkor kihadjuk
                    {
                        
                        if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors == null) { Debug.LogError($"EquipmnetItem.Container.Sectrors == null"); };
                        for (int sectorIndex = 0; sectorIndex < equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors.Length; sectorIndex++)//mivel a szector 2D array-okat tartalmaz ezert a sectorokon az az ezen 2D arrayokon iteralunk vegig
                        {
                            
                            if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex].GetLength(0) >= item.SizeX && equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex].GetLength(1) >= item.SizeY)//egy gyors ellen�rz�st v�gz�nk, hogy az itemunk a feltetelezett teljesen ures sectorba belef�r e, ha nem kihadjuk
                            {
                                Debug.Log($"AZ item elf�r X: {item.SizeX}   Y: {item.SizeY}");
                                List<ItemSlot> tartgetSlots = new List<ItemSlot>();//ha belefer akkor el kell d�nten�nk hov fer, ehez segit a targetSlot valtozo mely a lehetseges slotokat t�rolja
                                for (int row = 0; row < equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex].GetLength(0) && !ItemAdded; row++)//vegig iter�lunk a sorokon
                                {
                                    for (int colum = 0; colum < equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex].GetLength(1) && !ItemAdded; colum++)//a sorokon belul az oszlopokon
                                    {
                                        if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex][row, colum].PartOfItem == null)//ha a slot nem tagja egy itemnek sem akkor target
                                        {
                                            for (int itemY = 0; itemY < item.SizeY && !ItemAdded; itemY++)
                                            {
                                                for (int itemX = 0; itemX < item.SizeX && !ItemAdded; itemX++)
                                                {/*
                                                        if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex][itemX, itemY].PartOfItem != null) 
                                                        {
                                                            Debug.Log($"{equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex][itemX, itemY].PartOfItem.ItemName}");
                                                            for (int i = 0; i < equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex].GetLength(0); i++)
                                                            {
                                                                for (int j = 0; j < equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex].GetLength(1); j++)
                                                                {
                                                                    if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex][i, j].PartOfItem != null)
                                                                    {
                                                                        Debug.Log($"{equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex][i, j].PartOfItem.ItemName}       {i}x {j}y");
                                                                    }
                                                                    else
                                                                    {
                                                                        Debug.Log($"null {i}x {j}y");
                                                                    }
                                                                }
                                                            }
                                                        }
                                                         */
                                                    if (tartgetSlots.Count == item.SizeY*item.SizeX)
                                                    {
                                                        item.SlotUse = new string[tartgetSlots.Count];
                                                        for (int j1 = 0; j1 < tartgetSlots.Count; j1++)
                                                        {
                                                            Debug.Log($"{item.ItemName} slotuse add: {tartgetSlots[j1].name}");
                                                            item.SlotUse[j1] = tartgetSlots[j1].name;//NEM A SLOTNAME hasznaljuk mert azt csak akkor kapja meg ha letezik mint objektum, de egynkent az objetum neve �s adat neve ugyan az
                                                            tartgetSlots[j1].PartOfItem = item;
                                                        }
                                                        equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Items.Add(item);
                                                        InventoryUpdate();
                                                        ItemAdded = true;
                                                        Debug.LogWarning($"Item Added");
                                                    }
                                                    else if (equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex][itemY, itemX].PartOfItem == null)
                                                    {
                                                        Debug.Log($"{item.ItemName}  adding to container: slot add: {equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex][itemY, itemX].name}     x{itemX}  y{itemY}");
                                                        tartgetSlots.Add(equipments.equipmnetsData[equpmentIndex].EquipmnetItem.Container.Sectors[sectorIndex][itemY, itemX]);
                                                    }
                                                    else
                                                    {
                                                        tartgetSlots.Clear();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (!ItemAdded)
                {
                    Debug.Log($"item: {item.ItemName} cannot added, probably no space for that");
                }
            }
        }
        public void InventoryDelete(Item item)
        {

        }
        public void InventoryModify(Item item)
        {

        }


    };



}



namespace ItemHandler
{
    public class ItemObject : MonoBehaviour
    {
        public Item ActualData;
        private Item RefData;
        private int index = -1;

        private GameObject StarterObject;
        private void Update()
        {
            /*
            if (ActualData != RefData)
            {
                if (index==-1)
                {
                    StarterObject.GetComponent<EquipmentSlot>().ActualData = ActualData;
                }
                else
                {
                    StarterObject.GetComponent<ContainerObject>().ActualData.Container.Items[index] = ActualData;
                }
                RefData = ActualData;
            }
            if (ActualData == null)//az az ha az item torlodik
            {
                Destroy(gameObject);
            }*/
        }
        public void DataSynch(Item Data, GameObject Starter)
        {
            ActualData = Data;
            StarterObject = Starter;
            if (Data.Container != null)//11. ha az item adatai tartalmaznak containert akkor az l�trej�n
            {
                SetContainer();
            }
            SelfVisualisation();
        }
        public void DataSynch(Item Data, GameObject Starter,int index)
        {
            ActualData = Data;
            StarterObject = Starter;
            this.index = index;
            if (Data.Container != null)//11. ha az item adatai tartalmaznak containert akkor az l�trej�n
            {
                SetContainer();
            }
            SelfVisualisation();
        }
        private void SetContainer()
        {
            //--> ContainerObject.cs
            Debug.LogWarning($"{ActualData.ItemName} ItemObject ------- ref --------> ContainerObject.cs");
            GameObject containerObject = CreatePrefab(ActualData.Container.PrefabPath);
            containerObject.GetComponent<ContainerObject>().DataSynch(ActualData, gameObject);
        }
        private void SelfVisualisation()//ha az item equipment slotban van
        {
            RectTransform itemObjectRectTransform = gameObject.AddComponent<RectTransform>();
            SpriteRenderer itemObjectSpriteRedner = gameObject.AddComponent<SpriteRenderer>();
            itemObjectSpriteRedner.sprite = Resources.Load<Sprite>(gameObject.GetComponent<ItemObject>().ActualData.ImgPath);//az itemobjektum megkapja k�p�t
            itemObjectSpriteRedner.drawMode = SpriteDrawMode.Sliced;

            if (StarterObject.GetComponent<EquipmentSlot>() != null)
            {

                RectTransform EquipmentSlot = StarterObject.GetComponent<RectTransform>();

                gameObject.transform.SetParent(StarterObject.transform.parent, false);//itemObj parent set

                itemObjectRectTransform.localPosition = new Vector3(EquipmentSlot.localPosition.x, EquipmentSlot.localPosition.y, 0);
                itemObjectRectTransform.anchorMin = EquipmentSlot.anchorMin;
                itemObjectRectTransform.anchorMax = EquipmentSlot.anchorMax;
                itemObjectRectTransform.pivot = EquipmentSlot.pivot;
                itemObjectRectTransform.offsetMin = Vector2.zero;
                itemObjectRectTransform.offsetMax = Vector2.zero;

                float Scale = Mathf.Min(itemObjectRectTransform.rect.height / itemObjectSpriteRedner.sprite.bounds.size.y, itemObjectRectTransform.rect.width / itemObjectSpriteRedner.sprite.bounds.size.x);
                itemObjectSpriteRedner.size = new Vector2(itemObjectSpriteRedner.size.x * Scale, itemObjectSpriteRedner.size.y * Scale);

                StarterObject.transform.SetParent(gameObject.transform, false);//az itemobjketum az equipmentSlot parentobjektuma lesz

                EquipmentSlot.anchorMin = new Vector2(0, 0);
                EquipmentSlot.anchorMax = new Vector2(1f, 1f);
                EquipmentSlot.offsetMin = Vector2.zero;
                EquipmentSlot.offsetMax = Vector2.zero;
            }
            else if (StarterObject.GetComponent<ContainerObject>() != null)//ha az item containerben van
            {
                ContainerObject containerObject = StarterObject.GetComponent<ContainerObject>();
                List<GameObject> itemSlots = new List<GameObject>();
                Debug.Log($"{ActualData.ItemName} item Slotuses count : {ActualData.SlotUse.Length}");
                for (int sector = 0; sector < containerObject.SectorManagers.Length; sector++)
                {
                    for (int i = 0; i < ActualData.SlotUse.Length; i++)
                    {
                        Debug.Log(ActualData.SlotUse[i]);
                    }
                    for (int slot = 0; slot < containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots.Length; slot++)
                    {
                        Debug.Log($"Find item slot {slot}.try : {containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].name}           {containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name}");
                        Debug.Log($"{ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name)}");
                        if (ActualData.SlotUse.Contains(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].GetComponent<ItemSlot>().name))//mivel objektumkent nem letezik az itemslot ezert az obejktum(prefab) nevet hazsnaljuk
                        {
                            Debug.Log($"{containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot].name} added");
                            itemSlots.Add(containerObject.SectorManagers[sector].GetComponent<SectorManager>().ItemSlots[slot]);
                        }
                    }
                }
                

                gameObject.transform.SetParent(itemSlots[0].transform.parent, false);
                // Alap�rtelmezett kezd��rt�kek (�rtelmesen kisz�m�tva)
                Vector2 minPos = new Vector2(float.MaxValue, float.MaxValue);
                Vector2 maxPos = new Vector2(float.MinValue, float.MinValue);

                // V�gigmegy az �sszes itemSlot-on �s kisz�m�tja a minim�lis �s maxim�lis poz�ci�kat
                foreach (GameObject itemSlot in itemSlots)
                {
                    // Az itemSlot helyi poz�ci�ja a gameObject sz�l�h�z k�pest
                    Vector3 slotLocalPos = itemSlot.GetComponent<RectTransform>().localPosition;
                    Vector2 slotMin = slotLocalPos - (Vector3)itemSlot.GetComponent<RectTransform>().sizeDelta / 2;
                    Vector2 slotMax = slotLocalPos + (Vector3)itemSlot.GetComponent<RectTransform>().sizeDelta / 2;

                    // Be�ll�tja a minim�lis �s maxim�lis pontokat az �sszes itemSlot lefed�s�re
                    minPos = Vector2.Min(minPos, slotMin);
                    maxPos = Vector2.Max(maxPos, slotMax);
                }

                // M�ret kisz�m�t�sa �s a sz�l� objektum m�ret�nek be�ll�t�sa
                Vector2 newSize = maxPos - minPos;
                itemObjectRectTransform.sizeDelta = newSize;

                // A sz�l� objektum pivotj�t k�z�pre �ll�tja a pontos igaz�t�shoz
                itemObjectRectTransform.pivot = new Vector2(0.5f, 0.5f);

                // �j poz�ci� be�ll�t�sa �gy, hogy a sz�l� lefedje az itemSlots �sszes elem�t
                itemObjectRectTransform.localPosition = (Vector3)((maxPos + minPos) / 2f);

                float Scale = Mathf.Min(itemObjectRectTransform.rect.height / itemObjectSpriteRedner.sprite.bounds.size.y, itemObjectRectTransform.rect.width / itemObjectSpriteRedner.sprite.bounds.size.x);
                itemObjectSpriteRedner.size = new Vector2(itemObjectSpriteRedner.size.x * Scale, itemObjectSpriteRedner.size.y * Scale);
            }

        }
    }

    public class Item : ItemStruct
    {
        private void CopyProperties(Item source)
        {
            //altalanos adatok
            ItemType = source.ItemType;//ez alapj�n ker�lhet be egy slotba ugyan is vannak pecifikus slotok melyeknek typusv�ltoz�j�ban benen kell, hogy legyen.
            ItemName = source.ItemName;//ez alapj�n hozza l�tre egy item saj�t mag�t
            Description = source.Description;
            Quantity = source.Quantity;
            SlotUse = source.SlotUse;// ez a jelenleg elfoglalt helye, ezt a betolteskor hasznaljuk, hogy tudjuk mit hova raktunk el.
            ImgPath = source.ImgPath;
            SizeX = source.SizeX;
            SizeY = source.SizeY;
            //tartalom
            Container = source.Container;//tartalom
            //fegyver adatok
            DefaultMagasineSize = source.DefaultMagasineSize;
            Spread = source.Spread;
            Rpm = source.Rpm;
            Recoil = source.Recoil;
            Accturacy = source.Accturacy;
            Range = source.Range;
            Ergonomy = source.Ergonomy;
            BulletType = source.BulletType;
            Accessors = source.Accessors;
            //felhasznalhato e?
            usable = source.usable;
        }
        public void SetItem(string name)
        {
            Item completedItem = name switch
            {
                "TestWeapon" => new TestWeapon().Set(),
                "TestBackpack" => new TestBackpack().Set(),
                "TestVest" => new TestVest().Set(),
                "TestArmor" => new TestArmor().Set(),
                "TestHandgun" => new TestHandgun().Set(),
                /*
                "TestHelmet" => new TestVest().Set(),
                "TestFingers" => new TestVest().Set(),
                "TestBoots" => new TestVest().Set(),
                "TestMask" => new TestVest().Set(),
                "TestHeadset" => new TestVest().Set(),
                "TestSkin" => new TestVest().Set(),
                "TestPant" => new TestVest().Set(),
                "TestMelee" => new TestVest().Set(),
                */
                _ => throw new ArgumentException("Invalid type")
            };
            CopyProperties(completedItem);
            Debug.Log($"Item created {this}");
        }
        public Item() { }
        public Item(string name)// egy itemet mindeg n�v alapjan peldanyositunk
        {
            SetItem(name);
        }
    }
    public abstract class ItemStruct// az item hozza letre azt a panelt a slot inventoryban amely a tartlomert felelos, de csak akkor ha � equipment slotban van egybekent egy up relativ pozitcioju panelt hozzon letre mint az EFT-ban
    {

        //general
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string[] SlotUse { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public string ImgPath { get; set; }
        //contain
        public Container Container { get; set; }
        //weapon
        public int? DefaultMagasineSize { get; set; }
        public double? Spread { get; set; }
        public int? Rpm { get; set; }
        public double? Recoil { get; set; }
        public double? Accturacy { get; set; }
        public double? Range { get; set; }
        public double? Ergonomy { get; set; }
        public BulletType BulletType { get; set; }
        public Accessors Accessors { get; set; }
        //usable
        public bool usable { get; set; } = false;
        //ammo

        //med

        //armor
    }
    public class Container
    {
        //egy container az itemj�hez tartozik.
        //az item constructor selekci�j�n�l itemet peldanyositunk: pl: TestWeapon
        //ebben az eddig null �r�k� container v�ltoz� egy ures containerrre v�ltozik
        //az item p�d�nyos�t�s�n�l igy egy �j p�ld�ny k�sz�l a containerb�l is mely alapvet�en tartalmazza a container PrefabPath-�t
        //a kostructora az igy megkapott prefabPath-b�l lekerdezi a Sectorokat
        public List<Item> Items { get; set; }
        public string PrefabPath;
        public ItemSlot[][,] Sectors { get; set; }
        public Container(string prefabPath)
        {
            PrefabPath = prefabPath;
            Items = new List<Item>();
            GameObject[] sectors = Resources.Load(prefabPath).GetComponent<ContainerObject>().SectorManagers;
            Sectors = new ItemSlot[sectors.Length][,];
            for (int i = 0; i < Sectors.Length; i++)
            {
                int index = 0;
                Sectors[i] = new ItemSlot[sectors[i].GetComponent<SectorManager>().row, sectors[i].GetComponent<SectorManager>().columb];
                for (int row = 0; row < Sectors[i].GetLength(0); row++)
                {
                    for (int col = 0; col < Sectors[i].GetLength(1); col++)
                    {
                        Debug.Log($"index: {index} {sectors[i].GetComponent<SectorManager>().ItemSlots[index].GetComponent<ItemSlot>().name}     C{row}   R{col}");
                        Sectors[i][row, col] = sectors[i].GetComponent<SectorManager>().ItemSlots[index].GetComponent<ItemSlot>();
                        index++;
                    }
                }
            }
        }
    }
        public class BulletType
        {

        }

        public class Accessors
        {

        }
    }










namespace PlayerInventoryVisualBuild
{
    public class PlayerInventoryVisual : MonoBehaviour
    {
        private bool InventoryOpen = false;

        private GameObject InventoryObject;//az invenory f� objektum�nak t�rol�s��rt fele

        private PlayerInventory playerInventory;

        [HideInInspector] public static GameObject EquipmentsObject;//az inventory 3 als�brend�bb objektum�nak egyike
        [HideInInspector] public static GameObject SlotObject;//az inventory 3 als�brend�bb objektum�nak egyike
        [HideInInspector] public static GameObject LootObject;//az inventory 3 als�brend�bb objektum�nak egyike
        private void Update()
        {
            //6. ebben az update met�dusban az inventory be �s ki kapcsolasat figyeli
            OpenCloseInventory();
        }
        public void DataSynch(ref PlayerInventory playerInventory)
        {
            this.playerInventory = playerInventory;
        }
        public void OpenCloseInventory()//ez az inventoryt epiti fel
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (InventoryOpen)
                {
                    InventoryOpen = false;
                    Destroy(InventoryObject);
                }
                else
                {
                    InventoryOpen = true;
                    EquipmentInitialisation();
                }
            }
        }
        private void EquipmentInitialisation()//7. ha az inventory megnyit�dik akkor az inventory adatai fel�p�tik az inventoryt
        {
            GameObject UI = GameObject.FindGameObjectWithTag("InGameUI");
            InventoryObject = new GameObject("Inventory");

            if (UI != null)
            {
                InventoryObject.transform.SetParent(UI.transform, false);

                InventoryObject.AddComponent<RectTransform>().localPosition = new Vector3(0, 0, UI.transform.position.z);
            }
            else
            {
                Debug.LogError("UI nem tal�lhat�!");
            }
            if (playerInventory == null || playerInventory.equipments.equipmnetsData == null)
            {
                Debug.LogError("Equipmnets vagy EquipmnetsData null!");
            }

            float[] aranyok = Aranyszamitas(new float[] { 6, 5, 6 }, Main.DefaultWidth);

            EquipmentsObject = CreatePrefab("GameElements/Equipment-Inventory");
            EquipmentsObject.transform.SetParent(InventoryObject.transform);
            EquipmentsObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[0], Main.DefaultHeight);
            EquipmentsObject.GetComponent<RectTransform>().localPosition = new Vector3((aranyok[0] + aranyok[1] / 2) * -1, 0, 0);
            PanelEquipments panelEquipments = EquipmentsObject.GetComponent<PanelEquipments>();

            SlotObject = CreatePrefab("GameElements/Slots-Inventory");
            SlotObject.transform.SetParent(InventoryObject.transform);
            SlotObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[1], Main.DefaultHeight);
            SlotObject.GetComponent<RectTransform>().localPosition = new Vector3(aranyok[1] * -1 / 2, 0, 0);
            PanelSlots panelSlots = SlotObject.GetComponent<PanelSlots>();

            LootObject = CreatePrefab("GameElements/Loot-Inventory");
            LootObject.transform.SetParent(InventoryObject.transform);
            LootObject.GetComponent<RectTransform>().sizeDelta = new Vector2(aranyok[2], Main.DefaultHeight);
            LootObject.GetComponent<RectTransform>().localPosition = new Vector3(aranyok[1] / 2 + aranyok[2], 0, 0);

            for (int i = 0; i < panelEquipments.EquipmentsSlots.Length; i++)
            {
                for (int j = 0; j < playerInventory.equipments.equipmnetsData.Count; j++)
                {
                    if (playerInventory.equipments.equipmnetsData[j].EquipmnetItem.ItemName != null && playerInventory.equipments.equipmnetsData[j].EquipmnetItem.SlotUse.Contains(panelEquipments.EquipmentsSlots[i].name))
                    {
                        //--> EquipmnetSlot.cs
                        //8. az inventory equipmnetjei egyes�vel innin indulnak ki, ez a vizualiz�ci� �s az adat�raml�s l�ncreakci�szer� kiindul�sipontja.
                        Debug.LogWarning($"{playerInventory.equipments.equipmnetsData[j].EquipmnetItem.ItemName} PlayerInventory.cs ------- ref --------> EquipmentSlot.cs");
                        //9. az equipmnetSlot objektum komponense adatszinkroniz�ci�ban refer�l az Playerinventory adat equipmnet slotban l�v� equipment item�nek adat�ra
                        panelEquipments.EquipmentsSlots[i].GetComponent<EquipmentSlot>().DataSynch(ref playerInventory.equipments.equipmnetsData[j].EquipmnetItem);
                    }
                }
            }
        }
    };
}