using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace OrderInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;
    public OrdersController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetAll() =>
        await _db.Orders.Include(o => o.Items).AsNoTracking()
            .OrderByDescending(o => o.CreatedAt).ToListAsync();

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Order>> Get(int id)
    {
        var o = await _db.Orders.Include(x => x.Items).AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return o is null ? NotFound() : o;
    }

    // Places an order and reduces stock inside one transaction.
    [HttpPost]
    public async Task<ActionResult<Order>> Create(CreateOrderRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.CustomerName) || req.Items is null || req.Items.Count == 0)
            return BadRequest("Customer name and at least one item are required.");

        await using var tx = await _db.Database.BeginTransactionAsync();
        var order = new Order { CustomerName = req.CustomerName };

        foreach (var line in req.Items)
        {
            if (line.Quantity <= 0) return BadRequest("Quantity must be greater than zero.");
            var product = await _db.Products.FindAsync(line.ProductId);
            if (product is null) return NotFound($"Product {line.ProductId} not found.");
            if (product.Stock < line.Quantity) return BadRequest($"Not enough stock for {product.Name}.");

            product.Stock -= line.Quantity;
            order.Items.Add(new OrderItem { ProductId = product.Id, Quantity = line.Quantity, UnitPrice = product.Price });
            order.Total += product.Price * line.Quantity;
        }

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        return CreatedAtAction(nameof(Get), new { id = order.Id }, order);
    }
}
