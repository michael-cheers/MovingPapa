using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovingPapa.DB;

namespace MovingPapa.Pages
{
    public class book_rateModel : PageModel
    {
        readonly MovingpapaContext DB;
        public book_rateModel(MovingpapaContext db) => DB = db;
        public async Task<IActionResult> OnGet(string uuid)
        {
            var quote = await DB.QuotesAndContacts.SingleAsync(q => q.Uuid == uuid);
            await DB.Moves.AddAsync(new()
            {
                MoveDate = quote.MoveDate,
                MoveDetails = quote.MoveInfo,
                MoveTime = quote.MoveTime,
                PriceInCents = quote.PriceInCents.Value,
                QuoteId = quote.Id,
                Uuid = quote.Uuid,
                Email = quote.Email,
                PhoneNumber = quote.PhoneNumber
            });
            await DB.SaveChangesAsync();
            return Content(uuid);
        }
    }
}
