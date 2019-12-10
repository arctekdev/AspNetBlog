using System.IO;
using System.Net.Http;
using System.Linq;
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Blog.Models.Repos;
using Blog.Models;
using Blog.Models.Requests;
namespace Blog.Controllers{
    [Route("/admin")]
    public class AdminController : Controller{
        private BlogRepo BlogRepo{get;set;}

        // The Controller constructor will be auto called everytime we 
        // navigate to one of the routes/methods in the controller
        public AdminController(){
            BlogRepo = new BlogRepo();
        }

        public IActionResult Index(){
            return View(BlogRepo.Posts.FindAll());
        }

        // Since this method expects a GET request 
        // we simply return the view responsible for
        // creating the posts in question
        [HttpGet, Route("new/post")]
        public IActionResult NewPost(){
            return View();
        }

        [HttpPost, Route("new/post")]
        public IActionResult NewPost([FromForm] PostRequest PostReq){
            if(BlogRepo.Posts.Exists(p => p.Title == PostReq.Title)){
                return Content("Already exists");
            }
            Post Post = new Post(){
                Content = PostReq.Content,
                Title = PostReq.Title,
                Public = true,
            };
            var image = PostReq.Image;
            var imgPath = Path.Join("wwwroot/assets/images",image.FileName);
            if(image.Length > 0){
                using(FileStream fs = new FileStream( imgPath, FileMode.Create)) {
                    image.CopyTo(fs);
                }
            }
            if(System.IO.File.Exists(imgPath)){
                Post.CoverImagePath = imgPath;
            }
            BlogRepo.Posts.EnsureIndex(p => p.Title, true); 
            BlogRepo.Posts.EnsureIndex(p => p._id, true);
            BlogRepo.Posts.Insert(Post);
            if(BlogRepo.Posts.Exists(p => p.Title == PostReq.Title)){
                return StatusCode(201);
            }
            return StatusCode(400);
        }
        [HttpGet, Route("/admin/edit/{postTitle}")]
        public IActionResult PostEdit([FromRoute]string postTitle){
            postTitle = postTitle.Replace("-", " ");
            Post Post = BlogRepo.Posts.Find(p => p.Title == postTitle).FirstOrDefault();
            return View(Post);
        }

        // Http DELETE method 
        // Pretty self explanatory if you ask me
        // [FromBody] means that we'll expect a value to be passed in the body 
        // of the request with the key of postTitle
        [HttpDelete, Route("post/delete")]
        public IActionResult DeletePost([FromBody]string postTitle){
            BlogRepo.Posts.Delete(p => p.Title == postTitle);
            // We check the exact opposite of what we checked in post create
            // namely if the post does not exist anymore then we're all good
            if(!BlogRepo.Posts.Exists(p => p.Title == postTitle)){
                return StatusCode(200);
            }
            return StatusCode(400);
        }
        // Http PUT method expected
        // Stands for update in this case 
        [HttpPut, Route("post/update")]
        public IActionResult UpdatePost([FromBody]Post newPost, [FromBody]string postTitle){
            // Updating is a bit more complex because we obviously have to get the original post first
            // and then update it.
            // in this case LiteDB does mose of the work for us
            var postId = BlogRepo.Posts.Find(p => p.Title == postTitle ).FirstOrDefault()._id;
            bool status = BlogRepo.Posts.Update(postId, newPost);
            if(status){
                return StatusCode(200);
            }
            return StatusCode(400);
        }
    }
}