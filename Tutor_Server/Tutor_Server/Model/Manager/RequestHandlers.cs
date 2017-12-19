using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tutor_Server.Model.Data;

namespace Tutor_Server.Model.Manager
{
    public class RequestHandlers
    {
        private static string serverroot = SettingsManager.ServerRoot;
        public static Response GetResponse(RequestType Type, JObject Request)
        {
            
            switch(Type)
            {
                case RequestType.SignIn:
                    return SignInResponse(Request.ToObject<SignIn>());
                case RequestType.Home:
                    return HomeResponse(Request.ToObject<HomeRequest>());
                case RequestType.VideoDetails:
                    return VideoDetailsResponse(Request.ToObject<VideoDetails>());
                case RequestType.SignUp:
                    return CreateAccountResponse(Request.ToObject<CreateAccount>());
                case RequestType.AccountDetails:
                    return AccountDetailResponse(Request.ToObject<AccountDetails>());
                case RequestType.PostComment:
                    return PostCommentResponse(Request.ToObject<PostComment>());
                case RequestType.UsernameAvailability:
                    return UsernameAvailbilityResponse(Request.ToObject<AvailUsername>());
                case RequestType.DeleteComment:
                    return DeleteCommentResponse(Request.ToObject<DeleteComment>());
                case RequestType.UpdateProfile:
                    return UpdateProfileResponse(Request.ToObject<ProfileUpdateRequest>());
                case RequestType.CreateCourse:
                    return CreateCourseResponse(Request.ToObject<CreateCourseRequest>());
                case RequestType.CourseList:
                    return ListCourseResponse(Request.ToObject<CourseListRequest>());
                case RequestType.CreateVideo:
                    return AddToCourseResponse(Request.ToObject<CreateVideoRequest>());
                case RequestType.EditCourseVideo:
                    return EditVideoCoursesResponse(Request.ToObject<EditCourseVideos>());
                case RequestType.ModifyCourse:
                    return ModifyCourseResponse(Request.ToObject<CreateCourseRequest>());
                case RequestType.VideoUpdate:
                    return UpdateVideoResponse(Request.ToObject<VideoUpdateRequest>());
                case RequestType.DeleteVideo:
                    return DeleteVideoResponse(Request.ToObject<DeleteVideoRequest>());
                case RequestType.DeleteCourse:
                    return DeleteCourseResponse(Request.ToObject<DeleteCourseRequest>());
                case RequestType.AddFavorites:
                    return AddFavoritesResponse(Request.ToObject<AddToFavorites>());
                case RequestType.RateCourse:
                    return RateCourseResponse(Request.ToObject<RatingRequest>());
                case RequestType.LikeVideo:
                    return LikeVideoResponse(Request.ToObject<LikeVideo>());
                case RequestType.LikeComment:
                    return LikeCommentResponse(Request.ToObject<LikeComment>());
                case RequestType.Favorites:
                    return FavoriteVideosResponse(Request.ToObject<FavoritesRequest>());
                case RequestType.DeleteFavorite:
                    return DeleteFavoriteResponse(Request.ToObject<DeleteFavorite>());
                case RequestType.Search:
                    return SearchResponse(Request.ToObject<Search>());
            }
            return null;
        }

        private static Response SearchResponse(Search search)
        {
            string query = "";
            SearchResponse response = new SearchResponse() { Type = search.SearchBy , Status = "OK",  Reason=""};
            Response resp = new Response() { Type = ResponseType.Search, Status = "OK", Content = response };
            switch (search.SearchBy)
            {
                case SearchType.Title:
                    query = $"SELECT duration, thumbnail, title, videoid FROM video_details WHERE title LIKE '%{RefineContent(search.SearchText)}%' ";
                    break;
                case SearchType.Tags:
                    query = $"SELECT duration, thumbnail, title, videoid FROM video_details WHERE videoid IN " +
                            $"( SELECT videoid FROM tag_details WHERE tag LIKE '%{RefineContent(search.SearchText)}%' ) ";
                    break;
                case SearchType.Author:
                    query = $"SELECT duration, thumbnail, title, videoid FROM video_details WHERE authorid IN " +
                            $"( SELECT username FROM account_data WHERE fullname LIKE '%{RefineContent(search.SearchText)}%') ";
                    break;
                case SearchType.Course:
                    query = $"SELECT course_details.courseid, course_details.coursename, course_details.authorid, account_data.fullname,course_details.rating,course_details.createdon,course_details.videocount " +
                            $"FROM course_details INNER JOIN account_data" +
                            $" WHERE account_data.username = course_details.authorid AND" +
                            $" course_details.coursename LIKE '%{RefineContent(search.SearchText)}%' ";
                    break;
            }
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery(query);
            
            if(search.SearchBy==SearchType.Course && reader!=null)
            {
                response.CourseList = new List<CourseDetails>();
                while (reader.Read())
                {
                    response.CourseList.Add(new CourseDetails()
                    {
                        CourseID = reader.GetString(0),
                        CourseName = reader.GetString(1),
                        AuthorID = reader.GetString(2),
                        AuthorName = reader.GetString(3),
                        Rating = float.Parse(reader.GetString(4)),
                        CreatedOn = DateTime.Parse(reader.GetString(5)),
                        VideoCount = int.Parse(reader.GetString(6))
                    });
                }
            }
            else if(reader!=null)
            {
                response.VideoList = new List<VideoMini>();
                while (reader.Read())
                {
                    response.VideoList.Add(new VideoMini()
                    {
                        Duration = reader.GetString(0),
                        Thumbnail = reader.GetString(1),
                        Title = reader.GetString(2),
                        VideoId = reader.GetString(3)
                    });
                }
            }
            else
              response = new Data.SearchResponse() { Type = search.SearchBy, Status = "FAIL", Reason = "Unable to Search" };
            Connection.Close();
            return resp;

        }

        private static Response DeleteFavoriteResponse(DeleteFavorite deleteFavorite)
        {
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery($"DELETE FROM favorites WHERE videoid='{deleteFavorite.VideoID}' AND userid='{deleteFavorite.UserID}'");
            Acknowledge ack = new Acknowledge() { Status = "OK", Reason = "Video deleted successfully." };
            Response resp = new Response() { Type = ResponseType.Acknowledge, Status = "OK", Content = ack };
            if (reader == null)
            {
                ack.Reason = "Unable to delete video.";
                ack.Status = "FAIL";
                Connection.Close();
                return resp;
            }
            Connection.Close();
            return resp;
        }

        private static Response FavoriteVideosResponse(FavoritesRequest favoritesRequest)
        {
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery($"SELECT duration, thumbnail, title, videoid FROM video_details WHERE videoid IN ( SELECT videoid FROM favorites WHERE userid='{favoritesRequest.Userid}')");
            if (reader != null)
            {
                Data.EditCourseVideosResponse res = new Data.EditCourseVideosResponse();
                res.videoList = new List<VideoMini>();
                while (reader.Read())
                {
                    res.videoList.Add(new VideoMini()
                    {
                        Duration = reader.GetString(0),
                        Thumbnail = reader.GetString(1),
                        Title = reader.GetString(2),
                        VideoId = reader.GetString(3)
                    });
                }
                Connection.Close();
                return new Response() { Type = ResponseType.Favorites, Content = res, Status = "OK" };
            }
            Connection.Close();
            return new Response() { Content = null, Type = ResponseType.Favorites, Status = "FAIL" };
        }

        private static Response LikeCommentResponse(LikeComment likeComment)
        {
            DatabaseManager database = new DatabaseManager();
            string query = "";
            if (likeComment.LikeDislike > 0)
                query = $"UPDATE comments SET likes=likes+1 WHERE uid = '{likeComment.CommentID}'";
            else
                query = $"UPDATE comments SET dislikes=dislikes+1 WHERE uid = '{likeComment.CommentID}'";
            (MySqlDataReader reader, var Connection) = database.RunQuery(query);
            Acknowledge ack = new Acknowledge() { Status = "OK", Reason = "Liked" };
            Response resp = new Response() { Type = ResponseType.Acknowledge, Status = "OK", Content = ack };
            if (reader == null)
            {
                ack.Reason = "Unable to like";
                ack.Status = "FAIL";
            }
            Connection.Close();
            return resp;
        }

        private static Response LikeVideoResponse(LikeVideo likeVideo)
        {
            DatabaseManager database = new DatabaseManager();
            string query = "";
            if (likeVideo.LikeDislike > 0)
                query = $"UPDATE video_details SET likes=likes+1 WHERE videoid = '{likeVideo.VideoID}'";
            else
                query = $"UPDATE video_details SET dislikes=dislikes+1 WHERE videoid = '{likeVideo.VideoID}'";
            (MySqlDataReader reader, var Connection) = database.RunQuery(query);
            Acknowledge ack = new Acknowledge() { Status = "OK", Reason = "Liked" };
            Response resp = new Response() { Type = ResponseType.Acknowledge, Status = "OK", Content = ack };
            if (reader == null)
            {
                ack.Reason = "Unable to like";
                ack.Status = "FAIL";
            }
            Connection.Close();
            return resp;
        }

        private static Response RateCourseResponse(RatingRequest ratingRequest)
        {
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery($"UPDATE course_details SET rating=(rating * ratingCount +{ratingRequest.Rating})/(ratingCount+1) , ratingCount=ratingCount+1 WHERE courseid='{ratingRequest.CourseID}'");
            RatingResponse ack = new RatingResponse() { Status = "FAIL", Reason = "Failed to rate course." };
            Response resp = new Response() { Type = ResponseType.RateCourse, Status = "OK", Content = ack };
            if (reader!= null)
            {
                ack.Reason = "Course Rated Successfully.";
                ack.Status = "OK";
                Connection.Close();
                (reader, Connection) = database.RunQuery($"SELECT rating FROM course_details WHERE courseid='{ratingRequest.CourseID}'");
                if (reader != null)
                {
                    reader.Read();
                    ack.NewRating = float.Parse(reader.GetString(0));
                }

            }
            Connection.Close();
            return resp;
        }

        private static Response AddFavoritesResponse(AddToFavorites addToFavorites)
        {
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery($"INSERT INTO favorites (userid, videoid) VALUES('{addToFavorites.UserID}', '{addToFavorites.VideoID}')");
            Acknowledge ack = new Acknowledge() { Status = "OK", Reason = "Added to favorites." };
            Response resp = new Response() { Type = ResponseType.Acknowledge, Status = "OK", Content = ack };
            if (reader == null)
            {
                ack.Reason = "Unable to add to favorites.";
                ack.Status = "FAIL";
            }
            Connection.Close();
            return resp;
        }

        private static Response DeleteCourseResponse(DeleteCourseRequest deleteCourseRequest)
        {
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery($"DELETE FROM course_details WHERE courseid='{deleteCourseRequest.CourseID}' AND videocount='0'");
            Acknowledge ack = new Acknowledge() { Status = "OK", Reason = "Course deleted successfully." };
            Response resp = new Response() { Type = ResponseType.Acknowledge, Status = "OK", Content = ack };
            if (reader == null)
            {
                ack.Reason = "Non-empty Courses can't be deleted.";
                ack.Status = "FAIL";
            }
            Connection.Close();
            return resp;
        }

        private static Response DeleteVideoResponse(DeleteVideoRequest deleteVideoRequest)
        {
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery($"DELETE FROM video_details WHERE videoid='{deleteVideoRequest.VideoID}'");
            Acknowledge ack = new Acknowledge() { Status = "OK", Reason = "Video deleted successfully." };
            Response resp = new Response() { Type = ResponseType.Acknowledge, Status = "OK", Content = ack };
            if(reader==null)
            {
                ack.Reason = "Unable to delete video.";
                ack.Status = "FAIL";
                Connection.Close();
                return resp;
            }
            else
            {
                Connection.Close();
                (reader, Connection) = database.RunQuery($"UPDATE course_details SET videocount = videocount-1 WHERE courseid='{deleteVideoRequest.CourseID}'");
                Connection.Close();
                (reader, Connection) = database.RunQuery($"DELETE FROM comments WHERE videoid='{deleteVideoRequest.VideoID}'");
                Connection.Close();
                (reader, Connection) = database.RunQuery($"DELETE FROM tag_details WHERE videoid='{deleteVideoRequest.VideoID}'");
                Connection.Close();
                (reader, Connection) = database.RunQuery($"DELETE FROM favorites WHERE videoid='{deleteVideoRequest.VideoID}'");
                Connection.Close();
                if (File.Exists(serverroot + "/Thumbnails/" + deleteVideoRequest.VideoID + ".jpg"))
                    File.Delete(serverroot + "/Thumbnails/" + deleteVideoRequest.VideoID + ".jpg");
                if (File.Exists(serverroot + "/Videos/" + deleteVideoRequest.VideoID + ".mp4"))
                    File.Delete(serverroot + "/Videos/" + deleteVideoRequest.VideoID + ".mp4");
            }
            return resp;
        }

        private static Response UpdateVideoResponse(VideoUpdateRequest videoUpdateRequest)
        {
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery($"UPDATE video_details SET title='{RefineContent(videoUpdateRequest.Title)}', description='{RefineContent(videoUpdateRequest.Description)}' WHERE videoid='{videoUpdateRequest.VideoID}'");
            if (reader != null)
            {
                VideoUpdateResponse res = new VideoUpdateResponse()
                {
                    IsThumbnailUpdateRequired = videoUpdateRequest.IsThumbnailUpdateRequired,
                    ThumbnailImage= videoUpdateRequest.VideoID,
                    Reason = "Video Updated Successfully",
                    Status = "OK"
                };

                if (videoUpdateRequest.Tags.Count >= 1)
                {
                    database.RunQuery($"DELETE FROM tag_details WHERE videoid='{videoUpdateRequest.VideoID}'");
                    StringBuilder str = new StringBuilder($"INSERT INTO tag_details (tag, videoid) VALUES ('{RefineContent(videoUpdateRequest.Tags[0])}', '{videoUpdateRequest.VideoID}')");
                    for (int i = 1; i < videoUpdateRequest.Tags.Count; i++)
                        str.Append($", ('{videoUpdateRequest.Tags[i]}', '{videoUpdateRequest.VideoID}')");
                    database.RunQuery(str.ToString());
                }
                Connection.Close();
                return new Response() { Type = ResponseType.VideoUpdate, Content = res, Status = "OK" };
            }
            Connection.Close();
            return new Response() { Content = null, Type = ResponseType.VideoUpdate, Status = "FAIL" };
        }

        private static Response ModifyCourseResponse(CreateCourseRequest createCourseRequest)
        {
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery($"UPDATE course_details SET coursename='{createCourseRequest.CourseName}' WHERE courseid='{createCourseRequest.UserId}' ");
            if (reader != null)
            {
                Acknowledge ack = new Acknowledge() { Status = "OK", Reason = "" };
                if (reader.RecordsAffected > 0)
                {
                    Connection.Close();
                    return new Response() { Type = ResponseType.Acknowledge, Content = ack, Status = "OK" };
                }
            }
            Connection.Close();
            return new Response() { Content = null, Type = ResponseType.Acknowledge, Status = "FAIL" };
        }

        private static Response EditVideoCoursesResponse(EditCourseVideos editCourseVideos)
        {
            //request SQL DataBase
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery($"SELECT duration, thumbnail, title, videoid FROM video_details WHERE courseid ='{editCourseVideos.CourseID}' ");
            if (reader != null)
            {
                Data.EditCourseVideosResponse res = new Data.EditCourseVideosResponse();
                res.videoList = new List<VideoMini>();
                while (reader.Read())
                {
                    res.videoList.Add(new VideoMini()
                    {
                        Duration = reader.GetString(0),
                        Thumbnail = reader.GetString(1),
                        Title = reader.GetString(2),
                        VideoId = reader.GetString(3)
                    });
                }
                Connection.Close();
                return new Response() { Type = ResponseType.EditCourseVideo, Content = res, Status = "OK" };
            }
            Connection.Close();
            return new Response() { Content = null, Type = ResponseType.EditCourseVideo, Status = "FAIL" };
        }

        //Add a video to an existing course
        private static Response AddToCourseResponse(CreateVideoRequest createVideoResponse)
        {
            CreateVideoResponse ack = new CreateVideoResponse() { Status = "FAIL", Reason = "Video Creation Failed." };
            Response resp = new Response() { Type = ResponseType.CreateVideo, Status = "OK", Content = ack };
            string videoID = Guid.NewGuid().ToString();
            string query = $"INSERT INTO video_details(videoid, path, thumbnail, title, description, width, height, duration, authorid, authorname, authorimage, course, courseid) VALUES " +
                            $"('{videoID}','/Videos/{videoID}.mp4','/Thumbnails/{videoID}.jpg'," +
                            $"'{RefineContent(createVideoResponse.Title)}','{RefineContent(createVideoResponse.Decription)}','{createVideoResponse.Width}'," +
                            $"'{createVideoResponse.Height}','{createVideoResponse.Duration}','{createVideoResponse.AuthorId}','{RefineContent(createVideoResponse.AuthorName)}'," +
                            $"'/User/{createVideoResponse.AuthorId}.jpg','{RefineContent(createVideoResponse.CourseName)}','{createVideoResponse.CourseId}')";
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery(query);
            if (reader != null && reader.RecordsAffected > 0)
            {
                ack.Status = "OK";
                ack.Reason = "Success";
                ack.VideoID = videoID;
                query = $"UPDATE course_details SET videocount = videocount + 1 WHERE courseid='{createVideoResponse.CourseId}'";
                database.RunQuery(query);

                if (createVideoResponse.Tags.Count >= 1)
                {
                    StringBuilder str = new StringBuilder($"INSERT INTO tag_details (tag, videoid) VALUES ('{createVideoResponse.Tags[0]}', '{videoID}')");
                    for (int i = 1; i < createVideoResponse.Tags.Count; i++)
                        str.Append($", ('{createVideoResponse.Tags[i]}', '{videoID}')");
                    database.RunQuery(str.ToString());
                }
            }
            Connection.Close();
            return resp;
        }

        private static Response ListCourseResponse(CourseListRequest courseListRequest)
        {
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery($"SELECT course_details.courseid, course_details.coursename, course_details.authorid, account_data.fullname,course_details.rating,course_details.createdon,course_details.videocount " +
                $"FROM course_details INNER JOIN account_data" +
                $" WHERE account_data.username = course_details.authorid AND" +
                $" account_data.username = '{courseListRequest.AuthorID}'");

            if (reader != null)
            {
                CourseListResponse res = new CourseListResponse() { Status = "OK", Reason = "Success" };
                res.CourseList = new List<CourseDetails>();
                while (reader.Read())
                {
                    res.CourseList.Add(new CourseDetails()
                    {
                        CourseID = reader.GetString(0),
                        CourseName = reader.GetString(1),
                        AuthorID = reader.GetString(2),
                        AuthorName = reader.GetString(3),
                        Rating = float.Parse(reader.GetString(4)),
                        CreatedOn =DateTime.Parse(reader.GetString(5)),
                        VideoCount = int.Parse(reader.GetString(6))
                    });
                }
                Connection.Close();
                return new Response() { Type = ResponseType.CourseList, Content = res, Status = "OK" };
            }
            Connection.Close();
            return new Response() { Content = null, Type = ResponseType.CourseList, Status = "FAIL" };
        }

        private static Response CreateCourseResponse(CreateCourseRequest createCourseRequest)
        {
            Acknowledge ack = new Acknowledge() { Status = "FAIL", Reason = "Course Creation Failed." };
            Response resp = new Response() { Type = ResponseType.Acknowledge, Status = "OK", Content = ack };
            string query = String.Format("INSERT INTO course_details (courseid, coursename, authorid) VALUES (uuid(),'{0}','{1}')",RefineContent(createCourseRequest.CourseName), createCourseRequest.UserId);
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery(query);
            if (reader != null && reader.RecordsAffected > 0)
            {
                ack.Status = "OK";
                ack.Reason = "Success";
            }
            Connection.Close();
            return resp;
        }

        private static Response UpdateProfileResponse(ProfileUpdateRequest profileUpdateRequest)
        {
            ProfileUpdateResponse ack = new ProfileUpdateResponse() { Status = "OK", Reason = "Profile Updated Successfully.", RequiresImageUpdate = profileUpdateRequest.RequiresImageUpdate };
            Response resp = new Response() { Type = ResponseType.UpdateProfile, Status = "OK", Content = ack };
            string query = "";
            if (profileUpdateRequest.RequiresImageUpdate)
                query = string.Format("UPDATE account_data SET fullname='{0}', profileimage='/User/{1}.jpg' WHERE username='{2}'", RefineContent(profileUpdateRequest.FullName),profileUpdateRequest.Username ,profileUpdateRequest.Username);
            else
                query = string.Format("UPDATE account_data SET fullname='{0}' WHERE username='{1}'",RefineContent(profileUpdateRequest.FullName), profileUpdateRequest.Username);
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery(query);
            if (reader != null)
            {
                if (reader.RecordsAffected == 0)
                {
                    ack.Status = "FAIL";
                    ack.Reason = "Failed to update.";
                }
                else if(profileUpdateRequest.RequiresPasswordUpdate)
                {
                    query = string.Format("UPDATE user_login SET password='{0}' WHERE username='{1}' AND password='{2}'", profileUpdateRequest.Password, profileUpdateRequest.Username, profileUpdateRequest.OldPassword);
                    (reader, Connection) = database.RunQuery(query);
                    if (reader == null || reader.RecordsAffected == 0)
                    {
                        ack.Status = "FAIL";
                        ack.Reason = "Failed to Update Password.";
                    }

                }
                Connection.Close();
                return resp;
            }
            else
            {
                Connection.Close();
                return new Response() { Type = ResponseType.Acknowledge, Status = "FAIL" };
            }
        }

        private static Response DeleteCommentResponse(DeleteComment deleteComment)
        {
            Acknowledge ack = new Acknowledge() { Status = "OK", Reason = "" };
            Response resp = new Response() { Type = ResponseType.Acknowledge, Status = "OK", Content = ack };
            string query = string.Format("DELETE FROM comments WHERE uid = '{0}' AND videoid = '{1}'",deleteComment.CommentID, deleteComment.VideoID );
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) =  database.RunQuery(query);
            if (reader != null)
            {
                if (reader.RecordsAffected == 0)
                {
                    ack.Status = "FAIL";
                    ack.Reason = "Failed to delete comment.";
                }
                Connection.Close();
                return resp;
            }
            else
            {
                Connection.Close();
                return new Response() { Type = ResponseType.Acknowledge, Status = "FAIL" };
            }
                
        }

        private static Response UsernameAvailbilityResponse(AvailUsername username)
        {
            Response resp = new Response()
            {
                Type = ResponseType.Acknowledge,
                Status = "OK",
                Content = new Acknowledge()
                {
                    Status = "OK",
                    Reason = IsUsernameAvailable(username.Username) ? "Available" : "Username not available."
                }

            };
            return resp;
        }

        private static Response PostCommentResponse(PostComment postComment)
        {
            Acknowledge response = new Acknowledge() { Status = "FAIL", Reason = "Query Failed" };
            Response resp = new Response() { Type = ResponseType.Acknowledge, Status = "FAIL", Content = response };
            string uid = Guid.NewGuid().ToString();
            string query = String.Format("INSERT INTO comments(userid, videoid, content, uid) VALUES ('{0}','{1}','{2}','{3}')",postComment.UserID,postComment.VideoID,RefineContent( postComment.Content), uid);
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery(query);
            if (reader != null && reader.RecordsAffected > 0)
            {
                response.Status = "OK";
                response.Reason = uid;
            }
            Connection.Close();
            return resp;
        }

        private static Response AccountDetailResponse(AccountDetails accountDetails)
        {
            Response resp = new Response() { Type = ResponseType.AccountDetails, Status = "OK" };
            string query = "SELECT * FROM account_data WHERE username='"+accountDetails.Username+"'";
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery(query);
            AccountDetailsResponse response = new AccountDetailsResponse();
            if (reader != null)
            {
                reader.Read();

                response.Username = reader.GetString(0);
                response.FullName = reader.GetString(1);
                response.AccType = reader.GetString(2);
                response.Type = int.Parse(reader.GetString(3))==0 ? UserType.Student : UserType.Professional;
                response.ProfileImage = reader.GetString(4);
                resp.Content = response;
            }
            Connection.Close();
            return resp;
        }

        private static Response CreateAccountResponse(CreateAccount createAccount)
        {
            Acknowledge ack = new Acknowledge() { Status = "FAIL", Reason = "Username not available."} ;
            Response resp = new Response()
            {
                Content = ack,
                Type = ResponseType.Acknowledge,
                Status = "FAIL"
            };

            if (!IsUsernameAvailable(createAccount.Username))
                return resp;
            DatabaseManager database = new DatabaseManager();
            string query = String.Format("INSERT INTO user_login(username, password) VALUES ('{0}', '{1}')", RefineContent(createAccount.Username), RefineContent(createAccount.Password));
            (MySqlDataReader reader, var Connection) = database.RunQuery(query);
            if(reader != null && reader.RecordsAffected>0 )
            {
                query = String.Format("INSERT INTO account_data (username, fullname, acctype, type) VALUES ('{0}', '{1}', '{2}', '{3}')", createAccount.Username, createAccount.FullName, createAccount.Type.ToString(), (int)createAccount.Type);
                ( reader, Connection) = database.RunQuery(query);
                if(reader != null && reader.RecordsAffected > 0)
                {
                    ack.Reason = "";
                    resp.Status = "OK";
                    ack.Status = "OK";
                }
                else
                {
                    ack.Reason = "Account Creation Error.";
                }
            }
            else
                ack.Reason = "Account Creation Error.";
            Connection.Close();
            return resp;
        }

        private static Response VideoDetailsResponse(VideoDetails videoDetails)
        {
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery($"SELECT video_details.videoid, video_details.path, video_details.thumbnail, video_details.title, video_details.description," +
                $" video_details.width, video_details.height, video_details.duration, video_details.authorid, account_data.fullname, account_data.profileimage, course_details.coursename, video_details.courseid, " +
                $"video_details.lastupdatedon, video_details.likes, video_details.dislikes, course_details.rating FROM video_details INNER JOIN course_details INNER JOIN account_data WHERE videoid='{videoDetails.VideoID}' " +
                $"AND account_data.username=video_details.authorid AND course_details.courseid=video_details.courseid;");
            if (reader != null)
            {
                Data.HomePageResponse res = new Data.HomePageResponse();
                res.VideoList = new List<VideoMini>();
                reader.Read();
                Video video = new Video()
                {
                    VideoID = reader.GetString(0),
                    Path = reader.GetString(1),
                    Thumbnail = reader.GetString(2),
                    Title = reader.GetString(3),
                    Description = reader.GetString(4),
                    Width = int.Parse(reader.GetString(5)),
                    Height = int.Parse(reader.GetString(6)),
                    Duration = TimeSpan.Parse(reader.GetString(7)),
                    AuthorId = reader.GetString(8),
                    AuthorName = reader.GetString(9),
                    AuthorImage = reader.GetString(10),
                    Course = reader.GetString(11),
                    CourseID = reader.GetString(12),
                    LastUpdatedOn = DateTime.Parse(reader.GetString(13)),
                    Likes = int.Parse(reader.GetString(14)),
                    Dislikes = int.Parse(reader.GetString(15)),
                    CommentList = new List<Comment>(),
                    Tags = new List<string>(),
                    CourseRating = float.Parse(reader.GetString(16)),
                };
                Connection.Close();
                ( reader, Connection) = database.RunQuery($"SELECT * FROM favorites WHERE userid='{videoDetails.UserID}' AND videoid='{videoDetails.VideoID}'");
                if (reader != null && reader.HasRows)
                    video.IsFavorite = true;
                Connection.Close();

                (reader, Connection) = database.RunQuery($"SELECT comments.userid, comments.videoid, account_data.fullname, account_data.profileimage, comments.content, comments.likes, comments.dislikes, comments.date, comments.uid FROM comments INNER JOIN account_data WHERE videoid='{video.VideoID}' AND account_data.username=comments.userid ORDER BY date DESC ");

                if (reader!=null)
                {
                    while (reader.Read())
                    {
                        video.CommentList.Add(new Comment()
                        {
                            UserId = reader.GetString(0),
                            VideoID = reader.GetString(1),
                            Username = reader.GetString(2),
                            Thumbnail = reader.GetString(3),
                            Content = reader.GetString(4),
                            Likes = int.Parse(reader.GetString(5)),
                            Dislikes = int.Parse(reader.GetString(6)),
                            Date = DateTime.Parse(reader.GetString(7)),
                            Uid = reader.GetString(8)                            
                        }
                        );
                    }
                }

                Connection.Close();
                ( reader,Connection) = database.RunQuery($"SELECT tag from tag_details WHERE videoid = '{videoDetails.VideoID}'");
                if(reader!=null)
                {
                    while(reader.Read())
                    {
                        video.Tags.Add(reader.GetString(0));
                    }
                }

                Connection.Close();
                return new Response() { Type = ResponseType.VideoDetails, Content = video, Status = "OK" };
            }
            Connection.Close();
            return new Response() { Type = ResponseType.VideoDetails, Content = null, Status = "FAIL" };
        }

        private static Response SignInResponse(SignIn SignInRequest)
        {

            //request SQL DataBase
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery("SELECT * FROM user_login WHERE username='" + SignInRequest.Username + "'");
            Data.SignInResponse res = new Data.SignInResponse() { IsAccepted = false, Reason = "Invalid Username Or Password.", SessionID = "" };
            if (reader!=null && reader.HasRows)
            {
                var a = reader.HasRows;
                reader.Read();
                if (reader.GetString(1) == SignInRequest.Password)
                {
                    string authKey = Guid.NewGuid().ToString();
                    database.RunQuery("UPDATE user_login SET authkey ='" + authKey + "' WHERE  username  = '" + SignInRequest.Username + "'");
                    res = new Data.SignInResponse()
                    {
                        IsAccepted = true,
                        Reason = "",
                        SessionID = authKey
                    };
                }
                   
            }

            Connection.Close();
            Response response = new Response() { Type = ResponseType.SignIn, Content = res };
            return response;
        }

        private static Response HomeResponse(HomeRequest HomeRequest)
        {
            //request SQL DataBase
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader,var Connection) = database.RunQuery("SELECT duration, thumbnail, title, videoid FROM video_details ORDER BY lastupdatedon DESC LIMIT 20");
            if(reader!=null)
            {
                Data.HomePageResponse res = new Data.HomePageResponse();
                res.VideoList = new List<VideoMini>();
                while(reader.Read())
                {
                    res.VideoList.Add(new VideoMini()
                        {
                        Duration = reader.GetString(0) ,
                        Thumbnail = reader.GetString(1),
                        Title = reader.GetString(2),
                        VideoId = reader.GetString(3)
                        });
                }
                Connection.Close();
                return new Response() { Type = ResponseType.Home, Content = res, Status = "OK" };
            }
            Connection.Close();
            return new Response() { Content = null, Type = ResponseType.Home, Status = "FAIL" };
        }

        // NON response method
        private static string RefineContent(string content)
        {
            content = content.Replace("\"", "\\\"");
            content = content.Replace(@"\", @"\\");
            content = content.Replace(@"'", @"\'");
            content = content.Replace(@"%", @"\%");
            content = content.Replace(@"_", @"\_");
            return content;
        }

        private static bool IsUsernameAvailable(string username)
        {
            string query = String.Format("SELECT username FROM user_login WHERE username='{0}'",RefineContent(username));
            DatabaseManager database = new DatabaseManager();
            (MySqlDataReader reader, var Connection) = database.RunQuery(query);
            if (reader != null && reader.HasRows)
            {
                Connection.Close();
                return false;

            }
            Connection.Close();
            return true;
        }
    }
}
