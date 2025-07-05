using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;
using MyApp.Core.Entities;

namespace MyApp.Application.CQRS.Notifications.GetNotificationByUserId.Queries
{
    public class GetNotificationsByUserIdRequest : IRequest<GetNotificationsByUserIdResponse>
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageIndex phải >= 1")]
        public int PageIndex { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize phải trong khoảng từ 1 đến 100")]
        public int PageSize { get; set; } = 10;
    }
}
