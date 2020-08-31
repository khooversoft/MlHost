using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MlHostWeb.Server.Services;
using MlHostWeb.Shared;

namespace MlHostWeb.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Config : ControllerBase
    {
        private readonly IContentService _contentService;

        public Config(IContentService contentService)
        {
            _contentService = contentService;
        }

        [HttpGet]
        public Configuration Get()
        {
            return new Configuration
            {
                Models = new[]
                {
                    new ModelItem
                    {
                        Name = "Intent",
                        VersionId = "intent-v1",
                        ModelUrl = "http://localhost:5000/api/question",
                        SwaggerUrl = "http://localhost:5000/swagger",
                        DocId = "Intent.md",
                    },
                    new ModelItem
                    {
                        Name = "Emotion",
                        VersionId = "emotion-v2",
                        ModelUrl = "http://localhost:5000/api/question",
                        SwaggerUrl = "http://localhost:5000/swagger",
                        DocId = "Emotion.md",
                    },
                    new ModelItem
                    {
                        Name = "IdCard",
                        VersionId = "idcard-v1",
                        ModelUrl = "http://localhost:5000/api/question",
                        SwaggerUrl = "http://localhost:5000/swagger",
                        DocId = "IdCard.md",
                    },
                    new ModelItem
                    {
                        Name = "Sentiment",
                        VersionId = "sentiment-v2",
                        ModelUrl = "http://localhost:5000/api/question",
                        SwaggerUrl = "http://localhost:5000/swagger",
                        DocId = "Sentiment.md",
                    },
                }
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
