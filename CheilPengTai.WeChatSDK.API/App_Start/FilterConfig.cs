using System.Web;
using System.Web.Mvc;

namespace CheilPengTai.WeChatSDK.API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}