using ItemHandler;

namespace Masks
{
    public class TestMask : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Masks/TestMask",//az item képe
                ItemType = "Mask",//typus azonosito
                ItemName = "TestMask",//nev azonosito
                Description = "Ez egy Admin Mask, statjai a legjobbak",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga
            };

        }
    }
}