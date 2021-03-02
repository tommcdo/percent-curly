using System;
using System.Collections.Generic;
using PercentCurly.Templating.Case;

namespace PercentCurly.Templating
{
    public class PlaceholderName
    {
        private readonly Word[] words;

        public PlaceholderName(Word[] words)
        {
            this.words = words;
        }

        public override bool Equals(object obj)
        {
            return obj is PlaceholderName placeholderName &&
                EqualityComparer<Word[]>.Default.Equals(words, placeholderName.words);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(words);
        }
    }
}
