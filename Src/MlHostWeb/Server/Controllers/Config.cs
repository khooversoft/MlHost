using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MlHostWeb.Server.Application;
using MlHostWeb.Server.Services;
using MlHostWeb.Shared;

namespace MlHostWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Config : ControllerBase
    {
        private readonly IOption _option;
        private readonly IContentService _contentService;

        public Config(IOption option, IContentService contentService)
        {
            _option = option;
            _contentService = contentService;
        }

        [HttpGet]
        public Configuration Get()
        {
            return new Configuration
            {
                FrontEndUrl = _option.FrontEndUrl,
                Models = _option.Models.ToList(),
            };
        }

        [HttpGet("doc/{docId}")]
        public DocItem GetDoc(string docId)
        {
            return new DocItem
            {
                DocId = docId,
                Html = _contentService.GetDocHtml(docId),
            };
        }
    }
}
