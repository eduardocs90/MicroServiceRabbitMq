namespace Shared
{
    public class UserNotification
    {
        public int UserId { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public decimal? Saldo { get; set; }

        public DateTime? DataCriacao { get; set; } = DateTime.UtcNow;
    }

}
