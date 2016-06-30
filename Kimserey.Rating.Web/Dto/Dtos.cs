using Kimserey.Rating.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimserey.Rating.Web.Dto
{

    public class PhotoDto
    {
        public string Small { get; set; }
        public string Medium { get; set; }
        public string Large { get; set; }
        public string Relative { get; set; }
    }

    public class ProfileSummaryDto
    {
        public UserSettings Settings { get; set; }
        public bool HasNewMessages { get; set; }
        public UserViewDto User { get; set; }
    }

    public class UserSettings
    {
        public bool ShowWelcome { get; set; }
    }

    public class UserFullViewDto : UserViewDto
    {
        public string Description { get; set; }
        public string Location { get; set; }
        public List<PhotoDto> AlbumPhotos { get; set; }
        public bool CanEdit { get; set; }
    }

    public class UserViewDto : UserNameDto
    {
        public PhotoDto ProfilePhoto { get; set; }
    }

    public class UserNameDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public Gender Gender { get; set; }
        public Gender Interest { get; set; }
        public DateTime Birthday { get; set; }
        public Boolean IsOnline { get; set; }
    }

    public class VoteListDto
    {
        public Boolean CanVote { get; set; }
        public List<VoteDto> Votes { get; set; }
    }

    public class VoteDto
    {
        public Guid VoteId { get; set; }
        public int Rate { get; set; }
        public string Comment { get; set; }
        public DateTime RatedOn { get; set; }
        public UserViewDto VotedByUser { get; set; }
    }

    public class ConversationDto
    {
        public string Title { get; set; }
        public Guid ConversationId { get; set; }
        public List<UserViewDto> Users { get; set; }
        public List<MessageDto> Messages { get; set; }
        public bool IsInterlocutorOnline { get; set; }
        public Guid InterlocutorId { get; set; }
    }

    public class ConversationPreviewDto
    {
        public Guid ConversationId { get; set; }
        public UserViewDto User { get; set; }
        public MessageDto Message { get; set; }
        public bool HasNewMessages { get; set; }
    }

    public class MessageDto
    {
        public Guid SentByUserId { get; set; }
        public string Text { get; set; }
        public DateTime SentOn { get; set; }
        public bool IsSentByUser { get; set; }
    }

    public class TopsDto
    {
        public List<UserRankDto> MostVotedMan { get; set; }
        public List<UserRankDto> MostViewedMan { get; set; }
        public List<UserRankDto> MessageAddictsMan { get; set; }
        public List<UserRankDto> VoteAddictsMan { get; set; }
        public List<UserRankDto> MostVotedLady { get; set; }
        public List<UserRankDto> MostViewedLady { get; set; }
        public List<UserRankDto> MessageAddictsLady { get; set; }
        public List<UserRankDto> VoteAddictsLady { get; set; }
    }

    public class UserRankDto
    {
        public UserViewDto User { get; set; }
        public string Value { get; set; }
    }
}
