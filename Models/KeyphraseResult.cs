using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiFaqBot01.Models
{
    public class KeyphraseResult
    {
        public Document[] Documents { get; set; }
        public object[] Errors { get; set; }
    }

    public class Document
    {
        public string[] KeyPhrases { get; set; }
        public string Id { get; set; }
    }

}