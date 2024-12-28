using Amazon.Runtime.Internal;
using Application.Common.Models;
using DataAccess;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Events;

public class GetMostUsedTagsListQuery:IRequest<GetMostUsedTagsListResult>
{
}


public class GetMostUsedTagsListResult:ResultModel
{
    public List<string> Tags { get; set; }
}

public class GetMostUsedTagsListHandler : IRequestHandler<GetMostUsedTagsListQuery, GetMostUsedTagsListResult>
{
    private ApplicationDBContext dBContext;

    public GetMostUsedTagsListHandler(ApplicationDBContext dBContext)
    {
        this.dBContext = dBContext;
    }
    public async Task<GetMostUsedTagsListResult> Handle(GetMostUsedTagsListQuery request, CancellationToken cancellationToken)
    {
        var result = new GetMostUsedTagsListResult();

        result.Tags = await dBContext.EventTags
          .GroupBy(item => item.Tag)
          .OrderByDescending(group => group.Count())
          .Select(group => group.Key).Take(10).ToListAsync();

        result.Success = true;

        return result;
    }
}