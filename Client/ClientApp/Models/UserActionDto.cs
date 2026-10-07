namespace ClientApp.Models
{
    public class UserActionDto
    {
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string ActionType { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
    }
}
