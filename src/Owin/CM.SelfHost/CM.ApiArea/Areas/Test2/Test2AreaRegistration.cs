using System.Web.Http;
using System.Web.Mvc;

namespace CM.ApiArea.Areas.Test2
{
    public class Test2AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Test2";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                this.AreaName + "Api",
                "api/" + this.AreaName + "/{controller}/{action}/{id}",
                new
                {
                    action = RouteParameter.Optional,
                    id = RouteParameter.Optional,
                    namespaceName = new string[] { string.Format("CM.ApiArea.Areas.{0}.Controllers", this.AreaName) },
                    AreaName = this.AreaName
                }
            );
            Test2Config.Register(GlobalConfiguration.Configuration);
        }
    }
}