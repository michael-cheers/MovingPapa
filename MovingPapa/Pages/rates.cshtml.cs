using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovingPapa.DB;
using RestSharp;
using System.Text.Json;

namespace MovingPapa.Pages
{
    public class ratesModel : PageModel
    {
        readonly MovingpapaContext DB;
        public ratesModel(MovingpapaContext db) => DB = db;
        public record resultContainer (result result);
        public record result (List<review> reviews);
        public record review (string text, string author_name);
        public async Task<IActionResult> OnGet() {
            RestClient client = new(
                "https://maps.googleapis.com/maps/api/place/details/json?place_id=ChIJoeIpTEozK4gRRdz0QV9yJUw&key=AIzaSyAV5-u4FmaK19MYQByVruZuw6Hn4PHUUt0&fields=reviews"
            );
            return Content(JsonSerializer.Serialize(
            new
            {
                rates = DB.RateCalendars.Select(c => new
                {
                    Date = c.Date.ToString("yyyy-MM-dd"),
                    c.RatePerMoverInCents
                }),
                reviews = JsonSerializer.Deserialize<resultContainer>((await client.GetAsync(new RestRequest())).Content).result.reviews/*.Concat(
                    new[]
                    {
                        new review("Fantastic service from start to finish! The crew arrived on time, were professional, and handled everything with care. They took extra care with my fragile items and made sure nothing was damaged.", "Alexander Malko"),
                        new review("These guys were amazing! I was moving from Toronto to Mississauga, and they made the entire process stress-free.", "Ann Blanchard"),
                        new review("The movers were friendly and worked quickly. They arrived and got straight to work, handling everything with care. They even helped me unpack and arrange things once we got to the new place.", "Asha Shaji"),
                        new review("I couldn't be happier with the service! The movers were efficient, professional, and took great care with all my belongings. Everything arrived in perfect condition.", "Ali Baranpourian")
                    }
                )*/
            }
        ));
        }
    }
}
