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
        public record accesstokenobj (string access_token);
        public async Task<IActionResult> OnGet() {
            RestClient tokenClient = new(
                "https://oauth2.googleapis.com/token"
            );
            accesstokenobj aobj = await tokenClient.PostAsync<accesstokenobj>(new RestRequest()
                .AddParameter("refresh_token", "1//04IFbjCQ3qNd0CgYIARAAGAQSNwF-L9IrcDzisUxkA8tLvqlf-rumeU4vzAjALgGvowBdIjyc1YS6Jp7LlR78Gg9Kud2Na1yIRP8")
                //.AddParameter("code", "4/0AVG7fiSOEFUxEprAcH0RPB0uUhnq9GdAb0etGqbZ9AxwabLXN0DbzDiJM0Mj9ZEGjUkwvw")
                .AddParameter("code", "4/0AVG7fiSOEFUxEprAcH0RPB0uUhnq9GdAb0etGqbZ9AxwabLXN0DbzDiJM0Mj9ZEGjUkwvw")
                .AddParameter("client_id", "870777325668-sas4ukf4ge3cdnfojd79olatua783354.apps.googleusercontent.com")
                .AddParameter("client_secret", "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")
                //.AddParameter("redirect_uri", "https://localhost/")
                .AddParameter("grant_type", "authorization_code")
            );
            RestClient mybizClient = new(
                "https://mybusinessaccountmanagement.googleapis.com/v1/accounts"
            );

            /*(async () => {
    let req = await fetch('https://oauth2.googleapis.com/token', {method: 'POST',    headers:{
      'Content-Type': 'application/x-www-form-urlencoded'
    },    

        body: new URLSearchParams({"code": "4/0AVG7fiSOEFUxEprAcH0RPB0uUhnq9GdAb0etGqbZ9AxwabLXN0DbzDiJM0Mj9ZEGjUkwvw",
"client_id": "870777325668-sas4ukf4ge3cdnfojd79olatua783354.apps.googleusercontent.com",
"client_secret": "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",
"redirect_uri": "https://localhost/",
"grant_type": "authorization_code"})
});
    console.log(await req.json()); 
})()
             {
    "access_token": "ya29.a0AeDClZC6ByE4hYdr6-KiboJo2mTSLFUNj8W7Y8_rF_qy_2Rxi1F_jkuBsZMVglzyMQQxtCh297OecasqaAFsXZoY2_UHS72BAozjt2t6YjEt9bTUKmaME_e-SLJs_LbTd-XWtKs_iBl9o6_Weep6u3ZgYJ15Q0QBUB3ku_ZVaCgYKAVUSARMSFQHGX2MibKaKUfCXjSHCen12JoEKSA0175",
    "expires_in": 3599,
    "refresh_token": "1//04IFbjCQ3qNd0CgYIARAAGAQSNwF-L9IrcDzisUxkA8tLvqlf-rumeU4vzAjALgGvowBdIjyc1YS6Jp7LlR78Gg9Kud2Na1yIRP8",
    "scope": "https://www.googleapis.com/auth/business.manage",
    "token_type": "Bearer"
} */
            RestClient client = new(
                "https://maps.googleapis.com/maps/api/place/details/json?place_id=ChIJoeIpTEozK4gRRdz0QV9yJUw&key=AIzaSyAV5-u4FmaK19MYQByVruZuw6Hn4PHUUt0&fields=reviews"
            );
            var revs = JsonSerializer.Deserialize<resultContainer>((await client.GetAsync(new RestRequest())).Content).result.reviews.Where(r => r.text.Length <= 400);
            return Content(JsonSerializer.Serialize(
            new
            {
                rates = DB.RateCalendars.Select(c => new
                {
                    Date = c.Date.ToString("yyyy-MM-dd"),
                    c.RatePerMoverInCents
                }),
                reviews = revs.Concat(revs)
                    
                    /*.Concat(
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
