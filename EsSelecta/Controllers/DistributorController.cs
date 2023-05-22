using EsSelecta.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace EsSelecta.Controllers
{
    [ApiController]
    [Route("api/distributor")]
    public class DistributorController : ControllerBase
    {
        private readonly List<Item> items;
        private const string SessionKey = "Credit";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DistributorController(IHttpContextAccessor httpContextAccessor)
        {
            items = new List<Item>
            {
                new Item { Name = "Smarlies", Code = "A01", Quantity = 10, Price = 1 },
                new Item { Name = "Carampar", Code = "A02", Quantity = 5, Price = 2 },
                new Item { Name = "Avril", Code = "A03", Quantity = 2, Price = 1 },
                new Item { Name = "KokoKola", Code = "A04", Quantity = 1, Price = 3 }

            };

            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        [HttpPost("insert/{amount}")]
        public IActionResult Insert(int amount)
        {
            Session.SetInt32(SessionKey, amount);
            return Ok();
        }

        [HttpGet("choose")]
        public IActionResult Choose(string code)
        {
            var credit = Session.GetInt32(SessionKey);

            if (!credit.HasValue)
            {
                return BadRequest("No credit available. Insert credit first.");
            }

            Item selectedItem = items.Find(i => i.Code == code);

            if (selectedItem == null)
            {
                return BadRequest("Invalid selection!");
            }

            if (selectedItem.Quantity == 0)
            {
                return BadRequest($"Item {selectedItem.Name}: Out of stock!");
            }

            if (credit < selectedItem.Price)
            {
                return BadRequest("Not enough money!");
            }

            selectedItem.Quantity--;
            Session.SetInt32(SessionKey, credit.Value - (int)selectedItem.Price);

            return Ok($"Vending {selectedItem.Name}");
        }

        [HttpGet("getchange")]
        public IActionResult GetChange()
        {
            var credit = Session.GetInt32(SessionKey);
            return Ok(credit ?? 0);
        }

        [HttpGet("getbalance")]
        public IActionResult GetBalance()
        {
            var balance = Session.GetInt32(SessionKey) ?? 0;
            return Ok(balance);
        }
    }
}
  
