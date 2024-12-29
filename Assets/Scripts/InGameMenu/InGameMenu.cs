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
    public GameObject player; // A j�t�kos objektum
    public GameObject inventoryParent; // Inventory-t reprezent�l� objektum (sz�l�)

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
            // J�t�kos poz�ci� ment�se
            Vector3 playerPosition = player.transform.position;

            

            // Ment�si adatstrukt�ra l�trehoz�sa
            SaveData saveData = new SaveData
            {
                playerPosition = playerPosition
                
            };
           
            // Adatok JSON f�jlba �r�sa
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

            // J�t�kos poz�ci� vissza�ll�t�sa
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

    // Ment�si adatok strukt�r�ja
    [System.Serializable]
    public class SaveData
    {
        public Vector3 playerPosition; // J�t�kos poz�ci�
        public List<string> inventoryItemNames; // Inventory itemek neveinek list�ja
    }

    // P�lda InventoryAdd met�dus
    private void InventoryAdd(string itemName)
    {
        // Itt implement�lhatod, hogy az item neve alapj�n hozz�adod az inventory-hoz
        UnityEngine.Debug.Log($"Adding item to inventory: {itemName}");
    }
}



