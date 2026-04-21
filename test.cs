//### EF Core schema for v1
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<Client> Clients { get; set; } = new();
    public List<Service> Services { get; set; } = new();
    public List<Quote> Quotes { get; set; } = new();
    public List<Invoice> Invoices { get; set; } = new();
}

public class Client
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    public User User { get; set; } = null!;
}

public class Service
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal DefaultPrice { get; set; }

    public User User { get; set; } = null!;
}

public enum QuoteStatus { Draft, Sent, Viewed, Approved, Declined }

public class Quote
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ClientId { get; set; }
    public string QuoteNumber { get; set; } = null!; // Q-2026-0001
    public Guid PublicId { get; set; } = Guid.NewGuid();
    public QuoteStatus Status { get; set; } = QuoteStatus.Draft;
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public decimal DepositRequired { get; set; }
    public string? Notes { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public List<QuoteItem> Items { get; set; } = new();
    public Invoice? Invoice { get; set; }
}

public class QuoteItem
{
    public Guid Id { get; set; }
    public Guid QuoteId { get; set; }
    public Guid? ServiceId { get; set; } // nullable so deleting service doesn't break history
    public string Name { get; set; } = null!; // denormalized
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public Quote Quote { get; set; } = null!;
}

public enum InvoiceStatus { Unpaid, Partial, Paid, Void }

public class Invoice
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ClientId { get; set; }
    public Guid QuoteId { get; set; }
    public string InvoiceNumber { get; set; } = null!; // INV-2026-0001
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public decimal AmountPaid { get; set; }
    public string? StripePaymentIntent { get; set; }
    public string? PdfUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Client Client { get; set; } = null!;
    public Quote Quote { get; set; } = null!;
    public List<InvoiceItem> Items { get; set; } = new();
}

public class InvoiceItem
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public string Name { get; set; } = null!;
    public decimal Qty { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public Invoice Invoice { get; set; } = null!;
}

// Key EF config tips  
// - Use HasPrecision(18,2) for all decimals.  
// - Index Quote.PublicId and Quote.QuoteNumber for fast lookups.  
// - Quote → Invoice is 1:0..1 relationship.Set FK Invoice.QuoteId unique.
