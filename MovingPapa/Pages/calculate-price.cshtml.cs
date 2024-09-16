using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovingPapa.DB;
using RestSharp;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MovingPapa.Pages
{
    public enum ParkingType { LoadingDock, Driveway, Street, Other }
    public enum WalkingDistance { _14Min, _510Min, _10MinPlus }
    [Flags]
    public enum Access { Direct, Stairs, Elevator }
    public enum BuildingType { House, Apartment, Townhouse, Studio }
    public record Point (string address, BuildingType? buildingType, ParkingType parkingType, WalkingDistance walkingDistanceInMin, Access access);
    public enum MoveTime { EarlyMorning, Afternoon, LateAfternoon, Evening }
    public enum MoveType { FullMove, PartialMove, _5ItemsOrLess, Commercial }
    public record Item (string item, int quantity);
    public enum Room { Bedroom, LivingRoom, DiningRoom, Kitchen, Bathroom, Office, Garage, Basement, StorageLocker, Backyard, Other }
    public record RoomDetails (Room room, Item[] items);
    public record MoveDetails (Point[] points, string moveDate, MoveTime moveTime, MoveType moveType, bool needsPackingHelp, List<RoomDetails> rooms);
    public record GoogleRoute (int distanceMeters, string duration);
    public record RoutesResponse (GoogleRoute[] routes);

    public class calculate_priceModel : PageModel
    {
        readonly MovingpapaContext DB;
        public calculate_priceModel(MovingpapaContext db) => DB = db;
        public static (int m, decimal secs) GetRouteDetails(string[] addresses, DateTime travelTime)
        {
            RestClient client = new("https://routes.googleapis.com/directions/v2:computeRoutes");
            RestRequest request = new();

            request.AddHeader("Origin", "https://routes.googleapis.com");
            request.AddHeader("Referer", "https://routes.googleapis.com");
            // Headers
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-Goog-Api-Key", Environment.GetEnvironmentVariable("GOOGLE_API_KEY"));
            request.AddHeader("X-Goog-FieldMask", "routes.duration,routes.distanceMeters,routes.polyline.encodedPolyline");

            // Body
            var body = new
            {
                origin = new { address = addresses[0] },
                intermediates = addresses.Skip(1).Take(addresses.Length - 2).Select(a => new { address = a }).ToArray(),
                destination = new { address = addresses[^1] },
                travelMode = "DRIVE",
                routingPreference = "TRAFFIC_AWARE",
                computeAlternativeRoutes = false,
                routeModifiers = new
                {
                    avoidTolls = false,
                    avoidHighways = false,
                    avoidFerries = false
                },
                languageCode = "en-US",
                units = "METRIC",
                departureTime = travelTime.ToString(@"yyyy-M-d\TH:mm:ss\Z")
            };

            request.AddJsonBody(body);
            RoutesResponse response = client.Post<RoutesResponse>(request);
            GoogleRoute route = response.routes.First();
            return (route.distanceMeters, decimal.Parse(route.duration[..^1]));
        }

        public const string officeAddr = "7 Spadina Rd, Toronto, ON M5R 2S7, Canada";

        public async Task<IActionResult> OnGet(string moveDetails, string? uuid = null)
        {
            var q = await DB.QuotesAndContacts.SingleAsync/**/(q => q.Uuid == uuid);
            q.MoveInfo = moveDetails;
            MoveDetails moveDetailsParsed = JsonSerializer.Deserialize<MoveDetails>(moveDetails);
            var dt = TimeZoneInfo.ConvertTimeToUtc(DateTime.Parse(moveDetailsParsed.moveDate, CultureInfo.InvariantCulture)
                .AddHours(moveDetailsParsed.moveTime switch
                {
                    MoveTime.EarlyMorning => 8,
                    MoveTime.Afternoon => 12,
                    MoveTime.LateAfternoon => 16,
                    MoveTime.Evening => 20
                }), TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time"));
            (int m, decimal secs) = GetRouteDetails(
                moveDetailsParsed.points.Select(p => p.address).Prepend(officeAddr).Append(officeAddr).ToArray(),
                dt
            );
            int numTotalBedrooms = moveDetailsParsed.rooms.Count(r => r.room == Room.Bedroom);
            int numRelevantBedrooms = moveDetailsParsed.rooms.Count(r => r.items.Length > 0 && r.room == Room.Bedroom);
            int numRelevantExtraRooms = moveDetailsParsed.rooms.Count(r => r.items.Length > 0 &&
                r.room is Room.Backyard or Room.Garage or Room.Basement or Room.StorageLocker or Room.Other);
            int movers = (numRelevantBedrooms, moveDetailsParsed.points[0].buildingType) switch
            {
                ( 3, buildingType: BuildingType.Apartment or BuildingType.Studio) => numRelevantBedrooms,
                ( >= 4, _) => 4 + (numRelevantBedrooms - 3) / 2,
                var p => numRelevantBedrooms + 1
            } + (numRelevantExtraRooms > 0 ? 1 : 0);
            decimal dayRate = ((await DB.RateCalendars.SingleOrDefaultAsync(r => r.Date == DateTime.Parse(moveDetailsParsed.moveDate, CultureInfo.InvariantCulture)))?.RatePerMoverInCents ?? 6000) / 100m;
            decimal pricePerHour = movers * dayRate;
            int numBoxes = moveDetailsParsed.rooms.Sum(r => r.items.Where(i => i.item == "Boxes").Sum(i => i.quantity));
            decimal hours = ((decimal)secs / 3600)
                + moveDetailsParsed.points[0].buildingType
                switch
                {
                    BuildingType.House => 5m,
                    BuildingType.Apartment => 10m
                } * ((numBoxes - 1) / 3) / movers
                + moveDetailsParsed.rooms.Sum(r => r.room switch
                {
                    Room.LivingRoom => numTotalBedrooms switch
                    {
                        1 or 2 => 75m / 60,
                        3 or 4 => 90m / 60
                    } * r.items.Sum(i => i.quantity * i.item switch
                    {
                        "Couch/Sofa" => 200,
                        "Coffee table" => 50,
                        "TV stand" => 50,
                        "TV" => 30,
                        "Bookshelf" => 75,
                        "Rug" => 50,
                        "Side table" => 25,
                        "Plant" => 25,
                        "Artwork" => 5,
                        "Extras" => 25,
                        "Boxes" => 0
                    }) / 535m * 1.05m,
                    Room.Bedroom => (moveDetailsParsed.needsPackingHelp ? 0.5m : 0m) + numTotalBedrooms switch { 1 or 2 => 1m, 3 or 4 => 1.25m } *
                        r.items.Sum(i => i.quantity * i.item switch
                        {
                            "Bed frame" => 75,
                            "Mattress" => 75,
                            "Dresser" => 150,
                            "Nightstand" => 30,
                            "Wardrobe" => 150,
                            "TV" => 60,
                            "Lamp" => 5,
                            "Rug" => 50,
                            "Extras" => 25,
                            "Boxes" => 0,
                            _ => 0
                        }) / 620 * 1.05m,
                    Room.Kitchen => movers switch
                    {
                        2 => 0.5m,
                        >=3 => 20m / 60
                    } * r.items.Sum(i => i.quantity * i.item switch
                    {
                        "Refrigerator" => 200,
                        "Freezer" => 75,
                        "Washer/dryer" => 100,
                        "Extras" => 25,
                        "Boxes" => 0
                    }) / 375m * 1.05m,
                    Room.DiningRoom => (moveDetailsParsed.needsPackingHelp ? 0.5m : 20m / 60) * r.items.Sum(i => i.quantity * i.item switch
                    {
                        "Cabinet" => 150,
                        "Dining table" => 120,
                        "Chairs" => 10,
                        "Extras" => 25
                    }) / 310m * 1.05m,
                    Room.Office => (40m / 60) * r.items.Sum(i => i.quantity * i.item switch
                    {
                        "Desk" => 75,
                        "Chair" => 25,
                        "Bookshelf" => 50,
                        "Filing cabinet" => 50,
                        "Monitors" => 15,
                        "Extras" => 25
                    }) / 215m * 1.05m,
                    _ => r.items.Sum(i => i.quantity * i.item switch
                    {
                        "Patio" => 15m / 60,
                        "Furniture" => 15m / 60,
                        "Barbecue" => 5m / 60,
                        "Exercise equipment" => 15m / 60,
                        "Bicycles" => 5m / 60,
                        "Tires" => 5m / 60,
                        "Large toys" => 5m / 60
                    })
                });
            decimal km = m / 1000;
            decimal price = Math.Round(hours * pricePerHour + km * 0.96m, 2);
            var packages = new[]
            {
                new
                {
                    price,
                    time = hours
                },
                new
                {
                    price = Math.Round(price * 1.5m, 2),
                    time = hours * 1.5m
                }
            };
            q.Packages = JsonSerializer.Serialize(packages);
            q.TimeUpdated = DateTime.UtcNow;
            await DB.SaveChangesAsync();
            return new JsonResult(packages);
        }
    }
}
