using System.Linq;
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Blog.Models.Repos;
namespace Blog.Controllers{
    public class PostController : Controller{
        
        [Route("/post/{postTitle}")]
        public IActionResult Index([FromRoute] string postTitle){
            BlogRepo blogRepo = new BlogRepo();
            return View(
                blogRepo.Posts.Find(
                    p => p.Title ==postTitle.Replace("-", " ") 
                ).FirstOrDefault()
            );
        }
    }
}