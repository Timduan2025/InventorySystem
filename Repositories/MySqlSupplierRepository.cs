using InventorySystem.Models;
using MySql.Data.MySqlClient;

namespace InventorySystem.Repositories;

public class MySqlSupplierRepository : ISupplierRepository
{
    private readonly string _connectionString;

    public MySqlSupplierRepository(string connectionString)
    {
        _connectionString = connectionString;
        CreateSupplierTable();
    }

    public void CreateSupplierTable()
    {
        using (var connection = new MySqlConnection(_connectionString))
        {
            try
            {
                connection.Open();
                string createTableSql = @"
                 create table if not exists suppliers(
                     id int primary key auto_increment,
                     name varchar(100) not null,
                     address varchar(100) not null,
                     phone varchar(100) not null,
                     email varchar(100) not null
                 );";
                using (MySqlCommand cmd = new MySqlCommand(createTableSql, connection))
                {
                    cmd.ExecuteNonQuery();
                }

                Console.WriteLine("[Supplier]初始化MySql成功或已存在");
            }
            catch (MySqlException e)
            {
                Console.WriteLine($"[Supplier]初始化MySql失敗:{e.Message}");
            }
        }
    }

    public void AddSupplier(Supplier supplier)
    {
        throw new NotImplementedException();
    }

    public List<Supplier> GetAllSuppliers()
    {
        throw new NotImplementedException();
    }

    public Supplier GetSupplierById(int id)
    {
        throw new NotImplementedException();
    }

    public void UpdateSupplier(Supplier supplier)
    {
        throw new NotImplementedException();
    }

    public void DeleteSupplier(Supplier supplier)
    {
        throw new NotImplementedException();
    }

    public void ExistSupplier(int id)
    {
        throw new NotImplementedException();
    }
}