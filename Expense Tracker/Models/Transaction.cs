using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Range(1,int.MaxValue,ErrorMessage ="Будь ласка, виберіть категррію.")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [Required(ErrorMessage = "Сума є обов'язковою.")]
        [RegularExpression(@"^\d+(\,\d{1,2})?$", ErrorMessage = "Неправильно введена сума. Перевірте введені дані.")]
        [Range(1, int.MaxValue, ErrorMessage = "Кількість має бути більше за 0.")]
        public int Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? CategoryTitleWithIcon
        {
            get
            {
                return Category == null ? "" : Category.Icon + " " + Category.Title;
            }
        }

        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                return ((Category == null || Category.Type == "Income") ? "+ " : "- ") + Amount.ToString("C0");
            }
        }

    }
}
