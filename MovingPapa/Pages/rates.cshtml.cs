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
            //RestClient client = new(
            //    "https://maps.googleapis.com/maps/api/place/details/json?place_id=ChIJoeIpTEozK4gRRdz0QV9yJUw&key=AIzaSyAV5-u4FmaK19MYQByVruZuw6Hn4PHUUt0&fields=reviews"
            //);
            IEnumerable<review> revs =
                    new[]
                    {
                        new review("kevin and joel did a great job fast and cared a lot about not damaging anything", "McFury", "/McFury.jpg"),
                        new review("Rohit and Sam were amazing with the move. Very fast and efficient. Respectful, friendly and no damages!! Recommend Moving Papa 10/10.", "Niki V", "/NikiV.jpg"),
    new review("Moving Papa was so amazing, helpful, made everything from first contact till the job was done super easy! If there was 6 stars I give them all 6 🫶🏼", "Lisa North", "/Lisa.jpg"),
                        new review("Kevin and Joel couldn’t have been more amazing. They were prompt and extremely professional, they handled everything with care.", "Catherine MacFadyen", "/CatherineMacFadyen.jpg"),
    new review("Awesome Job! Super efficient, friendly and considerate. When I need great moving like this I say \"come to Papa - Moving Papa\"", "Mike Rewucki", "/Mike.jpg"),
                        //new review("Sam nd Rohit came did an amazing job! Moving papa is really a great company to deal with! From booking to offload these guys are amazingly smooth! I’m so happy I found these guys", "Eestbound Beats", "/EestboundBeats.jpg"),

                        new review("I am always hesitant when selecting a moving company but i was suggested Moving Papa a lot and oh i am very glad that i chose them.", "Unique Basnet", "/UniqueBasnet.jpg"),
                        //new review("Amazing job! Thanks Rohit and Abel did a great job. Highly recommend!", "Mary Rankowski", "/MaryRankowski.jpg"),
    // New reviews
    new review("We highly recommend moving papa crew, Kevin and Joel was super fast and extremely careful and we had a lovely time with them.👍", "Blake Oates", "/Blake.jpg"),
    new review("Rohit and Joel did a great job I highly recommend them for moving, they make my moving pretty easy and simple", "Bob Byakuleka", "/Bob.jpg"),
    new review("Sheldon & Kevin came to the rescue!! I will say, regardless of any ups and down. Moving papa came through.", "Cara Dorion", "/Cara.jpg"),
    new review("Kevin and Joel at moving papa made this the smoothest move ever for me and my wife. Record time. Cannot believe how smooth this was.", "Carl Casis", "/Carl.jpg"),
    new review("Excellent service, Joel and Abel! Very satisfied!", "Hugo Blanco", "/Hugo.jpg"),
    new review("Great experience with Kevin & Joel today for our office move. Thanks guys!", "Jared Kwart", "/Jared.jpg"),
    new review("Luis and Abel were fantastic! The move was efficient and careful. Thank you!", "Josslyn J", "/Jocelyn.jpg"),
    new review("Rohit nitish nd sam the amazing job moving us in rainy weather. I would definitely recomend thus tram. Moving papa was a great experience", "Kamal Bawa", "/Kamal.jpg"),
    new review("Highly recommend! These guys made my move so smooth, even with the unexpected challenge of no elevator access from the back.", "Karen Padilla", "/Karen.jpg"),
    new review("Rohit and Exon came by and efficiently moved my poorly packed apartment to another and were really good about it! They were hard working and friendly.", "Lachlan Bleackley", "/Lachlan.jpg"),
    new review("Joel & Kevin were very speedy yet careful movers! Their help made our move so much easier and they were a pleasure to have around for the morning!", "Melody Rudyk", "/Melody.jpg"),
    new review("Kevin, Joel and Gurman were great! Made my move easy and quick. Pleasure to work with.", "Nancy Marques", "/Nancy.jpg"),
    new review("Kevin and Joel did a great job! Recommend!", "Samuel Needham", "/Samuel.jpg"),
    new review("Luis and Able and Adrian did. Great job moving my place. Highly recommended!", "Sulaiman Mangal", "/Sulaiman.jpg")
                    };//}.Concat(JsonSerializer.Deserialize<resultContainer>((await client.GetAsync(new RestRequest())).Content).result.reviews.Where(r => r.text.Length <= 400))
                    //.DistinctBy(r => r.author_name);
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
