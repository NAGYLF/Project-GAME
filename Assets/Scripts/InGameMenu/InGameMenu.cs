using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static MainData.SupportScripts;
using UI;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using ItemHandler;
using static ItemHandler.InventorySystem;
using NaturalInventorys;
using PlayerInventoryClass;
using static PlayerInventoryClass.PlayerInventory;


public class InGameMenu : MonoBehaviour
{
    public GameObject player; // A játékos objektum
    public GameObject inventoryParent; // Inventory-t reprezentáló objektum (szülõ)

    private string savePath;

    private void Awake()
    {
        gameObject.SetActive(false);
        savePath = UnityEngine.Application.persistentDataPath + "/gameData.json";
    }

    public void Save()
    {
        if (player != null)
        {
            // Játékos pozíció mentése
            Vector3 playerPosition = player.transform.position;

            

            // Mentési adatstruktúra létrehozása
            SaveData saveData = new SaveData
            {
                playerPosition = playerPosition
                
            };
           
            // Adatok JSON fájlba írása
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(savePath, json);
            UnityEngine.Debug.Log("Game data saved: " + savePath);
        }
        else
        {
            UnityEngine.Debug.LogWarning("Player object is not assigned.");
        }
    }

    public void Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            // Játékos pozíció visszaállítása
            if (player != null)
            {
                player.transform.position = saveData.playerPosition;
                UnityEngine.Debug.Log("Player position loaded.");
            }

            
        }
        else
        {
            UnityEngine.Debug.LogWarning("Save file not found at: " + savePath);
        }
    }

    public void ExitToTheMainMenu()
    {
        SceneChange("Main Menu");
    }

    public void ExitToTheDesktop()
    {
        UnityEngine.Application.Quit();
    }

    // Mentési adatok struktúrája
    [System.Serializable]
    public class SaveData
    {
        public Vector3 playerPosition; // Játékos pozíció
        public List<string> inventoryItemNames; // Inventory itemek neveinek listája
    }

    // Példa InventoryAdd metódus
    private void InventoryAdd(string itemName)
    {
        // Itt implementálhatod, hogy az item neve alapján hozzáadod az inventory-hoz
        UnityEngine.Debug.Log($"Adding item to inventory: {itemName}");
    }
}



