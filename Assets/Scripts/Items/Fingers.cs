using ItemHandler;

namespace Fingers
{
    public class TestFingers : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Fingers/TestFingers",//az item képe
                ItemType = "Fingers",//typus azonosito
                ItemName = "TestFingers",//nev azonosito
                Description = "Ez egy Admin Finger, statjai a legjobbak",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga
            };

        }
    }
}