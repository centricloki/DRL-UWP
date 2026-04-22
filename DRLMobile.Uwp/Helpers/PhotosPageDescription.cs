using System;
using Windows.Foundation;

namespace DRLMobile.Uwp.Helpers
{
    public class PhotosPageDescription : IEquatable<PhotosPageDescription>
    {
        public Size Margin;
        public Size PageSize;
        public Size ViewablePageSize;
        public Size PictureViewSize;
        public bool IsContentCropped;

        public bool Equals(PhotosPageDescription other)
        {
            bool equal = (Math.Abs(PageSize.Width - other.PageSize.Width) < double.Epsilon) &&
                (Math.Abs(PageSize.Height - other.PageSize.Height) < double.Epsilon);

            if (equal)
            {
                equal = (Math.Abs(ViewablePageSize.Width - other.ViewablePageSize.Width) < double.Epsilon) &&
                    (Math.Abs(ViewablePageSize.Height - other.ViewablePageSize.Height) < double.Epsilon);
            }

            if (equal)
            {
                equal = (Math.Abs(PictureViewSize.Width - other.PictureViewSize.Width) < double.Epsilon) &&
                    (Math.Abs(PictureViewSize.Height - other.PictureViewSize.Height) < double.Epsilon);
            }

            if (equal)
            {
                equal = IsContentCropped == other.IsContentCropped;
            }

            return equal;
        }
    }
}
