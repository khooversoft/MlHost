using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MlHostWeb.Client.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MlHostWeb.Client.Pages
{
    public partial class Model : ComponentBase
    {
        private string _versionId;

        [Inject]
        public IContentService ContentService { get; set; }

        [Inject]
        public IModelService ModelService { get; set; }

        public ModelItem ModelItem { get; private set; }

        public ModelQuestion ModelInput { get; set; } = new ModelQuestion();

        [Parameter]
        public string VersionId
        {
            get { return _versionId; }
            set
            {
                _versionId = value;
                ModelItem = ModelService.GetModel(value);
            }
        }

        protected Task SubmitForm()
        {
            Console.WriteLine($"Question: {ModelInput.Question}*");
            return Task.FromResult(0);
        }
    }

    public class ModelQuestion
    {
        [Required]
        public string Question { get; set; }
        public string Result { get; set; }
    }
}
