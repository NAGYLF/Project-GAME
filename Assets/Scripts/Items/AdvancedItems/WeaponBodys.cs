using ItemHandler;

namespace WeaponBodys
{
    public class AKS74UBody : Item
    {
        public Item Set()
        {
            return new Item()
            {
                IsAdvancedItem = true,
                ItemType = "WeaponBody",//typus azonosito
                ItemName = "AKS-74U_Body",//nev azonosito
                Description = "",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 800,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 1.82,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 500,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 52,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }
}