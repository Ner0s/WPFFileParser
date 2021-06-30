namespace System.Windows
{
    internal class Forms
    {
        public static object DialogResult { get; internal set; }

        internal class OpenFileDialog
        {
            public OpenFileDialog()
            {
            }

            public object FileName { get; internal set; }

            internal object ShowDialog()
            {
                throw new NotImplementedException();
            }
        }
    }
}