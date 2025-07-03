using InventorySystem.Models;
using MySql.Data.MySqlClient;

namespace InventorySystem.Repositories;

public class MySqlProductRepository : IProductRepository
// java: implement -> interface
// java: extend -> parentObj
{
    private readonly string _connectionString;

    // constructor
    public MySqlProductRepository(string connectionString)
    {
        _connectionString = connectionString;
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();
                string createTableSql = @"
                 create table if not exists products(
                     id int primary key auto_increment,
                     name varchar(100) not null,
                     price decimal(10, 2) not null,
                     quantity int not null,
                     status int not null -- 對應enum的整數直
                 );";
                using (MySqlCommand cmd = new MySqlCommand(createTableSql, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("初始化MySql成功或已存在");
            }
            catch (MySqlException e)
            {
                Console.WriteLine($"初始化MySql失敗:{e.Message}");
            }
        }
    }

    public List<Product> GetAllProducts()
    {
        List<Product> products = new List<Product>();
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string selectSql = "SELECT * FROM products";
            // 1 box
            // 2 dish
            // 3 phone
            using (MySqlCommand cmd = new MySqlCommand(selectSql, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        // reader = 1 box -> 2 dish -> 3 phone
                    {
                        // 1. original way
                        // Product product = new Product(reader.GetInt32("id"),
                        //     reader.GetString("name"),
                        //     reader.GetDecimal("price"),
                        //     reader.GetInt32("quantity"));
                        // product.Status = (Product.ProductStatus)reader.GetInt32("status");
                        // products.Add(product);

                        // 2. obj initializer
                        products.Add(new Product(reader.GetInt32("id"),
                            reader.GetString("name"),
                            reader.GetDecimal("price"),
                            reader.GetInt32("quantity"))
                        {
                            Status = (Product.ProductStatus)reader.GetInt32("status")
                        });
                    }
                }
            }
        }

        return products;
    }

    public Product GetProductById(int id)
    {
        Product product = null;
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            // string selectSql = "SELECT * FROM products WHERE id =" + id;
            // generate by AI
            string selectSql = "SELECT * FROM products WHERE id = @id";

            using (MySqlCommand cmd = new MySqlCommand(selectSql, connection))
            {
                // 防止sql injection
                cmd.Parameters.AddWithValue("@id", id);
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product(reader.GetInt32("id"),
                            reader.GetString("name"),
                            reader.GetDecimal("price"),
                            reader.GetInt32("quantity"))
                        {
                            Status = (Product.ProductStatus)reader.GetInt32("status")
                        };
                    }
                }
            }
        }

        return product;
    }

    public void AddProduct(string name, decimal price, int quantity)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string insertSql =
                "INSERT INTO products  (name, price, quantity, status) VALUES (@name, @price, @quantity, @status)";
            using (MySqlCommand cmd = new MySqlCommand(insertSql, connection))
            {
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                // todo refactor
                cmd.Parameters.AddWithValue("@status", 1);
                cmd.ExecuteNonQuery();
            }
        }
    }
}