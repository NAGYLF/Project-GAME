using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemHandler;

namespace Skins
{
    public class TestSkin : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Skins/TestSkin",//az item képe
                ItemType = "Skin",//typus azonosito
                ItemName = "TestSkin",//nev azonosito
                Description = "Ez egy Admin Skin, statjai a legjobbak",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga
            };

        }
    }
}