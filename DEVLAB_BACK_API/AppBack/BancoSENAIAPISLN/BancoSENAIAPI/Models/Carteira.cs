using System.ComponentModel.DataAnnotations;

namespace BancoSENAIAPI.Models
{
    public class Carteira
    {
        [Key]
        public int NumeroCarteira { get; set; }
        [Required]
        public string NomeCarteira { get; set; }
        [Required]
        public decimal ApetiteCarteira { get; set; }
    }
}
