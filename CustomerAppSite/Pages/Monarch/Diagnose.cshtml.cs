using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CustomerAppSite.Pages.Asna
{
    public class DiagnoseModel : PageModel
    {
        public string AbEndMessage { get; set; }
        public string AbEndStack { get; set; }
        public void OnGet()
        {
            AbEndMessage = HttpContext.Session.GetString("ASNA_AbEndMessage");
            AbEndStack  = HttpContext.Session.GetString("ASNA_AbEndStack");

            if (AbEndStack != null)
            {
                string[] lines;
                lines = AbEndStack.Split(System.Environment.NewLine);
                string stackTrace = string.Empty;
                foreach (string line in lines)
                {
                    stackTrace = stackTrace + line + System.Environment.NewLine;
                }
                AbEndStack = stackTrace;
            }

            HttpContext.Session.Clear();
        }
    }
}