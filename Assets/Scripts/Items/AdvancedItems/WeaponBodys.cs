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

                Rpm = 800,//lovés percenként
                IsModificationAble = true,

                BulletType = new BulletType()//lõszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleõ tartalom
                },
            };

        }
    }

}