using InventorySystem.Models;

namespace InventorySystem.Repositories;

public interface ISupplierRepository
{
    void CreateSupplierTable();
    void AddSupplier(Supplier supplier);
    List<Supplier> GetAllSuppliers();
    Supplier GetSupplierById(int id);
    void UpdateSupplier(Supplier supplier);
    void DeleteSupplier(Supplier supplier);
    void ExistSupplier(int id);
    
}