﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AbleStrategiesWebsite.Pages
{
    public class BuyModel : PageModel
    {
        public string Message { get; set; }

        public void OnGet()
        {
            Message = "Ready to buy?";
        }
    }
}