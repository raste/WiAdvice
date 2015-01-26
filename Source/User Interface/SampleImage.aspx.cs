﻿﻿// Wi Advice (https://github.com/raste/WiAdvice)(http://www.wiadvice.com/)
// Copyright (c) 2015 Georgi Kolev. 
// Licensed under Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0).

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BusinessLayer;

namespace UserInterface
{
    public partial class SampleImage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                PopulateSampleImageTable();
            }
        }

        private void PopulateSampleImageTable()
        {
            string whichImage = Request.QueryString["show"];
            string imgUrl = string.Empty;
            int imgWidth = 0;
            int imgHeight = 0;

            if (whichImage != null)
            {
                GetImageHandler.GetImageUrlAndDimensions(whichImage, out imgUrl, out imgWidth, out imgHeight);
            }

            tblImageDimensions.Attributes.Add("style", "border-collapse:collapse;");

            TableRow imageRow = new TableRow();
            tblImageDimensions.Rows.Add(imageRow);
            TableCell imageCell = new TableCell();
            imageRow.Cells.Add(imageCell);
            Image img = new Image();
            imageCell.Controls.Add(img);
            img.ImageUrl = "GetImageHandler.ashx" + Request.Url.Query;
            TableCell vertLineCell = new TableCell();
            imageRow.Cells.Add(vertLineCell);
            vertLineCell.Attributes.Add("style", 
                "border-top-style:none; border-right-style:solid; border-bottom-style:none; border-left-style:none; border-right-width: 1px;");
            TableCell vertDimensionCell = new TableCell();
            imageRow.Cells.Add(vertDimensionCell);
            vertDimensionCell.VerticalAlign = VerticalAlign.Middle;
            if (string.IsNullOrEmpty(imgUrl) == false)
            {
                vertDimensionCell.Text = string.Format("{0}", imgHeight);
            }

            TableRow horzLineRow = new TableRow();
            tblImageDimensions.Rows.Add(horzLineRow);
            TableCell horzLineCell = new TableCell();
            horzLineRow.Cells.Add(horzLineCell);
            horzLineCell.Attributes.Add("style",
                "border-top-style:none; border-right-style:none; border-bottom-style:solid; border-left-style:none; border-bottom-width: 1px;");
            TableCell emptyCell1 = new TableCell();
            horzLineRow.Cells.Add(emptyCell1);
            TableCell emptyCell2 = new TableCell();
            horzLineRow.Cells.Add(emptyCell2);

            TableRow widthRow = new TableRow();
            tblImageDimensions.Rows.Add(widthRow);
            TableCell horzDimensionCell = new TableCell();
            widthRow.Cells.Add(horzDimensionCell);
            horzDimensionCell.HorizontalAlign = HorizontalAlign.Center;
            if (string.IsNullOrEmpty(imgUrl) == false)
            {
                horzDimensionCell.Text = string.Format("{0}", imgWidth);
            }
            TableCell emptyCell3 = new TableCell();
            horzLineRow.Cells.Add(emptyCell3);
            TableCell emptyCell4 = new TableCell();
            horzLineRow.Cells.Add(emptyCell4);

        }
    }
}
