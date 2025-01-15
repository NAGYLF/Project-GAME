using ItemHandler;

namespace Backpacks
{
    public class TestBackpack : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Backpacks/TestBackpack",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "TestBackpack",//nev azonosito
                Description = "Ez egy Admin backpack, statjai a legjobbak",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 3,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Backpacks/TestBackpack"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

 public class Camelback_Tri_Zip_assault_backpack : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Backpacks/Camelback_Tri_Zip_assault_backpack",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "Camelback_Tri_Zip_assault_backpack",//nev azonosito
                Description = "Ez egy közepes nagyságú backpack",
                SizeX = 5,//az item slotokban elfoglalt szelessege
                SizeY = 6,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Backpacks/Camelback_Tri-Zip_assault_backpack"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Flyye_MBSS_backpack : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Backpacks/Flyye_MBSS_backpack",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "Flyye_MBSS_backpack",//nev azonosito
                Description = "Ez egy kis nagyságú backpack",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 4,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Backpacks/Flyye_MBSS_backpack"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Gruppa_99_T30_backpack : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Backpacks/Gruppa_99_T30_backpack",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "Gruppa_99_T30_backpack",//nev azonosito
                Description = "Ez egy közepes nagyságú backpack",
                SizeX = 5,//az item slotokban elfoglalt szelessege
                SizeY = 6,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Backpacks/Gruppa_99_T30_backpack"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Sanitars_bag : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Backpacks/Sanitars_bag",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "Sanitars_bag",//nev azonosito
                Description = "Ez egy kis nagyságú backpack",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 4,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Backpacks/Sanitars_bag"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Scav_backpack : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Backpacks/Scav_backpack",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "Scav_backpack",//nev azonosito
                Description = "Ez egy közepes nagyságú backpack",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 5,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Backpacks/Scav_backpack"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Tactical_sling_bag : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Backpacks/Tactical_sling_bag",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "Tactical_sling_bag",//nev azonosito
                Description = "Ez egy kis nagyságú backpack",
                SizeX = 3,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Backpacks/Tactical_sling_bag"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Transformer_Bag : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Backpacks/Transformer_Bag",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "Transformer_Bag",//nev azonosito
                Description = "Ez egy kis nagyságú backpack",
                SizeX = 3,//az item slotokban elfoglalt szelessege
                SizeY = 3,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Backpacks/Transformer_Bag"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Vertx_Ready_Pack_Backpack : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Backpacks/Vertx_Ready_Pack_Backpack",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "Vertx_Ready_Pack_Backpack",//nev azonosito
                Description = "Ez egy kis nagyságú backpack",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 4,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Backpacks/Vertx_Ready_Pack_Backpack"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class VKBO_army_bag : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Backpacks/VKBO_army_bag",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "VKBO_army_bag",//nev azonosito
                Description = "Ez egy kis nagyságú backpack",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Backpacks/VKBO_army_bag"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class WARTECH_Berkut_Backpack : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Backpacks/WARTECH_Berkut_Backpack",//az item képe
                ItemType = "Backpack",//typus azonosito
                ItemName = "WARTECH_Berkut_Backpack",//nev azonosito
                Description = "Ez egy közepes nagyságú backpack",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 5,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Backpacks/WARTECH_Berkut_Backpack"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }
}