using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

class Program
{
    static void Main(string[] args)
    {
        // Lista de nomes de Pokémon
        List<string> pokemonNames = new List<string>()
        {
            "Pikachu", "Charmander", "Bulbasaur", "Squirtle", "Jigglypuff",
            "Meowth", "Psyduck", "Snorlax", "Eevee", "Gengar"
        };

        // Configurações do ChromeDriver
        var options = new ChromeOptions();
        options.AddArgument("--disable-extensions");
        options.AddArgument("--disable-popup-blocking");
        options.AddArgument("--start-maximized");
        options.AddArgument("user-data-dir=C:/Selenium/ChromeProfile"); // mantém a sessão do WhatsApp

        using (IWebDriver driver = new ChromeDriver(options))
        {
            driver.Navigate().GoToUrl("https://web.whatsapp.com");
            Console.WriteLine("🔐 Escaneie o QR Code e aguarde o carregamento...");

            // Aguarda as conversas carregarem
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
            wait.Until(driver => driver.FindElements(By.CssSelector("div[role='listitem']")).Count > 0);

            var chats = driver.FindElements(By.CssSelector("div[role='listitem']"));
            Console.WriteLine($"✅ Conversas encontradas: {chats.Count}");

            int countChats = 0;
            Actions actions = new Actions(driver);

            foreach (var chat in chats)
            {
                if (countChats >= 3) break;

                try
                {
                    Console.WriteLine($"➡️ Clicando no chat #{countChats + 1}...");
                    actions.MoveToElement(chat).Click().Perform();
                    Thread.Sleep(3000); // Aguarda a conversa abrir

                    // 🧠 Usa XPath para garantir que pegue a caixa de mensagem correta
                    var messageBoxWait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    var messageBox = messageBoxWait.Until(driver =>
                        driver.FindElement(By.XPath("//footer//div[@contenteditable='true']"))
                    );

                    Console.WriteLine("💬 Caixa de mensagem localizada. Enviando nomes de Pokémon...");

                    Random rnd = new Random();
                    for (int i = 0; i < 10; i++)
                    {
                        string poke = pokemonNames[rnd.Next(pokemonNames.Count)];
                        messageBox.SendKeys(poke);
                        Thread.Sleep(100);
                        messageBox.SendKeys(Keys.Enter);
                        Thread.Sleep(400);
                    }

                    countChats++;
                    Console.WriteLine("✅ Mensagens enviadas com sucesso.");
                    Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Erro ao enviar mensagens no chat #{countChats + 1}: {ex.Message}");
                    continue;
                }
            }

            Console.WriteLine("🏁 Processo concluído. Pressione qualquer tecla para encerrar...");
            Console.ReadKey();
        }
    }
}
