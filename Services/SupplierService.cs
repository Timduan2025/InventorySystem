using InventorySystem.Models;
using InventorySystem.Repositories;

namespace InventorySystem.Services;

public class SupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    public SupplierService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public void AddSupplier(String name, String address, String phone, String email)
    {
        Supplier supplier = new Supplier(name, address, phone, email);
        _supplierRepository.AddSupplier(supplier);
    }
}