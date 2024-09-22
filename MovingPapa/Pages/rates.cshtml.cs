using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovingPapa.DB;
using System.Text.Json;

namespace MovingPapa.Pages
{
    public class ratesModel : PageModel
    {
        readonly MovingpapaContext DB;
        public ratesModel(MovingpapaContext db) => DB = db;
        public IActionResult OnGet() => Content(JsonSerializer.Serialize(
            DB.RateCalendars.Select(c => new {
                Date = c.Date.ToString("yyyy-MM-dd"),
                c.RatePerMoverInCents
            })
        ));
    }
}
