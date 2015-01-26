﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CustomServerControls
{
    /// <summary>
    /// A button with wimages on both left and right sides.
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:DecoratedButton runat=server></{0}:DecoratedButton>")]
    public class DecoratedButton : Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecoratedButton"/> class.
        /// </summary>
        public DecoratedButton()
            : base()
        {
            CollapseOffset = 0;
        }

        protected readonly string NonBreakingSpace = "&nbsp;";
        private string imageLeftUrl = string.Empty;
        private string imageRightUrl = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the left image of the button.
        /// <para>The URL of the left image can only be set if it is empty.</para>
        /// </summary>
        /// <value>The URL of the left image of the button.</value>
        public string ImageLeftUrl
        {
            get { return imageLeftUrl ?? string.Empty; }
            set
            {
                if (string.IsNullOrEmpty(imageLeftUrl))
                {
                    imageLeftUrl = (value ?? string.Empty);
                }
            }
        }

        /// <summary>
        /// Gets or sets the URL of the right image of the button.
        /// <para>The URL of the right image can only be set if it is empty.</para>
        /// </summary>
        /// <value>The URL of the right image of the button.</value>
        public string ImageRightUrl
        {
            get { return imageRightUrl ?? string.Empty; }
            set
            {
                if (string.IsNullOrEmpty(imageRightUrl))
                {
                    imageRightUrl = (value ?? string.Empty);
                }
            }
        }

        /// <summary>
        /// Unconditionally changes <c>ImageLeftUrl</c>.
        /// </summary>
        /// <param name="imgLeftUrl">The new value of <c>ImageLeftUrl</c>.</param>
        public void ForceImageLeftUrl(string imgLeftUrl)
        {
            imageLeftUrl = imgLeftUrl ?? string.Empty;
        }

        /// <summary>
        /// Unconditionally changes <c>ImageRightUrl</c>.
        /// </summary>
        /// <param name="imgRightUrl">The new value of <c>ImageRightUrl</c>.</param>
        public void ForceImageRightUrl(string imgRightUrl)
        {
            imageRightUrl = imgRightUrl ?? string.Empty;
        }

        protected int CollapseOffset { get; set; }

        /// <summary>
        /// Outputs server control content to a provided <see cref="HtmlTextWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="HtmlTextWriter"/> object that receives the control content.</param>
        public override void RenderControl(HtmlTextWriter writer)
        {
            if (base.Visible)
            {
                writer.Write("<table cellpadding='0' cellspacing='0' class='dcrBtnTblStyle'><tr><td>");

                Image imgLeft = new Image();
                imgLeft.ImageUrl = ImageLeftUrl;
                imgLeft.RenderControl(writer);

                writer.Write("</td><td>");

                base.RenderControl(writer);

                writer.Write("</td><td>");

                Image imgRight = new Image();
                imgRight.ImageUrl = ImageRightUrl;
                imgRight.RenderControl(writer);

                writer.Write("</td></tr></table>");
            }

        }
    }
}
