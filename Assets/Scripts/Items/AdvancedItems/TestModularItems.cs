using ItemHandler;

namespace TestModulartItems
{
    internal class TestModularItems
    {
        public class TestCenter : Item
        {
            public Item Set()
            {
                return new Item()
                {
                    ImgPath = "Textures/ItemTextures/Weapons/Colt_M4A1_5_56x45_assault_rifle_KAC_RIS",//az item képe
                    ItemType = "TestCenter",//typus azonosito
                    ItemName = "TestCenter",//nev azonosito
                    Description = "...",
                };

            }
        }
    }
}
