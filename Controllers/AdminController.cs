using System.Linq;
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Blog.Models.Repos;
using Blog.Models;
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

        // This method won't return anything but a status code
        // that is because we'll be sending the post as a json string
        // via javascript and then reacting to it without reloading the page
        [HttpPost, Route("new/post")]
        public IActionResult NewPost([FromBody]Post post){
            BlogRepo.Posts.Insert(post);

            // set up the indices 
            BlogRepo.Posts.EnsureIndex(p => p.Title,true); // ensures that the title is unique
            BlogRepo.Posts.EnsureIndex(p => p._id, true); // ensures that the row has a valid bson index

            // check if the post has actually been created
            if(BlogRepo.Posts.Find(p => p.Title == post.Title).FirstOrDefault() != null ){
                // if the post actually exists return status code 201 - Created
                return StatusCode(201);
            }
            // We'll just assume something was wrong with the request
            // in an actual serious application we'd obviously do a bit more
            // to return a proper response to the user
            // which would allow the user to correct his mistake
            return StatusCode(400);
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
            if(BlogRepo.Posts.Find(p => p.Title == postTitle).FirstOrDefault() == null){
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