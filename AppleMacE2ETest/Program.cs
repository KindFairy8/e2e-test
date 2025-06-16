using System;
using System.Threading.Tasks;
using Microsoft.Playwright;

namespace AppleMacE2ETest
{
    class Program
    {
        public static async Task Main()
        {
            try
            {
                await RunTestAsync();
                Console.WriteLine("🎉 Всі продукти знайдено. Тест успішний.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Помилка: {ex.Message}");
                Environment.Exit(1);
            }
        }

        private static async Task RunTestAsync()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = true
            });

            var context = await browser.NewContextAsync();
            var page = await context.NewPageAsync();

            Console.WriteLine("🔍 Відкриваємо сторінку Apple Mac...");
            await page.GotoAsync("https://www.apple.com/mac/", new PageGotoOptions
            {
                WaitUntil = WaitUntilState.NetworkIdle
            });

            bool hasErrors = false;

            foreach (var product in Products.Items)
            {
                var locator = page.Locator($"text={product}");
                var count = await locator.CountAsync();

                if (count > 0)
                {
                    Console.WriteLine($"✅ Знайдено: {product}");
                }
                else
                {
                    Console.WriteLine($"❌ НЕ знайдено: {product}");
                    hasErrors = true;
                }
            }

            if (hasErrors)
            {
                await page.ScreenshotAsync(new() { Path = "error-screenshot.png", FullPage = true });
                Console.WriteLine("📸 Збережено скріншот помилки.");
                throw new Exception("Деякі продукти не знайдено або сталася помилка!");
            }

            await browser.CloseAsync();
        }
    }
}