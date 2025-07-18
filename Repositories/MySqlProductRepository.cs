using InventorySystem.Models;
using MySql.Data.MySqlClient;

namespace InventorySystem.Repositories;

public class MySqlProductRepository : IProductRepository
// java: implement -> interface
// java: extend -> parentObj
{
    private readonly string _connectionString;
    private IProductRepository _productRepositoryImplementation;
    private IProductRepository _productRepositoryImplementation1;
    private List<Product> _getAllOutOfProduct;

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

                Console.WriteLine("[Product]初始化MySql成功或已存在");
            }
            catch (MySqlException e)
            {
                Console.WriteLine($"[Product]初始化MySql失敗:{e.Message}");
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
    // id= or '1'='1'
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
                    while (reader.Read())
                    {
                        //object init
                        //inline
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

    public void AddProduct(Product product)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string insertSql = @"INSERT INTO products (name, price, quantity, status) 
                                    values (@name, @price, @quantity, @status)";
            using (MySqlCommand cmd = new MySqlCommand(insertSql, connection))
            {
                // 防止sql injection
                cmd.Parameters.AddWithValue("@id", product.Id);
                cmd.Parameters.AddWithValue("@name", product.Name);
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@quantity", product.Quantity);
                cmd.Parameters.AddWithValue("@status", product.Status);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public List<Product> SearchProduct()
    {
        throw new NotImplementedException();
    }

    public int GetNextProductId()
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string selectSql = @"SELECT IFNULL(MAX(id), 0) FROM products";
            using (MySqlCommand cmd = new MySqlCommand(selectSql, connection))
            {
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    return Convert.ToInt32(result) + 1;
                }
                return 0;
            }
        }
    }

    public void UpdateProduct(Product product)
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string insertSql = @"UPDATE products SET 
                    name=@name, price=@price, quantity=@quantity, status=@status
                WHERE id=@id"; 
            using (MySqlCommand cmd = new MySqlCommand(insertSql, connection))
            {
                // 防止sql injection
                cmd.Parameters.AddWithValue("@id", product.Id);
                cmd.Parameters.AddWithValue("@name", product.Name);
                cmd.Parameters.AddWithValue("@price", product.Price);
                cmd.Parameters.AddWithValue("@quantity", product.Quantity);
                cmd.Parameters.AddWithValue("@status", product.Status);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public List<Product> GetAllOutOfProduct()
    {
        List<Product> products = new List<Product>();
        using (var connection = new MySqlConnection(_connectionString))
        {
            connection.Open();
            string selectSql = @"SELECT products FROM status = 2";
            using (MySqlCommand cmd = new MySqlCommand(selectSql, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
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
}