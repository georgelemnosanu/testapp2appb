using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();
app.Urls.Add("http://0.0.0.0:8080");


app.UseCors();

app.MapGet("/deeplink", async (HttpContext context) =>
{
    Console.WriteLine("Cerere primită la endpoint-ul /deeplink");

    var query = context.Request.Query;
    string token = query.ContainsKey("token") ? query["token"].ToString() : "defaultToken";
    string videoId = query.ContainsKey("videoId") ? query["videoId"].ToString() : "dQw4w9WgXcQ";
    string appScheme = $"youtube://www.youtube.com/watch?v={HttpUtility.UrlEncode(videoId)}";

    string userAgent = context.Request.Headers["User-Agent"].ToString();
    string fallbackUrl = $"NU E NIMIC!";

    string html = $@"
    <html>
        <head>
            <meta http-equiv='refresh' content='0;url={appScheme}' />
        </head>
        <body>
            <script>
                setTimeout(function() {{
                    window.location.href = '{fallbackUrl}';
                }}, 2000);
            </script>
        </body>
    </html>";
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(html);
});

app.Run();