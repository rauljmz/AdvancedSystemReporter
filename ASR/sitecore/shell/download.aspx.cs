namespace ASR.sitecore.shell
{
    using System;
    using System.Web;
    using System.Web.UI;
    using Sitecore.Security.Accounts;
    using Sitecore.Shell;

    public class download : DownloadPage
    {
        private void Page_Load(object sender, System.EventArgs e)
        {
            if (Current.Context.Settings.AllowNonAdminDownloads)
            {
                using (new UserSwitcher(Current.Context.Settings.AdminUser, false))
                {
                    base.OnLoad(e);
                }
            }
            else
            {
                base.OnLoad(e);
            }
        }
    }
}
