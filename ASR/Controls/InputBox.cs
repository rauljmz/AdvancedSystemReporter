using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Globalization;

namespace ASR.Controls
{
    public class InputBox : Edit
    {
        protected override void DoRender(System.Web.UI.HtmlTextWriter output)
        {
            this.Attributes["placeholder"] = Translate.Text(this.Placeholder);
            string type = this.Password ? " type=\"password\"" : (this.Hidden ? " type=\"hidden\"" : "");
            this.SetWidthAndHeightStyle();
            output.Write(string.Concat("<input id='",
                this.ID,
                "'",
                this.ControlAttributes,
                type,
                ">"));
            this.RenderChildren(output);
        }
    }
}
