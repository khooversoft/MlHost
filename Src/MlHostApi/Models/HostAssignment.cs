using MlHostApi.Tools;

namespace MlHostApi.Models
{
    public class HostAssignment
    {
        public string? HostName { get; set; }

        public string? ModelId { get; set; }

        public void Verify()
        {
            HostName.VerifyNotEmpty(nameof(HostName));
            ModelId.VerifyNotEmpty(nameof(ModelId));
        }
    }
}
