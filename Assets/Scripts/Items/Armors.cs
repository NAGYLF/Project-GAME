using ItemHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Armors
{
    public class TestArmor : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Armors/TestArmor",//az item képe
                ItemType = "Armor",//typus azonosito
                ItemName = "TestArmor",//nev azonosito
                Description = "Ez egy Admin Armor, statjai a leheto legjobbak",
                SizeX = 3,//az item slotokban elfoglalt szelessege
                SizeY = 4,//az item slotokban elfoglalt magassaga
            };

        }
    }
}
