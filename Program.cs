// See https://aka.ms/new-console-template for more information

using InventorySystem.Models;
using InventorySystem.Repositories;

//Server: mysql所在伺服器位置 (localhost or ip xxx.xxx.xxx.xxx)
//Port: mysql端口 (預設3306)
//Database: inventory_db(CREATE DATABASE inventory_db;)
//uid: mysql使用者名稱
//pwd: mysql使用者密碼
const string MYSQL_CONNECTION_STRING = "Server=localhost;Port=3306;Database=inventory_db;uid=root;pwd=Root;";

MySqlProductRepository productRepository = new MySqlProductRepository(MYSQL_CONNECTION_STRING);

RunMenu();

void RunMenu()
{
    while (true)
    {
        DisplayMenu();
        string input = Console.ReadLine();
        switch (input)
        {
            case "1": GetAllProduct(); break;
            case "2": SearchProduct(); break;
            case "0": 
                Console.WriteLine("Goodbye");
                return;
        }

        void DisplayMenu()
        {
            Console.WriteLine("Welcome to the inventory system!");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. 查看所有產品");
            Console.WriteLine("2. 查詢產品");
            Console.WriteLine("0. 離開");
        }
        
        void GetAllProduct()
        {
            Console.WriteLine("\n--- 所有產品列表 ---");
            var products= productRepository.GetAllProducts();
            if (products != null)
            {
                Console.WriteLine("--------------------");
                Console.WriteLine("ID | Name | Price | Quantity | Status");
                Console.WriteLine("--------------------");
                foreach (var product in products)
                {
                    Console.WriteLine(product);   
                }
                Console.WriteLine("----------------------");
            }
        }
        
        void SearchProduct()
        {
            throw new NotImplementedException();
        }
    }
}