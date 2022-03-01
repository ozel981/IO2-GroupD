using SaleSystem.Models.Posts;
using System.Collections.Generic;

namespace SaleSystem.Services
{
    public interface IPostService
    {
        public ResponseNewPost CreatePost(PostCreate p, int userId);
        public void UpdatePost(int id, PostUpdate p, int userId);
        public void DeletePost(int id, int userId);
        public PostView GetPost(int id, int userId);
        public IEnumerable<PostView> GetUserPosts(int userId);
        public void PromotePost(PostPromotion p, int userId);
        public IEnumerable<PostLikerID> GetPostLikers(int id);
        public void UpdatePostLikeStatus(int commentId, PostLikeStatusUpdate likePostUpdate, int userId);
        public IEnumerable<PostView> GetAll(int userId);
        public IEnumerable<PostView> GetFilteredPosts(int userId, List<int> categoriesIDs);
        public PostPagedList GetFilteredPosts(int userId, List<int> categoriesIDs, int page, int pageSize);
    }
}
