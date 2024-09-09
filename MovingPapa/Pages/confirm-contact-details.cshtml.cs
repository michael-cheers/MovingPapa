using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovingPapa.DB;

namespace MovingPapa.Pages
{
    public class confirm_contact_detailsModel : PageModel
    {
        readonly MovingpapaContext DB;
        public confirm_contact_detailsModel (MovingpapaContext db) => DB = db;
        public async Task<IActionResult> OnGet(string fullName, string email, string phoneNumber, string service, string points, string moveDate, string moveTime, bool isCallNow, string? uuid = null)
        {
            uuid ??= Guid.NewGuid().ToString();
            Enum.Parse<MoveTime>(moveTime.Replace(" ", ""));
            await DB.QuotesAndContacts.AddAsync(new()
            {
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                Uuid = uuid,
                IsMuscleOnly = service == "Muscle only",
                Addresses = points,
                IsCallNow = isCallNow,
                MoveDate = DateTime.Parse(moveDate),
                MoveTime = moveTime.ToString()
            });
            await DB.SaveChangesAsync();
            return Content(uuid);
        }
    }
}
