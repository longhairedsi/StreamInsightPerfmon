using System.Web;
using System.Web.Mvc;

namespace PerformanceInsights.MonitorSite
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}