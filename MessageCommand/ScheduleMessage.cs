using System;
using System.Threading.Tasks;
using TL;
using WTelegram;

public class ScheduleMessage
{
    public static async Task SendScheduleMessage(Client client)
    {
        try
        {
            Console.WriteLine("Введи ник, кому необходимо отправить сообщение:");
            string? username = Console.ReadLine();
            var resolved = await client.Contacts_ResolveUsername(username);

            Console.Write("Введите через сколько минут отправить сообщение: ");
            if (!int.TryParse(Console.ReadLine(), out int minutes))
            {
                throw new Exception("Некорректный ввод времени.");
            }

            DateTime when = DateTime.UtcNow.AddMinutes(minutes);
            Console.WriteLine("Введите текст, который хотите отправить:");
            string? messagetext = Console.ReadLine();

            await client.SendMessageAsync(resolved, messagetext, schedule_date: when);
            Console.WriteLine("Сообщение запланировано на: " + when);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    public static async Task SomeSendScheduleMessage(Client client)
    {
        try
        {
            Console.WriteLine("Введи ник, кому необходимо отправить сообщение:");
            string? username = Console.ReadLine();
            var resolved = await client.Contacts_ResolveUsername(username);

            Console.Write("Введите интервал сообщений (в часах или днях):\n1 - Часы\n2 - Дни\nВыбор: ");
            int timeUnit = Convert.ToInt32(Console.ReadLine());

            Console.Write("Введите интервал сообщений: ");
            if (!int.TryParse(Console.ReadLine(), out int interval))
                throw new Exception("Некорректный ввод интервала.");

            // Преобразование интервала в минуты
            int intervalInMinutes = timeUnit == 1 ? interval : interval * 1440; // 1 день = 1440 минут

            Console.Write("Введите текст, который хотите отправить: ");
            string? messagetext = Console.ReadLine();

            Console.Write("Введите сколько раз вы хотите отправить сообщение: ");
            if (!int.TryParse(Console.ReadLine(), out int howmany))
                throw new Exception("Некорректный ввод количества.");

            // Ввод времени для первого сообщения в формате "часы:минуты"
            Console.Write("Введите через сколько отправить первое сообщение (в формате ЧЧ:ММ): ");
            string[]? timeParts = Console.ReadLine()?.Split(':');

            if (timeParts.Length != 2 || 
                !int.TryParse(timeParts[0], out int hours) || 
                !int.TryParse(timeParts[1], out int minutes))
            {
                throw new Exception("Некорректный ввод времени.");
            }

            DateTime when = DateTime.UtcNow.AddHours(hours).AddMinutes(minutes);

            for (int i = 0; i < howmany; i++)
            {
                await client.SendMessageAsync(resolved, messagetext, schedule_date: when);
                Console.WriteLine($"Сообщение {i + 1} запланировано на: {when}");
                when = when.AddMinutes(intervalInMinutes);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }
}