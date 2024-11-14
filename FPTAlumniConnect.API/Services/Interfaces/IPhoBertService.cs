
using FPTAlumniConnect.BusinessTier.Payload;

namespace FPTAlumniConnect.API.Services.Interfaces
{
    public interface IPhoBertService
    {
        Task<int?> FindBestMatchingCV(EmbeddingRequest jobDescription);
        Task<double[]> GenerateEmbedding(EmbeddingRequest text);
    }
}
