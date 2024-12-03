using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemHandler;

namespace Weapons
{
    public class TestWeapon : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/TestWeapon",//az item képe
                ItemType = "WeaponMain",//typus azonosito
                ItemName = "TestWeapon",//nev azonosito
                Description = "Ez egy Admin fegyver, statjai a leheto legjobbak",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga

                DefaultMagasineSize = 30,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Rpm = 500,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 100,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 1000,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 100,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
                Accessors = new Accessors()//a fegyver alapvető értékeit modosito felszereltségek NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //nem kötelelző tartalom
                },
            }; 

        }
    }

    public class TestHandgun : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/TestHandgun",//az item képe
                ItemType = "WeaponSecondary",//typus azonosito
                ItemName = "TestHandgun",//nev azonosito
                Description = "Ez egy Admin fegyver, statjai a leheto legjobbak",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                DefaultMagasineSize = 17,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Rpm = 500,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 100,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 1000,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 100,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
                Accessors = new Accessors()//a fegyver alapvető értékeit modosito felszereltségek NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //nem kötelelző tartalom
                },
            };

        }
    }

    public class AK103 : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/AK103",//az item képe
                ItemType = "WeaponMain",//typus azonosito
                ItemName = "AK103",//nev azonosito
                Description = "Ez egy Admin fegyver, statjai a leheto legjobbak",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga

                DefaultMagasineSize = 30,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Rpm = 500,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 100,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 1000,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 100,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
                Accessors = new Accessors()//a fegyver alapvető értékeit modosito felszereltségek NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //nem kötelelző tartalom
                },
            };

        }
    }
}
