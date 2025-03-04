using ItemHandler;

namespace Weapons
{

    public class Colt_M4A1_5_56x45_assault_rifle_KAC_RIS : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/Colt_M4A1_5_56x45_assault_rifle_KAC_RIS",//az item képe
                ItemType = "WeaponMain",//typus azonosito
                ItemName = "Colt_M4A1_5_56x45_assault_rifle_KAC_RIS",//nev azonosito
                Description = "The Colt M4A1 carbine is a fully automatic variant of the basic M4 Carbine and was primarily designed for special operations use.",
                SizeX = 5,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga

                MagasineSize = 30,//alap tár kapacitás
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

    public class Desert_Tech_MDR_5_56x45_assault_rifle_HHS_1_Tan : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/Desert_Tech_MDR_5_56x45_assault_rifle_HHS_1_Tan",//az item képe
                ItemType = "WeaponMain",//typus azonosito
                ItemName = "Desert_Tech_MDR_5_56x45_assault_rifle_HHS_1_Tan",//nev azonosito
                Description = "The MDR 5.56x45 bullpup assault rifle, designed and manufactured by Desert Tech LLC",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga

                MagasineSize = 30,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 650,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 1.43,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 500,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 75,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class DS_Arms_SA_58_7_62x51_assault_rifle_SPR : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/DS_Arms_SA_58_7_62x51_assault_rifle_SPR",//az item képe
                ItemType = "WeaponMain",//typus azonosito
                ItemName = "DS_Arms_SA_58_7_62x51_assault_rifle_SPR",//nev azonosito
                Description = "The SA-58 OSW (Operations Specialist Weapon), manufactured by American company DSA (or DS Arms - David Selvaggio Arms), is a legal copy of the FAL",
                SizeX = 5,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga

                MagasineSize = 20,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 700,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 2.8,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 900,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 55,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class DS_Arms_SA_58_7_62x51_assault_rifle_X_FAL : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/DS_Arms_SA_58_7_62x51_assault_rifle_X_FAL",//az item képe
                ItemType = "WeaponMain",//typus azonosito
                ItemName = "DS_Arms_SA_58_7_62x51_assault_rifle_X_FAL",//nev azonosito
                Description = "The SA-58 OSW (Operations Specialist Weapon), manufactured by American company DSA (or DS Arms - David Selvaggio Arms), is a legal copy of the FAL",
                SizeX = 5,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga

                MagasineSize = 20,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 700,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 2.8,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 900,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 55,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class FN_SCAR_H_7_62x51_assault_rifle_BOSS_Xe : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/FN_SCAR_H_7_62x51_assault_rifle_BOSS_Xe",//az item képe
                ItemType = "WeaponMain",//typus azonosito
                ItemName = "FN_SCAR_H_7_62x51_assault_rifle_BOSS_Xe",//nev azonosito
                Description = "The FN SCAR-H (Special Operations Forces Combat Assault Rifle - Heavy) assault rifle chambered in 7.62x51 NATO rounds, was adopted by USSOCOM (United States Special Operations Command) as the Mk 17.",
                SizeX = 5,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga

                MagasineSize = 20,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 600,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 1.77,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 500,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 45.5,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class Mosin_7_62x54R_bolt_action_rifle_Sniper_ATACR_7_35x56 : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/Mosin_7_62x54R_bolt_action_rifle_Sniper_ATACR_7_35x56",//az item képe
                ItemType = "WeaponMain",//typus azonosito
                ItemName = "Mosin_7_62x54R_bolt_action_rifle_Sniper_ATACR_7_35x56",//nev azonosito
                Description = "Mosin–Nagant M91/30 PU is a sniper variant of the famous russian rifle, which was commonly in use by russian snipers during WW2.",
                SizeX = 6,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                MagasineSize = 5,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 30,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 1.31,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 1000,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 13,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class SIG_MCX_SPEAR_6_8x51_assault_rifle_EXPS3 : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/SIG_MCX_SPEAR_6_8x51_assault_rifle_EXPS3",//az item képe
                ItemType = "WeaponMain",//typus azonosito
                ItemName = "SIG_MCX_SPEAR_6_8x51_assault_rifle_EXPS3",//nev azonosito
                Description = "The MCX SPEAR is a multi-caliber assault rifle designed and manufactured by SIG Sauer based on the MCX assault rifle.",
                SizeX = 6,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga

                MagasineSize = 20,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 800,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 1.43,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 500,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 33,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class SIG_MPX_9x19_submachine_gun_MRS : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/SIG_MPX_9x19_submachine_gun_MRS",//az item képe
                ItemType = "WeaponMain",//typus azonosito
                ItemName = "SIG_MPX_9x19_submachine_gun_MRS",//nev azonosito
                Description = "The SIG Sauer MPX submachine gun boasts an unprecedented operation speed in the familiar form factor of the AR platform.",
                SizeX = 4,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga

                MagasineSize = 30,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 850,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 5,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 500,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 65,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class TOZ_Simonov_SKS_7_62x39_carbine_TAPCO_Intrafuse : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/TOZ_Simonov_SKS_7_62x39_carbine_TAPCO_Intrafuse",//az item képe
                ItemType = "WeaponMain",//typus azonosito
                ItemName = "TOZ_Simonov_SKS_7_62x39_carbine_TAPCO_Intrafuse",//nev azonosito
                Description = "A Soviet semi-automatic carbine designed by Sergei Simonov for 7.62x39 cartridge and known abroad as SKS-45.",
                SizeX = 5,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                MagasineSize = 10,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 40,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 1.72,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 400,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 42,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class U_S_Ordnance_M60E6_7_62x51_light_machine_gun_Default : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/U_S_Ordnance_M60E6_7_62x51_light_machine_gun_Default",//az item képe
                ItemType = "WeaponMain",//typus azonosito
                ItemName = "U_S_Ordnance_M60E6_7_62x51_light_machine_gun_Default",//nev azonosito
                Description = "The M60E6 is a 7.62x51 caliber light machine gun, a lightweight modification of the M60E4.",
                SizeX = 5,//az item slotokban elfoglalt szelessege
                SizeY = 2,//az item slotokban elfoglalt magassaga

                MagasineSize = 10,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 550,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 1.18,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 800,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 20,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    //Secondary Weapons
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

                MagasineSize = 17,//alap tár kapacitás
                AmmoType = "9x19",
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 500,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 100,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 1000,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 100,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class _20x1mm_toy_gun : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/_20x1mm_toy_gun",//az item képe
                ItemType = "WeaponSecondary",//typus azonosito
                ItemName = "_20x1mm_toy_gun",//nev azonosito
                Description = "A plastic semi-automatic toy gun firing 20x1mm disks. Designed for children over 5 years old.",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                MagasineSize = 20,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 30,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 13.41,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 50,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 100,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class Glock_17_9x19_pistol_PS9 : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/Glock_17_9x19_pistol_PS9",//az item képe
                ItemType = "WeaponSecondary",//typus azonosito
                ItemName = "Glock_17_9x19_pistol_PS9",//nev azonosito
                Description = "Glock 17 is an Austrian pistol designed by Glock company for the Austrian army purposes.",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                MagasineSize = 17,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 30,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 11.69,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 50,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 87,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class Glock_19X_9x19_pistol : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/Glock_19X_9x19_pistol",//az item képe
                ItemType = "WeaponSecondary",//typus azonosito
                ItemName = "Glock_19X_9x19_pistol",//nev azonosito
                Description = "The Glock 19X is an Austrian pistol based on the Glock 19 Modular Handgun System developed by Glock.",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                MagasineSize = 17,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 30,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 10.31,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 50,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 90.5,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class Magnum_Research_Desert_Eagle_L5_50_AE_pistol_Default : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/Magnum_Research_Desert_Eagle_L5_50_AE_pistol_Default",//az item képe
                ItemType = "WeaponSecondary",//typus azonosito
                ItemName = "Magnum_Research_Desert_Eagle_L5_50_AE_pistol_Default",//nev azonosito
                Description = "Desert Eagle (Mk XIX) is the third modification of the .50 Action Express caliber sport-hunting pistol.",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                MagasineSize = 7,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 30,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 6.4,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 50,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 67,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class Magnum_Research_Desert_Eagle_L6_50_AE_pistol_WTS_Default : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/Magnum_Research_Desert_Eagle_L6_50_AE_pistol_WTS_Default",//az item képe
                ItemType = "WeaponSecondary",//typus azonosito
                ItemName = "Magnum_Research_Desert_Eagle_L6_50_AE_pistol_WTS_Default",//nev azonosito
                Description = "Desert Eagle L6 is the modification of the .50 Action Express caliber sport-hunting pistol.",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                MagasineSize = 7,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 30,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 6.88,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 50,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 70,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class MP_43_12ga_sawed_off_double_barrel_shotgun : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/MP_43_12ga_sawed_off_double_barrel_shotgun",//az item képe
                ItemType = "WeaponSecondary",//typus azonosito
                ItemName = "MP_43_12ga_sawed_off_double_barrel_shotgun",//nev azonosito
                Description = "A double-barreled sawed-off classic. Ryzhy's personal weapon. Loaded with 12-gauge rounds",
                SizeX = 3,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                MagasineSize = 7,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 900,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 23.38,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 50,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 53,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class RSh_12_12_7x55_revolver_TAC30 : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/RSh_12_12_7x55_revolver_TAC30",//az item képe
                ItemType = "WeaponSecondary",//typus azonosito
                ItemName = "RSh_12_12_7x55_revolver_TAC30",//nev azonosito
                Description = "The powerful RSh-12 (Revolver Shturmovoy 12 - Assault Revolver 12) revolver, manufactured by KBP Instrument Design Bureau, designed for use by special forces.",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                MagasineSize = 5,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 30,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 6.25,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 50,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 40,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class Glock_18C_9x19_machine_pistol : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/Glock_18C_9x19_machine_pistol",//az item képe
                ItemType = "WeaponSecondary",//typus azonosito
                ItemName = "Glock_18C_9x19_machine_pistol",//nev azonosito
                Description = "The Glock 18 is a selective-fire variant of the Glock 17.",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                MagasineSize = 17,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 1200,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 12.03,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 50,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 87,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class Glock_17_9x19_pistol_Tac_2 : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/Glock_17_9x19_pistol_Tac_2",//az item képe
                ItemType = "WeaponSecondary",//typus azonosito
                ItemName = "Glock_17_9x19_pistol_Tac_2",//nev azonosito
                Description = "Glock 17 is an Austrian pistol designed by Glock company for the Austrian army purposes.",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                MagasineSize = 17,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 30,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 11.69,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 50,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 87,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };

        }
    }

    public class TT_33_7_62x25_TT_pistol_Golden : Item
    {
        public Item Set()
        {
            return new Item()
            {
                ImgPath = "Textures/ItemTextures/Weapons/TT_33_7_62x25_TT_pistol_Golden",//az item képe
                ItemType = "WeaponSecondary",//typus azonosito
                ItemName = "TT_33_7_62x25_TT_pistol_Golden",//nev azonosito
                Description = "A legendary pistol that has seen numerous military conflicts throughout the years and is still in service in certain regions of the world, in one variation or another.",
                SizeX = 2,//az item slotokban elfoglalt szelessege
                SizeY = 1,//az item slotokban elfoglalt magassaga

                MagasineSize = 8,//alap tár kapacitás
                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 30,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 12.03,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 50,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 70,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
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
                MagasineSize = 30,//alap tár kapacitás
                AmmoType = "7.69x39",

                Spread = 1,//a spread minusz értéke és plusz erteke közötti random generált szögben indul ki a lövedék, az az ha ez 0 akkor minden lövedék pontos, ezt a recoil befolyasolja
                Fpm = 500,//lovés percenként
                Recoil = 1,//egy szorzó mely minden lövéssel önmagát szorozza, továbbá értéke mindig szorzatban áll a spreaddal.
                Accturacy = 100,//pontosság a lövedék találatának esélye, ugyan is a jatekban van sebzes nem csak testre hanem testrészekre, minnél pontosabb egy fegyver annál kevesebb a karcolás, RHA esélye illetve annál nagyobb az esélye hogy a lovedék ott sebez ahol a talalat erkezik.
                Range = 1000,//az a pont ameddig az általa ellőtt lövedék elmegy 1 = 1 méter
                Ergonomy = 100,//a célzás során a célkereszt mozog ha 0 akkor nagyon mozog ha 100 akkor egyáltalán nem.
                BulletType = new BulletType()//lőszer tipusa ez tartalmazza a sebzest, páncél átutest, stb NINCS KÉSZ
                {
                    // ez ugyan ugy fog kinezni mint a TestWeapon.Set() az az ez is egy páldány lesz
                    //Köteleő tartalom
                },
            };
        }
    }
}