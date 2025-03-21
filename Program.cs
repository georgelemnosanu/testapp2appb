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
    string phoneNumber = query.ContainsKey("phone") ? query["phone"].ToString() : "+40729813000"; 
    string message = query.ContainsKey("message") ? query["message"].ToString() : "Salut, vezi ca merge?!"; 

    
    string appScheme = $"whatsapp://send?phone={HttpUtility.UrlEncode(phoneNumber)}&text={HttpUtility.UrlEncode(message)}";

   
    string fallbackUrl = "https://web.whatsapp.com"; 

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