using API_Encurtador_URL;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/health");

app.MapPost("/tela/urls", (ShortUrl url) =>
{
    var identifier = Guid.NewGuid().ToString("N").Substring(0, 8);
    url.Id = identifier;
    url.ShortenedUrl = $"https://identificador.re/{identifier}";
    Store.Urls.Add(identifier, url);
    return Results.Created($"/tela/urls/{identifier}", url);
})
.WithName("CreateShortURL");

app.MapGet("/tela/urls/{id}", (string id) =>
{
    if (Store.Urls.TryGetValue(id, out var url))
    {
        return Results.Ok(url);
    }
    return Results.NotFound();
})
.WithName("GetShortURL");

app.MapDelete("/tela/urls/{id}", (string id) =>
{
    if (Store.Urls.Remove(id))
    {
        return Results.Ok();
    }
    return Results.NotFound();
})
.WithName("DeleteShortURL");

app.MapPut("/tela/urls/{id}", (string id, ShortUrl updatedUrlData) =>
{
    if (Store.Urls.ContainsKey(id))
    {
        Store.Urls[id].LongUrl = updatedUrlData.LongUrl;
        Store.Urls[id].ShortenedUrl = updatedUrlData.ShortenedUrl;
        return Results.Ok(Store.Urls[id]);
    }
    else
    {
        return Results.NotFound();
    }
})
.WithName("UpdateShortURL");

app.Run();
