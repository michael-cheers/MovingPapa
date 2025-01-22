using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MovingPapa.DB;
using System.Net;
using System.Text.Json;

namespace MovingPapa.Pages
{
    public class request_callModel : PageModel
    {
        readonly MovingpapaContext DB;
        public request_callModel(MovingpapaContext db) => DB = db;

        public async Task<IActionResult> OnGet(string uuid)
        {
            var q = await DB.QuotesAndContacts.SingleAsync/**/(q => q.Uuid == uuid);
            EmailService emailService = new();
            string[] addrs = JsonSerializer.Deserialize<string[]>(q.Addresses);
            await emailService.SendMessage(
                "sales@movingpapa.com",
                "CALL LEAD",
                "<ul>" + string.Concat($"Date: {q.MoveDate}\r\nFrom: {addrs[0]}\r\nTo: {addrs[1]}\r\nName:{q.FullName}\r\nPhone:{q.PhoneNumber}\r\nEmail:{q.Email}\r\nMove Info: {q.MoveInfo}"
                    .Split("\r\n").Select(k => "<li>" + WebUtility.HtmlEncode(k) + "</li>")) + "</ul>"
            );
            return this.StatusCode(200);
        }
    }
}
