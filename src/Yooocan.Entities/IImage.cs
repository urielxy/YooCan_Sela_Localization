using System;
using Yooocan.Enums;

namespace Yooocan.Entities
{
    interface IImage
    {
        int Id { get; set; }
        string Url { get; set; }
        int Order { get; set; }       
        ImageType Type { get; set; }
        bool IsDeleted { get; set; }
        DateTime InsertDate { get; set; }
    }
}
