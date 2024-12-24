using ItemHandler;

namespace Meds
{
    public class AI_2 : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Meds/AI_2",//az item képe
                ItemType = "Meds",//typus azonosito
                ItemName = "AI_2",//nev azonosito
                Description = "This is an AI-2 Medical kit, cheap, robust and there is a lot like a good communist medicine",
                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga
                IsUsable = true,
                UseLeft = 5,
            };

        }
    }
}