using ItemHandler;

namespace WeaponBodys
{
    public class AK103Body : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ItemPartPath = "Items/AK103/AK103Body",
                ItemType = "WeaponBody",//typus azonosito
                ItemName = "AK103Body",//nev azonosito
                Description = "TEST",
                SizeX = 1,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                Rpm = 800,//lov�s percenk�nt
                IsModificationAble = true,

                BulletType = new BulletType()//l�szer tipusa ez tartalmazza a sebzest, p�nc�l �tutest, stb NINCS K�SZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy p�ld�ny lesz
                    //K�tele� tartalom
                },
            };

        }
    }

}