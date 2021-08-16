using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public readonly struct TaggedText
{
    public string RawText { get; }
    public string Text { get; }
    public string[] Tags { get; }
    public bool HasTags { get; }

    public TaggedText(string rawText, string text, string[] tags = null)
    {
        RawText = rawText;
        Text = text;
        Tags = tags;
        HasTags = Tags != null && Tags.Length > 0;
    }
}

public class DialogueText
{
    public string RawText { get; }
    public IReadOnlyCollection<TaggedText> TextFragments { get; }
    
    public DialogueText(string targetText)
    {
        TextFragments = Parse(targetText);
        RawText = string.Join("", TextFragments.Select(t => t.Text));
    }

    private List<TaggedText> Parse(string text)
    {
        var list = new List<TaggedText>();

        var matches = Regex.Matches(text, 
            @"(?:<(?'startTag'[a-z]+)>)+(?'text'.*?)(?:<\/(?'endTag'[a-z]+)>)+",
            RegexOptions.IgnoreCase);

        if (matches.Count == 0)
        {
            list.Add(new TaggedText(text, text));
            return list;
        }

        var i = 0;
        
        foreach (Match match in matches)
        {
            var textWithoutTags = text.Substring(i, match.Index - i);
            i = match.Index + match.Length;

            if (textWithoutTags.Length > 0) list.Add(new TaggedText(match.Value, textWithoutTags));

            var startTagGroup = match.Groups["startTag"];
            var endTagGroup = match.Groups["endTag"];
            var textGroup = match.Groups["text"];

            if (startTagGroup != null && endTagGroup != null)
            {
                var startTags = (from Capture capture in startTagGroup.Captures select capture.Value).ToArray();
                var endTags = (from Capture capture in endTagGroup.Captures select capture.Value).ToArray();
                var textInTags = textGroup.Captures[0].Value;

                var commonTags = startTags.Intersect(endTags).ToArray();
                
                if (commonTags.Any() && startTags.Length > endTags.Length) 
                    list.Add(new TaggedText(match.Value, textInTags, commonTags));
                else if (startTags.SequenceEqual(endTags.Reverse())) 
                    list.Add(new TaggedText(match.Value, textInTags, startTags));
            }
        }

        if (i < text.Length - 1)
        {
            var lastText = text.Substring(i, text.Length - i);
            list.Add(new TaggedText(lastText, lastText));
        }

        return list;
    }
}