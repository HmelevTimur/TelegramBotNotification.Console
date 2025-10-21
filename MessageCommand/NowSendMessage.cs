using System;
using System.Threading.Tasks;
using TL;
using WTelegram;

public class NowSendMessage
{
    public static async Task SendMessage(Client client) // Отправить сообщение
    {
        try
        {
            Console.WriteLine("Введи ник, кому необходимо отправить сообщение:");
            string? username = Console.ReadLine();
            var resolved = await client.Contacts_ResolveUsername(username); // никнейм без @

            Console.WriteLine("Введите текст, который хотите отправить:");
            string? messagetext = Console.ReadLine();

            await client.SendMessageAsync(resolved, messagetext);
            Console.WriteLine("Сообщение успешно отправлено.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке сообщения: {ex.Message}");
        }
    }

    public static async Task SomeSendMessage(Client client) // Отправить несколько сообщений
    {
        try
        {
            Console.WriteLine("Введи ник, кому необходимо отправить сообщение:");
            string? username = Console.ReadLine();
            var resolved = await client.Contacts_ResolveUsername(username); // никнейм без @

            Console.WriteLine("Введите текст, который хотите отправить:");
            string? messagetext = Console.ReadLine();

            Console.WriteLine("Введите сколько раз вы хотите отправить сообщение:");
            if (!int.TryParse(Console.ReadLine(), out int howmany))
            {
                throw new Exception("Некорректный ввод количества сообщений.");
            }

            for (int i = 0; i < howmany; i++)
            {
                await client.SendMessageAsync(resolved, messagetext);
                Console.WriteLine($"Сообщение {i + 1} успешно отправлено.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке сообщений: {ex.Message}");
        }
    }
}