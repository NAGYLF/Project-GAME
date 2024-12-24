using ItemHandler;

namespace Vests
{
    public class TestVest : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Vests/TestVest",//az item k�pe
                ItemType = "Vest",//typus azonosito
                ItemName = "TestVest",//nev azonosito
                Description = "Ez egy Admin Vest, statjai a leheto legjobbak",
                SizeX = 3,//az item slotokban elfoglalt szelessege
                SizeY = 3,//az item slotokban elfoglalt magassaga
                Container = new Container("GameElements/ItemContainers/Vests/TestVest"),//t�rol�si k�pess�ggel ruh�zza fel az itemet, mely sor�n a SlotPanel-ben l�trj�n egy container prefab
            };

        }
    }
}

