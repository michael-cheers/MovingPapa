using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovingPapa.DB;
using RestSharp;
using System.Globalization;
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
            int numRelevantBedrooms = moveDetailsParsed.rooms.Count(r => r.items.Length > 0 && r.room == Room.Bedroom);
            int numRelevantExtraRooms = moveDetailsParsed.rooms.Count(r => r.items.Length > 0 &&
                r.room is Room.Backyard or Room.Garage or Room.Basement or Room.StorageLocker or Room.Other);
            int movers = (numRelevantBedrooms, moveDetailsParsed.points[0].buildingType) switch
            {
                ( >=3, buildingType: BuildingType.Apartment or BuildingType.Studio ) p => numRelevantBedrooms,
                var p => numRelevantBedrooms + 1
            } + (numRelevantExtraRooms > 0 ? 1 : 0);
            decimal pricePerHour = movers * 40;
            decimal hours = ((decimal)secs / 3600)
                + (numRelevantBedrooms + numRelevantExtraRooms) * (moveDetailsParsed.needsPackingHelp ? 1.5m : 1);
            decimal km = m / 1000;
            decimal price = Math.Round(hours * pricePerHour + km * 1, 2);
            q.PriceInCents = (int)(price * 100);
            q.TimeUpdated = DateTime.UtcNow;
            await DB.SaveChangesAsync();
            return new JsonResult(new[]
            {
                new
                {
                    price,
                    time = hours
                },
                new
                {
                    price = price * 1.5m,
                    time = hours * 1.5m
                }
            });
        }
    }
}
