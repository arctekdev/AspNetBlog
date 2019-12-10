using Blog.Models;
using Microsoft.AspNetCore.Http;
namespace Blog.Models.Requests{
    public class PostRequest : Post {
        public IFormFile Image {get;set;}
    }
}