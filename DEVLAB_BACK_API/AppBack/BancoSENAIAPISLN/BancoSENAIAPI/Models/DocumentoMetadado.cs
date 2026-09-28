using System.ComponentModel.DataAnnotations;

namespace BancoSENAIAPI.Models
{
    public class DocumentoMetadado
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nome { get; set;}
        [Required]
        public string Extensao { get; set;}
        [Required]
        public string Caminho { get; set; }
        [Required]
        public int CodigoCliente { get; set; }

    }
}
