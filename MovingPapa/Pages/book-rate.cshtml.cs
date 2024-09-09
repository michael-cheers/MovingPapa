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
        public static string GetOrderId (string uuid)
        {
            return new string(uuid.Take(5).Select(c => c switch
            {
                >='0' and <= '9' => (char)(c + ('A' - '0')),
                _ => char.ToUpper(c)
            }).ToArray());
        }
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
            string orderId = GetOrderId(uuid);
            await new EmailService().SendMessage(quote.Email, $"Your Move {orderId} Confirmed",
                @$"Dear {quote.FullName},
Thank you for booking with us!

Your card will be charged ${quote.PriceInCents / 100m - 50} 24 hours before your move.

Order Code
{orderId}

Thank you!".Replace(Environment.NewLine, "<br>"));
            await DB.SaveChangesAsync();
            return Content(uuid);
        }
    }
}
