using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GiecChallenge.Models;

public class Purchase
{
    public Guid id { get; set; }

    public DateTime datePurchase { get; set; }

    [Required]
    public User user { get; set; } = new User();

    public List<ProductPurchase> products { get; set; } = new List<ProductPurchase>();

    //Useful for loan
    public Purchase? initialPurchase { get; set; }
 }