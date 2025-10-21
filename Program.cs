using TelegramBotNotification.Services;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// API endpoints
app.MapGet("/", () => Results.Redirect("/index.html"));

app.MapPost("/api/init", async () =>
{
    var service = TelegramService.Instance;
    _ = Task.Run(async () => await service.InitializeAsync());
    await Task.Delay(1000); // Give it time to start
    
    if (service.IsWaitingForCode())
    {
        return Results.Ok(new { message = "Waiting for verification code", needsCode = true });
    }
    
    return Results.Ok(new { message = "Initializing...", needsCode = false });
});

app.MapGet("/api/check-status", () =>
{
    var service = TelegramService.Instance;
    return Results.Ok(new { 
        waitingForCode = service.IsWaitingForCode(),
        waitingForPassword = service.IsWaitingForPassword(),
        isInitialized = service.IsInitialized(),
        loginResult = service.GetLoginResult()
    });
});

app.MapPost("/api/verify-code", (VerifyCodeRequest request) =>
{
    var service = TelegramService.Instance;
    service.SetVerificationCode(request.Code);
    return Results.Ok(new { message = "Code submitted" });
});

app.MapPost("/api/verify-password", (VerifyPasswordRequest request) =>
{
    var service = TelegramService.Instance;
    service.SetPassword(request.Password);
    return Results.Ok(new { message = "Password submitted" });
});

app.MapPost("/api/send-message", async (MessageRequest request) =>
{
    var service = TelegramService.Instance;
    var result = await service.SendMessageAsync(request.Username, request.MessageText);
    return Results.Ok(new { message = result });
});

app.MapPost("/api/send-multiple", async (MultipleMessageRequest request) =>
{
    var service = TelegramService.Instance;
    var result = await service.SendMultipleMessagesAsync(request.Username, request.MessageText, request.Count);
    return Results.Ok(new { message = result });
});

app.MapPost("/api/send-scheduled", async (ScheduledMessageRequest request) =>
{
    var service = TelegramService.Instance;
    var result = await service.SendScheduledMessageAsync(request.Username, request.MessageText, request.Minutes);
    return Results.Ok(new { message = result });
});

app.MapPost("/api/send-multiple-scheduled", async (MultipleScheduledMessageRequest request) =>
{
    var service = TelegramService.Instance;
    var result = await service.SendMultipleScheduledMessagesAsync(
        request.Username, 
        request.MessageText, 
        request.Count, 
        request.IntervalMinutes, 
        request.DelayHours, 
        request.DelayMinutes);
    return Results.Ok(new { message = result });
});

app.Run();

// Request models
record MessageRequest(string Username, string MessageText);
record MultipleMessageRequest(string Username, string MessageText, int Count);
record ScheduledMessageRequest(string Username, string MessageText, int Minutes);
record MultipleScheduledMessageRequest(string Username, string MessageText, int Count, int IntervalMinutes, int DelayHours, int DelayMinutes);
record VerifyCodeRequest(string Code);
record VerifyPasswordRequest(string Password);
