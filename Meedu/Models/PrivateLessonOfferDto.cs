﻿using Meedu.Entities;

namespace Meedu.Models
{
    public class PrivateLessonOfferDto
    {
        public string LessonTitle { get; set; }
        public string City { get; set; }
        public decimal Price { get; set; }
        public bool isOnline { get; set; }
        public Place Place { get; set; }
        public SubjectDto Subject { get; set; }
        public string Description { get; set; }
        public TeachingRange TeachingRange { get; set; }
    }
}
