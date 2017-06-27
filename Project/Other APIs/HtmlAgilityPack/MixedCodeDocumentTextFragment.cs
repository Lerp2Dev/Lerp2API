namespace HtmlAgilityPack
{
    /// <summary>
    /// Class MixedCodeDocumentTextFragment.
    /// </summary>
    /// <seealso cref="HtmlAgilityPack.MixedCodeDocumentFragment" />
    public class MixedCodeDocumentTextFragment : MixedCodeDocumentFragment
    {
        internal MixedCodeDocumentTextFragment(MixedCodeDocument doc) : base(doc, MixedCodeDocumentFragmentType.Text)
        {
        }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get
            {
                return base.FragmentText;
            }
            set
            {
                base.FragmentText = value;
            }
        }
    }
}