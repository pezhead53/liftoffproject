﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using LiftoffProject.Models;
using LiftoffProject.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Threading.Tasks;
using System.Web;
using LiftoffProject.ViewModels;
using Newtonsoft.Json;


namespace LiftoffProject.Controllers
{
    public class GameController : Controller
    {
        private GameDbContext context;

        public GameController(GameDbContext dbContext)
        {
            context = dbContext;
        }

         HttpClient client = new HttpClient();

        async Task RunAsync()
        {
            client.BaseAddress = new Uri("https://api-2445582011268.apicast.io/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("user-key","53cd1e93d56afabd2fb7e79f1f54509f");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        async Task<Game[]> GetGameAsync(string path)
        {
            Game[] games = null;
            string jsonGame = "";
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                jsonGame = await response.Content.ReadAsStringAsync();
                games = JsonConvert.DeserializeObject<Game[]>(jsonGame);
            }
            return games;
        }

        public IActionResult Index()
        {
            IList<Game> games = context.Games.ToList();
            return View(games);
        }

        public IActionResult Add()
        {
            AddGameViewModel addGameViewModel = new AddGameViewModel();
            return View(addGameViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddGameViewModel addGameViewModel)
        {
            RunAsync().GetAwaiter().GetResult();

            if (ModelState.IsValid)
            {
                var response = await GetGameAsync("/games/?search=" + addGameViewModel.Name + "&fields=name");
                TempData["response"] = response;
            }

            return View(addGameViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddCollection(Game game)
        {
            if (ModelState.IsValid)
            {

                RunAsync().GetAwaiter().GetResult();
                var games = await GetGameAsync("games/" + game.Id);
                foreach (Game newGame in games)
                {
                    context.Games.Add(newGame);
                    context.SaveChanges();
                }

                return Redirect("/Game");
            }
            else
            {
                return Redirect("/Game/Add");
            }
        }

        public IActionResult Delete(int gameId)
        {
            Game game = context.Games.Single(g => g.Id == gameId);
            context.Games.Remove(game);
            context.SaveChanges();
            return Redirect("/Game");
        }

        public async Task<IActionResult> Test()
        {
            RunAsync().GetAwaiter().GetResult();

            var response = await GetGameAsync("games/1?fields=name");

            return View(response);
        }
    }
}
