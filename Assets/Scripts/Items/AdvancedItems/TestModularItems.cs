using ItemHandler;

namespace TestModulartItems
{
    internal class TestModularItems
    {
        public class TestCenter : Item
        {
            public Item Set()
            {
                return new Item()
                {
                    ItemType = "TestCenter",//typus azonosito
                    ItemName = "TestCenter",//nev azonosito
                    Description = "...",
                    SizeX = 1,
                    SizeY = 1,
                    IsAdvancedItem = true,
                };
            }
        }
        public class TestBox : Item
        {
            public Item Set()
            {
                return new Item()
                {
                    ItemType = "TestBox",//typus azonosito
                    ItemName = "TestBox",//nev azonosito
                    Description = "...",
                    SizeX = 1,
                    SizeY = 1,
                    IsAdvancedItem = true,
                    SizeChanger = new SizeChanger()
                    {
                        Plus = 1,//mekkora meretet adjon hozza
                        MaxPlus = 2,//mekkora a maximalis size amelyikbe novelhet
                        Direction = "R",//menyik iranyba adja hozza a meretet
                    }
                };
            }
        }
    }
}

/*
 *              "TestCenter" => new AK103().Set(),
                "TestBox" => new AK103().Set(),
                "TestUpper" => new AK103().Set(),
                "TestButtom" => new AK103().Set(),
                "TestHead" => new AK103().Set(),
                "TestFoot" => new AK103().Set(),
                "TestFront" => new AK103().Set(),
                "TestBack" => new AK103().Set(),
 * */