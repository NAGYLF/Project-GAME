using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemHandler;

namespace Headsets
{
    public class TestHeadset : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Headsets/TestHeadset",//az item képe
                ItemType = "Headset",//typus azonosito
                ItemName = "TestHeadset",//nev azonosito
                Description = "Ez egy Admin Headset, statjai a legjobbak",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga
            };

        }
    }
}