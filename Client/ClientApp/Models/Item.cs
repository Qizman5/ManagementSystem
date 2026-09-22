namespace ClientApp.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string OperationType { get; set; } = "Прихід (Прибуття)";
    }
}
