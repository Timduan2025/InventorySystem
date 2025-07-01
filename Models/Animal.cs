namespace InventorySystem.Models;

public abstract class Animal
{
    public string Name { get; set; }

    public Animal() 
    {
    }

    public Animal(string name)
    {
        Name = name;
    }

    public abstract void MakeSound();
}