﻿using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MagicVilla_Web.Controllers
{
    public class HomeController : Controller
    {

        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public HomeController(IVillaService villaService, IMapper mapper)
        {
            this._villaService = villaService;
            this._mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            List<VillaDTO> List = new();
            var response = await _villaService.GetAllAsync<APIResponse>();
            if (response != null && response.IsSuccess)
            {
                List = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
            }
            return View(List);
        }

     

    }
}