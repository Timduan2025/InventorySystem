using InventorySystem.Models;
using InventorySystem.Repositories;
using InventorySystem.Utils;

namespace InventorySystem.Services;

public class InventoryService
{
    // 1. 資料庫相關
    private readonly IProductRepository _productRepository;

    // 透過建構子，注入介面
    public InventoryService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public List<Product> GetAllProducts()
    {
        try
        {
            // 呼叫介面，而非實作 (DI)
            List<Product> products = _productRepository.GetAllProducts();
            if (!products.Any())
            {
                Console.WriteLine("No products found");
            }

            // 2. 通知功能相關
            // 使用EmailNotifier
            INotifier emailNotifier = new EmailNotifier();
            NotificationService emailService = new NotificationService(emailNotifier);
            emailService.NotifyUser("Tom", $"查詢完成");
            return products;
        }
        catch (Exception e)
        {
            Console.WriteLine("讀取產品列表失敗:{e.Message}");
            return new List<Product>();
        }
    }

    public Product SearchProductById()
    {
        Console.Write("請輸入產品編號：");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var product = _productRepository.GetProductById(id);
            if (product == null)
            {
                Console.WriteLine("查無此產品。");
            }

            return product;
        }

        Console.WriteLine("輸入格式錯誤。");
        return null;
    }

    public OperationResults<Product> GetProductById(int id)
    {
        try
        {
            Product product = _productRepository.GetProductById(id);
            if (product == null)
            {
                return OperationResults<Product>.ErrorResult("查無該產品");
            }

            return OperationResults<Product>.SuccessResult("操作成功", product);
        }
        catch (Exception e)
        {
            return OperationResults<Product>.ErrorResult($"讀取產品列表失敗:{e.Message}");
        }
    }

    public void AddProduct(string? name, decimal price, int quantity)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("產品名稱不能為空。");
            }

            //價格必須大於零
            if (price <= 0)
            {
                throw new ArgumentException("價格必須大於零。");
            }

            //數量不能小於零
            if (quantity < 0)
            {
                throw new ArgumentException("數量不能小於零。");
            }

            // 嘗試透過Repository新增產品
            var product = new Product(
                _productRepository.GetNextProductId(),
                name, price, quantity);
            // var newProduct = new Product (name, price, quantity)
            _productRepository.AddProduct(product);
        }
        catch (Exception e)
        {
            Console.WriteLine($"\n 錯誤:{e.Message}");
        }
    }

    public void UpdateProduct(Product product, string? name, decimal price, int quantity)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("產品名稱不能為空。");
            }

            if (price <= 0)
            {
                throw new ArgumentException("價格必須大於零。");
            }

            if (quantity < 0)
            {
                throw new ArgumentException("數量不能小於零。");
            }

            //執行更新(覆蓋origin product的屬性)
            product.Name = name;
            product.Price = price;
            product.Quantity = quantity;
            product.UpdateStatus();
            //呼叫repository
            _productRepository.UpdateProduct(product);
            Console.WriteLine($"產品Id: {product.Id} 已更新");
        }
        catch (Exception e)
        {
            Console.WriteLine($"錯誤:更新產品失敗:{e.Message}");
        }
    }

    public OperationResults<List<Product>> SearchProduct(string? input)
    {
        try
        {
            List<Product> products = _productRepository.GetAllProducts();
            if (string.IsNullOrWhiteSpace(input))
            {
                return OperationResults<List<Product>>.ErrorResult("請勿使用空字串");
            }

            var results = products
                .Where(product => product.Name.ToLower().Contains(input.ToLower()))
                .OrderBy(product => product.Name).ToList();

            if (!results.Any())
            {
                Console.WriteLine("No products found");
            }
            return OperationResults<List<Product>>.SuccessResult("操作成功", results);
        }
        catch (Exception e)
        {
            return OperationResults<List<Product>>.ErrorResult("讀取產品列表失敗:{e.Message}");
        }
    }

    public List<Product> SearchLowProduct()
    {
        try
        {
            List<Product> products = _productRepository.GetAllProducts();

            var results = products
                .Where(product => product.Quantity < 20)
                .Where(product => product.Status == Product.ProductStatus.LowStock)
                .OrderBy(product => product.Name).ToList();

            if (!results.Any())
            {
                Console.WriteLine("No products found");
            }

            return results;
        }
        catch (Exception e)
        {
            Console.WriteLine("讀取產品列表失敗:{e.Message}");
            return new List<Product>();
        }
    }

    public List<Product> SearchOutOfProduct()
    {
        try
        {
            List<Product> outOfProducts = _productRepository.GetAllOutOfProduct();

            if (!outOfProducts.Any())
            {
                Console.WriteLine("No products found");
            }

            return outOfProducts;
        }
        catch (Exception e)
        {
            Console.WriteLine("讀取產品列表失敗:{e.Message}");
            return new List<Product>();
        }
    }

    public void AdjustProductQuantity(Product product, int quantity)
    {
        int newQuantity = product.Quantity + quantity;
        if (newQuantity < 0)
        {
            Console.WriteLine($"庫存不足，嘗試操作 出庫{quantity}。");
        }
        product.Quantity = newQuantity;
        product.UpdateStatus();
        _productRepository.UpdateProduct(product);
        Console.WriteLine($"成功更新產品:{product.Name}，當前庫存為 {newQuantity}");
    }
}