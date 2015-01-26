﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

namespace CustomServerControls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:WbkdTransButton runat=server></{0}:WbkdTransButton>")]
    public class WbkdTransButton : HyperLink 
    {
        public WbkdTransButton()
            : base()
        {
            CollapseOffset = 0;
        }

        protected int CollapseOffset { get; set; }

        /// <summary>
        /// Outputs server control content to a provided <see cref="HtmlTextWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="HtmlTextWriter"/> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            Attributes.Add("style", "text-decoration: none;");
            Attributes.Add("onclick", "return webkbd.switcherClicked(event);");
            NavigateUrl = "http://code.ppetrov.com/webkbd/";
            Text = "WebKBD";
            if (base.Visible)
            {
                base.RenderControl(writer);
            }
        }
    }

}
