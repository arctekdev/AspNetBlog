using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Blog.Models.Repos;

namespace Blog.Controllers{
    [Route("/")]
    public class HomeController : Controller{
        public IActionResult Index(){
            BlogRepo blogRepo = new BlogRepo();
            return View(blogRepo.Posts.Find(
                p => p.Public == true && 
                p.Created <= DateTime.Now && 
                p.Deleted == false)
            );
        }
    }
}