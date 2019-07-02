using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace I_Facility.Models
{
    public class list
    {
        public string display()
        {
            List<SelectListItem> listItems = new List<SelectListItem>();
            listItems.Add(new SelectListItem()
            {
                Value = "1",
            });
            listItems.Add(new SelectListItem()
            {
                Value = "2",
            });
            listItems.Add(new SelectListItem()
            {
                Value = "3",
            });
            listItems.Add(new SelectListItem()
            {
                Value = "4",
            });
            listItems.Add(new SelectListItem()
            {
                Value = "5",
            });
            listItems.Add(new SelectListItem()
            {
                Value = "6",
            });
            listItems.Add(new SelectListItem()
            {
                Value = "7",
            });
            listItems.Add(new SelectListItem()
            {
                Value = "8",
            }); listItems.Add(new SelectListItem()
            {
                Value = "9",
            });
            listItems.Add(new SelectListItem()
            {
                Value = "10",
            });
            listItems.Add(new SelectListItem()
            {
                Value = "11",
                Selected = true,
            });
            listItems.Add(new SelectListItem()
            {
                Value = "12",
            });
           
            var listItem = listItems.ToString();
            return listItem;
        }
        public string display1()
        {
            List<SelectListItem> selecteditem = new List<SelectListItem>();
            selecteditem.Add(new SelectListItem()
            {
                Value = "1",
            });
            selecteditem.Add(new SelectListItem()
            {
                Value = "2",
            });
            selecteditem.Add(new SelectListItem()
            {
                Value = "3",
            });
            selecteditem.Add(new SelectListItem()
            {
                Value = "4",
            });
            selecteditem.Add(new SelectListItem()
            {
                Value = "5",
            });
            var selecteditems = selecteditem.ToString();
            return selecteditems;
        }
    }
}