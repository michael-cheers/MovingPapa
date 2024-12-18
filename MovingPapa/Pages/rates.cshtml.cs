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
        public record review (string text, string author_name, string? image = null);
        public async Task<IActionResult> OnGet() {
            RestClient client = new(
                "https://maps.googleapis.com/maps/api/place/details/json?place_id=ChIJoeIpTEozK4gRRdz0QV9yJUw&key=AIzaSyAV5-u4FmaK19MYQByVruZuw6Hn4PHUUt0&fields=reviews"
            );
            var revs =
                    new[]
                    {
                        new review("kevin and joel did a great job fast and cared a lot about not damaging anything", "McFury", "/McFury.jpg"),
                        new review("Rohit and Sam were amazing with the move. Very fast and efficient. Respectful, friendly and no damages!! Recommend Moving Papa 10/10.", "Niki V", "/NikiV.jpg"),
                        new review("Kevin and Joel couldn’t have been more amazing. They were prompt and extremely professional, they handled everything with care.", "Catherine MacFadyen", "/CatherineMacFadyen.jpg"),
                        new review("Sam nd Rohit came did an amazing job! Moving papa is really a great company to deal with! From booking to offload these guys are amazingly smooth! I’m so happy I found these guys", "Eestbound Beats", "/EestboundBeats.jpg"),
                        new review("I am always hesitant when selecting a moving company but i was suggested Moving Papa a lot and oh i am very glad that i chose them. They turned a very stressful day into a breeze.", "Unique Basnet", "/UniqueBasnet.jpg"),
                        new review("Amazing job! Thanks Rohit and Abel did a great job. Highly recommend!", "Mary Rankowski", "/MaryRankowski.jpg")
                    }.Concat(JsonSerializer.Deserialize<resultContainer>((await client.GetAsync(new RestRequest())).Content).result.reviews.Where(r => r.text.Length <= 400))
                    .DistinctBy(r => r.author_name);
            revs = revs.Skip(2).Concat(revs.Take(2));
            return Content(JsonSerializer.Serialize(
            new
            {
                rates = DB.RateCalendars.Select(c => new
                {
                    Date = c.Date.ToString("yyyy-MM-dd"),
                    c.RatePerMoverInCents
                }),
                reviews = revs.Concat(revs)

                //.Concat(
            }
        ));
        }
    }
}
