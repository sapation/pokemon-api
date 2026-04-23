using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Pokemon;
using Pokemon.Data;
using Pokemon.Helper;
using Pokemon.Interfaces;
using Pokemon.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddTransient<Seed>();
builder.Services.AddControllers().AddJsonOptions(x => 
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);
builder.Services.AddScoped<IPokemonRepository, PokemonRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICountryRepository, CountryRepository>();
builder.Services.AddScoped<IReviewerRepository, ReviewerRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registering database
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

if (args.Length == 1 && args[0].ToLower() == "seeddata")
    SeedData(app);

void SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<Seed>();
        service.SeedDataContext();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
// adding api controllers
app.MapControllers();

app.Run();

//dotnet run seeddata
//dotnet ef migrations add InitialCreate
//dotnet ef database update
//dotnet add package Microsoft.EntityFrameworkCore.Design



// ### Database schema
// Table | Key fields | Notes
// * Users * | id, email, name, company_name, created_at | One user = one business account for v1
// *Clients* | id, user_id, name, phone, email, address | Belongs to a User
// *Services* | id, user_id, name, description, default_price | Saved line items to speed up quoting
// *Quotes* | id, user_id, client_id, quote_number, status, subtotal, tax, total, deposit_required, notes, public_id, expires_at | status: draft, sent, viewed, approved, declined
// *QuoteItems* | id, quote_id, service_id, name, qty, unit_price, line_total | Denormalize name/price so edits don’t break old quotes
// *Invoices* | id, user_id, client_id, quote_id, invoice_number, status, subtotal, tax, total, amount_paid, stripe_payment_intent, pdf_url | status: unpaid, partial, paid, void
// *InvoiceItems* | id, invoice_id, name, qty, unit_price, line_total | Copied from QuoteItems on convert
// Relationships: User has many Clients, Services, Quotes, Invoices. Quote has many QuoteItems. Invoice has many InvoiceItems. Quote hasOne Invoice.

