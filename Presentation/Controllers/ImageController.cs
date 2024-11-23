using Application.Groups;
using Application.Image;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : BaseController
    {

        public ImageController(IMediator mediator) : base(mediator)
        {
            
        }

        [HttpPost(nameof(Upload))]
        public async Task<ActionResult<UploadResult>> Upload([FromForm] UploadCommnad request)
        {
            return await mediator.Send(request);
        }
    }
}
