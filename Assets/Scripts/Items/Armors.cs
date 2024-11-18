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
                ImgPath = "Textures/ItemTextures/Armors/TestArmor",
                ItemType = "Armor",
                ItemName = "TestArmor",
                Description = "Ez egy Admin Armor, statjai a leheto legjobbak",
                SizeX = 3,
                SizeY = 4,
            };
        }
    }
}
