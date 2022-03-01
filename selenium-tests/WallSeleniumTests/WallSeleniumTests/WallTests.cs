
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace SeleniumTests
{
    public class WallTests : SeleniumTests, IDisposable
    {
        private readonly string wall = "wall";
        private readonly string url = "http://localhost:3000/";

        public WallTests()
        {
            SetUpDriver(url);
        }

        [Fact]
        public void VerifyValidLogin()
        {
            LogIn();
            Assert.Equal(url + wall, driver.Url);
        }

        [Fact]
        public void VerifyValidCategoryFilter()
        {
            LogIn();

            string category = driver.FindElement(By.Id("category-1-label")).Text;

            driver.FindElement(By.Id("category-all-checkbox")).Click();
            driver.FindElement(By.Id("category-1-checkbox")).Click();
            driver.FindElement(By.Id("categories-filter-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            ReadOnlyCollection<IWebElement> posts = driver.FindElements(By.Id("post-div"));
            for (int i = 0; i < posts.Count; i++)
            {
                int postId = int.Parse(posts[i].GetAttribute("innerHTML").Split('-')[1]);
                Assert.Equal(category, posts[i].FindElement(By.Id($"post-{postId}-category")).Text);
            }
        }

        [Fact]
        public void VerifyValidPostCreation()
        {
            LogIn();

            string postContent = "Selenium create post test content";
            string postTitle = "Selenium create post test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-3")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            Assert.Equal(postTitle, driver.FindElement(By.Id($"post-{postId}-title")).Text);
            Assert.Equal(postContent, driver.FindElement(By.Id($"post-{postId}-content")).Text);
        }

        [Fact]
        public void VerifyValidPostDeleting()
        {
            LogIn();

            string postContent = "Selenium delete post test content";
            string postTitle = "Selenium delete post test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-3")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            driver.FindElement(By.Id($"post-{postId}-delete-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id($"post-{postId}-div")));
        }

        [Fact]
        public void VerifyValidPostLike()
        {
            LogIn();

            string postContent = "Selenium like post test content";
            string postTitle = "Selenium like post test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-3")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            int likeCount = int.Parse(driver.FindElement(By.Id($"post-{postId}-like-button")).Text);

            driver.FindElement(By.Id($"post-{postId}-like-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            Assert.Equal(likeCount + 1, int.Parse(driver.FindElement(By.Id($"post-{postId}-unlike-button")).Text));
        }

        [Fact]
        public void VerifyValidPostUnlike()
        {
            LogIn();

            string postContent = "Selenium unlike post test content";
            string postTitle = "Selenium unlike post test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-3")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            int likeCount = int.Parse(driver.FindElement(By.Id($"post-{postId}-like-button")).Text);

            driver.FindElement(By.Id($"post-{postId}-like-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            driver.FindElement(By.Id($"post-{postId}-unlike-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            Assert.Equal(likeCount, int.Parse(driver.FindElement(By.Id($"post-{postId}-like-button")).Text));
        }

        [Fact]
        public void VerifyValidPostTitleEdit()
        {
            LogIn();

            string postContent = "Selenium edit post test content";
            string postTitle = "Selenium edit post test title";
            string editPostTitle = "Selenium edited post test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-3")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            driver.FindElement(By.Id($"post-{postId}-edit-button")).Click();
            driver.FindElement(By.Id($"post-{postId}-edit-title-input")).Clear();
            driver.FindElement(By.Id($"post-{postId}-edit-title-input")).SendKeys(editPostTitle);
            driver.FindElement(By.Id($"post-{postId}-edit-accept-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            Assert.Equal(editPostTitle, driver.FindElement(By.Id($"post-{postId}-title")).Text);
        }

        [Fact]
        public void VerifyValidPostContentEdit()
        {
            LogIn();

            string postContent = "Selenium edit post test content";
            string editPostContent = "Selenium edited post test content";
            string postTitle = "Selenium edit post test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-3")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            driver.FindElement(By.Id($"post-{postId}-edit-button")).Click();
            driver.FindElement(By.Id($"post-{postId}-edit-content-input")).Clear();
            driver.FindElement(By.Id($"post-{postId}-edit-content-input")).SendKeys(editPostContent);
            driver.FindElement(By.Id($"post-{postId}-edit-accept-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            Assert.Equal(editPostContent, driver.FindElement(By.Id($"post-{postId}-content")).Text);
        }

        [Fact]
        public void VerifyValidPostContentAndTitleEdit()
        {
            LogIn();

            string postContent = "Selenium edit post test content";
            string editPostContent = "Selenium edited post test content";
            string postTitle = "Selenium edit post test title";
            string editPostTitle = "Selenium edited post test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-3")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            driver.FindElement(By.Id($"post-{postId}-edit-button")).Click();
            driver.FindElement(By.Id($"post-{postId}-edit-content-input")).Clear();
            driver.FindElement(By.Id($"post-{postId}-edit-content-input")).SendKeys(editPostContent);
            driver.FindElement(By.Id($"post-{postId}-edit-title-input")).Clear();
            driver.FindElement(By.Id($"post-{postId}-edit-title-input")).SendKeys(editPostTitle);
            driver.FindElement(By.Id($"post-{postId}-edit-accept-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            Assert.Equal(editPostContent, driver.FindElement(By.Id($"post-{postId}-content")).Text);
            Assert.Equal(editPostTitle, driver.FindElement(By.Id($"post-{postId}-title")).Text);
        }

        [Fact]
        public void VerifyValidPostEditRejection()
        {
            LogIn();

            string postContent = "Selenium edit post test content";
            string editPostContent = "Selenium edited post test content";
            string postTitle = "Selenium edit post test title";
            string editPostTitle = "Selenium edited post test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-3")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            driver.FindElement(By.Id($"post-{postId}-edit-button")).Click();
            driver.FindElement(By.Id($"post-{postId}-edit-content-input")).Clear();
            driver.FindElement(By.Id($"post-{postId}-edit-content-input")).SendKeys(editPostContent);
            driver.FindElement(By.Id($"post-{postId}-edit-title-input")).Clear();
            driver.FindElement(By.Id($"post-{postId}-edit-title-input")).SendKeys(editPostTitle);
            driver.FindElement(By.Id($"post-{postId}-edit-reject-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            Assert.Equal(postContent, driver.FindElement(By.Id($"post-{postId}-content")).Text);
            Assert.Equal(postTitle, driver.FindElement(By.Id($"post-{postId}-title")).Text);
        }

        [Fact]
        public void VerifyCategoryUnchecked()
        {
            LogIn();

            driver.FindElement(By.Id("category-1-checkbox")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("category-1-checkbox")));

            Assert.False(driver.FindElement(By.Id("category-1-checkbox")).Selected);
        }

        [Fact]
        public void VerifyCategoryChecked()
        {
            LogIn();

            driver.FindElement(By.Id("category-1-checkbox")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("category-1-checkbox")));

            driver.FindElement(By.Id("category-1-checkbox")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("category-1-checkbox")));

            Assert.True(driver.FindElement(By.Id("category-1-checkbox")).Selected);
        }

        [Fact]
        public void VerifyValidCommentCreation()
        {
            LogIn();

            string postContent = "Selenium post for comment creation test content";
            string postTitle = "Selenium post for comment creation test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            string commentContent = "Selenium create comment test content";

            driver.FindElement(By.Id($"post-{postId}-comment-content-input")).SendKeys(commentContent);
            driver.FindElement(By.Id($"post-{postId}-comment-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            driver.FindElement(By.Id($"post-{postId}-expand-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            ReadOnlyCollection<IWebElement> comments = driver.FindElement(By.Id($"post-{postId}-div")).FindElements(By.Id("comment-div"));
            int commentId = int.Parse(comments[^1].GetAttribute("innerHTML").Split('-')[1]);

            Assert.Equal(commentContent, comments[^1].FindElement(By.Id($"comment-{commentId}-content")).Text);
        }

        [Fact]
        public void VerifyValidCommentDeleting()
        {
            LogIn();

            string postContent = "Selenium post for comment deleting test content";
            string postTitle = "Selenium post for comments deleting title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            string commentContent = "Selenium delete comment test content";

            driver.FindElement(By.Id($"post-{postId}-comment-content-input")).SendKeys(commentContent);
            driver.FindElement(By.Id($"post-{postId}-comment-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            driver.FindElement(By.Id($"post-{postId}-expand-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            ReadOnlyCollection<IWebElement> comments = driver.FindElement(By.Id($"post-{postId}-div")).FindElements(By.Id("comment-div"));
            int commentId = int.Parse(comments[^1].GetAttribute("innerHTML").Split('-')[1]);

            comments[^1].FindElement(By.Id($"comment-{commentId}-delete-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id($"comment-{commentId}-div")));
        }

        [Fact]
        public void VerifyValidCommentLike()
        {
            LogIn();

            string postContent = "Selenium post for comment like test content";
            string postTitle = "Selenium post for comment like test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            string commentContent = "Selenium like comment test content";

            driver.FindElement(By.Id($"post-{postId}-comment-content-input")).SendKeys(commentContent);
            driver.FindElement(By.Id($"post-{postId}-comment-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            driver.FindElement(By.Id($"post-{postId}-expand-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            ReadOnlyCollection<IWebElement> comments = driver.FindElement(By.Id($"post-{postId}-div")).FindElements(By.Id("comment-div"));
            int commentId = int.Parse(comments[^1].GetAttribute("innerHTML").Split('-')[1]);
            int likeCount = int.Parse(comments[^1].FindElement(By.Id($"comment-{commentId}-like-button")).Text);

            comments[^1].FindElement(By.Id($"comment-{commentId}-like-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            Assert.Equal(likeCount + 1, int.Parse(driver.FindElement(By.Id($"comment-{commentId}-unlike-button")).Text));
        }

        [Fact]
        public void VerifyValidCommentUnlike()
        {
            LogIn();

            string postContent = "Selenium post for comment unlike test content";
            string postTitle = "Selenium post for comment unlike test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            string commentContent = "Selenium unlike comment test content";

            driver.FindElement(By.Id($"post-{postId}-comment-content-input")).SendKeys(commentContent);
            driver.FindElement(By.Id($"post-{postId}-comment-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            driver.FindElement(By.Id($"post-{postId}-expand-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            ReadOnlyCollection<IWebElement> comments = driver.FindElement(By.Id($"post-{postId}-div")).FindElements(By.Id("comment-div"));
            int commentId = int.Parse(comments[^1].GetAttribute("innerHTML").Split('-')[1]);
            int likeCount = int.Parse(comments[^1].FindElement(By.Id($"comment-{commentId}-like-button")).Text);

            comments[^1].FindElement(By.Id($"comment-{commentId}-like-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            driver.FindElement(By.Id($"comment-{commentId}-unlike-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            Assert.Equal(likeCount, int.Parse(driver.FindElement(By.Id($"comment-{commentId}-like-button")).Text));
        }

        [Fact]
        public void VerifyValidCommentEdit()
        {
            LogIn();

            string postContent = "Selenium post for comment edit test content";
            string postTitle = "Selenium post for comment edit test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            string commentContent = "Selenium comment test content";
            string editCommentContent = "Selenium edit comment test content";

            driver.FindElement(By.Id($"post-{postId}-comment-content-input")).SendKeys(commentContent);
            driver.FindElement(By.Id($"post-{postId}-comment-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            driver.FindElement(By.Id($"post-{postId}-expand-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            ReadOnlyCollection<IWebElement> comments = driver.FindElement(By.Id($"post-{postId}-div")).FindElements(By.Id("comment-div"));
            int commentId = int.Parse(comments[^1].GetAttribute("innerHTML").Split('-')[1]);
            comments[^1].FindElement(By.Id($"comment-{commentId}-edit-button")).Click();
            comments[^1].FindElement(By.Id($"comment-{commentId}-edit-content-input")).Clear();
            comments[^1].FindElement(By.Id($"comment-{commentId}-edit-content-input")).SendKeys(editCommentContent);
            comments[^1].FindElement(By.Id($"comment-{commentId}-edit-accept-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            Assert.Equal(editCommentContent, driver.FindElement(By.Id($"comment-{commentId}-content")).Text);
        }

        [Fact]
        public void VerifyValidCommentEditRejection()
        {
            LogIn();

            string postContent = "Selenium post for comment edit rejection test content";
            string postTitle = "Selenium post for comment edit rejection test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            string commentContent = "Selenium comment test content";
            string editCommentContent = "Selenium edit comment test content";

            driver.FindElement(By.Id($"post-{postId}-comment-content-input")).SendKeys(commentContent);
            driver.FindElement(By.Id($"post-{postId}-comment-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            driver.FindElement(By.Id($"post-{postId}-expand-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            ReadOnlyCollection<IWebElement> comments = driver.FindElement(By.Id($"post-{postId}-div")).FindElements(By.Id("comment-div"));
            int commentId = int.Parse(comments[^1].GetAttribute("innerHTML").Split('-')[1]);
            comments[^1].FindElement(By.Id($"comment-{commentId}-edit-button")).Click();
            comments[^1].FindElement(By.Id($"comment-{commentId}-edit-content-input")).Clear();
            comments[^1].FindElement(By.Id($"comment-{commentId}-edit-content-input")).SendKeys(editCommentContent);
            comments[^1].FindElement(By.Id($"comment-{commentId}-edit-reject-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            Assert.Equal(commentContent, driver.FindElement(By.Id($"comment-{commentId}-content")).Text);
        }

        public void Dispose()
        {
            if (driver != null)
                driver.Quit();
        }
    }
}
