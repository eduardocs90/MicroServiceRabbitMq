using System.ComponentModel.DataAnnotations;


namespace Domain.Entities
{


    public class User
    {
        public int Id { get; set; }

        [Required] // Indica que esse campo é obrigatório
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; }

        [Required] // Indica que esse campo é obrigatório
        [EmailAddress(ErrorMessage = "O e-mail fornecido não é válido.")]
        public string Email { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "O saldo deve ser um valor positivo.")]
        public decimal? Saldo { get; set; }

        public DateTime? DataCriacao { get; set; } = DateTime.Now; // Define a data de criação como o momento atual por padrão
    }
}
