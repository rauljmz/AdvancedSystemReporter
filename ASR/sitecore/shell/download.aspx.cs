namespace ASR.sitecore.shell
{
    using Sitecore.SecurityModel;
    using Sitecore.Shell;

    public class download : DownloadPage
    {
        protected virtual void OnLoad(object sender, System.EventArgs e)
        {
            if (Current.Context.Settings.AllowNonAdminDownloads)
            {
                using(new SecurityDisabler())
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
