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
string connectionString = "";
string configFile = "appsettings.ini";

if (File.Exists(configFile))
{
    Console.WriteLine($"Reading {configFile} file");
    try
    {
        Dictionary<string, Dictionary<string, string>> config = ReadFile(configFile);

        if (config.ContainsKey("Database"))
        {
            var dbConfig = config["Database"];
            connectionString = $"Server={dbConfig["Server"]};" +
                               $"Port={dbConfig["Port"]};" +
                               $"Database={dbConfig["Database"]};" +
                               $"uid={dbConfig["Uid"]};" +
                               $"pwd={dbConfig["Pwd"]}";
            Console.WriteLine($"讀取資料庫連接字串成功!");
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"錯誤:讀取配置檔案失敗:{e}");
        // throw
        connectionString = MYSQL_CONNECTION_STRING;
    }
}
else
{
    Console.WriteLine($"錯誤:配置檔案{configFile}不存在");
    connectionString = MYSQL_CONNECTION_STRING;
}

MySqlProductRepository productRepository = new MySqlProductRepository(connectionString);
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
            case "2": SearchProductById(); break;
            case "3": AddProduct(); break;
            case "4": UpdateProduct(); break;
            case "5": SearchProduct(); break;
            case "6": SearchLowProduct(); break;
            case "7": SearchOutOfProduct(); break;
            case "8": AdjustProductQuantity(); break;
            case "0": 
                Console.WriteLine("Goodbye");
                return;
        }

        void DisplayMenu()
        {
            Console.WriteLine("Welcome to the inventory system!");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("1. 查看所有產品");
            Console.WriteLine("2. 查詢產品ID");
            Console.WriteLine("3. 新增產品");
            Console.WriteLine("4. 更新產品");
            Console.WriteLine("5. 查詢產品");
            Console.WriteLine("6. 查詢庫存偏低");
            Console.WriteLine("7. 查詢已缺貨產品");
            Console.WriteLine("8. 調整產品庫存(出庫/入庫)");
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
        
        void SearchProductById()
        {
                Console.WriteLine("輸入欲查詢的產品編號");
                int input = ReadIntLine(1);
                // var product = productRepository.GetProductById(input)
                OperationResults<Product> product = inventoryService.GetProductById(input);
                if (product.Success)
                {
                    Console.WriteLine("--------------------");
                    Console.WriteLine("ID | Name | Price | Quantity | Status");
                    Console.WriteLine("--------------------");
                    Console.WriteLine(product.Data);
                    Console.WriteLine("--------------------");
                }
        }

        void SearchProduct()
        {
            Console.WriteLine("查詢產品名稱關鍵字:");
            string input = Console.ReadLine();
            OperationResults<List<Product>> results = inventoryService.SearchProduct(input);

            if (results.Data.Any())
            {
                Console.WriteLine($"------查詢條件:({input})-------");
                Console.WriteLine("ID | Name | Price | Quantity | Status");
                Console.WriteLine("--------------------");
                foreach (var product in results.Data)
                {
                    Console.WriteLine(product);   
                }
                Console.WriteLine("--------------------");

            }
        }

        void SearchLowProduct()
        {
            List<Product> products = inventoryService.SearchLowProduct();
            if (products.Any())
            {
                Console.WriteLine($"------產品低庫存清單-------");
                Console.WriteLine("ID | Name | Price | Quantity | Status");
                Console.WriteLine("--------------------");
                foreach (var product in products)
                {
                    Console.WriteLine(product);   
                }
                Console.WriteLine("--------------------");
            }
        }
        
        void SearchOutOfProduct()
        {
            var products = inventoryService.SearchOutOfProduct();
            if (products.Any())
            {
                Console.WriteLine($"------產品零庫存清單-------");
                Console.WriteLine("ID | Name | Price | Quantity | Status");
                Console.WriteLine("--------------------");
                foreach (var product in products)
                {
                    Console.WriteLine(product);   
                }
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
            OperationResults<Product> product = inventoryService.GetProductById(id);
            if (!product.Success)
            {
                Console.WriteLine("product.Message");
                return;
            }
            Console.WriteLine("輸入新名稱:");
            string name = Console.ReadLine();
            Console.WriteLine("輸入新價格:");
            decimal price = ReadDecimalLine();
            Console.WriteLine("輸入新數量:");
            int quantity = ReadIntLine();
            // service.UpdateProduct
            inventoryService.UpdateProduct(product.Data, name, price, quantity);
        }
        
        void AdjustProductQuantity()
        {
            Console.WriteLine("請輸入要調整庫存的產品ID");
            int id = ReadIntLine();
            OperationResults<Product> product = inventoryService.GetProductById(id);
            if (!product.Success)
            {
                Console.WriteLine("product.Message");
                return;
            }
            Console.WriteLine("輸入調整數量(正數入庫/負數出庫):");
            int quantity = ReadIntLine();
            inventoryService.AdjustProductQuantity(product.Data, quantity);
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

Dictionary<string, Dictionary<string, string>> ReadFile(string s)
{
    var config = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
    string currentSection = "";

    foreach (string line in File.ReadLines(s))
    {
        string trimmedLine = line.Trim();
        if (trimmedLine.StartsWith("#") || string.IsNullOrWhiteSpace(trimmedLine))
        {
            continue; // 跳過註釋和空行
        }

        if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
        {
            currentSection = trimmedLine.Substring(1, trimmedLine.Length - 2);
            config[currentSection] = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        else if (currentSection != "" && trimmedLine.Contains("="))
        {
            int equalsIndex = trimmedLine.IndexOf('=');
            string key = trimmedLine.Substring(0, equalsIndex).Trim();
            string value = trimmedLine.Substring(equalsIndex + 1).Trim();
            config[currentSection][key] = value;
        }
    }
    return config;
}