namespace InventorySystem.Models;

public class Product
{
    public enum ProductStatus
    {
        InStock, //有庫存 -0
        LowStock, //庫存偏低 -1
        OutOfStock //沒有庫存 -2
    }
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public ProductStatus Status { get; set; }

    public Product()
    {
    }

    public Product(int id, string name, decimal price, int quantity)
    {
        Id = id;
        Name = name;
        Price = price;
        Quantity = quantity;
        UpdateStatus();
    }

    public override string ToString()
    {
        return $"Id: {Id}, Name: {Name}, Price: {Price}, Quantity: {Quantity}";
    }

    public void UpdateStatus()
    {
        if (Quantity <= 0)
        {
            Status = ProductStatus.OutOfStock;
        }
        else if (Quantity < 20)
        {
            Status = ProductStatus.LowStock;
        }
        else
        {
            Status = ProductStatus.InStock;
        }
    }
}