using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web
{
    public static class Consts
    {
        public static string CdnUrl { get { return ConfigurationManager.AppSettings["CdnUrl"]; } }
        public static FacebookApp FacebookApp { get { return new FacebookApp(); } }
        public static GoogleApp GoogleApp { get { return new GoogleApp(); } }
        public static string WeeklyEmailKey { get { return ConfigurationManager.AppSettings["WeeklyEmailKey"]; } }

        public static string PhotoBlobContainerName { get { return ConfigurationManager.AppSettings["PhotoBlobContainerName"]; } }
        public static DateTime MinDate { get { return new DateTime(1980, 1, 1); } }
        public static ImageFormat ImageFormat { get { return new ImageFormat(); } }
        public static string SiteUrl { get { return ConfigurationManager.AppSettings["SiteUrl"]; } }
        public static string GoogleAnalyticsTrackingId { get { return ConfigurationManager.AppSettings["GoogleAnalyticsTrackingId"]; } }
        public static IEnumerable<Guid> FixedOnlineUsers { get { return ConfigurationManager.AppSettings["FixedOnlineUsers"].Split(',').Select(id => new Guid(id));} }
    }

    public class FacebookApp
    {
        public string AccessTokenAppId { get { return ConfigurationManager.AppSettings["FbAccessTokenAppId"]; } }
        public string AppId { get { return ConfigurationManager.AppSettings["FbAppId"]; } }
        public string AppSecret { get { return ConfigurationManager.AppSettings["FbAppSecret"]; } }
    }

    public class GoogleApp
    {
        public string ClientId { get { return ConfigurationManager.AppSettings["GgClientId"]; } }
        public string ClientSecret { get { return ConfigurationManager.AppSettings["GgClientSecret"]; } }
    }

    public class ImageFormat
    {
        public string Small { get { return ConfigurationManager.AppSettings["Small"]; } }
        public string Medium { get { return ConfigurationManager.AppSettings["Medium"]; } }
        public string Large { get { return ConfigurationManager.AppSettings["Large"]; } }
    }
}
