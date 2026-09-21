namespace ClientApp.Models;

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty; // Додано це поле
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
