using InventorySystem.Models;

namespace InventorySystem.Repositories;

public interface IProductRepository
{
    List<Product> GetAllProducts();
    Product GetProductById(int id);
    void AddProduct(Product product);
    List<Product> SearchProduct();
    int GetNextProductId();
    void UpdateProduct(Product product);
}