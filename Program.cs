using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Web;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.Urls.Add("http://0.0.0.0:8080");
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

app.UseCors();
app.MapGet("/deeplink", async (HttpContext context) =>
{
    var query = context.Request.Query;
    string token = query.ContainsKey("token") ? query["token"].ToString() : "defaultToken";
    string videoId = query.ContainsKey("videoId") ? query["videoId"].ToString() : "dQw4w9WgXcQ";

    string appScheme = $"youtube://www.youtube.com/watch?v={HttpUtility.UrlEncode(videoId)}";

 
    string userAgent = context.Request.Headers["User-Agent"].ToString();
    string fallbackUrl;

    if (userAgent.Contains("Android"))
    {
        fallbackUrl = "https://play.google.com/store/apps/details?id=com.google.android.youtube"; 
    }
    else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
    {
        fallbackUrl = "https://apps.apple.com/app/youtube/id544007664"; 
    }
    else
    {
        fallbackUrl = $"https://www.youtube.com/watch?v={HttpUtility.UrlEncode(videoId)}"; 
    }

    
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
