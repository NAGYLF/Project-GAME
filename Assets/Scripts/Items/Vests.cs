using ItemHandler;

namespace Vests
{
    public class TestVest : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/TestVest",//az item képe
                ItemType = "Vest",//typus azonosito
                ItemName = "TestVest",//nev azonosito
                Description = "Ez egy Admin Vest, statjai a leheto legjobbak",
                SizeX = 3,//az item slotokban elfoglalt szelessege
                SizeY = 3,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/TestVest"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class _6B5_15_Zh_86_Uley_armored_rig : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/_6B5_15_Zh_86_Uley_armored_rig",//az item képe
                ItemType = "Vest",//typus azonosito
                ItemName = "_6B5_15_Zh_86_Uley_armored_rig",//nev azonosito
                Description = "The 6B5 bulletproof vest was adopted by the armed forces of the USSR in 1986.",
                SizeX = 3,//az item slotokban elfoglalt szelessege
                SizeY = 4,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/_6B5_15_Zh_86_Uley_armored_rig"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class ANA_Tactical_M1_plate_carrier : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/ANA_Tactical_M1_plate_carrier",//az item képe
                ItemType = "Vest",//typus azonosito
                ItemName = "ANA_Tactical_M1_plate_carrier",//nev azonosito
                Description = "The M1 vest is created with the use of the best experience of Russian special forces operators.",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 4,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/ANA_Tactical_M1_plate_carrier"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class BlackRock_chest_rig : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/BlackRock_chest_rig",//az item képe
                ItemType = "Vest",//typus azonosito
                ItemName = "BlackRock_chest_rig",//nev azonosito
                Description = "A custom-made chest rig for wearing on top of body armor in urban operations.",
                SizeX = 3,//az item slotokban elfoglalt szelessege
                SizeY = 4,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/BlackRock_chest_rig"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Scav_Vest : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/Scav_Vest",//az item képe
                ItemType = "Vest",//typus azonosito
                ItemName = "Scav_Vest",//nev azonosito
                Description = "A fisherman's vest can more or less replace the chest rig, if the need is pressing enough.",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 3,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/Scav_Vest"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Security_vest : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/Security_vest",//az item képe
                ItemType = "Vest",//typus azonosito
                ItemName = "Security_vest",//nev azonosito
                Description = "The simplest, life-worn vest of the security services.",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 3,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/Security_vest"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class SOE_Micro_Rig : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/SOE_Micro_Rig",//az item képe
                ItemType = "Vest",//typus azonosito
                ItemName = "SOE_Micro_Rig",//nev azonosito
                Description = "An extra lightweight and small chest rig with the necessary minimum of pouches.",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 3,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/SOE_Micro_Rig"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Stich_Profi_Plate_Carrier_V2 : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/Stich_Profi_Plate_Carrier_V2",//az item képe
                ItemType = "Vest",//typus azonosito
                ItemName = "Stich_Profi_Plate_Carrier_V2",//nev azonosito
                Description = "An improved lightweight version of the standard plate carrier manufactured by Stich Profi.",
                SizeX = 3,//az item slotokban elfoglalt szelessege
                SizeY = 4,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/Stich_Profi_Plate_Carrier_V2"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Tasmanian_Tiger_Plate_Carrier_MKIII : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/Tasmanian_Tiger_Plate_Carrier_MKIII",//az item képe
                ItemType = "Vest",//typus azonosito
                ItemName = "Tasmanian_Tiger_Plate_Carrier_MKIII",//nev azonosito
                Description = "A lightweight low-profile plate carrier designed to fit SAPI plates. Manufactured by Tasmanian Tiger.",
                SizeX = 3,//az item slotokban elfoglalt szelessege
                SizeY = 3,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/Tasmanian_Tiger_Plate_Carrier_MKIII"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Umka_M33_SET1_hunter_vest : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/Umka_M33_SET1_hunter_vest",//az item képe
                ItemType = "Vest",//typus azonosito
                ItemName = "Umka_M33_SET1_hunter_vest",//nev azonosito
                Description = "The Umka M33-SET1 vest is designed for hunters, travelers, field professionals and security officers.",
                SizeX = 3,//az item slotokban elfoglalt szelessege
                SizeY = 4,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/Umka_M33_SET1_hunter_vest"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }

    public class Velocity_Systems_MPPV_Multi_Purpose_Patrol_Vest : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/Velocity_Systems_MPPV_Multi_Purpose_Patrol_Vest",//az item képe
                ItemType = "Vest",//typus azonosito
                ItemName = "Velocity_Systems_MPPV_Multi_Purpose_Patrol_Vest",//nev azonosito
                Description = "Multi-Purpose Patrol Vest is designed for those patrolling situations where armor is not needed.",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 3,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/Velocity_Systems_MPPV_Multi_Purpose_Patrol_Vest"),//tárolási képességgel ruházza fel az itemet, mely során a SlotPanel-ben létrjön egy container prefab
            };

        }
    }
}