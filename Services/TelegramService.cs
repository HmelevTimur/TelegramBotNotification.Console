using WTelegram;
using TL;

namespace TelegramBotNotification.Services;

public class TelegramService
{
    private Client? _client;
    private static TelegramService? _instance;
    private static readonly object _lock = new object();
    private TaskCompletionSource<string>? _verificationCodeTask;
    private TaskCompletionSource<string>? _passwordTask;
    private bool _waitingForCode = false;
    private bool _waitingForPassword = false;
    private bool _isInitialized = false;
    private string? _loginResult = null;

    private TelegramService() { }

    public static TelegramService Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TelegramService();
                    }
                }
            }
            return _instance;
        }
    }

    public async Task<string> InitializeAsync()
    {
        try
        {
            // Config function for WTelegram
            string Config(string what)
            {
                switch (what)
                {
                    case "api_id": return Environment.GetEnvironmentVariable("TELEGRAM_API_ID") ?? "";
                    case "api_hash": return Environment.GetEnvironmentVariable("TELEGRAM_API_HASH") ?? "";
                    case "phone_number": return Environment.GetEnvironmentVariable("TELEGRAM_PHONE") ?? "";
                    case "verification_code":
                        _waitingForCode = true;
                        _verificationCodeTask = new TaskCompletionSource<string>();
                        
                        // Wait for code from web interface with timeout
                        var codeTask = _verificationCodeTask.Task;
                        var timeoutTask = Task.Delay(TimeSpan.FromMinutes(5));
                        var completedTask = Task.WhenAny(codeTask, timeoutTask).Result;
                        
                        _waitingForCode = false;
                        
                        if (completedTask == codeTask)
                        {
                            return codeTask.Result;
                        }
                        return ""; // Timeout
                    case "password":
                        _waitingForPassword = true;
                        _passwordTask = new TaskCompletionSource<string>();
                        
                        // Wait for password from web interface with timeout
                        var passwordTaskResult = _passwordTask.Task;
                        var passwordTimeoutTask = Task.Delay(TimeSpan.FromMinutes(5));
                        var passwordCompletedTask = Task.WhenAny(passwordTaskResult, passwordTimeoutTask).Result;
                        
                        _waitingForPassword = false;
                        
                        if (passwordCompletedTask == passwordTaskResult)
                        {
                            return passwordTaskResult.Result;
                        }
                        return ""; // Timeout
                    case "session_pathname":
                        // Create data directory if it doesn't exist
                        var dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                        Directory.CreateDirectory(dataDir);
                        return Path.Combine(dataDir, "WTelegram.session");
                    default: return null;
                }
            }

            _client = new Client(Config);
            var myself = await _client.LoginUserIfNeeded();
            _loginResult = $"Logged in as {myself.first_name} {myself.last_name} (id: {myself.id})";
            _isInitialized = true;
            return _loginResult;
        }
        catch (Exception ex)
        {
            var errorMsg = ex.Message;
            
            // Provide user-friendly error messages
            if (errorMsg.Contains("PHONE_PASSWORD_FLOOD"))
            {
                _loginResult = "Error: Слишком много попыток ввода пароля. Подождите несколько часов и попробуйте снова.";
            }
            else if (errorMsg.Contains("PASSWORD_HASH_INVALID"))
            {
                _loginResult = "Error: Неверный пароль. Попробуйте ещё раз.";
            }
            else if (errorMsg.Contains("FLOOD_WAIT"))
            {
                _loginResult = "Error: Слишком много запросов. Подождите некоторое время и попробуйте снова.";
            }
            else if (errorMsg.Contains("PHONE_CODE_INVALID"))
            {
                _loginResult = "Error: Неверный код верификации.";
            }
            else
            {
                _loginResult = $"Error: {errorMsg}";
            }
            
            return _loginResult;
        }
    }

    public bool IsWaitingForCode() => _waitingForCode;
    
    public bool IsWaitingForPassword() => _waitingForPassword;
    
    public bool IsInitialized() => _isInitialized;
    
    public string? GetLoginResult() => _loginResult;

    public void SetVerificationCode(string code)
    {
        _verificationCodeTask?.TrySetResult(code);
    }
    
    public void SetPassword(string password)
    {
        _passwordTask?.TrySetResult(password);
    }

    public async Task<string> SendMessageAsync(string username, string messageText)
    {
        if (_client == null)
            return "Client not initialized";

        try
        {
            var resolved = await _client.Contacts_ResolveUsername(username);
            await _client.SendMessageAsync(resolved, messageText);
            return "Message sent successfully";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public async Task<string> SendMultipleMessagesAsync(string username, string messageText, int count)
    {
        if (_client == null)
            return "Client not initialized";

        try
        {
            var resolved = await _client.Contacts_ResolveUsername(username);
            for (int i = 0; i < count; i++)
            {
                await _client.SendMessageAsync(resolved, messageText);
            }
            return $"Sent {count} messages successfully";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public async Task<string> SendScheduledMessageAsync(string username, string messageText, int minutes)
    {
        if (_client == null)
            return "Client not initialized";

        try
        {
            var resolved = await _client.Contacts_ResolveUsername(username);
            DateTime when = DateTime.UtcNow.AddMinutes(minutes);
            await _client.SendMessageAsync(resolved, messageText, schedule_date: when);
            return $"Message scheduled for: {when:yyyy-MM-dd HH:mm:ss} UTC";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    public async Task<string> SendMultipleScheduledMessagesAsync(
        string username, 
        string messageText, 
        int count, 
        int intervalMinutes, 
        int delayHours, 
        int delayMinutes)
    {
        if (_client == null)
            return "Client not initialized";

        try
        {
            var resolved = await _client.Contacts_ResolveUsername(username);
            DateTime when = DateTime.UtcNow.AddHours(delayHours).AddMinutes(delayMinutes);
            
            for (int i = 0; i < count; i++)
            {
                await _client.SendMessageAsync(resolved, messageText, schedule_date: when);
                when = when.AddMinutes(intervalMinutes);
            }
            
            return $"Scheduled {count} messages successfully";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }
}
