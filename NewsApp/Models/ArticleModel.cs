using System;
using System.ComponentModel.DataAnnotations;

namespace NewsApp.Models
{
    public class Article
    {
        public string Id { get; set; }

        [UIHint("Photo")]
        public string Photo { get; set; }

        public string Title { get; set; }

        public string Review { get; set; }

        public string Text { get; set; }

        public DateTime Date { get; set; }
    }
}