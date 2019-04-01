using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PuppeteerSharp;

namespace PScraper.Controllers
{
    [Route("api/[controller]")]
    public class ScrapeController : Controller
    {
        static Browser BROWSER = null;
        static Page page = null;

        [HttpGet]
        public async Task<string> Get(string url, int timeoutMilliseconds = 30000)
        {
            if(url == null)
            {
                return "Please provide url querystring";
            }

            if (BROWSER == null)
            {
                await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

                BROWSER = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = true,
                    Args = new string[] { "--no-sandbox", "--disable-accelerated-2d-canvas", "--disable-gpu", "--proxy-server='direct://'", "--proxy-bypass-list=*" }
                });
            }

            if (page == null)
            {
                page = await BROWSER.NewPageAsync();
                await page.SetUserAgentAsync("PScraper-SiteCrawler");
                await page.SetViewportAsync(new ViewPortOptions() { Width = 1024, Height = 842 });
            }


            var response = await page.GoToAsync(url, new NavigationOptions() { Referer = "PScraper-SiteCrawler", Timeout = timeoutMilliseconds, WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } });
            return await response.TextAsync();
        }

    }
}