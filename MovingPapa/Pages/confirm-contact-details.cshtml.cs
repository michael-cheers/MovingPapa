using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MovingPapa.DB;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Cms;
using RestSharp;
using RingClient = RingCentral.RestClient;
using RestClient = RestSharp.RestClient;
using System.Net;
using System.Text.Json;
using RingCentral;

namespace MovingPapa.Pages
{
    public class confirm_contact_detailsModel : PageModel
    {
        readonly MovingpapaContext DB;
        public confirm_contact_detailsModel (MovingpapaContext db) => DB = db;
        public async Task<IActionResult> OnGet(string fullName, string email, string phoneNumber, string service, string points, string moveDate, string moveTime, bool isCallNow, string? uuid = null)
        {
            uuid ??= Guid.NewGuid().ToString();
            if (!DateTime.TryParse(moveDate, out DateTime dt))
                dt = default;
            //Enum.Parse<MoveTime>(moveTime.Replace(" ", ""));
            await DB.QuotesAndContacts.AddAsync(new()
            {
                FullName = fullName,
                Email = email,
                PhoneNumber = phoneNumber,
                Uuid = uuid,
                IsMuscleOnly = service == "Muscle only",
                Addresses = points,
                IsCallNow = isCallNow,
                MoveDate = dt == default ? null : dt,
                MoveTime = "Early Morning" //moveTime.ToString()
            });
            await DB.SaveChangesAsync();
            var pointsDecoded = JsonSerializer.Deserialize<List<string>>(points);
            var client = new RestClient($"https://api.smartmoving.com/api/leads/from-provider/v2?providerKey={Environment.GetEnvironmentVariable("SM_API_KEY")}");
            await client.PostAsync(new RestRequest()
                .AddJsonBody(new
                {
                    FullName = fullName,
                    PhoneNumber = phoneNumber,
                    Email = email,
                    MoveDate = dt == default ? "" : dt.ToString("yyyyMMdd"),
                    OriginAddressFull = pointsDecoded[0],
                    DestinationAddressFull = pointsDecoded[^1]
                })
            );
            EmailService emailService = new();
            await emailService.SendMessage(
                "sales@movingpapa.com",
                "NEW LEAD",
                "<ul>" + string.Concat($"Date: {moveDate}\r\nFrom: {pointsDecoded[0]}\r\nTo: {pointsDecoded[1]}\r\nName:{fullName}\r\nPhone:{phoneNumber}\r\nEmail:{email}\r\n"
                    .Split("\r\n").Select(k => "<li>" + WebUtility.HtmlEncode(k) + "</li>")) + "</ul>"
            );
//            await emailService.SendMessage(
//                email,
//                "We’ve Received Your Moving Request! 📦",
//                @$"Hi {fullName},<br><br>
//Thank you for choosing Moving Papa! We’ve received your moving details and will be giving you a call shortly to provide your personalized quote.<br><br>
//<b>Move Details:</b>
//<ul><li>Date: {DateTime.Parse(moveDate).ToString("MMMM d yyyy")}</li><li>From: {pointsDecoded[0]}</li><li>To: {pointsDecoded[1]}</li></ul>
//Expect a call from us soon at (647) 670-2576. If you need anything in the meantime, feel free to reply to this email!<br><br>
//Looking forward to helping you move with confidence!<br><br>
//Best,<br>
//The Moving Papa Team"
//            );
            await send_sms(
                "+16476702576",
                phoneNumber,
                $"Hi {fullName}! This is Moving Papa. We’ve received your moving request and will be calling you soon with your quote. Talk to you soon! 📞"
            );
            return Content(uuid);
        }

        static private async Task send_sms(string fromNumber, string toNumber, string message)
        {
            RingClient restClient = new (
                Environment.GetEnvironmentVariable("RC_APP_CLIENT_ID"),
                Environment.GetEnvironmentVariable("RC_APP_CLIENT_SECRET"),
                "https://platform.ringcentral.com");
            // Authenticate a user using a personal JWT token
            await restClient.Authorize(Environment.GetEnvironmentVariable("RC_USER_JWT"));

            try
            {
                var requestBody = new CreateSMSMessage();
                requestBody.from = new MessageStoreCallerInfoRequest
                {
                    phoneNumber = fromNumber
                };
                requestBody.to = new [] {
                    new MessageStoreCallerInfoRequest { phoneNumber = toNumber }
                };
                // To send group messaging, add more (max 10 recipients) 'phoneNumber' object. E.g.
                /*
                requestBody.to = new MessageStoreCallerInfoRequest[] {
                  new MessageStoreCallerInfoRequest { phoneNumber = "Recipient_1_Number" },
                  new MessageStoreCallerInfoRequest { phoneNumber = "Recipient_2_Number" }
                };
                */
                requestBody.text = message;

                var resp = await restClient.Restapi().Account().Extension().Sms().Post(requestBody);
                //Console.WriteLine("SMS sent. Message id: " + resp.id.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

    }
}
