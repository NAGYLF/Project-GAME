using System.Collections.Generic;
using UnityEngine;
using ItemHandler;


public class ChestLoot : MonoBehaviour
{
    [Header("Loot Configuration")]
    public string lootData;

    public List<LootItem> tier1Loot = new List<LootItem>
    {
        new LootItem("TestWeapon", 0f),
        new LootItem("TestArmor", 100f),
    };

    public List<LootItem> tier2Loot = new List<LootItem>
    {
        new LootItem("TestHandgun", 25f),
        new LootItem("AK103", 18f)
    };

    public List<LootItem> tier3Loot = new List<LootItem>
    {
        new LootItem("TestHelmet", 20f),
        new LootItem("TestMelee", 10f)
    };

    public List<LootItem> tier4Loot = new List<LootItem>
    {
        new LootItem("TestBackpack", 8f),
        new LootItem("TestVest", 12f)
    };

    public List<LootItem> tier5Loot = new List<LootItem>
    {
        new LootItem("TestBoots", 2f),
        new LootItem("7.62x39FMJ", 5f)
    };
    public class LootItem
    {
        public string Name;
        public float SpawnRate;

        public LootItem(string name, float spawnRate)
        {
            Name = name;
            SpawnRate = spawnRate;
        }
    }

    void Start()
    {
        if (!string.IsNullOrEmpty(lootData))
        {
            GenerateLoot(UnityEngine.Random.Range(3, 11));
        }
        else
        {
            Debug.LogError("Loot data string is empty!");
        }
    }

    private void GenerateLoot(int itemCount)
    {
        List<LootItem> lootPool;

        switch (lootData.ToLower())
        {
            case "tier1":
                lootPool = tier1Loot;
                break;
            case "tier2":
                lootPool = tier2Loot;
                break;
            case "tier3":
                lootPool = tier3Loot;
                break;
            case "tier4":
                lootPool = tier4Loot;
                break;
            case "tier5":
                lootPool = tier5Loot;
                break;
            default:
                Debug.LogError("Invalid loot data tier selected.");
                return;
        }

        for (int i = 0; i < itemCount; i++)
        {
            LootItem randomItem = GetRandomItem(lootPool);
            if (randomItem != null)
            {
                Debug.Log($"Generated item: {randomItem.Name} with spawn rate: {randomItem.SpawnRate}");
                Item itemInstance = new Item(randomItem.Name, UnityEngine.Random.Range(1, 10));
                SpawnItem(itemInstance);
            }
        }
    }

    private LootItem GetRandomItem(List<LootItem> lootPool)
    {
        int index = UnityEngine.Random.Range(0, lootPool.Count);
        return lootPool[index];
    }

    private void SpawnItem(Item item)
    {
        Debug.Log($"Spawned {item.ItemName} with spawn rate: {item.Quantity}");
    }
}
