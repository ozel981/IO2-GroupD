using SaleSystem.Models.Comments;
using SaleSystem.Models.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SaleSystem.Services.Interfaces
{
    public interface ICommentService
    {
        IEnumerable<CommentView> GetComments(int userID);
        IEnumerable<CommentView> GetPostComments(int postID, int userID);
        GetComment GetComment(int commentID, int userID);
        IEnumerable<LikerID> GetUsersLikingComment(int commentID);
        ResponseNewComment AddComment(NewComment newComment, int userID);
        void UpdateComment(CommentUpdate commentUpdate, int commentID, int userID);
        void RemoveComment(int commentID, int userID);
        void UpdateCommentLikeStatus(int commentID, CommentLikeStatusUpdate likeCommentUpdate, int userID);
    }
}
