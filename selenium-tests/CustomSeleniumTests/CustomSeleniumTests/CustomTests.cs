
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Xunit;

namespace SeleniumTests
{
    public class CustomTests : SeleniumTests, IDisposable
    {
        private readonly string wall = "wall";
        private readonly string user = "user";
        private readonly string url = "http://localhost:4000/";

        public CustomTests()
        {
            SetUpDriver(url);
        }

        [Fact]
        public void VerifyValidLogin()
        {
            LogIn();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("wall-tab")));
            Assert.Equal(url + wall, driver.Url);
        }

        [Fact]
        public void VerifyValidPostCreationAndDeleting()
        {
            LogIn();

            string postContent = "Selenium create/delete post test content";
            string postTitle = "Selenium create/delete post test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            Assert.Equal(postTitle, driver.FindElement(By.Id($"post-{postId}-title")).Text);
            Assert.Equal(postContent, driver.FindElement(By.Id($"post-{postId}-content")).Text);

            driver.FindElement(By.Id($"post-{postId}-delete-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id($"post-{postId}-div")));
        }

        [Fact]
        public void VerifyValidPostLikeAndUnlike()
        {
            LogIn();

            string postContent = "Selenium like/unlike post test content";
            string postTitle = "Selenium like/unlike post test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            int likeCount = int.Parse(driver.FindElement(By.Id($"post-{postId}-like-button")).Text);

            driver.FindElement(By.Id($"post-{postId}-like-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            Assert.Equal(likeCount + 1, int.Parse(driver.FindElement(By.Id($"post-{postId}-unlike-button")).Text));

            driver.FindElement(By.Id($"post-{postId}-unlike-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            Assert.Equal(likeCount, int.Parse(driver.FindElement(By.Id($"post-{postId}-like-button")).Text));

            driver.FindElement(By.Id($"post-{postId}-delete-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id($"post-{postId}-div")));
        }

        [Fact]
        public void VerifyValidPostEdit()
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
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            driver.FindElement(By.Id($"post-{postId}-edit-button")).Click();
            driver.FindElement(By.Id($"post-{postId}-edit-title-input")).Clear();
            driver.FindElement(By.Id($"post-{postId}-edit-title-input")).SendKeys(editPostTitle);
            driver.FindElement(By.Id($"post-{postId}-edit-content-input")).Clear();
            driver.FindElement(By.Id($"post-{postId}-edit-content-input")).SendKeys(editPostContent);
            driver.FindElement(By.Id($"post-{postId}-edit-accept-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            Assert.Equal(editPostTitle, driver.FindElement(By.Id($"post-{postId}-title")).Text);
            Assert.Equal(editPostContent, driver.FindElement(By.Id($"post-{postId}-content")).Text);

            driver.FindElement(By.Id($"post-{postId}-delete-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id($"post-{postId}-div")));
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
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
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

            driver.FindElement(By.Id($"post-{postId}-delete-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id($"post-{postId}-div")));
        }

        [Fact]
        public void VerifyValidPostPromoteAndUnpromote()
        {
            LogIn();

            string postContent = "Selenium promote/unpromote post test content";
            string postTitle = "Selenium promote/unpromote post test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");

            driver.FindElement(By.Id($"post-{postId}-promote-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-unpromote-button")));

            driver.FindElement(By.Id($"post-{postId}-unpromote-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-promote-button")));

            driver.FindElement(By.Id($"post-{postId}-delete-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id($"post-{postId}-div")));
        }

        [Fact]
        public void VerifyValidCommentCreationAndDeleting()
        {
            LogIn();

            string postContent = "Selenium post for comments test content";
            string postTitle = "Selenium post for comments test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            string commentContent = "Selenium create/delete comment test content";

            driver.FindElement(By.Id($"post-{postId}-comment-content-input")).SendKeys(commentContent);
            driver.FindElement(By.Id($"post-{postId}-comment-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"post-{postId}-div")));

            driver.FindElement(By.Id($"post-{postId}-expand-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            ReadOnlyCollection<IWebElement> comments = driver.FindElement(By.Id($"post-{postId}-div")).FindElements(By.Id("comment-div"));
            int commentId = int.Parse(comments[^1].GetAttribute("innerHTML").Split('-')[1]);

            Assert.Equal(commentContent, comments[^1].FindElement(By.Id($"comment-{commentId}-content")).Text);

            comments[^1].FindElement(By.Id($"comment-{commentId}-delete-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id($"comment-{commentId}-div")));

            driver.FindElement(By.Id($"post-{postId}-delete-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id($"post-{postId}-div")));
        }

        [Fact]
        public void VerifyValidCommentLikeAndUnlike()
        {
            LogIn();

            string postContent = "Selenium post for comments test content";
            string postTitle = "Selenium post for comments test title";

            List<int> oldIds = GetDivIdsList("post-div");

            driver.FindElement(By.Id("post-title-input")).SendKeys(postTitle);
            driver.FindElement(By.Id("post-content-input")).SendKeys(postContent);
            driver.FindElement(By.Id("post-category-selector")).Click();
            driver.FindElement(By.Id("react-select-2-option-2")).Click();
            driver.FindElement(By.Id("post-create-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            int postId = GetNewId(oldIds, "post-div");
            Assert.NotEqual(-1, postId);

            string commentContent = "Selenium like/unlike comment test content";

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

            comments[^1].FindElement(By.Id($"comment-{commentId}-unlike-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id($"comments-{postId}-div")));

            Assert.Equal(likeCount, int.Parse(driver.FindElement(By.Id($"comment-{commentId}-like-button")).Text));

            driver.FindElement(By.Id($"post-{postId}-delete-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id($"post-{postId}-div")));
        }

        [Fact]
        public void VerifyValidCommentEdit()
        {
            LogIn();

            string postContent = "Selenium post for comments test content";
            string postTitle = "Selenium post for comments test title";

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

            driver.FindElement(By.Id($"post-{postId}-delete-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id($"post-{postId}-div")));
        }

        [Fact]
        public void VerifyValidCommentEditRejection()
        {
            LogIn();

            string postContent = "Selenium post for comments test content";
            string postTitle = "Selenium post for comments test title";

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

            driver.FindElement(By.Id($"post-{postId}-delete-button")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("post-div")));

            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id($"post-{postId}-div")));
        }

        [Fact]
        public void VerifySubscribeCategoryStatusChange()
        {
            LogIn();

            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("categories-subs")));

            string beforeId = "unsub-cat-1-b";
            string endId = "sub-cat-1-b";

            try
            {
                driver.FindElement(By.Id(beforeId));
            }
            catch (NoSuchElementException)
            {
                beforeId = endId;
                endId = "un" + endId;
            }

            driver.FindElement(By.Id(beforeId)).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id(endId)));
            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id(beforeId)));

            driver.FindElement(By.Id(endId)).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id(beforeId)));
            Assert.Throws<NoSuchElementException>(() => driver.FindElement(By.Id(endId)));
        }

        [Fact]
        public void VerifyGoToMyAccountTab()
        {
            LogIn();

            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("go-to-user-button")));
            driver.FindElement(By.Id("go-to-user-button")).Click();
            Assert.Equal(url + user, driver.Url);
        }

        [Fact]
        public void VerifyGoToMyAccountTabAndBack()
        {
            LogIn();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("go-to-user-button")));

            driver.FindElement(By.Id("go-to-user-button")).Click();
            Assert.Equal(url + user, driver.Url);

            driver.FindElement(By.Id("go-to-wall-button")).Click();
            Assert.Equal(url + wall, driver.Url);
        }

        [Fact]
        public void VerifyChangingAccountSettings()
        {
            LogIn();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("go-to-user-button")));

            driver.FindElement(By.Id("go-to-user-button")).Click();
            Assert.Equal(url + user, driver.Url);

            string newUsername = "Kubuś";
            string newEmail = "jmich@mailbox.com";

            driver.FindElement(By.Id("username-textfield")).Click();
            string oldUsername = driver.FindElement(By.Id("username-textfield")).GetAttribute("value");
            driver.FindElement(By.Id("username-textfield")).Clear();
            driver.FindElement(By.Id("username-textfield")).SendKeys(newUsername);

            driver.FindElement(By.Id("email-textfield")).Click();
            string oldEmail = driver.FindElement(By.Id("email-textfield")).GetAttribute("value");
            driver.FindElement(By.Id("email-textfield")).Clear();
            driver.FindElement(By.Id("email-textfield")).SendKeys(newEmail);

            driver.FindElement(By.Id("save-changes")).Click();
            WaitAfterReloadUntil(ExpectedConditions.ElementExists(By.Id("success-info")));

            driver.FindElement(By.Id("go-to-wall-button")).Click();
            Assert.Equal(url + wall, driver.Url);

            driver.FindElement(By.Id("go-to-user-button")).Click();
            Assert.Equal(url + user, driver.Url);

            Assert.Equal(newUsername, driver.FindElement(By.Id("username-textfield")).GetAttribute("value"));
            Assert.Equal(newEmail, driver.FindElement(By.Id($"email-textfield")).GetAttribute("value"));

            driver.FindElement(By.Id("username-textfield")).Click();
            driver.FindElement(By.Id("username-textfield")).Clear();
            driver.FindElement(By.Id("username-textfield")).SendKeys(oldUsername);

            driver.FindElement(By.Id("email-textfield")).Click();
            driver.FindElement(By.Id("email-textfield")).Clear();
            driver.FindElement(By.Id("email-textfield")).SendKeys(oldEmail);

            driver.FindElement(By.Id("save-changes")).Click();
        }

        public void Dispose()
        {
            if (driver != null)
                driver.Quit();
        }
    }
}
