// See https://aka.ms/new-console-template for more information

using InventorySystem.Models;
using InventorySystem.Repositories;
using InventorySystem.Services;
using InventorySystem.Utils;

//Server: mysql所在伺服器位置 (localhost or ip xxx.xxx.xxx.xxx)
//Port: mysql端口 (預設3306)
//Database: inventory_db(CREATE DATABASE inventory_db;)
//uid: mysql使用者名稱
//pwd: mysql使用者密碼
const string MYSQL_CONNECTION_STRING = "Server=localhost;Port=3306;Database=inventory_db;uid=root;pwd=Root;";

MySqlProductRepository productRepository = new MySqlProductRepository(MYSQL_CONNECTION_STRING);
InventoryService inventoryService = new InventoryService(productRepository);

// 通知功能相關
// 使用EmailNotifier
INotifier emailNotifier = new EmailNotifier();
NotificationService emailService = new NotificationService(emailNotifier);

// 使用EmailNotifier
INotifier smsNotifier = new SmsNotifier();
NotificationService SmsService = new NotificationService(smsNotifier);
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
            case "3": AddProduct(); break;
            case "4": UpdateProduct(); break;
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
            Console.WriteLine("3. 新增產品");
            Console.WriteLine("4. 更新產品");
            Console.WriteLine("0. 離開");
        }
        
        void GetAllProduct()
        {
            Console.WriteLine("\n--- 所有產品列表 ---");
            var products= inventoryService.GetAllProducts();
            if (products.Any())
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
            emailService.NotifyUser("Tom", "查詢完成");
            
        }
        
        void SearchProduct()
        {
                Console.WriteLine("輸入欲查詢的產品編號");
                int input = ReadIntLine(1);
                // var product = productRepository.GetProductById(input)
                var product = inventoryService.GetProductById(input);
                if (product != null)
                {
                    Console.WriteLine("--------------------");
                    Console.WriteLine("ID | Name | Price | Quantity | Status");
                    Console.WriteLine("--------------------");
                    Console.WriteLine(product);
                    Console.WriteLine("--------------------");
                }
        }    
        
        void AddProduct()
        {
            Console.WriteLine("輸入產品名稱:");
            string name = Console.ReadLine();
            Console.WriteLine("輸入產品價格:");
            decimal price = ReadDecimalLine();
            Console.WriteLine("輸入產品數量:");
            int quantity = ReadIntLine();
            inventoryService.AddProduct(name, price, quantity);
            // productRepository.AddProduct(name, price, quantity);
            SmsService.NotifyUser("Nancy", "查詢成功。");
        }

        void UpdateProduct()
        {
            Console.WriteLine("請輸入要更新的產品ID");
            int id = ReadIntLine();
            //找到對應產品
            var product = inventoryService.GetProductById(id);
            if (product == null)
            {
                return;
            }
            Console.WriteLine("輸入新名稱:");
            string name = Console.ReadLine();
            Console.WriteLine("輸入新價格:");
            decimal price = ReadDecimalLine();
            Console.WriteLine("輸入新數量:");
            int quantity = ReadIntLine();
            // service.UpdateProduct
            inventoryService.UpdateProduct(product, name, price, quantity);
        }
        
        int ReadInt(string input)
        {
            try
            {
                return Convert.ToInt32(input);
            }
            catch (FormatException e)
            {
                Console.WriteLine("請輸入有效數字。");
                return 0;
            }
        }

        int ReadIntLine(int defaultValue = 0)
        {
            while (true)
            {
                String input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input) && defaultValue != 0)
                {
                    return defaultValue;
                }

                if (int.TryParse(input, out int value))
                {
                    return value;
                }
                else
                {
                    Console.WriteLine("請輸入有效數字。");
                }
            }
        }
    }
}

decimal ReadDecimalLine(decimal defaultValue = 0.0m)
{
    while (true)
    {
        String input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) && defaultValue != 0.0m)
        {
            return defaultValue;
        }
        //string parsing to int
        if (decimal.TryParse(input, out decimal value))
        {
            return value;
        }
        else
        {
            Console.WriteLine("請輸入有效數字。");
        }
    }
}